using UnityEngine;

public class QuestBookUI : UIBase
{
	public delegate void QuestBookUIBtnDelegate();

	private const int _kActiveQuestMenuCellMin = 1;

	private const int _kCompleteQuestMenuCellMin = 1;

	private const float _kQuestMenuPaddingTop = 85f;

	private const float _kQuestMenuPaddingBottom = 100f;

	private const float _kQuestMenuCellGap = -4f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _questMenu;

	public event QuestBookUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_questMenu = base.transform.FindChild("QuestMenu").GetComponent<ScrollMenu>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
	}

	public void SetUp()
	{
		_questMenu.SetUp(0, false, 85f, 100f, -4f);
		CreateActiveQuestCells();
		CreateCompleteQuestCells();
	}

	public override void SetLabels()
	{
		if (PlayerDefs.GetActiveQuestCount > 0)
		{
			_dialogue.SetText(TextDefs.QUEST_BOOK_DIAL);
		}
		else
		{
			_dialogue.SetText(TextDefs.QUEST_BOOK_DIAL_NONE);
		}
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateActiveQuestCells()
	{
		_questMenu.AddHeaderCell(TextDefs.QUEST_BOOK_SCROLL_MENU_ACTIVE_HEADER);
		int num = Mathf.Max(PlayerDefs.GetActiveQuestCount, 1);
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			if (num2 < PlayerDefs.GetActiveQuestCount)
			{
				AddActiveQuestCell(PlayerDefs.GetActiveQuestForIndex(num2));
			}
			else
			{
				AddActiveQuestCell(null);
			}
		}
	}

	private void AddActiveQuestCell(string id)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "QuestActiveCell"));
		QuestActiveCell component = rectTransform.GetComponent<QuestActiveCell>();
		component.SetUp(id);
		_questMenu.AddCell(component);
	}

	private void CreateCompleteQuestCells()
	{
		_questMenu.AddHeaderCell(TextDefs.QUEST_BOOK_SCROLL_MENU_COMPLETE_HEADER);
		int num = Mathf.Max(PlayerDefs.GetCompleteQuestCount, 1);
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			if (num2 < PlayerDefs.GetCompleteQuestCount)
			{
				AddCompleteQuestCell(PlayerDefs.GetCompleteQuestForIndex(num2), PlayerDefs.GetCompleteQuestSolutionForIndex(num2));
			}
			else
			{
				AddCompleteQuestCell(null, null);
			}
		}
	}

	private void AddCompleteQuestCell(string id, string solutionID)
	{
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", "QuestCompleteCell"));
		QuestCompleteCell component = rectTransform.GetComponent<QuestCompleteCell>();
		component.SetUp(id, solutionID);
		_questMenu.AddCell(component);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (index == 0 && this.BackEvent != null)
		{
			this.BackEvent();
		}
	}
}
