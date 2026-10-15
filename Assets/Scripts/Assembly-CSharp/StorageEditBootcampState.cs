using UnityEngine;

public class StorageEditBootcampState : StateBase
{
	private StorageEditBootcampUI _storageEditBootcampUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageEditBootcamp/{0}", "StorageEditBootcamp");
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
		_storageEditBootcampUI = _ui as StorageEditBootcampUI;
		_storageEditBootcampUI.SetUp(SharedStatInfo.storageBootcampInfo.selectedBootcampIndex, SharedStatInfo.storageBootcampInfo.selectedMortyData);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storageEditBootcampUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storageEditBootcampUI.SelectedEvent += UIMortySelectedHandler;
		_storageEditBootcampUI.StatsEvent += UIStatsHandler;
		_storageEditBootcampUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storageEditBootcampUI.SelectedEvent -= UIMortySelectedHandler;
		_storageEditBootcampUI.StatsEvent -= UIStatsHandler;
		_storageEditBootcampUI.BackEvent -= UIBackHandler;
	}

	private void UIMortySelectedHandler(MortyData mortyData)
	{
		SharedStatInfo.storageBootcampInfo.selectedMortyData = mortyData;
	}

	private void UIStatsHandler(MortyData mortyData)
	{
		SharedStatInfo.mortyStatsInfo = new MortyStatsInfo(mortyData);
		App.states.Set<MortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.storageBootcampInfo.selectedMortyData = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		App.saveData.Save(eSaveDataType.PLAYER);
	}
}
