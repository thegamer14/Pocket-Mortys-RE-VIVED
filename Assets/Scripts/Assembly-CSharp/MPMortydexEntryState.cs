using UnityEngine;

public class MPMortydexEntryState : StateBase
{
	private MPMortydexEntryUI _mortydexEntryUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPMortydexEntry/{0}", "MPMortydexEntry");
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
		_mortydexEntryUI = _ui as MPMortydexEntryUI;
		_mortydexEntryUI.SetUp(MPSharedStateInfo.mortydexEntryInfo.selectedMortyId);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mortydexEntryUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (MPSharedStateInfo.mortydexEntryInfo.IfOrigin(eMPStateOrigin.BATTLE) && App.states.Get<MPBattleState>().waitingToPlayTurnActDatas)
		{
			NetBattleComeBackHandler();
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mortydexEntryUI.BackEvent += UIBackHandler;
		App.net.battle.TurnResultEvent += NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent += NetBattleComeBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mortydexEntryUI.BackEvent -= UIBackHandler;
		App.net.battle.TurnResultEvent -= NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent -= NetBattleComeBackHandler;
	}

	private void UIBackHandler()
	{
		MPSharedStateInfo.mortydexEntryInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void NetBattleComeBackHandler()
	{
		if (IsActiveAndNotTransitioning())
		{
			App.states.RemoveLastBackstack();
			App.states.RemoveLastBackstack();
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
