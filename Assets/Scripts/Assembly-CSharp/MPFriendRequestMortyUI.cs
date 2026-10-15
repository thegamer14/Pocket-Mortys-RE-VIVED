using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MPFriendRequestMortyUI : UIBase
{
	public delegate void UIRequestDelegate(string mortyId);

	public delegate void UIDelegate();

	private const float _kScrollMenuPaddingTop = 85f;

	private const float _kScrollMenuPaddingBottom = 100f;

	private const int _KScrollMenuMinCells = 5;

	private Text _sortButtonsTitle;

	private SortButton _sortBtnName;

	private SortButton _sortBtnType;

	private ScrollMenu _scrollMenu;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private MPRegisterMortyCell _selectedCell;

	private bool _isSorting;

	private eRegisterMortiesSortyBy _sortScrollMenuBy;

	private bool _sortScrollMenuAscending;

	public event UIRequestDelegate RequestEvent;

	public event UIDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_sortButtonsTitle = base.transform.FindChild("SortOptions/Container/Title").GetComponent<Text>();
		_sortBtnName = base.transform.FindChild("SortOptions/Container/Name").GetComponent<SortButton>();
		_sortBtnType = base.transform.FindChild("SortOptions/Container/Type").GetComponent<SortButton>();
	}

	public void SetUp(List<string> mortyIds)
	{
		_scrollMenu.SetUp(1, false, 85f, 100f);
		CreateCells(mortyIds);
		_mainMenu.SetBtnEnabled(0, false);
		SetStoredMortiesSortMode(eRegisterMortiesSortyBy.NAME, true);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_REQUEST_MORTY_BTN);
		_dialogue.SetText(MPTextDefs.MP_FRIEND_REQUEST_MORTY_DIALOGUE);
		_sortButtonsTitle.text = MPTextDefs.MP_REGISTER_MORTIES_SORT_TITLE;
		_sortBtnName.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_NAME);
		_sortBtnType.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_TYPE);
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += CellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= CellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateCells(List<string> mortyIds)
	{
		for (int i = 0; i < mortyIds.Count; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPRegisterMortyCell"));
			MPRegisterMortyCell component = rectTransform.GetComponent<MPRegisterMortyCell>();
			component.SetUp(mortyIds[i]);
			_scrollMenu.AddCell(component);
		}
	}

	private void SetStoredMortiesSortMode(eRegisterMortiesSortyBy val, bool ascending)
	{
		if (_sortScrollMenuBy != val || _sortScrollMenuAscending != ascending)
		{
			_sortScrollMenuBy = val;
			_sortScrollMenuAscending = ascending;
			_isSorting = true;
			string text = ((!(_selectedCell != null)) ? null : _selectedCell.id);
			_scrollMenu.SetAllCellsUnselected();
			RefreshMortyCells();
			if (!string.IsNullOrEmpty(text))
			{
				_selectedCell = (MPRegisterMortyCell)_scrollMenu.GetCellForID(text);
				_scrollMenu.TrySetCellSelected(_selectedCell);
			}
			SetSortButtonStates(_sortScrollMenuBy, _sortScrollMenuAscending);
			_isSorting = false;
		}
	}

	private void RefreshMortyCells()
	{
		List<string> mortyIds = (from MPRegisterMortyCell cell in _scrollMenu.GetCellListCopy()
			select cell.id).ToList();
		mortyIds = GetSortedMortyIds(mortyIds, _sortScrollMenuBy, _sortScrollMenuAscending);
		for (int num = 0; num < mortyIds.Count; num++)
		{
			(_scrollMenu.GetCellAtIndex(num) as MPRegisterMortyCell).SetUp(mortyIds[num]);
		}
	}

	private List<string> GetSortedMortyIds(List<string> mortyIds, eRegisterMortiesSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eRegisterMortiesSortyBy.NAME:
			mortyIds = mortyIds.OrderBy((string id) => string.IsNullOrEmpty(id) ? null : MPMortyDefs.Name(id)).ToList();
			break;
		case eRegisterMortiesSortyBy.TYPE:
			mortyIds = mortyIds.OrderBy((string id) => string.IsNullOrEmpty(id) ? null : MPMortyDefs.ElementType(id)).ToList();
			break;
		}
		if (!_sortScrollMenuAscending)
		{
			mortyIds.Reverse();
		}
		return mortyIds;
	}

	private void SetSortButtonStates(eRegisterMortiesSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eRegisterMortiesSortyBy.NAME:
			_sortBtnName.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			_sortBtnType.SetArrowState(eSortButtonState.NONE);
			break;
		case eRegisterMortiesSortyBy.TYPE:
			_sortBtnName.SetArrowState(eSortButtonState.NONE);
			_sortBtnType.SetArrowState((!ascending) ? eSortButtonState.DESCENDING : eSortButtonState.ASCENDING);
			break;
		}
	}

	public void DisplayRequestMortyError(eFriendRequestMortyError error)
	{
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_FAILED_DIALOGUE, MPMortyDefs.Name(_selectedCell.id)));
	}

	public void CellSelectedHandler(CellBase cell, int index)
	{
		if (!_isSorting)
		{
			_selectedCell = (MPRegisterMortyCell)cell;
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_REQUEST_MORTY_CELL_SELECTED_DIALOGUE, MPMortyDefs.Name(_selectedCell.id)));
			_mainMenu.SetBtnEnabled(0, true);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.RequestEvent != null)
			{
				this.RequestEvent(_selectedCell.id);
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

	public void SortBtnNamePressedHandler()
	{
		bool flag = _sortScrollMenuBy != eRegisterMortiesSortyBy.NAME || !_sortScrollMenuAscending;
		SetStoredMortiesSortMode(eRegisterMortiesSortyBy.NAME, flag);
	}

	public void SortBtnTypePressedHandler()
	{
		bool flag = _sortScrollMenuBy != eRegisterMortiesSortyBy.TYPE || !_sortScrollMenuAscending;
		SetStoredMortiesSortMode(eRegisterMortiesSortyBy.TYPE, flag);
	}
}
