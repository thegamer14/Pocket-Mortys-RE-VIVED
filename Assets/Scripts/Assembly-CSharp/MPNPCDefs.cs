using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPNPCDefs
{
	private static JSONNode _infoJSON;

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["npc_id"].Value == id);
	}

	public static string AssetID(string id)
	{
		return GetNode(id)["asset_id"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.NPC, id, "name");
	}

	public static string Dialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.NPC, id, "dialogue");
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "NPCInfo")).text);
	}

	public static void Remove()
	{
		_infoJSON = null;
	}
}
