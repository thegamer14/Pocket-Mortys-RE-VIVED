using System.Collections.Generic;
using UnityEngine;

public class TournamentPartySelectUI : UIBase
{
	public delegate void TournamentPartySelectUIContDelegate(List<KeyValuePair<MortyData, int>> selectedMorties, List<KeyValuePair<string, int>> selectedItems);

	public delegate void TournamentPartySelectUIStatDelegate(MortyData mortyData);

	public delegate void TournamentPartySelectUIBtnDelegate();

	private const float _kMortyMenuPaddingTop = 85f;

	private const float _kMortyMenuPaddingBottom = 100f;

	private const float _kMortyMenuCellGap = -4f;

	private TournamentPartyBPBar _bpBar;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private int _totalBattlePoints;

	private int _remainingBattlePoints;

	private List<KeyValuePair<MortyData, int>> _selectedMorties;

	private List<KeyValuePair<string, int>> _selectedItems;

	private Dictionary<CellBase, KeyValuePair<MortyData, int>> _linkedMortyCells;

	private Dictionary<CellBase, KeyValuePair<string, int>> _linkedItemCells;

	public event TournamentPartySelectUIContDelegate ContinueEvent;

	public event TournamentPartySelectUIStatDelegate StatsEvent;

	public event TournamentPartySelectUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_bpBar = base.transform.FindChild("BattlePointsBar").GetComponent<TournamentPartyBPBar>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(string eventId)
	{
		_selectedMorties = new List<KeyValuePair<MortyData, int>>();
		_selectedItems = new List<KeyValuePair<string, int>>();
		_linkedMortyCells = new Dictionary<CellBase, KeyValuePair<MortyData, int>>();
		_linkedItemCells = new Dictionary<CellBase, KeyValuePair<string, int>>();
		_totalBattlePoints = TournamentDefs.EventBattlePoints(eventId);
		_remainingBattlePoints = _totalBattlePoints;
		_scrollMenu.SetUp(9999, true, 85f, 100f, -4f);
		CreateScrollSelectCells(TournamentDefs.EventSelectableMorties(eventId), TournamentDefs.EventSelectableItems(eventId));
		_bpBar.SetUp(_totalBattlePoints);
		_mainMenu.SetBtnEnabled(0, false);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.TOURNAMENT_PARTY_SELECT_BTN_CONTINUE);
		if (TournamentDefs.EventSelectableItems(SharedStatInfo.tournamentSetUpInfo.eventID).Count > 0)
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_PARTY_SELECT_MORTYS_AND_ITEMS_DIAL);
		}
		else
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_PARTY_SELECT_MORTYS_DIAL);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		RemoveAllCells();
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += MortyCellSelectedHandler;
		_scrollMenu.UnselectedEvent += MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= MortyCellSelectedHandler;
		_scrollMenu.UnselectedEvent -= MortyCellUnselectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateScrollSelectCells(List<KeyValuePair<MortyData, int>> availableMorties, List<KeyValuePair<string, int>> availableItems)
	{
		availableMorties.Sort((KeyValuePair<MortyData, int> s1, KeyValuePair<MortyData, int> s2) => s1.Value.CompareTo(s2.Value));
		availableItems.Sort((KeyValuePair<string, int> s1, KeyValuePair<string, int> s2) => s1.Value.CompareTo(s2.Value));
		_scrollMenu.AddHeaderCell("Mortys");
		for (int num = 0; num < availableMorties.Count; num++)
		{
			AddMortyCell(availableMorties[num]);
		}
		if (availableItems.Count > 0)
		{
			_scrollMenu.AddHeaderCell("Items");
		}
		for (int num2 = 0; num2 < availableItems.Count; num2++)
		{
			AddItemCell(availableItems[num2]);
		}
	}

	private void AddMortyCell(KeyValuePair<MortyData, int> keyValue)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MortyTournamentCell"));
		MortyTournamentCell component = rectTransform.GetComponent<MortyTournamentCell>();
		component.SetUp(keyValue.Key, false, keyValue.Value);
		component.StatsBtnEvent += StatsBtnHandler;
		_scrollMenu.AddCell(component);
		_linkedMortyCells.Add(component, keyValue);
	}

	private void AddItemCell(KeyValuePair<string, int> keyValue)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemTournamentCell"));
		ItemTournamentCell component = rectTransform.GetComponent<ItemTournamentCell>();
		component.SetUp(keyValue.Key, keyValue.Value);
		_scrollMenu.AddCell(component);
		_linkedItemCells.Add(component, keyValue);
	}

	private void RemoveAllCells()
	{
		List<CellBase> cellListCopy = _scrollMenu.GetCellListCopy();
		for (int i = 0; i < cellListCopy.Count; i++)
		{
			if (cellListCopy[i] is MortyTournamentCell)
			{
				((MortyTournamentCell)cellListCopy[i]).StatsBtnEvent -= StatsBtnHandler;
			}
			_scrollMenu.RemoveCell(cellListCopy[i]);
		}
	}

	private void RefreshCells()
	{
		for (int i = 0; i < _scrollMenu.count; i++)
		{
			CellBase cellAtIndex = _scrollMenu.GetCellAtIndex(i);
			if (cellAtIndex.state == eScrollMenuCellState.UNUSED)
			{
				continue;
			}
			bool flag = false;
			int num = -1;
			if (_linkedMortyCells.ContainsKey(cellAtIndex))
			{
				num = _linkedMortyCells[cellAtIndex].Value;
				if (_selectedMorties.Count >= 5)
				{
					flag = true;
				}
			}
			else if (_linkedItemCells.ContainsKey(cellAtIndex))
			{
				num = _linkedItemCells[cellAtIndex].Value;
			}
			if (cellAtIndex.state == eScrollMenuCellState.UNSELECTED && (flag || _remainingBattlePoints < num))
			{
				cellAtIndex.SetEnabled(false);
			}
			else if (cellAtIndex.state == eScrollMenuCellState.DISABLED && !flag && _remainingBattlePoints >= num)
			{
				cellAtIndex.SetEnabled(true);
			}
		}
	}

	public void MortyCellSelectedHandler(CellBase cell, int index)
	{
		if (_linkedMortyCells.ContainsKey(cell))
		{
			KeyValuePair<MortyData, int> item = _linkedMortyCells[cell];
			_selectedMorties.Add(item);
			_remainingBattlePoints -= item.Value;
			_mainMenu.SetBtnEnabled(0, true);
		}
		else if (_linkedItemCells.ContainsKey(cell))
		{
			KeyValuePair<string, int> item2 = _linkedItemCells[cell];
			_selectedItems.Add(item2);
			_remainingBattlePoints -= item2.Value;
		}
		_bpBar.TweenPoints(_totalBattlePoints, _remainingBattlePoints);
		RefreshCells();
	}

	public void MortyCellUnselectedHandler(CellBase cell, int index)
	{
		if (_linkedMortyCells.ContainsKey(cell))
		{
			KeyValuePair<MortyData, int> item = _linkedMortyCells[cell];
			_selectedMorties.Remove(item);
			_remainingBattlePoints += item.Value;
			if (_selectedMorties.Count <= 0)
			{
				_mainMenu.SetBtnEnabled(0, false);
			}
		}
		else if (_linkedItemCells.ContainsKey(cell))
		{
			KeyValuePair<string, int> item2 = _linkedItemCells[cell];
			_selectedItems.Remove(item2);
			_remainingBattlePoints += item2.Value;
		}
		_bpBar.TweenPoints(_totalBattlePoints, _remainingBattlePoints);
		RefreshCells();
	}

	public void StatsBtnHandler(MortyData mortyData)
	{
		if (this.StatsEvent != null)
		{
			this.StatsEvent(mortyData);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.ContinueEvent != null)
			{
				this.ContinueEvent(_selectedMorties, _selectedItems);
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
