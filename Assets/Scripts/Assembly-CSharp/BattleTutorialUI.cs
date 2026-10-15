using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleTutorialUI : BattleUI
{
	public eBattleTutorialStep currentStep;

	private BattleTutorialTipUI _tutorialTip;

	protected override IEnumerator StartTurn()
	{
		RemoveTutorialTip();
		yield return StartCoroutine(_003CStartTurn_003E__BaseCallProxy0());
	}

	protected override IEnumerator EndTurn()
	{
		yield return StartCoroutine(_003CEndTurn_003E__BaseCallProxy1());
		switch (currentStep)
		{
		case eBattleTutorialStep.T_1_1:
			Tutorial1Step2();
			break;
		case eBattleTutorialStep.T_2_1:
			Tutorial2Step2();
			break;
		case eBattleTutorialStep.T_3_1:
			Tutorial3Step2();
			break;
		}
	}

	protected override IEnumerator ShowMainMenu()
	{
		switch (currentStep)
		{
		case eBattleTutorialStep.T_1_0:
			Tutorial1Step1();
			break;
		case eBattleTutorialStep.T_2_0:
			Tutorial2Step1();
			break;
		case eBattleTutorialStep.T_3_0:
			Tutorial3Step1();
			break;
		case eBattleTutorialStep.T_3_3:
			RemoveTutorialTip();
			break;
		case eBattleTutorialStep.T_4_1:
			RemoveTutorialTip();
			currentStep = eBattleTutorialStep.T_4_0;
			break;
		}
		yield return StartCoroutine(_003CShowMainMenu_003E__BaseCallProxy2());
	}

	protected override IEnumerator ShowAttackMenu(MortyData mortyData)
	{
		RemoveTutorialTip();
		switch (currentStep)
		{
		case eBattleTutorialStep.T_3_2:
			Tutorial3Step3();
			break;
		case eBattleTutorialStep.T_4_0:
			Tutorial4Step1();
			break;
		}
		yield return StartCoroutine(_003CShowAttackMenu_003E__BaseCallProxy3(mortyData));
	}

	public void Tutorial1Step1()
	{
		currentStep = eBattleTutorialStep.T_1_1;
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, false);
		_mainMenu.SetBtnEnabled(3, false);
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_1_1, new Vector2(0f, 100f), new Vector2(-390f, -90f), 0);
	}

	public void Tutorial1Step2()
	{
		currentStep = eBattleTutorialStep.T_1_2;
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, false);
		_mainMenu.SetBtnEnabled(3, false);
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_1_2, new Vector2(0f, 100f), new Vector2(-390f, -90f), 0);
	}

	public void Tutorial2Step1()
	{
		currentStep = eBattleTutorialStep.T_2_1;
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, false);
		_mainMenu.SetBtnEnabled(3, false);
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_2_1, new Vector2(0f, 100f), new Vector2(-390f, -90f), 0);
	}

	public void Tutorial2Step2()
	{
		currentStep = eBattleTutorialStep.T_2_2;
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, true);
		_mainMenu.SetBtnEnabled(2, false);
		_mainMenu.SetBtnEnabled(3, false);
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_2_2, new Vector2(0f, 100f), new Vector2(-130f, -90f), 0);
	}

	public void Tutorial3Step1()
	{
		currentStep = eBattleTutorialStep.T_3_1;
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, true);
		_mainMenu.SetBtnEnabled(3, false);
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_3_1, new Vector2(-200f, 85f), new Vector2(120f, -100f), 0, "Type");
	}

	public void Tutorial3Step2()
	{
		currentStep = eBattleTutorialStep.T_3_2;
		_mainMenu.SetBtnEnabled(0, true);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, false);
		_mainMenu.SetBtnEnabled(3, false);
	}

	public void Tutorial3Step3()
	{
		currentStep = eBattleTutorialStep.T_3_3;
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_3_3, new Vector2(0f, 140f), new Vector2(-465f, -60f), 0);
	}

	public void Tutorial4Step1()
	{
		currentStep = eBattleTutorialStep.T_4_1;
		DisplayTutorialTip(TextDefs.BATTLE_TUTORIAL_4_1, new Vector2(0f, 140f), new Vector2(-465f, -60f), 0);
	}

	public void Tutorial4Step2()
	{
		currentStep = eBattleTutorialStep.T_4_2;
	}

	private void DisplayTutorialTip(string description, Vector2 descriptionPositon, Vector2 arrowPositon, int arrowRotation, string resourceName = "Default")
	{
		RemoveTutorialTip();
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Battle/TutorialTips/{0}", resourceName), _containerTransform);
		_tutorialTip = rectTransform.GetComponent<BattleTutorialTipUI>();
		_tutorialTip.SetUp(description, descriptionPositon, arrowPositon, arrowRotation);
	}

	private void RemoveTutorialTip()
	{
		if (_tutorialTip != null)
		{
			_tutorialTip.Remove();
			_tutorialTip = null;
		}
	}

	protected override void AttackMenuBtnLongPressStartHandler(int index)
	{
		eBattleTutorialStep eBattleTutorialStep2 = currentStep;
		if (eBattleTutorialStep2 == eBattleTutorialStep.T_4_1)
		{
			RemoveTutorialTip();
			Tutorial4Step2();
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CStartTurn_003E__BaseCallProxy0()
	{
		return base.StartTurn();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CEndTurn_003E__BaseCallProxy1()
	{
		return base.EndTurn();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CShowMainMenu_003E__BaseCallProxy2()
	{
		return base.ShowMainMenu();
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CShowAttackMenu_003E__BaseCallProxy3(MortyData mortyData)
	{
		return base.ShowAttackMenu(mortyData);
	}
}
