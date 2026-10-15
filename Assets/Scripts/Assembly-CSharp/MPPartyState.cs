using System.Collections.Generic;
using UnityEngine;

public class MPPartyState : StateBase
{
	private MPPartyUI _partyUI;

	private MPAttacksToLearnData _attacksToLearnData;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPParty/{0}", "MPParty");
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
		_partyUI = _ui as MPPartyUI;
		_partyUI.SetUp(MPSharedStateInfo.partyInfo.selectedMortyData, MPSharedStateInfo.partyInfo.battleOutMortyData, MPSharedStateInfo.partyInfo.selectedItemId, MPSharedStateInfo.partyInfo.forceSwitch);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_partyUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BATTLE) && App.states.Get<MPBattleState>().waitingToPlayTurnActDatas)
		{
			NetBattleComeBackHandler();
		}
		else if (MPSharedStateInfo.partyInfo.serverResponse != null)
		{
			NetUserUseItemsHandler(MPSharedStateInfo.partyInfo.serverResponse, MPSharedStateInfo.partyInfo.serverError);
		}
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_partyUI.MortySelectedEvent += UIMortySelectedHandler;
		_partyUI.SwitchEvent += UISwitchHandler;
		_partyUI.UseItemEvent += UIItemHandler;
		_partyUI.UseItemFinishedEvent += UIUseItemFinishedHandler;
		_partyUI.StatsEvent += UIStatsHandler;
		_partyUI.BackEvent += UIBackHandler;
		App.net.player.UseItemEvent += NetUserUseItemsHandler;
		App.net.battle.TurnResultEvent += NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent += NetBattleComeBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_partyUI.MortySelectedEvent -= UIMortySelectedHandler;
		_partyUI.SwitchEvent -= UISwitchHandler;
		_partyUI.UseItemEvent -= UIItemHandler;
		_partyUI.UseItemFinishedEvent -= UIUseItemFinishedHandler;
		_partyUI.StatsEvent -= UIStatsHandler;
		_partyUI.BackEvent -= UIBackHandler;
		App.net.player.UseItemEvent -= NetUserUseItemsHandler;
		App.net.battle.TurnResultEvent -= NetBattleComeBackHandler;
		App.net.battle.AbandonedEvent -= NetBattleComeBackHandler;
	}

	private void SetBackToState()
	{
		MPSharedStateInfo.partyInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void SetBackToBattleState()
	{
		if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BAG))
		{
			App.states.RemoveLastBackstack();
		}
		MPSharedStateInfo.partyInfo = null;
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void UIMortySelectedHandler(MPOpponentMortyData mortyData)
	{
		MPSharedStateInfo.partyInfo.selectedMortyData = mortyData;
	}

	private void UISwitchHandler(MPOpponentMortyData mortyData1, MPOpponentMortyData mortyData2)
	{
		if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			App.net.battle.SelectMove(eMPBattleMoveType.SWITCH, mortyData1.netId, mortyData2.netId);
			SetBackToBattleState();
		}
		else if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.WORLD))
		{
			StartCoroutine(_partyUI.SwitchOrder(mortyData1, mortyData2));
			List<string> list = new List<string>(App.net.player.data.GetActiveDeckIDs());
			int indexA = list.IndexOf(mortyData1.netId);
			int indexB = list.IndexOf(mortyData2.netId);
			list.Swap(indexA, indexB);
			App.net.player.EditDeck(App.net.player.data.activeDeckIndex, list, SwitchMortiesCallback);
		}
	}

	private void UIItemHandler()
	{
		MPOpponentMortyData selectedMortyData = MPSharedStateInfo.partyInfo.selectedMortyData;
		if (MPSharedStateInfo.partyInfo.selectedItemId != null)
		{
			if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BATTLE))
			{
				App.net.battle.SelectMove(eMPBattleMoveType.ITEM, selectedMortyData.netId, MPSharedStateInfo.partyInfo.selectedItemId);
				SetBackToBattleState();
			}
			else if (MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.WORLD))
			{
				App.net.player.UseItem(selectedMortyData.netId, MPSharedStateInfo.partyInfo.selectedItemId);
			}
		}
		else
		{
			MPSharedStateInfo.bagInfo = new MPBagInfo(MPSharedStateInfo.partyInfo.origin | eMPStateOrigin.PARTY, App.net.player.data.items, selectedMortyData);
			App.states.Set<MPBagState>(new FadeTransInfo(), new FadeTransInfo(), true);
		}
	}

	private void UIUseItemFinishedHandler()
	{
		bool flag = MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BAG);
		if (_attacksToLearnData != null && _attacksToLearnData.morties.Count > 0)
		{
			MPSharedStateInfo.learnAttacksInfo = new MPLearnAttacksInfo(MPSharedStateInfo.partyInfo.origin | eMPStateOrigin.PARTY, MPSharedStateInfo.worldInfo.worldId, _attacksToLearnData);
			if (flag)
			{
				MPSharedStateInfo.partyInfo = null;
			}
			App.states.Set<MPLearnAttacksState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			}, !flag);
		}
		else if (flag)
		{
			SetBackToState();
		}
		else
		{
			_partyUI.UseItemFinished();
		}
		_attacksToLearnData = null;
	}

	private void UIStatsHandler()
	{
		bool allowRelease = !MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BATTLE);
		MPSharedStateInfo.mortyStatsInfo = new MPMortyStatsInfo(MPSharedStateInfo.partyInfo.origin | eMPStateOrigin.PARTY, MPSharedStateInfo.partyInfo.selectedMortyData, allowRelease);
		App.states.Set<MPMortyStatsState>(new FadeTransInfo(), new FadeTransInfo(), true);
	}

	private void UIBackHandler()
	{
		SetBackToState();
	}

	private void SwitchMortiesCallback(eEditDeckError error)
	{
		_partyUI.SetUpMortyScrollMenu(MPSharedStateInfo.partyInfo.battleOutMortyData);
		_partyUI.EndSwitchingOrderMode();
	}

	private void NetUserUseItemsHandler(PlayerUseItemResponse responseData, eUseItemError error)
	{
		if (IsTransitioning() || base.status != eStateStatus.ACTIVE)
		{
			return;
		}
		App.net.player.data.SetMortiesFromDicts(responseData.owned_morties);
		App.net.player.data.SetItemsFromDicts(responseData.owned_items);
		MPSharedStateInfo.partyInfo.serverError = eUseItemError.NONE;
		MPSharedStateInfo.partyInfo.serverResponse = null;
		MPSharedStateInfo.partyInfo.selectedItemId = null;
		if (error == eUseItemError.NONE && responseData != null)
		{
			if (responseData.attacks_to_learn != null)
			{
				_attacksToLearnData = new MPAttacksToLearnData(responseData.attacks_to_learn);
			}
			_partyUI.UseItem(App.net.player.data.GetMortyData(responseData.owned_morty_id), responseData.item_id, responseData.is_success, responseData.effect_value);
		}
		else
		{
			StartCoroutine(_partyUI.UseItemFail(!MPSharedStateInfo.partyInfo.IfOrigin(eMPStateOrigin.BAG)));
		}
	}

	private void NetBattleComeBackHandler()
	{
		if (IsActiveAndNotTransitioning())
		{
			SetBackToBattleState();
		}
	}
}
