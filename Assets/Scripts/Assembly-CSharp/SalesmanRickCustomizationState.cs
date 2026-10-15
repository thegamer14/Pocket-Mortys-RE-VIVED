using UnityEngine;

public class SalesmanRickCustomizationState : StateBase
{
	private SalesmanRickCustomizationUI _customizationUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/SalesmanRickCustomization/{0}", "SalesmanRickCustomization");
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
		_customizationUI = _ui as SalesmanRickCustomizationUI;
		_customizationUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_customizationUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_customizationUI.BuyEvent += UIBuyHandler;
		_customizationUI.UseEvent += UIUseHandler;
		_customizationUI.BackEvent += UIBackHandler;
		_customizationUI.GetCoinsEvent += UIGetCoinsHandler;
		_customizationUI.GetCouponsEvent += UIGetCouponsHandler;
		PlayerDefs.CoinCountChangedEvent += CoinCountChangedHandler;
		PlayerDefs.CouponCountChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_customizationUI.BuyEvent -= UIBuyHandler;
		_customizationUI.UseEvent -= UIUseHandler;
		_customizationUI.BackEvent -= UIBackHandler;
		_customizationUI.GetCoinsEvent -= UIGetCoinsHandler;
		_customizationUI.GetCouponsEvent -= UIGetCouponsHandler;
		PlayerDefs.CoinCountChangedEvent -= CoinCountChangedHandler;
		PlayerDefs.CouponCountChangedEvent -= CouponCountChangedHandler;
	}

	private void UIBuyHandler(string id)
	{
		string resultText = null;
		bool flag = PlayerDefs.PurchaseAvatar(id, out resultText);
		if (flag)
		{
			App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SINK, id, PlayerAvatarDefs.Cost(id));
			App.sound.PlaySFX("Audio/SFX/Coins");
			resultText = string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_USED, PlayerAvatarDefs.Name(id));
			PlayerDefs.AvatarID = id;
			_customizationUI.AvatarSet(PlayerDefs.AvatarID);
			App.saveData.Save(eSaveDataType.PLAYER);
		}
		_customizationUI.PurchaseComplete(flag, resultText, id);
	}

	private void UIUseHandler(string id)
	{
		PlayerDefs.AvatarID = id;
		App.saveData.Save(eSaveDataType.PLAYER);
		_customizationUI.AvatarSet(id);
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetCoinsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.STORE_MENU, "get_schmeckles");
		App.states.Set<VideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIGetCouponsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PLAYER_CUSTOMIZE, "view_IAP");
		App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void CoinCountChangedHandler(int count)
	{
		_customizationUI.TweenCoins(count);
	}

	private void CouponCountChangedHandler(int count)
	{
		_customizationUI.TweenCoupons(count);
	}
}
