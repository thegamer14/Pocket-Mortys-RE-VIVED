using UnityEngine;

public class MPNodeMoveModule : MonoBehaviour
{
	public const float kDefaultMovementSpeed = 4.5f;

	private MPNodeFollowModule _followingNodeModule;

	public bool isManualUpdate;

	private Vector3 _startPosition;

	private Vector3 _targetPosition;

	private float _elapsedTimeSinceMoveStarted;

	private float _movementSpeedMultiplier = 1f;

	public Coord coord { get; protected set; }

	public eDirection direction { get; protected set; }

	public bool isMoving { get; protected set; }

	public bool wasMoving { get; protected set; }

	public int collisionIgnoreLayers { get; set; }

	public void SetUp(Coord newCoord, eDirection direction)
	{
		collisionIgnoreLayers = -257;
		SetPosition(newCoord);
		SetDirection(direction);
	}

	private void Update()
	{
		if (!isManualUpdate)
		{
			UpdateMovement();
		}
	}

	public virtual void UpdateMovement()
	{
		wasMoving = isMoving;
		if (isMoving)
		{
			_elapsedTimeSinceMoveStarted += Time.deltaTime * 4.5f * _movementSpeedMultiplier;
			base.transform.position = Vector3.Lerp(_startPosition, _targetPosition, _elapsedTimeSinceMoveStarted);
			if (base.transform.position == _targetPosition)
			{
				isMoving = false;
			}
		}
		if (_followingNodeModule != null)
		{
			_followingNodeModule.UpdateMovement();
		}
	}

	public void SetPosition(Coord newCoord)
	{
		_targetPosition = Coord.GetPositionFromCoord(newCoord);
		_targetPosition = new Vector3(_targetPosition.x, MPNodeUtils.GetGroundCollisionValue(_targetPosition), _targetPosition.z);
		base.transform.position = _targetPosition;
		coord = newCoord;
	}

	public virtual void SetDirection(eDirection newDirection)
	{
		direction = newDirection;
		base.transform.localEulerAngles = new Vector3(0f, HelperMethods.GetAngleFromDirection(direction) - 90f, 0f);
	}

	public void SetDirection(Coord newCoord)
	{
		Coord coord = newCoord - this.coord;
		eDirection directionFromCoord = Coord.GetDirectionFromCoord(coord);
		SetDirection(directionFromCoord);
	}

	public void MoveToNextCoord(eDirection newDirection)
	{
		MoveToCoord(coord + Coord.GetCoordFromDirection(newDirection));
	}

	public void MoveToCoord(Coord newCoord, bool force = false)
	{
		if ((force || !isMoving) && !(newCoord == coord))
		{
			SetDirection(newCoord);
			SetNewTarget(newCoord);
			if (_followingNodeModule != null)
			{
				_followingNodeModule.MoveToCoord(coord);
			}
			coord = newCoord;
		}
	}

	private void SetNewTarget(Coord newCoord)
	{
		_targetPosition = Coord.GetPositionFromCoord(newCoord);
		_targetPosition.y = MPNodeUtils.GetGroundCollisionValue(_targetPosition);
		_elapsedTimeSinceMoveStarted = 0f;
		_startPosition = base.transform.position;
		isMoving = true;
	}

	public void SetMovementSpeedMultiplier(float value)
	{
		_movementSpeedMultiplier = value;
		if ((bool)_followingNodeModule)
		{
			_followingNodeModule.SetMovementSpeedMultiplier(value);
		}
	}

	public void SetFollowModule(MPNodeFollowModule followModule)
	{
		_followingNodeModule = followModule;
	}
}
