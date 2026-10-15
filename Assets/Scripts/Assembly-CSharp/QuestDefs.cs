using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class QuestDefs
{
	public const string kQuestPlushSuitMortyID = "QuestMascotMorty";

	private static JSONNode infoJSON;

	public static int Count
	{
		get
		{
			return infoJSON.Count;
		}
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "QuestInfo")).text);
	}

	public static void Remove()
	{
		infoJSON = null;
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "name");
	}

	public static int BadgeRequirement(string id)
	{
		return infoJSON[id]["badgereq"].AsInt;
	}

	public static string CompleteRequirement(string id)
	{
		return infoJSON[id]["completereq"];
	}

	public static string GiverName(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "givername");
	}

	public static string GiverAssetID(string id)
	{
		return infoJSON[id]["giverassetid"];
	}

	public static bool IsWanderer(string id)
	{
		return infoJSON[id]["wanderer"].AsBool;
	}

	public static string[] SolutionIDs(string id)
	{
		return ((string)infoJSON[id]["solutionids"]).Split(new string[1] { "," }, StringSplitOptions.None);
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "description");
	}

	public static string ActiveDialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "activedialogue");
	}

	public static string RejectDialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "rejectdialogue");
	}

	public static string AcceptDialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "acceptdialogue");
	}

	public static string CompleteDialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Quest, id, "completedialogue");
	}

	public static List<RewardData> Rewards(string id)
	{
		List<RewardData> list = new List<RewardData>();
		string[] array = ((string)infoJSON[id]["rewards"]).Split(new string[1] { ", " }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(':');
			eRewardType type = (eRewardType)Enum.Parse(typeof(eRewardType), array2[0]);
			string id2 = array2[1];
			int value = ((!string.IsNullOrEmpty(array2[2])) ? int.Parse(array2[2]) : 0);
			RewardData item = new RewardData(type, id2, 100f, value, true);
			list.Add(item);
		}
		return list;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return infoJSON[index]["id"];
	}

	public static QuestGiverData GetNewQuestGiverData(string id)
	{
		if (infoJSON[id] != null)
		{
			return new QuestGiverData(id);
		}
		return null;
	}

	public static List<string> ConvertGuestIDListToAssetIds(List<string> questIDs)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < questIDs.Count; i++)
		{
			list.Add(GiverAssetID(questIDs[i]));
		}
		return list;
	}
}
