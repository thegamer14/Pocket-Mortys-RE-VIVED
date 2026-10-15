using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericPopupUI : UIBase
{
	public delegate void ButtonClickedDelegate(int buttonIndex);

	private const int _kBoxPaddingBottom = -60;

	private const int _kNoButtonsBoxPaddingBottom = 40;

	private Text _textLabel;

	private GameObject _waitingObj;

	private CouponBar _couponBar;

	private GameObject _closeBtn;

	private Transform _buttonsTrans;

	private GameObject _templateButtonObj;

	private List<GameObject> _buttons;

	private VerticalLayoutGroup _boxVLayoutGroup;

	private Vector3 _waitingRotBy = new Vector3(0f, 0f, -400f);

	public event ButtonClickedDelegate ButtonClickedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_textLabel = base.transform.FindChild("Box/Label").GetComponent<Text>();
		_waitingObj = base.transform.FindChild("Box/WaitingImg").gameObject;
		_couponBar = base.transform.FindChild("Box/CouponBar").GetComponent<CouponBar>();
		_buttonsTrans = base.transform.FindChild("Box/Buttons");
		_closeBtn = _buttonsTrans.FindChild("CloseBtn").gameObject;
		_templateButtonObj = _buttonsTrans.FindChild("ButtonTemplate").gameObject;
		_boxVLayoutGroup = base.transform.FindChild("Box").GetComponent<VerticalLayoutGroup>();
		_buttons = new List<GameObject>();
	}

	private void DestroyButtons()
	{
		for (int num = _buttons.Count - 1; num >= 0; num--)
		{
			if (_buttons[num] != null)
			{
				_buttons[num].GetComponent<Button>().onClick.RemoveAllListeners();
				Object.Destroy(_buttons[num]);
			}
		}
		_buttons.Clear();
	}

	public void SetDisplayValues(string text, string[] buttons, bool showCloseBtn = true, bool showWaiting = false, bool showCouponBar = false)
	{
		_textLabel.text = text;
		DestroyButtons();
		_closeBtn.SetActive(showCloseBtn);
		if (buttons != null && buttons.Length > 0)
		{
			_buttonsTrans.gameObject.SetActive(true);
			_templateButtonObj.SetActive(true);
			for (int i = 0; i < buttons.Length; i++)
			{
				GameObject newBtn = UI.Instantiate(_templateButtonObj, _buttonsTrans, eUISibling.FIRST).gameObject;
				newBtn.GetComponentInChildren<Text>().text = buttons[i];
				newBtn.GetComponent<Button>().onClick.AddListener(delegate
				{
					ButtonClickedHandler(newBtn);
				});
				_buttons.Add(newBtn);
			}
		}
		else if (showCloseBtn)
		{
			_buttonsTrans.gameObject.SetActive(true);
		}
		else
		{
			_buttonsTrans.gameObject.SetActive(false);
		}
		_templateButtonObj.SetActive(false);
		_boxVLayoutGroup.padding.bottom = ((!_buttonsTrans.gameObject.activeSelf) ? 40 : (-60));
		_waitingObj.SetActive(showWaiting);
		_couponBar.SetUp(PlayerDefs.CouponCount);
		_couponBar.SetLabels();
		_couponBar.gameObject.SetActive(showCouponBar);
	}

	private void OnDestroy()
	{
		DestroyButtons();
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (_waitingObj.activeSelf)
		{
			_waitingObj.transform.Rotate(_waitingRotBy * deltaTime);
		}
	}

	public void ButtonClickedHandler(GameObject btn)
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ButtonClickedEvent != null)
		{
			if (btn == _closeBtn)
			{
				this.ButtonClickedEvent(0);
				return;
			}
			int num = _buttons.IndexOf(btn);
			this.ButtonClickedEvent((num != -1) ? (num + 1) : num);
		}
	}
}
