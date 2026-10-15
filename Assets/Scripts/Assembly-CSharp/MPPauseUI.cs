using UnityEngine;
using UnityEngine.UI;

public class MPPauseUI : UIBase
{
	public delegate void PauseUIDelegate();

	public PauseUIDelegate PartyPressedEvent;

	public PauseUIDelegate MortydexPressedEvent;

	public PauseUIDelegate BagPressedEvent;

	public PauseUIDelegate SalesmanRickPressedEvent;

	public PauseUIDelegate FriendsPressedEvent;

	public PauseUIDelegate SettingsPressedEvent;

	public PauseUIDelegate SocialPressedEvent;

	public PauseUIDelegate ChallengePressedEvent;

	public PauseUIDelegate BackPressedEvent;

	public PauseUIDelegate QuitPressedEvent;

	private const float _kNotificationScrollSpeed = 250f;

	private Text _notificationLabel;

	private RectTransform _notificationLabelTrans;

	private GameObject _notificationIconObj;

	private GameObject _friendsBtnFlashObj;

	private GameObject _challengeBtnFlashObj;

	protected float _notificationMaskWidth;

	protected override void LateAwake()
	{
		base.LateAwake();
		_notificationLabel = base.transform.Find("NotificationMask/NotificationLabel").GetComponent<Text>();
		_notificationLabelTrans = _notificationLabel.GetComponent<RectTransform>();
		_notificationIconObj = base.transform.Find("NotificationIcon").gameObject;
		_friendsBtnFlashObj = base.transform.Find("FriendsBtn/Flash").gameObject;
		_challengeBtnFlashObj = base.transform.Find("ChallengesBtn/Flash").gameObject;
		SetPlatformSpecificButtons();
	}

	public void SetUp()
	{
		_notificationMaskWidth = ((RectTransform)_notificationLabelTrans.parent).rect.width;
		SetLabels();
	}

	private void SetPlatformSpecificButtons()
	{
		Sprite newSprite = Resources.Load<Sprite>(string.Format("UI/Sprites/Pause/{0}", "googlePlayBtn"));
		base.transform.FindChild("SocialBtn").GetComponent<LocalizedAssetImage>().ResetEnglishAsset(newSprite);
	}

	public override void SetLabels()
	{
		SetNotificationText(string.Format(MPTextDefs.MP_PAUSE_NOTIFICATION_DEFAULT, App.saveData.multiplayer.username));
	}

	private void LateUpdate()
	{
		ScrollNotificationText(Time.deltaTime);
	}

	private void ScrollNotificationText(float deltaTime)
	{
		if (!_notificationLabelTrans.gameObject.activeSelf || string.IsNullOrEmpty(_notificationLabel.text))
		{
			return;
		}
		float preferredWidth = _notificationLabel.preferredWidth;
		if (preferredWidth > 0f && preferredWidth < _notificationMaskWidth)
		{
			_notificationLabelTrans.anchoredPosition = new Vector2(0f, _notificationLabelTrans.anchoredPosition.y);
			return;
		}
		_notificationLabelTrans.anchoredPosition -= new Vector2(250f * deltaTime, 0f);
		if (_notificationLabelTrans.anchoredPosition.x + preferredWidth < 0f)
		{
			_notificationLabelTrans.anchoredPosition = new Vector2(_notificationMaskWidth, _notificationLabelTrans.anchoredPosition.y);
		}
	}

	public void SetNotificationText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			_notificationLabel.gameObject.SetActive(false);
			_notificationIconObj.SetActive(false);
			return;
		}
		_notificationLabel.gameObject.SetActive(true);
		_notificationIconObj.SetActive(true);
		_notificationLabel.text = text;
		_notificationLabelTrans.anchoredPosition = new Vector2(_notificationMaskWidth, _notificationLabelTrans.anchoredPosition.y);
	}

	public void TweenFriendBtnFlash(bool enable)
	{
		_friendsBtnFlashObj.SetActive(enable);
	}

	public void TweenChallengeBtnFlash(bool enable)
	{
		_challengeBtnFlashObj.SetActive(enable);
	}

	public void PartyPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (PartyPressedEvent != null)
		{
			PartyPressedEvent();
		}
	}

	public void MortydexPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (MortydexPressedEvent != null)
		{
			MortydexPressedEvent();
		}
	}

	public void BagPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (BagPressedEvent != null)
		{
			BagPressedEvent();
		}
	}

	public void SalesmanRickPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (SalesmanRickPressedEvent != null)
		{
			SalesmanRickPressedEvent();
		}
	}

	public void FriendsPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (FriendsPressedEvent != null)
		{
			FriendsPressedEvent();
		}
	}

	public void SettingsPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (SettingsPressedEvent != null)
		{
			SettingsPressedEvent();
		}
	}

	public void SocialPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (SocialPressedEvent != null)
		{
			SocialPressedEvent();
		}
	}

	public void ChallengePressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (ChallengePressedEvent != null)
		{
			ChallengePressedEvent();
		}
	}

	public void BackPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (BackPressedEvent != null)
		{
			BackPressedEvent();
		}
	}

	public void QuitPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (QuitPressedEvent != null)
		{
			QuitPressedEvent();
		}
	}
}
