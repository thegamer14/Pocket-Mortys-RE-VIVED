using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	public delegate void SfxVolumeUpdatedDelegate(float newValue);

	public delegate void SoundTrackDelegegate();

	private enum Fade
	{
		In = 0,
		Out = 1
	}

	private const float _kSystemMusicCheckTime = 3f;

	private AudioSource _soundtrackAudioSrc;

	protected const float _kFadeStepDuration = 0.05f;

	private List<string> _soundtrackQueue = new List<string>();

	private bool _bSoundtrackPaused;

	private float _pitchOverride = 1f;

	private string[] _ignorAudioArray;

	private float _musicVolume = 1f;

	private float _currentTrackVolume;

	private float _sfxVolume = 1f;

	private GameObject _container;

	private string tagST = "AudioST";

	private string tagSFX = "AudioSFX";

	private Dictionary<string, AudioClip> _audioClips;

	private Dictionary<string, AudioSource> _audioSources;

	private Dictionary<string[], string> _lastRandomPlayedSFX;

	private bool _systemMusicPlaying;

	public float musicVolume
	{
		get
		{
			return _musicVolume;
		}
		set
		{
			_musicVolume = value;
			if (_soundtrackAudioSrc != null)
			{
				_soundtrackAudioSrc.volume = _currentTrackVolume * value;
			}
		}
	}

	public float sfxVolume
	{
		get
		{
			return _sfxVolume;
		}
		set
		{
			_sfxVolume = value;
			if (this.SfxVolumeUpdatedEvent != null)
			{
				this.SfxVolumeUpdatedEvent(_sfxVolume);
			}
		}
	}

	public bool isSoundtrackPlaying
	{
		get
		{
			if (_soundtrackAudioSrc == null)
			{
				return false;
			}
			if (_bSoundtrackPaused)
			{
				return false;
			}
			return _soundtrackAudioSrc.isPlaying;
		}
	}

	public event SfxVolumeUpdatedDelegate SfxVolumeUpdatedEvent;

	public event SoundTrackDelegegate SoundTrackFinishedEvent;

	private void Awake()
	{
		base.gameObject.AddComponent<AudioListener>();
		_audioClips = new Dictionary<string, AudioClip>();
		_audioSources = new Dictionary<string, AudioSource>();
		_lastRandomPlayedSFX = new Dictionary<string[], string>();
		_container = new GameObject("AudioContainer");
		_container.transform.parent = base.transform;
	}

	private void Start()
	{
		InvokeRepeating("CheckSystemMusicPlaying", 3f, 3f);
	}

	private void OnDestroy()
	{
		CancelInvoke("CheckSystemMusicPlaying");
	}

	private void Update()
	{
		if (_soundtrackAudioSrc == null)
		{
			return;
		}
		if (!_soundtrackAudioSrc.isPlaying && _soundtrackAudioSrc.clip.loadState == AudioDataLoadState.Loaded && !_bSoundtrackPaused)
		{
			StopSoundtrack();
			if (this.SoundTrackFinishedEvent != null)
			{
				this.SoundTrackFinishedEvent();
			}
		}
		if (_soundtrackQueue.Count > 0 && !_bSoundtrackPaused && !_soundtrackAudioSrc.isPlaying)
		{
			bool flag = false;
			PlaySoundtrack(loop: (_soundtrackQueue.Count <= 1) ? true : false, resourcePath: _soundtrackQueue[0], volume: _soundtrackAudioSrc.volume);
			_soundtrackQueue.Remove(_soundtrackQueue[0]);
		}
	}

	public void QueueSoundtrack(string resourcePath)
	{
		if (_soundtrackAudioSrc == null)
		{
			DebugLog("Play: " + resourcePath + " immediatly");
			PlaySoundtrack(resourcePath, _musicVolume, 0f, false);
		}
		else
		{
			_soundtrackQueue.Add(resourcePath);
		}
	}

	public AudioSource PlaySoundtrack(string resourcePath, float volume = 1f, float fadeInDuration = 0f, bool loop = true, float delay = 0f)
	{
		AudioClip audioClip = Resources.Load<AudioClip>(resourcePath);
		if (audioClip == null){ UnityEngine.Debug.LogWarning("[AudioController] missing AudioClip: " + resourcePath); return null; }
		GameObject gameObject = GameObject.FindWithTag(tagST);
		if (gameObject != null)
		{
			if (gameObject.name == tagST + ": " + audioClip.name)
			{
				return null;
			}
			StopSoundtrack();
		}
		_systemMusicPlaying = IPodHandler.Active;
		if (_systemMusicPlaying)
		{
			return null;
		}
		GameObject gameObject2 = new GameObject(tagST + ": " + audioClip.name);
		gameObject2.transform.parent = _container.transform;
		gameObject2.transform.position = Vector3.zero;
		gameObject2.tag = tagST;
		AudioSource audioSource = gameObject2.AddComponent<AudioSource>();
		audioSource.clip = audioClip;
		audioSource.loop = loop;
		audioSource.volume = volume * musicVolume;
		audioSource.pitch = 1f;
		audioSource.playOnAwake = false;
		_soundtrackAudioSrc = audioSource;
		_currentTrackVolume = volume;
		audioSource.Play();
		if (fadeInDuration > 0f)
		{
			StartCoroutine(FadeAudio(audioSource, fadeInDuration, Fade.In));
		}
		return audioSource;
	}

	public void PlaySoundtrackDelayed(string resourcePath, float delayDuration, float volume = 1f, float fadeInDuration = 0f, bool loop = true)
	{
		StartCoroutine(PlaySoundtrackCoroutine(resourcePath, delayDuration, volume, fadeInDuration, loop));
	}

	protected IEnumerator PlaySoundtrackCoroutine(string resourcePath, float delayDuration, float volume, float fadeInDuration, bool loop)
	{
		yield return new WaitForSeconds(delayDuration);
		PlaySoundtrack(resourcePath, volume, fadeInDuration, loop);
	}

	public void StopSoundtrack(float fadeOutVal = 0f, bool clearQueue = false)
	{
		if (!(_soundtrackAudioSrc == null))
		{
			if (clearQueue)
			{
				_soundtrackQueue.Clear();
			}
			if (fadeOutVal == 0f)
			{
				_soundtrackAudioSrc.Stop();
				Object.Destroy(_soundtrackAudioSrc.gameObject);
				_soundtrackAudioSrc = null;
			}
			else
			{
				StartCoroutine(FadeAudio(_soundtrackAudioSrc, fadeOutVal, Fade.Out));
			}
		}
	}

	public void PauseSoundtrack()
	{
		if (!_systemMusicPlaying)
		{
			_bSoundtrackPaused = true;
			if (_soundtrackAudioSrc != null && _soundtrackAudioSrc.isPlaying)
			{
				_soundtrackAudioSrc.Pause();
			}
		}
	}

	public void UnPauseSoundtrack()
	{
		if (!_systemMusicPlaying)
		{
			if (_soundtrackAudioSrc != null && !_soundtrackAudioSrc.isPlaying && _soundtrackAudioSrc.enabled)
			{
				_soundtrackAudioSrc.Play();
			}
			_bSoundtrackPaused = false;
		}
	}

	public float GetCurrentSountrackTime()
	{
		return (!(_soundtrackAudioSrc != null)) ? 0f : _soundtrackAudioSrc.time;
	}

	public AudioSource PlaySFX(string resourcePath, float volume = 1f, bool loop = false, float pitch = 1f, float fadeInDuration = 0f, float delay = 0f, bool reset = true, string[] interruptGroup = null, float pan = 0f)
	{
		if (interruptGroup != null)
		{
			foreach (string text in interruptGroup)
			{
				if (!(text == resourcePath))
				{
					AudioSource value;
					_audioSources.TryGetValue(text, out value);
					StopSFX(value);
				}
			}
		}
		AudioClip audioClip = null;
		AudioSource audioSource = null;
		if (_audioClips.ContainsKey(resourcePath))
		{
			audioClip = _audioClips[resourcePath];
		}
		else
		{
			audioClip = (AudioClip)Resources.Load(resourcePath);
			if (audioClip == null)
			{
				DebugLog("No SFX found at path: " + resourcePath, true);
				return null;
			}
			_audioClips.Add(resourcePath, audioClip);
		}
		if (!reset)
		{
			audioSource = _container.AddComponent<AudioSource>();
			audioSource.clip = audioClip;
			if (!loop)
			{
				Object.Destroy(audioSource, delay + audioSource.clip.length);
			}
		}
		else if (!_audioSources.ContainsKey(resourcePath))
		{
			audioSource = _container.AddComponent<AudioSource>();
			audioSource.clip = audioClip;
			_audioSources.Add(resourcePath, audioSource);
		}
		else
		{
			audioSource = _audioSources[resourcePath];
		}
		audioSource.loop = loop;
		audioSource.volume = volume * _sfxVolume;
		audioSource.pitch = pitch;
		audioSource.playOnAwake = false;
		audioSource.panStereo = pan;
		if (_pitchOverride != 1f && pitch == 1f)
		{
			if (_ignorAudioArray != null)
			{
				if (!_ignorAudioArray.Any(audioSource.clip.name.Contains))
				{
					audioSource.pitch = _pitchOverride;
				}
			}
			else
			{
				audioSource.pitch = _pitchOverride;
			}
		}
		if (delay > 0f)
		{
			audioSource.PlayDelayed(delay);
		}
		else
		{
			audioSource.Play();
		}
		if (fadeInDuration > 0f)
		{
			StartCoroutine(FadeAudio(audioSource, fadeInDuration, Fade.In));
		}
		DebugLog("Playing SFX: " + audioClip.ToString());
		return audioSource;
	}

	public void StopSFX(AudioSource source, float fadeTime = 0f)
	{
		if (!(source == null))
		{
			if (fadeTime <= 0f)
			{
				source.Stop();
			}
			else
			{
				StartCoroutine(FadeAudio(source, fadeTime, Fade.Out));
			}
		}
	}

	public void StopAllSFX()
	{
		AudioSource[] components = _container.GetComponents<AudioSource>();
		foreach (AudioSource audioSource in components)
		{
			audioSource.Stop();
		}
	}

	public void PlayRndSFX(string[] sfxArray, float volume, bool neverRepeat = true, bool loop = false, float pitch = 1f, float fadeInDuration = 0f, float delay = 0f, bool reset = true, string[] interruptGroup = null, float pan = 0f)
	{
		if (sfxArray == null || sfxArray.Length == 0)
		{
			return;
		}
		string text = null;
		if (neverRepeat)
		{
			if (sfxArray.Length == 1)
			{
				text = sfxArray[0];
			}
			else if (_lastRandomPlayedSFX.ContainsKey(sfxArray))
			{
				List<string> list = sfxArray.ToList();
				list.Remove(_lastRandomPlayedSFX[sfxArray]);
				text = list[Mathf.FloorToInt(Random.Range(0, list.Count))];
				_lastRandomPlayedSFX[sfxArray] = text;
			}
			else
			{
				text = sfxArray[Mathf.FloorToInt(Random.Range(0, sfxArray.Length))];
				_lastRandomPlayedSFX.Add(sfxArray, text);
			}
		}
		else
		{
			text = sfxArray[Mathf.FloorToInt(Random.Range(0, sfxArray.Length))];
		}
		if (!string.IsNullOrEmpty(text))
		{
			PlaySFX(text, volume, loop, pitch, fadeInDuration, delay, reset, interruptGroup, pan);
		}
	}

	public bool IsSFXPlayingFromGroup(string[] sfxGroup)
	{
		foreach (string key in sfxGroup)
		{
			AudioSource value = null;
			_audioSources.TryGetValue(key, out value);
			if (value != null && value.isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	public void PauseSFX(string resourcePath)
	{
		if (_audioSources.ContainsKey(resourcePath))
		{
			AudioSource audioSource = _audioSources[resourcePath];
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
			}
		}
	}

	public void UnPauseSFX(string resourcePath)
	{
		if (_audioSources.ContainsKey(resourcePath))
		{
			AudioSource audioSource = _audioSources[resourcePath];
			if (audioSource.time != 0f && audioSource.volume > 0f)
			{
				audioSource.Play();
			}
		}
	}

	public void UnloadNonPlayingSFX()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AudioSource> audioSource in _audioSources)
		{
			if (!audioSource.Value.isPlaying)
			{
				list.Add(audioSource.Key);
				Object.Destroy(audioSource.Value);
			}
		}
		foreach (string item in list)
		{
			_audioSources.Remove(item);
		}
	}

	public void StopAllAudio()
	{
		StopAllSFX();
		StopSoundtrack();
	}

	public void PauseAllAudio()
	{
		PauseSoundtrack();
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagSFX);
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<AudioSource>().Pause();
		}
	}

	public void UnPauseAllAudio()
	{
		UnPauseSoundtrack();
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagSFX);
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<AudioSource>().Play();
		}
	}

	public void PitchAllAudio(float newPitch)
	{
		_pitchOverride = newPitch;
		AudioSource[] array = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
		AudioSource[] array2 = array;
		foreach (AudioSource audioSource in array2)
		{
			if (_ignorAudioArray != null && _ignorAudioArray.Any(audioSource.clip.name.Contains))
			{
				break;
			}
			audioSource.pitch = _pitchOverride;
		}
	}

	public void SetAudioStringsToIgnorPitch(string[] ignorAudioArray)
	{
		_ignorAudioArray = ignorAudioArray;
	}

	private IEnumerator FadeAudio(AudioSource source, float timer, Fade fadeType, bool deleteObject = false)
	{
		float startVol = ((fadeType != Fade.In) ? source.volume : 0f);
		float endVol = ((fadeType != Fade.In) ? 0f : source.volume);
		float currVol = startVol;
		float numFrames = timer / 0.05f;
		float stepVol = (endVol - startVol) / numFrames;
		bool finished = false;
		while (!finished && !(source == null))
		{
			currVol += stepVol;
			if (fadeType == Fade.In && currVol >= endVol)
			{
				currVol = endVol;
				finished = true;
			}
			else if (fadeType == Fade.Out && currVol <= endVol)
			{
				currVol = endVol;
				finished = true;
			}
			source.volume = currVol;
			if (!finished)
			{
				yield return new WaitForSeconds(0.05f);
			}
		}
		if (source != null && (fadeType == Fade.Out || deleteObject))
		{
			if (source.tag == tagST)
			{
				Object.Destroy(source.gameObject);
			}
			else if (source.tag == tagSFX)
			{
				StopSFX(source);
			}
		}
	}

	public void CheckSystemMusicPlaying()
	{
		if (!_systemMusicPlaying && IPodHandler.Active)
		{
			StopSoundtrack();
		}
		_systemMusicPlaying = IPodHandler.Active;
	}

	public void DebugLog(string str, bool isWarning = false)
	{
		if (TestDefs.DEBUG_AUDIO && !isWarning)
		{
		}
	}
}
