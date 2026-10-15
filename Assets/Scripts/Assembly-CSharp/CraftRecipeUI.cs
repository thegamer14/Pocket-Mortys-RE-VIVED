using UnityEngine;

public class CraftRecipeUI : UIBase
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
		for (int i = 0; i < RecipeDefs.Count; i++)
		{
			AddRecipeCell(RecipeDefs.GetID(i));
		}
	}

	private void AddRecipeCell(string id)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "RecipeCell"));
		RecipeCell component = rectTransform.GetComponent<RecipeCell>();
		int craftableQuantity = RecipeDefs.GetCraftableQuantity(id, ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot1(id)), ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot2(id)), ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot3(id)));
		component.SetUp(id, craftableQuantity, PlayerDefs.HasRecipedBeenLearned(id));
		_recipeMenu.AddCell(component);
	}

	private void RecipeCellSelectedHandler(CellBase cell, int index)
	{
		_selectedRecipeID = cell.id;
		if (!PlayerDefs.HasRecipedBeenLearned(_selectedRecipeID))
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_LOCKED));
			_mainMenu.SetBtnEnabled(0, false);
			return;
		}
		string itemID = RecipeDefs.ResultID(_selectedRecipeID);
		string arg = ItemDefs.Name(RecipeDefs.ResultID(_selectedRecipeID));
		if (RecipeDefs.GetCraftableQuantity(_selectedRecipeID, ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot1(_selectedRecipeID)), ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot2(_selectedRecipeID)), ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, RecipeDefs.Slot3(_selectedRecipeID))) == 0)
		{
			_dialogue.SetText(string.Format(TextDefs.CRAFT_RECIPE_DIAL_MISSING_ITEMS, arg));
			_mainMenu.SetBtnEnabled(0, false);
		}
		else if (ItemDefs.ItemsIndividualCount(SharedStatInfo.craftInfo.items, itemID) >= 10)
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
			if (this.CraftEvent != null)
			{
				this.CraftEvent(_selectedRecipeID);
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
