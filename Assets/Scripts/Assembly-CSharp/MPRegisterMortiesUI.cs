using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPRegisterMortiesUI : UIBase
{
	public delegate void UIDelegate();

	private const float _kItemMenuPaddingTop = 85f;

	private const float _kItemMenuPaddingBottom = 100f;

	private const float _kMortyOutPosX = 900f;

	private const float _kMortyTweenOutSpeed = 1600f;

	private const float _kMortyTweenInDuration = 0.25f;

	private Image _morty;

	private Text _sortButtonsTitle;

	private SortButton _sortBtnName;

	private SortButton _sortBtnType;

	private ScrollMenu _mortyMenu;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Transform _selectedMortiesContainer;

	private float _mortyStartPosX;

	private bool _isSorting;

	private Sequence _mortyTween;

	public event UIDelegate BackEvent;

	public event UIDelegate NextEvent;

	protected override void LateAwake()
	{
		_morty = base.transform.FindChild("Morty").GetComponent<Image>();
		_mortyMenu = base.transform.FindChild("ItemMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_selectedMortiesContainer = base.transform.FindChild("SelectedMorties");
		_sortButtonsTitle = base.transform.FindChild("SortOptions/Container/Title").GetComponent<Text>();
		_sortBtnName = base.transform.FindChild("SortOptions/Container/Name").GetComponent<SortButton>();
		_sortBtnType = base.transform.FindChild("SortOptions/Container/Type").GetComponent<SortButton>();
		_mortyStartPosX = _morty.transform.localPosition.x;
	}

	public void SetUp(List<string> preselectedMortyIds)
	{
		_mortyMenu.SetUp(3, true, 85f, 100f, 0f, true);
		CreateCells();
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnEnabled(0, false);
		RefreshSelectedMortyIcons();
		ShowMorty(false);
		if (preselectedMortyIds != null && preselectedMortyIds.Count > 0)
		{
			for (int i = 0; i < preselectedMortyIds.Count; i++)
			{
				CellBase cellForID = _mortyMenu.GetCellForID(preselectedMortyIds[i]);
				_mortyMenu.TrySetCellSelected(cellForID);
				if (i == preselectedMortyIds.Count - 1)
				{
					_mortyMenu.CenterToCell(cellForID);
				}
			}
		}
		SetSortButtonStates(MPSharedStateInfo.registrationInfo.sortMortiesBy, MPSharedStateInfo.registrationInfo.sortMortiesByAscending);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_REGISTER_MORTIES_BTN_NEXT);
		_dialogue.SetText(string.Format(MPTextDefs.MP_REGISTER_MORTIES_DIAL, 3));
		_sortButtonsTitle.text = MPTextDefs.MP_REGISTER_MORTIES_SORT_TITLE;
		_sortBtnName.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_NAME);
		_sortBtnType.SetText(TextDefs.MORTY_STORAGE_EDIT_DECK_SORT_BTN_TYPE);
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += CellSelectedHandler;
		_mortyMenu.UnselectedEvent += CellUnselectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= CellSelectedHandler;
		_mortyMenu.UnselectedEvent -= CellUnselectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public List<string> GetSelectedMortyIds()
	{
		return _mortyMenu.selectedCells.Select((CellBase x) => x.id).ToList();
	}

	private void CreateCells()
	{
		List<string> sortedMortyIds = GetSortedMortyIds(MPMortyDefs.GetUserRegisterMortyIDs(), MPSharedStateInfo.registrationInfo.sortMortiesBy, MPSharedStateInfo.registrationInfo.sortMortiesByAscending);
		foreach (string item in sortedMortyIds)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPRegisterMortyCell"));
			MPRegisterMortyCell component = rectTransform.GetComponent<MPRegisterMortyCell>();
			component.SetUp(item);
			_mortyMenu.AddCell(component);
		}
	}

	private void ShowMorty(bool value)
	{
		_morty.transform.localPosition = new Vector3((!value) ? 900f : _mortyStartPosX, _morty.transform.localPosition.y);
	}

	private void SetMorty(string id)
	{
		_morty.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", MPMortyDefs.AssetID(id), "front"));
		_morty.SetNativeSize();
	}

	private void SwapMorty(string id = null, float delay = 0f)
	{
		if (_mortyTween != null)
		{
			_mortyTween.Kill();
			_mortyTween = null;
		}
		float num = Mathf.Abs((_morty.rectTransform.localPosition.x - 900f) / 1600f);
		_mortyTween = DOTween.Sequence();
		_mortyTween.AppendInterval(delay);
		if (num > 0f)
		{
			_mortyTween.Append(_morty.rectTransform.DOLocalMoveX(900f, num).SetEase(Ease.Linear));
		}
		if (!string.IsNullOrEmpty(id))
		{
			_mortyTween.AppendCallback(delegate
			{
				SetMorty(id);
			});
			_mortyTween.Append(_morty.rectTransform.DOLocalMoveX(_mortyStartPosX, 0.25f).SetEase(Ease.Linear));
		}
		_mortyTween.Play();
	}

	private void RefreshSelectedMortyIcons()
	{
		for (int i = 0; i < 3; i++)
		{
			if (i >= _mortyMenu.selectedCount)
			{
				_selectedMortiesContainer.GetChild(i).gameObject.SetActive(false);
				continue;
			}
			_selectedMortiesContainer.GetChild(i).gameObject.SetActive(true);
			_selectedMortiesContainer.GetChild(i).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(_mortyMenu.selectedCells[i].id)));
			_selectedMortiesContainer.GetChild(i).Find("TypeIcon").gameObject.GetComponent<MortyElementImage>().Set(MPMortyDefs.ElementType(_mortyMenu.selectedCells[i].id));
		}
	}

	private void SetStoredMortiesSortMode(eRegisterMortiesSortyBy val, bool ascending)
	{
		if (MPSharedStateInfo.registrationInfo.sortMortiesBy == val && MPSharedStateInfo.registrationInfo.sortMortiesByAscending == ascending)
		{
			return;
		}
		MPSharedStateInfo.registrationInfo.sortMortiesBy = val;
		MPSharedStateInfo.registrationInfo.sortMortiesByAscending = ascending;
		_isSorting = true;
		List<string> list = _mortyMenu.selectedCells.Select((CellBase cell) => cell.id).ToList();
		_mortyMenu.SetAllCellsUnselected();
		RefreshMortyCells();
		foreach (string item in list)
		{
			_mortyMenu.TrySetCellSelected(_mortyMenu.GetCellForID(item));
		}
		SetSortButtonStates(MPSharedStateInfo.registrationInfo.sortMortiesBy, MPSharedStateInfo.registrationInfo.sortMortiesByAscending);
		_isSorting = false;
	}

	private void RefreshMortyCells()
	{
		List<string> mortyIds = (from MPRegisterMortyCell cell in _mortyMenu.GetCellListCopy()
			select cell.id).ToList();
		mortyIds = GetSortedMortyIds(mortyIds, MPSharedStateInfo.registrationInfo.sortMortiesBy, MPSharedStateInfo.registrationInfo.sortMortiesByAscending);
		for (int num = 0; num < mortyIds.Count; num++)
		{
			(_mortyMenu.GetCellAtIndex(num) as MPRegisterMortyCell).SetUp(mortyIds[num]);
		}
	}

	private List<string> GetSortedMortyIds(List<string> mortyIds, eRegisterMortiesSortyBy sortBy, bool ascending)
	{
		switch (sortBy)
		{
		case eRegisterMortiesSortyBy.NAME:
			mortyIds = mortyIds.OrderBy(MPMortyDefs.Name).ThenBy(MPMortyDefs.ElementType).ToList();
			break;
		case eRegisterMortiesSortyBy.TYPE:
			mortyIds = mortyIds.OrderBy(MPMortyDefs.ElementType).ThenBy(MPMortyDefs.Name).ToList();
			break;
		}
		if (!MPSharedStateInfo.registrationInfo.sortMortiesByAscending)
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

	public void CellSelectedHandler(CellBase cell, int index)
	{
		if (!_isSorting)
		{
			SwapMorty(cell.id);
			RefreshSelectedMortyIcons();
			_mainMenu.SetBtnEnabled(0, _mortyMenu.selectedCount == 3);
		}
	}

	public void CellUnselectedHandler(CellBase cell, int index)
	{
		if (!_isSorting)
		{
			SwapMorty();
			RefreshSelectedMortyIcons();
			_mainMenu.SetBtnEnabled(0, _mortyMenu.selectedCount == 3);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.NextEvent != null)
			{
				this.NextEvent();
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
		bool flag = MPSharedStateInfo.registrationInfo.sortMortiesBy != eRegisterMortiesSortyBy.NAME || !MPSharedStateInfo.registrationInfo.sortMortiesByAscending;
		SetStoredMortiesSortMode(eRegisterMortiesSortyBy.NAME, flag);
	}

	public void SortBtnTypePressedHandler()
	{
		bool flag = MPSharedStateInfo.registrationInfo.sortMortiesBy != eRegisterMortiesSortyBy.TYPE || !MPSharedStateInfo.registrationInfo.sortMortiesByAscending;
		SetStoredMortiesSortMode(eRegisterMortiesSortyBy.TYPE, flag);
	}
}
