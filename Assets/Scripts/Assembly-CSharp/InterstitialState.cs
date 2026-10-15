using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class InterstitialState : StateBase
{
	private const float _kCatchDeeplinkDelay = 4f;

	[HideInInspector]
	public eUpsightPlacementID placementID;

	[HideInInspector]
	public bool nextStateIsBack;

	[HideInInspector]
	public Type nextStateType;

	private bool _isMakingPurchase;

	private bool _isShowingCouponBuyPopup;

	private bool _popupIsDisplayed;

	private Coroutine _contentActionedCO;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Interstitial/{0}", "Interstitial");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_isMakingPurchase = false;
		_isShowingCouponBuyPopup = false;
		_popupIsDisplayed = false;
		_contentActionedCO = null;
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (_isMakingPurchase)
		{
			DebugLog("Skipping because coming back from IAP flow");
			GoToNextState();
		}
		else if (_isShowingCouponBuyPopup)
		{
			DebugLog("Skipping because coming back from coupon buy state");
			GoToNextState();
		}
		else if (App.saveData.settings.removeAds)
		{
			DebugLog("Skipping because removeAds is true");
			GoToNextState();
		}
		else if (TestDefs.TEST_IAP_PROMO_INTERSTITIAL)
		{
			ContentDismissedHandler(placementID.ToString(), "contentunittriggereddismiss");
			ActivateCouponBuyPopup();
		}
		else if (!App.upsight.isUpsightOpen)
		{
			DebugLog("Skipping because isUpsightOpen is false");
			App.upsight.AttemptUpsightConnection();
			GoToNextState();
		}
		else if (TestDefs.DISABLE_UPSIGHT_INTERSTITIALS)
		{
			DebugLog("Skipping because DISABLE_UPSIGHT_INTERSTITIALS test flag is true");
			GoToNextState();
		}
		else
		{
			_popupIsDisplayed = false;
			App.upsight.SendContentRequest(placementID);
		}
	}

	public override void Deactivate(TransInfo transInfo)
	{
		nextStateType = null;
		nextStateIsBack = false;
		if (_contentActionedCO != null)
		{
			StopCoroutine(_contentActionedCO);
		}
		base.Deactivate(transInfo);
	}

	private void GoToNextState()
	{
		DebugLog("GoToNextState()");
		if (nextStateIsBack)
		{
			App.states.SetBack(null, new FadeTransInfo());
		}
		else if (nextStateType != null)
		{
			App.states.Set(nextStateType, null, new FadeTransInfo());
		}
	}

	private IEnumerator ContentActionedCoroutine()
	{
		DebugLog("ContentActionedCoroutine()");
		yield return new WaitForSeconds(4f);
		_contentActionedCO = null;
		DebugLog("ContentActionedCoroutine() calling GoToNextState()");
		GoToNextState();
	}

	public void ActivateCouponBuyPopup()
	{
		DebugLog("ActivateCouponBuyPopup()");
		if (!IsActive())
		{
			DebugLog("skipping ActivateCouponBuyPopup(), interstitial state is not active.");
			return;
		}
		if (_isMakingPurchase || _isShowingCouponBuyPopup)
		{
			DebugLog("skipping ActivateCouponBuyPopup(), flow in progress. _isMakingPurchase:" + _isMakingPurchase + " _isShowingCouponBuyPopup:" + _isShowingCouponBuyPopup);
			return;
		}
		_isShowingCouponBuyPopup = true;
		if (_contentActionedCO != null)
		{
			DebugLog("stopping _contentActionedCO coroutine.");
			StopCoroutine(_contentActionedCO);
		}
		DebugLog("activate coupon buy popup state");
		if (App.currentMode == eGameMode.MULTIPLAYER)
		{
			App.states.Set<MPCouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else
		{
			App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
	}

	protected void TestMakePurchase()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("productIdentifier", "com.adultswim.pocketmorties.coupon05discount");
		MakePurchaseHandler(UpsightPurchase.purchaseFromJson(Json.Serialize(dictionary)));
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		UpsightManager.openRequestFailedEvent += OpenRequestFailedHandler;
		UpsightManager.contentRequestFailedEvent += ContentRequestFailedHandler;
		UpsightManager.contentDismissedEvent += ContentDismissedHandler;
		UpsightManager.makePurchaseEvent += MakePurchaseHandler;
		UpsightManager.contentWillDisplayEvent += ContentWillDisplayHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		UpsightManager.openRequestFailedEvent -= OpenRequestFailedHandler;
		UpsightManager.contentRequestFailedEvent -= ContentRequestFailedHandler;
		UpsightManager.contentDismissedEvent -= ContentDismissedHandler;
		UpsightManager.makePurchaseEvent -= MakePurchaseHandler;
		UpsightManager.contentWillDisplayEvent -= ContentWillDisplayHandler;
	}

	private void OpenRequestFailedHandler(string error)
	{
		DebugLog("OpenRequestFailedHandler() - " + error);
		GoToNextState();
	}

	private void ContentRequestFailedHandler(string placement, string error)
	{
		DebugLog("ContentRequestFailedHandler(placement: " + placement + ", error: " + error + ")");
		GoToNextState();
	}

	private void ContentDismissedHandler(string placement, string dismissType)
	{
		DebugLog("ContentDismissedHandler() - " + dismissType + " _isMakingPurchase:" + _isMakingPurchase + " _isShowingCouponBuyPopup:" + _isShowingCouponBuyPopup);
		if (!string.IsNullOrEmpty(dismissType) && dismissType.ToLower() == "purchase")
		{
			DebugLog("ContentDismissedHandler() - exiting function to allow purchase handler to take over.");
		}
		else if (!_isMakingPurchase && !_isShowingCouponBuyPopup)
		{
			if (!string.IsNullOrEmpty(dismissType) && (dismissType.ToLower() == "contentunittriggereddismiss" || dismissType.ToLower() == "launch"))
			{
				DebugLog("Staring ContentActionedCoroutine() coroutine");
				_contentActionedCO = StartCoroutine(ContentActionedCoroutine());
			}
			else
			{
				DebugLog("ContentDismissedHandler() calling GoToNextState()");
				GoToNextState();
			}
		}
	}

	private void MakePurchaseHandler(UpsightPurchase purchaseInfo)
	{
		DebugLog("MakePurchaseEventHandler()\n" + purchaseInfo);
		_isMakingPurchase = true;
		App.states.Get<IapFlowState>().productId = purchaseInfo.productIdentifier;
		App.states.Set<IapFlowState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void ContentWillDisplayHandler(string placement)
	{
		_popupIsDisplayed = true;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (base.isActiveAndEnabled && !pauseStatus)
		{
			DebugLog("OnApplicationPause() _popupIsDisplayed:" + _popupIsDisplayed + " _contentActionedCO:" + _contentActionedCO);
			if (_popupIsDisplayed && _contentActionedCO == null)
			{
				DebugLog("OnApplicationPause() calling GoToNextState()");
				GoToNextState();
			}
		}
	}

	protected void DebugLog(string str)
	{
		if (TestDefs.DEBUG_INTERSITIAL_STATE)
		{
		}
	}
}
