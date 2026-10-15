using UnityEngine;

public class TrophyCollectionState : StateBase
{
	private TrophyCollectionUI _trophyCollectionUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TrophyCollection/{0}", "TrophyCollection");
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
		_trophyCollectionUI = _ui as TrophyCollectionUI;
		_trophyCollectionUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_trophyCollectionUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_trophyCollectionUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_trophyCollectionUI.BackEvent -= UIBackHandler;
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
