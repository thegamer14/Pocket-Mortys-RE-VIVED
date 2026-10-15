using UnityEngine;

public class BadgeCollectionState : StateBase
{
	private BadgeCollectionUI _badgeCollectionUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/BadgeCollection/{0}", "BadgeCollection");
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
		_badgeCollectionUI = _ui as BadgeCollectionUI;
		_badgeCollectionUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_badgeCollectionUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_badgeCollectionUI.BackEvent += UIBackHandler;
		_badgeCollectionUI.TrophiesEvent += UITrophiesHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_badgeCollectionUI.BackEvent -= UIBackHandler;
		_badgeCollectionUI.TrophiesEvent -= UITrophiesHandler;
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UITrophiesHandler()
	{
		App.states.Set<TrophyCollectionState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}
}
