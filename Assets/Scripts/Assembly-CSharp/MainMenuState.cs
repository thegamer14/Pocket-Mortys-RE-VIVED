using System;
using UnityEngine;

public class MainMenuState : StateBase
{
	private MainMenuUI _mainMenuUI;

	private bool _authenticationTried;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MainMenu/{0}", "MainMenu");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		if (!_authenticationTried && !App.saveData.player.googlePlaySignInDeclined)
		{
			App.social.Authenticate();
			_authenticationTried = true;
		}
		GameDefs.RemoveDefs();
		MPGameDefs.RemoveDefs();
		SharedStatInfo.ResetAll();
		MPSharedStateInfo.ResetAll();
		App.currentMode = eGameMode.NONE;
		App.sound.PlaySoundtrack("Audio/Soundtrack/World/main_theme_final", 0.6f);
		MPNotificaionLayer.Destroy();
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (TestDefs.MP_AUTO_SIGN_IN_FROM_MAIN_MENU)
		{
			UIMultiplayerHandler();
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_mainMenuUI = _ui as MainMenuUI;
		_mainMenuUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mainMenuUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		MainMenuUI mainMenuUI = _mainMenuUI;
		mainMenuUI.CampaignEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Combine(mainMenuUI.CampaignEvent, new MainMenuUI.UIMainMenuBtnDelegate(UICampaignHandler));
		MainMenuUI mainMenuUI2 = _mainMenuUI;
		mainMenuUI2.MultiplayerEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Combine(mainMenuUI2.MultiplayerEvent, new MainMenuUI.UIMainMenuBtnDelegate(UIMultiplayerHandler));
		MainMenuUI mainMenuUI3 = _mainMenuUI;
		mainMenuUI3.SettingsEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Combine(mainMenuUI3.SettingsEvent, new MainMenuUI.UIMainMenuBtnDelegate(UISettingsHandler));
		MainMenuUI mainMenuUI4 = _mainMenuUI;
		mainMenuUI4.GoogleSignInEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Combine(mainMenuUI4.GoogleSignInEvent, new MainMenuUI.UIMainMenuBtnDelegate(UIGoogleSignInHandler));
		App.states.BackButtonEvent += BackButtonHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MainMenuUI mainMenuUI = _mainMenuUI;
		mainMenuUI.CampaignEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Remove(mainMenuUI.CampaignEvent, new MainMenuUI.UIMainMenuBtnDelegate(UICampaignHandler));
		MainMenuUI mainMenuUI2 = _mainMenuUI;
		mainMenuUI2.MultiplayerEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Remove(mainMenuUI2.MultiplayerEvent, new MainMenuUI.UIMainMenuBtnDelegate(UIMultiplayerHandler));
		MainMenuUI mainMenuUI3 = _mainMenuUI;
		mainMenuUI3.SettingsEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Remove(mainMenuUI3.SettingsEvent, new MainMenuUI.UIMainMenuBtnDelegate(UISettingsHandler));
		MainMenuUI mainMenuUI4 = _mainMenuUI;
		mainMenuUI4.GoogleSignInEvent = (MainMenuUI.UIMainMenuBtnDelegate)Delegate.Remove(mainMenuUI4.GoogleSignInEvent, new MainMenuUI.UIMainMenuBtnDelegate(UIGoogleSignInHandler));
		App.states.BackButtonEvent -= BackButtonHandler;
	}

	private void UICampaignHandler()
	{
		App.currentMode = eGameMode.CAMPAIGN;
		GameDefs.SetUpDefs();
		SharedStatInfo.SetUp();
		TestDefs.OverrideSaveData();
		App.saveData.RunHotfixes();
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.MODE, "campaign_mode");
		GameDefs.StartGame();
		App.sound.StopSoundtrack(0.5f);
	}

	private void UIMultiplayerHandler()
	{
		if (App.saveData.multiplayer.spTutorialComplete || App.saveData.player.badges.Count >= 1 || TestDefs.MP_DISABLE_MULTIPLAYER_BADGE_REQ)
		{
			App.currentMode = eGameMode.MULTIPLAYER;
			App.saveData.multiplayer.spTutorialComplete = true;
			App.saveData.Save(eSaveDataType.MULTIPLAYER);
			PlayerAvatarDefs.SetUp();
			MPGameDefs.SetUpDefs();
			MPSharedStateInfo.SetUp();
			TestDefs.OverrideSaveData();
			App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.MODE, "multiplayer_mode");
			if (App.saveData.multiplayer.secretID == null)
			{
				MPSharedStateInfo.serverMessage = new MPServerMessageInfo(typeof(MPRegisterAvatarState));
			}
			else
			{
				MPSharedStateInfo.serverMessage = new MPServerMessageInfo(typeof(MPUserSignInState));
			}
			App.states.Set<MPServerMessageState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
			genericPopupState.SetDisplayValues(TextDefs.MAIN_MENU_MULTIPLAYER_LOCKED, null);
			genericPopupState.ButtonClickedEvent += MultiplayerLockedPopupHandler;
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private void UISettingsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.MAIN_MENU, "settings_menu");
		App.states.Set<SettingsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIGoogleSignInHandler()
	{
		App.social.Authenticate();
	}

	private void BackButtonHandler()
	{
		if (base.isInputActive)
		{
			GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
			genericPopupState.SetDisplayValues(TextDefs.QUIT_APP_DESCRIPTION, new string[1] { TextDefs.QUIT_APP_BTN });
			genericPopupState.ButtonClickedEvent += QuitAppPopupHandler;
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private void QuitAppPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= QuitAppPopupHandler;
		if (buttonIndex == 1)
		{
			Application.Quit();
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}

	private void MultiplayerLockedPopupHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= MultiplayerLockedPopupHandler;
		App.states.SetBack(new PopTransInfo());
	}
}
