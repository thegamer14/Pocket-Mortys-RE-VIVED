using UnityEngine;

public class MortydexState : StateBase
{
	private MortydexUI _mortydexUI;

	private string _lastMortyIDSelected;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Mortydex/{0}", "Mortydex");
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
		_mortydexUI = _ui as MortydexUI;
		_mortydexUI.SetUp(_lastMortyIDSelected);
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
		_lastMortyIDSelected = id;
		SharedStatInfo.mortydexEntryInfo = new MortydexEntryInfo(id);
		App.states.Set<MortydexEntryState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		_lastMortyIDSelected = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
