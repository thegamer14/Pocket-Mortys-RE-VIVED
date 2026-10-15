using UnityEngine;

public class MPBagState : StateBase
{
	private MPBagUI _bagUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPBag/{0}", "MPBag");
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
		_bagUI = _ui as MPBagUI;
		_bagUI.SetUp(MPSharedStateInfo.bagInfo.items, MPSharedStateInfo.bagInfo.selectedItemId, MPSharedStateInfo.bagInfo.selectedMortyData);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_bagUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.BATTLE) && App.states.Get<MPBattleState>().waitingToPlayTurnActDatas)
		{
			NetBattleComeBackHandler();
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_bagUI.ItemSelectedEvent += UIItemSelectedHandler;
		_bagUI.UseEvent += UIUseHandler;
		_bagUI.BackEvent += UIBackHandler;
		App.net.battle.TurnResultEvent += NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent += NetBattleComeBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_bagUI.ItemSelectedEvent -= UIItemSelectedHandler;
		_bagUI.UseEvent -= UIUseHandler;
		_bagUI.BackEvent -= UIBackHandler;
		App.net.battle.TurnResultEvent -= NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent -= NetBattleComeBackHandler;
	}

	private void SetBackToState()
	{
		MPSharedStateInfo.bagInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIItemSelectedHandler(string id)
	{
		MPSharedStateInfo.bagInfo.selectedItemId = id;
	}

	private void UIUseHandler()
	{
		if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.CRAFT))
		{
			if (MPSharedStateInfo.craftInfo != null)
			{
				MPSharedStateInfo.craftInfo.SetSelectedSlot(MPSharedStateInfo.bagInfo.selectedItemId);
			}
			SetBackToState();
		}
		else if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.PARTY))
		{
			App.net.player.UseItem(MPSharedStateInfo.bagInfo.selectedMortyData.netId, MPSharedStateInfo.bagInfo.selectedItemId);
			MPSharedStateInfo.partyInfo.selectedItemId = MPSharedStateInfo.bagInfo.selectedItemId;
			SetBackToState();
		}
		else if (MPItemDefs.UseOnSelf(MPSharedStateInfo.bagInfo.selectedItemId))
		{
			if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.BATTLE))
			{
				MPSharedStateInfo.partyInfo = new MPPartyInfo(MPSharedStateInfo.bagInfo.origin | eMPStateOrigin.BAG, MPSharedStateInfo.battleInfo.player.currentMortyData, MPSharedStateInfo.bagInfo.selectedItemId);
				App.states.Set<MPPartyState>(new FadeTransInfo(), new FadeTransInfo(), true);
			}
			else
			{
				MPSharedStateInfo.partyInfo = new MPPartyInfo(MPSharedStateInfo.bagInfo.origin | eMPStateOrigin.BAG, null, MPSharedStateInfo.bagInfo.selectedItemId);
				App.states.Set<MPPartyState>(new FadeTransInfo(), new FadeTransInfo(), true);
			}
		}
		else
		{
			if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.BATTLE))
			{
				App.net.battle.SelectMove(eMPBattleMoveType.ITEM, MPSharedStateInfo.battleInfo.player.currentMortyData.netId, MPSharedStateInfo.bagInfo.selectedItemId);
			}
			SetBackToState();
		}
	}

	private void UIBackHandler()
	{
		SetBackToState();
	}

	private void NetBattleComeBackHandler()
	{
		if (IsActiveAndNotTransitioning())
		{
			SetBackToState();
		}
	}
}
