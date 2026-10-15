using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPNodeUdpPlacementModule : MonoBehaviour
{
	public delegate void PlacementDelegate(Coord[] coords);

	private float _kMovementSpeedModifierPerCoord = 0.2f;

	private float _kMaxMovementSpeedModifier = 3f;

	private const int _kMaxCatchupCoords = 7;

	private const float _kMoveFromStandstillDelay = 0.3f;

	private MPNodeMoveModule _moveModule;

	private Queue<Coord> _coordsQueue = new Queue<Coord>();

	private Coord[] _lastCoordHistory;

	private eDirection _lastDirection;

	private Coroutine _moveFromStandstillCo;

	private long _lastPlacementTime;

	public event PlacementDelegate SnapFollowingNodesEvent;

	public void SetUp()
	{
		_moveModule = GetComponent<MPNodeMoveModule>();
	}

	public void ApplyPlacementDataToQueue(MPNodePlacementData placementData)
	{
		long ticks = DateTime.Now.Ticks;
		DebugLog("ApplyPlacementDataToQueue() time since last: " + string.Format("{0:N2}", new TimeSpan(ticks - _lastPlacementTime).TotalMilliseconds));
		_lastPlacementTime = ticks;
		_lastDirection = placementData.direction;
		if (placementData.coord == _moveModule.coord)
		{
			_coordsQueue.Clear();
			return;
		}
		if (_moveModule.coord.IsEdgeAdjacent(placementData.coord))
		{
			_coordsQueue.Clear();
			_coordsQueue.Enqueue(placementData.coord);
			return;
		}
		Queue<Coord> queue = TryJoinQueueWithHistory(placementData.coord, placementData.coordHistory);
		if (queue != null && queue.Count > 0)
		{
			_coordsQueue = queue;
			ValidateMoveQueue();
		}
		else
		{
			DebugLog("No valid path found");
			_coordsQueue.Clear();
			_coordsQueue.Enqueue(placementData.coord);
		}
		if (!_coordsQueue.Peek().IsEdgeAdjacent(_moveModule.coord))
		{
			DebugLog("Path can't be made, snapping");
			_coordsQueue.Clear();
			_coordsQueue.Enqueue(placementData.coord);
			_lastCoordHistory = placementData.coordHistory;
		}
	}

	private Queue<Coord> TryJoinQueueWithHistory(Coord newCoord, Coord[] coordHistory)
	{
		List<Coord> list = new List<Coord>(_coordsQueue.ToArray());
		list.Insert(0, _moveModule.coord);
		List<Coord> list2 = new List<Coord>(coordHistory);
		list2.Insert(0, newCoord);
		bool flag = false;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < list2.Count; i++)
		{
			if (flag)
			{
				break;
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (flag)
				{
					break;
				}
				if (list2[i] == list[j] || list2[i].IsEdgeAdjacent(list[j]))
				{
					flag = true;
					num = j;
					num2 = i;
				}
			}
		}
		if (flag)
		{
			if (num + 1 < list.Count)
			{
				list.RemoveRange(num + 1, list.Count - (num + 1));
			}
			list.RemoveAt(0);
			if (num2 + 1 < list2.Count)
			{
				list2.RemoveRange(num2 + 1, list2.Count - (num2 + 1));
			}
			list2.Reverse();
			if (list.Count > 0 && list2.Count > 0 && list[list.Count - 1] == list2[0])
			{
				list2.RemoveAt(0);
			}
			list.AddRange(list2);
			if (list.Count > 0 && list[list.Count - 1] == _moveModule.coord)
			{
				list.RemoveAt(list.Count - 1);
			}
			return new Queue<Coord>(list);
		}
		return null;
	}

	private void Update()
	{
		if (!_moveModule.isMoving && _coordsQueue.Count > 0)
		{
			if (_moveFromStandstillCo == null)
			{
				if (!_moveModule.wasMoving)
				{
					_moveFromStandstillCo = StartCoroutine(MoveToNextCoordFromStandstillCo());
				}
				else
				{
					MoveToNextCoord();
				}
			}
		}
		else if (!_moveModule.isMoving && _lastDirection != eDirection.NONE && _moveModule.direction != _lastDirection)
		{
			_moveModule.SetDirection(_lastDirection);
		}
	}

	private void MoveToNextCoord()
	{
		if (_coordsQueue.Count == 0)
		{
			return;
		}
		Coord coord = _coordsQueue.Dequeue();
		if (!coord.IsEdgeAdjacent(_moveModule.coord))
		{
			_moveModule.SetPosition(coord);
			_moveModule.SetDirection(_lastDirection);
			if (this.SnapFollowingNodesEvent != null)
			{
				this.SnapFollowingNodesEvent(_lastCoordHistory);
			}
		}
		else
		{
			_moveModule.MoveToCoord(coord);
		}
		UpdateMoveSpeedMultiplier();
	}

	private IEnumerator MoveToNextCoordFromStandstillCo()
	{
		DebugLog("MoveToCoordFromStandstillCo()");
		yield return new WaitForSeconds(0.3f);
		_moveFromStandstillCo = null;
		MoveToNextCoord();
	}

	private void UpdateMoveSpeedMultiplier()
	{
		_moveModule.SetMovementSpeedMultiplier(Mathf.Min(_kMaxMovementSpeedModifier, 1f + (float)_coordsQueue.Count * _kMovementSpeedModifierPerCoord));
	}

	private void ValidateMoveQueue()
	{
		List<Coord> list = new List<Coord>(_coordsQueue);
		if (list[0] == _moveModule.coord)
		{
			list.RemoveAt(0);
		}
		for (int num = list.Count - 2; num >= 0; num--)
		{
			if (list[num] == list[num + 1])
			{
				list.RemoveAt(num);
			}
			else
			{
				if (!list[num].IsEdgeAdjacent(list[num + 1]))
				{
					list.RemoveRange(0, num + 1);
					break;
				}
				if (list.Count - num >= 7)
				{
					list.RemoveRange(0, num);
					break;
				}
			}
		}
		_coordsQueue = new Queue<Coord>(list);
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_PLACEMENT_MODULE)
		{
		}
	}

	private string DebugCoordArrayToSring(Coord[] array)
	{
		string text = "[";
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = text;
			text = text2 + "(" + array[i].x + "," + array[i].z + ")";
			if (i < array.Length - 1)
			{
				text += ", ";
			}
		}
		return text + "]";
	}
}
