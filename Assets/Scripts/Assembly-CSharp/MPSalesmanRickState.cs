using UnityEngine;

public class MPSalesmanRickState : StateBase
{
	private MPSalesmanRickUI _salesmanRickUI;

	public bool playStoreSoundtrack = true;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPSalesmanRick/{0}", "MPSalesmanRick");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		if (playStoreSoundtrack)
		{
			App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.8f);
		}
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_salesmanRickUI = _ui as MPSalesmanRickUI;
	}

	protected override void Destroy()
	{
		base.Destroy();
		_salesmanRickUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_salesmanRickUI.ItemsEvent += UIItemsHandler;
		_salesmanRickUI.CustomizationEvent += UICustomizationHandler;
		_salesmanRickUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_salesmanRickUI.ItemsEvent -= UIItemsHandler;
		_salesmanRickUI.CustomizationEvent -= UICustomizationHandler;
		_salesmanRickUI.BackEvent -= UIBackHandler;
	}

	private void UIItemsHandler()
	{
		App.states.Set<MPSalesmanRickItemsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UICustomizationHandler()
	{
		App.states.Set<MPSalesmanRickCustomizationState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		App.saveData.Save(eSaveDataType.PLAYER);
		if (playStoreSoundtrack)
		{
			App.sound.StopSoundtrack(0.5f);
		}
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
