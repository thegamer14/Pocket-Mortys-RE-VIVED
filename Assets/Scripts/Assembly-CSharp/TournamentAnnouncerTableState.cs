using UnityEngine;

public class TournamentAnnouncerTableState : StateBase
{
	private TournamentAnnouncerTableUI _announcerTableUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/TournamentAnnouncerTable/{0}", "TournamentAnnouncerTable");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_announcerTableUI = _ui as TournamentAnnouncerTableUI;
		_announcerTableUI.SetUp(TournamentDefs.currentTrainerID);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_announcerTableUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_announcerTableUI.ContinueEvent += UIContinueHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_announcerTableUI.ContinueEvent -= UIContinueHandler;
	}

	private void UIContinueHandler()
	{
		string nextTrainerID = TournamentDefs.GetNextTrainerID(TournamentDefs.currentEventID, TournamentDefs.currentTrainerID);
		TournamentDefs.SetNextTrainer(nextTrainerID);
		App.states.Set<TournamentArenaState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
	}
}
