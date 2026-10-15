using System.Collections;
using UnityEngine;

public class AdvancedNode : Node
{
	[HideInInspector]
	public eNodeType type;

	[HideInInspector]
	[SerializeField]
	protected NodeData _data;

	protected string _assetID;

	protected NodeMoveAnimatorModule _moveAnimatorModule;

	protected NodeAIWanderModule _aiWanderModule;

	protected NodeAIFollowModule _aiFollowModule;

	private Transform _interactionPoint;

	private eDirection _direction;

	private eDirection _preInteractDirection;

	protected string _sfxInteract;

	protected float _sfxInteractVolume = 1f;

	public virtual NodeData data
	{
		get
		{
			return _data;
		}
		private set
		{
			_data = value;
		}
	}

	public NodeMoveModule moveModule { get; private set; }

	public override void SetUp(NodeData data, eNodeInitialDirection initialDirection, bool applyRotation)
	{
		moveModule = GetComponent<NodeMoveModule>();
		_moveAnimatorModule = GetComponent<NodeMoveAnimatorModule>();
		_data = data;
		RefreshAssetID();
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetUp(_assetID);
		}
		if (moveModule != null)
		{
			moveModule.SetUp(Coord.GetCoordFromPosition(base.transform.position));
		}
		_interactionPoint = base.transform.FindChild("InteractionPoint");
		base.SetUp(data, initialDirection, applyRotation);
	}

	public void AddModuleAIWander(float moveCountdownMin = -1f, float moveCountdownMax = -1f, bool lockToDirection = false)
	{
		_aiWanderModule = base.gameObject.AddComponent<NodeAIWanderModule>();
		_aiWanderModule.SetUp(moveCountdownMin, moveCountdownMax, lockToDirection);
	}

	public void AddModuleAIFollow(NodeMoveModule followMoveModule)
	{
		Object.Destroy(GetComponent<BoxCollider>());
		_aiFollowModule = base.gameObject.AddComponent<NodeAIFollowModule>();
		_aiFollowModule.SetUp();
		followMoveModule.SetFollowModule(_aiFollowModule);
	}

	public override void SetDirection(eDirection value)
	{
		if (moveModule != null)
		{
			moveModule.ChangeDirection(value);
		}
		else
		{
			base.SetDirection(value);
		}
		_direction = value;
	}

	public eDirection GetDirection()
	{
		if (moveModule != null)
		{
			return moveModule.direction;
		}
		return _direction;
	}

	public virtual IEnumerator ForceMoveInDirectionFor(eDirection direction, int distance)
	{
		if (moveModule == null)
		{
			yield break;
		}
		for (int i = distance; i > 0; i--)
		{
			moveModule.MoveToNextCoord(direction);
			while (moveModule.isMoving)
			{
				yield return null;
			}
		}
	}

	public virtual bool IsMoving()
	{
		if (moveModule != null)
		{
			return moveModule.isMoving;
		}
		return false;
	}

	public void Refresh()
	{
		RefreshAssetID();
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetUpSprites(_assetID);
		}
		SetInitialDirection(base.initialDirection);
	}

	public override void SetApplyRotation(bool value)
	{
		if (moveModule != null)
		{
			moveModule.applyRotation = value;
		}
		base.SetApplyRotation(value);
	}

	protected virtual void RefreshAssetID()
	{
		if (Application.isPlaying)
		{
			_assetID = InteractionDefs.AssetID(data.id);
		}
		if (_assetID == null)
		{
			_assetID = data.id;
		}
	}

	public virtual void InteractionStarted(eDirection direction, bool isTrigger)
	{
		if (_aiWanderModule != null)
		{
			_aiWanderModule.enabled = false;
		}
		_preInteractDirection = GetDirection();
		if (moveModule != null)
		{
			moveModule.ChangeDirection(direction);
		}
	}

	public virtual void InteractionFinished()
	{
		if (_aiWanderModule != null)
		{
			_aiWanderModule.enabled = true;
		}
		if (moveModule != null && _preInteractDirection != eDirection.NONE)
		{
			moveModule.ChangeDirection(_preInteractDirection);
		}
		_preInteractDirection = eDirection.NONE;
	}

	public virtual void ActivateInteractionPoint(bool show)
	{
		if (_interactionPoint != null)
		{
			_interactionPoint.gameObject.SetActive(show);
		}
	}

	public virtual void InteractionSuccess()
	{
		if (!string.IsNullOrEmpty(_sfxInteract))
		{
			App.sound.PlaySFX(_sfxInteract, _sfxInteractVolume);
		}
	}
}
