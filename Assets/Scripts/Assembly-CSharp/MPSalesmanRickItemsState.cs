using UnityEngine;

public class MPSalesmanRickItemsState : StateBase
{
	private MPSalesmanRickItemsUI _salesmanRickUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPSalesmanRickItems/{0}", "MPSalesmanRickItems");
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
		_salesmanRickUI = _ui as MPSalesmanRickItemsUI;
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
		_salesmanRickUI.BlipsAndChitzSeletedEvent += UIBlipsAndChitzSelectedHandler;
		_salesmanRickUI.BuyEvent += UIBagBuyHandler;
		_salesmanRickUI.BackEvent += UIBagBackHandler;
		_salesmanRickUI.GetCoinsEvent += UIGetCoinsHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_salesmanRickUI.BlipsAndChitzSeletedEvent -= UIBlipsAndChitzSelectedHandler;
		_salesmanRickUI.BuyEvent -= UIBagBuyHandler;
		_salesmanRickUI.BackEvent -= UIBagBackHandler;
		_salesmanRickUI.GetCoinsEvent -= UIGetCoinsHandler;
	}

	private void UIBlipsAndChitzSelectedHandler()
	{
		App.states.Set<MPCouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBagBuyHandler(string id)
	{
		if (App.net.player.data.coins < MPItemDefs.Cost(id))
		{
			_salesmanRickUI.DisplayNotEnoughCoin(id);
			return;
		}
		if (App.net.player.data.items.ContainsKey(id) && App.net.player.data.items[id] >= 10)
		{
			_salesmanRickUI.DisplayNotEnoughRoom(id);
			return;
		}
		_salesmanRickUI.DisplayWaitingToPurchase();
		App.net.player.BuyItem(id, delegate(eBuyItemError error)
		{
			if (error == eBuyItemError.NONE)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SINK, id, MPItemDefs.Cost(id));
				_salesmanRickUI.DisplayPurchaseSuccess(id);
			}
			else
			{
				_salesmanRickUI.DisplayPurchaseFailed();
			}
		});
	}

	private void UIBagBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetCoinsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.STORE_MENU, "get_schmeckles");
		App.states.Set<MPVideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}
}
