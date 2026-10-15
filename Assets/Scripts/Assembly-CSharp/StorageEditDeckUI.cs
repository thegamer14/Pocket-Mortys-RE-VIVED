using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorageEditDeckUI : UIBase
{
	public delegate void PartyUIMortyDelegate(MortyData mortyData);

	public delegate void PartyUIBtnDelegate();

	private const int _kMortyMenuMinimumCellCount = 5;

	private const float _kMortyMenuPaddingTop = 85f;

	private const float _kMortyMenuPaddingBottom = 100f;

	private const float _kMortyMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Text _sortButtonsTitle;

	private SortButton _sortBtnName;

	private SortButton _sortBtnType;

	private SortButton _sortBtnLevel;

	private int _deckIndex;

	private Text _slotsTitle;

	private Text _slotsCount;

	private ScrollMenu _mortyMenu;

	private MortyCell _selectedCell;

	public event PartyUIMortyDelegate SelectedEvent;

	public event PartyUIMortyDelegate StatsEvent;

	public event PartyUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_slotsTitle = base.transform.FindChild("SlotsCount/Container/Title").GetComponent<Text>();
		_slotsCount = base.transform.FindChild("SlotsCount/Container/Background/Label").GetComponent<Text>();
		_sortButtonsTitle = base.transform.FindChild("SortOptions/Container/Title").GetComponent<Text>();
		_sortBtnName = base.transform.FindChild("SortOptions/Container/Name").GetComponent<SortButton>();
		_sortBtnType = base.transform.FindChild("SortOptions/Container/Type").GetComponent<SortButton>();
		_sortBtnLevel = base.transform.FindChild("SortOptions/Container/Level").GetComponent<SortButton>();
	}

	public void SetUp(int deckIndex, MortyData startingMortyData)
	{
		_deckIndex = deckIndex;
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		_mainMenu.SetBtnEnabled(2, false);
		_mortyMenu.SetUp(1, false, 85f, 100f, -4f);
		CreateMortyCells();
		MortyCell cellForData = GetCellForData(startingMortyData);
		if (cellForData != null)
		{
			_mortyMenu.TryCenterAndSelectCellWithID(cellForData.id);
		}
		SetSortButtonStates(SharedStatInfo.storageDecksInfo.sortBy, SharedStatInfo.storageDecksInfo.sortByAscending);
		SetSlotCount();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_WITHDRAW);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_DEPOSIT);
		_mainMenu.SetBtnTitle(2, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_STATS);
		_dialogue.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL);
		_slotsTitle.text = TextDefs.MORTY_STORAGE_EDIT_DECK_COUNTER_TITLE;
		_sortButtonsTitle.text = TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_TITLE;
		_sortBtnName.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_NAME);
		_sortBtnType.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_TYPE);
		_sortBtnLevel.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_LEVEL);
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += MortyCellSelectedHandler;
		_mortyMenu.UnselectedEvent += MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= MortyCellSelectedHandler;
		_mortyMenu.UnselectedEvent -= MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateMortyCells()
	{
		List<MortyData> allPlayerMorties = PlayerDefs.GetAllPlayerMorties();
		List<MortyData> mortiesForDeck = PlayerDefs.GetMortiesForDeck(_deckIndex);
		List<MortyData> mortyList = allPlayerMorties.Except(mortiesForDeck).ToList();
		mortyList = GetSortedMortys(mortyList, SharedStatInfo.storageDecksInfo.sortBy, SharedStatInfo.storageDecksInfo.sortByAscending);
		_mortyMenu.AddHeaderCell(string.Format(TextDefs.MORTY_STORAGE_EDIT_DECK_HEADER_DECK, ExtensionMethods.IntToLetters(_deckIndex)));
		for (int i = 0; i < 5; i++)
		{
			AddNewMortyCell((i >= mortiesForDeck.Count) ? null : mortiesForDeck[i]);
		}
		_mortyMenu.AddHeaderCell(TextDefs.MORTY_STORAGE_EDIT_DECK_HEADER_STORED);
		for (int j = 0; j < mortyList.Count; j++)
		{
			AddNewMortyCell(mortyList[j]);
		}
	}

	private void RefreshMortyCells()
	{
		List<MortyData> mortiesForDeck = PlayerDefs.GetMortiesForDeck(_deckIndex);
		int num = 0;
		for (int i = 1; i <= 5; i++)
		{
			MortyData mortyData = ((num >= mortiesForDeck.Count) ? null : mortiesForDeck[num]);
			bool isOut = mortyData != null && PlayerDefs.IsMoryInParty(mortyData);
			(_mortyMenu.GetCellAtIndex(i) as MortyCell).SetUp(mortyData, isOut);
			num++;
		}
		List<MortyData> allPlayerMorties = PlayerDefs.GetAllPlayerMorties();
		List<MortyData> mortyList = allPlayerMorties.Except(mortiesForDeck).ToList();
		mortyList = GetSortedMortys(mortyList, SharedStatInfo.storageDecksInfo.sortBy, SharedStatInfo.storageDecksInfo.sortByAscending);
		for (int j = 0; j < mortyList.Count; j++)
		{
			int num2 = j + 5 + 2;
			if (num2 < _mortyMenu.GetCellCount())
			{
				(_mortyMenu.GetCellAtIndex(num2) as MortyCell).SetUp(mortyList[j], PlayerDefs.IsMoryInParty(mortyList[j]));
			}
			else
			{
				AddNewMortyCell(mortyList[j]);
			}
		}
		while (_mortyMenu.GetCellCount() > mortyList.Count + 5 + 2)
		{
			_mortyMenu.RemoveCell(_mortyMenu.GetLastCell());
		}
	}

	private MortyCell AddNewMortyCell(MortyData mortyData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyCell"));
		MortyCell component = rectTransform.GetComponent<MortyCell>();
		component.SetUp(mortyData, PlayerDefs.IsMoryInParty(mortyData));
		_mortyMenu.AddCell(component);
		return component;
	}

	private MortyCell GetCellForData(MortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			CellBase cellAtIndex = _mortyMenu.GetCellAtIndex(i);
			if (cellAtIndex is MortyCell)
			{
				MortyCell mortyCell = (MortyCell)cellAtIndex;
				if (mortyCell.data == mortyData)
				{
					return mortyCell;
				}
			}
		}
		return null;
	}

	private void SetSlotCount()
	{
		_slotsCount.text = PlayerDefs.PlayerStorageCount + "/" + 300;
	}

	private void AddMortyToDeck(MortyData mortyData)
	{
		string failMessage = null;
		if (PlayerDefs.AddMortyToDeck(mortyData, _deckIndex, out failMessage))
		{
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.DAYCARE, "withdraw_morty");
			_mortyMenu.SetCellUnselected(_selectedCell);
			RefreshMortyCells();
			SetSlotCount();
			_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_WITHDRAW, MortyDefs.Name(mortyData.id)));
		}
		else
		{
			_dialogue.TweenText(failMessage);
		}
	}

	private void RemoveMortyFromDeck(MortyData mortyData)
	{
		string failMessage = null;
		if (PlayerDefs.RemoveMortyFromDeck(mortyData, _deckIndex, out failMessage))
		{
			App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.DAYCARE, "deposited_morty");
			_mortyMenu.SetCellUnselected(_selectedCell);
			RefreshMortyCells();
			SetSlotCount();
			_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_DEPOSIT, MortyDefs.Name(mortyData.id)));
		}
		else
		{
			_dialogue.TweenText(failMessage);
		}
	}

	private void SetStoredMortiesSortMode(eStorageDeckSortyBy val, bool ascending)
	{
		if (SharedStatInfo.storageDecksInfo.sortBy != val || SharedStatInfo.storageDecksInfo.sortByAscending != ascending)
		{
			_mortyMenu.SetCellUnselected(_selectedCell);
			SharedStatInfo.storageDecksInfo.sortBy = val;
			SharedStatInfo.storageDecksInfo.sortByAscending = ascending;
			SetSortButtonStates(SharedStatInfo.storageDecksInfo.sortBy, SharedStatInfo.storageDecksInfo.sortByAscending);
			RefreshMortyCells();
		}
	}

	private List<MortyData> GetSortedMortys(List<MortyData> mortyList, eStorageDeckSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eStorageDeckSortyBy.NAME:
			mortyList = mortyList.OrderBy((MortyData o) => MortyDefs.Name(o.id)).ToList();
			break;
		case eStorageDeckSortyBy.LEVEL:
			mortyList = mortyList.OrderBy((MortyData o) => o.level).ToList();
			break;
		case eStorageDeckSortyBy.TYPE:
			mortyList = mortyList.OrderBy((MortyData o) => MortyDefs.ElementType(o.id)).ToList();
			break;
		}
		if (!SharedStatInfo.storageDecksInfo.sortByAscending)
		{
			mortyList.Reverse();
		}
		return mortyList;
	}

	private void SetSortButtonStates(eStorageDeckSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eStorageDeckSortyBy.NAME:
			_sortBtnName.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			_sortBtnLevel.SetArrowState(eSortButtonState.NONE);
			_sortBtnType.SetArrowState(eSortButtonState.NONE);
			break;
		case eStorageDeckSortyBy.LEVEL:
			_sortBtnName.SetArrowState(eSortButtonState.NONE);
			_sortBtnLevel.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			_sortBtnType.SetArrowState(eSortButtonState.NONE);
			break;
		case eStorageDeckSortyBy.TYPE:
			_sortBtnName.SetArrowState(eSortButtonState.NONE);
			_sortBtnLevel.SetArrowState(eSortButtonState.NONE);
			_sortBtnType.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			break;
		}
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (MortyCell)cell;
		bool flag = PlayerDefs.GetMortiesForDeck(_deckIndex).Contains(_selectedCell.data);
		_mainMenu.SetBtnEnabled(0, !flag);
		_mainMenu.SetBtnEnabled(1, flag);
		_mainMenu.SetBtnEnabled(2, true);
		string format = ((!flag) ? TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_STORED_SELECTED : TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_PARTY_SELECTED);
		format = string.Format(format, MortyDefs.Name(_selectedCell.data.id));
		_dialogue.TweenText(format);
		if (this.SelectedEvent != null)
		{
			this.SelectedEvent(_selectedCell.data);
		}
	}

	public void MortyCellUnselectedHandler(CellBase cell, int index)
	{
		_selectedCell = null;
		_mainMenu.SetBtnEnabled(0, false);
		_mainMenu.SetBtnEnabled(1, false);
		if (this.SelectedEvent != null)
		{
			this.SelectedEvent(null);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			AddMortyToDeck(_selectedCell.data);
			break;
		case 1:
			RemoveMortyFromDeck(_selectedCell.data);
			break;
		case 2:
			if (this.StatsEvent != null)
			{
				this.StatsEvent(_selectedCell.data);
			}
			break;
		case 3:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}

	public void SortBtnNamePressedHandler()
	{
		bool flag = SharedStatInfo.storageDecksInfo.sortBy != eStorageDeckSortyBy.NAME || !SharedStatInfo.storageDecksInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageDeckSortyBy.NAME, flag);
	}

	public void SortBtnTypePressedHandler()
	{
		bool flag = SharedStatInfo.storageDecksInfo.sortBy != eStorageDeckSortyBy.TYPE || !SharedStatInfo.storageDecksInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageDeckSortyBy.TYPE, flag);
	}

	public void SortBtnLevelPressedHandler()
	{
		bool flag = SharedStatInfo.storageDecksInfo.sortBy != eStorageDeckSortyBy.LEVEL || !SharedStatInfo.storageDecksInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageDeckSortyBy.LEVEL, flag);
	}

	public void TopBtnPressedHandler()
	{
		_mortyMenu.SnapToTop();
	}
}
