using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MPBattleUI : UIBase
{
	public delegate void MPBattleUIDelegate();

	public delegate void MPBattleFinishedUIDelegate(eMPBattleTurnResult resultType);

	public delegate void MPBattleUIMoveDelegate(eMPBattleMoveType type, string mortyNetId, string moveId);

	public delegate void MPBattleUIEmoteSelectedDelgate(string playerId, string emoteId);

	private const float _kVoPanLeft = -0.5f;

	private const float _kVoPanRight = 0.5f;

	private const string _kAttackMovePowerAnimation = "PowerSwosh";

	private const string _kAttackMoveBuffAnimation = "Buff";

	private const string _kAttackMoveDebuffAnimation = "Debuff";

	private const string _kAttackMovePoisonAnimation = "Poison";

	private const string _kAttackMoveParalyseAnimation = "Paralyse";

	private const string _kAttackMoveAbsorbAnimation = "Absorb";

	private const string _kAttackMoveFaintAnimation = "Faint";

	protected Transform _container;

	private BattleBackground _background;

	private BattleLevelUp _levelUp;

	private BattleMoveAnimationController _moveAnimationController;

	private MPBattleTimer _moveTimer;

	private MenuDialogue _dialogue;

	protected MenuBase _mainMenu;

	private BattleAttackMenu _attackMenu;

	private BattleAttackMenu _attackStruggleMenu;

	private MenuBase _switchMenu;

	private GameObject _emoteBtnObj;

	private MPBattleEmoteBarUI _playerEmoteBar;

	private MPEmoteBubble _playerEmoteBubble;

	private MPEmoteBubble _enemyEmoteBubble;

	public MPBattleOpponent _player;

	public MPBattleOpponent _enemy;

	public GameObject _usernameBar;

	public event MPBattleUIDelegate StartFinishedEvent;

	public event MPBattleUIDelegate SelectingMoveEvent;

	public event MPBattleUIDelegate TurnFinishedEvent;

	public event MPBattleUIDelegate ItemBtnEvent;

	public event MPBattleUIDelegate SwitchBtnEvent;

	public event MPBattleUIDelegate RunBtnEvent;

	public event MPBattleFinishedUIDelegate BattleFinishedEvent;

	public event MPBattleUIMoveDelegate MoveSelectedEvent;

	public event MPBattleUIEmoteSelectedDelgate EmoteSelectedEvent;

	protected override void LateAwake()
	{
		_container = base.transform.FindChild("Container").transform;
		_background = _container.FindChild("Background").GetComponent<BattleBackground>();
		_levelUp = _container.FindChild("MortyLevelUp").GetComponent<BattleLevelUp>();
		_moveAnimationController = _container.FindChild("MoveAnimationController").GetComponent<BattleMoveAnimationController>();
		_emoteBtnObj = _container.FindChild("MenuBar/EmoteButton").gameObject;
		_usernameBar = _container.FindChild("UsernameBar").gameObject;
		_dialogue = _container.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_mainMenu = _container.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_attackMenu = _container.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
		_attackStruggleMenu = _container.FindChild("MenuBar/AttackStruggleMenu").GetComponent<BattleAttackMenu>();
		_switchMenu = _container.FindChild("MenuBar/SwitchMenu").GetComponent<MenuBase>();
		_playerEmoteBar = _container.FindChild("PlayerEmoteBar").GetComponent<MPBattleEmoteBarUI>();
		_playerEmoteBubble = _container.FindChild("PlayerEmote").GetComponent<MPEmoteBubble>();
		_enemyEmoteBubble = _container.FindChild("EnemyEmote").GetComponent<MPEmoteBubble>();
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(string.Format("UI/MPBattle/{0}", "Timer")));
		gameObject.transform.SetParent(_container);
		_moveTimer = gameObject.GetComponent<MPBattleTimer>();
	}

	public void SetUp(MPBattleOpponent player, MPBattleOpponent enemy)
	{
		_player = player;
		_enemy = enemy;
		_player.trainer = _container.FindChild("PlayerTrainer").GetComponent<BattleTrainer>();
		_player.morty = _container.FindChild("PlayerMorty").GetComponent<BattleMorty>();
		_player.mortyInfoBar = _container.FindChild("PlayerMortyInfoBar").GetComponent<MPBattleMortyInfoBar>();
		_player.mortyInfoBar.moveDirection = -1;
		_enemy.trainer = _container.FindChild("EnemyTrainer").GetComponent<BattleTrainer>();
		_enemy.morty = _container.FindChild("EnemyMorty").GetComponent<BattleMorty>();
		_enemy.mortyInfoBar = _container.FindChild("EnemyMortyInfoBar").GetComponent<MPBattleMortyInfoBar>();
		_enemy.mortyInfoBar.moveDirection = 1;
		_enemy.mortyInfoBar.SetUsername(enemy.opponentData.GetUsername());
		_moveAnimationController.SetUp(_player.morty, _player.mortyInfoBar.transform, _enemy.morty, _enemy.mortyInfoBar.transform);
		_player.trainer.SetUp(_player.opponentData.assetId, _player.side);
		_player.morty.SetUp(_player.side);
		if (_enemy.IsTrainer())
		{
			_enemy.trainer.SetUp(_enemy.opponentData.assetId, _enemy.side);
			_usernameBar.transform.FindChild("Background/Label").GetComponent<Text>().text = enemy.opponentData.GetUsername();
			_usernameBar.transform.FindChild("Background/Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", _enemy.opponentData.assetId));
		}
		else
		{
			_usernameBar.SetActive(false);
		}
		_enemy.morty.SetUp(_enemy.side);
		_enemy.mortyInfoBar.SetPartyActive(_enemy.opponentData.type != eMPOpponentType.WILD_MORTY);
		_player.trainer.SetPositionOut();
		_player.morty.SetPositionOut();
		_player.mortyInfoBar.SetPositionOut();
		_player.mortyInfoBar.SetPositionDownY();
		_enemy.trainer.SetPositionOut();
		_enemy.morty.SetPositionOut();
		_enemy.mortyInfoBar.SetPositionOut();
		_mainMenu.SetPositionOut();
		_switchMenu.SetPositionOut();
		_attackMenu.SetPositionOut();
		_attackStruggleMenu.SetPositionOut();
		_dialogue.SetText(string.Empty);
		_playerEmoteBar.gameObject.SetActive(false);
		if (enemy.opponentData.type != eMPOpponentType.USER)
		{
			_emoteBtnObj.SetActive(false);
		}
	}

	public void SetUpBackground(string worldId)
	{
		_background.SetBackground(MPWorldDefs.GetResourceId(worldId));
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
		_attackStruggleMenu.BtnPressedEvent += AttackStruggleMenuBtnPressedHandler;
		_switchMenu.BtnPressedEvent += SwitchMenuBtnPressedHandler;
		_playerEmoteBar.EmoteSelectedEvent += EmoteSelectedHandler;
		_playerEmoteBar.AnimateOnBeginEvent += EmoteBarAnimateOnBeginHandler;
		_playerEmoteBar.AnimateOffCompleteEvent += EmoteBarAnimateOffCompleteHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_attackMenu.BtnPressedEvent -= AttackMenuBtnPressedHandler;
		_attackMenu.BtnLongPressStartEvent -= AttackMenuBtnLongPressStartHandler;
		_attackStruggleMenu.BtnPressedEvent += AttackStruggleMenuBtnPressedHandler;
		_switchMenu.BtnPressedEvent -= SwitchMenuBtnPressedHandler;
		_playerEmoteBar.EmoteSelectedEvent -= EmoteSelectedHandler;
		_playerEmoteBar.AnimateOnBeginEvent -= EmoteBarAnimateOnBeginHandler;
		_playerEmoteBar.AnimateOffCompleteEvent -= EmoteBarAnimateOffCompleteHandler;
	}

	public void ShowContainer(bool value)
	{
		_container.gameObject.SetActive(value);
	}

	public IEnumerator StartBattle()
	{
		yield return new WaitForEndOfFrame();
		StartCoroutine(_player.trainer.TweenInCo());
		if (_enemy.IsTrainer())
		{
			yield return StartCoroutine(_enemy.trainer.TweenInCo());
			PlaySFXRickEngage();
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_START_TRAINER, _enemy.opponentData.GetUsername()), 2f));
		}
		StartCoroutine(_enemy.trainer.TweenOutCo());
		yield return StartCoroutine(SwitchSendMortyOut(_enemy, _enemy.opponentData.GetMortyData(_enemy.opponentData.battleStartMortyId)));
		StartCoroutine(_player.trainer.TweenOutCo());
		yield return StartCoroutine(SwitchSendMortyOut(_player, _player.opponentData.GetMortyData(_player.opponentData.battleStartMortyId)));
		StartMortyIdleTweens();
		SelectingMoveRequest();
		if (this.StartFinishedEvent != null)
		{
			this.StartFinishedEvent();
		}
	}

	public IEnumerator FinishBattle(eMPBattleTurnResult resultType)
	{
		SetAllMenusInteractable(false);
		HideAllMenusTween();
		StopMortyIdleTweens();
		if (_enemy.IsTrainer())
		{
			yield return StartCoroutine(FinishBattleTrainer(resultType));
		}
		else
		{
			yield return StartCoroutine(FinishBattleWildMorty(resultType));
		}
		if (this.BattleFinishedEvent != null)
		{
			this.BattleFinishedEvent(resultType);
		}
	}

	private IEnumerator FinishBattleTrainer(eMPBattleTurnResult resultType)
	{
		switch (resultType)
		{
		case eMPBattleTurnResult.WIN:
			PlaySFXRickWin();
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_FINISHED_TRAINER_WON, _enemy.opponentData.GetUsername()), 2f, true));
			break;
		case eMPBattleTurnResult.LOSE:
			PlaySFXRickLose();
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_FINISHED_TRAINER_LOST, _enemy.opponentData.GetUsername()), 2f, true));
			break;
		case eMPBattleTurnResult.DRAW:
			yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_FINISHED_TRAINER_DREW, 2f, true));
			break;
		case eMPBattleTurnResult.ABANDONED:
			_emoteBtnObj.SetActive(false);
			StartCoroutine(_player.mortyInfoBar.TweenOutCo());
			StartCoroutine(_enemy.morty.TweenOutCo());
			StartCoroutine(_enemy.mortyInfoBar.TweenOutCo());
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_FINISHED_TRAINER_ABANDONED, _enemy.opponentData.GetUsername()), 4f));
			break;
		case eMPBattleTurnResult.RUN:
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_RUN_FROM_TRAINER, _enemy.opponentData.GetUsername()), 2f));
			break;
		}
	}

	private IEnumerator FinishBattleWildMorty(eMPBattleTurnResult resultType)
	{
		switch (resultType)
		{
		case eMPBattleTurnResult.WIN:
			PlaySFXRickWin();
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_FINISHED_WILD_MORTY_WON, MPMortyDefs.Name(_enemy.opponentData.GetPartyMortyData(0).id)), 2f, true));
			break;
		case eMPBattleTurnResult.LOSE:
			PlaySFXRickLose();
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_FINISHED_WILD_MORTY_LOST, MPMortyDefs.Name(_enemy.opponentData.GetPartyMortyData(0).id)), 2f, true));
			break;
		case eMPBattleTurnResult.DRAW:
			yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_FINISHED_WILD_MORTY_DREW, 2f, true));
			break;
		case eMPBattleTurnResult.RUN:
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_RUN_FROM_WILD_MORTY, 2f));
			break;
		}
	}

	public IEnumerator StartTurn(List<MPBattleActData> turnActData, MPBattleTurnOpponentData turnPlayerData, MPBattleTurnOpponentData turnEnemyData)
	{
		_moveTimer.Show(false);
		_player.mortyInfoBar.TweenDownY();
		HideAllMenusTween();
		for (int i = 0; i < turnActData.Count; i++)
		{
			yield return StartCoroutine(StartTurnAct(turnActData[i]));
		}
		_player.opponentData.SetItemsFromDicts(turnPlayerData.itemsArr);
		yield return StartCoroutine(AwardXPDatas(_player, turnPlayerData.mortyXpDatas));
		yield return StartCoroutine(AwardXPDatas(_enemy, turnEnemyData.mortyXpDatas));
		TurnFinishedRequest();
	}

	private IEnumerator StartTurnAct(MPBattleActData actData)
	{
		MPBattleOpponent attacker = GetOpponent(actData.attackerNetId);
		MPBattleOpponent defender = GetOpponent(actData.defenderNetId);
		attacker.opponentData.SetMortiesFromDicts(actData.attackerMortyDataDicts);
		defender.opponentData.SetMortiesFromDicts(actData.defenderMortyDataDicts);
		switch (actData.type)
		{
		case eMPBattleActType.ATTACK:
			yield return StartCoroutine(TurnActAttack((MPBattleActAttackData)actData, attacker, defender));
			break;
		case eMPBattleActType.ITEM:
			yield return StartCoroutine(TurnActItem((MPBattleActItemData)actData, attacker, defender));
			break;
		case eMPBattleActType.SWITCH:
			yield return StartCoroutine(TurnActSwitch((MPBattleActSwitchData)actData, attacker));
			break;
		case eMPBattleActType.POISONED:
			yield return StartCoroutine(TurnActPoisoned(actData, defender));
			break;
		case eMPBattleActType.PARALYSED:
			yield return StartCoroutine(TurnActParalysed((MPBattleActParalysedData)actData, attacker));
			break;
		case eMPBattleActType.DISOBEY:
			yield return StartCoroutine(TurnActDisobey(attacker));
			break;
		}
		if (actData.type != eMPBattleActType.SWITCH)
		{
			if (attacker.currentMortyData.isFainted)
			{
				yield return StartCoroutine(TurnActFaint(attacker));
			}
			if (defender.currentMortyData.isFainted)
			{
				yield return StartCoroutine(TurnActFaint(defender));
			}
		}
	}

	private IEnumerator TurnActAttack(MPBattleActAttackData actData, MPBattleOpponent attacker, MPBattleOpponent defender)
	{
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_USED, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id), MPAttackDefs.Name(actData.id)), 1f));
		int hitCount = 0;
		for (int i = 0; i < actData.effectDatas.Count; i++)
		{
			if (actData.effectDatas[i].type == eMPBattleActAttackEffectType.HIT && actData.effectDatas[i].isAccurate && !actData.effectDatas[i].toSelf)
			{
				hitCount++;
			}
		}
		int succesfullEffects = 0;
		for (int j = 0; j < actData.effectDatas.Count; j++)
		{
			MPBattleActAttackEffectData effectData = actData.effectDatas[j];
			MPBattleOpponent effectDefender = ((!effectData.toSelf) ? defender : attacker);
			if (effectData.isAccurate)
			{
				succesfullEffects++;
				bool isPoisoned = effectDefender.currentMortyData.isPoisoned;
				bool isParalysed = effectDefender.currentMortyData.isParalysed;
				attacker.opponentData.SetMortiesFromDicts(effectData.attackerMortyDataDicts);
				effectDefender.opponentData.SetMortiesFromDicts(effectData.defenderMortyDataDicts);
				PlaySFXEffect(effectData.type, effectData.amount, defender.side);
				switch (effectData.type)
				{
				case eMPBattleActAttackEffectType.HIT:
					yield return StartCoroutine(TurnActAttackHitEffect(effectData, attacker, effectDefender, hitCount, actData.elementModifier));
					break;
				case eMPBattleActAttackEffectType.STAT:
					yield return StartCoroutine(TurnActAttackStatEffect(effectData, attacker, effectDefender));
					break;
				case eMPBattleActAttackEffectType.POISON:
					yield return StartCoroutine(TurnActAttackPoisonEffect(effectData, attacker, effectDefender, isPoisoned));
					break;
				case eMPBattleActAttackEffectType.PARALYSE:
					yield return StartCoroutine(TurnActAttackParalyseEffect(effectData, attacker, effectDefender, isParalysed));
					break;
				case eMPBattleActAttackEffectType.ABSORB:
					yield return StartCoroutine(TurnActAttackAbsorbEffect(effectData, attacker, effectDefender, hitCount, actData.elementModifier));
					break;
				}
				if (!_dialogue.IsReadDelayActive())
				{
					yield return new WaitForSeconds(0.5f);
				}
			}
		}
		if (succesfullEffects == 0)
		{
			if (attacker.opponentData.type == eMPOpponentType.PLAYER)
			{
				VoiceOverDefs.PlayRandom("Audio/VO/Morty - Worry/{0}", 0.5f, -0.5f);
			}
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MISSED, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
		}
		else if (hitCount > 1)
		{
			if (actData.elementModifier > 1f)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI_SUPER_EFFECTIVE, hitCount), 2f, true));
			}
			else if (actData.elementModifier < 1f)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI_NOT_VERY_EFFECTIVE, hitCount), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_MULTI, hitCount), 2f, true));
			}
		}
	}

	private IEnumerator TurnActAttackHitEffect(MPBattleActAttackEffectData effectData, MPBattleOpponent attacker, MPBattleOpponent defender, int hitCount, float elementModifier)
	{
		PlaySFXEffect(effectData.type, 0f, defender.side);
		yield return StartCoroutine(PlayMoveAnimation("PowerSwosh", defender.side));
		PlaySFXMortyHurt(defender.opponentData.type == eMPOpponentType.PLAYER);
		yield return StartCoroutine(defender.mortyInfoBar.SetHPWithTween(defender.currentMortyData.hp));
		if (attacker == defender)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_SELF_HARM, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
		}
		else if (hitCount == 1 && elementModifier > 1f)
		{
			if (effectData.isCritical)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRIT_SUPER_EFFECTIVE, 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_SUPER_EFFECTIVE, 2f, true));
			}
		}
		else if (hitCount == 1 && elementModifier < 1f)
		{
			if (effectData.isCritical)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRIT_NOT_VERY_EFFECTIVE, 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_NOT_VERY_EFFECTIVE, 2f, true));
			}
		}
		else if ((hitCount > 1 || elementModifier == 1f) && effectData.isCritical)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_CRITICAL, 2f, true));
		}
	}

	private IEnumerator TurnActAttackStatEffect(MPBattleActAttackEffectData effectData, MPBattleOpponent attacker, MPBattleOpponent defender)
	{
		if (effectData.amount > 0f)
		{
			yield return StartCoroutine(PlayMoveAnimation("Buff", defender.side));
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_BUFF, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id), MPTextDefs.GetMortyStatTextForType(effectData.stat)), 2f, true));
		}
		else
		{
			yield return StartCoroutine(PlayMoveAnimation("Debuff", defender.side));
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_DEBUFF, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id), MPTextDefs.GetMortyStatTextForType(effectData.stat)), 2f, true));
		}
	}

	private IEnumerator TurnActAttackPoisonEffect(MPBattleActAttackEffectData effectData, MPBattleOpponent attacker, MPBattleOpponent defender, bool wasPoisoned)
	{
		yield return StartCoroutine(PlayMoveAnimation("Poison", defender.side));
		if (!wasPoisoned)
		{
			defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
			if (attacker != defender)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED_SELF, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISONED_NO, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
		}
	}

	private IEnumerator TurnActAttackParalyseEffect(MPBattleActAttackEffectData effectData, MPBattleOpponent attacker, MPBattleOpponent defender, bool wasParalysed)
	{
		yield return StartCoroutine(PlayMoveAnimation("Paralyse", defender.side));
		if (!wasParalysed)
		{
			defender.mortyInfoBar.SetStatusBar(defender.currentMortyData, true);
			if (attacker != defender)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED_SELF, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
			}
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSED_NO, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
		}
	}

	private IEnumerator TurnActAttackAbsorbEffect(MPBattleActAttackEffectData effectData, MPBattleOpponent attacker, MPBattleOpponent defender, int hitCount, float elementModifier)
	{
		yield return StartCoroutine(TurnActAttackHitEffect(effectData, attacker, defender, hitCount, elementModifier));
		yield return StartCoroutine(PlayMoveAnimation("Absorb", attacker.side));
		yield return StartCoroutine(attacker.mortyInfoBar.SetHPWithTween(attacker.currentMortyData.hp));
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_ABSORB, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id), effectData.amount), 2f, true));
	}

	private IEnumerator TurnActItem(MPBattleActItemData actData, MPBattleOpponent attacker, MPBattleOpponent defender)
	{
		MPOpponentMortyData mortyData = attacker.GetMortyData(actData.mortyNetId);
		if (mortyData == null)
		{
			mortyData = attacker.currentMortyData;
		}
		if (mortyData == attacker.currentMortyData)
		{
			if (!MPItemDefs.UseOnSelf(actData.id))
			{
				_dialogue.TweenText(string.Format(TextDefs.BATTLE_ITEM_USED, attacker.pronounYou, MPItemDefs.Name(actData.id)));
			}
			else
			{
				_dialogue.TweenText(string.Format(TextDefs.BATTLE_ITEM_USED_ON, attacker.pronounYou, MPItemDefs.Name(actData.id), MPMortyDefs.Name(attacker.currentMortyData.id)));
			}
			if (actData.id.Equals("ItemMrMeeseek"))
			{
				yield return new WaitForSeconds(2f);
				_dialogue.TweenText(TextDefs.ITEM_APPLY_MR_MEESEEK);
			}
			PlaySFXUseItem(actData.id);
			yield return StartCoroutine(PlayMoveAnimation(MPItemDefs.AttackerAnimation(actData.id), attacker.side));
			yield return StartCoroutine(PlayMoveAnimation(MPItemDefs.DefenderAnimation(actData.id), defender.side));
			switch (MPItemDefs.EffectType(actData.id))
			{
			case "Capture":
				yield return StartCoroutine(UseItemCapture(actData.isSuccess));
				break;
			case "RestoreHP":
			case "Revive":
			case "Cure":
				yield return StartCoroutine(attacker.mortyInfoBar.SetHPWithTween(attacker.currentMortyData.hp));
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
			App.sound.PlaySFX("Audio/SFX/UseItem");
			if (!MPItemDefs.UseOnSelf(actData.id))
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_USED, attacker.pronounYou, MPItemDefs.Name(actData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_USED_ON, attacker.pronounYou, MPItemDefs.Name(actData.id), MPMortyDefs.Name(mortyData.id)), 2f, true));
			}
		}
		string resultText = MPItemDefs.GetItemUsedResultText(actData.id, actData.isSuccess, actData.effectValue, mortyData.id, defender.currentMortyData.id, attacker.pronounYours, actData.mortyRewardData);
		if (resultText != null)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(resultText, 2f, true));
		}
	}

	private IEnumerator UseItemCapture(bool isSuccess)
	{
		if (isSuccess)
		{
			yield return StartCoroutine(PlayMoveAnimation("CaptureSuccess", _enemy.side));
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Happy/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
			if (!App.net.player.data.mortydexCaught.Contains(_enemy.currentMortyData.id))
			{
				App.net.player.data.mortydexCaught.Add(_enemy.currentMortyData.id);
			}
			_enemy.mortyInfoBar.SetCaughtIndicator(eCaughtIndicatorType.OWNED, true);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ITEM_CAPTURED, MPMortyDefs.Name(_enemy.currentMortyData.id)), 2f, true));
		}
		else
		{
			yield return StartCoroutine(PlayMoveAnimation("CaptureFailed", _enemy.side));
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Moan/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
			yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ITEM_CAPTURE_FAILED, 2f, true));
		}
	}

	private IEnumerator TurnActSwitch(MPBattleActSwitchData actData, MPBattleOpponent attacker)
	{
		yield return StartCoroutine(SwitchBringMortyIn(attacker));
		yield return StartCoroutine(SwitchSendMortyOut(attacker, attacker.GetMortyData(actData.mortyNetId)));
	}

	private IEnumerator SwitchSendMortyOut(MPBattleOpponent opponent, MPOpponentMortyData mortyData)
	{
		if (!App.net.player.data.mortydexSeen.Contains(mortyData.id))
		{
			App.net.player.data.mortydexSeen.Add(mortyData.id);
		}
		opponent.currentMortyData = mortyData;
		yield return StartCoroutine(opponent.morty.SetImage(MPMortyDefs.AssetID(mortyData.id)));
		StartCoroutine(opponent.morty.TweenInCo());
		eCaughtIndicatorType caughtIndicatorType = eCaughtIndicatorType.NONE;
		if (opponent.opponentData.type == eMPOpponentType.WILD_MORTY)
		{
			caughtIndicatorType = ((App.net.player.data.morties.FirstOrDefault((MPOpponentMortyData e) => e.id == opponent.currentMortyData.id) != null) ? eCaughtIndicatorType.OWNED : ((!App.net.player.data.mortydexCaught.Contains(opponent.currentMortyData.id)) ? eCaughtIndicatorType.NOT_CAUGHT : eCaughtIndicatorType.CAUGHT));
		}
		opponent.mortyInfoBar.SetAll(opponent.opponentData.GetAllPartyMortyDatas(), mortyData);
		opponent.mortyInfoBar.SetCaughtIndicator(caughtIndicatorType);
		opponent.mortyInfoBar.TweenIn();
		App.sound.PlaySFX("Audio/SFX/SendMortyOut");
		if (opponent.opponentData.type == eMPOpponentType.PLAYER)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Send Morty Out/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 1f);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_PLAYER_MORTY_OUT, MPMortyDefs.Name(mortyData.id)), 2f));
			yield break;
		}
		if (opponent.opponentData.type == eMPOpponentType.WILD_MORTY)
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_WILD_MORTY_OUT, MPMortyDefs.Name(mortyData.id)), 2f));
			yield break;
		}
		if (UnityEngine.Random.value > 0.5f)
		{
			VoiceOverDefs.PlayRandom("Audio/VO/Morty - Worry/{0}", 0.5f, -0.5f);
		}
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_ENEMY_MORTY_OUT, _enemy.opponentData.GetUsername(), MPMortyDefs.Name(mortyData.id)), 2f, true));
	}

	private IEnumerator SwitchBringMortyIn(MPBattleOpponent opponent)
	{
		opponent.mortyInfoBar.TweenOut();
		if (!opponent.currentMortyData.isFainted)
		{
			StartCoroutine(opponent.morty.TweenOutCo());
			App.sound.PlaySFX("Audio/SFX/BringMortyIn");
			if (opponent.opponentData.type == eMPOpponentType.PLAYER)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_PLAYER_MORTY_IN, MPMortyDefs.Name(opponent.currentMortyData.id)), 2f, true));
			}
			else
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_SWITCH_ENEMY_MORTY_IN, _enemy.opponentData.GetUsername(), MPMortyDefs.Name(opponent.currentMortyData.id)), 1f));
			}
		}
		else
		{
			opponent.morty.SetPositionOut();
		}
	}

	private IEnumerator TurnActPoisoned(MPBattleActData actData, MPBattleOpponent defender)
	{
		PlaySFXEffect(eMPBattleActAttackEffectType.POISON, 0f, defender.side);
		yield return StartCoroutine(PlayMoveAnimation("Poison", defender.side));
		yield return StartCoroutine(defender.mortyInfoBar.SetHPWithTween(defender.currentMortyData.hp));
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_POISON, defender.pronounYours, MPMortyDefs.Name(defender.currentMortyData.id)), 2f, true));
	}

	private IEnumerator TurnActParalysed(MPBattleActParalysedData actData, MPBattleOpponent attacker)
	{
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_USED, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id), MPAttackDefs.Name(actData.attackId)), 1f));
		StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_ATTACK_PARALYSE, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id)), 2f, true));
		PlaySFXEffect(eMPBattleActAttackEffectType.PARALYSE, 0f, attacker.side);
		yield return StartCoroutine(PlayMoveAnimation("Paralyse", attacker.side));
	}

	public IEnumerator TurnActDisobey(MPBattleOpponent attacker)
	{
		if (attacker.opponentData.type == eMPOpponentType.PLAYER)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Morty Put Down/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_DISOBEY_PLAYER, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id)), 4f, true));
		}
		else
		{
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_DISOBEY_ENEMY, attacker.pronounYours, MPMortyDefs.Name(attacker.currentMortyData.id)), 4f, true));
		}
	}

	private IEnumerator TurnActFaint(MPBattleOpponent attacker)
	{
		attacker.mortyInfoBar.SetParty(attacker.opponentData.GetAllPartyMortyDatas());
		yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(TextDefs.BATTLE_FAINTED, MPMortyDefs.Name(attacker.currentMortyData.id))));
		yield return StartCoroutine(attacker.mortyInfoBar.TweenOutCo());
		App.sound.PlaySFX("Audio/SFX/Dazed", 1f, false, 1f, 0f, 0.5f);
		VoiceOverDefs.PlayRandom("Audio/VO/Morty - Dazed/{0}", 0.5f, GetVOPan(attacker.side), 0.8f);
		if (attacker.opponentData.GetPartyFirstAliveMortyData() != null)
		{
			if (attacker.opponentData.type == eMPOpponentType.PLAYER)
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Morty Put Down/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
			}
			else
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Complement/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
			}
		}
		yield return StartCoroutine(PlayMoveAnimation("Faint", attacker.side));
	}

	private void PlayerConfirmRun()
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		string text = ((!_enemy.IsTrainer() || App.net.player.data.xpMultiplier <= 0) ? TextDefs.BATTLE_POPUP_RUN_DESCRIPTION : MPTextDefs.MP_BATTLE_POPUP_RUN_DESCRIPTION_XP_MULTI);
		genericPopupState.SetDisplayValues(text, new string[1] { TextDefs.BATTLE_POPUP_RUN_BTN_YES });
		genericPopupState.ButtonClickedEvent += GenericPopupRunConfirmationHandler;
		App.states.Set<GenericPopupState>(null, new PopTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private IEnumerator AwardXPDatas(MPBattleOpponent attacker, MPMortyXpData[] mortyXpDatas)
	{
		if (mortyXpDatas == null)
		{
			yield break;
		}
		int currentMortyIndex = Array.IndexOf(mortyXpDatas, mortyXpDatas.FirstOrDefault((MPMortyXpData e) => e.netId == attacker.currentMortyData.netId));
		if (currentMortyIndex > -1)
		{
			mortyXpDatas.Swap(currentMortyIndex, 0);
		}
		foreach (MPMortyXpData mortyXpData in mortyXpDatas)
		{
			MPOpponentMortyData mortyData = attacker.GetMortyData(mortyXpData.netId);
			if (attacker.side == eBattleOpponentSide.LEFT)
			{
				string text = ((mortyData != attacker.currentMortyData) ? TextDefs.BATTLE_REWARD_XP_OTHER : TextDefs.BATTLE_REWARD_XP_CURRENT);
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(text, MPMortyDefs.Name(mortyData.id), mortyXpData.xpEarned), 2f, true));
			}
			yield return StartCoroutine(ApplyXPDataMortyDict(attacker, mortyData, mortyXpData.mortyDatasDicts));
		}
	}

	private IEnumerator ApplyXPDataMortyDict(MPBattleOpponent attacker, MPOpponentMortyData mortyData, Dictionary<string, object>[] xpDataMortyDicts)
	{
		bool didCurrentMortyGetSwapped = false;
		for (int i = 0; i < xpDataMortyDicts.Length; i++)
		{
			int oldLevel = mortyData.level;
			int oldHpStat = mortyData.hpStat;
			int oldAttackStat = mortyData.attackStat;
			int oldDefenceStat = mortyData.defenceStat;
			int oldSpeedStat = mortyData.speedStat;
			mortyData.SetFromDict(xpDataMortyDicts[i]);
			if (attacker.side == eBattleOpponentSide.LEFT)
			{
				App.sound.PlaySFX("Audio/SFX/GainXP", 0.5f);
			}
			if (didCurrentMortyGetSwapped || mortyData == attacker.currentMortyData)
			{
				yield return StartCoroutine(attacker.mortyInfoBar.SetXPWithTween(mortyData.xp));
				attacker.mortyInfoBar.SetLevel(mortyData.level);
				attacker.mortyInfoBar.SetHPMax(mortyData.hpStat, mortyData.hp);
			}
			if (mortyData.level > oldLevel)
			{
				yield return StartCoroutine(ApplyXpDataLevel(attacker, mortyData, oldHpStat, oldAttackStat, oldDefenceStat, oldSpeedStat, !didCurrentMortyGetSwapped));
				didCurrentMortyGetSwapped = mortyData != attacker.currentMortyData;
			}
		}
		if (didCurrentMortyGetSwapped)
		{
			attacker.mortyInfoBar.TweenOut();
			yield return StartCoroutine(attacker.morty.TweenOutCo());
			attacker.mortyInfoBar.SetAll(attacker.opponentData.GetAllPartyMortyDatas(), attacker.currentMortyData);
			yield return StartCoroutine(attacker.morty.SetImage(MPMortyDefs.AssetID(attacker.currentMortyData.id)));
			attacker.morty.TweenIn();
			yield return StartCoroutine(attacker.mortyInfoBar.TweenInCo());
		}
	}

	private IEnumerator ApplyXpDataLevel(MPBattleOpponent attacker, MPOpponentMortyData mortyData, int oldHpStat, int oldAttackStat, int oldDefenceStat, int oldSpeedStat, bool swapIfNotCurrent)
	{
		StopMortyIdleTweens();
		string resultText = string.Format(TextDefs.BATTLE_REWARD_LEVEL, attacker.pronounYours, MPMortyDefs.Name(mortyData.id), mortyData.level);
		_dialogue.TweenText(resultText);
		App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
		int hpStatDif = mortyData.hpStat - oldHpStat;
		int attackStatDif = mortyData.attackStat - oldAttackStat;
		int defenceStatDif = mortyData.defenceStat - oldDefenceStat;
		int speedStatDif = mortyData.speedStat - oldSpeedStat;
		_levelUp.SetStats(mortyData.hpStat, hpStatDif, mortyData.attackStat, attackStatDif, mortyData.defenceStat, defenceStatDif, mortyData.speedStat, speedStatDif);
		if (swapIfNotCurrent && mortyData != attacker.currentMortyData)
		{
			attacker.mortyInfoBar.SetPositionOut();
			attacker.morty.SetPositionOut();
			attacker.mortyInfoBar.SetAll(attacker.opponentData.GetAllPartyMortyDatas(), mortyData);
			yield return StartCoroutine(attacker.morty.SetImage(MPMortyDefs.AssetID(mortyData.id), true));
			attacker.morty.TweenIn();
			attacker.mortyInfoBar.TweenIn();
		}
		if (attacker.side == eBattleOpponentSide.LEFT)
		{
			App.states.FlashCurrentState(0.1f);
			yield return StartCoroutine(_levelUp.TweenBackground(attacker.side));
		}
		if (attacker.side == eBattleOpponentSide.LEFT)
		{
			yield return StartCoroutine(_levelUp.TweenStatsBoxIn());
		}
		yield return StartCoroutine(_dialogue.SetTextCo(resultText, 2f, true));
		if (attacker.side == eBattleOpponentSide.LEFT)
		{
			yield return StartCoroutine(_levelUp.TweenStatsBoxOut());
		}
		int xpTargetLow = MPMortyDefs.GetXPTarget(mortyData.level);
		int xpTargetHigh = MPMortyDefs.GetXPTarget(mortyData.level + 1);
		attacker.mortyInfoBar.SetXPTarget(xpTargetLow, xpTargetHigh, mortyData.xp);
		StartMortyIdleTweens();
	}

	public void SelectingMoveRequest()
	{
		_player.mortyInfoBar.TweenToStartingY();
		StartCoroutine(ShowMainMenu());
		if (this.SelectingMoveEvent != null)
		{
			this.SelectingMoveEvent();
		}
	}

	public void SelectingSwitchMoveRequest()
	{
		_player.mortyInfoBar.TweenToStartingY();
		StartCoroutine(ShowSwitchMenu());
		if (this.SelectingMoveEvent != null)
		{
			this.SelectingMoveEvent();
		}
	}

	private void SelectMoveRequest(eMPBattleMoveType type, string mortyNetId, string moveId)
	{
		if (this.MoveSelectedEvent != null)
		{
			this.MoveSelectedEvent(type, mortyNetId, moveId);
		}
	}

	public void MoveSelected()
	{
		_moveTimer.Show(false);
		if (base.gameObject.activeInHierarchy)
		{
			_dialogue.TweenText(MPTextDefs.MP_BATTLE_WAITING_FOR_ENEMY);
			_player.mortyInfoBar.TweenDownY();
			HideAllMenusTween();
		}
		else
		{
			_dialogue.SetText(MPTextDefs.MP_BATTLE_WAITING_FOR_ENEMY);
			_player.mortyInfoBar.SetPositionDownY();
			HideAllMenus();
		}
	}

	private void TurnFinishedRequest()
	{
		if (this.TurnFinishedEvent != null)
		{
			this.TurnFinishedEvent();
		}
	}

	private MPBattleOpponent GetOpponent(string netId)
	{
		if (_player.opponentData.netId == netId)
		{
			return _player;
		}
		if (_enemy.opponentData.netId == netId)
		{
			return _enemy;
		}
		return null;
	}

	private void HideAllMenus()
	{
		_mainMenu.SetPositionOut();
		_attackMenu.SetPositionOut();
		_attackStruggleMenu.SetPositionOut();
		_switchMenu.SetPositionOut();
	}

	private void HideAllMenusTween()
	{
		StartCoroutine(HideMainMenu());
		StartCoroutine(HideAttackMenu());
		StartCoroutine(HideAttackStruggleMenu());
		StartCoroutine(HideSwitchMenu());
	}

	private void SetAllMenusInteractable(bool isInteractable)
	{
		_mainMenu.SetInteractable(isInteractable);
		_attackMenu.SetInteractable(isInteractable);
		_attackStruggleMenu.SetInteractable(isInteractable);
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

	private IEnumerator MainToAttackStruggleMenu()
	{
		yield return StartCoroutine(HideMainMenu());
		yield return StartCoroutine(ShowAttackStruggleMenu(_player.currentMortyData));
	}

	private IEnumerator AttackStruggleToMainMenu()
	{
		yield return StartCoroutine(HideAttackStruggleMenu());
		yield return StartCoroutine(ShowMainMenu());
	}

	protected IEnumerator ShowMainMenu()
	{
		StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_MAIN_MENU));
		yield return StartCoroutine(_mainMenu.TweenIn());
	}

	private IEnumerator HideMainMenu()
	{
		yield return StartCoroutine(_mainMenu.TweenOut());
	}

	private IEnumerator ShowSwitchMenu()
	{
		StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_SWITCH_PLAYER_WANT_TO));
		yield return StartCoroutine(_switchMenu.TweenIn());
	}

	private IEnumerator HideSwitchMenu()
	{
		yield return StartCoroutine(_switchMenu.TweenOut());
	}

	protected IEnumerator ShowAttackMenu(MPOpponentMortyData mortyData)
	{
		RefreshAttackMenu(mortyData);
		yield return StartCoroutine(_attackMenu.TweenIn());
	}

	private IEnumerator HideAttackMenu()
	{
		yield return StartCoroutine(_attackMenu.TweenOut());
	}

	private void RefreshAttackMenu(MPOpponentMortyData mortyData)
	{
		for (int i = 0; i < 4; i++)
		{
			if (mortyData.attacks.ContainsKey(i))
			{
				_attackMenu.SetBtnEnabled(i, true);
				_attackMenu.SetUpBtn(i, new AttackDisplayInfo(mortyData.attacks[i].id, mortyData.attacks[i].PP));
			}
			else
			{
				_attackMenu.SetBtnEnabled(i, false);
			}
		}
	}

	protected IEnumerator ShowAttackStruggleMenu(MPOpponentMortyData mortyData)
	{
		_attackStruggleMenu.SetUpBtn(0, new AttackDisplayInfo("AttackStruggle"), false);
		StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_ATTACK_STRUGGLE_MENU, MortyDefs.Name(mortyData.id))));
		yield return StartCoroutine(_attackStruggleMenu.TweenIn());
	}

	private IEnumerator HideAttackStruggleMenu()
	{
		yield return StartCoroutine(_attackStruggleMenu.TweenOut());
	}

	public string GetDialogueText()
	{
		return _dialogue.GetText();
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

	public void ResetEmoteBubbles()
	{
		_enemyEmoteBubble.SnapOff();
		_playerEmoteBubble.SnapOff();
	}

	public void DisplayEnemyEmote(string emoteId, string otherUserId)
	{
		if (_enemy.opponentData.netId == otherUserId)
		{
			_enemyEmoteBubble.AnimateOn(emoteId);
		}
	}

	public void DisplayPlayerEmote(string emoteId)
	{
		_playerEmoteBubble.AnimateOn(emoteId);
	}

	private float GetVOPan(eBattleOpponentSide side)
	{
		return (side != eBattleOpponentSide.LEFT) ? 0.5f : (-0.5f);
	}

	private void PlaySFXRickEngage()
	{
		VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Engage/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f);
	}

	private void PlaySFXRickWin()
	{
		VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Win/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f);
	}

	private void PlaySFXRickLose()
	{
		if (_enemy.opponentData.type == eMPOpponentType.WILD_MORTY)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Morty Put Down/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
		}
		else
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Lose Trainer/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
		}
	}

	private void PlaySFXRickHappy()
	{
		VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Happy/{0}/{1}", App.net.player.data.avatarCategory, 0.75f, -0.5f, 0.8f);
	}

	private void PlaySFXEffect(eMPBattleActAttackEffectType type, float value, eBattleOpponentSide side)
	{
		switch (type)
		{
		case eMPBattleActAttackEffectType.HIT:
			App.sound.PlaySFX("Audio/SFX/PowerAttack", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eMPBattleActAttackEffectType.STAT:
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
		case eMPBattleActAttackEffectType.POISON:
			App.sound.PlaySFX("Audio/SFX/Poison", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eMPBattleActAttackEffectType.PARALYSE:
			App.sound.PlaySFX("Audio/SFX/Paralyse", 1f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		case eMPBattleActAttackEffectType.ABSORB:
			App.sound.PlaySFX("Audio/SFX/Absorb", 0.65f, false, 1f, 0f, 0f, true, null, GetVOPan(side));
			break;
		}
	}

	private void PlaySFXMortyHurt(bool isPlayer)
	{
		if (isPlayer)
		{
			VoiceOverDefs.PlayRandom("Audio/VO/Morty - Hurt/{0}", 0.5f, 0.5f);
		}
		else if (UnityEngine.Random.value > 0.3f)
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
		if (!string.IsNullOrEmpty(MPItemDefs.AttackerAnimation(itemID)) || !string.IsNullOrEmpty(MPItemDefs.DefenderAnimation(itemID)))
		{
			switch (itemID)
			{
			case "ItemMrMeeseek":
				App.sound.PlaySFX("Audio/SFX/MeeseekAttack");
				break;
			case "ItemMortyChip":
				App.sound.PlaySFX("Audio/SFX/UseChipItem", 1f, false, 1f, 0f, 1.2f);
				break;
			case "ItemPoisonCure":
			case "ItemParalysisCure":
				App.sound.PlaySFX("Audio/SFX/UseItemSpray", 1f, false, 1f, 0f, 1.2f);
				break;
			default:
				App.sound.PlaySFX("Audio/SFX/UseItem", 1f, false, 1f, 0f, 1.3f);
				break;
			}
		}
	}

	private void MainMenuBtnPressedHandler(int index)
	{
		StartCoroutine(_playerEmoteBar.AnimateOffCo());
		switch (index)
		{
		case 0:
			if (_player.currentMortyData.GetTotalRemainingPP() > 0)
			{
				StartCoroutine(MainToAttackMenu());
			}
			else
			{
				StartCoroutine(MainToAttackStruggleMenu());
			}
			break;
		case 1:
			if (this.ItemBtnEvent != null)
			{
				this.ItemBtnEvent();
			}
			break;
		case 2:
			if (this.SwitchBtnEvent != null)
			{
				this.SwitchBtnEvent();
			}
			break;
		case 3:
			PlayerConfirmRun();
			break;
		}
	}

	private void AttackMenuBtnPressedHandler(int index)
	{
		StartCoroutine(_playerEmoteBar.AnimateOffCo());
		if (index == 4)
		{
			StartCoroutine(AttackToMainMenu());
		}
		else if (_player.currentMortyData.attacks[index].PP > 0)
		{
			SelectMoveRequest(eMPBattleMoveType.ATTACK, _player.currentMortyData.netId, _player.currentMortyData.attacks[index].id);
		}
		else
		{
			StartCoroutine(_dialogue.TweenTextCo(TextDefs.BATTLE_ATTACK_OUT_OF_PP));
		}
	}

	protected void AttackMenuBtnLongPressStartHandler(int index)
	{
	}

	private void AttackStruggleMenuBtnPressedHandler(int index)
	{
		if (index == 1)
		{
			StartCoroutine(AttackStruggleToMainMenu());
		}
		else
		{
			SelectMoveRequest(eMPBattleMoveType.ATTACK, _player.currentMortyData.netId, "AttackStruggle");
		}
	}

	private void SwitchMenuBtnPressedHandler(int index)
	{
		StartCoroutine(_playerEmoteBar.AnimateOffCo());
		switch (index)
		{
		case 0:
			if (this.SwitchBtnEvent != null)
			{
				this.SwitchBtnEvent();
			}
			break;
		case 1:
			SelectMoveRequest(eMPBattleMoveType.NONE, _player.currentMortyData.netId, null);
			break;
		}
	}

	private void EmoteSelectedHandler(string emoteId)
	{
		if (this.EmoteSelectedEvent != null)
		{
			this.EmoteSelectedEvent(_enemy.opponentData.netId, emoteId);
		}
		DisplayPlayerEmote(emoteId);
	}

	private void EmoteBarAnimateOnBeginHandler()
	{
		_emoteBtnObj.SetActive(false);
	}

	private void EmoteBarAnimateOffCompleteHandler()
	{
		_emoteBtnObj.SetActive(true);
	}

	private void GenericPopupRunConfirmationHandler(int buttonIndex)
	{
		GenericPopupState genericPopupState = App.states.Get<GenericPopupState>();
		genericPopupState.ButtonClickedEvent -= GenericPopupRunConfirmationHandler;
		App.states.SetBack(new PopTransInfo());
		if (buttonIndex == 1 && this.RunBtnEvent != null)
		{
			this.RunBtnEvent();
		}
	}

	public void EmoteButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		StartCoroutine(_playerEmoteBar.AnimateOnCo());
	}
}
