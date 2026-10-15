using UnityEngine;

public class SalesmanRickItemsState : StateBase
{
	private SalesmanRickItemsUI _salesmanRickUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/SalesmanRickItems/{0}", "SalesmanRickItems");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_salesmanRickUI = _ui as SalesmanRickItemsUI;
		_salesmanRickUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_salesmanRickUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		App.iap.PurchaseSuccessfulEvent += IapPurchaseSuccessfulEventHandler;
		_salesmanRickUI.BlipsAndChitzSeletedEvent += UIBlipsAndChitzSelectedHandler;
		_salesmanRickUI.BuyEvent += UIBagBuyHandler;
		_salesmanRickUI.BackEvent += UIBagBackHandler;
		_salesmanRickUI.GetCoinsEvent += UIGetCoinsHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		App.iap.PurchaseSuccessfulEvent -= IapPurchaseSuccessfulEventHandler;
		_salesmanRickUI.BlipsAndChitzSeletedEvent -= UIBlipsAndChitzSelectedHandler;
		_salesmanRickUI.BuyEvent -= UIBagBuyHandler;
		_salesmanRickUI.BackEvent -= UIBagBackHandler;
		_salesmanRickUI.GetCoinsEvent -= UIGetCoinsHandler;
	}

	private void IapPurchaseSuccessfulEventHandler(string productId, string token)
	{
	}

	private void UIBlipsAndChitzSelectedHandler()
	{
		App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBagBuyHandler(string id)
	{
		string resultText = null;
		if (ItemDefs.PurchaseItem(PlayerDefs.CurrentItems(), id, out resultText))
		{
			App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SINK, id, ItemDefs.Cost(id));
			App.sound.PlaySFX("Audio/SFX/Coins");
			App.saveData.Save(eSaveDataType.PLAYER);
		}
		_salesmanRickUI.PurchaseComplete(resultText);
	}

	private void UIBagBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetCoinsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.STORE_MENU, "get_schmeckles");
		App.states.Set<VideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}
}
