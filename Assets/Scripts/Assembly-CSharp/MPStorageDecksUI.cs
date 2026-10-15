public class MPStorageDecksUI : UIBase
{
	public delegate void StorageDecksBtnDelegate(int selectedDeckIndex = 0);

	private const float _kDeckMenuPaddingTop = 0f;

	private const float _kDeckMenuPaddingBottom = 0f;

	private const float _kDeckMenuCellGap = 7f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _decksMenu;

	private MPDeckCell _selectedCell;

	public event StorageDecksBtnDelegate EditEvent;

	public event StorageDecksBtnDelegate WithdrawEvent;

	public event StorageDecksBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_decksMenu = base.transform.FindChild("DecksMenu").GetComponent<ScrollMenu>();
	}

	public void SetUp()
	{
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		_decksMenu.SetUp(1, false, 0f, 0f, 7f);
		AddDeckCells();
	}

	private void AddDeckCells()
	{
		for (int i = 0; i < 3; i++)
		{
			MPDeckCell component = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPDeckCell")).GetComponent<MPDeckCell>();
			component.SetUp(i);
			_decksMenu.AddCell(component);
			if (component.deckIndex == MPSharedStateInfo.storageDecksInfo.editDeckIndex)
			{
				_decksMenu.TrySetCellSelected(component);
			}
		}
	}

	public void UpdateAfterDeckWithdrawn()
	{
		for (int i = 0; i < _decksMenu.GetCellCount(); i++)
		{
			(_decksMenu.GetCellAtIndex(i) as MPDeckCell).SetActiveTab();
		}
		if (_selectedCell != null)
		{
			_mainMenu.SetBtnEnabled(0, _selectedCell.deckIndex != App.net.player.data.activeDeckIndex);
		}
		ActivateInput(true);
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_DECKS_DIAL_WITHDRAWN);
	}

	public void DisplayWaitingToWithdraw()
	{
		_dialogue.TweenText(MPTextDefs.MP_MORTY_STORAGE_DECKS_WITHDRAW_WAIT_DIAL);
		ActivateInput(false);
		StartCoroutine(_mainMenu.TweenOut());
	}

	public void DisplayWithdrawError()
	{
		_dialogue.TweenText(MPTextDefs.MP_MORTY_STORAGE_DECKS_WITHDRAW_ERROR_DIAL);
		ActivateInput(true);
		StartCoroutine(_mainMenu.TweenIn());
	}

	public void DisplayCannotWithdrawEmptyDeck()
	{
		_dialogue.TweenText(MPTextDefs.MP_MORTY_STORAGE_DECKS_WITHDRAW_ERROR_EMPTY_DIAL);
	}

	public void DisplayCannotWithdrawFaintedDeck()
	{
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_DECKS_DIAL_WITHDRAW_FAIL_FAINTED);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_DECKS_BTN_WITHDRAW);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STORAGE_DECKS_BTN_EDIT);
		_dialogue.SetText(TextDefs.MORTY_STORAGE_DECKS_DIAL);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_decksMenu.SelectedEvent += DeckCellSelectedHandler;
		_decksMenu.UnselectedEvent += DeckCellUnselectedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_decksMenu.SelectedEvent -= DeckCellSelectedHandler;
		_decksMenu.UnselectedEvent -= DeckCellUnselectedHandler;
	}

	public void DeckCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (MPDeckCell)cell;
		_mainMenu.SetBtnEnabled(0, _selectedCell.deckIndex != App.net.player.data.activeDeckIndex);
		_mainMenu.SetBtnEnabled(1, true);
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_DECKS_DIAL_SELECTED);
	}

	public void DeckCellUnselectedHandler(CellBase cell, int index)
	{
		_selectedCell = null;
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		_dialogue.TweenText(TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.WithdrawEvent != null)
			{
				this.WithdrawEvent(_selectedCell.deckIndex);
			}
			break;
		case 1:
			if (this.EditEvent != null)
			{
				this.EditEvent(_selectedCell.deckIndex);
			}
			break;
		case 2:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
