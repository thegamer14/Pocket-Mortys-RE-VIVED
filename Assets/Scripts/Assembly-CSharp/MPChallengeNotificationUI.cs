using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MPChallengeNotificationUI : MonoBehaviour
{
	public delegate void ResponseDelegate(MPBattleInviteData data, bool accept);

	public delegate void InfoDelegate(MPBattleInviteData data);

	private const int _kOutPositionY = -146;

	private const float _kInOutDuration = 0.3f;

	private RectTransform _rectTrans;

	private Button _infoButton;

	private Button _acceptButton;

	private Button _declineButton;

	private Text _dialogueText;

	private Text _usernameText;

	private Image _avatarIcon;

	private float _startingPositionY;

	public eMPChallengeNotificationUIState state { get; private set; }

	public MPBattleInviteData data { get; private set; }

	public event ResponseDelegate InviteResponseEvent;

	public event InfoDelegate InfoEvent;

	private void Awake()
	{
		_rectTrans = GetComponent<RectTransform>();
		_infoButton = base.transform.Find("InfoBtn").GetComponent<Button>();
		_acceptButton = base.transform.Find("AcceptBtn").GetComponent<Button>();
		_declineButton = base.transform.Find("DeclineBtn").GetComponent<Button>();
		_dialogueText = _infoButton.transform.FindChild("DialogueLabel").GetComponent<Text>();
		_usernameText = _infoButton.transform.FindChild("UsernameLabel").GetComponent<Text>();
		_avatarIcon = _infoButton.transform.FindChild("AvatarIcon").GetComponent<Image>();
		_startingPositionY = base.transform.localPosition.y;
		_acceptButton.enabled = false;
		_declineButton.enabled = false;
		_infoButton.enabled = false;
		state = eMPChallengeNotificationUIState.OUT;
		SetPositionOut();
	}

	public void SetUp(MPBattleInviteData data)
	{
		SetPositionOut();
		this.data = data;
		_usernameText.text = data.userData.username;
		_avatarIcon.sprite = Resources.Load<Sprite>(string.Format("World/NodeAssets/{0}/icon", data.userData.assetId));
		UpdateDialogueText();
	}

	private void Update()
	{
		UpdateDialogueText();
	}

	private void UpdateDialogueText()
	{
		if (data != null)
		{
			_dialogueText.text = string.Format(MPTextDefs.MP_BATTLE_INVITE_NOTIFICAION_DIAL, Mathf.CeilToInt(data.ttl));
		}
	}

	public void SetPositionIn()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, _startingPositionY);
		_acceptButton.enabled = true;
		_declineButton.enabled = true;
		_infoButton.enabled = true;
		state = eMPChallengeNotificationUIState.IN;
	}

	public void SetPositionOut()
	{
		_rectTrans.anchoredPosition = new Vector3(_rectTrans.anchoredPosition.x, -146f);
		_acceptButton.enabled = false;
		_declineButton.enabled = false;
		_infoButton.enabled = false;
		data = null;
		state = eMPChallengeNotificationUIState.OUT;
	}

	public void TweenIn()
	{
		StartCoroutine(TweenInCo());
		state = eMPChallengeNotificationUIState.IN;
	}

	public IEnumerator TweenInCo()
	{
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, _startingPositionY), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
		_acceptButton.enabled = true;
		_declineButton.enabled = true;
		_infoButton.enabled = true;
	}

	public void TweenOut()
	{
		StartCoroutine(TweenOutCo());
		data = null;
		state = eMPChallengeNotificationUIState.OUT;
	}

	public IEnumerator TweenOutCo()
	{
		_acceptButton.enabled = false;
		_declineButton.enabled = false;
		_infoButton.enabled = false;
		base.transform.DOKill();
		yield return _rectTrans.DOAnchorPos(new Vector2(0f, -146f), 0.3f).SetEase(Ease.Linear).WaitForCompletion();
	}

	public void AcceptPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.InviteResponseEvent != null)
		{
			this.InviteResponseEvent(data, true);
		}
	}

	public void DeclinePressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.InviteResponseEvent != null)
		{
			this.InviteResponseEvent(data, false);
		}
	}

	public void InfoPressedHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.InfoEvent != null)
		{
			this.InfoEvent(data);
		}
	}
}
