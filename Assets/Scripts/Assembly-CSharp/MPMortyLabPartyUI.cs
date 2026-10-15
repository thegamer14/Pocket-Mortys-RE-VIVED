using System.Collections;
using UnityEngine;

public class MPMortyLabPartyUI : UIBase
{
	public delegate void PartyUIMortyDelegate(MPOpponentMortyData mortyData);

	public delegate void PartyUIBtnDelegate();

	private const int _kMortyCellMax = 5;

	private const float _kMortyMenuPaddingTop = 0f;

	private const float _kMortyMenuPaddingBottom = 0f;

	private const float _kMortyMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _mortyMenu;

	private MPMortyCell _selectedCell;

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
		int activeDeckCount = App.net.player.data.GetActiveDeckCount();
		for (int i = 0; i < 5; i++)
		{
			if (i > activeDeckCount)
			{
				AddMortyCell(null);
				continue;
			}
			MPOpponentMortyData mortyData = ((activeDeckCount <= i) ? null : App.net.player.data.GetPartyMortyData(i));
			AddMortyCell(mortyData);
		}
	}

	private void AddMortyCell(MPOpponentMortyData mortyData)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
		MPMortyCell component = rectTransform.GetComponent<MPMortyCell>();
		component.SetUp(mortyData);
		_mortyMenu.AddCell(component);
	}

	private MPMortyCell GetCellForData(MPOpponentMortyData mortyData)
	{
		if (mortyData == null)
		{
			return null;
		}
		for (int i = 0; i < _mortyMenu.count; i++)
		{
			MPMortyCell mPMortyCell = (MPMortyCell)_mortyMenu.GetCellAtIndex(i);
			if (mPMortyCell.data == mortyData)
			{
				return mPMortyCell;
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
		_selectedCell = (MPMortyCell)cell;
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
