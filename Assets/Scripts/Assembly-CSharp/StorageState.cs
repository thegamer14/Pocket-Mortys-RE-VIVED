using UnityEngine;

public class StorageState : StateBase
{
	private StorageUI _storageUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Storage/{0}", "Storage");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Storage/RaM204m12.BBQBackground.01.08.44.10.Ver1.0.FUTZED");
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_storageUI = _ui as StorageUI;
		_storageUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storageUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storageUI.DecksEvent += UIDecksHandler;
		_storageUI.BootcampEvent += UIBootcampHandler;
		_storageUI.CombineEvent += UICombineHandler;
		_storageUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageUI.DecksEvent -= UIDecksHandler;
		_storageUI.BootcampEvent -= UIBootcampHandler;
		_storageUI.CombineEvent -= UICombineHandler;
		_storageUI.BackEvent -= UIBackHandler;
	}

	private void UIDecksHandler()
	{
		App.states.Set<StorageDecksState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBootcampHandler()
	{
		App.states.Set<StorageBootcampState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UICombineHandler()
	{
		App.states.Set<StorageCombineState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		App.saveData.Save(eSaveDataType.PLAYER);
		App.sound.StopSoundtrack(0.5f);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
