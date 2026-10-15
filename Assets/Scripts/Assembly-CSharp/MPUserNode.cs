using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class MPUserNode : MPOpponentNode
{
	public delegate void UserNodeDelegate();

	public delegate void UserNodeInteractRequestDelegate(MPNode node);

	protected MPNodePlayerInputModule _playerInputModule;

	protected MPNodePlayerBroadcastModule _playerBroadcastModule;

	private MPUserNodeLevel _level;

	private TextMesh _username;

	private GameObject _emoteObj;

	private SpriteAnimation _emoteAnim;

	private Sequence _emoteTweenSeq;

	public MPUserData userData
	{
		get
		{
			return (MPUserData)base.data;
		}
	}

	public event UserNodeDelegate InteractAvailableEvent;

	public event UserNodeDelegate InteractUnavailableEvent;

	public event UserNodeInteractRequestDelegate InteractStartRequestEvent;

	public event UserNodeInteractRequestDelegate InteractEndRequestEvent;

	public override void Awake()
	{
		base.Awake();
		_level = base.transform.FindChild("Level").GetComponent<MPUserNodeLevel>();
		_username = base.transform.FindChild("NameLabel/Text").GetComponent<TextMesh>();
		_emoteObj = base.transform.FindChild("Emote").gameObject;
		_emoteAnim = base.transform.FindChild("Emote/Icon").GetComponent<SpriteAnimation>();
	}

	public override void SetUp()
	{
		base.SetUp();
		RefreshUsername();
		RefreshLevel();
		HideEmote();
	}

	public void SetUpWithData(MPUserData userData, eDirection direction)
	{
		base.SetUpWithData(userData, direction);
		_characterMoveModule.SetZDepth(0.248f);
		if (base.data.type == eMPOpponentType.PLAYER)
		{
			SetUpAsPlayer();
		}
		else
		{
			AddUdpPlacementModule();
		}
	}

	private void SetUpAsPlayer()
	{
		moveModule.collisionIgnoreLayers = -1281;
		EnableInteraction(false);
		_username.gameObject.SetActive(false);
		_level.gameObject.SetActive(false);
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		AddPlayerInputModule();
		AddPlayerBroadcastModule();
		_characterMoveModule.SetZDepth(0.246f);
		InvokeRepeating("PlayRandomVO", 5f, 60f);
	}

	public override void OnDestroy()
	{
		if (base.data.type == eMPOpponentType.PLAYER)
		{
			CancelInvoke("PlayRandomVO");
			_playerInputModule.InteractedEvent -= InputInteractedHandler;
			base.data.placementData = new MPNodePlacementData(GetCoord(), GetDirection(), GetMortyCoords(), GetMortyDirections());
		}
		base.OnDestroy();
	}

	private void Update()
	{
		if (base.data.type != eMPOpponentType.PLAYER)
		{
			return;
		}
		MPNode collidingWithInteractionPoint = MPNodeUtils.GetCollidingWithInteractionPoint(base.transform.position, GetDirection());
		if (collidingWithInteractionPoint != null && collidingWithInteractionPoint.IsInteractable())
		{
			if (this.InteractAvailableEvent != null)
			{
				this.InteractAvailableEvent();
			}
		}
		else if (this.InteractUnavailableEvent != null)
		{
			this.InteractUnavailableEvent();
		}
	}

	private void AddPlayerInputModule()
	{
		_playerInputModule = base.gameObject.AddComponent<MPNodePlayerInputModule>();
		_playerInputModule.SetUp();
		_playerInputModule.InteractedEvent += InputInteractedHandler;
	}

	private void AddPlayerBroadcastModule()
	{
		_playerBroadcastModule = base.gameObject.AddComponent<MPNodePlayerBroadcastModule>();
	}

	public void DPadChanged(eDirection direction)
	{
		_playerInputModule.DPadPressed(direction);
	}

	public void FaceBtnAPressed()
	{
		_playerInputModule.FaceBtnAPressed();
	}

	public override IEnumerator ForceMoveInDirectionFor(eDirection direction, int distance)
	{
		_playerInputModule.enabled = false;
		yield return StartCoroutine(_003CForceMoveInDirectionFor_003E__BaseCallProxy0(direction, distance));
		_playerInputModule.enabled = true;
	}

	public void RefreshUsername()
	{
		_username.text = userData.username;
	}

	public void RefreshLevel()
	{
		_level.SetLevel(userData.level);
	}

	public void DisplayEmote(string emoteId)
	{
		HideEmote();
		_username.gameObject.SetActive(false);
		_level.gameObject.SetActive(false);
		_emoteObj.SetActive(true);
		_emoteAnim.sprites = Resources.LoadAll<Sprite>(string.Format("UI/MPWorld/Emotes/Sprites/{0}", emoteId));
		_emoteAnim.Play();
		_emoteObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		_emoteTweenSeq = DOTween.Sequence();
		_emoteTweenSeq.Append(_emoteObj.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
		_emoteTweenSeq.AppendInterval(MPEmoteDefs.kWorldEmoteDuration);
		_emoteTweenSeq.OnComplete(delegate
		{
			HideEmote(true);
		});
	}

	public void HideEmote(bool tweenOut = false)
	{
		if (_emoteTweenSeq != null)
		{
			_emoteTweenSeq.Kill();
			_emoteTweenSeq = null;
		}
		_emoteAnim.Stop();
		if (tweenOut)
		{
			_emoteTweenSeq = DOTween.Sequence();
			_emoteTweenSeq.Append(_emoteObj.transform.DOScale(0.1f, 0.2f).SetEase(Ease.InBack));
			_emoteTweenSeq.OnComplete(delegate
			{
				_emoteObj.SetActive(false);
			});
		}
		else
		{
			_emoteObj.SetActive(false);
		}
		if (base.data.type != eMPOpponentType.PLAYER)
		{
			_username.gameObject.SetActive(true);
			_level.gameObject.SetActive(true);
		}
	}

	private void PlayRandomVO()
	{
		if (moveModule.isMoving)
		{
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Random/{0}/{1}", userData.avatarCategory, 0.35f, 0f, 0.2f);
		}
	}

	public void InputInteractedHandler(MPNode node)
	{
		if (this.InteractStartRequestEvent != null)
		{
			this.InteractStartRequestEvent(node);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		MPNode component = other.transform.parent.GetComponent<MPNode>();
		if (this.InteractStartRequestEvent != null && component != null && component.IsInteractable())
		{
			this.InteractStartRequestEvent(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MPNode component = other.transform.parent.GetComponent<MPNode>();
		if (this.InteractEndRequestEvent != null && component != null)
		{
			this.InteractEndRequestEvent(component);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CForceMoveInDirectionFor_003E__BaseCallProxy0(eDirection direction, int distance)
	{
		return base.ForceMoveInDirectionFor(direction, distance);
	}
}
