using UnityEngine;

public class MPNodePlayerInputModule : MonoBehaviour
{
	public delegate void PlayerNodeDelegate(MPNode node);

	private float _kDirectionChangeSpeed = 0.13f;

	public float _kCollisionBreakLimit = 0.2f;

	private MPNodeMoveModule _moveModule;

	private eDirection _dpadDirection;

	private float _changingDirectionCountdown;

	private float collisionBreakTimer;

	public bool isChangingDirection { get; private set; }

	public event PlayerNodeDelegate InteractedEvent;

	public void SetUp()
	{
		_moveModule = GetComponent<MPNodeMoveModule>();
		_dpadDirection = eDirection.NONE;
	}

	private void Update()
	{
		UpdateChangingDirection();
		if (_dpadDirection == eDirection.NONE)
		{
			isChangingDirection = false;
			return;
		}
		if (!_moveModule.isMoving)
		{
			if (_dpadDirection != _moveModule.direction)
			{
				_moveModule.SetDirection(_dpadDirection);
				if (!isChangingDirection && !_moveModule.wasMoving)
				{
					_changingDirectionCountdown = _kDirectionChangeSpeed;
					isChangingDirection = true;
				}
			}
			if (!isChangingDirection)
			{
				if (MPNodeUtils.IsCollidingInDirection(base.transform.position, _dpadDirection, _moveModule.collisionIgnoreLayers) && collisionBreakTimer < _kCollisionBreakLimit)
				{
					if (MPNodeUtils.IsCollidingWithUser(base.transform.position, _dpadDirection, _moveModule.collisionIgnoreLayers))
					{
						collisionBreakTimer += Time.deltaTime;
					}
					return;
				}
				_moveModule.MoveToNextCoord(_dpadDirection);
			}
		}
		collisionBreakTimer = 0f;
	}

	private void UpdateChangingDirection()
	{
		if (isChangingDirection)
		{
			_changingDirectionCountdown -= Time.deltaTime;
			if (_changingDirectionCountdown <= 0f)
			{
				isChangingDirection = false;
			}
		}
	}

	public void FaceBtnAPressed()
	{
		if (this.InteractedEvent != null)
		{
			MPNode collidingWithInteractionPoint = MPNodeUtils.GetCollidingWithInteractionPoint(base.transform.position, _moveModule.direction);
			if (collidingWithInteractionPoint != null && collidingWithInteractionPoint.IsInteractable())
			{
				this.InteractedEvent(collidingWithInteractionPoint);
			}
		}
	}

	public void DPadPressed(eDirection direction)
	{
		_dpadDirection = direction;
	}
}
