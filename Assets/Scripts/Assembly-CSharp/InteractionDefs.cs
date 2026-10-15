using SimpleJSON;
using UnityEngine;

public class InteractionDefs
{
	private static JSONNode infoJSON;

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "InteractionInfo")).text);
	}

	public static void Remove()
	{
		infoJSON = null;
	}

	public static string AssetID(string id)
	{
		return infoJSON[id]["assetid"];
	}

	public static string InteractionType(string id)
	{
		return infoJSON[id]["interactiontype"];
	}

	public static string TransitionDirection(string id)
	{
		return infoJSON[id]["transitiondirection"];
	}

	public static string TransitionEntrance(string id)
	{
		return infoJSON[id]["transitionentrance"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Interactions, id, "name");
	}

	public static string Dialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Interactions, id, "dialogue");
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}
}
