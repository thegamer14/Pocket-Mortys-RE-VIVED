using UnityEngine;

public class MPSalesmanRickCustomizationState : StateBase
{
	private MPSalesmanRickCustomizationUI _customizationUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPSalesmanRickCustomization/{0}", "MPSalesmanRickCustomization");
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
		_customizationUI = _ui as MPSalesmanRickCustomizationUI;
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
		App.net.player.data.CoinsChangedEvent += CoinCountChangedHandler;
		App.net.player.data.CouponsChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_customizationUI.BuyEvent -= UIBuyHandler;
		_customizationUI.UseEvent -= UIUseHandler;
		_customizationUI.BackEvent -= UIBackHandler;
		_customizationUI.GetCoinsEvent -= UIGetCoinsHandler;
		_customizationUI.GetCouponsEvent -= UIGetCouponsHandler;
		App.net.player.data.CoinsChangedEvent -= CoinCountChangedHandler;
		App.net.player.data.CouponsChangedEvent -= CouponCountChangedHandler;
	}

	private void UIBuyHandler(string id)
	{
		if (MPPlayerAvatarDefs.CurrencyType(id) == eCurrencyType.COIN && MPPlayerAvatarDefs.Cost(id) > App.net.player.data.coins)
		{
			_customizationUI.DisplayCantAfford(eCurrencyType.COIN);
			return;
		}
		if (MPPlayerAvatarDefs.CurrencyType(id) == eCurrencyType.COUPON && MPPlayerAvatarDefs.Cost(id) > App.net.player.data.coupons)
		{
			_customizationUI.DisplayCantAfford(eCurrencyType.COUPON);
			return;
		}
		_customizationUI.DisplayWaiting();
		App.net.player.BuyAvatar(id, delegate(eBuyAvatarError error)
		{
			if (error == eBuyAvatarError.NONE)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SINK, id, MPPlayerAvatarDefs.Cost(id));
				App.sound.PlaySFX("Audio/SFX/Coins");
				_customizationUI.DisplayPurchaseComplete();
			}
			else
			{
				_customizationUI.DisplayBuyError();
			}
		});
	}

	private void UIUseHandler(string id)
	{
		_customizationUI.DisplayWaiting();
		if (App.net.player.data.ownedAvatars.Contains(id))
		{
			App.net.player.UseAvatar(id, delegate(eUseAvatarError error)
			{
				if (error == eUseAvatarError.NONE)
				{
					_customizationUI.DisplayPurchaseComplete();
				}
				else
				{
					_customizationUI.DisplayUseError();
				}
			});
			return;
		}
		App.net.player.BuyAvatar(id, delegate(eBuyAvatarError error)
		{
			if (error == eBuyAvatarError.NONE)
			{
				_customizationUI.DisplayPurchaseComplete();
			}
			else
			{
				_customizationUI.DisplayUseError();
			}
		});
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIGetCoinsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.STORE_MENU, "get_schmeckles");
		App.states.Set<MPVideoRewardState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIGetCouponsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.PLAYER_CUSTOMIZE, "view_IAP");
		App.states.Set<MPCouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
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
