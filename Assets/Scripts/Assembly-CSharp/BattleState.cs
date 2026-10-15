using System.Collections.Generic;
using UnityEngine;

public class BattleState : StateBase
{
	public eBattleTutorialStep battleTutorialStep;

	private BattleUI _battleUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Battle/{0}", "Battle");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack(AudioDefs.ST_BATTLE_ARR[Random.Range(0, AudioDefs.ST_BATTLE_ARR.Length)], 0.55f);
		StartCoroutine(ActivateComplete());
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		if (SharedStatInfo.partyInfo != null)
		{
			if (SharedStatInfo.bagInfo != null)
			{
				_battleUI.UncachedFromBagMenu(SharedStatInfo.bagInfo.selectedID, SharedStatInfo.partyInfo.selectedMortyData);
				SharedStatInfo.bagInfo = null;
			}
			else
			{
				_battleUI.UncachedFromPartyMenu(SharedStatInfo.partyInfo.selectedMortyData);
			}
			SharedStatInfo.partyInfo = null;
		}
		_battleUI.UncacheFinished();
	}

	public override void Deactivate(TransInfo transInfo)
	{
		base.Deactivate(transInfo);
		float fadeOutVal = ((transInfo == null) ? 0.5f : transInfo.duration);
		App.sound.StopSoundtrack(fadeOutVal);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		if (battleTutorialStep != eBattleTutorialStep.NONE)
		{
			BattleTutorialUI battleTutorialUI = _uiRectTrans.gameObject.AddComponent<BattleTutorialUI>();
			battleTutorialUI.currentStep = battleTutorialStep;
			_ui = battleTutorialUI;
			battleTutorialStep = eBattleTutorialStep.NONE;
		}
		else
		{
			_ui = _uiRectTrans.gameObject.AddComponent<BattleUI>();
		}
		_ui.SetLabels();
		if (SharedStatInfo.battleInfo.enemy.type == eBattleOpponentType.AI_TRAINER && ((BattleOpponentAI)SharedStatInfo.battleInfo.enemy).trainerType == eTrainerType.GYM_LEADER)
		{
			App.upsight.CustomAnalyticEvent(eSubType1.LEVEL, eSubType2.LEVEL_PROGRESSION, "badge_level_start", PlayerDefs.BadgeCount);
		}
		_battleUI = _ui as BattleUI;
		if (PlayerDefs.GetLocationType() == eLocationType.TOURNAMENT_ARENA)
		{
			_battleUI.SetUpBackground("Tournament", "Tournament");
		}
		else
		{
			_battleUI.SetUpBackground(SharedStatInfo.worldInfo.worldID, SharedStatInfo.worldInfo.nodeSetID);
		}
		_battleUI.SetUp(SharedStatInfo.battleInfo.player, SharedStatInfo.battleInfo.enemy, SharedStatInfo.battleInfo.isLevelingActive);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_battleUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_battleUI.BattleSwitchEvent += UISwitchHandler;
		_battleUI.BattleItemEvent += UIItemHandler;
		_battleUI.BattleFinishedEvent += UIFinishedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_battleUI.BattleSwitchEvent -= UISwitchHandler;
		_battleUI.BattleItemEvent -= UIItemHandler;
		_battleUI.BattleFinishedEvent -= UIFinishedHandler;
	}

	public override void ShowUILayer()
	{
		_battleUI.ShowContainer(true);
	}

	public override void HideUILayer()
	{
		_battleUI.ShowContainer(false);
	}

	private void UISwitchHandler(MortyData currentMortyData, bool forceSwitching)
	{
		SharedStatInfo.partyInfo = new PartyInfo(SharedStatInfo.battleInfo.player.mortyDatas, SharedStatInfo.battleInfo.player.items, currentMortyData, forceSwitching);
		App.states.Set<PartyState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIItemHandler()
	{
		SharedStatInfo.bagInfo = new BagInfo(SharedStatInfo.battleInfo.player.mortyDatas, SharedStatInfo.battleInfo.player.items, eBagFromState.BATTLE, null);
		App.states.Set<BagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIFinishedHandler(eBattleOutcomeType outcomeType, MortyData lastEnemyMorty)
	{
		SharedStatInfo.battleInfo.outcomeType = outcomeType;
		App.states.Get<WorldState>().BattleFinished();
		switch (outcomeType)
		{
		case eBattleOutcomeType.PLAYER_WON:
		{
			List<RewardData> list = null;
			if (SharedStatInfo.battleInfo.node is TrainerNode)
			{
				TrainerNode trainerNode2 = (TrainerNode)SharedStatInfo.battleInfo.node;
				if (trainerNode2.trainerData.type == eTrainerType.DEFAULT || trainerNode2.data.id == "TrainerTutorial")
				{
					PlayerDefs.AddTrainersDefeated(1);
				}
				else if (trainerNode2.trainerData.type == eTrainerType.COUNCIL_MEMBER)
				{
					App.upsight.CustomAnalyticValueEvent(eSubType1.PLAY, eSubType2.COUNCIL_BATTLE_OVER, "won_council_battle", PlayerDefs.NextCouncilMember);
				}
				trainerNode2.trainerData.isDefeated = true;
				list = RewardsDefs.GetTrainerReward(trainerNode2.trainerData.id, lastEnemyMorty.level);
			}
			else if (SharedStatInfo.battleInfo.node is MortyNode)
			{
				list = RewardsDefs.GetWildMortyReward(lastEnemyMorty);
			}
			else if (SharedStatInfo.battleInfo.node is TournamentTrainerNode)
			{
				TournamentTrainerNode tournamentTrainerNode = (TournamentTrainerNode)SharedStatInfo.battleInfo.node;
				tournamentTrainerNode.trainerData.isDefeated = true;
				list = RewardsDefs.GetTournamentTrainerReward(tournamentTrainerNode.trainerData.id, lastEnemyMorty.level);
			}
			if (list != null && list.Count > 0)
			{
				SharedStatInfo.rewardInfo = new RewardInfo(list, "won_battle");
				App.states.Get<RewardState>().goToInterstitialWhenFinished = true;
				App.states.Set<RewardState>(new FadeTransInfo
				{
					duration = 0.05f,
					color = Color.white
				}, new FadeTransInfo
				{
					duration = 0.2f,
					color = Color.white
				});
				return;
			}
			break;
		}
		case eBattleOutcomeType.PLAYER_LOST:
			if (SharedStatInfo.battleInfo.node is TrainerNode)
			{
				TrainerNode trainerNode = (TrainerNode)SharedStatInfo.battleInfo.node;
				if (trainerNode.trainerData.type == eTrainerType.GYM_LEADER)
				{
					App.upsight.CustomAnalyticEvent(eSubType1.LEVEL, eSubType2.LEVEL_PROGRESSION, "badge_fail", PlayerDefs.BadgeCount);
					App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.RICK_BATTLE_OVER, "lost_rick_battle");
				}
				else if (trainerNode.trainerData.type == eTrainerType.COUNCIL_MEMBER)
				{
					App.upsight.CustomAnalyticValueEvent(eSubType1.PLAY, eSubType2.COUNCIL_BATTLE_OVER, "lost_council_battle", PlayerDefs.NextCouncilMember);
				}
			}
			SharedStatInfo.battleInfo = null;
			if (PlayerDefs.GetLocationType() == eLocationType.TOURNAMENT_ARENA)
			{
				App.states.Set<TournamentBlackedOutState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
				return;
			}
			App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
			App.states.Set<BlackedOutState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
			return;
		case eBattleOutcomeType.WILD_MORTY_CAUGHT:
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQEA");
			break;
		}
		App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
		App.states.Get<InterstitialState>().nextStateIsBack = true;
		App.states.Set<InterstitialState>(new FadeTransInfo());
	}
}
