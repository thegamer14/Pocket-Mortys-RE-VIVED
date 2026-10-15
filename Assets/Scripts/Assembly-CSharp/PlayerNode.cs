using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerNode : AdvancedNode
{
	public delegate void PlayerNodeDelegate(AdvancedNode node);

	public delegate void PlayerNodeInteractStartDelegate(AdvancedNode node, bool isTrigger);

	[HideInInspector]
	public bool allowRandomVOClips;

	[HideInInspector]
	public bool allowRandomVOClipsForSegmentedWorld;

	protected NodePlayerInputModule _playerInputModule;

	protected List<MortyNode> _followingMorties;

	private AudioSource _randomVOClip;

	public bool isForcedWalking { get; private set; }

	public event PlayerNodeDelegate InteractAvailableEvent;

	public event PlayerNodeDelegate InteractUnavailableEvent;

	public event PlayerNodeDelegate InteractEndEvent;

	public event PlayerNodeInteractStartDelegate InteractStartEvent;

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		base.SetUp(data, initialDirection, applyRotation);
		base.moveModule.collisionIgnoreLayers = -1281;
		_followingMorties = new List<MortyNode>();
		_playerInputModule = GetComponent<NodePlayerInputModule>();
		_playerInputModule.SetUp();
		_playerInputModule.InteractedEvent += InputInteractedHandler;
		InvokeRepeating("PlayRandomVO", 5f, 60f);
	}

	private void OnDestroy()
	{
		CancelInvoke("PlayRandomVO");
		_playerInputModule.InteractedEvent -= InputInteractedHandler;
	}

	private void Update()
	{
		AdvancedNode collidingWithInteractionPoint = base.moveModule.GetCollidingWithInteractionPoint();
		if (collidingWithInteractionPoint != null)
		{
			if (this.InteractAvailableEvent != null)
			{
				this.InteractAvailableEvent(collidingWithInteractionPoint);
			}
		}
		else if (this.InteractUnavailableEvent != null)
		{
			this.InteractUnavailableEvent(collidingWithInteractionPoint);
		}
	}

	public void RefreshAsset()
	{
		data.id = PlayerDefs.AssetID;
		RefreshAssetID();
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetUp(_assetID, _moveAnimatorModule.direction);
		}
	}

	public void DPadChanged(eDirection direction)
	{
		_playerInputModule.DPadPressed(direction);
	}

	public void FaceBtnAPressed()
	{
		_playerInputModule.FaceBtnAPressed();
	}

	public void SetUpMorties(Object resource, Vector3[] positions = null, eDirection[] directions = null)
	{
		if (positions == null)
		{
			positions = GetMortyPositions();
		}
		if (directions == null)
		{
			directions = GetMortyDirections();
		}
		RemoveMorties();
		for (int i = 0; i < PlayerDefs.CurrentPartyCount; i++)
		{
			if (i < positions.Length && i < directions.Length)
			{
				MortyNode mortyNode = AddMorty(resource, PlayerDefs.CurrentPartyGetMorty(i), positions[i], false);
				mortyNode.SetDirection(directions[i]);
				continue;
			}
			eDirection direction = ((_followingMorties.Count <= 0) ? GetDirection() : _followingMorties[_followingMorties.Count - 1].GetDirection());
			Vector3 position = HelperMethods.GetVector3FromDirection(HelperMethods.GetOppositeDirection(direction)) * 1f;
			MortyNode mortyNode2 = AddMorty(resource, PlayerDefs.CurrentPartyGetMorty(i), position, true);
			mortyNode2.SetDirection(direction);
		}
	}

	public IEnumerator SetUpMortiesWithAnimation(Object resource, eDirection direction)
	{
		_playerInputModule.enabled = false;
		RemoveMorties();
		for (int i = 0; i < PlayerDefs.CurrentPartyCount; i++)
		{
			AddMorty(position: HelperMethods.GetVector3FromDirection(HelperMethods.GetOppositeDirection(direction)) * 1f, resource: resource, data: PlayerDefs.CurrentPartyGetMorty(i), applyPositionToFollow: true);
			base.moveModule.MoveToNextCoord(direction);
			while (base.moveModule.isMoving)
			{
				yield return null;
			}
		}
		_playerInputModule.enabled = true;
	}

	public MortyNode AddMorty(Object resource, MortyData data, Vector3 position, bool applyPositionToFollow)
	{
		NodeMoveModule nodeMoveModule = base.moveModule;
		if (_followingMorties.Count > 0)
		{
			nodeMoveModule = _followingMorties[_followingMorties.Count - 1].moveModule;
		}
		GameObject gameObject = Object.Instantiate(resource) as GameObject;
		MortyNode component = gameObject.GetComponent<MortyNode>();
		component.name = resource.name;
		component.transform.SetParent(base.transform.parent, false);
		component.transform.position = (applyPositionToFollow ? (position + nodeMoveModule.transform.position) : position);
		component.SetUp(data, eNodeInitialDirection.FIXED_DOWN, false);
		component.AddModuleAIFollow(nodeMoveModule);
		component.ActivateInteractionPoint(false);
		_followingMorties.Add(component);
		return component;
	}

	private void RemoveMorties()
	{
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			Object.Destroy(_followingMorties[i].gameObject);
		}
		_followingMorties.Clear();
	}

	public IEnumerator RemoveMortiesWithAnimation()
	{
		_playerInputModule.enabled = false;
		base.gameObject.SetActive(false);
		float angleToPlayer = Mathf.Atan2(_followingMorties[0].transform.position.z - base.transform.position.z, base.transform.position.x - _followingMorties[0].transform.position.x) * 57.29578f;
		eDirection directionToPortal = HelperMethods.GetDirectionFromAngle(angleToPlayer);
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			_followingMorties[i].moveModule.MoveToNextCoord(directionToPortal);
			while (_followingMorties[i].moveModule.isMoving)
			{
				yield return null;
			}
			_followingMorties[i].gameObject.SetActive(false);
		}
	}

	public void AlignMortysAroundPlayer()
	{
		eDirection direction = GetDirection();
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			Coord coord = base.moveModule.coord;
			switch (i)
			{
			case 2:
				coord += Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(direction));
				break;
			case 3:
				coord += Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(direction)) + Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(direction));
				break;
			case 1:
				coord += Coord.GetCoordFromDirection(HelperMethods.GetOppositeDirection(direction)) + Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(direction, false));
				break;
			case 4:
				coord += Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(direction));
				break;
			case 0:
				coord += Coord.GetCoordFromDirection(HelperMethods.GetNextDirection(direction, false));
				break;
			}
			_followingMorties[i].moveModule.SetUp(coord);
			_followingMorties[i].SetDirection(direction);
		}
	}

	public Vector3[] GetMortyPositions()
	{
		Vector3[] array = new Vector3[_followingMorties.Count];
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			array[i] = _followingMorties[i].transform.position;
		}
		return array;
	}

	public eDirection[] GetMortyDirections()
	{
		eDirection[] array = new eDirection[_followingMorties.Count];
		for (int i = 0; i < _followingMorties.Count; i++)
		{
			array[i] = _followingMorties[i].GetDirection();
		}
		return array;
	}

	public MortyNode[] GetFollowingMorties()
	{
		if (_followingMorties != null)
		{
			return _followingMorties.ToArray();
		}
		return null;
	}

	public override IEnumerator ForceMoveInDirectionFor(eDirection direction, int distance)
	{
		isForcedWalking = true;
		_playerInputModule.enabled = false;
		yield return StartCoroutine(_003CForceMoveInDirectionFor_003E__BaseCallProxy0(direction, distance));
		_playerInputModule.enabled = true;
		isForcedWalking = false;
	}

	public void SuccessfulNodeInteraction(eNodeType type)
	{
		if (type == eNodeType.ITEM || type == eNodeType.ITEM_CRATE)
		{
			if (_randomVOClip != null && _randomVOClip.isPlaying)
			{
				_randomVOClip.Stop();
			}
			VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Happy/{0}/{1}", PlayerDefs.AvatarCategory, 0.35f, 0f, 0.2f);
		}
	}

	private void PlayRandomVO()
	{
		if (allowRandomVOClips && base.moveModule.isMoving)
		{
			if (allowRandomVOClipsForSegmentedWorld && Random.value > 0.5f)
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Random In Segmented/{0}/{1}", PlayerDefs.AvatarCategory, 0.35f, 0f, 0.2f);
			}
			else
			{
				VoiceOverDefs.PlayAvatarRandom("Audio/VO/Trainer - Random/{0}/{1}", PlayerDefs.AvatarCategory, 0.35f, 0f, 0.2f);
			}
		}
	}

	public void InputInteractedHandler(AdvancedNode node)
	{
		if (this.InteractStartEvent != null)
		{
			this.InteractStartEvent(node, false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		AdvancedNode component = other.transform.parent.GetComponent<AdvancedNode>();
		if (this.InteractStartEvent != null && component != null)
		{
			this.InteractStartEvent(component, true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		AdvancedNode component = other.transform.parent.GetComponent<AdvancedNode>();
		if (this.InteractEndEvent != null && component != null)
		{
			this.InteractEndEvent(component);
		}
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003CForceMoveInDirectionFor_003E__BaseCallProxy0(eDirection direction, int distance)
	{
		return base.ForceMoveInDirectionFor(direction, distance);
	}
}
