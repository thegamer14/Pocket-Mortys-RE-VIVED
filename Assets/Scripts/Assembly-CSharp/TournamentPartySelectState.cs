using System.Collections.Generic;
using UnityEngine;

public class TournamentPartySelectState : StateBase
{
	private TournamentPartySelectUI _partySelectUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TournamentPartySelect/{0}", "TournamentPartySelect");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/Store/RaM202m01.HumanMuzak.01005513.Ver1.0.Clean", 0.6f);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_partySelectUI = _ui as TournamentPartySelectUI;
		_partySelectUI.SetUp(SharedStatInfo.tournamentSetUpInfo.eventID);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_partySelectUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_partySelectUI.ContinueEvent += UIContinueHandler;
		_partySelectUI.StatsEvent += UIStatsBtnHandler;
		_partySelectUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_partySelectUI.ContinueEvent -= UIContinueHandler;
		_partySelectUI.StatsEvent -= UIStatsBtnHandler;
		_partySelectUI.BackEvent -= UIBackHandler;
	}

	private void UIContinueHandler(List<KeyValuePair<MortyData, int>> selectedMorties, List<KeyValuePair<string, int>> selectedItems)
	{
		TournamentDefs.StartEvent(SharedStatInfo.tournamentSetUpInfo.tournamentID, SharedStatInfo.tournamentSetUpInfo.eventID, selectedMorties, selectedItems);
		SharedStatInfo.tournamentSetUpInfo = null;
		App.states.Set<TournamentArenaState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}

	public void UIStatsBtnHandler(MortyData mortyData)
	{
		SharedStatInfo.mortyStatsInfo = new MortyStatsInfo(mortyData);
		App.states.Set<MortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIBackHandler()
	{
		SharedStatInfo.tournamentSetUpInfo = null;
		SharedStatInfo.worldInfo.worldID = "TournamentLobby";
		SharedStatInfo.worldInfo.entranceID = "TournamentLobbyOrganiserPortal";
		App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}
}
