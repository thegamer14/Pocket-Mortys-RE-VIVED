using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBtnBase : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void MenuBtnBaseDelegate(MenuBtnBase btn);

	public float longPressThreshold;

	public Sprite unavailableSprite;

	protected Button _btn;

	protected Text _titleLabel;

	protected Sprite _availableSprite;

	protected bool _isEnabled;

	private bool isPointerDown;

	private bool longPressTriggered;

	private float timePressStarted;

	public event MenuBtnBaseDelegate PressedEvent;

	public event MenuBtnBaseDelegate LongPressStartEvent;

	public event MenuBtnBaseDelegate LongPressEndEvent;

	protected override void Awake()
	{
		base.Awake();
		_btn = GetComponent<Button>();
		_titleLabel = base.transform.GetComponentInChildren<Text>();
		_availableSprite = ((Image)_btn.targetGraphic).sprite;
		_isEnabled = true;
	}

	private void Update()
	{
		if (longPressThreshold > 0f && isPointerDown && !longPressTriggered && Time.time - timePressStarted > longPressThreshold)
		{
			longPressTriggered = true;
			LongPressStarted();
		}
	}

	public virtual void SetEnabled(bool isEnabled)
	{
		_isEnabled = isEnabled;
		((Image)_btn.targetGraphic).sprite = ((!_isEnabled) ? unavailableSprite : _availableSprite);
		_btn.interactable = _isEnabled;
	}

	public void SetTitle(string title)
	{
		if (_titleLabel != null)
		{
			_titleLabel.text = title;
		}
	}

	protected virtual void Press()
	{
		if (this.PressedEvent != null)
		{
			this.PressedEvent(this);
		}
	}

	protected virtual void LongPressStarted()
	{
		if (this.LongPressStartEvent != null)
		{
			this.LongPressStartEvent(this);
		}
	}

	protected virtual void LongPressEnded()
	{
		if (this.LongPressEndEvent != null)
		{
			this.LongPressEndEvent(this);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (_btn.IsInteractable())
		{
			timePressStarted = Time.time;
			isPointerDown = true;
			longPressTriggered = false;
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (!_btn.IsInteractable())
		{
			isPointerDown = false;
			longPressTriggered = false;
			return;
		}
		if (longPressTriggered)
		{
			LongPressEnded();
		}
		else if (isPointerDown)
		{
			Press();
		}
		isPointerDown = false;
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		isPointerDown = false;
	}
}
