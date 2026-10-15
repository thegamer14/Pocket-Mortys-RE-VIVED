using System.Collections;
using UnityEngine;

public class MPPlayerLevelUI : UIBase
{
	public delegate void UIDelegate();

	public delegate void UIRewardDelegate(MPRewardData[] data);

	public UIDelegate FinishedEvent;

	public UIRewardDelegate RewardEvent;

	private BattleBackground _background;

	private BattleTrainer _trainer;

	private MPPlayerLevelBarUI _levelBar;

	private MenuDialogue _dialogue;

	protected MPPlayerXpData _playerXpData;

	protected int _currentIncrementIndex;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background").GetComponent<BattleBackground>();
		_trainer = base.transform.FindChild("Trainer").GetComponent<BattleTrainer>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MPPlayerLevelBarUI>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(MPPlayerXpData playerXpData, string backgroundId, string trainerAssetId, string startingDialogueStr)
	{
		_background.SetBackground(MPWorldDefs.GetResourceId(backgroundId));
		_trainer.SetUp(trainerAssetId, eBattleOpponentSide.LEFT);
		_trainer.SetPositionOut();
		_levelBar.SetUp();
		_levelBar.SetPositionOut();
		_dialogue.SetText(startingDialogueStr);
		_playerXpData = playerXpData;
		_currentIncrementIndex = -1;
	}

	public void TransitionIn()
	{
		StartCoroutine(DisplayPlayerXpData());
	}

	public IEnumerator DisplayPlayerXpData()
	{
		_currentIncrementIndex++;
		if (_currentIncrementIndex == 0)
		{
			yield return StartCoroutine(DisplayPlayerXpDataIntro());
			if (_playerXpData.xpEarned > 0)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_PLAYER_XP_EARNED, _playerXpData.xpEarned), 2f, true));
			}
		}
		if (_currentIncrementIndex < _playerXpData.increments.Length)
		{
			MPPlayerXpIncrementData incrementData = _playerXpData.increments[_currentIncrementIndex];
			yield return StartCoroutine(DisplayPlayerXpIncrementData(incrementData));
			if (incrementData.rewards != null)
			{
				if (RewardEvent != null)
				{
					RewardEvent(incrementData.rewards);
				}
				yield break;
			}
		}
		yield return StartCoroutine(DisplayPlayerXpMultiplier());
		yield return new WaitForSeconds(1f);
		FinishedEvent();
	}

	public IEnumerator DisplayPlayerXpDataIntro()
	{
		yield return StartCoroutine(_trainer.TweenInPlayerLevelCo());
		yield return StartCoroutine(_levelBar.TweenInCo());
	}

	public IEnumerator DisplayPlayerXpIncrementData(MPPlayerXpIncrementData incrementData)
	{
		App.sound.PlaySFX("Audio/SFX/GainXP", 0.5f);
		yield return StartCoroutine(_levelBar.SetXPWithTween(incrementData.xp));
		if (incrementData.level > _levelBar.level)
		{
			_levelBar.SetLevel(incrementData.level);
			_levelBar.SetXPTarget(incrementData.level, incrementData.level + 1, incrementData.xp);
			App.sound.PlaySFX("Audio/SFX/LevelUP", 0.5f);
			yield return StartCoroutine(_dialogue.TweenTextCo(string.Format(MPTextDefs.MP_BATTLE_PLAYER_LEVEL_EARNED, incrementData.level), 2f, true));
			if (!App.saveData.multiplayer.seenXpTutorial)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_PLAYER_XP_EARNED_TUTORIAL_1, -1f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_PLAYER_XP_EARNED_TUTORIAL_2, -1f, true));
				App.saveData.multiplayer.seenXpTutorial = true;
			}
		}
	}

	public IEnumerator DisplayPlayerXpMultiplier()
	{
		if (_playerXpData.xpMultiplier > App.net.player.data.xpMultiplier)
		{
			yield return new WaitForSeconds(0.5f);
			App.sound.PlaySFX("Audio/SFX/PickupItem");
			yield return StartCoroutine(_levelBar.TweenMultiplier(_playerXpData.xpMultiplier));
			yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_XP_MULTIPLIER_EARNED, 2f, true));
			if (!App.saveData.multiplayer.seenXpMultiplierTutorial)
			{
				yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_XP_MULTIPLIER_TUTORIAL_1, -1f, true));
				yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_XP_MULTIPLIER_TUTORIAL_2, -1f, true));
				App.saveData.multiplayer.seenXpMultiplierTutorial = true;
			}
		}
		else if (_playerXpData.xpMultiplier == 0 && App.net.player.data.xpMultiplier > 0)
		{
			yield return new WaitForSeconds(0.5f);
			App.sound.PlaySFX("Audio/SFX/UseItem");
			yield return StartCoroutine(_levelBar.TweenMultiplerToZero());
			yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_BATTLE_XP_MULTIPLIER_RESET, 2f, true));
		}
	}
}
