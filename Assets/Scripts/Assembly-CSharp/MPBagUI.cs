using System.Collections.Generic;
using UnityEngine;

public class MPBagUI : UIBase
{
	public delegate void BagUIItemDelegate(string id);

	public delegate void BagUIBtnDelegate();

	private const float _kItemMenuPaddingTop = 85f;

	private const float _kItemMenuPaddingBottom = 100f;

	private const float _kItemMenuCellGap = -4f;

	private const int _kItemMinimumCellCount = 3;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private CellBase _selectedCell;

	private int _itemTypeCellCount;

	private int _partTypeCellCount;

	private MPOpponentMortyData _selectedMorty;

	public event BagUIItemDelegate ItemSelectedEvent;

	public event BagUIBtnDelegate UseEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollMenu = base.transform.FindChild("ItemMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(string.Format("UI/MPBattle/{0}", "Timer")));
		gameObject.transform.SetParent(base.transform);
	}

	public void SetUp(Dictionary<string, int> items, string selectedItemId, MPOpponentMortyData selectedMorty)
	{
		_selectedMorty = selectedMorty;
		_mainMenu.SetBtnEnabled(0, false);
		_scrollMenu.SetUp(1, false, 85f, 100f, -4f);
		_itemTypeCellCount = CreateItemCells(items, eMPItemType.ITEM, TextDefs.BAG_SCROLL_MENU_ITEM_HEADER);
		if (!MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			_partTypeCellCount = CreateItemCells(items, eMPItemType.PART, TextDefs.BAG_SCROLL_MENU_PART_HEADER);
		}
		_scrollMenu.TryCenterAndSelectCellWithID(selectedItemId);
		SetLabels();
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.BAG_MAIN_MENU_SELECTED_BTN);
		if (_selectedMorty == null)
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
			_dialogue.SetText(string.Format(TextDefs.BAG_DIAL_SET_UP_PARTY, MPMortyDefs.Name(_selectedMorty.id)));
		}
	}

	protected override void AddEvents()
	{
		_scrollMenu.SelectedEvent += ScrollMenuCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_scrollMenu.SelectedEvent -= ScrollMenuCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private int CreateItemCells(Dictionary<string, int> items, eMPItemType type, string header)
	{
		_scrollMenu.AddHeaderCell(header);
		if (type == eMPItemType.ITEM && App.net.player.data.coupons > 0)
		{
			AddCouponCell();
		}
		SortedList<int, string> orderedListOfItems = MPItemDefs.GetOrderedListOfItems(items, type);
		int num = Mathf.Max(3, orderedListOfItems.Count);
		for (int i = 0; i < num; i++)
		{
			if (i < orderedListOfItems.Count)
			{
				string text = orderedListOfItems.Values[i];
				AddItemCell(text, items[text]);
			}
			else
			{
				AddItemCell(null);
			}
		}
		return orderedListOfItems.Count;
	}

	private void AddItemCell(string id, int count = 0)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemDescriptionCell"));
		ItemCell component = rectTransform.GetComponent<ItemCell>();
		if (id != null)
		{
			component.SetUp(id, MPItemDefs.Name(id), MPItemDefs.Description(id), MPItemDefs.AssetID(id), count);
		}
		else
		{
			component.SetUp(null, null, null, null, 0);
		}
		_scrollMenu.AddCell(component);
	}

	private void AddCouponCell()
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "ItemDescriptionCell"));
		ItemCell component = rectTransform.GetComponent<ItemCell>();
		component.SetUp("ItemCouponMP", MPItemDefs.Name("ItemCouponMP"), MPItemDefs.Description("ItemCouponMP"), MPItemDefs.AssetID("ItemCouponMP"), App.net.player.data.coupons);
		_scrollMenu.AddCell(component);
	}

	public void ScrollMenuCellSelectedHandler(CellBase cell, int index)
	{
		_selectedCell = cell;
		if (this.ItemSelectedEvent != null)
		{
			this.ItemSelectedEvent(_selectedCell.id);
		}
		if (cell.id.Equals("ItemCouponMP"))
		{
			_mainMenu.SetBtnEnabled(0, false);
			_dialogue.TweenText(MPTextDefs.MP_BAG_DIAL_ITEM_COUPON);
			return;
		}
		if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.WORLD))
		{
			if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.CRAFT))
			{
				if (!MPItemDefs.UsableInCraft(_selectedCell.id))
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_CRAFT);
					return;
				}
			}
			else if (!MPItemDefs.UsableInWorld(_selectedCell.id))
			{
				if (MPItemDefs.UsableInBattle(_selectedCell.id))
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD_IN_BATTLE);
				}
				else if (MPItemDefs.UsableInCraft(_selectedCell.id))
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD_IN_CRAFT);
				}
				else
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_WORLD);
				}
				return;
			}
		}
		if (MPSharedStateInfo.bagInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			if (!MPItemDefs.UsableInBattle(_selectedCell.id))
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_BATTLE);
				return;
			}
			if (MPItemDefs.EffectType(_selectedCell.id).Equals("Capture"))
			{
				if (MPSharedStateInfo.battleInfo.enemy.opponentData.type != eMPOpponentType.WILD_MORTY)
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(TextDefs.BAG_DIAL_ITEM_NOT_IN_BATTLE_TRAINER);
					return;
				}
				if (App.net.player.data.morties.Count >= 300)
				{
					_mainMenu.SetBtnEnabled(0, false);
					_dialogue.TweenText(MPTextDefs.MP_BAG_DIAL_ITEM_NOT_IN_BATTLE_MAX_MORTIES);
					return;
				}
			}
		}
		if (_selectedMorty != null)
		{
			string resultText;
			if (MPItemDefs.CanItemBeApplyedToMorty(_selectedCell.id, _selectedMorty, out resultText))
			{
				_mainMenu.SetBtnEnabled(0, true);
				_dialogue.TweenText(string.Format(TextDefs.BAG_DIAL_ITEM_USE_ON_MORTY, MPItemDefs.Name(_selectedCell.id), MPMortyDefs.Name(_selectedMorty.id)));
			}
			else
			{
				_mainMenu.SetBtnEnabled(0, false);
				_dialogue.TweenText(resultText);
			}
		}
		else
		{
			_mainMenu.SetBtnEnabled(0, true);
			_dialogue.TweenText(string.Format(TextDefs.BAG_DIAL_ITEM_USE, MPItemDefs.Name(_selectedCell.id)));
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
}
