using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class TournamentDefs
{
	public const string kTrainerDialogueOrganiser = "DialTourn_{0}_Organiser";

	public const string kTrainerDialogueAntag = "DialTourn_{0}_BattleIntro";

	public const string kTrainerDialoguePre = "DialTourn_{0}_BattleStart";

	public const string kTrainerDialoguePost = "DialTourn_{0}_BattleEnd";

	public const string kTrainerDialogueAnnoun = "DialTourn_{0}_Announcer";

	public const string kBattleBackgroundID = "Tournament";

	public const int kCountdownTargetOffset = 86400;

	private static JSONNode infoJSON;

	private static JSONNode eventInfoJSON;

	private static JSONNode trainerInfoJSON;

	private static JSONNode trainerDialogueInfoJSON;

	private static List<string> _orderedTournamentIDs;

	public static List<string> orderedTournamentIDs
	{
		get
		{
			return _orderedTournamentIDs;
		}
	}

	public static string currentTournamentID { get; private set; }

	public static string currentEventID { get; private set; }

	public static string currentTrainerID { get; private set; }

	public static List<MortyData> currentMorties { get; private set; }

	public static Dictionary<string, int> currentItems { get; private set; }

	public static int currentTicketCount
	{
		get
		{
			return App.saveData.tournament.ownedTickets.Count;
		}
	}

	public static string TournamentName(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Tournament, id, "name");
	}

	public static string TournamentAssetID(string id)
	{
		return infoJSON[id]["assetid"];
	}

	public static int TournamentDisplayOrder(string id)
	{
		return infoJSON[id]["displayorder"].AsInt;
	}

	public static string[] TournamentEventIDs(string id)
	{
		return ((string)infoJSON[id]["events"]).Split(new string[1] { ", " }, StringSplitOptions.RemoveEmptyEntries);
	}

	public static string EventName(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.TournamentEvent, id, "name");
	}

	public static string EventDialogueID(string id)
	{
		return eventInfoJSON[id]["dialogueid"];
	}

	public static int EventBattlePoints(string id)
	{
		return eventInfoJSON[id]["battlepoints"].AsInt;
	}

	public static string[] EventTrainerIDs(string id)
	{
		return ((string)eventInfoJSON[id]["trainers"]).Split(new string[1] { ", " }, StringSplitOptions.RemoveEmptyEntries);
	}

	public static List<KeyValuePair<MortyData, int>> EventSelectableMorties(string id)
	{
		List<KeyValuePair<MortyData, int>> list = new List<KeyValuePair<MortyData, int>>();
		if (!string.IsNullOrEmpty(eventInfoJSON[id]["selectablemorties"]))
		{
			JSONNode jSONNode = JSONNode.Parse(eventInfoJSON[id]["selectablemorties"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				string id2 = jSONNode[i]["id"];
				int asInt = jSONNode[i]["level"].AsInt;
				MortyData newMortyData = MortyDefs.GetNewMortyData(id2, asInt, eIVSetUp.MAX, eEVSetUp.MAX);
				int battlePointsForMorty = GetBattlePointsForMorty(newMortyData);
				list.Add(new KeyValuePair<MortyData, int>(newMortyData, battlePointsForMorty));
			}
		}
		return list;
	}

	public static List<KeyValuePair<string, int>> EventSelectableItems(string id)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		if (!string.IsNullOrEmpty(eventInfoJSON[id]["selectableitems"]))
		{
			JSONNode jSONNode = JSONNode.Parse(eventInfoJSON[id]["selectableitems"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				string text = jSONNode[i]["id"];
				int battlePointsForItem = GetBattlePointsForItem(text);
				list.Add(new KeyValuePair<string, int>(text, battlePointsForItem));
			}
		}
		return list;
	}

	public static List<RewardData> EventStashedItems(string id)
	{
		List<RewardData> list = new List<RewardData>();
		if (!string.IsNullOrEmpty(eventInfoJSON[id]["stasheditems"]))
		{
			JSONNode jSONNode = JSONNode.Parse(eventInfoJSON[id]["stasheditems"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				string id2 = jSONNode[i]["id"];
				int value = Mathf.Max(1, jSONNode[i]["count"].AsInt);
				list.Add(new RewardData(eRewardType.ITEM, id2, 100f, value));
			}
		}
		return list;
	}

	public static List<RewardData> EventRewards(string id)
	{
		List<RewardData> list = new List<RewardData>();
		if (!string.IsNullOrEmpty(eventInfoJSON[id]["rewards"]))
		{
			JSONNode jSONNode = JSONNode.Parse(eventInfoJSON[id]["rewards"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				eRewardType type = (eRewardType)Enum.Parse(typeof(eRewardType), jSONNode[i]["type"]);
				string id2 = jSONNode[i]["id"];
				int asInt = jSONNode[i]["chance"].AsInt;
				int asInt2 = jSONNode[i]["value"].AsInt;
				list.Add(new RewardData(type, id2, asInt, asInt2));
			}
		}
		return list;
	}

	public static string TrainerAssetID(string id)
	{
		return trainerInfoJSON[id]["assetid"];
	}

	public static string TrainerName(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.TournamentTrainer, id, "name");
	}

	public static string TrainerDialogueID(string id)
	{
		return trainerInfoJSON[id]["dialogueid"];
	}

	public static string TrainerRingAsset(string id)
	{
		return trainerInfoJSON[id]["ringasset"];
	}

	public static string TrainerScreenAsset(string id)
	{
		return trainerInfoJSON[id]["screenasset"];
	}

	public static string TrainerStageAsset(string id)
	{
		return trainerInfoJSON[id]["stageasset"];
	}

	public static float TrainerIntelligence(string id)
	{
		return trainerInfoJSON[id]["intelligence"].AsFloat;
	}

	public static List<MortyData> TrainerMorties(string id)
	{
		List<MortyData> list = new List<MortyData>();
		if (TestDefs.TOURNAMENT_TRAINERS_EASY)
		{
			list.Add(MortyDefs.GetNewMortyData("MortyDefault", 1, eIVSetUp.NONE, eEVSetUp.NONE));
			return list;
		}
		if (!string.IsNullOrEmpty(trainerInfoJSON[id]["morties"]))
		{
			JSONNode jSONNode = JSONNode.Parse(trainerInfoJSON[id]["morties"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				string id2 = jSONNode[i]["id"];
				int asInt = jSONNode[i]["level"].AsInt;
				list.Add(MortyDefs.GetNewMortyData(id2, asInt, eIVSetUp.NONE, eEVSetUp.MAX));
			}
		}
		return list;
	}

	public static Dictionary<string, int> TrainerItems(string id)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		if (!string.IsNullOrEmpty(trainerInfoJSON[id]["items"]))
		{
			JSONNode jSONNode = JSONNode.Parse(trainerInfoJSON[id]["items"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				string key = jSONNode[i]["id"];
				int asInt = jSONNode[i]["count"].AsInt;
				dictionary.Add(key, (asInt <= 0) ? 1 : asInt);
			}
		}
		return dictionary;
	}

	public static List<RewardData> TrainerRewards(string id)
	{
		List<RewardData> list = new List<RewardData>();
		if (!string.IsNullOrEmpty(trainerInfoJSON[id]["rewards"]))
		{
			JSONNode jSONNode = JSONNode.Parse(trainerInfoJSON[id]["rewards"]);
			for (int i = 0; i < jSONNode.Count; i++)
			{
				eRewardType type = (eRewardType)Enum.Parse(typeof(eRewardType), jSONNode[i]["type"]);
				string id2 = jSONNode[i]["id"];
				int asInt = jSONNode[i]["chance"].AsInt;
				int asInt2 = jSONNode[i]["value"].AsInt;
				list.Add(new RewardData(type, id2, asInt, asInt2));
			}
		}
		return list;
	}

	public static DialogueData DialogueOrganiser(string id)
	{
		string id2 = string.Format("DialTourn_{0}_Organiser", EventDialogueID(id));
		return TrainerDialogueAppended(id2);
	}

	public static DialogueData DialogueAntag(string id)
	{
		string id2 = string.Format("DialTourn_{0}_BattleIntro", TrainerDialogueID(id));
		return TrainerDialogueAppended(id2);
	}

	public static DialogueData DialogueTrainerPre(string id)
	{
		string id2 = string.Format("DialTourn_{0}_BattleStart", TrainerDialogueID(id));
		return TrainerDialogueAppended(id2);
	}

	public static DialogueData DialogueTrainerPost(string id)
	{
		string id2 = string.Format("DialTourn_{0}_BattleEnd", TrainerDialogueID(id));
		return TrainerDialogueAppended(id2);
	}

	public static DialogueData DialogueAnnouncer(string id)
	{
		string id2 = string.Format("DialTourn_{0}_Announcer", TrainerDialogueID(id));
		return TrainerDialogueAppended(id2);
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "TournamentInfo")).text);
		eventInfoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "TournamentEventInfo")).text);
		trainerInfoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "TournamentTrainerInfo")).text);
		trainerDialogueInfoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "TournamentDialogueInfo")).text);
		_orderedTournamentIDs = new List<string>();
		for (int i = 0; i < infoJSON.Count; i++)
		{
			_orderedTournamentIDs.Add(infoJSON[i]["id"]);
		}
		_orderedTournamentIDs.Sort((string s1, string s2) => TournamentDisplayOrder(s1).CompareTo(TournamentDisplayOrder(s2)));
	}

	public static void Remove()
	{
		infoJSON = null;
		eventInfoJSON = null;
		trainerInfoJSON = null;
		trainerDialogueInfoJSON = null;
		_orderedTournamentIDs = null;
	}

	public static void SetTimestamp(double timestamp)
	{
		App.saveData.tournament.timestamp = timestamp;
	}

	public static IEnumerator GetTimestampCountdownServer(MonoBehaviour owner)
	{
		int urlRandomizer = UnityEngine.Random.Range(0, 999999999);
		CoroutineWithData cd = new CoroutineWithData(owner, TimeServer.GetServerTimeJSONURL(string.Format("http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}", urlRandomizer), "utc_timestamp"));
		yield return cd.coroutine;
		double serverTime = Convert.ToDouble(cd.result);
		double targetTime = App.saveData.tournament.timestamp + 86400.0;
		if (serverTime > -1.0)
		{
			yield return Math.Max(0.0, targetTime - serverTime);
		}
		else
		{
			yield return -1;
		}
	}

	public static double GetTimestampCountdownLocal()
	{
		double localUnixTime = TimeServer.GetLocalUnixTime();
		double num = App.saveData.tournament.timestamp + 86400.0;
		return Math.Max(0.0, num - localUnixTime);
	}

	public static List<string> GetAllTournamentIDs()
	{
		return _orderedTournamentIDs;
	}

	public static string GetNextTournamentID(string tournamentID)
	{
		int num = _orderedTournamentIDs.IndexOf(tournamentID);
		if (num + 1 < _orderedTournamentIDs.Count)
		{
			return _orderedTournamentIDs[num + 1];
		}
		return null;
	}

	public static string GetPreviousTournamentID(string tournamentID)
	{
		int num = _orderedTournamentIDs.IndexOf(tournamentID);
		if (num - 1 > -1)
		{
			return _orderedTournamentIDs[num - 1];
		}
		return null;
	}

	public static TournamentEventData GetEventData(string tournamentID, string eventID, bool forceCreate = false)
	{
		if (forceCreate)
		{
			if (!App.saveData.tournament.eventDatas.ContainsKey(tournamentID))
			{
				App.saveData.tournament.eventDatas.Add(tournamentID, new Dictionary<string, TournamentEventData>());
			}
			if (!App.saveData.tournament.eventDatas[tournamentID].ContainsKey(eventID))
			{
				App.saveData.tournament.eventDatas[tournamentID].Add(eventID, new TournamentEventData());
			}
		}
		else if (!App.saveData.tournament.eventDatas.ContainsKey(tournamentID) || !App.saveData.tournament.eventDatas[tournamentID].ContainsKey(eventID))
		{
			return null;
		}
		return App.saveData.tournament.eventDatas[tournamentID][eventID];
	}

	public static KeyValuePair<string, string> GetNextUncompleteEvent()
	{
		string[] array = null;
		for (int i = 0; i < _orderedTournamentIDs.Count; i++)
		{
			array = TournamentEventIDs(_orderedTournamentIDs[i]);
			for (int j = 0; j < array.Length; j++)
			{
				TournamentEventData eventData = GetEventData(_orderedTournamentIDs[i], array[j], true);
				if (!eventData.isComplete)
				{
					return new KeyValuePair<string, string>(_orderedTournamentIDs[i], array[j]);
				}
			}
		}
		return new KeyValuePair<string, string>(_orderedTournamentIDs[_orderedTournamentIDs.Count - 1], array[array.Length - 1]);
	}

	public static void SetEventComplete(string tournamentID, string eventID)
	{
		TournamentEventData eventData = GetEventData(tournamentID, eventID, true);
		eventData.isComplete = true;
	}

	public static void StartEvent(string tournamentID, string eventID, List<KeyValuePair<MortyData, int>> mortyList, List<KeyValuePair<string, int>> itemList)
	{
		currentTournamentID = tournamentID;
		currentEventID = eventID;
		currentTrainerID = EventTrainerIDs(eventID)[0];
		PlayerSetMorties(mortyList);
		PlayerSetItems(itemList);
	}

	public static void EndEvent()
	{
		currentTournamentID = null;
		currentEventID = null;
		currentTrainerID = null;
		currentMorties = null;
		currentItems = null;
	}

	public static void CompleteEvent()
	{
		SetEventComplete(currentTournamentID, currentEventID);
		App.saveData.Save(eSaveDataType.TOURNAMENT);
		App.upsight.CustomAnalyticEvent(eSubType1.TOURNAMENTS, eSubType2.SUCCESS, currentEventID);
	}

	public static void SetNextTrainer(string trainerID)
	{
		currentTrainerID = trainerID;
	}

	public static string GetNextTrainerID(string eventID, string trainerID)
	{
		string[] array = EventTrainerIDs(eventID);
		if (trainerID == null)
		{
			return array[0];
		}
		int num = Array.IndexOf(array, trainerID);
		if (num + 1 < array.Length)
		{
			return array[num + 1];
		}
		return null;
	}

	public static TournamentTrainerData GetTrainerData(string id)
	{
		return new TournamentTrainerData(id);
	}

	private static DialogueData TrainerDialogueAppended(string id)
	{
		int num = 1;
		string text = id + "_" + num;
		List<DialogueDataEntry> list = new List<DialogueDataEntry>();
		while (trainerDialogueInfoJSON[text] != null)
		{
			string value = trainerDialogueInfoJSON[text]["side"];
			eDirection direction = (string.IsNullOrEmpty(value) ? eDirection.RIGHT : ((eDirection)Enum.Parse(typeof(eDirection), value)));
			string localizedString = LocalizationDefs.GetLocalizedString(eGooglePage.TournamentDialogue, text, "name");
			string localizedString2 = LocalizationDefs.GetLocalizedString(eGooglePage.TournamentDialogue, text, "dialogue");
			list.Add(new DialogueDataEntry(localizedString2, localizedString, trainerDialogueInfoJSON[text]["assetid"], direction));
			num++;
			text = id + "_" + num;
		}
		if (list.Count > 0)
		{
			return new DialogueData(list.ToArray());
		}
		return null;
	}

	private static void PlayerSetMorties(List<KeyValuePair<MortyData, int>> mortyList)
	{
		currentMorties = new List<MortyData>();
		for (int i = 0; i < mortyList.Count; i++)
		{
			currentMorties.Add(mortyList[i].Key);
		}
	}

	private static void PlayerSetItems(List<KeyValuePair<string, int>> itemList)
	{
		currentItems = new Dictionary<string, int>();
		for (int i = 0; i < itemList.Count; i++)
		{
			if (currentItems.ContainsKey(itemList[i].Key))
			{
				currentItems[itemList[i].Key]++;
			}
			else
			{
				currentItems.Add(itemList[i].Key, 1);
			}
		}
	}

	private static int GetBattlePointsForMorty(MortyData data)
	{
		return Mathf.RoundToInt(data.level * MortyDefs.StatTotal(data.id) / 225);
	}

	private static int GetBattlePointsForItem(string id)
	{
		return ItemDefs.BattlePoints(id);
	}

	public static int GetItemStashCurrentCost()
	{
		return 1000;
	}

	public static void SetItemStashActive(bool isActive)
	{
		App.saveData.tournament.dirtyRickActive = isActive;
	}

	public static bool IsItemStashActive()
	{
		return App.saveData.tournament.dirtyRickActive;
	}

	public static string GetNextTicket()
	{
		if (App.saveData.tournament.ownedTickets.Count > 0)
		{
			return App.saveData.tournament.ownedTickets[0];
		}
		return null;
	}

	public static void RemoveTicket(string id)
	{
		if (App.saveData.tournament.ownedTickets.Contains(id))
		{
			App.saveData.tournament.ownedTickets.Remove(id);
		}
	}
}
