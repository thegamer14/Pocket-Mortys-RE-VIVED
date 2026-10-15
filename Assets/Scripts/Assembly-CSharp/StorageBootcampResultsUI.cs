using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBootcampResultsUI : UIBase
{
	public delegate void StorageBootcampResultsUIDelegate();

	protected Transform _containerTransform;

	private BattleBackground _background;

	private BattleLevelUp _levelUp;

	private AttackLearnNewBox _newAttackBox;

	private Transform _attackLearnContainer;

	private BattleMortyInfoBar _mortyInfoBar;

	private BattleMorty _battleMorty;

	private MenuDialogue _dialogue;

	private BattleAttackMenu _attackMenu;

	private int _levelUpSwapAttackIndex;

	public event StorageBootcampResultsUIDelegate BackEvent;

	protected override void LateAwake()
	{
		_containerTransform = base.transform.FindChild("Container").transform;
		_background = _containerTransform.FindChild("Background").GetComponent<BattleBackground>();
		_levelUp = _containerTransform.FindChild("LevelUp").GetComponent<BattleLevelUp>();
		_attackLearnContainer = _containerTransform.FindChild("AttackLearnContainer");
		_mortyInfoBar = _containerTransform.FindChild("PlayerMortyInfoBar").GetComponent<BattleMortyInfoBar>();
		_battleMorty = _containerTransform.FindChild("PlayerMorty").GetComponent<BattleMorty>();
		_dialogue = _containerTransform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_attackMenu = _containerTransform.FindChild("MenuBar/AttackMenu").GetComponent<BattleAttackMenu>();
	}

	public void SetUp()
	{
		_levelUpSwapAttackIndex = -1;
		_mortyInfoBar.moveDirection = -1;
		_mortyInfoBar.SetPositionDownY();
		_mortyInfoBar.SetPositionOut();
		_battleMorty.SetPositionOut();
		_attackMenu.SetPositionOut();
		_dialogue.SetText(string.Empty);
		string worldID = SharedStatInfo.worldInfo.worldID;
		string nodeSetID = SharedStatInfo.worldInfo.nodeSetID;
		if ((!string.IsNullOrEmpty(worldID) && !worldID.Equals("Citadel")) || (!string.IsNullOrEmpty(nodeSetID) && !nodeSetID.Equals("Citadel")))
		{
			_background.SetBackground(worldID, nodeSetID);
		}
		else
		{
			_background.SetBackground("Summer", "Summer");
		}
	}

	protected override void AddEvents()
	{
		_attackMenu.BtnPressedEvent += AttackMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_attackMenu.BtnPressedEvent -= AttackMenuBtnPressedHandler;
	}

	public IEnumerator RewardXP(MortyData mortyData, int xpReward)
	{
		_battleMorty.TweenOut();
		yield return StartCoroutine(_mortyInfoBar.TweenOutCo());
		yield return StartCoroutine(_battleMorty.SetImage(mortyData.id, true));
		_battleMorty.TweenIn();
		_mortyInfoBar.SetPositionDownY();
		_mortyInfoBar.SetAll(null, mortyData);
		_mortyInfoBar.SetPartyActive(false);
		_mortyInfoBar.TweenIn();
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
				string rewardText = string.Format(TextDefs.BATTLE_REWARD_XP_CURRENT, MortyDefs.Name(mortyData.id), xpReward);
				yield return StartCoroutine(_dialogue.TweenTextCo(rewardText, 2f, true));
			}
			App.sound.PlaySFX("Audio/SFX/GainXP", 0.5f);
			mortyData.xp += xpToAdd;
			yield return StartCoroutine(_mortyInfoBar.SetXPWithTween(mortyData.xp));
			if (targetXP == mortyData.xp)
			{
				if (mortyData.level >= 100)
				{
					break;
				}
				int oldHPMax = mortyData.hpStat;
				int oldAttackMax = mortyData.attackStat;
				int oldDefenceMax = mortyData.defenceStat;
				int oldSpeedMax = mortyData.speedStat;
				string resultText;
				MortyDefs.IncrementLevel(mortyData, false, TextDefs.PRONOUN_PLAYERS, out resultText);
				_mortyInfoBar.SetLevel(mortyData.level);
				_mortyInfoBar.SetHPMax(mortyData.hpStat, mortyData.hp);
				_dialogue.TweenText(resultText);
				App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
				_levelUp.SetStats(mortyData.hpStat, mortyData.hpStat - oldHPMax, mortyData.attackStat, mortyData.attackStat - oldAttackMax, mortyData.defenceStat, mortyData.defenceStat - oldDefenceMax, mortyData.speedStat, mortyData.speedStat - oldSpeedMax);
				App.states.FlashCurrentState(0.1f);
				yield return StartCoroutine(_levelUp.TweenBackground());
				yield return StartCoroutine(_levelUp.TweenStatsBoxIn());
				yield return StartCoroutine(_dialogue.SetTextCo(resultText, 2f, true));
				yield return StartCoroutine(_levelUp.TweenStatsBoxOut());
				yield return StartCoroutine(MortyLeveledUpLearnAttack(mortyData));
			}
			_mortyInfoBar.SetXPTarget(MortyDefs.GetXPTarget(mortyData.level), MortyDefs.GetXPTarget(mortyData.level + 1));
			_mortyInfoBar.SetXP(mortyData.xp);
			xpRewardRemaining -= xpToAdd;
		}
	}

	private IEnumerator MortyLeveledUpLearnAttack(MortyData mortyData)
	{
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
				_mortyInfoBar.TweenToStartingY();
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
			_mortyInfoBar.TweenDownY();
			StartCoroutine(HideAttackMenu());
			_levelUpSwapAttackIndex = -1;
		}
	}

	private void SetMenusInteractable(bool isInteractable)
	{
		_attackMenu.SetInteractable(isInteractable);
	}

	protected virtual IEnumerator ShowAttackMenu(MortyData mortyData)
	{
		RefreshAttackMenu(mortyData);
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

	private void AttackMenuBtnPressedHandler(int index)
	{
		_levelUpSwapAttackIndex = index;
	}
}
