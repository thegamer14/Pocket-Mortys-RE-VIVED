using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TournamentEventSelectUI : UIBase
{
	public delegate void BagUIBtnDelegate();

	private const float _kScrollMenuPaddingTop = 85f;

	private const float _kScrollMenuPaddingBottom = 200f;

	private const float _kScrollMenuCellGap = -4f;

	private const float _kTweenDuration = 0.35f;

	private const int _kTitleInY = 0;

	private const int _kTitleOutY = 230;

	private const int _kAntagonistInX = -400;

	private const int _kAntagonistOutX = -1200;

	private const int _kScrollMenuInX = 240;

	private const int _kScrollMenuOutX = 1100;

	private RawImage _background;

	private RawImage _backgroundFade;

	private Image _title;

	private Image _antagonist;

	private MenuBase _mainMenu;

	private MenuBase _unlockMenu;

	private MenuBase _pageMenu;

	private MenuDialogue _dialogue;

	private ScrollMenu _scrollMenu;

	private TournamentEventCell _openCell;

	private string _currentTournamentID;

	private bool _isTimeStampLocked;

	private double _lastCountdown;

	private eTournamentEventSelectState _state;

	public event BagUIBtnDelegate EnterEvent;

	public event BagUIBtnDelegate UnlockEvent;

	public event BagUIBtnDelegate BackEvent;

	protected override void LateAwake()
	{
		_background = base.transform.FindChild("Background/Layer1").GetComponent<RawImage>();
		_backgroundFade = base.transform.FindChild("Background/LayerFade").GetComponent<RawImage>();
		_title = base.transform.FindChild("Title").GetComponent<Image>();
		_antagonist = base.transform.FindChild("Antagonist").GetComponent<Image>();
		_mainMenu = base.transform.FindChild("MenuBar/MainMenu").GetComponent<MenuBase>();
		_unlockMenu = base.transform.FindChild("MenuBar/UnlockMenu").GetComponent<MenuBase>();
		_pageMenu = base.transform.FindChild("MenuBar/PageMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.FindChild("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_scrollMenu = base.transform.FindChild("ScrollMenu").GetComponent<ScrollMenu>();
		_backgroundFade.gameObject.SetActive(false);
	}

	public void SetUp(string startingTournamentID, bool isTimeStampLocked, double countdown)
	{
		_isTimeStampLocked = isTimeStampLocked;
		_lastCountdown = countdown;
		_scrollMenu.SetUp(0, false, 85f, 200f, -4f);
		_state = eTournamentEventSelectState.ALL_LOCKED;
		SetTournamentComponents(startingTournamentID);
	}

	public override void SetLabels()
	{
		_mainMenu.SetBtnTitle(0, TextDefs.TOURNAMENT_EVENT_SELECT_BTN_ENTER);
		_unlockMenu.SetBtnTitle(0, TextDefs.TOURNAMENT_EVENT_SELECT_BTN_UNLOCK);
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
		_unlockMenu.BtnPressedEvent += UnlockMenuBtnPressedHandler;
		_pageMenu.BtnPressedEvent += PageMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
		_unlockMenu.BtnPressedEvent -= UnlockMenuBtnPressedHandler;
		_pageMenu.BtnPressedEvent -= PageMenuBtnPressedHandler;
	}

	public void SetStateAllLocked()
	{
		if (_state != eTournamentEventSelectState.ALL_LOCKED)
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_EVENT_SELECT_DIAL_ALL_LOCKED);
		}
		StartCoroutine(_mainMenu.TweenIn());
		_mainMenu.SetBtnEnabled(0, false);
		_state = eTournamentEventSelectState.ALL_LOCKED;
	}

	public void SetStateOpen(bool isTimeStampLocked)
	{
		if (!isTimeStampLocked)
		{
			if (_state != eTournamentEventSelectState.OPEN || _lastCountdown > 0.0)
			{
				_dialogue.TweenText(TextDefs.TOURNAMENT_EVENT_SELECT_DIAL_OPEN);
				StartCoroutine(_mainMenu.TweenIn());
				StartCoroutine(_unlockMenu.TweenOut());
				_mainMenu.SetBtnEnabled(0, true);
				if (_openCell != null)
				{
					_openCell.EnableCountdown(false);
				}
			}
		}
		else if (_state != eTournamentEventSelectState.OPEN || _lastCountdown <= 0.0)
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_EVENT_SELECT_DIAL_TIME_LOCKED);
			StartCoroutine(_unlockMenu.TweenIn());
			StartCoroutine(_mainMenu.TweenOut());
			if (_openCell != null)
			{
				_openCell.EnableCountdown(true);
				_openCell.SetCountdown(_lastCountdown);
			}
		}
		_state = eTournamentEventSelectState.OPEN;
	}

	public void SetStateAllComplete()
	{
		if (_state != eTournamentEventSelectState.ALL_COMPLETE)
		{
			_dialogue.TweenText(TextDefs.TOURNAMENT_EVENT_SELECT_DIAL_ALL_COMPLETE);
		}
		StartCoroutine(_mainMenu.TweenIn());
		StartCoroutine(_unlockMenu.TweenOut());
		_mainMenu.SetBtnEnabled(0, false);
		_state = eTournamentEventSelectState.ALL_COMPLETE;
	}

	private IEnumerator SetNewTournament(string nextTournamentID)
	{
		ActivateInput(false);
		StartCoroutine(_mainMenu.TweenOut(0.35f));
		StartCoroutine(_unlockMenu.TweenOut(0.35f));
		yield return StartCoroutine(TweenUIOut());
		SetTournamentComponents(nextTournamentID);
		StartCoroutine(FadeInNewBackground());
		yield return StartCoroutine(TweenUIIn());
		ActivateInput(true);
	}

	private void SetTournamentComponents(string nextTournamentID)
	{
		_currentTournamentID = nextTournamentID;
		string arg = TournamentDefs.TournamentAssetID(_currentTournamentID);
		_backgroundFade.texture = _background.texture;
		_backgroundFade.uvRect = _background.uvRect;
		_background.texture = Resources.Load<Texture>(string.Format("UI/TournamentEventSelect/Sprites/{0}/{1}", arg, "background"));
		_title.sprite = Resources.Load<Sprite>(string.Format("UI/TournamentEventSelect/Sprites/{0}/{1}", arg, string.Format("Title/{0}", LocalizationDefs.GetCurrentLanguageCode())));
		_antagonist.sprite = Resources.Load<Sprite>(string.Format("UI/TournamentEventSelect/Sprites/{0}/{1}", arg, "antagonist"));
		_title.SetNativeSize();
		_antagonist.SetNativeSize();
		_pageMenu.SetBtnVisible(0, TournamentDefs.GetPreviousTournamentID(_currentTournamentID) != null);
		_pageMenu.SetBtnVisible(1, TournamentDefs.GetNextTournamentID(_currentTournamentID) != null);
		switch (SetScrollMenu(_currentTournamentID))
		{
		case eTournamentEventSelectState.ALL_LOCKED:
			SetStateAllLocked();
			break;
		case eTournamentEventSelectState.OPEN:
			SetStateOpen(_isTimeStampLocked);
			break;
		case eTournamentEventSelectState.ALL_COMPLETE:
			SetStateAllComplete();
			break;
		}
	}

	private IEnumerator FadeInNewBackground()
	{
		_backgroundFade.gameObject.SetActive(true);
		yield return _backgroundFade.DOFade(0f, 0.35f).WaitForCompletion();
		_backgroundFade.color = new Color(_backgroundFade.color.r, _backgroundFade.color.g, _backgroundFade.color.b, 1f);
		_backgroundFade.gameObject.SetActive(false);
	}

	private eTournamentEventSelectState SetScrollMenu(string tournamentID)
	{
		eTournamentEventSelectState eTournamentEventSelectState2 = eTournamentEventSelectState.ALL_LOCKED;
		_scrollMenu.RemoveAllCells();
		string[] array = TournamentDefs.TournamentEventIDs(tournamentID);
		for (int i = 0; i < array.Length; i++)
		{
			TournamentEventData eventData = TournamentDefs.GetEventData(tournamentID, array[i]);
			eTournamentEventCellState eTournamentEventCellState2 = eTournamentEventCellState.LOCKED;
			if (eventData != null)
			{
				if (!eventData.isComplete)
				{
					eTournamentEventCellState2 = eTournamentEventCellState.OPEN;
					eTournamentEventSelectState2 = eTournamentEventSelectState.OPEN;
				}
				else
				{
					eTournamentEventCellState2 = eTournamentEventCellState.COMPLETE;
					if (i == array.Length - 1 && eTournamentEventSelectState2 == eTournamentEventSelectState.ALL_LOCKED)
					{
						eTournamentEventSelectState2 = eTournamentEventSelectState.ALL_COMPLETE;
					}
				}
			}
			TournamentEventCell openCell = AddEventCell(array[i], i, eTournamentEventCellState2);
			if (eTournamentEventCellState2 == eTournamentEventCellState.OPEN)
			{
				_openCell = openCell;
			}
		}
		if (_openCell != null)
		{
			_scrollMenu.CenterToCell(_openCell);
		}
		return eTournamentEventSelectState2;
	}

	private TournamentEventCell AddEventCell(string eventID, int index, eTournamentEventCellState cellState)
	{
		string arg = ((cellState != eTournamentEventCellState.OPEN) ? "TournamentEventCell" : "TournamentNextEventCell");
		RectTransform rectTransform = UI.Instantiate(string.Format("UI/Menu/Cells/{0}", arg));
		TournamentEventCell component = rectTransform.GetComponent<TournamentEventCell>();
		component.SetUp(eventID, index, cellState);
		_scrollMenu.AddCell(component);
		return component;
	}

	public void SetTimestampLocked(bool isTimeStampLocked)
	{
		if (isTimeStampLocked && !_isTimeStampLocked)
		{
			SetStateOpen(isTimeStampLocked);
		}
		else if (!isTimeStampLocked && _isTimeStampLocked)
		{
			SetStateOpen(isTimeStampLocked);
		}
		_isTimeStampLocked = isTimeStampLocked;
	}

	public void SetCountdown(double countdown)
	{
		if (countdown == 0.0 && _lastCountdown > 0.0)
		{
			SetTimestampLocked(false);
		}
		else if (_openCell != null)
		{
			_openCell.SetCountdown(countdown);
		}
		_lastCountdown = countdown;
	}

	private IEnumerator TweenUIIn()
	{
		_title.rectTransform.DOAnchorPos(new Vector2(_title.rectTransform.anchoredPosition.x, 0f), 0.35f);
		_antagonist.rectTransform.DOAnchorPos(new Vector2(-400f, _antagonist.rectTransform.anchoredPosition.y), 0.35f);
		_scrollMenu.rectTransform.DOAnchorPos(new Vector2(240f, _scrollMenu.rectTransform.anchoredPosition.y), 0.35f);
		yield return new WaitForSeconds(0.45f);
	}

	private IEnumerator TweenUIOut()
	{
		_title.rectTransform.DOAnchorPos(new Vector2(_title.rectTransform.anchoredPosition.x, 230f), 0.35f);
		_antagonist.rectTransform.DOAnchorPos(new Vector2(-1200f, _antagonist.rectTransform.anchoredPosition.y), 0.35f);
		_scrollMenu.rectTransform.DOAnchorPos(new Vector2(1100f, _scrollMenu.rectTransform.anchoredPosition.y), 0.35f);
		yield return new WaitForSeconds(0.45f);
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.EnterEvent != null)
			{
				this.EnterEvent();
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

	public void UnlockMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (this.UnlockEvent != null)
			{
				this.UnlockEvent();
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

	public void PageMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			StartCoroutine(SetNewTournament(TournamentDefs.GetPreviousTournamentID(_currentTournamentID)));
			break;
		case 1:
			StartCoroutine(SetNewTournament(TournamentDefs.GetNextTournamentID(_currentTournamentID)));
			break;
		}
	}
}
