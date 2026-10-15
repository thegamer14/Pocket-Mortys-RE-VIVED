using UnityEngine;

public class BlackedOutState : StateBase
{
	private BlackedOutUI _blackOutUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/BlackedOut/{0}", "BlackedOut");
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
		_blackOutUI = _ui as BlackedOutUI;
		_blackOutUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_blackOutUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_blackOutUI.FinishedEvent += UIFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_blackOutUI.FinishedEvent -= UIFinishedHandler;
	}

	private void UIFinishedHandler()
	{
		App.states.Get<HealingCenterState>().isRevive = true;
		App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
		App.states.Get<InterstitialState>().nextStateType = typeof(HealingCenterState);
		App.states.Set<InterstitialState>(null, new FadeTransInfo());
	}
}
