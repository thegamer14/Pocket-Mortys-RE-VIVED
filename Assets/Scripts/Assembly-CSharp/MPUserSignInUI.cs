using UnityEngine;
using UnityEngine.UI;

public class MPUserSignInUI : UIBase
{
	public delegate void MPUserSginInBtnDelegate();

	private GameObject _currentStateObj;

	private GameObject _defaultStateObj;

	private GameObject _connectingStateObj;

	private Text _welcomeLabel;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private GameObject _waitingObj;

	private Vector3 _waitingRotBy = new Vector3(0f, 0f, -400f);

	private Image _avatar;

	private Image _avatarShadow;

	private string _connectingMsg;

	public event MPUserSginInBtnDelegate SignInBtnEvent;

	public event MPUserSginInBtnDelegate BackBtnEvent;

	protected override void LateAwake()
	{
		_defaultStateObj = base.transform.FindChild("Default").gameObject;
		_connectingStateObj = base.transform.FindChild("Connecting").gameObject;
		_welcomeLabel = base.transform.FindChild("Default/WelcomeLabel").GetComponent<Text>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_waitingObj = _connectingStateObj.transform.FindChild("Wheel").gameObject;
		_avatar = base.transform.FindChild("Avatar").GetComponent<Image>();
		_avatarShadow = base.transform.FindChild("AvatarShadow").GetComponent<Image>();
		_defaultStateObj.SetActive(false);
		_connectingStateObj.SetActive(false);
		SetAvatarSprite();
	}

	private void SetAvatarSprite()
	{
		if (!string.IsNullOrEmpty(App.saveData.multiplayer.playerAvatarID))
		{
			Sprite sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPPlayerAvatarDefs.AssetID(App.saveData.multiplayer.playerAvatarID), "frontFlipped"));
			if (sprite == null)
			{
				sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPPlayerAvatarDefs.AssetID(App.saveData.multiplayer.playerAvatarID), "front"));
			}
			_avatar.sprite = sprite;
			_avatarShadow.sprite = sprite;
			_avatar.SetNativeSize();
			_avatarShadow.SetNativeSize();
		}
	}

	public void SetUp(string connectingMsg, string errorMessage)
	{
		_connectingMsg = connectingMsg;
		if (string.IsNullOrEmpty(errorMessage))
		{
			SetStateDefault();
		}
		else
		{
			SetErrorMessage(errorMessage);
		}
	}

	public override void SetLabels()
	{
		_connectingStateObj.transform.FindChild("Label").GetComponent<Text>().text = MPTextDefs.MP_SIGN_IN_TITLE_CONNECTING;
		_welcomeLabel.text = string.Format(MPTextDefs.MP_SIGN_IN_TITLE, App.saveData.multiplayer.username);
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_SIGN_IN_BTN_ENTER);
		_dialogue.SetText(string.Format(MPTextDefs.MP_SIGN_IN_DIALOGUE_ENTER, App.saveData.multiplayer.username));
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
		_mainMenu.SetBtnVisible(0, true);
		_mainMenu.SetBtnVisible(1, true);
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
		_mainMenu.SetBtnVisible(0, false);
		_mainMenu.SetBtnVisible(1, false);
		_dialogue.TweenText(_connectingMsg);
	}

	public void SetErrorMessage(string errorMessage)
	{
		SetStateDefault();
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
			if (this.SignInBtnEvent != null)
			{
				this.SignInBtnEvent();
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
