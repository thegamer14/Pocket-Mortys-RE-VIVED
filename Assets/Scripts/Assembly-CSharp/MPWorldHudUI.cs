using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPWorldHudUI : MonoBehaviour
{
	public delegate void UIDelegate();

	private const int _kOutPositionY = 350;

	private const float _kInOutDuration = 0.3f;

	private RectTransform _rectTrans;

	private CanvasGroup _canvasGroup;

	private MPWorldHudBtn _pauseBtn;

	private MPWorldHudBtn _mapBtn;

	private MPWorldHudBtn _emoteBtn;

	private MPPlayerLevelBarUI _playerLevelBar;

	private Text _usernameTxt;

	public event UIDelegate PauseBtnPressedEvent;

	public event UIDelegate MapBtnPressedEvent;

	public event UIDelegate EmoteBtnPressedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_canvasGroup = GetComponent<CanvasGroup>();
		_pauseBtn = base.transform.FindChild("Pause").GetComponent<MPWorldHudBtn>();
		_mapBtn = base.transform.FindChild("Map").GetComponent<MPWorldHudBtn>();
		_emoteBtn = base.transform.FindChild("Emote").GetComponent<MPWorldHudBtn>();
		_playerLevelBar = base.transform.FindChild("LevelBar").GetComponent<MPPlayerLevelBarUI>();
		_usernameTxt = base.transform.FindChild("LevelBar/Username").GetComponent<Text>();
	}

	public void SetUp()
	{
		_pauseBtn.SetUp();
		_mapBtn.SetUp();
		_emoteBtn.SetUp();
		_playerLevelBar.SetUp();
		SetPositionOut();
		_pauseBtn.TweenShake(App.net.battleInvite.friendInvitePending);
		_usernameTxt.text = App.net.player.data.username;
		AddEvents(true);
	}

	public void RefreshFromCache()
	{
		_playerLevelBar.RefreshAllValues();
	}

	public void AddEvents(bool value)
	{
		if (value)
		{
			_pauseBtn.PressedEvent += PauseBtnPressedHandler;
			_mapBtn.PressedEvent += MapBtnPressedHandler;
			_emoteBtn.PressedEvent += EmoteBtnPressedHandler;
		}
		else
		{
			_pauseBtn.PressedEvent -= PauseBtnPressedHandler;
			_mapBtn.PressedEvent -= MapBtnPressedHandler;
			_emoteBtn.PressedEvent -= EmoteBtnPressedHandler;
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	public void RefreshForBattleInvites(bool friendInvitePending)
	{
		_pauseBtn.TweenShake(friendInvitePending);
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, 0f);
		_canvasGroup.interactable = true;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, 350f);
		_canvasGroup.interactable = false;
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, 0f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
		_canvasGroup.interactable = true;
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		_canvasGroup.interactable = false;
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, 350f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
	}

	private void PauseBtnPressedHandler()
	{
		if (this.PauseBtnPressedEvent != null)
		{
			this.PauseBtnPressedEvent();
		}
	}

	private void MapBtnPressedHandler()
	{
		if (this.MapBtnPressedEvent != null)
		{
			this.MapBtnPressedEvent();
		}
	}

	private void EmoteBtnPressedHandler()
	{
		if (this.EmoteBtnPressedEvent != null)
		{
			this.EmoteBtnPressedEvent();
		}
	}
}
