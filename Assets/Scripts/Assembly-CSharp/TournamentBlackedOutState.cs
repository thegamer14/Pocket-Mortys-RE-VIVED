using UnityEngine;

public class TournamentBlackedOutState : StateBase
{
	private TournamentBlackedOutUI _tournamentBlackOutUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TournamentBlackedOut/{0}", "TournamentBlackedOut");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.upsight.CustomAnalyticEvent(eSubType1.TOURNAMENTS, eSubType2.FAILED, TournamentDefs.currentEventID);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_tournamentBlackOutUI = _ui as TournamentBlackedOutUI;
		_tournamentBlackOutUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_tournamentBlackOutUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_tournamentBlackOutUI.FinishedEvent += UIFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_tournamentBlackOutUI.FinishedEvent -= UIFinishedHandler;
	}

	private void UIFinishedHandler()
	{
		TournamentDefs.EndEvent();
		SharedStatInfo.worldInfo.worldID = "TournamentLobby";
		SharedStatInfo.worldInfo.entranceID = "TournamentLobbyToiletPortal";
		App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
		App.states.Get<InterstitialState>().nextStateType = typeof(WorldState);
		App.states.Set<InterstitialState>(null, new FadeTransInfo());
	}
}
