using System;
using System.Collections.Generic;
using UnityEngine;

public class MPPauseState : StateBase
{
	private MPPauseUI _pauseUI;

	private bool _transitionedIntoPopup;

	public override void Awake()
	{
		base.Awake();
		base.hideStateBellow = false;
		_dontDestroyUI = true;
		_useBackgroundFade = true;
		_backgroundFadeColor = new Color(0f, 0f, 0f, 0.2f);
		_uiResourcePath = string.Format("UI/MPPause/{0}", "MPPause");
		PreLoadUI();
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_transitionedIntoPopup = false;
		_pauseUI.TweenFriendBtnFlash(App.net.battleInvite.friendInvitePending);
		_pauseUI.TweenChallengeBtnFlash(App.net.player.data.challengeComplete);
		StartCoroutine(ActivateComplete());
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		_transitionedIntoPopup = false;
		_pauseUI.TweenFriendBtnFlash(App.net.battleInvite.friendInvitePending);
		_pauseUI.TweenChallengeBtnFlash(App.net.player.data.challengeComplete);
		ShowWorld(true);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_pauseUI = _ui as MPPauseUI;
		_pauseUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_pauseUI = null;
	}

	protected override void DestroyCache()
	{
		base.DestroyCache();
		if (!_transitionedIntoPopup)
		{
			ShowWorld(false);
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		MPPauseUI pauseUI = _pauseUI;
		pauseUI.PartyPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI.PartyPressedEvent, new MPPauseUI.PauseUIDelegate(UIPartyPressedHandler));
		MPPauseUI pauseUI2 = _pauseUI;
		pauseUI2.MortydexPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI2.MortydexPressedEvent, new MPPauseUI.PauseUIDelegate(UIMortydexPressedHandler));
		MPPauseUI pauseUI3 = _pauseUI;
		pauseUI3.BagPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI3.BagPressedEvent, new MPPauseUI.PauseUIDelegate(UIBagPressedHandler));
		MPPauseUI pauseUI4 = _pauseUI;
		pauseUI4.SalesmanRickPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI4.SalesmanRickPressedEvent, new MPPauseUI.PauseUIDelegate(UISalesmanRickPressedHandler));
		MPPauseUI pauseUI5 = _pauseUI;
		pauseUI5.FriendsPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI5.FriendsPressedEvent, new MPPauseUI.PauseUIDelegate(UIFriendsPressedHandler));
		MPPauseUI pauseUI6 = _pauseUI;
		pauseUI6.SettingsPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI6.SettingsPressedEvent, new MPPauseUI.PauseUIDelegate(UISettingsPressedHandler));
		MPPauseUI pauseUI7 = _pauseUI;
		pauseUI7.SocialPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI7.SocialPressedEvent, new MPPauseUI.PauseUIDelegate(UISocialPressedHandler));
		MPPauseUI pauseUI8 = _pauseUI;
		pauseUI8.ChallengePressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI8.ChallengePressedEvent, new MPPauseUI.PauseUIDelegate(UIChallengePressedHandler));
		MPPauseUI pauseUI9 = _pauseUI;
		pauseUI9.BackPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI9.BackPressedEvent, new MPPauseUI.PauseUIDelegate(UIBackPressedHandler));
		MPPauseUI pauseUI10 = _pauseUI;
		pauseUI10.QuitPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Combine(pauseUI10.QuitPressedEvent, new MPPauseUI.PauseUIDelegate(UIQuitPressedHandler));
		App.net.battleInvite.InviteQueueChangeEvent += InviteQueueChangeHandler;
		App.net.player.ChallengeCompleteEvent += ChallengeCompleteHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MPPauseUI pauseUI = _pauseUI;
		pauseUI.PartyPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI.PartyPressedEvent, new MPPauseUI.PauseUIDelegate(UIPartyPressedHandler));
		MPPauseUI pauseUI2 = _pauseUI;
		pauseUI2.MortydexPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI2.MortydexPressedEvent, new MPPauseUI.PauseUIDelegate(UIMortydexPressedHandler));
		MPPauseUI pauseUI3 = _pauseUI;
		pauseUI3.BagPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI3.BagPressedEvent, new MPPauseUI.PauseUIDelegate(UIBagPressedHandler));
		MPPauseUI pauseUI4 = _pauseUI;
		pauseUI4.SalesmanRickPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI4.SalesmanRickPressedEvent, new MPPauseUI.PauseUIDelegate(UISalesmanRickPressedHandler));
		MPPauseUI pauseUI5 = _pauseUI;
		pauseUI5.FriendsPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI5.FriendsPressedEvent, new MPPauseUI.PauseUIDelegate(UIFriendsPressedHandler));
		MPPauseUI pauseUI6 = _pauseUI;
		pauseUI6.SettingsPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI6.SettingsPressedEvent, new MPPauseUI.PauseUIDelegate(UISettingsPressedHandler));
		MPPauseUI pauseUI7 = _pauseUI;
		pauseUI7.SocialPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI7.SocialPressedEvent, new MPPauseUI.PauseUIDelegate(UISocialPressedHandler));
		MPPauseUI pauseUI8 = _pauseUI;
		pauseUI8.ChallengePressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI8.ChallengePressedEvent, new MPPauseUI.PauseUIDelegate(UIChallengePressedHandler));
		MPPauseUI pauseUI9 = _pauseUI;
		pauseUI9.BackPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI9.BackPressedEvent, new MPPauseUI.PauseUIDelegate(UIBackPressedHandler));
		MPPauseUI pauseUI10 = _pauseUI;
		pauseUI10.QuitPressedEvent = (MPPauseUI.PauseUIDelegate)Delegate.Remove(pauseUI10.QuitPressedEvent, new MPPauseUI.PauseUIDelegate(UIQuitPressedHandler));
		App.social.AuthenticateEvent -= SocialAuthenticateHandler;
		App.net.battleInvite.InviteQueueChangeEvent -= InviteQueueChangeHandler;
		App.net.player.ChallengeCompleteEvent -= ChallengeCompleteHandler;
	}

	public void ShowWorld(bool value)
	{
		if (App.states.Get<MPWorldState>().status == eStateStatus.CACHED)
		{
			App.states.Get<MPWorldState>().ShowWorld(value);
		}
	}

	public void UIPartyPressedHandler()
	{
		MPSharedStateInfo.partyInfo = new MPPartyInfo(eMPStateOrigin.WORLD);
		App.states.Set<MPPartyState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UIMortydexPressedHandler()
	{
		MPSharedStateInfo.mortydexInfo = new MPMortydexInfo(eMPStateOrigin.WORLD);
		App.states.Set<MPMortydexState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UIBagPressedHandler()
	{
		MPSharedStateInfo.bagInfo = new MPBagInfo(eMPStateOrigin.WORLD, App.net.player.data.items);
		App.states.Set<MPBagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UISalesmanRickPressedHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PAUSE_MENU, "store_menu");
		App.states.Get<MPSalesmanRickState>().playStoreSoundtrack = false;
		App.states.Set<MPSalesmanRickState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UIFriendsPressedHandler()
	{
		MPSharedStateInfo.friendInfo = new MPFriendsInfo(eMPStateOrigin.WORLD, null);
		App.states.Set<MPFriendState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UISettingsPressedHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PAUSE_MENU, "settings_menu");
		App.states.Set<SettingsState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void UISocialPressedHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PAUSE_MENU, "game_center");
		if (!App.social.IsAuthenticated())
		{
			App.social.AuthenticateEvent -= SocialAuthenticateHandler;
			App.social.AuthenticateEvent += SocialAuthenticateHandler;
			App.social.Authenticate();
		}
		else
		{
			App.social.ShowAchievementUIOverlay();
		}
	}

	public void UIChallengePressedHandler()
	{
		App.states.Set<MPChallengeState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	public void UIBackPressedHandler()
	{
		App.states.SetBack(new SlideTransInfo
		{
			direction = eDirection.DOWN,
			duration = 0.3f
		});
	}

	public void UIQuitPressedHandler()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(TextDefs.PAUSE_QUIT_POPUP_DESCRIPTION, new string[1] { TextDefs.QUIT_APP_BTN });
		genericPopupState.ButtonClickedEvent += QuitPopupHandler;
		_transitionedIntoPopup = true;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	public void SocialAuthenticateHandler(bool success)
	{
		App.social.AuthenticateEvent -= SocialAuthenticateHandler;
		if (!success)
		{
			GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
			genericPopupState.SetDisplayValues(TextDefs.SOCIAL_AUTHENTICATE_FAILED, new string[0]);
			genericPopupState.ButtonClickedEvent += SocialAuthenticatePopupHandler;
			_transitionedIntoPopup = true;
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (App.social.IsAuthenticated())
		{
			App.social.ShowAchievementUIOverlay();
		}
	}

	private void InviteQueueChangeHandler(List<MPBattleInviteData> invites, bool friendInvitePending)
	{
		_pauseUI.TweenFriendBtnFlash(friendInvitePending);
	}

	private void ChallengeCompleteHandler()
	{
		_pauseUI.TweenChallengeBtnFlash(true);
	}

	private void SocialAuthenticatePopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= SocialAuthenticatePopupHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void QuitPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= QuitPopupHandler;
		if (buttonIndex == 1)
		{
			App.net.session.Disconnect(eNetSessionDisconnectType.USER_QUIT);
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}
}
