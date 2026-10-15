using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPWorldDefs : MonoBehaviour
{
	public const string kWorld1Id = "1";

	public const string kWorld2Id = "2";

	public const string kWorld3Id = "3";

	public const string kWorld4Id = "4";

	private static JSONNode _infoJSON;

	public static string[] allWorldIDs { get; private set; }

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["world_id"].Value == id);
	}

	public static JSONArray Zones(string id)
	{
		return GetNode(id)["zones"].AsArray;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "WorldInfo")).text);
		allWorldIDs = new string[_infoJSON.Count];
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			allWorldIDs[i] = _infoJSON[i]["world_id"];
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
	}

	public static string GetResourceId(string netId)
	{
		return string.Format("MPDimension{0}", netId);
	}

	public static Coord GetPlayerSpawnCoord(string worldId, bool isRevive = false)
	{
		List<Coord> list = new List<Coord>();
		if (!isRevive)
		{
			switch (worldId)
			{
			case "1":
				list.Add(new Coord(30, 36));
				break;
			case "2":
				list.Add(new Coord(21, 73));
				break;
			case "3":
				list.Add(new Coord(31, 51));
				break;
			case "4":
				list.Add(new Coord(31, 76));
				break;
			}
		}
		else
		{
			switch (worldId)
			{
			case "1":
				list.Add(new Coord(41, 34));
				list.Add(new Coord(43, 34));
				list.Add(new Coord(45, 34));
				break;
			case "2":
				list.Add(new Coord(29, 69));
				list.Add(new Coord(31, 69));
				list.Add(new Coord(33, 69));
				break;
			case "3":
				list.Add(new Coord(68, 32));
				list.Add(new Coord(70, 32));
				list.Add(new Coord(72, 32));
				break;
			case "4":
				list.Add(new Coord(29, 17));
				list.Add(new Coord(31, 17));
				list.Add(new Coord(33, 17));
				break;
			}
		}
		return (list.Count <= 0) ? new Coord(0, 0) : list.PickRandom();
	}

	public static List<Vector3> GetItemSpawnCoords(string worldId)
	{
		JSONArray asArray = GetNode(worldId)["pickups"]["placements"].AsArray;
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < asArray.Count; i++)
		{
			list.Add(new Vector3(asArray[i].AsArray[0].AsInt, 0f, asArray[i].AsArray[1].AsInt));
		}
		return list;
	}

	public static List<Vector3> GetWildMortySpawnCoords(string worldId)
	{
		JSONArray asArray = GetNode(worldId)["wild_morties"]["placements"].AsArray;
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < asArray.Count; i++)
		{
			list.Add(new Vector3(asArray[i].AsArray[0].AsInt, 0f, asArray[i].AsArray[1].AsInt));
		}
		return list;
	}

	public static MPWorldMapData GetMapWorldDataForId(string id)
	{
		MPWorldMapData mPWorldMapData = new MPWorldMapData();
		switch (id)
		{
		case "1":
			mPWorldMapData.sizeMin = new Coord(0, 1);
			mPWorldMapData.sizeMax = new Coord(72, 88);
			mPWorldMapData.top = 330f;
			mPWorldMapData.bottom = -260f;
			mPWorldMapData.topLeft = -320f;
			mPWorldMapData.topRight = 370f;
			mPWorldMapData.bottomLeft = -400f;
			mPWorldMapData.bottomRight = 450f;
			break;
		case "2":
			mPWorldMapData.sizeMin = new Coord(3, 12);
			mPWorldMapData.sizeMax = new Coord(59, 73);
			mPWorldMapData.top = 290f;
			mPWorldMapData.bottom = -190f;
			mPWorldMapData.topLeft = -350f;
			mPWorldMapData.topRight = 310f;
			mPWorldMapData.bottomLeft = -420f;
			mPWorldMapData.bottomRight = 370f;
			break;
		case "3":
			mPWorldMapData.sizeMin = new Coord(22, 6);
			mPWorldMapData.sizeMax = new Coord(97, 68);
			mPWorldMapData.top = 240f;
			mPWorldMapData.bottom = -170f;
			mPWorldMapData.topLeft = -360f;
			mPWorldMapData.topRight = 360f;
			mPWorldMapData.bottomLeft = -430f;
			mPWorldMapData.bottomRight = 430f;
			break;
		case "4":
			mPWorldMapData.sizeMin = new Coord(4, 2);
			mPWorldMapData.sizeMax = new Coord(58, 75);
			mPWorldMapData.top = 300f;
			mPWorldMapData.bottom = -250f;
			mPWorldMapData.topLeft = -300f;
			mPWorldMapData.topRight = 300f;
			mPWorldMapData.bottomLeft = -340f;
			mPWorldMapData.bottomRight = 340f;
			break;
		}
		return mPWorldMapData;
	}
}
