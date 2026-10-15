using System.Collections.Generic;
using UnityEngine;

public class MPChallengeUI : UIBase
{
	public delegate void BtnWithDataDelegate(MPChallengeData challengeData);

	public delegate void BtnDelegate();

	private const float _kScrollMenuPaddingTop = 85f;

	private const float _kScrollMenuPaddingBottom = 100f;

	private const float _kScrollMenuCellGap = -4f;

	private const int _KScrollMenuMinFriendCells = 3;

	private GameObject _waitingWheel;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private MPChallengeCell _selectedCell;

	public event BtnWithDataDelegate ClaimEvent;

	public event BtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_waitingWheel = base.transform.FindChild("WaitingWheel").gameObject;
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		_scrollMenu.SetUp(1, false, 85f, 100f, -4f);
		_mainMenu.SetBtnEnabled(0, false);
		_dialogue.SetText(MPTextDefs.MP_CHALLENGE_LOADING_DIALOGUE);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_CHALLENGE_CLAIM_BTN);
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += ScrollCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= ScrollCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public override void ManualUpdate(float deltaTime)
	{
		base.ManualUpdate(deltaTime);
		if (_waitingWheel.activeSelf)
		{
			_waitingWheel.transform.Rotate(new Vector3(0f, 0f, -400f) * deltaTime);
		}
	}

	public void SetScrollMenu(List<MPChallengeData> challenges)
	{
		_selectedCell = null;
		_scrollMenu.RemoveAllCells();
		if (challenges.Count > 0)
		{
			CreateChallengeCells(challenges);
		}
		_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_DEFAULT_DIALOGUE);
	}

	private void CreateChallengeCells(List<MPChallengeData> challenges)
	{
		_waitingWheel.SetActive(false);
		for (int i = 0; i < challenges.Count; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPChallengeCell"));
			MPChallengeCell component = rectTransform.GetComponent<MPChallengeCell>();
			component.SetUp(challenges[i]);
			_scrollMenu.AddCell(component);
		}
	}

	public void DisplayGetError(eGetChallengesError error)
	{
		_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_GET_ERROR);
	}

	public void DisplayClaimError(eClaimChallengeError error)
	{
		_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_CLAIM_ERROR);
	}

	private void ScrollCellSelectedHandler(CellBase cell, int index)
	{
		MPChallengeCell selectedCell = (MPChallengeCell)cell;
		_selectedCell = selectedCell;
		if (_selectedCell.data.IsCoolingDown())
		{
			_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_COOLING_DOWN_DIALOGUE);
			_mainMenu.SetBtnEnabled(0, false);
		}
		else if (_selectedCell.data.IsClaimable())
		{
			_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_CLAIMABLE_DIALOGUE);
			_mainMenu.SetBtnEnabled(0, true);
		}
		else
		{
			_dialogue.TweenText(MPTextDefs.MP_CHALLENGE_IN_PROGRESS_DIALOGUE);
			_mainMenu.SetBtnEnabled(0, false);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.ClaimEvent != null)
			{
				this.ClaimEvent(_selectedCell.data);
			}
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
