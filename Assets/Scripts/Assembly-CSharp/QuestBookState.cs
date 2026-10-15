using UnityEngine;

public class QuestBookState : StateBase
{
	private QuestBookUI _questBookUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/QuestBook/{0}", "QuestBook");
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
		_questBookUI = _ui as QuestBookUI;
		_questBookUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_questBookUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_questBookUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_questBookUI.BackEvent -= UIBackHandler;
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
