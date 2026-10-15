using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentEventSelectState : StateBase
{
	private TournamentEventSelectUI _eventSelectUI;

	private GenericPopupState _popupState;

	private KeyValuePair<string, string> _tournamentAndEventID;

	private TournamentEventData _eventData;

	private double _countdown;

	private bool _inBuyCouponFlow;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TournamentEventSelect/{0}", "TournamentEventSelect");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.6f);
		_inBuyCouponFlow = false;
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	public override void SetAsCurrentComplete()
	{
		if (_inBuyCouponFlow)
		{
			_inBuyCouponFlow = false;
			if (PlayerDefs.CouponCount > 0)
			{
				DisplaySpendCouponPopup();
			}
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_eventSelectUI = _ui as TournamentEventSelectUI;
		_tournamentAndEventID = TournamentDefs.GetNextUncompleteEvent();
		_eventData = TournamentDefs.GetEventData(_tournamentAndEventID.Key, _tournamentAndEventID.Value);
		_countdown = TournamentDefs.GetTimestampCountdownLocal();
		bool isTimeStampLocked = _countdown > 0.0 && !_eventData.isOpenWithCoupon && _eventData.attempts == 0;
		_eventSelectUI.SetUp(_tournamentAndEventID.Key, isTimeStampLocked, _countdown);
		StartCoroutine(SetUpCheckServerTime());
	}

	protected override void Destroy()
	{
		base.Destroy();
		_eventSelectUI = null;
		_popupState = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_eventSelectUI.EnterEvent += UIEnterHandler;
		_eventSelectUI.UnlockEvent += UIUnlockHandler;
		_eventSelectUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_eventSelectUI.EnterEvent -= UIEnterHandler;
		_eventSelectUI.UnlockEvent -= UIUnlockHandler;
		_eventSelectUI.BackEvent -= UIBackHandler;
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (!(_eventSelectUI == null) && _countdown > 0.0)
		{
			_countdown = TournamentDefs.GetTimestampCountdownLocal();
			_eventSelectUI.SetCountdown(Math.Ceiling(_countdown));
		}
	}

	private IEnumerator SetUpCheckServerTime()
	{
		CoroutineWithData cd = new CoroutineWithData(this, TournamentDefs.GetTimestampCountdownServer(this));
		yield return cd.coroutine;
		double serverCountdown = Convert.ToDouble(cd.result);
		if (IsActive())
		{
			bool timestampLocked = serverCountdown > 0.0 && !_eventData.isOpenWithCoupon && _eventData.attempts == 0;
			if (serverCountdown > -1.0)
			{
				_eventSelectUI.SetTimestampLocked(timestampLocked);
				_eventSelectUI.SetCountdown(serverCountdown);
			}
		}
	}

	private IEnumerator EnterCheckServerTime()
	{
		CoroutineWithData cd = new CoroutineWithData(this, TournamentDefs.GetTimestampCountdownServer(this));
		yield return cd.coroutine;
		double serverCountdown = Convert.ToDouble(cd.result);
		if (serverCountdown > -1.0)
		{
			if (serverCountdown == 0.0 || _eventData.isOpenWithCoupon)
			{
				EnterNextEvent(true);
				yield break;
			}
			_popupState.ButtonClickedEvent += NotOpenButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.TOURNAMENT_EVENT_SELECT_NOT_OPEN_POPUP, null);
			_countdown = TournamentDefs.GetTimestampCountdownLocal();
			_eventSelectUI.SetTimestampLocked(true);
			_eventSelectUI.SetCountdown(Math.Ceiling(_countdown));
		}
		else
		{
			_popupState.ButtonClickedEvent += CantConnectButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.TOURNAMENT_EVENT_SELECT_CANT_CONNECT_POPUP, null);
		}
	}

	private void EnterNextEvent(bool fromPopup)
	{
		if (_eventData.attempts == 0)
		{
			TournamentDefs.SetTimestamp(TimeServer.lastServerTime);
		}
		_eventData.attempts++;
		App.saveData.Save(eSaveDataType.TOURNAMENT);
		SharedStatInfo.tournamentSetUpInfo = new TournamentSetUpInfo(_tournamentAndEventID.Key, _tournamentAndEventID.Value);
		if (fromPopup)
		{
			StartCoroutine(App.states.SetBackFromPopup(new FadeTransInfo(), new FadeTransInfo()));
		}
		else
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void DisplayWaitingForServerPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.TOURNAMENT_EVENT_SELECT_WAIT_POPUP, null, false, true);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.TransitionInCompleteEvent += WaitingForServerPopupInComplete;
	}

	private void DisplaySpendCouponPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.TOURNAMENT_EVENT_SELECT_SPEND_COUPON_POPUP, new string[1] { TextDefs.TOURNAMENT_EVENT_SELECT_POPUP_SPEND_BTN }, true, false, true);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += SpendCouponButtonClickedHandler;
	}

	private void DisplayBuyCouponPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.TOURNAMENT_EVENT_SELECT_BUY_COUPONS_POPUP, new string[1] { TextDefs.TOURNAMENT_EVENT_SELECT_POPUP_BUY_BTN });
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += BuyCouponButtonClickedHandler;
	}

	private void UIEnterHandler()
	{
		if (_eventData.attempts > 0)
		{
			EnterNextEvent(false);
		}
		else
		{
			DisplayWaitingForServerPopup();
		}
	}

	private void UIUnlockHandler()
	{
		if (PlayerDefs.CouponCount > 0)
		{
			DisplaySpendCouponPopup();
		}
		else
		{
			DisplayBuyCouponPopup();
		}
	}

	private void UIBackHandler()
	{
		SharedStatInfo.tournamentSetUpInfo = null;
		SharedStatInfo.worldInfo.worldID = "TournamentLobby";
		SharedStatInfo.worldInfo.entranceID = "TournamentLobbyOrganiserPortal";
		App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	public void WaitingForServerPopupInComplete(StateBase self)
	{
		_popupState.TransitionInCompleteEvent -= WaitingForServerPopupInComplete;
		StartCoroutine(EnterCheckServerTime());
	}

	private void NotOpenButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= NotOpenButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void CantConnectButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= CantConnectButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void SpendCouponButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= SpendCouponButtonClickedHandler;
		if (buttonIndex == 1)
		{
			PlayerDefs.RemoveCoupon(1);
			_eventData.isOpenWithCoupon = true;
			App.saveData.Save(eSaveDataType.PLAYER | eSaveDataType.TOURNAMENT);
			_eventSelectUI.SetTimestampLocked(false);
			App.upsight.CustomAnalyticEvent(eSubType1.TOURNAMENTS, eSubType2.UNLOCK_TOURNAMENT_BC, _tournamentAndEventID.Value);
		}
		App.states.SetBack(new PopTransInfo());
	}

	private void BuyCouponButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= BuyCouponButtonClickedHandler;
		switch (buttonIndex)
		{
		case 0:
			App.states.SetBack(new PopTransInfo());
			break;
		case 1:
			_inBuyCouponFlow = true;
			App.states.Set<CouponBuyPopupState>(new PopTransInfo(), new PopTransInfo());
			break;
		}
	}
}
