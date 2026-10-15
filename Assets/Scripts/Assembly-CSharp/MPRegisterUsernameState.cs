using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class MPRegisterUsernameState : StateBase
{
	private MPRegisterUsernameUI _userRegisterUI;

	private bool _tutorialShown;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPRegisterUsername/{0}", "MPRegisterUsername");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.ONBOARDING, "picked_username");
		_userRegisterUI.SetStateDefault();
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (!_tutorialShown && string.IsNullOrEmpty(MPSharedStateInfo.registrationInfo.username))
		{
			App.states.Get<MPTutorialPopupState>().SetDisplayValues(MPTextDefs.MP_REGISTER_USERNAME_TIP);
			App.states.Set<MPTutorialPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
			_tutorialShown = true;
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_userRegisterUI = _ui as MPRegisterUsernameUI;
		_userRegisterUI.SetUp(MPSharedStateInfo.registrationInfo.username);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_userRegisterUI = null;
		_tutorialShown = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_userRegisterUI.RegisterBtnEvent += UIRegisterBtnHandler;
		_userRegisterUI.BackBtnEvent += UIBackBtnHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_userRegisterUI.RegisterBtnEvent -= UIRegisterBtnHandler;
		_userRegisterUI.BackBtnEvent -= UIBackBtnHandler;
	}

	private bool ValidateUsername(string username)
	{
		if (username.Length < 3)
		{
			_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_LENGTH);
			return false;
		}
		Regex regex = new Regex("^[a-zA-Z0-9-_]*$");
		if (!regex.IsMatch(username) || username.Any(char.IsWhiteSpace))
		{
			_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_CHAR);
			return false;
		}
		return true;
	}

	private void Register()
	{
		_userRegisterUI.SetStateConnecting();
		App.net.player.Register(MPSharedStateInfo.registrationInfo.username, MPSharedStateInfo.registrationInfo.avatarID, MPSharedStateInfo.registrationInfo.mortyIDs, delegate(eRegisterFailedError error)
		{
			if (error == eRegisterFailedError.NONE)
			{
				MPSharedStateInfo.registrationInfo = null;
				App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.ONBOARDING, "onboarding_process_complete");
				ConnectToSession();
			}
			else
			{
				_userRegisterUI.SetStateDefault();
				switch (error)
				{
				case eRegisterFailedError.USERNAME_LENGTH:
					_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_LENGTH);
					break;
				case eRegisterFailedError.USERNAME_CHARACTERS:
					_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_CHAR);
					break;
				case eRegisterFailedError.USERNAME_DUPLICATE:
					_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_TAKEN);
					break;
				case eRegisterFailedError.USERNAME_PROFANITY:
					_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_USERNAME_INVALID_PROFANITY);
					break;
				default:
					_userRegisterUI.SetErrorMessage(string.Format(MPTextDefs.MP_REGISTER_FAIL, error));
					break;
				}
			}
		});
	}

	private void ConnectToSession()
	{
		_userRegisterUI.SetStateConnecting();
		App.net.session.Connect(App.saveData.multiplayer.secretID, delegate(eNetSessionConnectError error)
		{
			switch (error)
			{
			case eNetSessionConnectError.NONE:
				JoinRoom();
				break;
			case eNetSessionConnectError.GET_PLAYER_DATA_ERROR:
				_userRegisterUI.SetStateLoginError();
				_userRegisterUI.SetErrorMessage(MPTextDefs.MP_REGISTER_GET_PLAYER_ERROR);
				break;
			default:
				_userRegisterUI.SetStateLoginError();
				_userRegisterUI.SetErrorMessage(string.Format(MPTextDefs.MP_REGISTER_CONNECT_ERROR, error));
				break;
			}
		});
	}

	private void JoinRoom()
	{
		App.net.room.Join("1", delegate(eJoinError error, string worldId)
		{
			if (error == eJoinError.NONE)
			{
				App.net.player.data.placementData.walkIn = true;
				App.redirectOnMPSessionDisconnect = true;
				MPSharedStateInfo.worldInfo = new MPWorldInfo(worldId);
				App.states.Set<MPWorldState>(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				_userRegisterUI.SetStateLoginError();
				_userRegisterUI.SetErrorMessage(string.Format(MPTextDefs.MP_REGISTER_JOIN_ROOM_ERROR, error));
			}
		});
	}

	private void UIRegisterBtnHandler(string username = null)
	{
		if (!App.net.player.IsRegistered())
		{
			MPSharedStateInfo.registrationInfo.username = username;
			if (ValidateUsername(username))
			{
				Register();
			}
		}
		else
		{
			ConnectToSession();
		}
	}

	private void UIBackBtnHandler(string username = null)
	{
		if (!App.net.player.IsRegistered())
		{
			MPSharedStateInfo.registrationInfo.username = _userRegisterUI.username;
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
