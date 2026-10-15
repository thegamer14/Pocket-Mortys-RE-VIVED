using UnityEngine;
using UnityEngine.UI;

public class MPRegisterUsernameUI : UIBase
{
	public delegate void MPUserRegisterBtnDelegate(string username = null);

	private GameObject _currentStateObj;

	private GameObject _defaultStateObj;

	private GameObject _connectingStateObj;

	private GameObject _loginErrorStateObj;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private InputField _inputField;

	private GameObject _waitingObj;

	private Text _titleText;

	private Text _connectionText;

	private Text _loginErrorText;

	private Vector3 _waitingRotBy = new Vector3(0f, 0f, -400f);

	public string username
	{
		get
		{
			return _inputField.text;
		}
	}

	public event MPUserRegisterBtnDelegate RegisterBtnEvent;

	public event MPUserRegisterBtnDelegate BackBtnEvent;

	protected override void LateAwake()
	{
		_defaultStateObj = base.transform.FindChild("Default").gameObject;
		_connectingStateObj = base.transform.FindChild("Connecting").gameObject;
		_loginErrorStateObj = base.transform.FindChild("LoginError").gameObject;
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_inputField = _defaultStateObj.transform.FindChild("UsernameInput").GetComponent<InputField>();
		_waitingObj = _connectingStateObj.transform.FindChild("Wheel").gameObject;
		_titleText = base.transform.FindChild("Default/InputTitle").GetComponent<Text>();
		_connectionText = base.transform.FindChild("Connecting/Label").GetComponent<Text>();
		_loginErrorText = base.transform.FindChild("LoginError/Label").GetComponent<Text>();
		_defaultStateObj.SetActive(false);
		_connectingStateObj.SetActive(false);
		_loginErrorStateObj.SetActive(false);
	}

	public void SetUp(string username)
	{
		if (!string.IsNullOrEmpty(username))
		{
			_inputField.text = username;
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_REGISTER_USERNAME_BTN_NEXT);
		_titleText.text = MPTextDefs.MP_REGISTER_USERNAME_TITLE;
		_connectionText.text = MPTextDefs.MP_REGISTER_USERNAME_CONNECTING;
		_dialogue.SetText(MPTextDefs.MP_REGISTER_USERNAME_DIAL);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MenuBtnPressedHandler;
	}

	public void SetStateDefault()
	{
		if (_currentStateObj != null)
		{
			_currentStateObj.SetActive(false);
		}
		_defaultStateObj.SetActive(true);
		_currentStateObj = _defaultStateObj;
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, true);
	}

	public void SetStateConnecting()
	{
		if (_currentStateObj != null)
		{
			_currentStateObj.SetActive(false);
		}
		_connectingStateObj.SetActive(true);
		_currentStateObj = _connectingStateObj;
		_dialogue.TweenText(MPTextDefs.MP_REGISTER_USERNAME_CONNECTING_DIAL);
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
	}

	public void SetStateLoginError()
	{
		if (_currentStateObj != null)
		{
			_currentStateObj.SetActive(false);
		}
		_loginErrorStateObj.SetActive(true);
		_currentStateObj = _loginErrorStateObj;
		_loginErrorText.text = string.Format(MPTextDefs.MP_REGISTER_USERNAME_LOGIN_ERROR, App.saveData.multiplayer.username);
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, true);
	}

	public void SetErrorMessage(string errorMessage)
	{
		_dialogue.TweenText(errorMessage);
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (_waitingObj.activeSelf)
		{
			_waitingObj.transform.Rotate(_waitingRotBy * deltaTime);
		}
	}

	private void MenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.RegisterBtnEvent != null)
			{
				this.RegisterBtnEvent(_inputField.text);
			}
			break;
		case 1:
			if (this.BackBtnEvent != null)
			{
				this.BackBtnEvent();
			}
			break;
		}
	}
}
