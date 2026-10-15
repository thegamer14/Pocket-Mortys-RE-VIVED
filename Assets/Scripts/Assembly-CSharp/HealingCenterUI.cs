using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealingCenterUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private const float _kLayer2AlphaIn = 0.9f;

	private const float _kLayer2AlphaFull = 1f;

	private const float _kLayer2AlphaOut = 0.1f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Image _layer2;

	public event BagUIBtnDelegate HealEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_layer2 = base.transform.FindChild("Background/Layer2").GetComponent<Image>();
	}

	public void SetUp(bool isRevive)
	{
		if (isRevive)
		{
			StartCoroutine(ReviveCo());
		}
		else
		{
			StartIdleAnimation();
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_HEALING_CENTER_BTN_HEAL);
		_dialogue.SetText(TextDefs.MORTY_HEALING_CENTER_DIAL);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void StartIdleAnimation()
	{
		_layer2.color = new Color(1f, 1f, 1f, 0.1f);
		_layer2.DOColor(new Color(1f, 1f, 1f, 0.9f), 2f).SetLoops(-1, LoopType.Yoyo);
	}

	private void StopIdleAnimation()
	{
		_layer2.DOKill();
	}

	private IEnumerator HealCo()
	{
		App.sound.StopSoundtrack(1f);
		StartCoroutine(_mainMenu.TweenOut());
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.MORTY_HEALING_CENTER_HEALING_1, 2f, true));
		_dialogue.TweenText(TextDefs.MORTY_HEALING_CENTER_HEALING_2);
		yield return StartCoroutine(HealAnimation());
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.MORTY_HEALING_CENTER_HEALING_3, 4f, true));
		if (this.HealEvent != null)
		{
			this.HealEvent();
		}
	}

	private IEnumerator HealAnimation()
	{
		StopIdleAnimation();
		float flashDelay = 0.5f;
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.35f).WaitForCompletion();
		App.sound.PlaySFX("Audio/SFX/HealingPreFlash");
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 1f), 0.1f).WaitForCompletion();
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.35f).WaitForCompletion();
		yield return new WaitForSeconds(flashDelay);
		App.sound.PlaySFX("Audio/SFX/HealingPreFlash");
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 1f), 0.1f).WaitForCompletion();
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.35f).WaitForCompletion();
		yield return new WaitForSeconds(flashDelay);
		App.sound.PlaySFX("Audio/SFX/HealingPreFlash");
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 1f), 0.1f).WaitForCompletion();
		yield return _layer2.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.35f).WaitForCompletion();
		yield return new WaitForSeconds(flashDelay);
		App.sound.PlaySFX("Audio/SFX/HealingComplete");
		App.states.FlashCurrentState(1f);
		StartIdleAnimation();
	}

	private IEnumerator ReviveCo()
	{
		_mainMenu.SetPositionOut();
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.MORTY_HEALING_CENTER_REVIVE, 4f));
		if (this.HealEvent != null)
		{
			this.HealEvent();
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			StartCoroutine(HealCo());
			break;
		case 1:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
