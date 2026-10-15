using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Node3dSound : MonoBehaviour
{
	private AudioSource _audioSource;

	private void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		App.sound.SfxVolumeUpdatedEvent += SfxVolumeUpdatedHandler;
		SfxVolumeUpdatedHandler(App.sound.sfxVolume);
	}

	private void OnDisable()
	{
		App.sound.SfxVolumeUpdatedEvent -= SfxVolumeUpdatedHandler;
	}

	private void SfxVolumeUpdatedHandler(float volume)
	{
		_audioSource.volume = volume;
	}
}
