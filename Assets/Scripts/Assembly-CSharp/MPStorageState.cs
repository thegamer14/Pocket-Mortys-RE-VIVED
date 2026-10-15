using UnityEngine;

public class MPStorageState : StateBase
{
	private MPStorageUI _storageUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPStorage/{0}", "MPStorage");
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
		_storageUI = _ui as MPStorageUI;
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
		_storageUI.CombineEvent += UICombineHandler;
		_storageUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageUI.DecksEvent -= UIDecksHandler;
		_storageUI.CombineEvent -= UICombineHandler;
		_storageUI.BackEvent -= UIBackHandler;
	}

	private void UIDecksHandler()
	{
		App.states.Set<MPStorageDecksState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UICombineHandler()
	{
		App.states.Set<MPStorageCombineState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		App.saveData.Save(eSaveDataType.PLAYER);
		App.sound.StopSoundtrack(0.5f);
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
