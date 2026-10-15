using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPHealingCenterUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private const float _kLayer2AlphaIn = 0.9f;

	private const float _kLayer2AlphaFull = 1f;

	private const float _kLayer2AlphaOut = 0.1f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Image _layer2;

	private MPPlayerLevelBarUI _levelBar;

	public event BagUIBtnDelegate TryHealEvent;

	public event BagUIBtnDelegate HealAnimationCompleteEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_layer2 = base.transform.FindChild("Background/Layer2").GetComponent<Image>();
		_levelBar = base.transform.FindChild("LevelBar").GetComponent<MPPlayerLevelBarUI>();
	}

	public void SetUp(bool isRevive)
	{
		_levelBar.SetUp();
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

	public void TryHeal()
	{
		_dialogue.TweenText(MPTextDefs.MP_HEALING_CENTER_WAITING);
		StartCoroutine(_mainMenu.TweenOut());
		if (this.TryHealEvent != null)
		{
			this.TryHealEvent();
		}
	}

	public IEnumerator HealCo()
	{
		App.sound.StopSoundtrack(1f);
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.MORTY_HEALING_CENTER_HEALING_1, 2f, true));
		_dialogue.TweenText(TextDefs.MORTY_HEALING_CENTER_HEALING_2);
		yield return StartCoroutine(HealAnimation());
		yield return StartCoroutine(_dialogue.TweenTextCo(TextDefs.MORTY_HEALING_CENTER_HEALING_3, 4f, true));
		if (_levelBar.xpMultiplier > 0)
		{
			StartCoroutine(_levelBar.TweenMultiplerToZero());
			yield return StartCoroutine(_dialogue.TweenTextCo(MPTextDefs.MP_HEALING_CENTER_HEALING_MULTIPLIER, 4f, true));
		}
		if (this.HealAnimationCompleteEvent != null)
		{
			this.HealAnimationCompleteEvent();
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
		_dialogue.SetText(TextDefs.MORTY_HEALING_CENTER_REVIVE);
		yield return StartCoroutine(_dialogue.SetTextCo(TextDefs.MORTY_HEALING_CENTER_REVIVE, 4f));
		if (this.HealAnimationCompleteEvent != null)
		{
			this.HealAnimationCompleteEvent();
		}
	}

	public void HealFailed(eHealActiveMortyDeckError error)
	{
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_HEALING_CENTER_ERROR);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			TryHeal();
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
