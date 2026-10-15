using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[Serializable]
public class PlayerSaveData : SaveDataBase
{
	private const string _kTotalGameTimeKey = "TotalGameTimeKey";

	private const string _kSessionCountKey = "SessionCountKey";

	private const string _kDimensionCountKey = "DimensionCountKey";

	private const string _kAssetIDKey = "AssetIDKey";

	private const string _kPlayerAvatarIDKey = "PlayerAvatarIDKey";

	private const string _kStoriesSeenKey = "StoriesSeenKey";

	private const string _kNotificationAlertIDKey = "NotificationAlertIDKey";

	private const string _kPartyMortiesKey = "PartyMortiesKey";

	private const string _kMortyDecksKey = "MortyDecksKey";

	private const string _kCurrentMortyDeckIndexKey = "CurrentMortyDeckIndexKey";

	private const string _kStorageMortiesKey = "StorageMortiesKey";

	private const string _kSeenMortiesKey = "SeenMortiesKey";

	private const string _kCaughtMortiesKey = "CaughtMortiesKey";

	private const string _kCoinsKey = "CoinsKey";

	private const string _kCouponsKey = "CouponsKey";

	private const string _kItemsKey = "ItemsKey";

	private const string _kBadgesKey = "BadgesKey";

	private const string _kTrophiesKey = "TrophiesKey";

	private const string _kTrainersDefeatedKey = "TrainersDefeatedKey";

	private const string _kCouncilMembersDefeatedKey = "CouncilMembersDefeatedKey";

	private const string _kActiveQuestsKey = "ActiveQuestsKey";

	private const string _kCompleteQuestsKey = "CompleteQuestsKey";

	private const string _kCraftRecipesLearnedKey = "CraftRecipesLearnedKey";

	private const string _kLastGymLeaderIDsKey = "LastGymLeaderIDsKey";

	private const string _kLastSegmentedWorldIDsKey = "LastSegmentedWorldIDsKey";

	private const string _kGachaPullsSinceUpdateKey = "GachaPullsSinceUpdateKey";

	private const string _kShowUpdateStateKey = "ShowUpdateStateKey";

	private const string _kGooglePlaySignInDeclined = "GooglePlaySignInDeclined";

	private const string _kLastNormalWorldIDKey = "LastNormalWorldIDKey";

	private const string _kLastNormalWorldEntranceIDKey = "LastNormalWorldEntranceIDKey";

	private const string _kBootcampMortiesKey = "BootcampMortiesKey";

	private const string _kBootcampTimestampsKey = "BootcampTimestampsKey";

	private const string _kBoughtPlayerAvatarIDsKey = "BoughtPlayerAvatarIDsKey";

	private const string _kInvestmentTimestampsKey = "InvestmentTimestampsKey";

	public float totalGameTime;

	public int sessionCount;

	public int dimensionCount;

	private string assetID;

	public string playerAvatarID;

	public List<string> boughtPlayerAvatarIDs;

	public List<string> storiesSeen;

	public string notificationAlertID;

	public List<string> seenMorties;

	public List<string> caughtMorties;

	public List<MortyData> partyMorties;

	public List<List<string>> mortyDecks;

	public int currentMortyDeckIndex;

	public List<MortyData> storageMorties;

	public int coins;

	public int coupons;

	public Dictionary<string, int> items;

	public List<int> badges;

	public List<string> trophies;

	public int trainersDefeated;

	public int councilMembersDefeated;

	public List<string> activeQuests;

	public List<QuestCompleteData> completedQuests;

	public List<string> craftRecipesLearned;

	public List<string> lastGymLeaderIDs;

	public List<string> lastSegmentedWorldIDs;

	public int gachaPullsSinceUpdate;

	public bool showUpdateState;

	public bool googlePlaySignInDeclined;

	public string lastNormalWorldID;

	public string lastNormalWorldEntranceID;

	public List<List<string>> bootcampMorties;

	public List<double> bootcampTimestamps;

	public List<double> investmentTimestamps;

	protected override int _targetVersionID
	{
		get
		{
			return 10;
		}
	}

	public PlayerSaveData()
	{
	}

	public PlayerSaveData(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	protected override void SetDefaults()
	{
		base.SetDefaults();
		totalGameTime = 0f;
		sessionCount = 0;
		dimensionCount = 0;
		assetID = null;
		playerAvatarID = "AvatarRickDefault";
		storiesSeen = new List<string>();
		notificationAlertID = null;
		seenMorties = new List<string>();
		caughtMorties = new List<string>();
		partyMorties = new List<MortyData>();
		storageMorties = new List<MortyData>();
		currentMortyDeckIndex = 0;
		items = new Dictionary<string, int>();
		badges = new List<int>();
		trophies = new List<string>();
		activeQuests = new List<string>();
		completedQuests = new List<QuestCompleteData>();
		craftRecipesLearned = new List<string>();
		lastGymLeaderIDs = new List<string>();
		lastSegmentedWorldIDs = new List<string>();
		gachaPullsSinceUpdate = 3;
		showUpdateState = false;
		googlePlaySignInDeclined = false;
		lastNormalWorldID = "Citadel";
		lastNormalWorldEntranceID = "CitadelPortal";
		coins = 0;
		SetDecksToDefault();
		SetBootcampsToDefault();
		SetBoughtPlayerAvatarsToDefault();
		SetInvestmentsToDefaults();
	}

	public void SetDecksToDefault()
	{
		mortyDecks = new List<List<string>>();
		while (mortyDecks.Count < 3)
		{
			mortyDecks.Add(new List<string>());
		}
	}

	public void SetBootcampsToDefault()
	{
		bootcampMorties = new List<List<string>>();
		while (bootcampMorties.Count < 3)
		{
			bootcampMorties.Add(new List<string>());
		}
		bootcampTimestamps = new List<double>();
		while (bootcampTimestamps.Count < 3)
		{
			bootcampTimestamps.Add(-1.0);
		}
	}

	public void SetBoughtPlayerAvatarsToDefault()
	{
		boughtPlayerAvatarIDs = new List<string>();
		boughtPlayerAvatarIDs.Add("AvatarRickDefault");
	}

	public void SetInvestmentsToDefaults()
	{
		investmentTimestamps = new List<double>();
		while (investmentTimestamps.Count < 3)
		{
			investmentTimestamps.Add(-1.0);
		}
	}

	protected override void SerializeValues(SerializationInfo info, StreamingContext context)
	{
		base.SerializeValues(info, context);
		info.AddValue("TotalGameTimeKey", totalGameTime, typeof(float));
		info.AddValue("SessionCountKey", sessionCount, typeof(int));
		info.AddValue("DimensionCountKey", dimensionCount, typeof(int));
		info.AddValue("AssetIDKey", assetID, typeof(string));
		info.AddValue("PlayerAvatarIDKey", playerAvatarID, typeof(string));
		info.AddValue("BoughtPlayerAvatarIDsKey", boughtPlayerAvatarIDs, typeof(List<string>));
		info.AddValue("StoriesSeenKey", storiesSeen, typeof(List<string>));
		info.AddValue("NotificationAlertIDKey", notificationAlertID, typeof(string));
		info.AddValue("SeenMortiesKey", seenMorties, typeof(List<string>));
		info.AddValue("CaughtMortiesKey", caughtMorties, typeof(List<string>));
		info.AddValue("PartyMortiesKey", partyMorties, typeof(List<MortyData>));
		info.AddValue("MortyDecksKey", mortyDecks, typeof(List<List<string>>));
		info.AddValue("CurrentMortyDeckIndexKey", currentMortyDeckIndex, typeof(int));
		info.AddValue("StorageMortiesKey", storageMorties, typeof(List<MortyData>));
		info.AddValue("CoinsKey", coins, typeof(int));
		info.AddValue("CouponsKey", coupons, typeof(int));
		info.AddValue("ItemsKey", items, typeof(Dictionary<string, int>));
		info.AddValue("BadgesKey", badges, typeof(List<int>));
		info.AddValue("TrophiesKey", trophies, typeof(List<string>));
		info.AddValue("TrainersDefeatedKey", trainersDefeated, typeof(int));
		info.AddValue("CouncilMembersDefeatedKey", councilMembersDefeated, typeof(int));
		info.AddValue("ActiveQuestsKey", activeQuests, typeof(List<string>));
		info.AddValue("CompleteQuestsKey", completedQuests, typeof(List<QuestCompleteData>));
		info.AddValue("CraftRecipesLearnedKey", craftRecipesLearned, typeof(List<string>));
		info.AddValue("LastGymLeaderIDsKey", lastGymLeaderIDs, typeof(List<string>));
		info.AddValue("LastSegmentedWorldIDsKey", lastSegmentedWorldIDs, typeof(List<string>));
		info.AddValue("GachaPullsSinceUpdateKey", gachaPullsSinceUpdate, typeof(int));
		info.AddValue("ShowUpdateStateKey", showUpdateState, typeof(bool));
		info.AddValue("GooglePlaySignInDeclined", googlePlaySignInDeclined, typeof(bool));
		info.AddValue("LastNormalWorldIDKey", lastNormalWorldID, typeof(string));
		info.AddValue("LastNormalWorldEntranceIDKey", lastNormalWorldEntranceID, typeof(string));
		info.AddValue("BootcampMortiesKey", bootcampMorties, typeof(List<string>));
		info.AddValue("BootcampTimestampsKey", bootcampTimestamps, typeof(List<double>));
		info.AddValue("InvestmentTimestampsKey", investmentTimestamps, typeof(List<double>));
	}

	protected override void DeserializeValues(SerializationInfo info, StreamingContext context)
	{
		base.DeserializeValues(info, context);
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "TotalGameTimeKey":
				totalGameTime = (float)current.Value;
				break;
			case "SessionCountKey":
				sessionCount = (int)current.Value;
				break;
			case "DimensionCountKey":
				dimensionCount = (int)current.Value;
				break;
			case "AssetIDKey":
				assetID = (string)current.Value;
				break;
			case "PlayerAvatarIDKey":
				playerAvatarID = (string)current.Value;
				break;
			case "BoughtPlayerAvatarIDsKey":
				boughtPlayerAvatarIDs = (List<string>)current.Value;
				break;
			case "StoriesSeenKey":
				storiesSeen = (List<string>)current.Value;
				break;
			case "NotificationAlertIDKey":
				notificationAlertID = (string)current.Value;
				break;
			case "SeenMortiesKey":
				seenMorties = (List<string>)current.Value;
				break;
			case "CaughtMortiesKey":
				caughtMorties = (List<string>)current.Value;
				break;
			case "PartyMortiesKey":
				partyMorties = (List<MortyData>)current.Value;
				break;
			case "MortyDecksKey":
				mortyDecks = (List<List<string>>)current.Value;
				break;
			case "CurrentMortyDeckIndexKey":
				currentMortyDeckIndex = (int)current.Value;
				break;
			case "StorageMortiesKey":
				storageMorties = (List<MortyData>)current.Value;
				break;
			case "CoinsKey":
				coins = (int)current.Value;
				break;
			case "CouponsKey":
				coupons = (int)current.Value;
				break;
			case "ItemsKey":
				items = (Dictionary<string, int>)current.Value;
				break;
			case "BadgesKey":
				badges = (List<int>)current.Value;
				break;
			case "TrophiesKey":
				trophies = (List<string>)current.Value;
				break;
			case "TrainersDefeatedKey":
				trainersDefeated = (int)current.Value;
				break;
			case "CouncilMembersDefeatedKey":
				councilMembersDefeated = (int)current.Value;
				break;
			case "ActiveQuestsKey":
				activeQuests = (List<string>)current.Value;
				break;
			case "CompleteQuestsKey":
				completedQuests = (List<QuestCompleteData>)current.Value;
				break;
			case "CraftRecipesLearnedKey":
				craftRecipesLearned = (List<string>)current.Value;
				break;
			case "LastGymLeaderIDsKey":
				lastGymLeaderIDs = (List<string>)current.Value;
				break;
			case "LastSegmentedWorldIDsKey":
				lastSegmentedWorldIDs = (List<string>)current.Value;
				break;
			case "GachaPullsSinceUpdateKey":
				gachaPullsSinceUpdate = (int)current.Value;
				break;
			case "ShowUpdateStateKey":
				showUpdateState = (bool)current.Value;
				break;
			case "GooglePlaySignInDeclined":
				googlePlaySignInDeclined = (bool)current.Value;
				break;
			case "LastNormalWorldIDKey":
				lastNormalWorldID = (string)current.Value;
				break;
			case "LastNormalWorldEntranceIDKey":
				lastNormalWorldEntranceID = (string)current.Value;
				break;
			case "BootcampMortiesKey":
				bootcampMorties = (List<List<string>>)current.Value;
				break;
			case "BootcampTimestampsKey":
				bootcampTimestamps = (List<double>)current.Value;
				break;
			case "InvestmentTimestampsKey":
				investmentTimestamps = (List<double>)current.Value;
				break;
			}
		}
	}

	protected override void UpdateToVersion2(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
	}

	protected override void UpdateToVersion3(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
		for (int i = 0; i < partyMorties.Count; i++)
		{
			for (int j = partyMorties[i].attackIDs.Count; j < 4; j++)
			{
				partyMorties[i].attackIDs.Add(null);
				partyMorties[i].attackPPs.Add(-1);
			}
		}
		for (int k = 0; k < storageMorties.Count; k++)
		{
			for (int l = storageMorties[k].attackIDs.Count; l < 4; l++)
			{
				storageMorties[k].attackIDs.Add(null);
				storageMorties[k].attackPPs.Add(-1);
			}
		}
	}

	protected override void UpdateToVersion4(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
	}

	protected override void UpdateToVersion5(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
		List<MortyData> list = new List<MortyData>();
		list.AddRange(partyMorties);
		list.AddRange(storageMorties);
		foreach (MortyData item in list)
		{
			item.GetGuid();
		}
		List<string> list2 = new List<string>();
		if (partyMorties != null)
		{
			foreach (MortyData partyMorty in partyMorties)
			{
				list2.Add(partyMorty.GetGuid());
			}
		}
		mortyDecks[0] = list2;
		for (int i = 1; i < mortyDecks.Count; i++)
		{
			List<string> list3 = new List<string>();
			if (partyMorties != null && partyMorties.Count > 0)
			{
				list3.Add(partyMorties.First((MortyData m) => m.id == "MortyDefault").GetGuid());
			}
			mortyDecks[i] = list3;
		}
	}

	protected override void UpdateToVersion6(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
		playerAvatarID = ((!(assetID == "RickSashAndCape")) ? "AvatarRickDefault" : "AvatarRickSashAndCape");
	}

	protected override void UpdateToVersion7(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
		List<MortyData> list = new List<MortyData>();
		list.AddRange(partyMorties);
		list.AddRange(storageMorties);
		for (int i = 0; i < bootcampMorties.Count; i++)
		{
			for (int num = bootcampMorties[i].Count - 1; num >= 0; num--)
			{
				string guid = bootcampMorties[i][num];
				if (!string.IsNullOrEmpty(guid) && list.Find((MortyData m) => m.GetGuid().Equals(guid)) == null)
				{
					bootcampMorties[i].RemoveAt(num);
				}
			}
		}
	}

	protected override void UpdateToVersion8(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
	}

	protected override void UpdateToVersion9(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
	}

	protected override void UpdateToVersion10(SerializationInfo info)
	{
		gachaPullsSinceUpdate = 0;
		showUpdateState = true;
	}
}
