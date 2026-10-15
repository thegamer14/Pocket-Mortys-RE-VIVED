using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WorldInputDPadUI : MonoBehaviour
{
	public delegate void WorldInputDPadDelegate(eDirection direction);

	private const float _kTouchDistance = 200f;

	private const int _kOutPositionY = -190;

	private const float _kInOutDuration = 0.3f;

	public Sprite dPadOff;

	public Sprite dPadUp;

	public Sprite dPadDown;

	public Sprite dPadRight;

	public Sprite dPadLeft;

	private RectTransform _rectTrans;

	private Image _sprite;

	private float _startingPositionY;

	private bool _enabled;

	private Camera _uiCamera;

	public eDirection direction { get; private set; }

	public event WorldInputDPadDelegate DirectionChangedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_sprite = base.transform.FindChild("Sprite").GetComponent<Image>();
		_startingPositionY = base.transform.localPosition.y;
		_enabled = false;
		_uiCamera = GameObject.FindWithTag("CanvasCamera").GetComponent<Camera>();
	}

	private void Update()
	{
		if (!_enabled)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		if (Input.GetMouseButton(0))
		{
			zero = Input.mousePosition;
		}
		else
		{
			if (Input.touchCount <= 0)
			{
				if (Input.GetKey(KeyCode.RightArrow))
				{
					SetDirection(eDirection.RIGHT);
				}
				else if (Input.GetKey(KeyCode.LeftArrow))
				{
					SetDirection(eDirection.LEFT);
				}
				else if (Input.GetKey(KeyCode.UpArrow))
				{
					SetDirection(eDirection.UP);
				}
				else if (Input.GetKey(KeyCode.DownArrow))
				{
					SetDirection(eDirection.DOWN);
				}
				else
				{
					SetDirection(eDirection.NONE);
				}
				return;
			}
			zero = Input.touches[0].position;
		}
		Vector2 vector = base.transform.InverseTransformPoint(_uiCamera.ScreenToWorldPoint(zero));
		if (vector.magnitude <= 200f)
		{
			float angle = Mathf.Atan2(0f - vector.y, vector.x) * 57.29578f;
			SetDirection(HelperMethods.GetDirectionFromAngle(angle));
		}
	}

	public void SetDirection(eDirection newDirection)
	{
		if (direction != newDirection)
		{
			switch (newDirection)
			{
			default:
				_sprite.sprite = dPadOff;
				break;
			case eDirection.UP:
				_sprite.sprite = dPadUp;
				break;
			case eDirection.DOWN:
				_sprite.sprite = dPadDown;
				break;
			case eDirection.RIGHT:
				_sprite.sprite = dPadRight;
				break;
			case eDirection.LEFT:
				_sprite.sprite = dPadLeft;
				break;
			}
			direction = newDirection;
			if (this.DirectionChangedEvent != null)
			{
				this.DirectionChangedEvent(direction);
			}
		}
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY);
		_enabled = true;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, -190f);
		_enabled = false;
	}

	public IEnumerator TweenIn()
	{
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, _startingPositionY), 0.3f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
		_enabled = true;
	}

	public IEnumerator TweenOut()
	{
		_enabled = false;
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, -190f), 0.3f);
		tween.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();
	}
}
