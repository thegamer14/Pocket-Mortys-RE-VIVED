using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleAttackBtn : MenuBtnBase
{
	private const float _kPopupPositionBtn0X = 25f;

	private const float _kPopupPositionBtn1X = 0f;

	private const float _kPopupPositionBtn2X = -15f;

	public float popupPositionY;

	public Sprite availableBuffUpSprite;

	public Sprite availableBuffDownSprite;

	public Sprite availableDebuffUpSprite;

	public Sprite availableDebuffDownSprite;

	public Sprite availableGoldenUpSprite;

	public Sprite availableGoldenDownSprite;

	private GameObject _ppObj;

	private Text _ppLabel;

	private Transform _elementBar;

	private MortyElementImage _elementImage;

	private Sprite _availableNormalUpSprite;

	private Sprite _availableNormalDownSprite;

	private AttackStatPopup _statPopup;

	private int _btnIndex = -1;

	private AttackDisplayInfo _displayInfo;

	protected override void Awake()
	{
		base.Awake();
		_ppObj = base.transform.FindChild("PPBar").gameObject;
		_ppLabel = _ppObj.transform.FindChild("Label").GetComponent<Text>();
		_elementBar = base.transform.FindChild("ElementBar");
		if (_elementBar != null)
		{
			_elementImage = _elementBar.GetComponentInChildren<MortyElementImage>();
		}
		_availableNormalUpSprite = _availableSprite;
		_availableNormalDownSprite = _btn.spriteState.pressedSprite;
	}

	public void SetUp(int btnIndex, AttackDisplayInfo displayInfo, bool showRemainingPP = true)
	{
		_btnIndex = btnIndex;
		_displayInfo = displayInfo;
		SetTitle(displayInfo.name);
		RefreshTargetSprite();
		if (showRemainingPP)
		{
			_ppLabel.text = string.Format("{0} {1}/{2}", TextDefs.MORTY_STAT_PP, _displayInfo.remainingPP, _displayInfo.maxPP);
		}
		else
		{
			_ppLabel.text = string.Format("{0}: {1}", TextDefs.MORTY_STAT_PP, _displayInfo.maxPP);
		}
		if (_elementImage != null)
		{
			if (_elementImage.Set(_displayInfo.element))
			{
				_elementBar.gameObject.SetActive(true);
			}
			else
			{
				_elementBar.gameObject.SetActive(false);
			}
		}
	}

	public override void SetEnabled(bool isEnabled)
	{
		base.SetEnabled(isEnabled);
		_ppObj.SetActive(isEnabled);
		_titleLabel.gameObject.SetActive(isEnabled);
		if (_elementBar != null)
		{
			_elementBar.gameObject.SetActive(isEnabled);
		}
	}

	private void RefreshTargetSprite()
	{
		Sprite availableNormalDownSprite;
		if (_displayInfo.catagory == eAttackCatagory.Golden)
		{
			_availableSprite = availableGoldenUpSprite;
			availableNormalDownSprite = availableGoldenDownSprite;
		}
		else if (_displayInfo.catagory == eAttackCatagory.Buff)
		{
			_availableSprite = availableBuffUpSprite;
			availableNormalDownSprite = availableBuffDownSprite;
		}
		else if (_displayInfo.catagory == eAttackCatagory.Debuff)
		{
			_availableSprite = availableDebuffUpSprite;
			availableNormalDownSprite = availableDebuffDownSprite;
		}
		else
		{
			_availableSprite = _availableNormalUpSprite;
			availableNormalDownSprite = _availableNormalDownSprite;
		}
		if (_isEnabled)
		{
			((Image)_btn.targetGraphic).sprite = _availableSprite;
			SpriteState spriteState = new SpriteState
			{
				pressedSprite = availableNormalDownSprite,
				highlightedSprite = _btn.spriteState.highlightedSprite,
				disabledSprite = _btn.spriteState.disabledSprite
			};
			_btn.spriteState = spriteState;
		}
	}

	protected override void Press()
	{
		base.Press();
	}

	protected override void LongPressStarted()
	{
		base.LongPressStarted();
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/AttackStatPopup/AttackStatPopup{0}", _displayInfo.catagory.ToString()), base.transform.parent);
		switch (_btnIndex)
		{
		case 0:
			rectTransform.localPosition = base.transform.localPosition + new Vector3(25f, popupPositionY);
			break;
		case 1:
			rectTransform.localPosition = base.transform.localPosition + new Vector3(0f, popupPositionY);
			break;
		case 2:
		case 3:
			rectTransform.localPosition = base.transform.localPosition + new Vector3(-15f, popupPositionY);
			break;
		}
		_statPopup = rectTransform.GetComponent<AttackStatPopup>();
		_statPopup.SetUp(_displayInfo);
		_statPopup.TweenIn();
	}

	protected override void LongPressEnded()
	{
		base.LongPressEnded();
		if (_statPopup != null)
		{
			_statPopup.TweenOut(true);
			_statPopup = null;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (_statPopup != null)
		{
			_statPopup.TweenOut(true);
			_statPopup = null;
		}
	}
}
