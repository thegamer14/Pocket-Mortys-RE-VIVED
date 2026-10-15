using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SegmentResourcePool
{
	private const string _kResourcePathSpawn = "/Spawn";

	private const string _kResourcePathRegular = "/Regular";

	private const string _kTypeDefault = "Default/";

	private const string _kTypeSpawn = "Spawn/";

	private const string _kTypeGym = "Gym/";

	private const string _kJointTypeBase = "Base";

	private const string _kJointTypeCorner = "Corner";

	private const string _kJointTypeOneWay = "OneWay";

	private const string _kJointTypeTwoWay = "TwoWay";

	private const string _kJointTypeThreeWay = "ThreeWay";

	private const string _kJointTypeFourWay = "FourWay";

	private const string _kJointTypeEdge = "Edge";

	private const string _kJointTypeEdgeCorner = "EdgeCorner";

	private Dictionary<eSegmentType, Dictionary<eSegmentJointType, UnityEngine.Object[]>> _segmentDict;

	private Dictionary<eSegmentType, Dictionary<eSegmentJointType, int>> _segmentIndexDict;

	private Material _material;

	public int eSegmentTypeCount = Enum.GetNames(typeof(eSegmentType)).Length;

	public int eSegmentJointTypeCount = Enum.GetNames(typeof(eSegmentJointType)).Length;

	public SegmentResourcePool(string worldID)
	{
		_segmentDict = new Dictionary<eSegmentType, Dictionary<eSegmentJointType, UnityEngine.Object[]>>();
		_segmentIndexDict = new Dictionary<eSegmentType, Dictionary<eSegmentJointType, int>>();
		LoadResourcesForSet(worldID);
	}

	public void LoadResourcesForSet(string worldID)
	{
		_segmentDict.Clear();
		_segmentIndexDict.Clear();
		string text = string.Format("World/Segments/{0}/", ((!WorldDefs.IsSegmented(worldID)) ? string.Empty : "Segmented/") + WorldDefs.SegmentID(worldID));
		_segmentDict.Add(eSegmentType.DEFAULT, CreateDictForJointResources(text + "Default/"));
		_segmentDict.Add(eSegmentType.SPAWN, CreateDictForJointResources(text + "Spawn/"));
		_segmentDict.Add(eSegmentType.GYM, CreateDictForJointResources(text + "Gym/"));
		_segmentIndexDict.Add(eSegmentType.DEFAULT, CreateIndexDictForJointResources());
		_segmentIndexDict.Add(eSegmentType.SPAWN, CreateIndexDictForJointResources());
		_segmentIndexDict.Add(eSegmentType.GYM, CreateIndexDictForJointResources());
		_material = Resources.Load(string.Format("World/SegmentedMaterials/{0}", WorldDefs.MaterialID(worldID))) as Material;
	}

	private Dictionary<eSegmentJointType, UnityEngine.Object[]> CreateDictForJointResources(string assetPath)
	{
		Dictionary<eSegmentJointType, UnityEngine.Object[]> dictionary = new Dictionary<eSegmentJointType, UnityEngine.Object[]>();
		dictionary.Add(eSegmentJointType.BASE, (UnityEngine.Object[])Resources.LoadAll(assetPath + "Base").Shuffle());
		dictionary.Add(eSegmentJointType.CORNER, (UnityEngine.Object[])Resources.LoadAll(assetPath + "Corner").Shuffle());
		dictionary.Add(eSegmentJointType.ONE_WAY, (UnityEngine.Object[])Resources.LoadAll(assetPath + "OneWay").Shuffle());
		dictionary.Add(eSegmentJointType.TWO_WAY, (UnityEngine.Object[])Resources.LoadAll(assetPath + "TwoWay").Shuffle());
		dictionary.Add(eSegmentJointType.THREE_WAY, (UnityEngine.Object[])Resources.LoadAll(assetPath + "ThreeWay").Shuffle());
		dictionary.Add(eSegmentJointType.FOUR_WAY, (UnityEngine.Object[])Resources.LoadAll(assetPath + "FourWay").Shuffle());
		dictionary.Add(eSegmentJointType.EDGE, (UnityEngine.Object[])Resources.LoadAll(assetPath + "Edge").Shuffle());
		dictionary.Add(eSegmentJointType.EDGE_CORNER, (UnityEngine.Object[])Resources.LoadAll(assetPath + "EdgeCorner").Shuffle());
		return dictionary;
	}

	private Dictionary<eSegmentJointType, int> CreateIndexDictForJointResources()
	{
		Dictionary<eSegmentJointType, int> dictionary = new Dictionary<eSegmentJointType, int>();
		dictionary.Add(eSegmentJointType.BASE, 0);
		dictionary.Add(eSegmentJointType.CORNER, 0);
		dictionary.Add(eSegmentJointType.ONE_WAY, 0);
		dictionary.Add(eSegmentJointType.TWO_WAY, 0);
		dictionary.Add(eSegmentJointType.THREE_WAY, 0);
		dictionary.Add(eSegmentJointType.FOUR_WAY, 0);
		dictionary.Add(eSegmentJointType.EDGE, 0);
		dictionary.Add(eSegmentJointType.EDGE_CORNER, 0);
		return dictionary;
	}

	public UnityEngine.Object Get(eSegmentType type, eSegmentJointType jointType, string resourceName)
	{
		if (resourceName == null)
		{
			return Get(type, jointType);
		}
		return _segmentDict[type][jointType].Where((UnityEngine.Object e) => e.name == resourceName).First();
	}

	public UnityEngine.Object Get(eSegmentType type, eSegmentJointType jointType)
	{
		if (type == eSegmentType.NONE || jointType == eSegmentJointType.NONE)
		{
			return null;
		}
		_segmentIndexDict[type][jointType]++;
		_segmentIndexDict[type][jointType] = _segmentIndexDict[type][jointType] % _segmentDict[type][jointType].Length;
		return _segmentDict[type][jointType][_segmentIndexDict[type][jointType]];
	}

	public Material GetSegmentedMaterial()
	{
		return _material;
	}

	public int GetSegmentTypeCount(eSegmentType type, eSegmentJointType jointType)
	{
		if (_segmentDict.ContainsKey(type) && _segmentDict[type].ContainsKey(jointType))
		{
			return _segmentDict[type][jointType].Length;
		}
		return 0;
	}
}
