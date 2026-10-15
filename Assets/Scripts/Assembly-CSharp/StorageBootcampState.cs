using System;
using System.Collections;
using UnityEngine;

public class StorageBootcampState : StateBase
{
	private StorageBootcampUI _storageBootcampUI;

	private GenericPopupState _popupState;

	private bool _inBuyCouponFlow;

	private bool _gotoResultsOnActivate;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageBootcamp/{0}", "StorageBootcamp");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.storageBootcampInfo == null)
		{
			SharedStatInfo.storageBootcampInfo = new StorageBootcampInfo();
		}
		base.Activate(uiContainerTrans, transInInfo);
		_inBuyCouponFlow = false;
		_gotoResultsOnActivate = false;
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED");
		StartCoroutine(ActivateComplete());
	}

	public override void SetAsCurrentComplete()
	{
		if (_inBuyCouponFlow)
		{
			_inBuyCouponFlow = false;
			if (PlayerDefs.CouponCount > 0 && BootcampDefs.GetBootcampState(_storageBootcampUI.selectedBootcampIndex) == eBootcampState.PROGRESS)
			{
				DisplaySpendCouponPopup();
			}
		}
		if (_gotoResultsOnActivate)
		{
			_gotoResultsOnActivate = false;
			App.states.Set<StorageBootcampResultsState>(new FadeTransInfo(), new FadeTransInfo(), true);
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_storageBootcampUI = _ui as StorageBootcampUI;
		_storageBootcampUI.SetUp();
		RefreshServerTimestamp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storageBootcampUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storageBootcampUI.BeginEvent += UIBeginHandler;
		_storageBootcampUI.EditEvent += UIEditHandler;
		_storageBootcampUI.ExpediteEvent += UIExpediteHandler;
		_storageBootcampUI.ResultsEvent += UIResultsHandler;
		_storageBootcampUI.BackEvent += UIBackHandler;
		_storageBootcampUI.BuyCouponsBtnEvent += UIBuyCouponsHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageBootcampUI.BeginEvent -= UIBeginHandler;
		_storageBootcampUI.EditEvent -= UIEditHandler;
		_storageBootcampUI.ExpediteEvent -= UIExpediteHandler;
		_storageBootcampUI.ResultsEvent -= UIResultsHandler;
		_storageBootcampUI.BackEvent -= UIBackHandler;
		_storageBootcampUI.BuyCouponsBtnEvent -= UIBuyCouponsHandler;
	}

	private void DisplayBeginConfirmPopup()
	{
		double num = BootcampDefs.GetBootcampDuration(_storageBootcampUI.selectedBootcampIndex);
		int num2 = (int)Math.Floor(num / 3600.0);
		int num3 = (int)Math.Floor(num % 3600.0 / 60.0);
		string arg = ((num3 != 0) ? string.Format("{0} hours and {1} minutes", num2, num3) : string.Format("{0} hours", num2));
		string text = string.Format(TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_CONFIRM, arg);
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(text, new string[1] { TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_BEGIN });
		_popupState.ButtonClickedEvent += ConfirmPopupButtonClickedHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void DisplayBeginBootcampServerCheckPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_WAIT_POPUP, null, false, true);
		StartCoroutine(BeginBootcampServerCheck());
	}

	private void DisplayBootcampBegunPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.ButtonClickedEvent += BootcampBegunButtonClickedHandler;
		_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_BEGUN_POPUP, null);
	}

	private void DisplaySpendCouponPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_EXPEDITE_POPUP_CONFIRM, new string[1] { TextDefs.MORTY_STORAGE_BOOTCAMP_BTN_EXPEDITE }, true, false, true);
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += SpendCouponButtonClickedHandler;
	}

	private void DisplayBuyCouponPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_EXPEDITE_POPUP_COUPONS, new string[1] { TextDefs.MORTY_STORAGE_BOOTCAMP_EXPEDITE_POPUP_COUPONS_BTN_BUY });
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		_popupState.ButtonClickedEvent += BuyCouponButtonClickedHandler;
	}

	private void DisplayViewResultsServerCheckPopup()
	{
		_popupState = App.states.Get<GenericPopupState>();
		_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_VIEW_RESULTS_WAIT_POPUP, null, false, true);
		_popupState.TransitionInCompleteEvent += ViewResultsServerCheckAnimInCompleteHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void RefreshServerTimestamp()
	{
		int num = UnityEngine.Random.Range(0, 999999999);
		StartCoroutine(TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", num), "utc_timestamp"));
	}

	private IEnumerator BeginBootcampServerCheck()
	{
		int urlRandomizer = UnityEngine.Random.Range(0, 999999999);
		CoroutineWithData cd = new CoroutineWithData(this, TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", urlRandomizer), "utc_timestamp"));
		yield return cd.coroutine;
		double timestamp = Convert.ToDouble(cd.result);
		if (timestamp > -1.0)
		{
			BootcampDefs.BeginBootcamp(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex, timestamp);
			App.saveData.Save(eSaveDataType.PLAYER);
			DisplayBootcampBegunPopup();
		}
		else
		{
			_popupState.ButtonClickedEvent += CantConnectButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_CANNOT_CONNECT, null);
		}
	}

	private IEnumerator ViewResultsServerCheck()
	{
		int urlRandomizer = UnityEngine.Random.Range(0, 999999999);
		CoroutineWithData cd = new CoroutineWithData(this, TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", urlRandomizer), "utc_timestamp"));
		yield return cd.coroutine;
		double timestamp = Convert.ToDouble(cd.result);
		if (timestamp > -1.0)
		{
			if (BootcampDefs.GetBootcampState(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex) == eBootcampState.RESULT_AVAILABLE)
			{
				_gotoResultsOnActivate = true;
				App.states.SetBack(new PopTransInfo());
			}
			else
			{
				_popupState.ButtonClickedEvent += NotCompleteButtonClickedHandler;
				_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_NOT_COMPLETE_POPUP, null);
			}
		}
		else
		{
			_popupState.ButtonClickedEvent += CantConnectButtonClickedHandler;
			_popupState.SetDisplayValues(TextDefs.MORTY_STORAGE_BOOTCAMP_BEGIN_CANNOT_CONNECT, null);
		}
	}

	private void UIEditHandler(int selectedDeckIndex = 0)
	{
		SharedStatInfo.storageBootcampInfo.selectedBootcampIndex = selectedDeckIndex;
		App.states.Set<StorageEditBootcampState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBeginHandler(int selectedDeckIndex = 0)
	{
		SharedStatInfo.storageBootcampInfo.selectedBootcampIndex = selectedDeckIndex;
		string failMessage = string.Empty;
		if (!BootcampDefs.CanBootcampBegin(selectedDeckIndex, out failMessage))
		{
			_storageBootcampUI.DisplayDialogue(failMessage);
		}
		else
		{
			DisplayBeginConfirmPopup();
		}
	}

	private void UIExpediteHandler(int selectedBootcampIndex = 0)
	{
		SharedStatInfo.storageBootcampInfo.selectedBootcampIndex = selectedBootcampIndex;
		if (PlayerDefs.CouponCount > 0)
		{
			DisplaySpendCouponPopup();
		}
		else
		{
			DisplayBuyCouponPopup();
		}
	}

	private void UIResultsHandler(int selectedBootcampIndex = 0)
	{
		SharedStatInfo.storageBootcampInfo.selectedBootcampIndex = selectedBootcampIndex;
		DisplayViewResultsServerCheckPopup();
	}

	private void UIBackHandler(int selectedDeckIndex = 0)
	{
		SharedStatInfo.storageBootcampInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIBuyCouponsHandler(int selectedDeckIndex = 0)
	{
		App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void ConfirmPopupButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= ConfirmPopupButtonClickedHandler;
		switch (buttonIndex)
		{
		case 0:
			App.states.SetBack(new PopTransInfo());
			break;
		case 1:
			DisplayBeginBootcampServerCheckPopup();
			break;
		}
	}

	private void CantConnectButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= CantConnectButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void NotCompleteButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= NotCompleteButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void BootcampBegunButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= BootcampBegunButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
	}

	private void SpendCouponButtonClickedHandler(int buttonIndex)
	{
		_popupState.ButtonClickedEvent -= SpendCouponButtonClickedHandler;
		if (buttonIndex == 1)
		{
			PlayerDefs.RemoveCoupon(1);
			BootcampDefs.SetBootcampComplete(_storageBootcampUI.selectedBootcampIndex);
			App.saveData.Save(eSaveDataType.PLAYER);
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

	private void ViewResultsServerCheckAnimInCompleteHandler(StateBase self)
	{
		_popupState.TransitionInCompleteEvent -= ViewResultsServerCheckAnimInCompleteHandler;
		StartCoroutine(ViewResultsServerCheck());
	}
}
