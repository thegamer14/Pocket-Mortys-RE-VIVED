using UnityEngine;

public class MPMortydexState : StateBase
{
	private MPMortydexUI _mortydexUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortydex/{0}", "MPMortydex");
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
		_mortydexUI = _ui as MPMortydexUI;
		_mortydexUI.SetUp(MPSharedStateInfo.mortydexInfo.lastSelectedMortyId);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortydexUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortydexUI.AboutEvent += UIAboutHandler;
		_mortydexUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortydexUI.AboutEvent -= UIAboutHandler;
		_mortydexUI.BackEvent -= UIBackHandler;
	}

	private void UIAboutHandler(string id)
	{
		MPSharedStateInfo.mortydexInfo.lastSelectedMortyId = id;
		MPSharedStateInfo.mortydexEntryInfo = new MPMortydexEntryInfo(MPSharedStateInfo.mortydexInfo.origin | eMPStateOrigin.MORTYDEX, id);
		App.states.Set<MPMortydexEntryState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.mortydexInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
