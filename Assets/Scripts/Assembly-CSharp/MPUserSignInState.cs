using System.Collections.Generic;
using UnityEngine;

public class MPUserSignInState : StateBase
{
	private MPUserSignInUI _userSignInUI;

	public eNetSessionDisconnectType disconnectType;

	private bool _autoConnect;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPUserSignIn/{0}", "MPUserSignIn");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		string text = null;
		string connectingMsg = MPTextDefs.MP_SIGN_IN_DIALOGUE_CONNECTING;
		if (disconnectType != eNetSessionDisconnectType.NONE)
		{
			if (disconnectType == eNetSessionDisconnectType.SERVER_ENDED)
			{
				text = string.Format(MPTextDefs.MP_SIGN_IN_KEEPALIVE_TIMEOUT_ERROR, disconnectType.ToString());
				connectingMsg = text;
			}
			else
			{
				text = string.Format(MPTextDefs.MP_SIGN_IN_ERROR, disconnectType.ToString());
			}
		}
		_userSignInUI.SetUp(connectingMsg, text);
		if (disconnectType == eNetSessionDisconnectType.NONE || disconnectType == eNetSessionDisconnectType.SERVER_ENDED)
		{
			_userSignInUI.SetStateConnecting();
			_autoConnect = true;
		}
		disconnectType = eNetSessionDisconnectType.NONE;
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_userSignInUI = _ui as MPUserSignInUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_userSignInUI = null;
		_autoConnect = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_userSignInUI.SignInBtnEvent += UISignInBtnHandler;
		_userSignInUI.BackBtnEvent += UIBackBtnHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_userSignInUI.SignInBtnEvent -= UISignInBtnHandler;
		_userSignInUI.BackBtnEvent -= UIBackBtnHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (TestDefs.MP_AUTO_SIGN_IN_FROM_MAIN_MENU || _autoConnect)
		{
			UISignInBtnHandler();
		}
	}

	private void JoinRoom()
	{
		App.net.room.Join(App.saveData.multiplayer.lastWorldId, delegate(eJoinError error, string worldId)
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
				_userSignInUI.SetErrorMessage(string.Format(MPTextDefs.MP_REGISTER_JOIN_ROOM_ERROR, error));
			}
		});
	}

	private void UISignInBtnHandler()
	{
		_userSignInUI.SetStateConnecting();
		App.net.session.Connect(App.saveData.multiplayer.secretID, delegate(eNetSessionConnectError error)
		{
			switch (error)
			{
			case eNetSessionConnectError.NONE:
				if (PlayerDefs.AddAvatarsOwned(new List<string>(App.net.player.data.ownedAvatars)))
				{
					App.saveData.Save(eSaveDataType.PLAYER);
				}
				JoinRoom();
				break;
			case eNetSessionConnectError.GET_PLAYER_DATA_ERROR:
				_userSignInUI.SetErrorMessage(MPTextDefs.MP_REGISTER_GET_PLAYER_ERROR);
				break;
			default:
				_userSignInUI.SetErrorMessage(string.Format(MPTextDefs.MP_REGISTER_CONNECT_ERROR, error));
				break;
			}
		});
	}

	private void UIBackBtnHandler()
	{
		App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	private void UIDeleteUserBtnHandler()
	{
		App.saveData.multiplayer.secretID = null;
		App.saveData.Save(eSaveDataType.MULTIPLAYER);
		App.states.Set<MPRegisterUsernameState>(new FadeTransInfo(), new FadeTransInfo());
	}
}
