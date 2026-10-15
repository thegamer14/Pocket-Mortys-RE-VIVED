using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VideoRewardUI : UIBase
{
	public delegate void UIVideoRewardBtnDelegate();

	public UIVideoRewardBtnDelegate QuitBtnEvent;

	public UIVideoRewardBtnDelegate WatchBtnEvent;

	private Text _coins;

	private MenuBase _mainMenu;

	private MenuDialogue _dialogue;

	private GameObject _waitingScreen;

	private GameObject _successScreen;

	private GameObject _errorScreen;

	private Text _waitingText;

	private Text _errorText;

	private Text _successText;

	private int _coinValue;

	private eVideoRewardUIState _state;

	private Tween _coinTween;

	protected override void LateAwake()
	{
		_coins = base.transform.FindChild("MenuBar/CoinBar/Count/Label").GetComponent<Text>();
		_mainMenu = base.transform.Find("MenuBar/MainMenu").GetComponent<MenuBase>();
		_dialogue = base.transform.Find("MenuBar/Dialogue").GetComponent<MenuDialogue>();
		_waitingScreen = base.transform.Find("Waiting").gameObject;
		_errorScreen = base.transform.Find("Error").gameObject;
		_successScreen = base.transform.Find("Success").gameObject;
		_waitingText = _waitingScreen.transform.Find("Text").GetComponent<Text>();
		_errorText = _errorScreen.transform.Find("Text").GetComponent<Text>();
		_successText = _successScreen.transform.Find("Text").GetComponent<Text>();
	}

	public override void SetLabels()
	{
		base.SetLabels();
		base.transform.FindChild("MenuBar/CoinBar/Count/Title").GetComponent<Text>().text = TextDefs.COINS_TITLE;
		_mainMenu.transform.Find("WatchBtn").GetComponentInChildren<Text>().text = TextDefs.VIDEO_WATCH_BTN;
		_waitingText.text = TextDefs.VIDEO_TV_WAITING;
		_errorText.text = TextDefs.VIDEO_TV_ERROR;
		_successText.text = TextDefs.VIDEO_TV_SUCCESS;
	}

	protected override void AddEvents()
	{
		_mainMenu.BtnPressedEvent += MainMenuBtnPressedHandler;
	}

	protected override void RemoveEvents()
	{
		_mainMenu.BtnPressedEvent -= MainMenuBtnPressedHandler;
	}

	public void SetCoins(int coins)
	{
		_coinValue = coins;
		_coins.text = _coinValue.ToString();
	}

	public void TweenCoins(int coins)
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

	public void SetState(eVideoRewardUIState state, int rewardAmount = 0)
	{
		switch (state)
		{
		case eVideoRewardUIState.WAITING_FOR_VIDEO:
			if (_state == eVideoRewardUIState.NONE)
			{
				_dialogue.SetText(TextDefs.VIDEO_WAITING_FOR_VIDEO_MESSAGE);
			}
			else
			{
				_dialogue.TweenText(TextDefs.VIDEO_WAITING_FOR_VIDEO_MESSAGE);
			}
			_mainMenu.SetBtnEnabled(0, false);
			_waitingScreen.SetActive(true);
			_errorScreen.SetActive(false);
			_successScreen.SetActive(false);
			break;
		case eVideoRewardUIState.WAITING_FOR_REWARD:
			_dialogue.SetText(TextDefs.VIDEO_WAITING_FOR_REWARD_MESSAGE);
			_mainMenu.SetBtnEnabled(0, false);
			_waitingScreen.SetActive(true);
			_errorScreen.SetActive(false);
			_successScreen.SetActive(false);
			break;
		case eVideoRewardUIState.DISPLAY_REWARD:
			if (rewardAmount > 0)
			{
				_dialogue.TweenText(string.Format(TextDefs.VIDEO_REWARD_MESSAGE, rewardAmount));
				_waitingScreen.SetActive(false);
				_errorScreen.SetActive(false);
				_successScreen.SetActive(true);
			}
			else
			{
				_dialogue.TweenText(TextDefs.VIDEO_NO_REWARD_MESSAGE);
				_waitingScreen.SetActive(false);
				_errorScreen.SetActive(true);
				_successScreen.SetActive(false);
			}
			_mainMenu.SetBtnEnabled(0, true);
			break;
		case eVideoRewardUIState.NO_VIDEO_AVAILABLE:
			_dialogue.TweenText(TextDefs.VIDEO_NO_VIDEO_MESSAGE);
			_mainMenu.SetBtnEnabled(0, false);
			_waitingScreen.SetActive(false);
			_errorScreen.SetActive(true);
			_successScreen.SetActive(false);
			break;
		}
		_state = state;
	}

	public void MainMenuBtnPressedHandler(int index)
	{
		switch (index)
		{
		case 0:
			if (WatchBtnEvent != null)
			{
				WatchBtnEvent();
			}
			break;
		case 1:
			if (QuitBtnEvent != null)
			{
				QuitBtnEvent();
			}
			break;
		}
	}
}
