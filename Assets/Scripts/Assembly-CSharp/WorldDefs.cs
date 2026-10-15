using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class WorldDefs
{
	public const string kCitadelWinnerStatueNodeID = "WinnerStatue";

	public const int kUniqueWildMortiesPerWorld = 3;

	public const float kChanceOfRareMortyPerWorld = 0.3f;

	public const int kUniqueTrainersPerWorld = 3;

	public const string kTutorialID = "Tutorial";

	public const string kTutorialEntranceID = "SegmentedWorldPortal";

	public const string kTutorialExitID = "TutorialWorld";

	public const string kCitadelID = "Citadel";

	public const string kCitadelChambersEntranceID = "CitadelCouncilChambers";

	public const string kCitadelTournamentEntranceID = "CitadelTournament";

	public const string kCitadelPortalEntranceID = "CitadelPortal";

	public const string kCitadelHealingCenterEntranceID = "CitadelHealingCenter";

	public const string kTournamentLobbyID = "TournamentLobby";

	public const string kTournamentLobbyCitadelEntranceID = "TournamentLobbyCitadel";

	public const string kTournamentLobbyOrganiserPortalEntranceID = "TournamentLobbyOrganiserPortal";

	public const string kTournamentLobbyToiletPortalEntranceID = "TournamentLobbyToiletPortal";

	public const string kSegmentedWorldEnteranceID = "SegmentedWorldPortal";

	public const string kCouponBackgroundID = "Summer";

	public const string kBootcampBackgroundID = "Summer";

	public const string kCourierFlapEntrance = "CitadelCourierFlapPortal";

	private static JSONNode infoJSON;

	private static List<string> allSegmentedWorldIDs;

	public static string[] allWorldIDs { get; private set; }

	public static int SegmentedWorldIDCount
	{
		get
		{
			return allSegmentedWorldIDs.Count;
		}
	}

	public static bool isSegmentedWorldSaved
	{
		get
		{
			return App.saveData.segmentedWorld.worldID != null;
		}
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "WorldInfo")).text);
		allWorldIDs = new string[infoJSON.Count];
		for (int i = 0; i < infoJSON.Count; i++)
		{
			allWorldIDs[i] = infoJSON[i]["id"];
		}
		allSegmentedWorldIDs = new List<string>();
		for (int j = 0; j < allWorldIDs.Length; j++)
		{
			if (IsSegmented(allWorldIDs[j]) && IsDimensionPortal(allWorldIDs[j]))
			{
				allSegmentedWorldIDs.Add(allWorldIDs[j]);
			}
		}
	}

	public static void Remove()
	{
		infoJSON = null;
		allWorldIDs = null;
		allSegmentedWorldIDs = null;
	}

	public static string SegmentID(string id)
	{
		return infoJSON[id]["segmentid"];
	}

	public static string[] NodeSetIDs(string id)
	{
		return ((string)infoJSON[id]["nodesetids"]).Split(',');
	}

	public static bool IsSegmented(string id)
	{
		return infoJSON[id]["issegmented"].AsBool;
	}

	public static bool IsDimensionPortal(string id)
	{
		return infoJSON[id]["isdimensionportal"].AsBool;
	}

	public static string MaterialID(string id)
	{
		return infoJSON[id]["materialid"];
	}

	public static float SegmentWidth(string id)
	{
		return (float)infoJSON[id]["segmentwidth"].AsInt * 1f;
	}

	public static float SegmentDepth(string id)
	{
		return (float)infoJSON[id]["segmentdepth"].AsInt * 1f;
	}

	public static string[] NodeLimits(string id)
	{
		return ((string)infoJSON[id]["nodelimits"]).Split(',');
	}

	public static float ItemPartTypeSplit(string id)
	{
		return infoJSON[id]["itemparttypesplit"].AsFloat;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}

	public static string GetSegmentedWorldID(int index)
	{
		if (index > -1 && index < allSegmentedWorldIDs.Count)
		{
			return allSegmentedWorldIDs[index];
		}
		return null;
	}

	public static string GetRandomSegmentedWorldID(List<string> excludingIDs = null)
	{
		List<string> list = new List<string>(allSegmentedWorldIDs);
		if (excludingIDs != null)
		{
			for (int i = 0; i < excludingIDs.Count; i++)
			{
				list.Remove(excludingIDs[i]);
			}
		}
		return list.PickRandom();
	}

	public static bool IsWorldLockedToBounds(string id)
	{
		return !string.IsNullOrEmpty(infoJSON[id]["camerabounds"]);
	}

	public static Vector3 BoundsMin(string id)
	{
		string aJSON = infoJSON[id]["camerabounds"];
		JSONNode jSONNode = JSONNode.Parse(aJSON);
		return new Vector3(jSONNode["min"]["x"].AsFloat, jSONNode["min"]["y"].AsFloat, jSONNode["min"]["z"].AsFloat);
	}

	public static Vector3 BoundsMax(string id)
	{
		string aJSON = infoJSON[id]["camerabounds"];
		JSONNode jSONNode = JSONNode.Parse(aJSON);
		return new Vector3(jSONNode["max"]["x"].AsFloat, jSONNode["max"]["y"].AsFloat, jSONNode["max"]["z"].AsFloat);
	}

	public static Dictionary<ePlaceholderType, int> GetEmptyLimitDict(int number = 0)
	{
		Dictionary<ePlaceholderType, int> dictionary = new Dictionary<ePlaceholderType, int>();
		dictionary.Add(ePlaceholderType.ITEM_PART_OR_CRATE, number);
		dictionary.Add(ePlaceholderType.MORTY, number);
		dictionary.Add(ePlaceholderType.NPC, number);
		dictionary.Add(ePlaceholderType.TRAINER, number);
		dictionary.Add(ePlaceholderType.PORTAL, number);
		dictionary.Add(ePlaceholderType.SIGN_POST, number);
		dictionary.Add(ePlaceholderType.GYM, number);
		dictionary.Add(ePlaceholderType.GACHA_OR_CRAFT, number);
		dictionary.Add(ePlaceholderType.QUEST_GIVER, number);
		return dictionary;
	}

	public static Dictionary<ePlaceholderType, int> GetLimits(string worldID, int worldSize)
	{
		Dictionary<ePlaceholderType, int> emptyLimitDict = GetEmptyLimitDict();
		string[] array = NodeLimits(worldID);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Trim('{', '}').Split(':');
			ePlaceholderType ePlaceholderType2 = (ePlaceholderType)Enum.Parse(typeof(ePlaceholderType), array2[0]);
			float num = float.Parse(array2[1]);
			if (ePlaceholderType2 != ePlaceholderType.NONE)
			{
				if (num == -1f)
				{
					emptyLimitDict.Remove(ePlaceholderType2);
				}
				else
				{
					emptyLimitDict[ePlaceholderType2] = Mathf.FloorToInt(num * (float)worldSize);
				}
			}
		}
		return emptyLimitDict;
	}

	public static void SetSavedSegmentedWorldSegments(string worldID, string nodeSetID, string entranceID, List<Segment> segments, TrainerData gymLeader)
	{
		if (App.saveData.segmentedWorld.worldID == null)
		{
			List<SegmentSaveData> list = new List<SegmentSaveData>();
			for (int i = 0; i < segments.Count; i++)
			{
				list.Add(new SegmentSaveData(segments[i]));
			}
			App.saveData.segmentedWorld.worldID = worldID;
			App.saveData.segmentedWorld.nodeSetID = nodeSetID;
			App.saveData.segmentedWorld.entranceID = entranceID;
			App.saveData.segmentedWorld.segments = list;
			App.saveData.segmentedWorld.gymLeader = gymLeader;
		}
		else
		{
			for (int j = 0; j < App.saveData.segmentedWorld.segments.Count; j++)
			{
				App.saveData.segmentedWorld.segments[j].UpdateNonBasicNodes(segments[j]);
			}
		}
	}

	public static void SetSavedSegmentedWorldPlayer(Vector3 playerPosition, eDirection playerDirection, Vector3[] playerMortyPositions, eDirection[] playerMortyDirections)
	{
		SerializableVector3[] array = new SerializableVector3[playerMortyPositions.Length];
		for (int i = 0; i < playerMortyPositions.Length; i++)
		{
			array[i] = new Vector3(playerMortyPositions[i].x, playerMortyPositions[i].y, playerMortyPositions[i].z);
		}
		App.saveData.segmentedWorld.playerPosition = playerPosition;
		App.saveData.segmentedWorld.playerDirection = playerDirection;
		App.saveData.segmentedWorld.playerMortyPositions = array;
		App.saveData.segmentedWorld.playerMortyDirections = playerMortyDirections;
	}

	public static string GetSavedSegmentedWorldID()
	{
		return App.saveData.segmentedWorld.worldID;
	}

	public static string GetSavedSegmentedNodeSetID()
	{
		return App.saveData.segmentedWorld.nodeSetID;
	}

	public static string GetSavedSegmentedEntranceID()
	{
		return App.saveData.segmentedWorld.entranceID;
	}

	public static List<SegmentSaveData> GetSavedSegmentedWorldSegments()
	{
		return App.saveData.segmentedWorld.segments;
	}

	public static TrainerData GetSavedSegmentedGymLeaderData()
	{
		return App.saveData.segmentedWorld.gymLeader;
	}

	public static Vector3 GetSavedSegmentedPlayerPosition()
	{
		return App.saveData.segmentedWorld.playerPosition;
	}

	public static eDirection GetSavedSegmentedPlayerDirection()
	{
		return App.saveData.segmentedWorld.playerDirection;
	}

	public static Vector3[] GetSavedSegmentedPlayerMortyPositions()
	{
		SerializableVector3[] playerMortyPositions = App.saveData.segmentedWorld.playerMortyPositions;
		Vector3[] array = new Vector3[playerMortyPositions.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Vector3(playerMortyPositions[i].x, playerMortyPositions[i].y, playerMortyPositions[i].z);
		}
		return array;
	}

	public static eDirection[] GetSavedSegmentedPlayerMortyDirections()
	{
		return App.saveData.segmentedWorld.playerMortyDirections;
	}
}
