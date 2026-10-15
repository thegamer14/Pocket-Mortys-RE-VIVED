using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TrainerDefs
{
	public const string kBirdpersonAssetID = "NPCBirdperson";

	public const string kBirdpersonName = "Birdperson";

	private const int _kGymLeaderBadgeTypesCount = 6;

	public const string kGarageIntroTrainerID = "TrainerGarageIntro";

	public const string kTutorialTrainerID = "TrainerTutorial";

	public const string kTutorialGymLeaderID = "TrainerTutorialGymLeader";

	public const string kCouncilMemberID = "TrainerCouncil{0}";

	public const string kMysteriousRickCouncilMemberID = "TrainerCouncilMysteriousRick";

	public const string kMysteriousRickGymLeaderID = "TrainerGymLeader24";

	private static JSONNode _infoJSON;

	private static List<string> _trainerIDs;

	private static List<string> _gymLeaderIDs;

	public static string AssetID(string id)
	{
		return _infoJSON[id]["assetid"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Trainer, id, "name");
	}

	public static eTrainerType Type(string id)
	{
		return (!string.IsNullOrEmpty(_infoJSON[id]["type"])) ? ((eTrainerType)Enum.Parse(typeof(eTrainerType), _infoJSON[id]["type"])) : eTrainerType.NONE;
	}

	public static string DialoguePreBattle1(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Trainer, id, "dialogueprebattle1");
	}

	public static string DialoguePreBattle2(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Trainer, id, "dialogueprebattle2");
	}

	public static string DialoguePreBattle3(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Trainer, id, "dialogueprebattle3");
	}

	public static string DialoguePostBattle(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Trainer, id, "dialoguepostbattle");
	}

	public static List<MortyData> Morties(string id)
	{
		List<MortyData> list = new List<MortyData>();
		if (!string.IsNullOrEmpty(_infoJSON[id]["morties"]))
		{
			string[] array = ((string)_infoJSON[id]["morties"]).Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':');
				string id2 = array2[0];
				int level = (string.IsNullOrEmpty(array2[1]) ? MortyDefs.GetBalancedMortyLevelForTrainerType(Type(id), false) : int.Parse(array2[1]));
				list.Add(MortyDefs.GetNewMortyData(id2, level, eIVSetUp.RANDOM_RESTRICTED, eEVSetUp.RANDOM));
			}
		}
		return list;
	}

	public static Dictionary<string, int> Items(string id)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		if (!string.IsNullOrEmpty(_infoJSON[id]["items"]))
		{
			string[] array = ((string)_infoJSON[id]["items"]).Split(new string[1] { ", " }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':');
				dictionary.Add(array2[0], int.Parse(array2[1]));
			}
		}
		return dictionary;
	}

	public static List<RewardData> Rewards(string id)
	{
		List<RewardData> list = new List<RewardData>();
		string[] array = ((string)_infoJSON[id]["rewards"]).Split(new string[1] { ", " }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(':');
			eRewardType type = (eRewardType)Enum.Parse(typeof(eRewardType), array2[0]);
			string id2 = array2[1];
			int num = int.Parse(array2[2]);
			int value = ((!string.IsNullOrEmpty(array2[3])) ? int.Parse(array2[3]) : 0);
			RewardData item = new RewardData(type, id2, num, value);
			list.Add(item);
		}
		return list;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "TrainerInfo")).text);
		_trainerIDs = new List<string>();
		_gymLeaderIDs = new List<string>();
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			switch (Type(_infoJSON[i]["id"]))
			{
			case eTrainerType.DEFAULT:
				_trainerIDs.Add(_infoJSON[i]["id"]);
				break;
			case eTrainerType.GYM_LEADER:
				_gymLeaderIDs.Add(_infoJSON[i]["id"]);
				break;
			}
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
		_trainerIDs = null;
		_gymLeaderIDs = null;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && _infoJSON[id] != null;
	}

	public static TrainerData GetNewTrainerData(string id)
	{
		if (_infoJSON[id] != null)
		{
			return new TrainerData(id);
		}
		return null;
	}

	public static TrainerData GetNewRandomTrainerData(List<string> excluding = null)
	{
		List<string> list = new List<string>(_trainerIDs);
		if (excluding != null)
		{
			list.RemoveAll((string e) => excluding.Contains(e));
		}
		string id = ((list.Count <= 0) ? _trainerIDs.PickRandom() : list.PickRandom());
		return new TrainerData(id);
	}

	public static TrainerData GetNewRandomGymLeaderData(List<string> excluding = null)
	{
		List<string> list = new List<string>(_gymLeaderIDs);
		if (PlayerDefs.IsCouncilStoryComplete())
		{
			list.Remove("TrainerGymLeader24");
		}
		if (excluding != null)
		{
			list.RemoveAll((string e) => excluding.Contains(e));
		}
		string id = ((list.Count <= 0) ? _gymLeaderIDs.PickRandom() : list.PickRandom());
		return new TrainerData(id);
	}

	public static string GetRandomPreBattleDialogue(string id)
	{
		List<string> list = new List<string>();
		list.Add(DialoguePreBattle1(id));
		list.Add(DialoguePreBattle2(id));
		list.Add(DialoguePreBattle3(id));
		list.RemoveAll(string.IsNullOrEmpty);
		if (list.Count > 0)
		{
			return list.PickRandom();
		}
		return null;
	}

	public static int GetRandomGymLeaderBadgeID()
	{
		return UnityEngine.Random.Range(0, 6) + 1;
	}

	public static int GetCouncilMemberBadgeRequirement(int number)
	{
		switch (number)
		{
		default:
			return 0;
		case 1:
			return 2;
		case 2:
			return 6;
		case 3:
			return 11;
		case 4:
			return 20;
		case 5:
			return 30;
		case 6:
			return 42;
		}
	}
}
