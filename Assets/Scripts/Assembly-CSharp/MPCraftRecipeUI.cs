using System.Collections.Generic;
using UnityEngine;

public class MPCraftRecipeUI : UIBase
{
	public delegate void CraftRecipeUIBtnDelegate();

	public delegate void CraftUIResultDelegate(string id);

	private const float _kRecipeMenuPaddingTop = 85f;

	private const float _kRecipeMenuPaddingBottom = 100f;

	private const float _kRecipeMenuCellGap = -4f;

	private string _selectedRecipeID;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _recipeMenu;

	public event CraftRecipeUIBtnDelegate BackEvent;

	public event CraftUIResultDelegate CraftEvent;

	protected override void LateAwake()
	{
		_recipeMenu = base.transform.FindChild("RecipeMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp(string selectedRecipeID)
	{
		_recipeMenu.SetUp(1, false, 85f, 100f, -4f);
		_mainMenu.SetBtnEnabled(0, false);
		CreateRecipeCells();
		_dialogue.SetText(TextDefs.CRAFT_RECIPE_DIAL);
		if (!string.IsNullOrEmpty(selectedRecipeID))
		{
			_recipeMenu.TryCenterAndSelectCellWithID(selectedRecipeID);
		}
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.CRAFT_RECIPE_BTN);
	}

	protected override void AddEvents()
	{
		_recipeMenu.SelectedEvent += RecipeCellSelectedHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_recipeMenu.SelectedEvent -= RecipeCellSelectedHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateRecipeCells()
	{
		SortedList<int, string> orderedIds = MPRecipeDefs.GetOrderedIds();
		foreach (KeyValuePair<int, string> item in orderedIds)
		{
			AddRecipeCell(item.Value, item.Key);
		}
	}

	private void AddRecipeCell(string id, int number)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "MPRecipeCell"));
		MPRecipeCell component = rectTransform.GetComponent<MPRecipeCell>();
		int craftableQuantity = MPRecipeDefs.GetCraftableQuantity(id, MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot1(id)), MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot2(id)), MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot3(id)));
		component.SetUp(id, number, craftableQuantity, App.net.player.data.recipes.Contains(id));
		_recipeMenu.AddCell(component);
	}

	public void TryCraft()
	{
		if (!string.IsNullOrEmpty(_selectedRecipeID))
		{
			StartCoroutine(_mainMenu.TweenOut());
			_dialogue.TweenText(MPTextDefs.MP_CRAFT_WAITING);
			if (this.CraftEvent != null)
			{
				this.CraftEvent(_selectedRecipeID);
			}
		}
	}

	public void TryCraftFailed(eCraftRecipeError error)
	{
		StartCoroutine(_mainMenu.TweenIn());
		_dialogue.TweenText(MPTextDefs.MP_CRAFT_ERROR);
	}

	private void RecipeCellSelectedHandler(CellBase cell, int index)
	{
		_selectedRecipeID = cell.id;
		if (!App.net.player.data.recipes.Contains(_selectedRecipeID))
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_LOCKED));
			_mainMenu.SetBtnEnabled(0, false);
			return;
		}
		string itemID = MPRecipeDefs.ResultID(_selectedRecipeID);
		string arg = MPItemDefs.Name(MPRecipeDefs.ResultID(_selectedRecipeID));
		if (MPRecipeDefs.GetCraftableQuantity(_selectedRecipeID, MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot1(_selectedRecipeID)), MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot2(_selectedRecipeID)), MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, MPRecipeDefs.Slot3(_selectedRecipeID))) == 0)
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_MISSING_ITEMS, arg));
			_mainMenu.SetBtnEnabled(0, false);
		}
		else if (MPItemDefs.ItemsIndividualCount(MPSharedStateInfo.craftInfo.items, itemID) >= 10)
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_NOT_ENOUGH_SPACE, arg));
			_mainMenu.SetBtnEnabled(0, false);
		}
		else
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_QUICK_CRAFT, arg));
			_mainMenu.SetBtnEnabled(0, true);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			TryCraft();
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
