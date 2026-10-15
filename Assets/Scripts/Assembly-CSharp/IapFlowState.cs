using UnityEngine;

public class IapFlowState : GenericPopupState
{
	public string productId;

	private bool _success;

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		SetDisplayValues(TextDefs.IAP_CONNECTING_TO_STORE, null, false, true);
		if (!string.IsNullOrEmpty(productId))
		{
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		App.iap.PurchaseSuccessfulEvent += PurchaseSuccessfulHandler;
		App.iap.PurchaseFailedEvent += PurchaseFailedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		App.iap.PurchaseSuccessfulEvent -= PurchaseSuccessfulHandler;
		App.iap.PurchaseFailedEvent -= PurchaseFailedHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		App.iap.PurchaseIAP(productId);
	}

	private void PurchaseFailedHandler(string productId, string error, string googleOrderId, bool didCancel)
	{
		App.iap.PurchaseSuccessfulEvent -= PurchaseSuccessfulHandler;
		App.iap.PurchaseFailedEvent -= PurchaseFailedHandler;
		_success = false;
		SetDisplayValues(TextDefs.IAP_PURCHASE_FAILED, null);
	}

	private void PurchaseSuccessfulHandler(string productId, string token)
	{
		App.iap.PurchaseSuccessfulEvent -= PurchaseSuccessfulHandler;
		App.iap.PurchaseFailedEvent -= PurchaseFailedHandler;
		_success = true;
		SetDisplayValues(TextDefs.IAP_PURCHASE_SUCCESS, null);
	}

	protected override void ButtonClickedHandler(int buttonIndex)
	{
		if (!_success && (productId == "com.adultswim.pocketmorties.coupon01" || productId == "com.adultswim.pocketmorties.coupon05" || productId == "com.adultswim.pocketmorties.coupon15" || productId == "com.adultswim.pocketmorties.coupon40"))
		{
			App.states.Set<CouponBuyPopupState>(new PopTransInfo(), new PopTransInfo());
		}
		else
		{
			App.states.SetBack(new PopTransInfo());
		}
	}
}
