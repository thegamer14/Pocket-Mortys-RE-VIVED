using UnityEngine;

public class MortydexEntryState : StateBase
{
	private MortydexEntryUI _mortydexEntryUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MortydexEntry/{0}", "MortydexEntry");
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
		_mortydexEntryUI = _ui as MortydexEntryUI;
		_mortydexEntryUI.SetUp(SharedStatInfo.mortydexEntryInfo.selectedMortyID);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortydexEntryUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortydexEntryUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortydexEntryUI.BackEvent -= UIBackHandler;
	}

	private void UIBackHandler()
	{
		SharedStatInfo.mortydexEntryInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
