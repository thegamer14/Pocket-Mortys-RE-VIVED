using System;
using UnityEngine;

public class CouponUseState : StateBase
{
	private CouponUseUI _couponUseUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/CouponUse/{0}", "CouponUse");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/GachaMachine/RaM202m05.BlipsandChitz.01031922.Ver1.0", 1f, 0f, false);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0.5f);
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		_couponUseUI.RewardFinished();
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_couponUseUI = _ui as CouponUseUI;
		_couponUseUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_couponUseUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		CouponUseUI couponUseUI = _couponUseUI;
		couponUseUI.UseEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI.UseEvent, new CouponUseUI.UICouponUseBtnDelegate(UIUseHandler));
		CouponUseUI couponUseUI2 = _couponUseUI;
		couponUseUI2.BuyCouponsEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI2.BuyCouponsEvent, new CouponUseUI.UICouponUseBtnDelegate(UIBuyCouponsHandler));
		CouponUseUI couponUseUI3 = _couponUseUI;
		couponUseUI3.BackEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI3.BackEvent, new CouponUseUI.UICouponUseBtnDelegate(UIBackHandler));
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		CouponUseUI couponUseUI = _couponUseUI;
		couponUseUI.UseEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI.UseEvent, new CouponUseUI.UICouponUseBtnDelegate(UIUseHandler));
		CouponUseUI couponUseUI2 = _couponUseUI;
		couponUseUI2.BuyCouponsEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI2.BuyCouponsEvent, new CouponUseUI.UICouponUseBtnDelegate(UIBuyCouponsHandler));
		CouponUseUI couponUseUI3 = _couponUseUI;
		couponUseUI3.BackEvent = (CouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI3.BackEvent, new CouponUseUI.UICouponUseBtnDelegate(UIBackHandler));
	}

	private void UIUseHandler()
	{
		PlayerDefs.RemoveCoupon(1);
		PlayerDefs.GachaPulled();
		App.upsight.CustomAnalyticValueEvent(eSubType1.COUPONS, eSubType2.SINK, "coupon_machine", 1);
		SharedStatInfo.rewardInfo = new RewardInfo(RewardsDefs.GetCouponReward(), "coupon_machine");
		App.states.Set<RewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true);
	}

	private void UIBuyCouponsHandler()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.FLOW, eSubType2.COUPON_MACHINE, "view_IAP");
		App.states.Set<CouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
