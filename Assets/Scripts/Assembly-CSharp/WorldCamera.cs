using UnityEngine;

public class WorldCamera : MonoBehaviour
{
	private const float _kDefaultFOV = 18f;

	private Camera _camera;

	private Transform _followTransform;

	private Vector3 _followOffset;

	private bool _isFollowEnabled;

	private bool isLock;

	private Vector3 _lockCameraMin;

	private Vector3 _lockCameraMax;

	private AudioListener _audioListener;

	private AudioListener _appAudioListener;

	private UILayer _uiLayer;

	private void Awake()
	{
		_camera = GetComponent<Camera>();
		SetUpAudioListener();
		_uiLayer = App.states.GetUILayerForState(App.states.Get<WorldState>());
	}

	public void SetUp(Transform followTransform, Vector3 followOffset, Vector3 followRotation, float FOV = 0f)
	{
		_followTransform = followTransform;
		_followOffset = followOffset;
		_isFollowEnabled = true;
		base.transform.eulerAngles = followRotation;
		_camera.fieldOfView = ((!(FOV > 0f)) ? 18f : FOV);
		UpdateCameraPosition();
	}

	private void OnEnable()
	{
		SetAudioListenerEnabled(true);
	}

	private void OnDisable()
	{
		SetAudioListenerEnabled(false);
	}

	public void SetFollowEnabled(bool isEnabled)
	{
		_isFollowEnabled = isEnabled;
	}

	public void SetLocked(Vector3 min, Vector3 max)
	{
		isLock = true;
		_lockCameraMin = min;
		_lockCameraMax = max;
	}

	private void UpdateCameraPosition()
	{
		Vector3 position = _followTransform.position + _followOffset;
		if (isLock)
		{
			if (position.x < _lockCameraMin.x)
			{
				position.x = _lockCameraMin.x;
			}
			if (position.x > _lockCameraMax.x)
			{
				position.x = _lockCameraMax.x;
			}
			if (position.y < _lockCameraMin.y)
			{
				position.y = _lockCameraMin.y;
			}
			if (position.y > _lockCameraMax.y)
			{
				position.y = _lockCameraMax.y;
			}
			if (position.z < _lockCameraMin.z)
			{
				position.z = _lockCameraMin.z;
			}
			if (position.z > _lockCameraMax.z)
			{
				position.z = _lockCameraMax.z;
			}
		}
		base.transform.position = position;
		_audioListener.transform.position = _followTransform.position;
	}

	private void SetUpAudioListener()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("World/Utils/WorldAudioListener"));
		gameObject.transform.parent = base.transform;
		_audioListener = gameObject.GetComponent<AudioListener>();
		_audioListener.enabled = false;
		_appAudioListener = GameObject.Find("App").GetComponent<AudioListener>();
	}

	public void SetAudioListenerEnabled(bool enable)
	{
		if (enable)
		{
			_appAudioListener.enabled = false;
			_audioListener.enabled = true;
			return;
		}
		if (_appAudioListener != null)
		{
			_appAudioListener.enabled = true;
		}
		if (_audioListener != null)
		{
			_audioListener.enabled = false;
		}
		AudioListener.volume = 1f;
	}

	private void LateUpdate()
	{
		if (_followTransform != null && _isFollowEnabled)
		{
			UpdateCameraPosition();
		}
		if (_audioListener.enabled && _uiLayer != null)
		{
			AudioListener.volume = 1f - _uiLayer.transitionAlpha;
		}
	}
}
