using UnityEngine;

public class MPBattleState : StateBase
{
	private MPBattleUI _mpBattleUI;

	private bool _waitingToPlayTurnActDatas;

	private bool _moveSelectedOnActivate;

	private bool _moveSelectFailedOnActive;

	private Coroutine _battleCo;

	public bool waitingToPlayTurnActDatas
	{
		get
		{
			return _waitingToPlayTurnActDatas;
		}
	}

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPBattle/{0}", "MPBattle");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		_waitingToPlayTurnActDatas = false;
		_moveSelectedOnActivate = false;
		_moveSelectFailedOnActive = false;
		App.sound.PlaySoundtrack(AudioDefs.ST_BATTLE_ARR[Random.Range(0, AudioDefs.ST_BATTLE_ARR.Length)], 0.55f);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		if (App.net.player.data != null)
		{
			App.net.player.data.ResetPartyBattleValues();
		}
	}

	public override void Uncache(TransInfo transInInfo)
	{
		base.Uncache(transInInfo);
		if (_moveSelectedOnActivate)
		{
			_mpBattleUI.MoveSelected();
		}
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_mpBattleUI = _ui as MPBattleUI;
		_mpBattleUI.SetUp(MPSharedStateInfo.battleInfo.player, MPSharedStateInfo.battleInfo.enemy);
		_mpBattleUI.SetUpBackground(MPSharedStateInfo.battleInfo.backgroundId);
		_battleCo = StartCoroutine(_mpBattleUI.StartBattle());
	}

	protected override void DestroyCache()
	{
		base.DestroyCache();
		_mpBattleUI.ResetEmoteBubbles();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_mpBattleUI = null;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		if (_waitingToPlayTurnActDatas && _battleCo == null)
		{
			PlayTurnResults();
		}
		else if (_moveSelectFailedOnActive)
		{
			_mpBattleUI.SelectingMoveRequest();
		}
		_moveSelectedOnActivate = false;
		_moveSelectFailedOnActive = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_mpBattleUI.StartFinishedEvent += UIStartFinishedHandler;
		_mpBattleUI.SelectingMoveEvent += UISelectingMoveHandler;
		_mpBattleUI.MoveSelectedEvent += UIMoveSelectedHandler;
		_mpBattleUI.TurnFinishedEvent += UITurnFinishedHandler;
		_mpBattleUI.BattleFinishedEvent += UIBattleFinishedHandler;
		_mpBattleUI.ItemBtnEvent += UIItemBtnHandler;
		_mpBattleUI.SwitchBtnEvent += UISwitchBtnHandler;
		_mpBattleUI.RunBtnEvent += UIRunBtnHandler;
		_mpBattleUI.EmoteSelectedEvent += UIEmoteSelectedHandler;
		App.net.session.EmoteEvent += NetSessionEmoteDirectHandler;
		App.net.battle.MoveSelectedEvent += NetBattleMoveSelectedHandler;
		App.net.battle.MoveSelectFinishedEvent += NetBattleMoveSelectFinishedHandler;
		App.net.battle.TurnResultEvent += NetBattleTurnResultHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_mpBattleUI.StartFinishedEvent -= UIStartFinishedHandler;
		_mpBattleUI.SelectingMoveEvent -= UISelectingMoveHandler;
		_mpBattleUI.MoveSelectedEvent -= UIMoveSelectedHandler;
		_mpBattleUI.TurnFinishedEvent -= UITurnFinishedHandler;
		_mpBattleUI.BattleFinishedEvent -= UIBattleFinishedHandler;
		_mpBattleUI.ItemBtnEvent -= UIItemBtnHandler;
		_mpBattleUI.SwitchBtnEvent -= UISwitchBtnHandler;
		_mpBattleUI.RunBtnEvent -= UIRunBtnHandler;
		_mpBattleUI.EmoteSelectedEvent -= UIEmoteSelectedHandler;
		App.net.session.EmoteEvent -= NetSessionEmoteDirectHandler;
		App.net.battle.MoveSelectedEvent -= NetBattleMoveSelectedHandler;
		App.net.battle.MoveSelectFinishedEvent -= NetBattleMoveSelectFinishedHandler;
		App.net.battle.TurnResultEvent -= NetBattleTurnResultHandler;
	}

	public void PlayTurnResults()
	{
		_waitingToPlayTurnActDatas = false;
		_battleCo = StartCoroutine(_mpBattleUI.StartTurn(App.net.battle.turnActDatas, App.net.battle.turnPlayerData, App.net.battle.turnEnemyData));
	}

	private void UIStartFinishedHandler()
	{
		_battleCo = null;
		if (_waitingToPlayTurnActDatas)
		{
			PlayTurnResults();
		}
	}

	private void UISelectingMoveHandler()
	{
		if (App.net.battle.turnResult != eMPBattleTurnResult.ABANDONED)
		{
			App.net.battle.SelectingMove();
		}
	}

	private void UIMoveSelectedHandler(eMPBattleMoveType type, string mortyNetId, string moveId)
	{
		App.net.battle.SelectMove(type, mortyNetId, moveId);
	}

	private void UITurnFinishedHandler()
	{
		_battleCo = null;
		if (App.net.battle.turnResult == eMPBattleTurnResult.CONTINUE)
		{
			_mpBattleUI.SelectingMoveRequest();
		}
		else if (App.net.battle.turnResult == eMPBattleTurnResult.FORCE_SWITCH)
		{
			App.net.battle.SelectingMove();
			MPSharedStateInfo.partyInfo = new MPPartyInfo(eMPStateOrigin.BATTLE, MPSharedStateInfo.battleInfo.player.currentMortyData, null, true);
			App.states.Set<MPPartyState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (App.net.battle.turnResult == eMPBattleTurnResult.ASK_SWITCH)
		{
			MPOpponentMortyData partyFirstAliveMortyData = MPSharedStateInfo.battleInfo.player.opponentData.GetPartyFirstAliveMortyData();
			if (partyFirstAliveMortyData != null)
			{
				_mpBattleUI.SelectingSwitchMoveRequest();
			}
			else
			{
				App.net.battle.SelectMove(eMPBattleMoveType.NONE, MPSharedStateInfo.battleInfo.player.currentMortyData.netId, null);
			}
		}
		else
		{
			StartCoroutine(_mpBattleUI.FinishBattle(App.net.battle.turnResult));
		}
	}

	private void UIBattleFinishedHandler(eMPBattleTurnResult resultType)
	{
		switch (resultType)
		{
		case eMPBattleTurnResult.WIN:
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.BATTLE_WIN, MPTextDefs.GetTitleForWorld(MPSharedStateInfo.worldInfo.worldId));
			break;
		case eMPBattleTurnResult.LOSE:
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.BATTLE_LOSE, MPTextDefs.GetTitleForWorld(MPSharedStateInfo.worldInfo.worldId));
			break;
		case eMPBattleTurnResult.DRAW:
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.BATTLE_DRAW, MPTextDefs.GetTitleForWorld(MPSharedStateInfo.worldInfo.worldId));
			break;
		case eMPBattleTurnResult.RUN:
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.BATTLE_RUN, MPTextDefs.GetTitleForWorld(MPSharedStateInfo.worldInfo.worldId));
			break;
		case eMPBattleTurnResult.ABANDONED:
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.BATTLE_ABANDONED, MPTextDefs.GetTitleForWorld(MPSharedStateInfo.worldInfo.worldId));
			break;
		}
		if (MPSharedStateInfo.battleInfo.battleType != "PvF")
		{
			MPSharedStateInfo.worldInfo.isRevive = resultType == eMPBattleTurnResult.LOSE || resultType == eMPBattleTurnResult.DRAW;
		}
		else
		{
			MPSharedStateInfo.worldInfo.isRevive = false;
		}
		App.states.Get<MPPlayerLevelState>()._startingDialogueStr = _mpBattleUI.GetDialogueText();
		if (App.net.battle.turnPlayerData.IsAttacksToLearnStateRequired())
		{
			MPSharedStateInfo.learnAttacksInfo = new MPLearnAttacksInfo(MPSharedStateInfo.battleInfo.origin | eMPStateOrigin.BATTLE, MPSharedStateInfo.worldInfo.worldId, App.net.battle.turnPlayerData);
			MPSharedStateInfo.battleInfo = null;
			App.states.Set<MPLearnAttacksState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			});
		}
		else if (App.net.battle.turnPlayerData.IsRewardStateRequired())
		{
			MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.battleInfo.origin | eMPStateOrigin.BATTLE, App.net.battle.turnPlayerData, "battle_over");
			MPSharedStateInfo.battleInfo = null;
			App.states.Set<MPRewardState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			});
		}
		else if (App.net.battle.turnPlayerData.IsPlayerLevelStateRequired())
		{
			MPSharedStateInfo.playerLevelInfo = new MPPlayerLevelInfo(MPSharedStateInfo.battleInfo.origin | eMPStateOrigin.BATTLE, MPSharedStateInfo.worldInfo.worldId, App.net.battle.turnPlayerData);
			MPSharedStateInfo.battleInfo = null;
			App.states.Set<MPPlayerLevelState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			});
		}
		else if (App.net.battle.turnPlayerData.IsVideoRewardStateRequired())
		{
			MPSharedStateInfo.videoRewardMeeseekInfo = new MPVideoRewardMeeseekInfo(MPSharedStateInfo.battleInfo.origin | eMPStateOrigin.BATTLE, App.net.battle.turnPlayerData.videoRewardMeeseeksData);
			MPSharedStateInfo.battleInfo = null;
			App.states.Set<MPVideoRewardMeeseekState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			MPSharedStateInfo.battleInfo = null;
			App.states.Get<InterstitialState>().nextStateType = typeof(MPWorldState);
			App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
			App.states.Set<InterstitialState>(new FadeTransInfo(), null, false, eStateAction.DEACTIVATE_ALL);
		}
	}

	private void UIItemBtnHandler()
	{
		MPSharedStateInfo.bagInfo = new MPBagInfo(eMPStateOrigin.BATTLE, App.net.player.data.items);
		App.states.Set<MPBagState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UISwitchBtnHandler()
	{
		MPSharedStateInfo.partyInfo = new MPPartyInfo(eMPStateOrigin.BATTLE, MPSharedStateInfo.battleInfo.player.currentMortyData);
		App.states.Set<MPPartyState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIRunBtnHandler()
	{
		App.net.battle.SelectMove(eMPBattleMoveType.RUN, MPSharedStateInfo.battleInfo.player.currentMortyData.netId, null);
	}

	private void UIEmoteSelectedHandler(string otherUserId, string emoteId)
	{
		App.net.session.EmoteToUser(emoteId, otherUserId);
	}

	private void NetSessionEmoteDirectHandler(string emoteId, string playerId)
	{
		if (base.status == eStateStatus.ACTIVE && !IsTransitioningOut())
		{
			_mpBattleUI.DisplayEnemyEmote(emoteId, playerId);
		}
	}

	private void NetBattleMoveSelectedHandler()
	{
		if (base.status == eStateStatus.ACTIVE)
		{
			_mpBattleUI.MoveSelected();
		}
		else
		{
			_moveSelectedOnActivate = true;
		}
	}

	private void NetBattleMoveSelectFinishedHandler(eBattleMoveSelectError error)
	{
		if (error != eBattleMoveSelectError.NONE)
		{
			if (base.status == eStateStatus.ACTIVE)
			{
				_mpBattleUI.SelectingMoveRequest();
			}
			else
			{
				_moveSelectFailedOnActive = true;
			}
		}
	}

	private void NetBattleTurnResultHandler()
	{
		if (IsActiveAndNotTransitioning() && _battleCo == null)
		{
			PlayTurnResults();
		}
		else
		{
			_waitingToPlayTurnActDatas = true;
		}
	}
}
