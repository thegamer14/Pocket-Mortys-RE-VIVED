using UnityEngine;

public class StorageEditDeckState : StateBase
{
	private StorageEditDeckUI _storagePartyEditUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/StorageEditDeck/{0}", "StorageEditDeck");
		base.showLoadingScreen = true;
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
		_storagePartyEditUI = _ui as StorageEditDeckUI;
		_storagePartyEditUI.SetUp(SharedStatInfo.storageDecksInfo.editDeckIndex, SharedStatInfo.storageDecksInfo.selectedMortyData);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_storagePartyEditUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_storagePartyEditUI.SelectedEvent += UIMortySelectedHandler;
		_storagePartyEditUI.StatsEvent += UIStatsHandler;
		_storagePartyEditUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_storagePartyEditUI.SelectedEvent -= UIMortySelectedHandler;
		_storagePartyEditUI.StatsEvent -= UIStatsHandler;
		_storagePartyEditUI.BackEvent -= UIBackHandler;
	}

	private void UIMortySelectedHandler(MortyData mortyData)
	{
		SharedStatInfo.storageDecksInfo.selectedMortyData = mortyData;
	}

	private void UIStatsHandler(MortyData mortyData)
	{
		SharedStatInfo.mortyStatsInfo = new MortyStatsInfo(mortyData);
		App.states.Set<MortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.storageDecksInfo.selectedMortyData = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		App.saveData.Save(eSaveDataType.PLAYER);
	}
}
