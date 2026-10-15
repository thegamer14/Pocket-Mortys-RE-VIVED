using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : UIBase
{
	public delegate void BattleUIBtnDelegate();

	public delegate void BattleUISwitchDelegate(MortyData currentMortyData, bool forceSwitching);

	public delegate void BattleFinishedUIDelegate(eBattleOutcomeType outcomeType, MortyData lastDefeatedMorty);

	private const float _kVoPanLeft = -0.5f;

	private const float _kVoPanRight = 0.5f;

	private const string _kAttackMovePowerAnimation = "PowerSwosh";

	private const string _kAttackMoveBuffAnimation = "Buff";

	private const string _kAttackMoveDebuffAnimation = "Debuff";

	private const string _kAttackMovePoisonAnimation = "Poison";

	private const string _kAttackMoveParalyseAnimation = "Paralyse";

	private const string _kAttackMoveAbsorbAnimation = "Absorb";

	private const string _kAttackMoveFaintAnimation = "Faint";

	protected Transform _containerTransform;

	private BattleBackground _background;

	private BattleLevelUp _levelUp;

	private AttackLearnNewBox _newAttackBox;

	private BattleMoveAnimationController _moveAnimationController;

	private Transform _attackLearnContainer;

	private MenuDialogue _dialogue;

	protected MenuBase _mainMenu;

	private BattleAttackMenu _attackMenu;

	private MenuBase _switchMenu;

	private BattleOpponent _player;

	private BattleOpponent _enemy;

	private bool _isLevelingActive;

	private bool _isLevelUpSwapAttack;

	private int _levelUpSwapAttackIndex;

	private bool _waitingForPartyStateToSwitch;

	private bool _waitingForSwitchMenu;

	public event BattleUIBtnDelegate BattleItemEvent;

	public event BattleUISwitchDelegate BattleSwitchEvent;

	public event BattleFinishedUIDelegate BattleFinishedEvent;

	protected override void LateAwake()
	{
		_containerTransform = base.transform.FindChild("Container").transform;
		_background = _containerTransform.FindChild("Background").GetComponent<BattleBackground>();
		_levelUp = _containerTransform.FindChild("LevelUp").GetComponent<BattleLevelUp>();
		_moveAnimationController = _containerTransform.FindChild("MoveAnimationController").GetComponent<BattleMoveAnimationController>();
		_attackLearnContainer = base.transform.FindChild("Container/AttackLearnContainer");
		_dialogue = _containerTransform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_mainMenu = _containerTransform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_attackMenu = _containerTransform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
		_switchMenu = _containerTransform.FindChild("MenuBar/SwitchMenu").GetComponent<MenuBase>();
		_levelUpSwapAttackIndex = -1;
		_dialogue.SetText(string.Empty);
	}

	public void SetUp(BattleOpponent player, BattleOpponent enemy, bool isLevelingActive)
	{
		_player = player;
		_enemy = enemy;
		_isLevelingActive = isLevelingActive;
		_player.trainer = _containerTransform.FindChild("PlayerTrainer").GetComponent<BattleTrainer>();
		_player.morty = _containerTransform.FindChild("PlayerMorty").GetComponent<BattleMorty>();
		_player.mortyInfoBar = _containerTransform.FindChild("PlayerMortyInfoBar").GetComponent<BattleMortyInfoBar>();
		_player.mortyInfoBar.moveDirection = -1;
		_enemy.trainer = _containerTransform.FindChild("EnemyTrainer").GetComponent<BattleTrainer>();
		_enemy.morty = _containerTransform.FindChild("EnemyMorty").GetComponent<BattleMorty>();
		_enemy.mortyInfoBar = _containerTransform.FindChild("EnemyMortyInfoBar").GetComponent<BattleMortyInfoBar>();
		_enemy.mortyInfoBar.moveDirection = 1;
		_moveAnimationController.SetUp(_player.morty, _player.mortyInfoBar.transform, _enemy.morty, _enemy.mortyInfoBar.transform);
		_player.trainer.SetUp(_player.assetID, _player.side);
		_player.morty.SetUp(_player.side);
		_player.mortyInfoBar.ActivateLeveling(_isLevelingActive);
		if (_enemy.type != eBattleOpponentType.AI_WILD_MORTY)
		{
			_enemy.trainer.SetUp(_enemy.assetID, _enemy.side);
		}
		_enemy.morty.SetUp(_enemy.side);
		_enemy.mortyInfoBar.SetPartyActive(_enemy.type != eBattleOpponentType.AI_WILD_MORTY);
		_player.trainer.SetPositionOut();
		_player.morty.SetPositionOut();
		_player.mortyInfoBar.SetPositionOut();
		_enemy.trainer.SetPositionOut();
		_enemy.morty.SetPositionOut();
		_enemy.mortyInfoBar.SetPositionOut();
		_mainMenu.SetPositionOut();
		_switchMenu.SetPositionOut();
		_attackMenu.SetPositionOut();
		StartCoroutine(StartBattle());
	}

	public void SetUpBackground(string worldID, string nodeSetID)
	{
		_background.SetBackground(worldID, nodeSetID);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.BATTLE_MAIN_MENU_BTN_ATTACK);
		_mainMenu.SetBtnTitle(1, TextDefs.BATTLE_MAIN_MENU_BTN_ITEM);
		_mainMenu.SetBtnTitle(2, TextDefs.BATTLE_MAIN_MENU_BTN_SWITCH);
		_mainMenu.SetBtnTitle(3, TextDefs.BATTLE_MAIN_MENU_BTN_RUN);
		_switchMenu.SetBtnTitle(0, TextDefs.BATTLE_MAIN_MENU_BTN_SWITCH);
		_switchMenu.SetBtnTitle(1, TextDefs.BATTLE_MAIN_MENU_BTN_CONTINUE);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_attackMenu.BtnPressedEvent += AttackMenuBtnPressedHandler;
		_attackMenu.BtnLongPressStartEvent += AttackMenuBtnLongPressStartHandler;
		_switchMenu.BtnPressedEvent += SwitchMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_attackMenu.BtnPressedEvent -= AttackMenuBtnPressedHandler;
		_attackMenu.BtnLongPressStartEvent -= AttackMenuBtnLongPressStartHandler;
		_switchMenu.BtnPressedEvent -= SwitchMenuBtnPressedHandler;
	}

	public void ShowContainer(bool value)
	{
		_containerTransform.gameObject.SetActive(value);
	}

	private IEnumerator StartBattle()
	{
		yield return new WaitForEndOfFrame();
		StartCoroutine(_player.trainer.TweenInCo());
		if (_enemy.type != eBattleOpponentType.AI_WILD_MORTY)
		{
			yield return StartCoroutine(_enemy.trainer.TweenInCo());
			PlaySFXRickEngage((_enemy is BattleOpponentAI) ? ((BattleOpponentAI)_enemy).trainerType : eTrainerType.NONE);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_START_TRAINER, _enemy.name), 2f));
		}
		yield return StartCoroutine(SwitchSendMortyOut(newMortyData: _enemy.mortyDatas.Find((MortyData e) => e.hp > 0), opponent: _enemy));
		yield return StartCoroutine(SwitchSendMortyOut(newMortyData: _player.mortyDatas.Find((MortyData e) => e.hp > 0), opponent: _player));
		StartMortyIdleTweens();
		yield return StartCoroutine(ShowMainMenu());
	}

	private IEnumerator BattleEndTrainerBeaten()
	{
		SetMenusInteractable(false);
		StopMortyIdleTweens();
		PlaySFXRickWin();
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FINISHED_TRAINER, _enemy.name), 2f, true));
		BattleFinished(eBattleOutcomeType.PLAYER_WON);
	}

	private IEnumerator BattleEndWildMortyBeaten()
	{
		SetMenusInteractable(false);
		StopMortyIdleTweens();
		PlaySFXRickHappy();
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FINISHED_WILD_MORTY, MortyDefs.Name(_enemy.currentMortyData.id)), 2f, true));
		BattleFinished(eBattleOutcomeType.PLAYER_WON);
	}

	private IEnumerator BattleEndWildMortyCaptured()
	{
		SetMenusInteractable(false);
		StopMortyIdleTweens();
		yield return new WaitForSeconds(1f);
		BattleFinished(eBattleOutcomeType.WILD_MORTY_CAUGHT);
	}

	private IEnumerator BattleEndPlayerBlackOut()
	{
		SetMenusInteractable(false);
		StopMortyIdleTweens();
		PlaySFXRickLose();
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_FINISHED_LOST, 2f));
		yield return new WaitForSeconds(1.5f);
		BattleFinished(eBattleOutcomeType.PLAYER_LOST);
	}

	private IEnumerator BattleEndPlayerRan()
	{
		StartCoroutine(HideMainMenu());
		StopMortyIdleTweens();
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_RUN_FROM_WILD_MORTY, 2f));
		BattleFinished(eBattleOutcomeType.PLAYER_RAN);
	}

	private void BattleFinished(eBattleOutcomeType outcomeType)
	{
		if (outcomeType != eBattleOutcomeType.PLAYER_WON)
		{
			for (int i = 0; i < _enemy.mortyDatas.Count; i++)
			{
				_enemy.mortyDatas[i].ReviveFull();
			}
		}
		for (int j = 0; j < _player.mortyDatas.Count; j++)
		{
			_player.mortyDatas[j].ResetBattleModifiers();
		}
		if (this.BattleFinishedEvent != null)
		{
			this.BattleFinishedEvent(outcomeType, _enemy.currentMortyData);
		}
	}

	protected virtual IEnumerator StartTurn()
	{
		if (_enemy is BattleOpponentAI)
		{
			((BattleOpponentAI)_enemy).RefreshMove();
		}
		if (_player.move != null && _enemy.move != null)
		{
			if (!_player.mortiesToRewardXP.Contains(_player.currentMortyData))
			{
				_player.mortiesToRewardXP.Add(_player.currentMortyData);
			}
			_player.mortyInfoBar.TweenDownY();
			yield return StartCoroutine(HideAttackMenu());
			yield return StartCoroutine(HideMainMenu());
			if (_player.move.type != eBattleMoveType.ATTACK || (_enemy.move.type == eBattleMoveType.ATTACK && _player.currentMortyData.speed >= _enemy.currentMortyData.speed))
			{
				yield return StartCoroutine(StartMove(_player, _enemy));
				yield return StartCoroutine(StartMove(_enemy, _player));
			}
			else
			{
				yield return StartCoroutine(StartMove(_enemy, _player));
				yield return StartCoroutine(StartMove(_player, _enemy));
			}
			yield return StartCoroutine(EndTurn());
			_player.mortyInfoBar.TweenToStartingY();
			yield return StartCoroutine(ShowMainMenu());
		}
	}

	protected virtual IEnumerator EndTurn()
	{
		_player.move = null;
		_enemy.move = null;
		yield return StartCoroutine(ApplyTurnEndEffects(_enemy));
		yield return StartCoroutine(ApplyTurnEndEffects(_player));
	}

	private IEnumerator ApplyTurnEndEffects(BattleOpponent opponent)
	{
		if (opponent.currentMortyData.isPoisoned)
		{
			PlaySFXEffect(eAttackEffectType.Poison, 0f, opponent.side);
			MortyDefs.ApplyPoisonDamage(opponent.currentMortyData);
			yield return StartCoroutine(PlayMoveAnimation("Poison", opponent.side));
			yield return StartCoroutine(opponent.mortyInfoBar.SetHPWithTween(opponent.currentMortyData.hp));
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISON, opponent.pronounYours, MortyDefs.Name(opponent.currentMortyData.id)), 2f, true));
		}
		if (opponent.currentMortyData.hp == 0)
		{
			yield return StartCoroutine(FaintMorty(opponent));
			yield return StartCoroutine(SwitchMortyTrySendOutNext(opponent));
			opponent.move = null;
		}
	}

	private IEnumerator StartMove(BattleOpponent attacker, BattleOpponent defender)
	{
		if (attacker.move != null)
		{
			switch (attacker.move.type)
			{
			case eBattleMoveType.ATTACK:
				yield return StartCoroutine(PerformMoveAttack(attacker, defender));
				break;
			case eBattleMoveType.ITEM:
				yield return StartCoroutine(PerformMoveItem(attacker, defender));
				break;
			case eBattleMoveType.SWITCH:
				yield return StartCoroutine(PerformMoveSwitch(attacker));
				break;
			}
			if (defender.currentMortyData.hp == 0)
			{
				yield return StartCoroutine(FaintMorty(defender));
			}
			if (attacker.currentMortyData.hp == 0)
			{
				yield return StartCoroutine(FaintMorty(attacker));
			}
			if (defender.currentMortyData.hp == 0)
			{
				yield return StartCoroutine(SwitchMortyTrySendOutNext(defender));
				defender.move = null;
			}
			if (attacker.currentMortyData.hp == 0)
			{
				yield return StartCoroutine(SwitchMortyTrySendOutNext(attacker));
			}
			attacker.move = null;
		}
	}

	private IEnumerator PerformMoveAttack(BattleOpponent attacker, BattleOpponent defender)
	{
		AttackData attackData = attacker.move.attackData;
		if (attacker.currentMortyData.isParalysed && Random.value < 0.2f)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_USED, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id), attackData.name), 1f));
			StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSE, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
			PlaySFXEffect(eAttackEffectType.Paralyse, 0f, attacker.side);
			yield return StartCoroutine(PlayMoveAnimation("Paralyse", attacker.side));
			yield break;
		}
		if (attackData.id == "AttackStruggle")
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_PLAYER_OUT_OF_PP, MortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
		}
		int attackIndex = attacker.currentMortyData.attackIDs.IndexOf(attackData.id);
		if (attackIndex > -1)
		{
			attacker.currentMortyData.attackPPs[attackIndex]--;
		}
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_USED, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id), attackData.name), 1f));
		yield return StartCoroutine(ApplyAllAttackEffects(attacker, defender, attackData));
	}

	private IEnumerator ApplyAllAttackEffects(BattleOpponent attacker, BattleOpponent defender, AttackData attackData)
	{
		int succesfullEffects = 0;
		for (int e = 0; e < attackData.effectDatas.Length; e++)
		{
			AttackEffectData effectData = attackData.effectDatas[e];
			BattleOpponent effectDefender = ((!effectData.toSelf) ? defender : attacker);
			float accuracyValue = MortyDefs.GetAccuracyAndEvadeStepValue(attacker.currentMortyData.accuracyModifier - effectDefender.currentMortyData.evadeModifier) * effectData.accuracy;
			bool isAccuracyPass = SharedStatInfo.battleInfo.isTutorial || effectData.accuracy == -1f || Random.value < accuracyValue;
			if (isAccuracyPass)
			{
				yield return StartCoroutine(ApplyAttackEffect(attacker, effectDefender, effectData));
				succesfullEffects++;
				if (effectDefender.currentMortyData.hp <= 0 && (e + 1 >= attackData.effectDatas.Length || !attackData.effectDatas[e + 1].toSelf))
				{
					break;
				}
			}
			else if (!effectData.accuracyContinue)
			{
				break;
			}
			if (!_dialogue.IsReadDelayActive() && isAccuracyPass)
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
		if (succesfullEffects == 0)
		{
			if (attacker.type == eBattleOpponentType.PLAYER)
			{
				VoiceOverDefs.PlayRandom("Audio/VO/Morty - Worry/{0}", 0.5f, -0.5f);
			}
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MISSED, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
		}
		else if (attackData.isMultiHit)
		{
			if (attacker.move.elementModifier > 1f)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI_SUPER_EFFECTIVE, attacker.move.successfulHitsOnDefender), 2f, true));
			}
			else if (attacker.move.elementModifier < 1f)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI_NOT_VERY_EFFECTIVE, attacker.move.successfulHitsOnDefender), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI, attacker.move.successfulHitsOnDefender), 2f, true));
			}
		}
	}

	private IEnumerator ApplyAttackEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		attacker.move.lastEffectDamage = 0;
		switch (effectData.type)
		{
		case eAttackEffectType.Hit:
			PlaySFXEffect(effectData.type, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackHitEffect(attacker, defender, effectData));
			break;
		case eAttackEffectType.Stat:
			PlaySFXEffect(effectData.type, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackStatEffect(attacker, defender, effectData));
			break;
		case eAttackEffectType.Poison:
			PlaySFXEffect(effectData.type, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackPoisonEffect(attacker, defender, effectData));
			break;
		case eAttackEffectType.Paralyse:
			PlaySFXEffect(effectData.type, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackParalyseEffect(attacker, defender, effectData));
			break;
		case eAttackEffectType.Absorb:
			PlaySFXEffect(eAttackEffectType.Hit, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackHitEffect(attacker, defender, effectData));
			PlaySFXEffect(effectData.type, effectData.power, defender.side);
			yield return StartCoroutine(ApplyAttackAbsorbEffect(attacker, defender, effectData));
			break;
		}
	}

	private IEnumerator ApplyAttackHitEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		attacker.move.isCritical = attacker != defender && Random.value < 0.1f;
		attacker.move.elementModifier = MortyDefs.GetElementTypeModifier(attacker.move.attackData.element, MortyDefs.ElementType(defender.currentMortyData.id));
		int damage = MortyDefs.ApplyDamage(attacker.currentMortyData, defender.currentMortyData, effectData, ref attacker.move.isCritical, ref attacker.move.elementModifier);
		attacker.move.lastEffectDamage = damage;
		yield return StartCoroutine(PlayMoveAnimation("PowerSwosh", defender.side));
		PlaySFXMortyHurt(attacker.type);
		yield return StartCoroutine(defender.mortyInfoBar.SetHPWithTween(defender.currentMortyData.hp));
		if (attacker == defender)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_SELF_HARM, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
			yield break;
		}
		attacker.move.successfulHitsOnDefender++;
		if (!attacker.move.attackData.isMultiHit && attacker.move.elementModifier > 1f)
		{
			if (attacker.move.isCritical)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRIT_SUPER_EFFECTIVE, 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_SUPER_EFFECTIVE, 2f, true));
			}
		}
		else if (!attacker.move.attackData.isMultiHit && attacker.move.elementModifier < 1f)
		{
			if (attacker.move.isCritical)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRIT_NOT_VERY_EFFECTIVE, 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_NOT_VERY_EFFECTIVE, 2f, true));
			}
		}
		else if ((attacker.move.attackData.isMultiHit || attacker.move.elementModifier == 1f) && attacker.move.isCritical)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRITICAL, 2f, true));
		}
	}

	private IEnumerator ApplyAttackStatEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		if (MortyDefs.ApplyStatModifier(defender.currentMortyData, effectData.stat, effectData.power))
		{
			if (effectData.power > 0f)
			{
				yield return StartCoroutine(PlayMoveAnimation("Buff", defender.side));
				defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_BUFF, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id), TextDefs.GetMortyStatTextForType(effectData.stat)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(PlayMoveAnimation("Debuff", defender.side));
				defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_DEBUFF, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id), TextDefs.GetMortyStatTextForType(effectData.stat)), 2f, true));
			}
		}
	}

	private IEnumerator ApplyAttackPoisonEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		yield return StartCoroutine(PlayMoveAnimation("Poison", defender.side));
		if (!defender.currentMortyData.isPoisoned)
		{
			defender.currentMortyData.isPoisoned = true;
			defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
			if (attacker != defender)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED_SELF, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED_NO, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
		}
	}

	private IEnumerator ApplyAttackParalyseEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		yield return StartCoroutine(PlayMoveAnimation("Paralyse", defender.side));
		if (!defender.currentMortyData.isParalysed)
		{
			defender.currentMortyData.isParalysed = true;
			defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
			if (attacker != defender)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED_SELF, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED_NO, defender.pronounYours, MortyDefs.Name(defender.currentMortyData.id)), 2f, true));
		}
	}

	private IEnumerator ApplyAttackAbsorbEffect(BattleOpponent attacker, BattleOpponent defender, AttackEffectData effectData)
	{
		if (attacker.move.lastEffectDamage > 0)
		{
			int hpGain = Mathf.CeilToInt((float)attacker.move.lastEffectDamage * effectData.percent);
			int hpGained = attacker.currentMortyData.IncrementHP(hpGain);
			yield return StartCoroutine(PlayMoveAnimation("Absorb", attacker.side));
			yield return StartCoroutine(attacker.mortyInfoBar.SetHPWithTween(attacker.currentMortyData.hp));
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_ABSORB, attacker.pronounYours, MortyDefs.Name(attacker.currentMortyData.id), hpGained), 2f, true));
		}
	}

	private IEnumerator PerformMoveItem(BattleOpponent attacker, BattleOpponent defender)
	{
		string itemID = attacker.move.id;
		string resultText = null;
		bool isSuccess = ItemDefs.ApplyItemToMorty(itemID, attacker.items, attacker.move.mortyData, defender.currentMortyData, attacker.pronounYours, out resultText);
		if (attacker.type == eBattleOpponentType.PLAYER)
		{
			App.upsight.CustomAnalyticEvent(eSubType1.ITEMS, eSubType2.USE, itemID);
		}
		if (attacker.move.mortyData == attacker.currentMortyData)
		{
			if (!ItemDefs.UseOnSelf(itemID))
			{
				_dialogue.TweenText(string.Format(TextDefs.BATTLE_ITEM_USED, attacker.pronounYou, ItemDefs.Name(itemID)));
			}
			else
			{
				_dialogue.TweenText(string.Format(TextDefs.BATTLE_ITEM_USED_ON, attacker.pronounYou, ItemDefs.Name(itemID), MortyDefs.Name(attacker.move.mortyData.id)));
			}
			if (itemID.Equals("ItemMrMeeseek"))
			{
				_dialogue.TweenText(TextDefs.ITEM_APPLY_MR_MEESEEK);
			}
			PlaySFXUseItem(itemID);
			yield return StartCoroutine(PlayMoveAnimation(ItemDefs.AttackerAnimation(itemID), attacker.side));
			yield return StartCoroutine(PlayMoveAnimation(ItemDefs.DefenderAnimation(itemID), defender.side));
			switch (ItemDefs.EffectType(itemID))
			{
			case "Capture":
				yield return StartCoroutine(UseItemCapture(isSuccess, _enemy.type == eBattleOpponentType.AI_WILD_MORTY));
				break;
			case "RestoreHP":
			case "Revive":
			case "Cure":
				yield return StartCoroutine(attacker.mortyInfoBar.SetHPWithTween(attacker.move.mortyData.hp));
				break;
			case "DamageHP":
				yield return StartCoroutine(defender.mortyInfoBar.SetHPWithTween(defender.currentMortyData.hp));
				break;
			}
			attacker.mortyInfoBar.SetStatusBar(attacker.currentMortyData, true);
			defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
		}
		else
		{
			if (isSuccess)
			{
				App.sound.PlaySFX("Audio/SFX/UseItem");
			}
			if (!ItemDefs.UseOnSelf(itemID))
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_USED, attacker.pronounYou, ItemDefs.Name(itemID)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_USED_ON, attacker.pronounYou, ItemDefs.Name(itemID), MortyDefs.Name(attacker.move.mortyData.id)), 2f, true));
			}
		}
		if (resultText != null)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(resultText, 2f, true));
		}
	}

	private IEnumerator UseItemCapture(bool isSuccess, bool isWild)
	{
		if (isSuccess)
		{
			yield return StartCoroutine(PlayMoveAnimation("CaptureSuccess", _enemy.side));
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Happy/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
			_enemy.mortyInfoBar.SetCaughtIndicator(eCaughtIndicatorType.OWNED, true);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_CAPTURED, MortyDefs.Name(_enemy.currentMortyData.id)), 2f, true));
			string resultText = null;
			bool isCaught = PlayerDefs.SetMortyCaught(_enemy.currentMortyData, out resultText);
			yield return StartCoroutine(_dialogue.TweenTextCo(resultText, 2f, true));
			if (isCaught)
			{
				StopAllCoroutines();
				StartCoroutine(BattleEndWildMortyCaptured());
			}
		}
		else
		{
			yield return StartCoroutine(PlayMoveAnimation("CaptureFailed", _enemy.side));
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Moan/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ITEM_CAPTURE_FAILED, 2f, true));
		}
	}

	private IEnumerator PerformMoveSwitch(BattleOpponent opponent)
	{
		yield return StartCoroutine(SwitchBringMortyIn(opponent));
		yield return StartCoroutine(SwitchSendMortyOut(opponent, opponent.move.mortyData));
	}

	private IEnumerator SwitchMortyTrySendOutNext(BattleOpponent opponent)
	{
		opponent.move = null;
		if (opponent == _player)
		{
			yield return StartCoroutine(SwitchMortyPlayerTrySendOutNext());
		}
		else
		{
			yield return StartCoroutine(SwitchMortyEnemyTrySendOutNext());
		}
	}

	private IEnumerator SwitchMortyPlayerTrySendOutNext()
	{
		if (_player.mortyDatas.Find((MortyData e) => e.hp > 0) != null)
		{
			_player.move = new BattleSwitchMove(null);
			if (this.BattleSwitchEvent != null)
			{
				_waitingForPartyStateToSwitch = true;
				this.BattleSwitchEvent(_player.currentMortyData, true);
			}
			while (_waitingForPartyStateToSwitch)
			{
				yield return null;
			}
			if (_player.move.type == eBattleMoveType.SWITCH && _player.move.mortyData != null)
			{
				yield return StartCoroutine(SwitchSendMortyOut(_player, _player.move.mortyData));
				yield break;
			}
		}
		StopAllCoroutines();
		StartCoroutine(BattleEndPlayerBlackOut());
	}

	private IEnumerator SwitchMortyEnemyTrySendOutNext()
	{
		if (_player.mortyDatas.Find((MortyData e) => !e.isFainted) == null)
		{
			StopAllCoroutines();
			StartCoroutine(BattleEndPlayerBlackOut());
			yield break;
		}
		if (_enemy is BattleOpponentAI)
		{
			((BattleOpponentAI)_enemy).RefreshSwitchMove(true);
		}
		if (_enemy.move != null && _enemy.move.type == eBattleMoveType.SWITCH && _enemy.move.mortyData != null)
		{
			yield return StartCoroutine(_enemy.morty.SetImage(MortyDefs.AssetID(_enemy.move.mortyData.id)));
			StartCoroutine(_enemy.morty.TweenInCo());
			if (_player.mortyDatas.Find((MortyData e) => e.hp > 0 && e != _player.currentMortyData) != null)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_ENEMY_MORTY_WANTS_OUT, _enemy.name, MortyDefs.Name(_enemy.move.mortyData.id)), 2f, true));
				if (_player.currentMortyData.isFainted)
				{
					if (this.BattleSwitchEvent != null)
					{
						_player.move = new BattleSwitchMove(null);
						_waitingForPartyStateToSwitch = true;
						this.BattleSwitchEvent(_player.currentMortyData, true);
					}
				}
				else
				{
					_player.mortyInfoBar.TweenToStartingY();
					yield return StartCoroutine(ShowSwitchMenu());
				}
				while (_waitingForSwitchMenu || _waitingForPartyStateToSwitch)
				{
					yield return null;
				}
				_player.mortyInfoBar.TweenDownY();
				if (_player.move != null && _player.move.type == eBattleMoveType.SWITCH && _player.move.mortyData != null)
				{
					if (_player.currentMortyData.isFainted)
					{
						yield return StartCoroutine(SwitchSendMortyOut(_player, _player.move.mortyData));
					}
					else
					{
						yield return StartCoroutine(PerformMoveSwitch(_player));
					}
				}
			}
			yield return StartCoroutine(SwitchSendMortyOut(_enemy, _enemy.move.mortyData, true));
			if (Random.value > 0.5f)
			{
				VoiceOverDefs.PlayRandom("Audio/VO/Morty - Worry/{0}", 0.5f, -0.5f);
			}
			_player.move = null;
		}
		else
		{
			StopAllCoroutines();
			if (_enemy.type == eBattleOpponentType.AI_TRAINER)
			{
				StartCoroutine(BattleEndTrainerBeaten());
			}
			else
			{
				StartCoroutine(BattleEndWildMortyBeaten());
			}
		}
	}

	private IEnumerator SwitchSendMortyOut(BattleOpponent opponent, MortyData newMortyData, bool ignoreMortyUI = false)
	{
		PlayerDefs.SetMortySeen(newMortyData.id);
		opponent.currentMortyData = newMortyData;
		StartCoroutine(opponent.trainer.TweenOutCo());
		if (!ignoreMortyUI)
		{
			yield return StartCoroutine(opponent.morty.SetImage(MortyDefs.AssetID(newMortyData.id)));
			StartCoroutine(opponent.morty.TweenInCo());
		}
		eCaughtIndicatorType caughtIndicatorType = eCaughtIndicatorType.NONE;
		if (opponent.type == eBattleOpponentType.AI_WILD_MORTY)
		{
			caughtIndicatorType = (PlayerDefs.DoesPlayerOwnMortyOfID(newMortyData.id) ? eCaughtIndicatorType.OWNED : ((!PlayerDefs.HasCaughtMorty(newMortyData.id)) ? eCaughtIndicatorType.NOT_CAUGHT : eCaughtIndicatorType.CAUGHT));
		}
		opponent.mortyInfoBar.SetAll(opponent.mortyDatas, newMortyData);
		opponent.mortyInfoBar.SetCaughtIndicator(caughtIndicatorType);
		opponent.mortyInfoBar.TweenIn();
		App.sound.PlaySFX("Audio/SFX/SendMortyOut");
		if (opponent == _player)
		{
			_enemy.SetEnemyMorty(opponent.currentMortyData);
		}
		else if (opponent == _enemy)
		{
			_player.SetEnemyMorty(opponent.currentMortyData);
		}
		if (opponent.type == eBattleOpponentType.PLAYER)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Send Morty Out/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 1f);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_PLAYER_MORTY_OUT, MortyDefs.Name(newMortyData.id)), 2f));
		}
		else if (opponent.type == eBattleOpponentType.AI_WILD_MORTY)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_WILD_MORTY_OUT, MortyDefs.Name(opponent.currentMortyData.id)), 2f));
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_ENEMY_MORTY_OUT, _enemy.name, MortyDefs.Name(opponent.currentMortyData.id)), 2f, true));
		}
	}

	private IEnumerator SwitchBringMortyIn(BattleOpponent opponent)
	{
		StartCoroutine(opponent.morty.TweenOutCo());
		opponent.mortyInfoBar.TweenOut();
		App.sound.PlaySFX("Audio/SFX/BringMortyIn");
		if (opponent.type == eBattleOpponentType.PLAYER)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_PLAYER_MORTY_IN, MortyDefs.Name(opponent.currentMortyData.id)), 2f, true));
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_ENEMY_MORTY_IN, _enemy.name, MortyDefs.Name(opponent.currentMortyData.id)), 1f));
		}
	}

	private IEnumerator FaintMorty(BattleOpponent opponent)
	{
		opponent.mortyInfoBar.SetParty(opponent.mortyDatas);
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FAINTED, MortyDefs.Name(opponent.currentMortyData.id))));
		yield return StartCoroutine(opponent.mortyInfoBar.TweenOutCo());
		App.sound.PlaySFX("Audio/SFX/Dazed", 1f, false, 1f, 0f, 0.5f);
		VoiceOverDefs.PlayRandom("Audio/VO/Morty - Dazed/{0}", 0.5f, GetVOPan(opponent.side), 0.8f);
		if (opponent.type == eBattleOpponentType.PLAYER)
		{
			if (opponent.mortyDatas.Find((MortyData e) => e.hp > 0) != null)
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Morty Put Down/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
			}
		}
		else if (opponent.mortyDatas.Find((MortyData e) => e.hp > 0) != null)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Complement/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
		}
		yield return StartCoroutine(PlayMoveAnimation("Faint", opponent.side));
		opponent.mortiesToRewardXP.Remove(opponent.currentMortyData);
		if (_isLevelingActive && opponent.type != eBattleOpponentType.PLAYER)
		{
			yield return StartCoroutine(EnemyFaintedRewardXP());
		}
		if (opponent.type != eBattleOpponentType.PLAYER)
		{
			_player.mortiesToRewardXP.Clear();
		}
		else
		{
			_enemy.mortiesToRewardXP.Clear();
		}
	}

	private IEnumerator EnemyFaintedRewardXP()
	{
		_player.mortiesToRewardXP.Remove(_player.currentMortyData);
		_player.mortiesToRewardXP.Insert(0, _player.currentMortyData);
		float xpTotalToReward = MortyDefs.GetXPEarned(_player.currentMortyData, _enemy.currentMortyData, _enemy.type == eBattleOpponentType.AI_WILD_MORTY);
		float xpSplitForCurrent = Mathf.Clamp(1f / (float)_player.mortiesToRewardXP.Count + 1f / (float)_player.mortiesToRewardXP.Count / 2f, 0f, 1f);
		float xpSplitForOthers = Mathf.Clamp((1f - xpSplitForCurrent) / (float)(_player.mortiesToRewardXP.Count - 1), 0f, 1f);
		for (int i = 0; i < _player.mortiesToRewardXP.Count; i++)
		{
			yield return StartCoroutine(RewardXP(xpReward: Mathf.FloorToInt(xpTotalToReward * ((_player.mortiesToRewardXP[i] != _player.currentMortyData) ? xpSplitForOthers : xpSplitForCurrent)), mortyData: _player.mortiesToRewardXP[i]));
		}
		_player.currentMortyData.ApplyBattleEV(_enemy.currentMortyData.id);
	}

	private IEnumerator RewardXP(MortyData mortyData, int xpReward)
	{
		if (TestDefs.PLAYER_GUARANTEE_LEVEL_UP)
		{
			xpReward = MortyDefs.GetXPTarget(mortyData.level + 1) - mortyData.xp;
		}
		int xpRewardRemaining = xpReward;
		while (xpRewardRemaining > 0)
		{
			int targetLevel = mortyData.level + 1;
			int targetXP = MortyDefs.GetXPTarget(targetLevel);
			int xpToAdd = Mathf.Clamp(xpRewardRemaining, 0, targetXP - mortyData.xp);
			if (xpRewardRemaining == xpReward)
			{
				string rewardText = string.Format((_player.currentMortyData != mortyData) ? TextDefs.BATTLE_REWARD_XP_OTHER : TextDefs.BATTLE_REWARD_XP_CURRENT, MortyDefs.Name(mortyData.id), xpReward);
				yield return StartCoroutine(_dialogue.TweenTextCo(rewardText, 2f, true));
			}
			App.sound.PlaySFX("Audio/SFX/GainXP", 0.5f);
			mortyData.xp += xpToAdd;
			if (mortyData == _player.currentMortyData)
			{
				yield return StartCoroutine(_player.mortyInfoBar.SetXPWithTween(mortyData.xp));
			}
			if (targetXP == mortyData.xp)
			{
				if (mortyData.level >= 100)
				{
					yield break;
				}
				StopMortyIdleTweens();
				int oldHPMax = mortyData.hpStat;
				int oldAttackMax = mortyData.attackStat;
				int oldDefenceMax = mortyData.defenceStat;
				int oldSpeedMax = mortyData.speedStat;
				string resultText;
				MortyDefs.IncrementLevel(mortyData, false, _player.pronounYours, out resultText);
				if (mortyData == _player.currentMortyData)
				{
					_player.mortyInfoBar.SetLevel(mortyData.level);
					_player.mortyInfoBar.SetHPMax(mortyData.hpStat, mortyData.hp);
				}
				_dialogue.TweenText(resultText);
				App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
				_levelUp.SetStats(mortyData.hpStat, mortyData.hpStat - oldHPMax, mortyData.attackStat, mortyData.attackStat - oldAttackMax, mortyData.defenceStat, mortyData.defenceStat - oldDefenceMax, mortyData.speedStat, mortyData.speedStat - oldSpeedMax);
				if (mortyData != _player.currentMortyData)
				{
					_player.morty.SetPositionOut();
					yield return StartCoroutine(_player.morty.SetImage(MortyDefs.AssetID(mortyData.id), true));
					_player.morty.TweenIn();
					_player.mortyInfoBar.SetPositionOut();
					_player.mortyInfoBar.SetAll(_player.mortyDatas, mortyData);
					_player.mortyInfoBar.TweenIn();
				}
				App.states.FlashCurrentState(0.1f);
				yield return StartCoroutine(_levelUp.TweenBackground());
				yield return StartCoroutine(_levelUp.TweenStatsBoxIn());
				yield return StartCoroutine(_dialogue.SetTextCo(resultText, 2f, true));
				yield return StartCoroutine(_levelUp.TweenStatsBoxOut());
				yield return StartCoroutine(MortyLeveledUpLearnAttack(mortyData));
				if (mortyData != _player.currentMortyData)
				{
					_player.mortyInfoBar.TweenOut();
					yield return StartCoroutine(_player.morty.TweenOutCo());
					_player.mortyInfoBar.SetAll(_player.mortyDatas, _player.currentMortyData);
					yield return StartCoroutine(_player.morty.SetImage(MortyDefs.AssetID(_player.currentMortyData.id)));
					_player.mortyInfoBar.TweenIn();
					yield return StartCoroutine(_player.morty.TweenInCo());
				}
			}
			_player.mortyInfoBar.SetXPTarget(MortyDefs.GetXPTarget(_player.currentMortyData.level), MortyDefs.GetXPTarget(_player.currentMortyData.level + 1));
			_player.mortyInfoBar.SetXP(_player.currentMortyData.xp);
			xpRewardRemaining -= xpToAdd;
		}
		StartMortyIdleTweens();
	}

	private IEnumerator MortyLeveledUpLearnAttack(MortyData mortyData)
	{
		_isLevelUpSwapAttack = true;
		List<KeyValuePair<int, string>> unlockableAttacks = MortyDefs.Attacks(mortyData.id);
		KeyValuePair<int, string>[] attacksToUnlock = unlockableAttacks.FindAll((KeyValuePair<int, string> keyValuePair) => keyValuePair.Key == mortyData.level).ToArray();
		for (int a = 0; a < attacksToUnlock.Length; a++)
		{
			AttackData attackData = AttackDefs.GetData(attacksToUnlock[a].Value);
			eAttackCatagory attackCatagory = AttackDefs.GetAttackCatagory(attackData);
			int availableAttackSlotIndex = mortyData.GetAvailableAttackSlot();
			if (availableAttackSlotIndex > -1)
			{
				_levelUpSwapAttackIndex = availableAttackSlotIndex;
				mortyData.AddAttackToSlot(availableAttackSlotIndex, attackData.id);
			}
			else
			{
				RectTransform attackBoxTrans = UI.Instantiate(string.Format("UI/Menu/AttackLearnNewBox/AttackLearnNewBox{0}", attackCatagory), _attackLearnContainer);
				_newAttackBox = attackBoxTrans.GetComponent<AttackLearnNewBox>();
				_newAttackBox.SetUp(new AttackDisplayInfo(attackData));
				_newAttackBox.SetPositionOut();
				yield return new WaitForEndOfFrame();
				StartCoroutine(_newAttackBox.TweenIn());
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_1, MortyDefs.Name(mortyData.id), attackData.name), 2f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_WANTS_TO_LEARN_2, 2f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_WANTS_TO_LEARN_3, attackData.name)));
				_player.mortyInfoBar.TweenToStartingY();
				StartCoroutine(ShowAttackMenu(mortyData));
				while (_levelUpSwapAttackIndex == -1)
				{
					yield return null;
				}
				SetMenusInteractable(false);
				if (_levelUpSwapAttackIndex != 4)
				{
					string forgetAttackID = mortyData.attackIDs[_levelUpSwapAttackIndex];
					string forgetAttackName = AttackDefs.GetData(forgetAttackID).name;
					mortyData.RemoveAttackFromSlot(_levelUpSwapAttackIndex);
					RefreshAttackMenu(mortyData);
					yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FORGOT_ATTACK, MortyDefs.Name(mortyData.id), forgetAttackName), 2f, true));
					mortyData.AddAttackToSlot(_levelUpSwapAttackIndex, attackData.id);
					RefreshAttackMenu(mortyData);
				}
				else
				{
					yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_DID_NOT_LEARN, MortyDefs.Name(mortyData.id), attackData.name), 2f, true));
				}
				StartCoroutine(_newAttackBox.TweenOut(true));
			}
			if (_levelUpSwapAttackIndex > -1 && _levelUpSwapAttackIndex < 4)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_LEARNED, MortyDefs.Name(mortyData.id), attackData.name), 2f, true));
			}
			_player.mortyInfoBar.TweenDownY();
			StartCoroutine(HideAttackMenu());
			_levelUpSwapAttackIndex = -1;
		}
		_isLevelUpSwapAttack = false;
	}

	private void SetMenusInteractable(bool isInteractable)
	{
		_mainMenu.SetInteractable(isInteractable);
		_attackMenu.SetInteractable(isInteractable);
		_switchMenu.SetInteractable(isInteractable);
	}

	private IEnumerator MainToAttackMenu()
	{
		yield return StartCoroutine(HideMainMenu());
		yield return StartCoroutine(ShowAttackMenu(_player.currentMortyData));
	}

	private IEnumerator AttackToMainMenu()
	{
		yield return StartCoroutine(HideAttackMenu());
		yield return StartCoroutine(ShowMainMenu());
	}

	protected virtual IEnumerator ShowMainMenu()
	{
		StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_MAIN_MENU));
		yield return StartCoroutine(_mainMenu.TweenIn());
	}

	private IEnumerator HideMainMenu()
	{
		yield return StartCoroutine(_mainMenu.TweenOut());
	}

	protected virtual IEnumerator ShowSwitchMenu()
	{
		_waitingForSwitchMenu = true;
		StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_SWITCH_PLAYER_WANT_TO));
		yield return StartCoroutine(_switchMenu.TweenIn());
	}

	private IEnumerator HideSwitchMenu()
	{
		yield return StartCoroutine(_switchMenu.TweenOut());
		_waitingForSwitchMenu = false;
	}

	protected virtual IEnumerator ShowAttackMenu(MortyData mortyData)
	{
		RefreshAttackMenu(mortyData);
		if (!_isLevelUpSwapAttack)
		{
			StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MENU, MortyDefs.Name(mortyData.id))));
		}
		yield return StartCoroutine(_attackMenu.TweenIn());
	}

	private IEnumerator HideAttackMenu()
	{
		yield return StartCoroutine(_attackMenu.TweenOut());
	}

	private void RefreshAttackMenu(MortyData mortyData)
	{
		for (int i = 0; i < 4; i++)
		{
			if (i < mortyData.attackIDs.Count && mortyData.attackIDs[i] != null)
			{
				_attackMenu.SetBtnEnabled(i, true);
				string id = mortyData.attackIDs[i];
				AttackData data = AttackDefs.GetData(id);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(data, mortyData.attackPPs[i]));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	private IEnumerator PlayMoveAnimation(string id, eBattleOpponentSide side)
	{
		yield return StartCoroutine(_moveAnimationController.PlayAnimation(id, side));
	}

	private void StartMortyIdleTweens()
	{
		_player.morty.StartIdleTween(false);
		_enemy.morty.StartIdleTween(true);
	}

	private void StopMortyIdleTweens()
	{
		_player.morty.StopIdleTween();
		_enemy.morty.StopIdleTween();
	}

	private float GetVOPan(eBattleOpponentSide side)
	{
		return (side != eBattleOpponentSide.LEFT) ? 0.5f : (-0.5f);
	}

	private void PlaySFXRickEngage(eTrainerType type)
	{
		switch (type)
		{
		default:
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Engage/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
			break;
		case eTrainerType.GYM_LEADER:
			if (Random.value > 0.3f)
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Engage Against Rick/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
			}
			else
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Engage Against Rick Dimension/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
			}
			break;
		case eTrainerType.COUNCIL_MEMBER:
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Engage Against Rick/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
			break;
		}
	}

	private void PlaySFXRickWin()
	{
		eTrainerType eTrainerType2 = ((_enemy is BattleOpponentAI) ? ((BattleOpponentAI)_enemy).trainerType : eTrainerType.NONE);
		if ((eTrainerType2 == eTrainerType.GYM_LEADER || eTrainerType2 == eTrainerType.COUNCIL_MEMBER) && Random.value > 0.7f)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Win Against Rick/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
		}
		else
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Win/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f);
		}
	}

	private void PlaySFXRickLose()
	{
		if (_enemy.type == eBattleOpponentType.AI_WILD_MORTY)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Morty Put Down/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
		}
		else
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Lose Trainer/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
		}
	}

	private void PlaySFXRickHappy()
	{
		VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Happy/{0}/{1}", PlayerDefs.AvatarCategory, 0.75f, -0.5f, 0.8f);
	}

	private void PlaySFXEffect(eAttackEffectType type, float value, eBattleOpponentSide side)
	{
		switch (type)
		{
		case eAttackEffectType.Hit:
			App.sound.PlaySFX("Audio/SFX/PowerAttack", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eAttackEffectType.Stat:
			if (value > 0f)
			{
				App.sound.PlaySFX("Audio/SFX/BuffAttack", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
				VoiceOverDefs.PlayRandom("Audio/VO/Morty - Buff/{0}", 0.5f, GetVOPan(side));
			}
			else if (value < 0f)
			{
				App.sound.PlaySFX("Audio/SFX/DebuffAttack", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
				VoiceOverDefs.PlayRandom("Audio/VO/Morty - Debuff/{0}", 0.5f, GetVOPan(side));
			}
			break;
		case eAttackEffectType.Poison:
			App.sound.PlaySFX("Audio/SFX/Poison", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eAttackEffectType.Paralyse:
			App.sound.PlaySFX("Audio/SFX/Paralyse", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eAttackEffectType.Absorb:
			App.sound.PlaySFX("Audio/SFX/Absorb", 0.65f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		}
	}

	private void PlaySFXMortyHurt(eBattleOpponentType type)
	{
		if (type == eBattleOpponentType.PLAYER)
		{
			VoiceOverDefs.PlayRandom("Audio/VO/Morty - Hurt/{0}", 0.5f, 0.5f);
		}
		else if (Random.value > 0.3f)
		{
			VoiceOverDefs.PlayRandom("Audio/VO/Morty - Hurt/{0}", 0.5f, -0.5f);
		}
		else
		{
			VoiceOverDefs.PlayRandom("Audio/VO/Morty - Moan/{0}", 0.5f, -0.5f);
		}
	}

	private void PlaySFXUseItem(string itemID)
	{
		if (!string.IsNullOrEmpty(ItemDefs.AttackerAnimation(itemID)) || !string.IsNullOrEmpty(ItemDefs.DefenderAnimation(itemID)))
		{
			switch (itemID)
			{
			case "ItemMrMeeseek":
				App.sound.PlaySFX("Audio/SFX/MeeseekAttack");
				break;
			case "ItemMortyChip":
				App.sound.PlaySFX("Audio/SFX/UseChipItem", 1f, false, 1f, 0f, 1.2f);
				break;
			case "ItemParalysisCure":
			case "ItemPoisonCure":
				App.sound.PlaySFX("Audio/SFX/UseItemSpray", 1f, false, 1f, 0f, 1.2f);
				break;
			default:
				App.sound.PlaySFX("Audio/SFX/UseItem", 1f, false, 1f, 0f, 1.3f);
				break;
			}
		}
	}

	public void UncachedFromBagMenu(string itemID, MortyData mortyData)
	{
		if (_player.move == null)
		{
			_player.move = new BattleItemMove(itemID, mortyData);
			StartCoroutine(StartTurn());
		}
		else if (_player.move.type == eBattleMoveType.ITEM)
		{
			_player.move.id = itemID;
			_player.move.mortyData = mortyData;
		}
	}

	public void UncachedFromPartyMenu(MortyData newMortyData)
	{
		if (_player.move == null)
		{
			_player.move = new BattleSwitchMove(newMortyData);
			StartCoroutine(StartTurn());
		}
		else if (_player.move.type == eBattleMoveType.SWITCH)
		{
			_player.move.mortyData = newMortyData;
		}
	}

	public void UncacheFinished()
	{
		_waitingForPartyStateToSwitch = false;
	}

	private void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (_player.currentMortyData.GetTotalRemainingPP() > 0)
			{
				StartCoroutine(MainToAttackMenu());
				break;
			}
			_player.move = new BattleAttackMove(AttackDefs.GetData("AttackStruggle"));
			StartCoroutine(StartTurn());
			break;
		case 1:
			if (this.BattleItemEvent != null)
			{
				this.BattleItemEvent();
			}
			break;
		case 2:
			if (this.BattleSwitchEvent != null)
			{
				_waitingForPartyStateToSwitch = true;
				this.BattleSwitchEvent(_player.currentMortyData, false);
			}
			break;
		case 3:
			if (_enemy.type == eBattleOpponentType.AI_WILD_MORTY)
			{
				PlayerConfirmRun();
			}
			else
			{
				StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_CANT_RUN_FROM_TRAINER));
			}
			break;
		}
	}

	private void PlayerConfirmRun()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string bATTLE_POPUP_RUN_DESCRIPTION = TextDefs.BATTLE_POPUP_RUN_DESCRIPTION;
		genericPopupState.SetDisplayValues(bATTLE_POPUP_RUN_DESCRIPTION, new string[1] { TextDefs.BATTLE_POPUP_RUN_BTN_YES });
		genericPopupState.ButtonClickedEvent += GenericPopupRunConfirmationHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void AttackMenuBtnPressedHandler(int index)
	{
		if (!_isLevelUpSwapAttack)
		{
			if (index == 4)
			{
				StartCoroutine(AttackToMainMenu());
			}
			else if (_player.currentMortyData.attackPPs[index] > 0)
			{
				_player.move = new BattleAttackMove(AttackDefs.GetData(_player.currentMortyData.attackIDs[index]));
				StartCoroutine(StartTurn());
			}
			else
			{
				StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_OUT_OF_PP));
			}
		}
		else
		{
			_levelUpSwapAttackIndex = index;
		}
	}

	protected virtual void AttackMenuBtnLongPressStartHandler(int index)
	{
	}

	private void SwitchMenuBtnPressedHandler(int index)
	{
		StartCoroutine(HideSwitchMenu());
		if (index == 0)
		{
			_player.move = new BattleSwitchMove(null);
			if (this.BattleSwitchEvent != null)
			{
				_waitingForPartyStateToSwitch = true;
				this.BattleSwitchEvent(_player.currentMortyData, false);
			}
		}
	}

	private void GenericPopupRunConfirmationHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= GenericPopupRunConfirmationHandler;
		App.states.SetBack(new PopTransInfo());
		if (buttonIndex == 1)
		{
			StopAllCoroutines();
			StartCoroutine(BattleEndPlayerRan());
		}
	}
}
