using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MPFriendDonateMortyUI : UIBase
{
	public delegate void DonateDelegate(MPOpponentMortyData mortyData);

	public delegate void BtnDelegate();

	private const float _kScrollMenuPaddingTop = 85f;

	private const float _kScrollMenuPaddingBottom = 100f;

	private const float _kScrollMenuCellGap = -4f;

	private const int _KScrollMenuMinFriendCells = 3;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private MPOpponentMortyData _selectedCellData;

	private MPFriendData _friendData;

	public event DonateDelegate DonateEvent;

	public event BtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(MPFriendData friendData, List<MPOpponentMortyData> mortyDatas)
	{
		_friendData = friendData;
		_scrollMenu.SetUp(1, false, 85f, 100f, -4f);
		SetScrollMenu(mortyDatas);
		_mainMenu.SetBtnEnabled(0, false);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, MPTextDefs.MP_FRIEND_DONATE_MORTY_BTN);
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

	public void SetScrollMenu(List<MPOpponentMortyData> availableMortyDatas)
	{
		ClearScrollMenu();
		CreateMortyCells(availableMortyDatas);
		CellBase cellBase = null;
		if (_selectedCellData != null)
		{
			cellBase = _scrollMenu.GetCellForID(_selectedCellData.netId);
		}
		if (cellBase != null)
		{
			_scrollMenu.TrySetCellSelected(cellBase);
			_scrollMenu.TweenToCell(cellBase);
			ScrollCellSelectedHandler(cellBase, _scrollMenu.GetIndexFromCell(cellBase));
		}
		else
		{
			_selectedCellData = null;
			_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_DONATE_MORTY_DIALOGUE, MPMortyDefs.Name(_friendData.requestingMortyId), _friendData.username));
		}
	}

	private void CreateMortyCells(List<MPOpponentMortyData> mortyDatas)
	{
		List<MPOpponentMortyData> list = new List<MPOpponentMortyData>(mortyDatas.OrderBy((MPOpponentMortyData e) => e.level));
		_scrollMenu.AddHeaderCell(string.Format(MPTextDefs.MP_FRIEND_DONATE_MORTY_CELL_HEADER, MPMortyDefs.Name(_friendData.requestingMortyId)));
		int num = Mathf.Max(3, list.Count);
		for (int num2 = 0; num2 < num; num2++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPMortyCell"));
			MPMortyCell component = rectTransform.GetComponent<MPMortyCell>();
			component.SetUp((num2 >= list.Count) ? null : list[num2], false, false, true);
			_scrollMenu.AddCell(component);
		}
	}

	private void ClearScrollMenu()
	{
		_scrollMenu.RemoveAllCells();
	}

	public void DisplayError(eFriendDonateMortyError error)
	{
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_DONATE_MORTY_FAILED_DIALOGUE, MPMortyDefs.Name(_friendData.requestingMortyId), _friendData.username));
	}

	private void ScrollCellSelectedHandler(CellBase cell, int index)
	{
		_mainMenu.SetBtnEnabled(0, true);
		_selectedCellData = ((MPMortyCell)cell).data;
		_dialogue.TweenText(string.Format(MPTextDefs.MP_FRIEND_DONATE_MORTY_CELL_SELECTED_DIALOGUE, MPMortyDefs.Name(_friendData.requestingMortyId), _friendData.username));
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.DonateEvent != null)
			{
				if (App.net.player.data.IsMortyInParty(_selectedCellData) && App.net.player.data.GetActiveDeckCount() <= 1)
				{
					_dialogue.TweenText(MPTextDefs.MP_FRIEND_DONATE_MORTY_LAST_MORTY_DIALOGUE);
					break;
				}
				if (App.net.player.data.IsMortyInParty(_selectedCellData) && App.net.player.data.GetActiveDeckAliveCount() <= 1 && !_selectedCellData.isFainted)
				{
					_dialogue.TweenText(MPTextDefs.MP_FRIEND_DONATE_MORTY_LAST_ALIVE_MORTY_DIALOGUE);
					break;
				}
				StartCoroutine(_mainMenu.TweenOut());
				_dialogue.TweenText(MPTextDefs.MP_FRIEND_DONATE_MORTY_WAITING_DIALOGUE);
				this.DonateEvent(_selectedCellData);
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
