using System;
using UnityEngine;

public class MPCouponUseState : StateBase
{
	private MPCouponUseUI _couponUseUI;

	private MPRewardData[] _rewardDatas;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPCouponUse/{0}", "MPCouponUse");
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
		_rewardDatas = null;
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
		_couponUseUI = _ui as MPCouponUseUI;
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
		MPCouponUseUI couponUseUI = _couponUseUI;
		couponUseUI.UseEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI.UseEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIUseHandler));
		MPCouponUseUI couponUseUI2 = _couponUseUI;
		couponUseUI2.UseCoFinishedEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI2.UseCoFinishedEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIUseCoFinishedHandler));
		MPCouponUseUI couponUseUI3 = _couponUseUI;
		couponUseUI3.BuyCouponsEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI3.BuyCouponsEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIBuyCouponsHandler));
		MPCouponUseUI couponUseUI4 = _couponUseUI;
		couponUseUI4.BackEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Combine(couponUseUI4.BackEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIBackHandler));
		App.net.player.data.CouponsChangedEvent += CouponCountChangedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MPCouponUseUI couponUseUI = _couponUseUI;
		couponUseUI.UseEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI.UseEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIUseHandler));
		MPCouponUseUI couponUseUI2 = _couponUseUI;
		couponUseUI2.UseCoFinishedEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI2.UseCoFinishedEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIUseCoFinishedHandler));
		MPCouponUseUI couponUseUI3 = _couponUseUI;
		couponUseUI3.BuyCouponsEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI3.BuyCouponsEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIBuyCouponsHandler));
		MPCouponUseUI couponUseUI4 = _couponUseUI;
		couponUseUI4.BackEvent = (MPCouponUseUI.UICouponUseBtnDelegate)Delegate.Remove(couponUseUI4.BackEvent, new MPCouponUseUI.UICouponUseBtnDelegate(UIBackHandler));
		App.net.player.data.CouponsChangedEvent -= CouponCountChangedHandler;
	}

	private void UIUseHandler()
	{
		App.net.player.UseGacha(delegate(MPRewardData[] rewardDatas, eUseGachaError error)
		{
			if (error == eUseGachaError.NONE)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.COUPONS, eSubType2.SINK, "coupon_machine", 1);
				_rewardDatas = rewardDatas;
				StartCoroutine(_couponUseUI.UseCouponCo());
			}
			else
			{
				_couponUseUI.UseCouponFailed(error);
			}
		});
	}

	private void UIUseCoFinishedHandler()
	{
		MPSharedStateInfo.rewardInfo = new MPRewardInfo(eMPStateOrigin.COUPON_USE, _rewardDatas, "coupon_machine");
		App.states.Set<MPRewardState>(new FadeTransInfo
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
		App.states.Set<MPCouponBuyPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void CouponCountChangedHandler(int newValue)
	{
		_couponUseUI.UpdateCouponCount(newValue);
	}
}
