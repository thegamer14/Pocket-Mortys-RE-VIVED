using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPUserBoxUI : MonoBehaviour
{
	public delegate void MPUserBoxUIDelegate();

	public delegate void MPUserBoxUIWithIniviteDelegate(MPBattleInviteData inviteData = null);

	public delegate void MPUserBoxUIBattleDelegate(string userId);

	private const int _kInPositionY = 0;

	private const int _kOutPositionY = -800;

	private const float _kInOutDuration = 0.25f;

	private const float _kTextScrollSpeed = 0.01f;

	private const float _kWaitingWheelSpeed = -400f;

	private const float _kCharacterNameBoxSize = 170f;

	private RectTransform _rectTrans;

	private Text _label;

	private Image _character;

	private RectTransform _characterNameBarBackground;

	private Text _characterNameLabel;

	private Transform _waitingWheelTrans;

	private Image[] _mortyIcons;

	private Button _battleButton;

	private Text _battleButtonText;

	private Button _addFriendButton;

	private Text _addFriendButtonText;

	private Button _battleInviteReplyButton;

	private Text _battleInviteReplyButtonText;

	private Button _closeButton;

	private Tween _textTween;

	private bool _battlRequestEnabled;

	private bool _addFriendEnabled;

	public eUserBoxState state { get; private set; }

	public MPBattleInviteData battleInviteData { get; private set; }

	public MPUserData userData { get; private set; }

	public event MPUserBoxUIDelegate StartedEvent;

	public event MPUserBoxUIDelegate FinishedEvent;

	public event MPUserBoxUIWithIniviteDelegate CloseEvent;

	public event MPUserBoxUIWithIniviteDelegate AcceptInviteEvent;

	public event MPUserBoxUIBattleDelegate BattleInviteEvent;

	public event MPUserBoxUIBattleDelegate AddFriendEvent;

	private void Awake()
	{
		state = eUserBoxState.NONE;
		_rectTrans = GetComponent<RectTransform>();
		_label = base.transform.FindChild("Box/Label").GetComponent<Text>();
		_battleButton = base.transform.FindChild("Buttons/BattleBtn").GetComponent<Button>();
		_battleButtonText = _battleButton.transform.FindChild("Label").GetComponent<Text>();
		_addFriendButton = base.transform.FindChild("Buttons/AddFriendBtn").GetComponent<Button>();
		_addFriendButtonText = _addFriendButton.transform.FindChild("Label").GetComponent<Text>();
		_battleInviteReplyButton = base.transform.FindChild("Buttons/AcceptBtn").GetComponent<Button>();
		_battleInviteReplyButtonText = _battleInviteReplyButton.transform.FindChild("Label").GetComponent<Text>();
		_closeButton = base.transform.FindChild("Buttons/CloseBtn").GetComponent<Button>();
		_waitingWheelTrans = base.transform.FindChild("Box/WaitingImg").transform;
		_mortyIcons = base.transform.FindChild("MortyIcons").GetComponentsInChildren<Image>();
		Transform transform = base.transform.FindChild("Character");
		if (transform != null)
		{
			_character = transform.GetComponent<Image>();
		}
		Transform transform2 = base.transform.FindChild("NameBar");
		if (transform2 != null)
		{
			_characterNameBarBackground = transform2.FindChild("Background").GetComponent<RectTransform>();
			_characterNameLabel = transform2.FindChild("Label").GetComponent<Text>();
		}
	}

	private void Update()
	{
		if (_waitingWheelTrans.gameObject.activeSelf)
		{
			_waitingWheelTrans.Rotate(0f, 0f, -400f * Time.deltaTime);
		}
	}

	public void SetStateDefault(MPUserData userData)
	{
		state = eUserBoxState.DEFAULT;
		battleInviteData = null;
		this.userData = userData;
		_battlRequestEnabled = true;
		_addFriendEnabled = !App.net.friend.IsFriendUserAlreadyAFriend(userData.netId);
		SetCharacterName(userData.username);
		StartCoroutine(SetCharacterImage(userData.assetId));
		SetMorties(userData.GetAllPartyMortyDatas());
		_waitingWheelTrans.gameObject.SetActive(false);
		_battleButton.gameObject.SetActive(true);
		_addFriendButton.gameObject.SetActive(true);
		_battleInviteReplyButton.gameObject.SetActive(false);
		_battleButton.interactable = userData.state != eMPOpponentState.BATTLE && _battlRequestEnabled;
		_addFriendButton.interactable = _addFriendEnabled;
		_closeButton.interactable = true;
		_battleButtonText.text = MPTextDefs.MP_USER_BOX_BATTLE_BTN;
		_addFriendButtonText.text = ((!_addFriendEnabled) ? MPTextDefs.MP_USER_BOX_ADD_FRIEND_DONE_BTN : MPTextDefs.MP_USER_BOX_ADD_FRIEND_BTN);
		SetText(MPTextDefs.MP_USER_BOX_DIALOGUE);
		StartCoroutine(TweenIn());
		if (this.StartedEvent != null)
		{
			this.StartedEvent();
		}
	}

	public void SetStateBattleInviteReceived(MPBattleInviteData inviteData)
	{
		state = eUserBoxState.BATTLE_INVITE_RECEIVED;
		battleInviteData = inviteData;
		userData = null;
		SetCharacterName(inviteData.userData.username);
		StartCoroutine(SetCharacterImage(inviteData.userData.assetId));
		SetMorties(inviteData.userData.GetAllPartyMortyDatas());
		_waitingWheelTrans.gameObject.SetActive(false);
		_battleButton.gameObject.SetActive(false);
		_addFriendButton.gameObject.SetActive(false);
		_battleInviteReplyButton.gameObject.SetActive(true);
		_battleInviteReplyButton.interactable = true;
		_closeButton.interactable = true;
		_battleInviteReplyButtonText.text = MPTextDefs.MP_USER_BOX_ACCEPT_BTN;
		StartCoroutine(TweenIn());
		if (this.StartedEvent != null)
		{
			this.StartedEvent();
		}
	}

	public void SetStateBattleInviteDeclined()
	{
		state = eUserBoxState.BATTLE_INVITE_DECLINED;
		_battleButton.gameObject.SetActive(true);
		_addFriendButton.gameObject.SetActive(true);
		_battleInviteReplyButton.gameObject.SetActive(false);
		_waitingWheelTrans.gameObject.SetActive(false);
		_battleButton.interactable = false;
		_addFriendButton.interactable = _addFriendEnabled;
		_closeButton.interactable = true;
		_battleButtonText.text = MPTextDefs.MP_USER_BOX_DECLINED_BTN;
	}

	public void SetStateBattleInviteFailed()
	{
		state = eUserBoxState.BATTLE_INVITE_FAILED;
		_battleButton.gameObject.SetActive(true);
		_addFriendButton.gameObject.SetActive(true);
		_battleInviteReplyButton.gameObject.SetActive(false);
		_waitingWheelTrans.gameObject.SetActive(false);
		_battleButton.interactable = false;
		_addFriendButton.interactable = _addFriendEnabled;
		_closeButton.interactable = true;
		_battleButtonText.text = MPTextDefs.MP_USER_BOX_FAILED_BTN;
	}

	public void SetStateAddFriendComplete(eFriendSendRequestError error)
	{
		state = eUserBoxState.ADD_FRIEND_LIMIT;
		_addFriendEnabled = false;
		_battleButton.gameObject.SetActive(true);
		_addFriendButton.gameObject.SetActive(true);
		_battleInviteReplyButton.gameObject.SetActive(false);
		_waitingWheelTrans.gameObject.SetActive(false);
		_battleButton.interactable = userData.state != eMPOpponentState.BATTLE && _battlRequestEnabled;
		_addFriendButton.interactable = false;
		_closeButton.interactable = true;
		switch (error)
		{
		case eFriendSendRequestError.NONE:
			_addFriendButtonText.text = MPTextDefs.MP_USER_BOX_ADD_FRIEND_DONE_BTN;
			break;
		default:
			StartCoroutine(TweenText(MPTextDefs.MP_USER_BOX_ADD_FRIEND_FAILED_DIALOGUE));
			break;
		case eFriendSendRequestError.FRIEND_DUPLICATE:
			StartCoroutine(TweenText(MPTextDefs.MP_USER_BOX_ADD_FRIEND_ALREADY_DONE_DIALOGUE));
			break;
		case eFriendSendRequestError.FRIEND_LIMIT_SELF:
			StartCoroutine(TweenText(MPTextDefs.MP_USER_BOX_ADD_FRIEND_LIMIT_MINE_DIALOGUE));
			break;
		case eFriendSendRequestError.FRIEND_LIMIT_OTHER:
			StartCoroutine(TweenText(MPTextDefs.MP_USER_BOX_ADD_FRIEND_LIMIT_OTHER_DIALOGUE));
			break;
		}
	}

	public void SetStateWaiting()
	{
		state = eUserBoxState.WAITING;
		_waitingWheelTrans.gameObject.SetActive(true);
		_battleButton.interactable = false;
		_addFriendButton.interactable = false;
		_battleInviteReplyButton.interactable = false;
		_closeButton.interactable = true;
	}

	public void RefreshUserStateChanged()
	{
		_battleButton.interactable = userData.state != eMPOpponentState.BATTLE && _battlRequestEnabled;
	}

	public void RefreshUserLeft()
	{
		_battlRequestEnabled = false;
		_battleButton.interactable = false;
		_battleButtonText.text = MPTextDefs.MP_USER_BOX_LEFT_BTN;
	}

	public void SetText(string text)
	{
		_label.text = text;
	}

	public IEnumerator TweenText(string text)
	{
		SetText(string.Empty);
		if (_textTween != null)
		{
			_textTween.Kill();
		}
		_textTween = ShortcutExtensions46.DOText(duration: (float)text.Length * 0.01f, target: _label, endValue: text);
		_textTween.SetEase(Ease.Linear);
		yield return _textTween.WaitForCompletion();
	}

	private IEnumerator SetCharacterImage(string assetID)
	{
		if (!(_character == null))
		{
			_character.sprite = null;
			_character.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetID, "frontFlipped"));
			if (_character.sprite == null)
			{
				_character.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/Battle/{1}", assetID, "front"));
			}
			_character.SetNativeSize();
			yield return new WaitForEndOfFrame();
			_character.gameObject.SetActive(_character.sprite != null);
		}
	}

	public void SetCharacterName(string name)
	{
		if (!(_characterNameLabel == null))
		{
			base.gameObject.SetActive(true);
			StartCoroutine(SetCharacterNameCo(name));
		}
	}

	private IEnumerator SetCharacterNameCo(string name)
	{
		_characterNameLabel.text = name;
		yield return new WaitForEndOfFrame();
		float width = _characterNameLabel.rectTransform.sizeDelta.x;
		_characterNameBarBackground.sizeDelta = new Vector2(width + 170f, _characterNameBarBackground.sizeDelta.y);
	}

	private void SetMorties(List<MPOpponentMortyData> morties)
	{
		for (int i = 0; i < _mortyIcons.Length; i++)
		{
			if (i < morties.Count)
			{
				_mortyIcons[i].sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", MPMortyDefs.AssetID(morties[i].id)));
			}
			else
			{
				_mortyIcons[i].gameObject.SetActive(false);
			}
		}
	}

	public void SetPositionIn()
	{
		base.gameObject.SetActive(true);
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, 0f);
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector2(_rectTrans.anchoredPosition.x, -800f);
		base.gameObject.SetActive(false);
		userData = null;
		battleInviteData = null;
	}

	public IEnumerator TweenIn()
	{
		base.gameObject.SetActive(true);
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, 0f), 0.25f);
		yield return tween.WaitForCompletion();
	}

	public IEnumerator TweenOut()
	{
		state = eUserBoxState.NONE;
		userData = null;
		battleInviteData = null;
		_battleButton.interactable = false;
		_addFriendButton.interactable = false;
		_battleInviteReplyButton.interactable = false;
		Tween tween = _rectTrans.DOAnchorPos(new Vector2(_rectTrans.anchoredPosition.x, -800f), 0.25f);
		yield return tween.WaitForCompletion();
		base.gameObject.SetActive(false);
		if (this.FinishedEvent != null)
		{
			this.FinishedEvent();
		}
	}

	public void BattlePressedHandler()
	{
		_battlRequestEnabled = false;
		if (this.BattleInviteEvent != null)
		{
			this.BattleInviteEvent(userData.netId);
		}
	}

	public void AddFriendPressedHandler()
	{
		if (this.AddFriendEvent != null)
		{
			this.AddFriendEvent(userData.netId);
		}
	}

	public void AcceptPressedHandler()
	{
		if (this.AcceptInviteEvent != null)
		{
			this.AcceptInviteEvent(battleInviteData);
		}
	}

	public void CloseButtonPressedHandler()
	{
		if (this.CloseEvent != null)
		{
			this.CloseEvent(battleInviteData);
		}
	}
}
