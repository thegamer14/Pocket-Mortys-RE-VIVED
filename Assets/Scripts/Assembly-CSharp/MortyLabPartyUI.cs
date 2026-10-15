using System.Collections;
using UnityEngine;

public class MortyLabPartyUI : UIBase
{
	public delegate void PartyUIMortyDelegate(MortyData mortyData);

	public delegate void PartyUIBtnDelegate();

	private const int _kMortyCellMax = 5;

	private const float _kMortyMenuPaddingTop = 0f;

	private const float _kMortyMenuPaddingBottom = 0f;

	private const float _kMortyMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _mortyMenu;

	private MortyCell _selectedCell;

	private string _selectedItemID;

	public event PartyUIMortyDelegate MortySelectedEvent;

	public event PartyUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_mortyMenu = base.transform.FindChild("MortyMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		_mortyMenu.SetUp(1, false, 0f, 0f, -4f);
		CreateMortyCells();
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.PARTY_DIAL_SET_UP);
	}

	protected override void AddEvents()
	{
		_mortyMenu.SelectedEvent += MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mortyMenu.SelectedEvent -= MortyCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateMortyCells()
	{
		for (int i = 0; i < 5; i++)
		{
			if (i > PlayerDefs.CurrentPartyCount)
			{
				AddMortyCell(null);
				continue;
			}
			MortyData mortyData = ((PlayerDefs.CurrentPartyCount <= i) ? null : PlayerDefs.CurrentPartyGetMorty(i));
			AddMortyCell(mortyData);
		}
	}

	private void AddMortyCell(MortyData mortyData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyCell"));
		MortyCell component = rectTransform.GetComponent<MortyCell>();
		component.SetUp(mortyData);
		_mortyMenu.AddCell(component);
	}

	private MortyCell GetCellForData(MortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			MortyCell mortyCell = (MortyCell)_mortyMenu.GetCellAtIndex(i);
			if (mortyCell.data == mortyData)
			{
				return mortyCell;
			}
		}
		return null;
	}

	private void ShowMenu(MenuBase menu)
	{
		_mainMenu.SetPositionOut();
		if (menu != null)
		{
			menu.SetPositionIn();
		}
	}

	private IEnumerator ShowMenuWithTween(MenuBase menu)
	{
		_mortyMenu.SetInteractable(false);
		StartCoroutine(_mainMenu.TweenOut());
		if (menu != null)
		{
			yield return StartCoroutine(menu.TweenIn());
			_mortyMenu.SetInteractable(true);
		}
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = (MortyCell)cell;
		if (this.MortySelectedEvent != null)
		{
			this.MortySelectedEvent(_selectedCell.data);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (this.BackEvent != null)
		{
			this.BackEvent();
		}
	}
}
