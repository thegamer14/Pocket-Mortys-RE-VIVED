using UnityEngine;

public class CreditsState : StateBase
{
	private CreditsUI _creditsUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Credits/{0}", "Credits");
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
		_creditsUI = _ui as CreditsUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_creditsUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_creditsUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_creditsUI.BackEvent -= UIBackHandler;
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
