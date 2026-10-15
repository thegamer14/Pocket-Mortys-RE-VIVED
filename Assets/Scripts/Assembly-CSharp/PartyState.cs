using UnityEngine;

public class PartyState : StateBase
{
	private PartyUI _partyUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Party/{0}", "Party");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		if (SharedStatInfo.partyInfo == null)
		{
			SharedStatInfo.partyInfo = new PartyInfo(PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
		}
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_partyUI = _ui as PartyUI;
		MortyData startingMortyData = SharedStatInfo.partyInfo.startingMortyData;
		MortyData currentBattleMorty = ((SharedStatInfo.battleInfo == null) ? null : SharedStatInfo.battleInfo.player.currentMortyData);
		if (SharedStatInfo.partyInfo.forceSwitch)
		{
			_partyUI.SetUp(SharedStatInfo.partyInfo.mortyDatas, startingMortyData, currentBattleMorty, SharedStatInfo.partyInfo.forceSwitch);
			return;
		}
		string selectedItemID = ((SharedStatInfo.bagInfo == null) ? null : SharedStatInfo.bagInfo.selectedID);
		_partyUI.SetUp(SharedStatInfo.partyInfo.mortyDatas, startingMortyData, currentBattleMorty, SharedStatInfo.partyInfo.items, selectedItemID);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_partyUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_partyUI.MortySelectedEvent += UIMortySelectedHandler;
		_partyUI.SwitchEvent += UISwitchHandler;
		_partyUI.SwitchInBattleEvent += UISwitchInBattleHandler;
		_partyUI.UseItemInBattleEvent += UIUseItemInBattleHandler;
		_partyUI.UseItemNotYetSetEvent += UIUseItemNotYetSet;
		_partyUI.UseItemFinishedEvent += UIUseItemFinishedHandler;
		_partyUI.StatsEvent += UIStatsHandler;
		_partyUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_partyUI.MortySelectedEvent -= UIMortySelectedHandler;
		_partyUI.SwitchEvent -= UISwitchHandler;
		_partyUI.SwitchInBattleEvent -= UISwitchInBattleHandler;
		_partyUI.UseItemInBattleEvent -= UIUseItemInBattleHandler;
		_partyUI.UseItemNotYetSetEvent -= UIUseItemNotYetSet;
		_partyUI.UseItemFinishedEvent -= UIUseItemFinishedHandler;
		_partyUI.StatsEvent -= UIStatsHandler;
		_partyUI.BackEvent -= UIBackHandler;
	}

	private void UIMortySelectedHandler(MortyData mortyData)
	{
		SharedStatInfo.partyInfo.selectedMortyData = mortyData;
		SharedStatInfo.partyInfo.startingMortyData = mortyData;
	}

	private void UISwitchHandler(int index1, int index2)
	{
		SharedStatInfo.partyInfo.mortyDatas.Swap(index1, index2);
		App.saveData.Save(eSaveDataType.PLAYER);
	}

	private void UISwitchInBattleHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIUseItemNotYetSet()
	{
		SharedStatInfo.bagInfo = new BagInfo(SharedStatInfo.partyInfo.mortyDatas, SharedStatInfo.partyInfo.items, eBagFromState.WORLD, null);
		App.states.Set<BagState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIUseItemInBattleHandler()
	{
		App.states.RemoveLastBackstack();
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIUseItemFinishedHandler(bool isSuccess, bool bagStateOnComplete)
	{
		if (isSuccess)
		{
			App.saveData.Save(eSaveDataType.PLAYER);
		}
		if (bagStateOnComplete)
		{
			UIBackHandler();
		}
		else
		{
			SharedStatInfo.bagInfo = null;
		}
	}

	private void UIStatsHandler()
	{
		SharedStatInfo.mortyStatsInfo = new MortyStatsInfo(SharedStatInfo.partyInfo.selectedMortyData);
		App.states.Set<MortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.partyInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
