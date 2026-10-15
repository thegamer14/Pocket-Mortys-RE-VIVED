using UnityEngine;

public class CourierFlapState : StateBase
{
	private CourierFlapUI _courierFlapUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/CourierFlap/{0}", "CourierFlap");
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
		_courierFlapUI = _ui as CourierFlapUI;
		_courierFlapUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_courierFlapUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_courierFlapUI.FinishedEvent += UIFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_courierFlapUI.FinishedEvent -= UIFinishedHandler;
	}

	private void UIFinishedHandler()
	{
		SharedStatInfo.worldInfo.worldID = "Citadel";
		SharedStatInfo.worldInfo.entranceID = "CitadelCourierFlapPortal";
		App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}
}
