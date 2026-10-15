using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StorageEditBootcampUI : UIBase
{
	public delegate void BootcampUIMortyDelgate(MortyData mortyData);

	public delegate void BootcampUIBtnDelegate();

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

	private int _bootcampIndex;

	private ScrollMenu _mortyMenu;

	private MortyCell _selectedCell;

	public event BootcampUIMortyDelgate SelectedEvent;

	public event BootcampUIMortyDelgate StatsEvent;

	public event BootcampUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_sortButtonsTitle = base.transform.FindChild("SortOptions/Container/Title").GetComponent<Text>();
		_sortBtnName = base.transform.FindChild("SortOptions/Container/Name").GetComponent<SortButton>();
		_sortBtnType = base.transform.FindChild("SortOptions/Container/Type").GetComponent<SortButton>();
		_sortBtnLevel = base.transform.FindChild("SortOptions/Container/Level").GetComponent<SortButton>();
	}

	public void SetUp(int bootcampIndex, MortyData startingMortyData)
	{
		_bootcampIndex = bootcampIndex;
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
		SetSortButtonStates(SharedStatInfo.storageBootcampInfo.sortBy, SharedStatInfo.storageBootcampInfo.sortByAscending);
		_dialogue.SetText(string.Format(TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL, BootcampDefs.GetBootcampLevel(_bootcampIndex)));
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_WITHDRAW);
		_mainMenu.SetBtnTitle(1, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_DEPOSIT);
		_mainMenu.SetBtnTitle(2, TextDefs.MORTY_STORAGE_EDIT_DECK_BTN_STATS);
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
		List<MortyData> mortiesForBootcamp = BootcampDefs.GetMortiesForBootcamp(_bootcampIndex);
		List<MortyData> sortedMortys = GetSortedMortys(BootcampDefs.GetOwnedMortiesNotInBootcamp(_bootcampIndex), SharedStatInfo.storageBootcampInfo.sortBy, SharedStatInfo.storageBootcampInfo.sortByAscending);
		_mortyMenu.AddHeaderCell(string.Format(TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_HEADER, BootcampDefs.GetBootcampLevel(_bootcampIndex)));
		for (int i = 0; i < 5; i++)
		{
			AddNewMortyCell((i >= mortiesForBootcamp.Count) ? null : mortiesForBootcamp[i]);
		}
		_mortyMenu.AddHeaderCell(TextDefs.MORTY_STORAGE_EDIT_DECK_HEADER_STORED);
		for (int j = 0; j < sortedMortys.Count; j++)
		{
			AddNewMortyCell(sortedMortys[j]);
		}
	}

	private void RefreshMortyCells()
	{
		List<MortyData> mortiesForBootcamp = BootcampDefs.GetMortiesForBootcamp(_bootcampIndex);
		int num = 0;
		for (int i = 1; i <= 5; i++)
		{
			MortyData mortyData = ((num >= mortiesForBootcamp.Count) ? null : mortiesForBootcamp[num]);
			bool isOut = mortyData != null && PlayerDefs.IsMoryInParty(mortyData);
			(_mortyMenu.GetCellAtIndex(i) as MortyCell).SetUp(mortyData, isOut);
			num++;
		}
		List<MortyData> ownedMortiesNotInBootcamp = BootcampDefs.GetOwnedMortiesNotInBootcamp(_bootcampIndex);
		ownedMortiesNotInBootcamp = GetSortedMortys(ownedMortiesNotInBootcamp, SharedStatInfo.storageBootcampInfo.sortBy, SharedStatInfo.storageBootcampInfo.sortByAscending);
		for (int j = 0; j < ownedMortiesNotInBootcamp.Count; j++)
		{
			int num2 = j + 5 + 2;
			if (num2 < _mortyMenu.GetCellCount())
			{
				MortyCell mortyCell = _mortyMenu.GetCellAtIndex(num2) as MortyCell;
				mortyCell.SetUp(ownedMortiesNotInBootcamp[j], PlayerDefs.IsMoryInParty(ownedMortiesNotInBootcamp[j]));
				if (ownedMortiesNotInBootcamp[j].level < BootcampDefs.GetBootcampLevel(_bootcampIndex))
				{
					mortyCell.SetEnabled(false);
				}
			}
			else
			{
				AddNewMortyCell(ownedMortiesNotInBootcamp[j]);
			}
		}
		while (_mortyMenu.GetCellCount() > ownedMortiesNotInBootcamp.Count + 5 + 2)
		{
			_mortyMenu.RemoveCell(_mortyMenu.GetLastCell());
		}
	}

	private MortyCell AddNewMortyCell(MortyData mortyData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyCell"));
		MortyCell component = rectTransform.GetComponent<MortyCell>();
		component.SetUp(mortyData, PlayerDefs.IsMoryInParty(mortyData));
		if (mortyData != null && mortyData.level < BootcampDefs.GetBootcampLevel(_bootcampIndex))
		{
			component.SetEnabled(false);
		}
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

	private void AddMortyToBootcamp(MortyData mortyData)
	{
		if (BootcampDefs.GetMortiesForBootcamp(_bootcampIndex).Count() < 5)
		{
			BootcampDefs.AddMortyToBootcamp(mortyData, _bootcampIndex);
			_mortyMenu.SetCellUnselected(_selectedCell);
			RefreshMortyCells();
			_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL_WITHDRAW, MortyDefs.Name(mortyData.id)));
		}
		else
		{
			_dialogue.TweenText(TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL_WITHDRAW_FAIL);
		}
	}

	private void RemoveMortyFromBootcamp(MortyData mortyData)
	{
		BootcampDefs.RemoveMortyFromBootcamp(mortyData, _bootcampIndex);
		_mortyMenu.SetCellUnselected(_selectedCell);
		RefreshMortyCells();
		_dialogue.TweenText(string.Format(TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL_DEPOSIT, MortyDefs.Name(mortyData.id)));
	}

	private void SetStoredMortiesSortMode(eStorageBootcampSortyBy val, bool ascending)
	{
		if (SharedStatInfo.storageBootcampInfo.sortBy != val || SharedStatInfo.storageBootcampInfo.sortByAscending != ascending)
		{
			_mortyMenu.SetCellUnselected(_selectedCell);
			SharedStatInfo.storageBootcampInfo.sortBy = val;
			SharedStatInfo.storageBootcampInfo.sortByAscending = ascending;
			SetSortButtonStates(SharedStatInfo.storageBootcampInfo.sortBy, SharedStatInfo.storageBootcampInfo.sortByAscending);
			RefreshMortyCells();
		}
	}

	private List<MortyData> GetSortedMortys(List<MortyData> mortyList, eStorageBootcampSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eStorageBootcampSortyBy.NAME:
			mortyList = mortyList.OrderBy((MortyData o) => MortyDefs.Name(o.id)).ToList();
			break;
		case eStorageBootcampSortyBy.LEVEL:
			mortyList = mortyList.OrderBy((MortyData o) => o.level).ToList();
			break;
		case eStorageBootcampSortyBy.TYPE:
			mortyList = mortyList.OrderBy((MortyData o) => MortyDefs.ElementType(o.id)).ToList();
			break;
		}
		if (!SharedStatInfo.storageBootcampInfo.sortByAscending)
		{
			mortyList.Reverse();
		}
		return mortyList;
	}

	private void SetSortButtonStates(eStorageBootcampSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eStorageBootcampSortyBy.NAME:
			_sortBtnName.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			_sortBtnLevel.SetArrowState(eSortButtonState.NONE);
			_sortBtnType.SetArrowState(eSortButtonState.NONE);
			break;
		case eStorageBootcampSortyBy.LEVEL:
			_sortBtnName.SetArrowState(eSortButtonState.NONE);
			_sortBtnLevel.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			_sortBtnType.SetArrowState(eSortButtonState.NONE);
			break;
		case eStorageBootcampSortyBy.TYPE:
			_sortBtnName.SetArrowState(eSortButtonState.NONE);
			_sortBtnLevel.SetArrowState(eSortButtonState.NONE);
			_sortBtnType.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			break;
		}
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (MortyCell)cell;
		bool flag = BootcampDefs.GetMortiesForBootcamp(_bootcampIndex).Contains(_selectedCell.data);
		_mainMenu.SetBtnEnabled(0, !flag);
		_mainMenu.SetBtnEnabled(1, flag);
		_mainMenu.SetBtnEnabled(2, true);
		string format = ((!flag) ? TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL_STORED_SELECTED : TextDefs.MORTY_STORAGE_EDIT_BOOTCAMP_DIAL_PARTY_SELECTED);
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
			AddMortyToBootcamp(_selectedCell.data);
			break;
		case 1:
			RemoveMortyFromBootcamp(_selectedCell.data);
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
		bool flag = SharedStatInfo.storageBootcampInfo.sortBy != eStorageBootcampSortyBy.NAME || !SharedStatInfo.storageBootcampInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageBootcampSortyBy.NAME, flag);
	}

	public void SortBtnTypePressedHandler()
	{
		bool flag = SharedStatInfo.storageBootcampInfo.sortBy != eStorageBootcampSortyBy.TYPE || !SharedStatInfo.storageBootcampInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageBootcampSortyBy.TYPE, flag);
	}

	public void SortBtnLevelPressedHandler()
	{
		bool flag = SharedStatInfo.storageBootcampInfo.sortBy != eStorageBootcampSortyBy.LEVEL || !SharedStatInfo.storageBootcampInfo.sortByAscending;
		SetStoredMortiesSortMode(eStorageBootcampSortyBy.LEVEL, flag);
	}

	public void TopBtnPressedHandler()
	{
		_mortyMenu.SnapToTop();
	}
}
