using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UIBase
{
	public delegate void PauseUIDelegate();

	public PauseUIDelegate PartyPressedEvent;

	public PauseUIDelegate MortydexPressedEvent;

	public PauseUIDelegate BagPressedEvent;

	public PauseUIDelegate SalesmanRickPressedEvent;

	public PauseUIDelegate QuestPressedEvent;

	public PauseUIDelegate SettingsPressedEvent;

	public PauseUIDelegate SocialPressedEvent;

	public PauseUIDelegate BadgesPressedEvent;

	public PauseUIDelegate BackPressedEvent;

	public PauseUIDelegate QuitPressedEvent;

	private const float _kNotificationScrollSpeed = 250f;

	private Text _notificationLabel;

	private RectTransform _notificationLabelTrans;

	private GameObject _notificationIconObj;

	protected float _notificationMaskWidth;

	protected override void LateAwake()
	{
		base.LateAwake();
		_notificationLabel = base.transform.Find("NotificationMask/NotificationLabel").GetComponent<Text>();
		_notificationLabelTrans = _notificationLabel.GetComponent<RectTransform>();
		_notificationIconObj = base.transform.Find("NotificationIcon").gameObject;
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
		SetNotificationTextForNotificationID(PlayerDefs.CurrentAlertNotificationID);
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

	public void SetNotificationTextForNotificationID(string id)
	{
		string text = null;
		if (!string.IsNullOrEmpty(id))
		{
			text = NotificationDefs.GetText(id);
		}
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

	public void QuestPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (QuestPressedEvent != null)
		{
			QuestPressedEvent();
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

	public void BadgesPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (BadgesPressedEvent != null)
		{
			BadgesPressedEvent();
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
