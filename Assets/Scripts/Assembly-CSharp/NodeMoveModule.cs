using UnityEngine;

public class NodeMoveModule : MonoBehaviour
{
	public float movementSpeed = 4.5f;

	private const float _kCollisionYPos = 10f;

	private const float _kCollisionYDistance = 20f;

	public const int KInteractionLayer = 256;

	public const int KSegmentLayer = 512;

	public const int KBlockersLayer = 1024;

	private NodeAIFollowModule _aiFollowModule;

	private NodeMoveAnimatorModule _moveAnimatorModule;

	public bool applyRotation;

	public bool isMoving;

	public bool wasMoving;

	private float _elapsedTimeSinceMoveStarted;

	private Vector3 _startPosition;

	private Vector3 _targetPosition;

	public int collisionIgnoreLayers;

	public Coord coord { get; private set; }

	public eDirection direction { get; private set; }

	public void SetUp(Coord newCoord)
	{
		_moveAnimatorModule = GetComponent<NodeMoveAnimatorModule>();
		collisionIgnoreLayers = -257;
		SetCoord(newCoord);
		base.transform.position = new Vector3(base.transform.position.x, GetGroundCollisionValue(), base.transform.position.z);
	}

	public void SetFollowModule(NodeAIFollowModule aiFollowModule)
	{
		_aiFollowModule = aiFollowModule;
	}

	private void Update()
	{
		wasMoving = isMoving;
		if (isMoving)
		{
			UpdateMove();
		}
	}

	private void UpdateMove()
	{
		_elapsedTimeSinceMoveStarted += Time.deltaTime * movementSpeed;
		base.transform.position = Vector3.Lerp(_startPosition, _targetPosition, _elapsedTimeSinceMoveStarted);
		if (base.transform.position == _targetPosition)
		{
			base.transform.position = _targetPosition;
			isMoving = false;
			if (_moveAnimatorModule != null)
			{
				_moveAnimatorModule.SetMoving(false);
			}
		}
	}

	protected virtual void SetCoord(Coord newCoord)
	{
		if (Application.isPlaying)
		{
			base.transform.position = Coord.GetPositionFromCoord(newCoord);
		}
		_targetPosition = base.transform.position;
		coord = newCoord;
	}

	public void ChangeDirection(Coord newCoord)
	{
		Coord coord = newCoord - this.coord;
		eDirection directionFromCoord = Coord.GetDirectionFromCoord(coord);
		ChangeDirection(directionFromCoord);
	}

	public void ChangeDirection(eDirection newDirection)
	{
		direction = newDirection;
		if (applyRotation)
		{
			base.transform.eulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(newDirection) - 90f, 0f);
		}
		else
		{
			base.transform.eulerAngles = Vector3.zero;
		}
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetDirection(newDirection);
		}
	}

	public void MoveToNextCoord(eDirection newDirection)
	{
		MoveToCoord(coord + Coord.GetCoordFromDirection(newDirection));
	}

	public void MoveToCoord(Coord newCoord, bool force = false)
	{
		if (force || !isMoving)
		{
			ChangeDirection(newCoord);
			SetNewTarget(newCoord);
			UpdateMove();
			if (_aiFollowModule != null)
			{
				_aiFollowModule.MoveToCoord(coord);
			}
			coord = newCoord;
		}
	}

	private void SetNewTarget(Coord newCoord)
	{
		_targetPosition = new Vector3((float)newCoord.x * 1f + 0.5f, base.transform.position.y, (float)newCoord.z * 1f + 0.5f);
		_targetPosition.y = GetGroundCollisionValue();
		_elapsedTimeSinceMoveStarted = 0f;
		_startPosition = base.transform.position;
		isMoving = true;
		if (_moveAnimatorModule != null)
		{
			_moveAnimatorModule.SetMoving(true);
		}
	}

	public bool IsCollidingInDirection(eDirection colDirection)
	{
		Vector3 vector = Coord.GetCoordFromDirection(colDirection).ToVector3();
		Vector3 origin = base.transform.position + new Vector3(0f, 0.9f, 0f);
		if (Physics.Raycast(origin, vector, 1f, collisionIgnoreLayers))
		{
			return true;
		}
		Vector3 origin2 = new Vector3(origin.x + vector.x, 10f, origin.z + vector.z);
		if (!Physics.Raycast(origin2, Vector3.down, 20f, 512))
		{
			return true;
		}
		return false;
	}

	public AdvancedNode GetCollidingWithInteractionPoint()
	{
		Vector3 vector = Coord.GetCoordFromDirection(direction).ToVector3();
		Vector3 origin = base.transform.position + new Vector3(0f, 0.9f, 0f);
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, vector, out hitInfo, 1f, 256) && !hitInfo.collider.isTrigger)
		{
			return hitInfo.transform.parent.gameObject.GetComponent<AdvancedNode>();
		}
		return null;
	}

	private float GetGroundCollisionValue()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(new Vector3(_targetPosition.x, 10f, _targetPosition.z), Vector3.down, out hitInfo, 20f, 512))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	public int GetCollidingDistanceInDirection(eDirection colDirection, int distance)
	{
		Vector3 vector = Coord.GetCoordFromDirection(colDirection).ToVector3();
		Vector3 origin = base.transform.position + new Vector3(0f, 0.9f, 0f);
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, vector, out hitInfo, (float)distance * 1f, collisionIgnoreLayers))
		{
			return Mathf.FloorToInt(Vector3.Distance(hitInfo.point, base.transform.position) / 1f);
		}
		return -1;
	}
}
