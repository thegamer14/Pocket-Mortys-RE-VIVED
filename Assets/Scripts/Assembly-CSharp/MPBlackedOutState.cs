using UnityEngine;

public class MPBlackedOutState : StateBase
{
	private MPBlackedOutUI _blackOutUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPBlackedOut/{0}", "MPBlackedOut");
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
		_blackOutUI = _ui as MPBlackedOutUI;
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
		MPSharedStateInfo.healingCenterInfo = new MPHealingCenterInfo(eMPStateOrigin.NONE, true);
		App.states.Set<MPHealingCenterState>(new FadeTransInfo(), new FadeTransInfo());
	}
}
