using System.Collections.Generic;
using UnityEngine;

public class NodeAIWanderModule : MonoBehaviour
{
	public float moveCountdownMin = 0.5f;

	public float moveCountdownMax = 3f;

	private NodeMoveModule _moveModule;

	private eNodeAIWanderState _state;

	private eDirection _erraticMovementIgnoreDirection;

	private bool _lockToDirection;

	private eDirection _lastDirection;

	private float _moveCountdown;

	private void Awake()
	{
		_moveModule = GetComponent<NodeMoveModule>();
	}

	public void SetUp(float moveCountdownMin = -1f, float moveCountdownMax = -1f, bool lockToDirection = false)
	{
		_moveModule = GetComponent<NodeMoveModule>();
		_state = eNodeAIWanderState.NORMAL;
		_lockToDirection = lockToDirection;
		if (moveCountdownMin > -1f)
		{
			this.moveCountdownMin = moveCountdownMin;
		}
		if (moveCountdownMin > -1f)
		{
			this.moveCountdownMax = moveCountdownMax;
		}
		_moveCountdown = Random.Range(0f, moveCountdownMax);
	}

	private void Update()
	{
		if (_state == eNodeAIWanderState.EXHAUSTED)
		{
			return;
		}
		_moveCountdown -= Time.deltaTime;
		if (_state == eNodeAIWanderState.NORMAL)
		{
			if (_moveCountdown < 0f)
			{
				if (_lockToDirection || Random.Range(0, 3) == 0)
				{
					ChangeDirection();
				}
				else
				{
					Move();
				}
				_moveCountdown = Random.Range(moveCountdownMin, moveCountdownMax);
			}
		}
		else if (_state == eNodeAIWanderState.ERRATIC && _moveCountdown < 0f)
		{
			Move();
			_moveCountdown = 0.25f;
		}
	}

	private void ChangeDirection()
	{
		_lastDirection = HelperMethods.GetRandomDirection(_lastDirection);
		_moveModule.ChangeDirection(_lastDirection);
	}

	private void Move(eDirection direction = eDirection.NONE)
	{
		if (direction == eDirection.NONE)
		{
			List<eDirection> list = new List<eDirection>();
			list.Add(eDirection.UP);
			list.Add(eDirection.DOWN);
			list.Add(eDirection.RIGHT);
			list.Add(eDirection.LEFT);
			List<eDirection> list2 = list;
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				if (_moveModule.IsCollidingInDirection(list2[num]))
				{
					list2.RemoveAt(num);
				}
			}
			if (_state == eNodeAIWanderState.ERRATIC && list2.Count > 1)
			{
				list2.Remove(_erraticMovementIgnoreDirection);
			}
			direction = list2.PickRandom();
		}
		if (direction != eDirection.NONE)
		{
			_moveModule.MoveToNextCoord(direction);
			_lastDirection = direction;
		}
	}

	public void SetStateNormal()
	{
		_state = eNodeAIWanderState.NORMAL;
		_moveCountdown = Random.Range(0f, moveCountdownMax);
		_erraticMovementIgnoreDirection = eDirection.NONE;
	}

	public void SetStateErratic(eDirection direction, float moveDelay)
	{
		_state = eNodeAIWanderState.ERRATIC;
		_moveCountdown = moveDelay;
		if (direction != eDirection.NONE)
		{
			_erraticMovementIgnoreDirection = direction;
		}
	}

	public void SetStateExhausted()
	{
		_state = eNodeAIWanderState.EXHAUSTED;
	}
}
