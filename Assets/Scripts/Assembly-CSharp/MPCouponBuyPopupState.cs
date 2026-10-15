using UnityEngine;

public class MPCouponBuyPopupState : StateBase
{
	private MPCouponBuyPopupUI _couponBuyUI;

	private bool _requestedSignIn;

	public override void Awake()
	{
		base.Awake();
		base.hideStateBellow = false;
		_useBackgroundFade = true;
		_uiResourcePath = string.Format("UI/MPCouponBuyPopup/{0}", "MPCouponBuyPopup");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_requestedSignIn = false;
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_couponBuyUI = _ui as MPCouponBuyPopupUI;
		App.iap.LoadProducts();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_couponBuyUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (!App.iap.billingSupported && !_requestedSignIn)
		{
			App.states.Get<GenericPopupState>().SetDisplayValues(TextDefs.IAP_SIGN_IN_TO_GOOGLE_PLAY, new string[0]);
			App.states.Get<GenericPopupState>().ButtonClickedEvent += SignInPopupButtonClickedHandler;
			App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
			_requestedSignIn = true;
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		App.social.AuthenticateEvent += HandleAuthenticateEvent;
		_couponBuyUI.backClickedEvent += backClickedHandler;
		_couponBuyUI.ProductClickedEvent += ProductClickedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		App.social.AuthenticateEvent -= HandleAuthenticateEvent;
		_couponBuyUI.backClickedEvent -= backClickedHandler;
		_couponBuyUI.ProductClickedEvent -= ProductClickedHandler;
	}

	protected void HandleAuthenticateEvent(bool success)
	{
		if (success && !App.iap.billingSupported)
		{
			App.iap.InitGoogleIAB();
		}
	}

	protected virtual void backClickedHandler()
	{
		App.states.SetBack(new PopTransInfo());
	}

	protected virtual void ProductClickedHandler(string productId)
	{
		App.states.Get<IapFlowState>().productId = productId;
		App.states.Set<IapFlowState>(new PopTransInfo(), new PopTransInfo());
	}

	protected void SignInPopupButtonClickedHandler(int buttonIndex)
	{
		App.states.Get<GenericPopupState>().ButtonClickedEvent -= SignInPopupButtonClickedHandler;
		App.states.SetBack(new PopTransInfo());
		App.social.Authenticate();
	}
}
