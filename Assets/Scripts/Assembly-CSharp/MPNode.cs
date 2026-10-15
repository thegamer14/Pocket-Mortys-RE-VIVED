using System.Collections;
using UnityEngine;

public class MPNode : MonoBehaviour
{
	public eMPNodeInteractionType interactionType;

	public eDirection initialDirection;

	public MPNodeMoveModule moveModule;

	protected MPNodeUdpPlacementModule _udpPlacementModule;

	protected MPNodeFollowModule _followModule;

	protected Transform _interactionPoint;

	public Vector3 touchPositionOffset = Vector3.zero;

	protected string _sfxInteract;

	protected float _sfxInteractVolume = 1f;

	public bool isForcedMoving { get; private set; }

	public virtual void SetUp()
	{
		_interactionPoint = base.transform.FindChild("InteractionPoint");
	}

	public virtual void OnDestroy()
	{
		if (_udpPlacementModule != null)
		{
			_udpPlacementModule.SnapFollowingNodesEvent -= UDPPlacementSnapFollowingNodesHandler;
		}
	}

	protected MPNodeMoveModule SetUpMoveModule()
	{
		moveModule = GetComponent<MPNodeMoveModule>();
		if (moveModule != null)
		{
			moveModule.SetUp(Coord.GetCoordFromPosition(base.transform.position), initialDirection);
		}
		return moveModule;
	}

	protected MPNodeCharacterMoveModule SetUpCharacterMoveModule(string assetId)
	{
		MPNodeCharacterMoveModule component = GetComponent<MPNodeCharacterMoveModule>();
		if (component != null)
		{
			component.SetUp(Coord.GetCoordFromPosition(base.transform.position), initialDirection, assetId);
			moveModule = component;
		}
		return component;
	}

	protected MPNodeUdpPlacementModule AddUdpPlacementModule()
	{
		_udpPlacementModule = base.gameObject.AddComponent<MPNodeUdpPlacementModule>();
		_udpPlacementModule.SetUp();
		_udpPlacementModule.SnapFollowingNodesEvent += UDPPlacementSnapFollowingNodesHandler;
		return _udpPlacementModule;
	}

	public MPNodeFollowModule AddModuleFollow(MPNodeMoveModule moveModuleToFollow)
	{
		Object.Destroy(GetComponent<BoxCollider>());
		_followModule = base.gameObject.AddComponent<MPNodeFollowModule>();
		_followModule.SetUp();
		moveModuleToFollow.SetFollowModule(_followModule);
		return _followModule;
	}

	protected virtual string GetAssetId()
	{
		return null;
	}

	public void SetDirection(eDirection direction)
	{
		if (moveModule != null)
		{
			moveModule.SetDirection(direction);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(initialDirection) - 90f, 0f);
		}
	}

	public eDirection GetDirection()
	{
		if (moveModule != null)
		{
			return moveModule.direction;
		}
		return initialDirection;
	}

	public virtual IEnumerator ForceMoveInDirectionFor(eDirection direction, int distance)
	{
		if (moveModule == null)
		{
			yield break;
		}
		isForcedMoving = true;
		for (int i = distance; i > 0; i--)
		{
			moveModule.MoveToNextCoord(direction);
			while (moveModule.isMoving)
			{
				yield return null;
			}
		}
		isForcedMoving = false;
	}

	public Coord GetCoord()
	{
		if (moveModule != null)
		{
			return moveModule.coord;
		}
		int x = Mathf.FloorToInt(base.transform.localPosition.x / 1f);
		int z = Mathf.FloorToInt(base.transform.localPosition.z / 1f);
		return new Coord(x, z);
	}

	public virtual void ManualGroundCollisionUpdate()
	{
		base.transform.position = new Vector3(base.transform.position.x, MPNodeUtils.GetGroundCollisionValue(base.transform.position), base.transform.position.z);
	}

	public void AddFollowingNode(MPNode nodeAttaching)
	{
		nodeAttaching.AddModuleFollow(moveModule);
	}

	public void EnableInteraction(bool enable)
	{
		_interactionPoint.gameObject.SetActive(enable);
	}

	public virtual bool IsInteractable()
	{
		if (_interactionPoint == null)
		{
			return false;
		}
		return _interactionPoint.gameObject.activeSelf;
	}

	public virtual void InteractionStarted(MPNode otherNode, eDirection direction)
	{
	}

	public virtual void InteractionFinished(bool isSuccess = true)
	{
		if (!string.IsNullOrEmpty(_sfxInteract))
		{
			App.sound.PlaySFX(_sfxInteract, _sfxInteractVolume);
		}
	}

	public virtual void UpdateWithUdpPlacementData(MPNodePlacementData placementData)
	{
		if (_udpPlacementModule != null)
		{
			_udpPlacementModule.ApplyPlacementDataToQueue(placementData);
		}
	}

	public virtual void SetEditorValues(eDirection initialDirection)
	{
		this.initialDirection = initialDirection;
		if (SetUpCharacterMoveModule(GetAssetId()) == null)
		{
			SetUpMoveModule();
		}
		SetDirection(initialDirection);
	}

	public virtual void UDPPlacementSnapFollowingNodesHandler(Coord[] coords)
	{
	}
}
