using System.Collections;
using UnityEngine;

public class MPVideoRewardMeeseekUI : UIBase
{
	public delegate void BtnDelegate();

	public delegate void BtnFinishDelegate(bool isSuccess, bool isDismissed);

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private GameObject _meeseek;

	private GameObject _pop;

	private Animator _animator;

	private string _type;

	public event BtnDelegate WatchEvent;

	public event BtnFinishDelegate FinishedEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_meeseek = base.transform.FindChild("Meeseek").gameObject;
		_pop = base.transform.FindChild("Pop").gameObject;
		_animator = _pop.GetComponent<Animator>();
	}

	public void SetUp(string type)
	{
		_type = type;
		_animator.enabled = false;
		_animator.enabled = true;
		_animator.Rebind();
		_animator.Update(0f);
		SetDefaultState();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.VIDEO_WATCH_BTN);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetDefaultState()
	{
		_pop.SetActive(false);
		switch (_type)
		{
		case "PvPWinAd":
			_dialogue.SetText(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_BATTLE_WIN);
			break;
		case "PvPLoseAd":
			_dialogue.SetText(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_BATTLE_LOSE);
			break;
		case "NPCAd":
			_dialogue.SetText(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_NPC);
			break;
		}
	}

	public void SetWaitingVideoState()
	{
		StartCoroutine(_mainMenu.TweenOut());
		_dialogue.TweenText(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_WAITING_VIDEO);
	}

	public void SetFailedVideoState()
	{
		StartCoroutine(FinishWithDialogue(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_VIDEO_FAILED, false));
	}

	public void SetNoVideoState()
	{
		StartCoroutine(FinishWithDialogue(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_NO_VIDEO, false));
	}

	public void SetWaitingRewardState()
	{
		StartCoroutine(_mainMenu.TweenOut());
		_dialogue.TweenText(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_WAITING_REWARDS);
	}

	public void SetFailedRewardState()
	{
		StartCoroutine(FinishWithDialogue(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_REWARD_FAILED));
	}

	public void SetRewardReceivedState()
	{
		StartCoroutine(FinishWithDialogue(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_REWARD_SUCCESS));
	}

	public IEnumerator FinishWithDialogue(string dialogueStr, bool isSuccess = true, bool isDismissed = false)
	{
		StartCoroutine(_mainMenu.TweenOut());
		yield return StartCoroutine(_dialogue.TweenTextCo(dialogueStr, 2f));
		if (isSuccess)
		{
			_meeseek.SetActive(false);
			App.sound.PlaySFX("Audio/SFX/MeeseekPop");
			_pop.SetActive(true);
			yield return StartCoroutine(_animator.PlayAndWait("Animation"));
		}
		if (this.FinishedEvent != null)
		{
			this.FinishedEvent(isSuccess, isDismissed);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.WatchEvent != null)
			{
				this.WatchEvent();
			}
			break;
		case 1:
			StartCoroutine(FinishWithDialogue(MPTextDefs.MP_VIDEO_REWARD_MEESEEK_DIALOGUE_BACK, false, true));
			break;
		}
	}
}
