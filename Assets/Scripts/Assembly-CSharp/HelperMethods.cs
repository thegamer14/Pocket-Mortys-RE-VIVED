using System.Collections.Generic;
using UnityEngine;

public class HelperMethods
{
	public static Vector3 GetVector3FromDirection(eDirection direction)
	{
		switch (direction)
		{
		default:
			return Vector3.zero;
		case eDirection.UP:
			return new Vector3(0f, 0f, 1f);
		case eDirection.DOWN:
			return new Vector3(0f, 0f, -1f);
		case eDirection.LEFT:
			return new Vector3(-1f, 0f, 0f);
		case eDirection.RIGHT:
			return new Vector3(1f, 0f, 0f);
		}
	}

	public static eDirection GetDirectionFromAngle(float angle)
	{
		switch (Mathf.FloorToInt((angle + 45f).DegAngleTo360() / 90f))
		{
		default:
			return eDirection.NONE;
		case 0:
			return eDirection.RIGHT;
		case 1:
			return eDirection.DOWN;
		case 2:
			return eDirection.LEFT;
		case 3:
			return eDirection.UP;
		}
	}

	public static float GetAngleFromDirection(eDirection direction)
	{
		switch (direction)
		{
		default:
			return 0f;
		case eDirection.UP:
			return 270f;
		case eDirection.DOWN:
			return 90f;
		case eDirection.LEFT:
			return 180f;
		case eDirection.RIGHT:
			return 0f;
		}
	}

	public static eDirection GetRandomDirection(eDirection excluding = eDirection.NONE)
	{
		List<eDirection> list = new List<eDirection>();
		list.Add(eDirection.UP);
		list.Add(eDirection.DOWN);
		list.Add(eDirection.RIGHT);
		list.Add(eDirection.LEFT);
		List<eDirection> list2 = list;
		list2.Remove(excluding);
		return list2.PickRandom();
	}

	public static eDirection GetOppositeDirection(eDirection direction)
	{
		switch (direction)
		{
		default:
			return eDirection.NONE;
		case eDirection.UP:
			return eDirection.DOWN;
		case eDirection.DOWN:
			return eDirection.UP;
		case eDirection.RIGHT:
			return eDirection.LEFT;
		case eDirection.LEFT:
			return eDirection.RIGHT;
		}
	}

	public static eDirection GetNextDirection(eDirection direction, bool clockwise = true)
	{
		switch (direction)
		{
		default:
			return eDirection.NONE;
		case eDirection.UP:
			return clockwise ? eDirection.RIGHT : eDirection.LEFT;
		case eDirection.DOWN:
			return (!clockwise) ? eDirection.RIGHT : eDirection.LEFT;
		case eDirection.RIGHT:
			return (!clockwise) ? eDirection.UP : eDirection.DOWN;
		case eDirection.LEFT:
			return (!clockwise) ? eDirection.DOWN : eDirection.UP;
		}
	}
}
