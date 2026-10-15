using System.Collections.Generic;
using UnityEngine;

public class BagUI : UIBase
{
	public delegate void BagUIItemDelegate(CellBase selectedCell);

	public delegate void BagUIBtnDelegate();

	private const float _kItemMenuPaddingTop = 85f;

	private const float _kItemMenuPaddingBottom = 100f;

	private const float _kItemMenuCellGap = -4f;

	private const int _kItemMinimumCellCount = 3;

	private MenuBase _mainMenu;

	private MenuBase _giveMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private CellBase _selectedCell;

	private int _itemTypeCellCount;

	private int _partTypeCellCount;

	private MortyData _selectedMorty;

	public event BagUIItemDelegate ItemSelectedEvent;

	public event BagUIBtnDelegate UseEvent;

	public event BagUIBtnDelegate GiveEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollMenu = base.transform.FindChild("ItemMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_giveMenu = base.transform.FindChild("MenuBar/GiveMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(Dictionary<string, int> items, string selectedItemID, MortyData selectedMorty)
	{
		_selectedMorty = selectedMorty;
		if (SharedStatInfo.bagInfo.questGiverNode == null)
		{
			_mainMenu.SetPositionIn();
			_giveMenu.SetPositionOut();
			_mainMenu.SetBtnEnabled(0, false);
		}
		else
		{
			_mainMenu.SetPositionOut();
			_giveMenu.SetPositionIn();
			_giveMenu.SetBtnEnabled(0, false);
		}
		_scrollMenu.SetUp(1, false, 85f, 100f, -4f);
		_itemTypeCellCount = CreateItemCells(eItemType.ITEM, items, TextDefs.BAG_SCROLL_MENU_ITEM_HEADER);
		if (SharedStatInfo.bagInfo.fromState != eBagFromState.BATTLE && PlayerDefs.GetLocationType() != eLocationType.TOURNAMENT_ARENA)
		{
			_partTypeCellCount = CreateItemCells(eItemType.PART, items, TextDefs.BAG_SCROLL_MENU_PART_HEADER);
		}
		_scrollMenu.TryCenterAndSelectCellWithID(selectedItemID);
		SetLabels();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.BAG_MAIN_MENU_SELECTED_BTN);
		_giveMenu.SetBtnTitle(0, TextDefs.BAG_GIVE_MENU_SELECTED_BTN);
		if (SharedStatInfo.bagInfo.questGiverNode != null)
		{
			_dialogue.SetText(string.Format(TextDefs.BAG_DIAL_SET_UP_QUEST, QuestDefs.GiverName(SharedStatInfo.bagInfo.questGiverNode.data.id)));
		}
		else if (_selectedMorty == null)
		{
			if (_itemTypeCellCount == 0)
			{
				if (_partTypeCellCount == 0)
				{
					_dialogue.SetText(TextDefs.BAG_DIAL_SET_UP_EMPTY);
				}
				else
				{
					_dialogue.SetText(TextDefs.BAG_DIAL_SET_UP_NO_ITEMS);
				}
			}
			else
			{
				_dialogue.SetText(TextDefs.BAG_DIAL_SET_UP);
			}
		}
		else
		{
			_dialogue.SetText(string.Format(TextDefs.BAG_DIAL_SET_UP_PARTY, MortyDefs.Name(_selectedMorty.id)));
		}
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += ScrollMenuCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_giveMenu.BtnPressedEvent += GiveMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= ScrollMenuCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_giveMenu.BtnPressedEvent -= GiveMenuBtnPressedHandler;
	}

	private int CreateItemCells(eItemType type, Dictionary<string, int> items, string header)
	{
		_scrollMenu.AddHeaderCell(header);
		if (type == eItemType.ITEM && PlayerDefs.GetLocationType() != eLocationType.TOURNAMENT_ARENA)
		{
			if (PlayerDefs.CouponCount > 0)
			{
				AddCouponCell();
			}
			if (TournamentDefs.currentTicketCount > 0)
			{
				AddTournamentTicketCell();
			}
		}
		List<string> itemsOfType = ItemDefs.GetItemsOfType(type, items);
		itemsOfType.Sort((string s1, string s2) => ItemDefs.DisplayOrder(s1).CompareTo(ItemDefs.DisplayOrder(s2)));
		int num = Mathf.Max(3, itemsOfType.Count);
		for (int num2 = 0; num2 < num; num2++)
		{
			if (num2 < itemsOfType.Count)
			{
				AddItemCell(itemsOfType[num2], items[itemsOfType[num2]]);
			}
			else
			{
				AddItemCell(null);
			}
		}
		return itemsOfType.Count;
	}

	private void AddItemCell(string id, int count = 0)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemDescriptionCell"));
		ItemCell component = rectTransform.GetComponent<ItemCell>();
		if (id != null)
		{
			component.SetUp(id, ItemDefs.Name(id), ItemDefs.Description(id), ItemDefs.AssetID(id), count);
		}
		else
		{
			component.SetUp(null, null, null, null, 0);
		}
		_scrollMenu.AddCell(component);
	}

	private void AddTournamentTicketCell()
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemDescriptionCell"));
		ItemCell component = rectTransform.GetComponent<ItemCell>();
		component.SetUp("ItemTournamentTicket", ItemDefs.Name("ItemTournamentTicket"), ItemDefs.Description("ItemTournamentTicket"), ItemDefs.AssetID("ItemTournamentTicket"), TournamentDefs.currentTicketCount);
		_scrollMenu.AddCell(component);
	}

	private void AddCouponCell()
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemDescriptionCell"));
		ItemCell component = rectTransform.GetComponent<ItemCell>();
		component.SetUp("ItemCoupon", ItemDefs.Name("ItemCoupon"), ItemDefs.Description("ItemCoupon"), ItemDefs.AssetID("ItemCoupon"), PlayerDefs.CouponCount);
		_scrollMenu.AddCell(component);
	}

	public void ScrollMenuCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = cell;
		if (cell.id.Equals("ItemCoupon"))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_COUPON);
		}
		else if (SharedStatInfo.bagInfo.fromState == eBagFromState.BATTLE && !ItemDefs.UsableInBattle(_selectedCell.id))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_BATTLE);
		}
		else if (SharedStatInfo.bagInfo.fromState == eBagFromState.BATTLE && SharedStatInfo.battleInfo.enemy.type != eBattleOpponentType.AI_WILD_MORTY && ItemDefs.EffectType(_selectedCell.id).Equals("Capture"))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_BATTLE_TRAINER);
		}
		else if (SharedStatInfo.bagInfo.fromState == eBagFromState.WORLD && !ItemDefs.UsableInWorld(_selectedCell.id))
		{
			if (ItemDefs.UsableInBattle(_selectedCell.id))
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD_IN_BATTLE);
			}
			else if (ItemDefs.UsableInCraft(_selectedCell.id))
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD_IN_CRAFT);
			}
			else
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD);
			}
		}
		else if (SharedStatInfo.bagInfo.fromState == eBagFromState.CRAFT && !ItemDefs.UsableInCraft(_selectedCell.id))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_CRAFT);
		}
		else if (SharedStatInfo.bagInfo.fromState == eBagFromState.QUEST)
		{
			_giveMenu.SetBtnEnabled(0, true);
			_dialogue.TweenText(string.Format(TextDefs.BAG_DIAL_ITEM_IN_QUEST, ItemDefs.Name(cell.id), QuestDefs.GiverName(SharedStatInfo.bagInfo.questGiverNode.data.id)));
		}
		else if (_selectedMorty != null)
		{
			string resultText;
			if (ItemDefs.CheckCanApplyItemToMorty(_selectedCell.id, _selectedMorty, out resultText))
			{
				_mainMenu.SetBtnEnabled(0, true);
				_dialogue.TweenText(string.Format(TextDefs.BAG_DIAL_ITEM_USE_ON_MORTY, ItemDefs.Name(_selectedCell.id), MortyDefs.Name(_selectedMorty.id)));
			}
			else
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(resultText);
			}
		}
		else if (cell.id.Equals("ItemCourierFlap") && SharedStatInfo.bagInfo.fromState == eBagFromState.WORLD && !WorldDefs.IsDimensionPortal(SharedStatInfo.worldInfo.worldID))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_COURIER_FLAP_CITADEL);
		}
		else
		{
			_mainMenu.SetBtnEnabled(0, true);
			_dialogue.TweenText(string.Format(TextDefs.BAG_DIAL_ITEM_USE, ItemDefs.Name(_selectedCell.id)));
		}
		if (this.ItemSelectedEvent != null)
		{
			this.ItemSelectedEvent(_selectedCell);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.UseEvent != null)
			{
				this.UseEvent();
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

	public void GiveMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.GiveEvent != null)
			{
				this.GiveEvent();
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
