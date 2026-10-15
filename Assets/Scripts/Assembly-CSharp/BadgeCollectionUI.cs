using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgeCollectionUI : UIBase
{
	public delegate void BadgeUIBtnDelegate();

	private const int _kBadgeDisplayCapMin = 18;

	private const int _kBadgeDisplayCapMax = 102;

	private const int _kBadgesHorizontalCount = 6;

	private const float _kGymLeaderItemMenuPaddingTop = 435f;

	private const float _kCouncilMemberItemMenuPaddingTop = 120f;

	private const float _kItemMenuPaddingBottom = 20f;

	private const float _kItemMenuHorizontalGap = 195f;

	private const float _kItemMenuVerticalGap = 195f;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private Text _counterTitle;

	private Text _counter;

	public Slider slider;

	private ScrollRect _scrollRect;

	private Text _dividerTitle;

	private List<RectTransform> _badges;

	public event BadgeUIBtnDelegate BackEvent;

	public event BadgeUIBtnDelegate TrophiesEvent;

	protected override void LateAwake()
	{
		_scrollRect = base.transform.FindChild("BadgeMenu").GetComponent<ScrollRect>();
		_dividerTitle = base.transform.FindChild("BadgeMenu/Container/Divider/Label").GetComponent<Text>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_counterTitle = base.transform.FindChild("MenuBar/CounterBar/Image/Title").GetComponent<Text>();
		_counter = base.transform.FindChild("MenuBar/CounterBar/Image/Label").GetComponent<Text>();
		_badges = new List<RectTransform>();
	}

	public void SetUp()
	{
		CreateCouncilMemberItemCells();
		CreateGymLeaderItemCells();
		float y = 435f + (float)Mathf.FloorToInt(_badges.Count / 6) * 195f + 20f;
		_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, y);
	}

	public override void SetLabels()
	{
		_dividerTitle.text = TextDefs.BADGE_COLLECTION_DIVIDER_TITLE;
		_counterTitle.text = TextDefs.BADGE_COLLECTION_COUNT_TITLE;
		_counter.text = PlayerDefs.BadgeCount.ToString();
		_mainMenu.SetBtnTitle(1, TextDefs.BADGE_COLLECTION_BTN_TROPHY);
		_dialogue.SetText(TextDefs.BADGE_COLLECTION_DIAL_TITLE);
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
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	private void CreateCouncilMemberItemCells()
	{
		for (int i = 0; i < 6; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/BadgeCollection/{0}", "BadgeCouncil"));
			BadgeUI component = rectTransform.GetComponent<BadgeUI>();
			rectTransform.SetParent(_scrollRect.content, false);
			float x = -487.5f + (float)(i % 6) * 195f;
			float y = -120f - (float)Mathf.FloorToInt(i / 6) * 195f;
			rectTransform.localPosition = new Vector3(x, y);
			component.SetUp((i >= PlayerDefs.DefeatedCouncilMembers) ? null : ("CouncilMember/" + (i + 1)), false);
			_badges.Add(rectTransform);
		}
	}

	private void CreateGymLeaderItemCells()
	{
		int num = Mathf.Clamp(Mathf.CeilToInt((float)PlayerDefs.BadgeCount / 6f) * 6, 18, 102);
		for (int i = 0; i < num; i++)
		{
			RectTransform rectTransform = UI.Instantiate(string.Format("UI/BadgeCollection/{0}", "Badge"));
			BadgeUI component = rectTransform.GetComponent<BadgeUI>();
			rectTransform.SetParent(_scrollRect.content, false);
			float x = -487.5f + (float)(i % 6) * 195f;
			float y = -435f - (float)Mathf.FloorToInt(i / 6) * 195f;
			rectTransform.localPosition = new Vector3(x, y);
			int badgeID = PlayerDefs.GetBadgeID(i);
			component.SetUp((badgeID <= 0) ? null : ("GymLeader/" + badgeID));
			_badges.Add(rectTransform);
		}
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.BackEvent != null)
			{
				this.BackEvent();
			}
			break;
		case 1:
			if (this.TrophiesEvent != null)
			{
				this.TrophiesEvent();
			}
			break;
		}
	}
}
