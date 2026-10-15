using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SalesmanRickItemsUI : UIBase
{
	public delegate void BagUIDelegate();

	public delegate void BagUIIDBtnDelegate(string itemID);

	public delegate void BagUIBtnDelegate();

	private const int _kItemMenuMinimumCellCount = 5;

	private const float _kItemMenuPaddingTop = 85f;

	private const float _kItemMenuPaddingBottom = 100f;

	private const float _kItemMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private CoinBar _coinBar;

	private ScrollMenu _itemMenu;

	private string _selectedItemID;

	private int _coinValue;

	private Tween _coinTween;

	public event BagUIDelegate BlipsAndChitzSeletedEvent;

	public event BagUIIDBtnDelegate BuyEvent;

	public event BagUIBtnDelegate BackEvent;

	public event BagUIBtnDelegate GetCoinsEvent;

	protected override void LateAwake()
	{
		_itemMenu = base.transform.FindChild("ItemMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_coinBar = base.transform.FindChild("MenuBar/MainMenu/CoinBar").GetComponent<CoinBar>();
	}

	public void SetUp()
	{
		_selectedItemID = null;
		_itemMenu.SetUp(1, false, 85f, 100f, -4f);
		CreateItemCells();
		_mainMenu.SetPositionIn();
		_mainMenu.SetBtnEnabled(1, false);
		_coinValue = PlayerDefs.CoinCount;
		_coinBar.SetUp(PlayerDefs.CoinCount);
	}

	public override void SetLabels()
	{
		_coinBar.SetLabels();
		_mainMenu.SetBtnTitle(1, TextDefs.SALESMAN_RICK_BTN_BUY);
		_dialogue.SetText(TextDefs.SALESMAN_RICK_ITEMS_DIAL);
	}

	protected override void AddEvents()
	{
		_itemMenu.SelectedEvent += ItemCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_itemMenu.SelectedEvent -= ItemCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateItemCells()
	{
		if (!App.saveData.settings.removeAds)
		{
			AddBlipsAndChitzCell();
		}
		List<string> badgeLockedPurchasableItemIDs = ItemDefs.GetBadgeLockedPurchasableItemIDs();
		int num = Mathf.Max(5, badgeLockedPurchasableItemIDs.Count);
		for (int i = 0; i < num; i++)
		{
			AddItemCell((i >= badgeLockedPurchasableItemIDs.Count) ? null : badgeLockedPurchasableItemIDs[i]);
		}
	}

	private void AddItemCell(string id)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "SalesmanRickItemCell"));
		SalesmanRickItemCell component = rectTransform.GetComponent<SalesmanRickItemCell>();
		component.SetUp(id);
		_itemMenu.AddCell(component);
	}

	private void AddBlipsAndChitzCell()
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "BlipsAndChitzCell"));
		CellBase component = rectTransform.GetComponent<CellBase>();
		_itemMenu.AddCell(component);
	}

	private void SetCoins(int coins)
	{
		_coinValue = coins;
		_coinBar.SetCount(_coinValue);
	}

	private void TweenCoins(int coins)
	{
		_coinTween = DOTween.To(() => _coinValue, delegate(int x)
		{
			_coinValue = x;
		}, coins, 1f);
		_coinTween.OnUpdate(delegate
		{
			SetCoins(_coinValue);
		});
	}

	public void PurchaseComplete(string outcomeText)
	{
		TweenCoins(PlayerDefs.CoinCount);
		_dialogue.TweenText(outcomeText);
	}

	public void RemoveAdsPurchased()
	{
		_itemMenu.RemoveAllCellsOfType(typeof(BlipsAndChitzCell));
	}

	public void ItemCellSelectedHandler(CellBase cell, int index)
	{
		if (cell.GetType() == typeof(BlipsAndChitzCell))
		{
			if (this.BlipsAndChitzSeletedEvent != null)
			{
				this.BlipsAndChitzSeletedEvent();
			}
		}
		else
		{
			_mainMenu.SetBtnEnabled(1, true);
			SalesmanRickItemCell salesmanRickItemCell = (SalesmanRickItemCell)cell;
			_selectedItemID = salesmanRickItemCell.id;
			_dialogue.TweenText(string.Format(TextDefs.ITEM_BUY, ItemDefs.Name(salesmanRickItemCell.id)));
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			App.sound.PlaySFX("Audio/SFX/GenericButton");
			if (this.GetCoinsEvent != null)
			{
				this.GetCoinsEvent();
			}
			break;
		case 1:
			if (this.BuyEvent != null)
			{
				this.BuyEvent(_selectedItemID);
			}
			break;
		case 2:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		}
	}
}
