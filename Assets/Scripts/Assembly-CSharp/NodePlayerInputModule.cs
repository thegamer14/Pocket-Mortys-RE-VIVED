using UnityEngine;

public class NodePlayerInputModule : MonoBehaviour
{
	public delegate void PlayerNodeDelegate(AdvancedNode node);

	private float _kDirectionChangeSpeed = 0.13f;

	private NodeMoveModule _moveModule;

	private eDirection _direction;

	private bool _isChangingDirection;

	private float _changingDirectionCountdown;

	public event PlayerNodeDelegate InteractedEvent;

	public void SetUp()
	{
		_moveModule = GetComponent<NodeMoveModule>();
		_direction = eDirection.NONE;
	}

	private void Update()
	{
		UpdateChangingDirection();
		if (_direction == eDirection.NONE)
		{
			_isChangingDirection = false;
		}
		else
		{
			if (_moveModule.isMoving)
			{
				return;
			}
			if (_direction != _moveModule.direction)
			{
				_moveModule.ChangeDirection(_direction);
				if (!_isChangingDirection && !_moveModule.wasMoving)
				{
					_changingDirectionCountdown = _kDirectionChangeSpeed;
					_isChangingDirection = true;
				}
			}
			if (!_isChangingDirection && !_moveModule.IsCollidingInDirection(_direction))
			{
				_moveModule.MoveToNextCoord(_direction);
			}
		}
	}

	private void UpdateChangingDirection()
	{
		if (_isChangingDirection)
		{
			_changingDirectionCountdown -= Time.deltaTime;
			if (_changingDirectionCountdown <= 0f)
			{
				_isChangingDirection = false;
			}
		}
	}

	public void FaceBtnAPressed()
	{
		AdvancedNode collidingWithInteractionPoint = _moveModule.GetCollidingWithInteractionPoint();
		if (collidingWithInteractionPoint != null && !collidingWithInteractionPoint.IsMoving() && this.InteractedEvent != null)
		{
			this.InteractedEvent(collidingWithInteractionPoint);
		}
	}

	public void DPadPressed(eDirection direction)
	{
		_direction = direction;
	}
}
