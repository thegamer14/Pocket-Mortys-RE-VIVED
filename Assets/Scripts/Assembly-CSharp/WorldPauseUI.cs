using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WorldPauseUI : MonoBehaviour
{
	public delegate void PasueUIDelegate();

	private const int _kOutPositionY = 350;

	private const float _kInOutDuration = 0.3f;

	private const int _kNotficationOutX = 963;

	private const int _kNotficationInX = 5;

	private RectTransform _rectTrans;

	private Button _mPadBtn;

	private GameObject _mPadBtnNotificationIconObj;

	private Button _portalGunBtn;

	private RectTransform _notificationMaskedTrans;

	private Text _notificationLabel;

	private GameObject _notificationObj;

	private Image _notificationIconImg;

	private float _startingPositionY;

	private Coroutine _notificationLoopCoroutine;

	private Coroutine _notificationCoroutine;

	public event PasueUIDelegate MPadPressedEvent;

	public event PasueUIDelegate PortalGunPressedEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_mPadBtn = base.transform.FindChild("mPadBtn").GetComponent<Button>();
		_mPadBtnNotificationIconObj = _mPadBtn.transform.FindChild("NotificationIcon").gameObject;
		_portalGunBtn = base.transform.FindChild("PortalGunBtn").GetComponent<Button>();
		_notificationObj = base.transform.Find("Notification").gameObject;
		_notificationMaskedTrans = base.transform.FindChild("Notification/Masked").GetComponent<RectTransform>();
		_notificationLabel = base.transform.FindChild("Notification/Masked/Text").GetComponent<Text>();
		_notificationIconImg = base.transform.FindChild("Notification/Masked/InfoIcon").GetComponent<Image>();
		_startingPositionY = base.transform.localPosition.y;
		_mPadBtn.enabled = false;
	}

	private void Start()
	{
		bool active = PlayerDefs.DefeatedCouncilMembers == 6 && WorldDefs.IsSegmented(SharedStatInfo.worldInfo.worldID);
		_portalGunBtn.gameObject.SetActive(active);
	}

	public void SetUp()
	{
		SetPositionOut();
		SetNotificationStartState();
		SetMPadBtnNotificationIconVisible(false);
		AddEvents(true);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			PlayerDefs.NewNotificationSetEvent += PlayerDefsNewNotificationSetHandler;
			App.states.BackButtonEvent += BackButtonHandler;
		}
		else
		{
			PlayerDefs.NewNotificationSetEvent -= PlayerDefsNewNotificationSetHandler;
			App.states.BackButtonEvent -= BackButtonHandler;
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	public void DisplayWaitingNotifications()
	{
		if (_notificationLoopCoroutine == null && _mPadBtn.enabled)
		{
			_notificationLoopCoroutine = StartCoroutine(DisplayWaitingNotificationsCo());
		}
	}

	public IEnumerator DisplayWaitingNotificationsCo()
	{
		while (PlayerDefs.notificationsWaitingToDisplay.Count > 0)
		{
			KeyValuePair<string, eNotificationType> waitingToDisplay = PlayerDefs.notificationsWaitingToDisplay[0];
			string text = NotificationDefs.GetText(waitingToDisplay.Key);
			PlayerDefs.notificationsWaitingToDisplay.RemoveAt(0);
			_notificationCoroutine = StartCoroutine(DisplayNotificationCo(waitingToDisplay.Value, text));
			yield return _notificationCoroutine;
		}
		KillNotificationCoroutine();
	}

	private IEnumerator DisplayNotificationCo(eNotificationType notificationType, string text)
	{
		_notificationObj.SetActive(true);
		_notificationLabel.text = text;
		_notificationIconImg.sprite = GetNotificationSpriteForType(notificationType);
		_notificationIconImg.SetNativeSize();
		App.sound.PlaySFX("Audio/SFX/Notification");
		yield return _notificationMaskedTrans.DOAnchorPos(new Vector2(5f, _notificationMaskedTrans.anchoredPosition.y), 0.7f).SetEase(Ease.OutQuint).WaitForCompletion();
		yield return new WaitForSeconds(4f);
		yield return _notificationMaskedTrans.DOAnchorPos(new Vector2(963f, _notificationMaskedTrans.anchoredPosition.y), 0.4f).SetEase(Ease.InQuint).WaitForCompletion();
		_notificationObj.SetActive(false);
	}

	private void KillNotificationCoroutine()
	{
		if (_notificationLoopCoroutine != null)
		{
			StopCoroutine(_notificationLoopCoroutine);
			_notificationLoopCoroutine = null;
			if (_notificationCoroutine != null)
			{
				StopCoroutine(_notificationCoroutine);
				_notificationCoroutine = null;
				_notificationMaskedTrans.DOKill();
				_notificationMaskedTrans.DOAnchorPos(new Vector2(963f, _notificationMaskedTrans.anchoredPosition.y), 0.4f).SetEase(Ease.InQuint);
			}
		}
	}

	private Sprite GetNotificationSpriteForType(eNotificationType type)
	{
		string arg = null;
		switch (type)
		{
		case eNotificationType.ALERT:
			arg = "Alert";
			break;
		case eNotificationType.INFO:
			arg = "Info";
			break;
		}
		return Resources.Load<Sprite>(string.Format("UI/World/{0}", arg));
	}

	private void SetNotificationStartState()
	{
		_notificationMaskedTrans.anchoredPosition = new Vector2(963f, _notificationMaskedTrans.anchoredPosition.y);
		_notificationObj.SetActive(false);
	}

	public void SetMPadBtnNotificationIconVisible(bool value)
	{
		_mPadBtnNotificationIconObj.SetActive(value);
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, _startingPositionY);
		_mPadBtn.enabled = true;
		DisplayWaitingNotifications();
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, 350f);
		_mPadBtn.enabled = false;
		KillNotificationCoroutine();
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, _startingPositionY), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
		_mPadBtn.enabled = true;
		DisplayWaitingNotifications();
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
	}

	public IEnumerator TweenOutCo()
	{
		_mPadBtn.enabled = false;
		KillNotificationCoroutine();
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, 350f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
	}

	public void MPadPressedHandler()
	{
		if (this.MPadPressedEvent != null)
		{
			this.MPadPressedEvent();
		}
	}

	public void PortalGunPressedHandler()
	{
		if (this.PortalGunPressedEvent != null)
		{
			this.PortalGunPressedEvent();
		}
	}

	private void BackButtonHandler()
	{
		if (_mPadBtn.enabled)
		{
			MPadPressedHandler();
		}
	}

	private void PlayerDefsNewNotificationSetHandler()
	{
		DisplayWaitingNotifications();
	}
}
