using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrophyCollectionUI : UIBase
{
	public delegate void TrophyCollectionUIBtnDelegate();

	private const int _kTrophyHorizontalCount = 5;

	private const float _kScrollMenuPaddingTop = 470f;

	private const float _kScrollMenuPaddingBottom = 150f;

	private const float _kScrollMenuHorizontalGap = 220f;

	private const float _kScrollMenuVerticalGap = 350f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	public Slider slider;

	private ScrollRect _scrollRect;

	private Text _dividerTitle;

	private List<TrophyUI> _trophies;

	public event TrophyCollectionUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_scrollRect = base.transform.FindChild("TrophyMenu").GetComponent<ScrollRect>();
		_dividerTitle = base.transform.FindChild("TrophyMenu/Container/Divider/Label").GetComponent<Text>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_trophies = new List<TrophyUI>();
	}

	public void SetUp()
	{
		CreateShelfCells();
		CreateTrophyCells();
		float y = 470f + (float)Mathf.FloorToInt(_trophies.Count / 5) * 350f + 150f;
		_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, y);
	}

	public override void SetLabels()
	{
		_dividerTitle.text = TextDefs.TROPHY_COLLECTION_DIVIDER_TITLE;
		_dialogue.SetText(TextDefs.TROPHY_COLLECTION_DIAL_TITLE);
		_mainMenu.SetBtnTitle(0, TextDefs.TROPHY_COLLECTION_BTN_BADGES);
	}

	private void Update()
	{
		if (slider != null)
		{
			slider.value = _scrollRect.normalizedPosition.y;
		}
	}

	protected override void AddEvents()
	{
		App.states.BackButtonEvent += BackButtonHandler;
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		App.states.BackButtonEvent -= BackButtonHandler;
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		for (int i = 0; i < _trophies.Count; i++)
		{
			_trophies[i].PressedEvent -= TrophyPressed;
		}
	}

	private void CreateShelfCells()
	{
		int trophyCount = PlayerDefs.TrophyCount;
		int num = Mathf.FloorToInt(trophyCount / 6) + 1;
		for (int i = 0; i < num; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/TrophyCollection/{0}", "Shelf"));
			rectTransform.SetParent(_scrollRect.content, false);
			float y = -470f - (float)i * 350f;
			rectTransform.localPosition = new Vector3(0f, y);
		}
		if (num <= 1)
		{
			slider.gameObject.SetActive(false);
			_scrollRect.vertical = false;
		}
	}

	private void CreateTrophyCells()
	{
		int trophyCount = PlayerDefs.TrophyCount;
		for (int i = 0; i < trophyCount; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/TrophyCollection/{0}", "Trophy"));
			TrophyUI component = rectTransform.GetComponent<TrophyUI>();
			rectTransform.SetParent(_scrollRect.content, false);
			float x = -440f + (float)(i % 5) * 220f;
			float y = -470f - (float)Mathf.FloorToInt(i / 5) * 350f;
			rectTransform.localPosition = new Vector3(x, y);
			string trophyID = PlayerDefs.GetTrophyID(i);
			component.SetUp(trophyID);
			component.PressedEvent += TrophyPressed;
			_trophies.Add(component);
		}
	}

	private void TrophyPressed(string id)
	{
		_dialogue.TweenText(string.Format(TextDefs.TROPHY_TITLE, TournamentDefs.TournamentName(id)));
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		if (index == 0 && this.BackEvent != null)
		{
			this.BackEvent();
		}
	}

	private void BackButtonHandler()
	{
		if (_canvasGroup.interactable && this.BackEvent != null)
		{
			this.BackEvent();
		}
	}
}
