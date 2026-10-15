using System;
using UnityEngine;

public class PauseState : StateBase
{
	private PauseUI _pauseUI;

	private bool _transitionedIntoPopup;

	public override void Awake()
	{
		base.Awake();
		base.hideStateBellow = false;
		_dontDestroyUI = true;
		_useBackgroundFade = true;
		_backgroundFadeColor = new Color(0f, 0f, 0f, 0.2f);
		_uiResourcePath = string.Format("UI/Pause/{0}", "Pause");
		PreLoadUI();
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (PlayerDefs.CurrentAlertNotificationID.Equals("AlertTutorialUseMPadID"))
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertTutorialUseItemIDKey");
			PlayerDefs.RemoveAllNotificationsFromWaitingList();
		}
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		_transitionedIntoPopup = false;
		ShowWorld(true);
		if ("AlertTutorialUseItemIDKey".Equals(PlayerDefs.CurrentAlertNotificationID) && ItemDefs.ItemsIndividualCount(PlayerDefs.CurrentItems(), "ItemSerum") == 0)
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertTutorialFindRickIDKey");
			PlayerDefs.RemoveAllNotificationsFromWaitingList();
			_pauseUI.SetNotificationTextForNotificationID(PlayerDefs.CurrentAlertNotificationID);
		}
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		if ("AlertTutorialUseItemIDKey".Equals(PlayerDefs.CurrentAlertNotificationID))
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertTutorialFindRickIDKey");
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_pauseUI = _ui as PauseUI;
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
		PauseUI pauseUI = _pauseUI;
		pauseUI.PartyPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI.PartyPressedEvent, new PauseUI.PauseUIDelegate(UIPartyPressedHandler));
		PauseUI pauseUI2 = _pauseUI;
		pauseUI2.MortydexPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI2.MortydexPressedEvent, new PauseUI.PauseUIDelegate(UIMortydexPressedHandler));
		PauseUI pauseUI3 = _pauseUI;
		pauseUI3.BagPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI3.BagPressedEvent, new PauseUI.PauseUIDelegate(UIBagPressedHandler));
		PauseUI pauseUI4 = _pauseUI;
		pauseUI4.SalesmanRickPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI4.SalesmanRickPressedEvent, new PauseUI.PauseUIDelegate(UISalesmanRickPressedHandler));
		PauseUI pauseUI5 = _pauseUI;
		pauseUI5.QuestPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI5.QuestPressedEvent, new PauseUI.PauseUIDelegate(UIQuestPressedHandler));
		PauseUI pauseUI6 = _pauseUI;
		pauseUI6.SettingsPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI6.SettingsPressedEvent, new PauseUI.PauseUIDelegate(UISettingsPressedHandler));
		PauseUI pauseUI7 = _pauseUI;
		pauseUI7.SocialPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI7.SocialPressedEvent, new PauseUI.PauseUIDelegate(UISocialPressedHandler));
		PauseUI pauseUI8 = _pauseUI;
		pauseUI8.BadgesPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI8.BadgesPressedEvent, new PauseUI.PauseUIDelegate(UIBadgesPressedHandler));
		PauseUI pauseUI9 = _pauseUI;
		pauseUI9.BackPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI9.BackPressedEvent, new PauseUI.PauseUIDelegate(UIBackPressedHandler));
		PauseUI pauseUI10 = _pauseUI;
		pauseUI10.QuitPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Combine(pauseUI10.QuitPressedEvent, new PauseUI.PauseUIDelegate(UIQuitPressedHandler));
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		PauseUI pauseUI = _pauseUI;
		pauseUI.PartyPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI.PartyPressedEvent, new PauseUI.PauseUIDelegate(UIPartyPressedHandler));
		PauseUI pauseUI2 = _pauseUI;
		pauseUI2.MortydexPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI2.MortydexPressedEvent, new PauseUI.PauseUIDelegate(UIMortydexPressedHandler));
		PauseUI pauseUI3 = _pauseUI;
		pauseUI3.BagPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI3.BagPressedEvent, new PauseUI.PauseUIDelegate(UIBagPressedHandler));
		PauseUI pauseUI4 = _pauseUI;
		pauseUI4.SalesmanRickPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI4.SalesmanRickPressedEvent, new PauseUI.PauseUIDelegate(UISalesmanRickPressedHandler));
		PauseUI pauseUI5 = _pauseUI;
		pauseUI5.QuestPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI5.QuestPressedEvent, new PauseUI.PauseUIDelegate(UIQuestPressedHandler));
		PauseUI pauseUI6 = _pauseUI;
		pauseUI6.SettingsPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI6.SettingsPressedEvent, new PauseUI.PauseUIDelegate(UISettingsPressedHandler));
		PauseUI pauseUI7 = _pauseUI;
		pauseUI7.SocialPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI7.SocialPressedEvent, new PauseUI.PauseUIDelegate(UISocialPressedHandler));
		PauseUI pauseUI8 = _pauseUI;
		pauseUI8.BadgesPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI8.BadgesPressedEvent, new PauseUI.PauseUIDelegate(UIBadgesPressedHandler));
		PauseUI pauseUI9 = _pauseUI;
		pauseUI9.BackPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI9.BackPressedEvent, new PauseUI.PauseUIDelegate(UIBackPressedHandler));
		PauseUI pauseUI10 = _pauseUI;
		pauseUI10.QuitPressedEvent = (PauseUI.PauseUIDelegate)Delegate.Remove(pauseUI10.QuitPressedEvent, new PauseUI.PauseUIDelegate(UIQuitPressedHandler));
		App.social.AuthenticateEvent -= SocialAuthenticateHandler;
	}

	private void ShowWorld(bool value)
	{
		if (App.states.Get<WorldState>().status == eStateStatus.CACHED)
		{
			App.states.Get<WorldState>().ShowWorld(value);
		}
		if (App.states.Get<TournamentArenaState>().status == eStateStatus.CACHED)
		{
			App.states.Get<TournamentArenaState>().ShowWorld(value);
		}
	}

	private void RefreshPlayerMorties()
	{
		if (App.states.Get<WorldState>().status == eStateStatus.CACHED)
		{
			App.states.Get<WorldState>().RefreshPlayerMorties();
		}
		if (App.states.Get<TournamentArenaState>().status == eStateStatus.CACHED)
		{
			App.states.Get<TournamentArenaState>().RefreshPlayerMorties();
		}
	}

	private void RefreshPlayerAsset()
	{
		if (App.states.Get<WorldState>().status == eStateStatus.CACHED)
		{
			App.states.Get<WorldState>().RefreshPlayerAsset();
		}
	}

	private void UIPartyPressedHandler()
	{
		SharedStatInfo.partyInfo = new PartyInfo(PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
		App.states.Set<PartyState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIMortydexPressedHandler()
	{
		App.states.Set<MortydexState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBagPressedHandler()
	{
		SharedStatInfo.bagInfo = new BagInfo(PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems(), eBagFromState.WORLD, null);
		App.states.Set<BagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UISalesmanRickPressedHandler()
	{
		if (PlayerDefs.GetLocationType() == eLocationType.TOURNAMENT_ARENA)
		{
			GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
			genericPopupState.SetDisplayValues(TextDefs.PAUSE_SALEMAN_PRESS_IN_TOURNAMENT, null);
			genericPopupState.ButtonClickedEvent += SalesmanPressInTournamentPopupHandler;
			_transitionedIntoPopup = true;
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else
		{
			App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PAUSE_MENU, "store_menu");
			App.states.Get<SalesmanRickState>().playStoreSoundtrack = false;
			App.states.Set<SalesmanRickState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	private void UIQuestPressedHandler()
	{
		App.states.Set<QuestBookState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UISettingsPressedHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PAUSE_MENU, "settings_menu");
		App.states.Set<SettingsState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UISocialPressedHandler()
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

	private void UIBadgesPressedHandler()
	{
		App.states.Set<BadgeCollectionState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackPressedHandler()
	{
		RefreshPlayerMorties();
		RefreshPlayerAsset();
		App.states.SetBack(new SlideTransInfo
		{
			direction = eDirection.DOWN,
			duration = 0.3f
		});
	}

	private void UIQuitPressedHandler()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.SetDisplayValues(TextDefs.PAUSE_QUIT_POPUP_DESCRIPTION, new string[1] { TextDefs.QUIT_APP_BTN });
		genericPopupState.ButtonClickedEvent += QuitAppPopupHandler;
		_transitionedIntoPopup = true;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void SocialAuthenticateHandler(bool success)
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

	private void SocialAuthenticatePopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= SocialAuthenticatePopupHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void SalesmanPressInTournamentPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= SalesmanPressInTournamentPopupHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void QuitAppPopupHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= QuitAppPopupHandler;
		if (buttonIndex == 1)
		{
			App.states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}
}
