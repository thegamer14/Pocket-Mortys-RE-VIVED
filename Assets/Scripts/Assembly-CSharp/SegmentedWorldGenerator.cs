using System;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedWorldGenerator
{
	private int _width;

	private int _depth;

	private bool _includeEdges;

	private bool[,] _joints;

	private Coord _spawnCoord;

	private Coord _gymCoord;

	private List<Coord> _segmentCoordsJoined;

	private List<Coord> _segmentCoordsMarkedToJoin;

	private SegmentedSegmentInfo[,] _segmentInfos;

	public SegmentedSegmentInfo[,] Generate(int width, int depth, bool includeEdges)
	{
		_width = width;
		_depth = depth;
		_includeEdges = includeEdges;
		_joints = new bool[_width * 2 - 1, _depth];
		_spawnCoord = new Coord(-1);
		_gymCoord = new Coord(-1);
		_segmentCoordsJoined = new List<Coord>();
		_segmentCoordsMarkedToJoin = new List<Coord>();
		CreateSegmentInfoArray();
		DefineSpawnCoord();
		DefineGymCoord();
		CreateJoints();
		CreateSegmentInfos();
		return _segmentInfos;
	}

	private void CreateJoints()
	{
		Coord coord;
		GetCoordEmptySegmentInfo(out coord);
		_segmentCoordsMarkedToJoin.Add(coord);
		while (_segmentCoordsMarkedToJoin.Count > 0)
		{
			Coord coord2 = _segmentCoordsMarkedToJoin.PickRandom();
			List<Coord> doneNeighbourCoord = GetDoneNeighbourCoord(coord2);
			if (doneNeighbourCoord.Count > 0)
			{
				Coord coord3 = doneNeighbourCoord.PickRandom();
				JoinCoords(coord2, coord3);
			}
			List<Coord> untouchedNeighbourCoords = GetUntouchedNeighbourCoords(coord2);
			_segmentCoordsMarkedToJoin.AddRange(untouchedNeighbourCoords);
			_segmentCoordsJoined.Add(coord2);
			_segmentCoordsMarkedToJoin.Remove(coord2);
		}
	}

	private void JoinCoords(Coord coord1, Coord coord2)
	{
		if (coord1.x > coord2.x)
		{
			SetJointFor(coord1, eDirection.LEFT, true);
		}
		else if (coord1.x < coord2.x)
		{
			SetJointFor(coord1, eDirection.RIGHT, true);
		}
		if (coord1.z > coord2.z)
		{
			SetJointFor(coord1, eDirection.UP, true);
		}
		else if (coord1.z < coord2.z)
		{
			SetJointFor(coord1, eDirection.DOWN, true);
		}
	}

	private void SetJointFor(Coord coord, eDirection direction, bool value)
	{
		Coord coordOffsetForJoint = GetCoordOffsetForJoint(direction);
		int num = coord.x * 2 + coordOffsetForJoint.x;
		int num2 = coord.z + coordOffsetForJoint.z;
		if (num > -1 && num < _joints.GetLength(0) && num2 > -1 && num2 < _joints.GetLength(1))
		{
			_joints[num, num2] = value;
		}
	}

	private bool GetJointFor(Coord coord, eDirection direction)
	{
		Coord coordOffsetForJoint = GetCoordOffsetForJoint(direction);
		int num = coord.x * 2 + coordOffsetForJoint.x;
		int num2 = coord.z + coordOffsetForJoint.z;
		if (num > -1 && num < _joints.GetLength(0) && num2 > -1 && num2 < _joints.GetLength(1))
		{
			return _joints[num, num2];
		}
		return false;
	}

	private bool[] GetAllJointsFor(Coord coord)
	{
		return new bool[4]
		{
			GetJointFor(coord, eDirection.RIGHT),
			GetJointFor(coord, eDirection.DOWN),
			GetJointFor(coord, eDirection.LEFT),
			GetJointFor(coord, eDirection.UP)
		};
	}

	private Coord GetCoordOffsetForJoint(eDirection direction)
	{
		switch (direction)
		{
		default:
			return Coord.zero;
		case eDirection.RIGHT:
			return new Coord(1, 0);
		case eDirection.LEFT:
			return new Coord(-1, 0);
		case eDirection.UP:
			return new Coord(0, -1);
		}
	}

	private void CreateSegmentInfoArray()
	{
		int num = _width + (_includeEdges ? 2 : 0);
		int num2 = _depth + (_includeEdges ? 2 : 0);
		_segmentInfos = new SegmentedSegmentInfo[num, num2];
		if (_includeEdges)
		{
			GenerateEdgeSegmentInfos();
		}
	}

	private void GenerateEdgeSegmentInfos()
	{
		for (int i = 0; i < _segmentInfos.GetLength(0); i++)
		{
			for (int j = 0; j < _segmentInfos.GetLength(1); j++)
			{
				eSegmentJointType eSegmentJointType2 = eSegmentJointType.NONE;
				int num = 0;
				if (i == 0)
				{
					if (j == 0)
					{
						num = 0;
						eSegmentJointType2 = eSegmentJointType.EDGE_CORNER;
					}
					else if (j == _segmentInfos.GetLength(1) - 1)
					{
						num = 270;
						eSegmentJointType2 = eSegmentJointType.EDGE_CORNER;
					}
					else
					{
						eSegmentJointType2 = eSegmentJointType.EDGE;
						num = 0;
					}
				}
				else if (i == _segmentInfos.GetLength(0) - 1)
				{
					if (j == 0)
					{
						num = 90;
						eSegmentJointType2 = eSegmentJointType.EDGE_CORNER;
					}
					else if (j == _segmentInfos.GetLength(1) - 1)
					{
						num = 180;
						eSegmentJointType2 = eSegmentJointType.EDGE_CORNER;
					}
					else
					{
						eSegmentJointType2 = eSegmentJointType.EDGE;
						num = 180;
					}
				}
				else if (j == 0)
				{
					eSegmentJointType2 = eSegmentJointType.EDGE;
					num = 90;
				}
				else if (j == _segmentInfos.GetLength(1) - 1)
				{
					eSegmentJointType2 = eSegmentJointType.EDGE;
					num = 270;
				}
				if (eSegmentJointType2 != eSegmentJointType.NONE)
				{
					_segmentInfos[i, j] = new SegmentedSegmentInfo(eSegmentType.DEFAULT, eSegmentJointType2, num);
				}
			}
		}
	}

	private void CreateSegmentInfos()
	{
		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _depth; j++)
			{
				Coord coord = new Coord(i, j);
				if (coord == _spawnCoord)
				{
					AddSegmentInfo(eSegmentType.SPAWN, coord);
				}
				else if (coord == _gymCoord)
				{
					AddSegmentInfo(eSegmentType.GYM, coord);
				}
				else
				{
					AddSegmentInfo(eSegmentType.DEFAULT, coord);
				}
			}
		}
	}

	private void AddSegmentInfo(eSegmentType type, Coord coord)
	{
		eSegmentJointType segmentJointType = GetSegmentJointType(coord);
		SegmentedSegmentInfo segmentedSegmentInfo = new SegmentedSegmentInfo(type, segmentJointType, GetSegmentRotation(coord, segmentJointType));
		if (_includeEdges)
		{
			_segmentInfos[coord.x + 1, coord.z + 1] = segmentedSegmentInfo;
		}
		else
		{
			_segmentInfos[coord.x, coord.z] = segmentedSegmentInfo;
		}
	}

	private void DefineSpawnCoord()
	{
		GetCoordEmptySegmentInfo(out _spawnCoord);
	}

	private void DefineGymCoord()
	{
		if (GetCoordEmptySegmentInfo(out _gymCoord))
		{
			List<Coord> untouchedNeighbourCoords = GetUntouchedNeighbourCoords(_gymCoord);
			untouchedNeighbourCoords.Remove(_spawnCoord);
			Coord coord = untouchedNeighbourCoords.PickRandom();
			JoinCoords(_gymCoord, coord);
		}
	}

	private bool GetCoordEmptySegmentInfo(out Coord coord)
	{
		List<Coord> list = new List<Coord>();
		for (int i = 0; i < _width; i++)
		{
			for (int j = 0; j < _depth; j++)
			{
				if (!_segmentCoordsJoined.Contains(new Coord(i, j)))
				{
					list.Add(new Coord(i, j));
				}
			}
		}
		list.Remove(_spawnCoord);
		list.Remove(_gymCoord);
		if (list.Count > 0)
		{
			coord = list.PickRandom();
			return true;
		}
		coord = Coord.zero;
		return false;
	}

	private List<Coord> GetNeighbourCoords(Coord coord)
	{
		List<Coord> list = new List<Coord>();
		if (coord.x + 1 < Mathf.CeilToInt((float)_joints.GetLength(0) / 2f))
		{
			list.Add(new Coord(coord.x + 1, coord.z));
		}
		if (coord.x - 1 > -1)
		{
			list.Add(new Coord(coord.x - 1, coord.z));
		}
		if (coord.z + 1 < _joints.GetLength(1))
		{
			list.Add(new Coord(coord.x, coord.z + 1));
		}
		if (coord.z - 1 > -1)
		{
			list.Add(new Coord(coord.x, coord.z - 1));
		}
		return list;
	}

	private List<Coord> GetDoneNeighbourCoord(Coord coord)
	{
		List<Coord> neighbourCoords = GetNeighbourCoords(coord);
		neighbourCoords.RemoveAll((Coord e) => !_segmentCoordsJoined.Contains(e));
		neighbourCoords.Remove(_gymCoord);
		return neighbourCoords;
	}

	private List<Coord> GetUntouchedNeighbourCoords(Coord coord)
	{
		List<Coord> neighbourCoords = GetNeighbourCoords(coord);
		neighbourCoords.RemoveAll((Coord e) => _segmentCoordsJoined.Contains(e));
		neighbourCoords.RemoveAll((Coord e) => _segmentCoordsMarkedToJoin.Contains(e));
		neighbourCoords.Remove(_gymCoord);
		return neighbourCoords;
	}

	public eSegmentJointType GetSegmentJointType(Coord coord)
	{
		eSegmentJointType result = eSegmentJointType.NONE;
		bool[] allJointsFor = GetAllJointsFor(coord);
		switch (Array.FindAll(allJointsFor, (bool e) => e).Length)
		{
		case 1:
			result = eSegmentJointType.ONE_WAY;
			break;
		case 2:
			result = (((!allJointsFor[0] || !allJointsFor[2]) && (!allJointsFor[1] || !allJointsFor[3])) ? eSegmentJointType.CORNER : eSegmentJointType.TWO_WAY);
			break;
		case 3:
			result = eSegmentJointType.THREE_WAY;
			break;
		case 4:
			result = eSegmentJointType.FOUR_WAY;
			break;
		}
		return result;
	}

	public int GetSegmentRotation(Coord coord, eSegmentJointType jointType)
	{
		int result = 0;
		bool[] allJointsFor = GetAllJointsFor(coord);
		switch (jointType)
		{
		case eSegmentJointType.ONE_WAY:
		case eSegmentJointType.TWO_WAY:
		{
			int num2 = Array.FindIndex(allJointsFor, (bool e) => e);
			result = num2 * 90;
			break;
		}
		case eSegmentJointType.CORNER:
		{
			int num3 = Array.FindLastIndex(allJointsFor, (bool e) => e);
			if (num3 != 3 || !allJointsFor[0])
			{
				num3--;
			}
			result = num3 * 90;
			break;
		}
		case eSegmentJointType.THREE_WAY:
		{
			int num = Array.FindIndex(allJointsFor, (bool e) => !e);
			result = (num + 1) % 4 * 90;
			break;
		}
		case eSegmentJointType.FOUR_WAY:
			result = UnityEngine.Random.Range(0, 4) * 90;
			break;
		}
		return result;
	}

	public void DebugJoints()
	{
		string[] array = new string[_segmentInfos.GetLength(0) * 3];
		array.Populate(string.Empty);
		for (int i = 0; i < _joints.GetLength(1); i++)
		{
			for (int j = 0; j < Mathf.CeilToInt((float)_joints.GetLength(0) / 2f); j++)
			{
				Coord coord = new Coord(j, i);
				array[i * 3] = array[i * 3] + (GetJointFor(coord, eDirection.UP) ? "080" : "000");
				array[i * 3 + 1] = array[i * 3 + 1] + (GetJointFor(coord, eDirection.LEFT) ? "88" : "08");
				array[i * 3 + 1] = array[i * 3 + 1] + (GetJointFor(coord, eDirection.RIGHT) ? "8" : "0");
				array[i * 3 + 2] = array[i * 3 + 2] + (GetJointFor(coord, eDirection.DOWN) ? "080" : "000");
			}
		}
		string text = string.Empty;
		for (int k = 0; k < array.Length; k++)
		{
			text = text + array[k] + "\n";
		}
	}
}
