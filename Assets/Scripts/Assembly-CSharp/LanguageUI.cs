using System;
using UnityEngine;

public class LanguageUI : UIBase
{
	public delegate void LanguageUISelectedDelegate(eLanguage language);

	public delegate void LanguageUIDelegate();

	private const float _kScrollMenuPaddingTop = 0f;

	private const float _kScrollMenuPaddingBottom = 0f;

	private const float _kScrollMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	public event LanguageUISelectedDelegate LanguageSelectedEvent;

	public event LanguageUIDelegate BackEvent;

	protected override void LateAwake()
	{
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
	}

	public void SetUp()
	{
		_scrollMenu.SetUp(1, false, 0f, 0f, -4f);
		CreateLanguageCells();
		_scrollMenu.TryCenterAndSelectCellWithID(LocalizationDefs.GetCurrentLanguage().ToString());
	}

	public override void SetLabels()
	{
		_dialogue.SetText(TextDefs.LANGUAGE_SELECT_DIAL);
		_mainMenu.SetBtnTitle(0, TextDefs.LANGUAGE_SELECT_BTN_OK);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_scrollMenu.SelectedEvent += LanguageCellSelectedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_scrollMenu.SelectedEvent -= LanguageCellSelectedHandler;
	}

	private void CreateLanguageCells()
	{
		foreach (eLanguage value in Enum.GetValues(typeof(eLanguage)))
		{
			AddLanguageCell(value);
		}
	}

	private void AddLanguageCell(eLanguage language)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "LanguageCell"));
		LanguageCell component = rectTransform.GetComponent<LanguageCell>();
		component.SetUp(language);
		_scrollMenu.AddCell(component);
	}

	private void MainMenuBtnPressedHandler(int index)
	{
		if (index == 0 && this.BackEvent != null)
		{
			this.BackEvent();
		}
	}

	private void LanguageCellSelectedHandler(CellBase cell, int index)
	{
		if (this.LanguageSelectedEvent != null)
		{
			this.LanguageSelectedEvent((eLanguage)Enum.Parse(typeof(eLanguage), cell.id));
		}
	}
}
