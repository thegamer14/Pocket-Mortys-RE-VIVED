using UnityEngine;

public class RestorePurchasesState : GenericPopupState
{
	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		SetDisplayValues(TextDefs.IAP_CONNECTING_TO_STORE, null, false, true);
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		App.iap.PurchasesRestoredSuccessEvent += RestoreSuccessfulHandler;
		App.iap.PurchasesRestoredFailedEvent += RestoreFailedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		App.iap.PurchasesRestoredSuccessEvent -= RestoreSuccessfulHandler;
		App.iap.PurchasesRestoredFailedEvent -= RestoreFailedHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		App.iap.RestorePurchases();
	}

	private void RestoreFailedHandler(string error)
	{
		App.iap.PurchasesRestoredSuccessEvent -= RestoreSuccessfulHandler;
		App.iap.PurchasesRestoredFailedEvent -= RestoreFailedHandler;
		SetDisplayValues(TextDefs.IAP_RESTORE_PURCHASES_FAILED, null);
	}

	private void RestoreSuccessfulHandler()
	{
		App.iap.PurchasesRestoredSuccessEvent -= RestoreSuccessfulHandler;
		App.iap.PurchasesRestoredFailedEvent -= RestoreFailedHandler;
		SetDisplayValues(TextDefs.IAP_RESTORE_PURCHASES_SUCCESS, null);
	}

	protected override void ButtonClickedHandler(int buttonIndex)
	{
		App.states.SetBack(new PopTransInfo());
	}
}
