using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class NPCDefs
{
	public const string kChambersGuard = "NPCRickChambersGuard";

	private static JSONNode infoJSON;

	private static List<string> npcIDs;

	public static int Count
	{
		get
		{
			return infoJSON.Count;
		}
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "NPCInfo")).text);
	}

	public static void Remove()
	{
		infoJSON = null;
	}

	public static string AssetID(string id)
	{
		return infoJSON[id]["assetid"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.NPC, id, "name");
	}

	public static string Dialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.NPC, id, "dialogue");
	}

	public static string RandomWorldID(string id)
	{
		return infoJSON[id]["randomworldid"];
	}

	public static bool IsWanderer(string id)
	{
		return infoJSON[id]["wanderer"].AsBool;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return infoJSON[index]["id"];
	}

	public static List<string> GetListOfRandomNPCsForWorld(string worldID, List<string> excludeQuestGiverAssetIDs)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Count; i++)
		{
			string iD = GetID(i);
			if (worldID.Equals(RandomWorldID(iD)) && !excludeQuestGiverAssetIDs.Contains(AssetID(iD)))
			{
				list.Add(iD);
			}
		}
		return list;
	}

	public static NPCData GetNewNPCData(string id)
	{
		if (infoJSON[id] != null)
		{
			return new NPCData(id);
		}
		return null;
	}

	public static NPCData GetNewRandomNPCData()
	{
		int aIndex = Random.Range(0, infoJSON.Count);
		return new NPCData(infoJSON[aIndex]["id"]);
	}
}
