using UnityEngine;

public class SplashState : StateBase
{
	private SplashUI _splashUI;

	private bool showASG;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Splash/{0}", "Splash");
		showASG = true;
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
		_splashUI = _ui as SplashUI;
		_splashUI.SetUp(showASG);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_splashUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_splashUI.FinishedEvent += UIFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_splashUI.FinishedEvent -= UIFinishedHandler;
	}

	private void UIFinishedHandler()
	{
		showASG = !showASG;
		if (!showASG)
		{
			App.states.Set<SplashState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			App.states.Set<SaveConflictState>(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
