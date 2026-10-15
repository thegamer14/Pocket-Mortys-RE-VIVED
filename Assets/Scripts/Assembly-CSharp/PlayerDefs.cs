using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDefs
{
	public delegate void PlayerDefsNotificationDelegate();

	public delegate void PlayerDefsCouponDelegate(int count);

	public delegate void PlayerDefsCoinDelegate(int count);

	public delegate void PlayerDefsPartyMortiesChangedDelegate(List<MortyData> partyMorties);

	public const string kRickDefaultAvatarID = "AvatarRickDefault";

	public const string kRickWinnerRickAvatarID = "AvatarRickSashAndCape";

	public const int kMaxMortiesInParty = 5;

	public const int kMaxMortiesInStorage = 300;

	public const int kDefaultMortyStartingLevel = 5;

	public const int kMaxMortyDecks = 3;

	private const int _kMortyCoinValue = 5000;

	public const int kMaxIndividualItemCountInBag = 10;

	private const int _kMaxCoins = 99999999;

	public const int kMaxCoupons = 99999;

	private const int _kMaxBadges = 99999999;

	public const int kMaxTrainersDefeated = 99999;

	public const int kMaxCouncilMembers = 6;

	public const int kMaxActiveQuests = 3;

	private const int _kGymLeaderIDRecycleCount = 5;

	private const int _kWorldIDRecycleCount = 5;

	public static List<KeyValuePair<string, eNotificationType>> notificationsWaitingToDisplay = new List<KeyValuePair<string, eNotificationType>>();

	public static string AssetID
	{
		get
		{
			return PlayerAvatarDefs.AssetID(AvatarID);
		}
	}

	public static string AvatarID
	{
		get
		{
			return App.saveData.player.playerAvatarID;
		}
		set
		{
			App.saveData.player.playerAvatarID = value;
		}
	}

	public static string AvatarCategory
	{
		get
		{
			return PlayerAvatarDefs.Category(AvatarID);
		}
	}

	public static float TotalGameTime
	{
		get
		{
			return App.saveData.player.totalGameTime;
		}
		set
		{
			App.saveData.player.totalGameTime = value;
		}
	}

	public static int SessionCount
	{
		get
		{
			return App.saveData.player.sessionCount;
		}
		set
		{
			App.saveData.player.sessionCount = value;
		}
	}

	public static int DimensionCount
	{
		get
		{
			return App.saveData.player.dimensionCount;
		}
		set
		{
			App.saveData.player.dimensionCount = value;
		}
	}

	public static string lastNormalWorldID
	{
		get
		{
			return App.saveData.player.lastNormalWorldID;
		}
	}

	public static string lastNormalWorldEntranceID
	{
		get
		{
			return App.saveData.player.lastNormalWorldEntranceID;
		}
	}

	public static string CurrentAlertNotificationID
	{
		get
		{
			return App.saveData.player.notificationAlertID;
		}
	}

	public static int CurrentPartyCount
	{
		get
		{
			return CurrentParty().Count;
		}
	}

	public static int CurrentTotalMortyCount
	{
		get
		{
			return CurrentAllMorties().Count;
		}
	}

	public static int PlayerPartyCount
	{
		get
		{
			return App.saveData.player.partyMorties.Count;
		}
	}

	public static int PlayerStorageCount
	{
		get
		{
			return App.saveData.player.storageMorties.Count;
		}
	}

	public static int PlayerTotalMortyCount
	{
		get
		{
			return PlayerPartyCount + PlayerStorageCount;
		}
	}

	public static int MortySeenCount
	{
		get
		{
			return App.saveData.player.seenMorties.Count;
		}
	}

	public static int MortyCaughtCount
	{
		get
		{
			return App.saveData.player.caughtMorties.Count;
		}
	}

	public static int CurrentItemCount
	{
		get
		{
			return CurrentItems().Count;
		}
	}

	public static int playerItemCount
	{
		get
		{
			return App.saveData.player.items.Count;
		}
	}

	public static Dictionary<string, int> playerItems
	{
		get
		{
			return App.saveData.player.items;
		}
	}

	public static int CoinCount
	{
		get
		{
			return App.saveData.player.coins;
		}
	}

	public static int CouponCount
	{
		get
		{
			return App.saveData.player.coupons;
		}
	}

	public static int BadgeCount
	{
		get
		{
			return (TestDefs.BADGES_COUNT_OVERRIDE <= -1) ? App.saveData.player.badges.Count : TestDefs.BADGES_COUNT_OVERRIDE;
		}
	}

	public static int NextCouncilMemberBadgeRequirement
	{
		get
		{
			return TrainerDefs.GetCouncilMemberBadgeRequirement(NextCouncilMember);
		}
	}

	public static int CurrentBadgesRequired
	{
		get
		{
			return NextCouncilMemberBadgeRequirement - BadgeCount;
		}
	}

	public static int TrophyCount
	{
		get
		{
			return App.saveData.player.trophies.Count;
		}
	}

	public static int DefeatedTrainers
	{
		get
		{
			return (TestDefs.TRAINERS_DEFEATED_OVERRIDE <= -1) ? App.saveData.player.trainersDefeated : TestDefs.TRAINERS_DEFEATED_OVERRIDE;
		}
	}

	public static int DefeatedCouncilMembers
	{
		get
		{
			return (TestDefs.COUNCIL_MEMBERS_DEFEATED_OVERRIDE <= -1) ? App.saveData.player.councilMembersDefeated : TestDefs.COUNCIL_MEMBERS_DEFEATED_OVERRIDE;
		}
	}

	public static int NextCouncilMember
	{
		get
		{
			return DefeatedCouncilMembers + 1;
		}
	}

	public static string NextCouncilMemberID
	{
		get
		{
			return string.Format("TrainerCouncil{0}", NextCouncilMember);
		}
	}

	public static int GetActiveQuestCount
	{
		get
		{
			return App.saveData.player.activeQuests.Count;
		}
	}

	public static int GetCompleteQuestCount
	{
		get
		{
			return App.saveData.player.completedQuests.Count;
		}
	}

	public static List<string> lastGymLeaderIDs
	{
		get
		{
			return App.saveData.player.lastGymLeaderIDs;
		}
	}

	public static List<string> lastSegmentedWorldIDs
	{
		get
		{
			return App.saveData.player.lastSegmentedWorldIDs;
		}
	}

	public static event PlayerDefsNotificationDelegate NewNotificationSetEvent;

	public static event PlayerDefsCouponDelegate CouponCountChangedEvent;

	public static event PlayerDefsCoinDelegate CoinCountChangedEvent;

	public static event PlayerDefsPartyMortiesChangedDelegate PartyMortiesChangedEvent;

	public static eLocationType GetLocationType()
	{
		if (TournamentDefs.currentTournamentID != null)
		{
			return eLocationType.TOURNAMENT_ARENA;
		}
		return eLocationType.DEFAULT;
	}

	public static void SetNormalWorldLocation(string worldID, string entranceID)
	{
		App.saveData.player.lastNormalWorldID = worldID;
		App.saveData.player.lastNormalWorldEntranceID = entranceID;
	}

	public static void SetStorySeen(string id)
	{
		if (!App.saveData.player.storiesSeen.Contains(id))
		{
			App.saveData.player.storiesSeen.Add(id);
		}
	}

	public static bool GetStorySeen(string id)
	{
		return App.saveData.player.storiesSeen.Contains(id);
	}

	public static void DisplayNotification(eNotificationType type, string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			if (type == eNotificationType.ALERT)
			{
				App.saveData.player.notificationAlertID = id;
				App.saveData.Save(eSaveDataType.PLAYER);
			}
			AddNotificationToWaitingList(type, id);
			if (PlayerDefs.NewNotificationSetEvent != null)
			{
				PlayerDefs.NewNotificationSetEvent();
			}
		}
	}

	public static void ReDisplayCurrentAlertNotification()
	{
		string notificationAlertID = App.saveData.player.notificationAlertID;
		if (!string.IsNullOrEmpty(notificationAlertID))
		{
			AddNotificationToWaitingList(eNotificationType.ALERT, notificationAlertID);
			if (PlayerDefs.NewNotificationSetEvent != null)
			{
				PlayerDefs.NewNotificationSetEvent();
			}
		}
	}

	private static void AddNotificationToWaitingList(eNotificationType type, string id)
	{
		notificationsWaitingToDisplay.Add(new KeyValuePair<string, eNotificationType>(id, type));
	}

	public static void RemoveAllNotificationsFromWaitingList()
	{
		notificationsWaitingToDisplay.Clear();
	}

	public static void CheckForNextAlert()
	{
		switch (App.saveData.player.notificationAlertID)
		{
		case "AlertTutorialGoThroughPortalIDKey":
			if (App.states.Get<WorldState>().IsActive() && SharedStatInfo.worldInfo.worldID.Equals("Citadel"))
			{
				DisplayNotification(eNotificationType.ALERT, "AlertCitadelFirstTimeID");
			}
			break;
		case "AlertCitadelFirstTimeID":
			if (App.states.Get<WorldState>().IsActive() && WorldDefs.IsSegmented(SharedStatInfo.worldInfo.worldID))
			{
				DisplayNotification(eNotificationType.ALERT, "AlertFirstSegmentedWorldID");
			}
			break;
		case "AlertFirstSegmentedWorldID":
			if (SharedStatInfo.battleInfo != null && SharedStatInfo.battleInfo.enemy is BattleOpponentAI && ((BattleOpponentAI)SharedStatInfo.battleInfo.enemy).trainerType == eTrainerType.GYM_LEADER && SharedStatInfo.battleInfo.outcomeType == eBattleOutcomeType.PLAYER_WON)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertSegmentedFirstCouncilMemberID");
			}
			break;
		case "AlertSegmentedFirstCouncilMemberID":
			if (App.states.Get<WorldState>().IsActive() && SharedStatInfo.worldInfo.worldID.Equals("Citadel"))
			{
				DisplayNotification(eNotificationType.ALERT, "AlertCitadelFirstCouncilMemberID");
			}
			break;
		case "AlertCitadelFirstCouncilMemberID":
			if (BadgeCount >= NextCouncilMemberBadgeRequirement)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertCouncilMemberReadyID");
			}
			else if (SharedStatInfo.battleInfo != null && SharedStatInfo.battleInfo.enemy is BattleOpponentAI && ((BattleOpponentAI)SharedStatInfo.battleInfo.enemy).trainerType == eTrainerType.COUNCIL_MEMBER && SharedStatInfo.battleInfo.outcomeType == eBattleOutcomeType.PLAYER_WON)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertBadgesRequiredID");
			}
			break;
		case "AlertBadgesRequiredID":
			if (BadgeCount >= NextCouncilMemberBadgeRequirement)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertCouncilMemberReadyID");
			}
			else if (SharedStatInfo.battleInfo != null && SharedStatInfo.battleInfo.enemy is BattleOpponentAI && ((BattleOpponentAI)SharedStatInfo.battleInfo.enemy).trainerType == eTrainerType.GYM_LEADER && SharedStatInfo.battleInfo.outcomeType == eBattleOutcomeType.PLAYER_WON)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertBadgesRequiredID");
			}
			break;
		case "AlertCouncilMemberReadyID":
			if (IsCouncilStoryComplete())
			{
				DisplayNotification(eNotificationType.ALERT, "AlertEndGameID");
			}
			else if (BadgeCount < NextCouncilMemberBadgeRequirement)
			{
				DisplayNotification(eNotificationType.ALERT, "AlertBadgesRequiredID");
			}
			break;
		}
	}

	public static List<MortyData> CurrentParty()
	{
		eLocationType locationType = GetLocationType();
		if (locationType != eLocationType.TOURNAMENT_ARENA)
		{
			return App.saveData.player.partyMorties;
		}
		return TournamentDefs.currentMorties;
	}

	public static MortyData CurrentPartyGetMorty(int index)
	{
		if (index > -1 && index < CurrentPartyCount)
		{
			return CurrentParty()[index];
		}
		return null;
	}

	public static List<MortyData> CurrentAllMorties()
	{
		eLocationType locationType = GetLocationType();
		if (locationType != eLocationType.TOURNAMENT_ARENA)
		{
			return GetAllPlayerMorties();
		}
		return TournamentDefs.currentMorties;
	}

	public static void CurrentPartyReviveAllToFull()
	{
		List<MortyData> list = CurrentParty();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].ReviveFull();
		}
	}

	public static int GetPartyHPToRegain()
	{
		int num = 0;
		List<MortyData> list = CurrentParty();
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].hpStat - list[i].hp;
		}
		return num;
	}

	public static List<MortyData> GetMortiesForDeck(int deckIndex)
	{
		List<MortyData> allPlayerMorties = GetAllPlayerMorties();
		List<MortyData> list = new List<MortyData>();
		foreach (string guid in App.saveData.player.mortyDecks[deckIndex])
		{
			list.Add(allPlayerMorties.Find((MortyData m) => m.GetGuid().Equals(guid)));
		}
		return list;
	}

	public static int GetAliveMortiesCountForDeck(int deckIndex)
	{
		int num = 0;
		List<MortyData> mortiesForDeck = GetMortiesForDeck(deckIndex);
		for (int i = 0; i < mortiesForDeck.Count; i++)
		{
			if (!mortiesForDeck[i].isFainted)
			{
				num++;
			}
		}
		return num;
	}

	public static int DeckCount()
	{
		return App.saveData.player.mortyDecks.Count;
	}

	public static int CurrentDeckIndex()
	{
		return App.saveData.player.currentMortyDeckIndex;
	}

	public static bool AddMortyToDeck(MortyData mortyData, int deckIndex, out string failMessage)
	{
		failMessage = null;
		if (CurrentDeckIndex() == deckIndex && BootcampDefs.IsMortyOnBootcamp(mortyData))
		{
			failMessage = string.Format(TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_WITHDRAW_FAIL_BOOTCAMP, MortyDefs.Name(mortyData.id));
			return false;
		}
		if (GetMortiesForDeck(deckIndex).Count() >= 5)
		{
			failMessage = TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_WITHDRAW_FAIL_FULL;
			return false;
		}
		App.saveData.player.mortyDecks[deckIndex].Add(mortyData.GetGuid());
		if (deckIndex == CurrentDeckIndex())
		{
			string failMessage2;
			SetPartyMortiesFromDeck(deckIndex, out failMessage2);
		}
		return true;
	}

	public static bool RemoveMortyFromDeck(MortyData mortyData, int deckIndex, out string failMessage)
	{
		failMessage = null;
		if (mortyData.id.Equals("MortyDefault") && GetMortiesForDeck(deckIndex).FindAll((MortyData e) => e.id == "MortyDefault").Count <= 1)
		{
			failMessage = TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_DEFAULT_M;
			return false;
		}
		if (GetMortiesForDeck(deckIndex).Count <= 1)
		{
			failMessage = TextDefs.MORTY_STORAGE_EDIT_DECK_DIAL_DEPOSIT_FAIL;
			return false;
		}
		if (CurrentDeckIndex() == deckIndex && GetAliveMortiesCountForDeck(deckIndex) <= 1 && !mortyData.isFainted)
		{
			failMessage = TextDefs.MORTY_STORAGE_DECKS_DIAL_EDIT_FAIL_FAINTED;
			return false;
		}
		App.saveData.player.mortyDecks[deckIndex].Remove(mortyData.GetGuid());
		if (deckIndex == CurrentDeckIndex())
		{
			string failMessage2;
			SetPartyMortiesFromDeck(deckIndex, out failMessage2);
		}
		return true;
	}

	public static bool SetPartyMortiesFromDeck(int deckIndex, out string failMessage)
	{
		failMessage = null;
		if (BootcampDefs.AreAnyMortiesOnBootamp(GetMortiesForDeck(deckIndex)))
		{
			failMessage = TextDefs.MORTY_STORAGE_DECKS_DIAL_WITHDRAWN_FAIL_BOOTCAMP;
			return false;
		}
		if (GetAliveMortiesCountForDeck(deckIndex) <= 0)
		{
			failMessage = TextDefs.MORTY_STORAGE_DECKS_DIAL_WITHDRAW_FAIL_FAINTED;
			return false;
		}
		List<MortyData> allPlayerMorties = GetAllPlayerMorties();
		List<MortyData> mortiesForDeck = GetMortiesForDeck(deckIndex);
		App.saveData.player.currentMortyDeckIndex = deckIndex;
		App.saveData.player.partyMorties = mortiesForDeck;
		App.saveData.player.storageMorties = allPlayerMorties.Except(mortiesForDeck).ToList();
		if (PlayerDefs.PartyMortiesChangedEvent != null)
		{
			PlayerDefs.PartyMortiesChangedEvent(App.saveData.player.partyMorties);
		}
		return true;
	}

	public static MortyData PlayerPartyGetMorty(int index)
	{
		if (index > -1 && index < PlayerPartyCount)
		{
			return App.saveData.player.partyMorties[index];
		}
		return null;
	}

	public static MortyData PlayerStorageGetMorty(int index)
	{
		if (index > -1 && index < PlayerStorageCount)
		{
			return App.saveData.player.storageMorties[index];
		}
		return null;
	}

	public static bool DoesPlayerOwnMortyOfID(string id)
	{
		return App.saveData.player.partyMorties.Count((MortyData e) => e.id.Equals(id)) > 0 || App.saveData.player.storageMorties.Count((MortyData e) => e.id.Equals(id)) > 0;
	}

	public static bool IsMoryInParty(MortyData data)
	{
		if (data == null)
		{
			return false;
		}
		return App.saveData.player.partyMorties.Contains(data);
	}

	public static bool AreAnyMortiesInParty(List<MortyData> morties)
	{
		return CurrentParty().Any((MortyData x) => morties.Any((MortyData y) => y.Equals(x)));
	}

	public static List<MortyData> GetAllPlayerMorties()
	{
		List<MortyData> list = new List<MortyData>();
		list.AddRange(App.saveData.player.partyMorties);
		list.AddRange(App.saveData.player.storageMorties);
		return list;
	}

	public static bool HasSeenMorty(string id)
	{
		return App.saveData.player.seenMorties.Contains(id);
	}

	public static bool HasCaughtMorty(string id)
	{
		return App.saveData.player.caughtMorties.Contains(id);
	}

	public static void SetMortySeen(string mortyID)
	{
		if (!App.saveData.player.seenMorties.Contains(mortyID))
		{
			App.saveData.player.seenMorties.Add(mortyID);
		}
	}

	public static bool SetMortyCaught(MortyData mortyData, out string resultText)
	{
		SetMortySeen(mortyData.id);
		if (App.saveData.player.partyMorties.Count < 5)
		{
			App.saveData.player.partyMorties.Add(mortyData);
			resultText = string.Format(TextDefs.MORTY_CAPTURED_PARTY, MortyDefs.Name(mortyData.id));
			string failMessage = null;
			if (mortyData.id.Equals("MortyDefault"))
			{
				for (int i = 0; i < DeckCount(); i++)
				{
					AddMortyToDeck(mortyData, i, out failMessage);
				}
			}
			else
			{
				AddMortyToDeck(mortyData, CurrentDeckIndex(), out failMessage);
			}
		}
		else
		{
			if (App.saveData.player.storageMorties.Count >= 300)
			{
				resultText = string.Format(TextDefs.MORTY_CAPTURED_PARTY_AND_STORAGE_FULL, MortyDefs.Name(mortyData.id));
				return false;
			}
			App.saveData.player.storageMorties.Add(mortyData);
			resultText = string.Format(TextDefs.MORTY_CAPTURED_STORAGE, MortyDefs.Name(mortyData.id));
		}
		if (!HasCaughtMorty(mortyData.id))
		{
			App.saveData.player.caughtMorties.Add(mortyData.id);
		}
		return true;
	}

	public static void RemoveOwnedMorty(MortyData mortyData)
	{
		for (int i = 0; i < DeckCount(); i++)
		{
			string failMessage;
			if (!GetMortiesForDeck(i).Contains(mortyData) || !RemoveMortyFromDeck(mortyData, i, out failMessage))
			{
			}
		}
		BootcampDefs.RemoveMortyFromAllBootcamps(mortyData);
		if (App.saveData.player.storageMorties.Contains(mortyData))
		{
			App.saveData.player.storageMorties.Remove(mortyData);
		}
	}

	public static bool TryReleaseMorty(MortyData mortyData, out string releaseFailMessage)
	{
		releaseFailMessage = null;
		if (mortyData == null)
		{
			releaseFailMessage = "mortyData is null.";
			return false;
		}
		if (GetLocationType() == eLocationType.TOURNAMENT_ARENA)
		{
			releaseFailMessage = TextDefs.MORTY_STATS_RELEASE_FAIL_TOURNAMENT;
			return false;
		}
		if (BootcampDefs.IsMortyOnBootcamp(mortyData))
		{
			releaseFailMessage = TextDefs.MORTY_STATS_RELEASE_FAIL_BOOTCAMP;
			return false;
		}
		for (int i = 0; i < DeckCount(); i++)
		{
			List<MortyData> mortiesForDeck = GetMortiesForDeck(i);
			if (mortiesForDeck.Contains(mortyData))
			{
				if (mortiesForDeck.Count == 1)
				{
					releaseFailMessage = TextDefs.MORTY_STATS_RELEASE_FAIL_LAST_MORTY;
					return false;
				}
				if (mortyData.id.Equals("MortyDefault") && mortiesForDeck.FindAll((MortyData e) => e.id == "MortyDefault").Count <= 1)
				{
					releaseFailMessage = TextDefs.MORTY_STATS_RELEASE_FAIL_ORIGINAL_MORTY;
					return false;
				}
				if (CurrentDeckIndex() == i && GetAliveMortiesCountForDeck(i) <= 1 && !mortyData.isFainted)
				{
					releaseFailMessage = TextDefs.MORTY_STATS_RELEASE_FAIL_LAST_ALIVE_MORTY;
					return false;
				}
				string failMessage = null;
				if (!RemoveMortyFromDeck(mortyData, i, out failMessage))
				{
					releaseFailMessage = failMessage;
					return false;
				}
			}
		}
		RemoveOwnedMorty(mortyData);
		return true;
	}

	public static MortyData CombineMorties(MortyData mortyData1, MortyData mortyData2, out string resultsText)
	{
		if (mortyData1.id != mortyData2.id)
		{
		}
		string text = MortyDefs.Evolution(mortyData1.id);
		if (text == null)
		{
			resultsText = string.Format(TextDefs.MORTY_STORAGE_COMBINE_DIAL_COMBINE_FAIL, MortyDefs.Name(mortyData1.id));
			return null;
		}
		if (BootcampDefs.IsMortyOnBootcamp(mortyData1) || BootcampDefs.IsMortyOnBootcamp(mortyData2))
		{
			resultsText = TextDefs.MORTY_STORAGE_COMBINE_DIAL_COMBINE_FAIL_BOOTCAMP;
			return null;
		}
		RemoveOwnedMorty(mortyData1);
		RemoveOwnedMorty(mortyData2);
		MortyData mortyData3 = ((mortyData1.level <= mortyData2.level) ? mortyData2 : mortyData1);
		MortyData mortyData4 = ((mortyData1.level >= mortyData2.level) ? mortyData2 : mortyData1);
		int level = mortyData3.level + Mathf.FloorToInt(mortyData4.level / 2);
		MortyData newMortyData = MortyDefs.GetNewMortyData(text, level, eIVSetUp.RANDOM, eEVSetUp.NONE);
		newMortyData.GenerateIVsFromCombine(mortyData3, mortyData4);
		newMortyData.GenerateEVsFromCombine(mortyData3, mortyData4);
		newMortyData.SetLevel(level);
		newMortyData.ReviveFull();
		SetMortyCaught(newMortyData, out resultsText);
		return newMortyData;
	}

	public static int GetMortyCombineCountForID(string id)
	{
		List<MortyData> allPlayerMorties = GetAllPlayerMorties();
		return Mathf.FloorToInt(allPlayerMorties.Count((MortyData e) => e.id.Equals(id)) / 2);
	}

	public static int GetMortyCombineCount()
	{
		List<MortyData> allPlayerMorties = GetAllPlayerMorties();
		int num = 0;
		for (int i = 0; i < MortyDefs.idCount; i++)
		{
			string id = MortyDefs.GetId(i);
			if (!string.IsNullOrEmpty(MortyDefs.Evolution(id)))
			{
				num += Mathf.FloorToInt(allPlayerMorties.FindAll((MortyData e) => e.id.Equals(id)).Count / 2);
			}
		}
		return num;
	}

	public static Dictionary<string, int> CurrentItems()
	{
		eLocationType locationType = GetLocationType();
		if (locationType != eLocationType.TOURNAMENT_ARENA)
		{
			return App.saveData.player.items;
		}
		return TournamentDefs.currentItems;
	}

	public static bool IsAvatarOwned(string avatarID)
	{
		int num = PlayerAvatarDefs.CouncilRequirement(avatarID);
		if (num >= 0)
		{
			return DefeatedCouncilMembers >= num;
		}
		return App.saveData.player.boughtPlayerAvatarIDs.Contains(avatarID);
	}

	public static void SetAvatarOwned(string avatarID)
	{
		if (!App.saveData.player.boughtPlayerAvatarIDs.Contains(avatarID))
		{
			App.saveData.player.boughtPlayerAvatarIDs.Add(avatarID);
		}
	}

	public static bool AddAvatarsOwned(List<string> mpAvatars)
	{
		bool result = false;
		if (mpAvatars == null || mpAvatars.Count == 0)
		{
			return result;
		}
		List<string> allSPAvatars = PlayerAvatarDefs.GetAvatarIDs();
		mpAvatars.RemoveAll((string id) => !allSPAvatars.Contains(id) || App.saveData.player.boughtPlayerAvatarIDs.Contains(id));
		if (mpAvatars.Count > 0)
		{
			result = true;
			App.saveData.player.boughtPlayerAvatarIDs.AddRange(mpAvatars);
		}
		return result;
	}

	public static bool PurchaseAvatar(string avatarID, out string resultText)
	{
		int num = PlayerAvatarDefs.Cost(avatarID);
		eCurrencyType eCurrencyType2 = PlayerAvatarDefs.CurrencyType(avatarID);
		resultText = null;
		if ((eCurrencyType2 == eCurrencyType.COIN && App.saveData.player.coins >= num) || (eCurrencyType2 == eCurrencyType.COUPON && App.saveData.player.coupons >= num))
		{
			switch (eCurrencyType2)
			{
			case eCurrencyType.COIN:
				RemoveCoin(num);
				break;
			case eCurrencyType.COUPON:
				RemoveCoupon(num);
				break;
			}
			SetAvatarOwned(avatarID);
			resultText = string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_BOUGHT, PlayerAvatarDefs.Name(avatarID));
			return true;
		}
		switch (eCurrencyType2)
		{
		case eCurrencyType.COIN:
			resultText = string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_NO_COIN, PlayerAvatarDefs.Name(avatarID));
			break;
		case eCurrencyType.COUPON:
			resultText = string.Format(TextDefs.SALESMAN_RICK_CUSTOMIZE_NO_COUPON, PlayerAvatarDefs.Name(avatarID));
			break;
		}
		return false;
	}

	public static bool AddCoin(int amount, out int resultAmount, out string resultText)
	{
		resultAmount = Mathf.Min(99999999 - App.saveData.player.coins, amount);
		App.saveData.player.coins += resultAmount;
		if (PlayerDefs.CoinCountChangedEvent != null)
		{
			PlayerDefs.CoinCountChangedEvent(App.saveData.player.coins);
		}
		if (resultAmount < amount)
		{
			resultText = TextDefs.COINS_MAX;
			return false;
		}
		resultText = null;
		return true;
	}

	public static void RemoveCoin(int amount)
	{
		App.saveData.player.coins = Mathf.Max(0, App.saveData.player.coins - amount);
		if (PlayerDefs.CoinCountChangedEvent != null)
		{
			PlayerDefs.CoinCountChangedEvent(App.saveData.player.coins);
		}
	}

	public static bool AddCoupon(int amount, out int resultAmount, out string resultText)
	{
		resultAmount = Mathf.Min(99999 - App.saveData.player.coupons, amount);
		App.saveData.player.coupons += resultAmount;
		if (PlayerDefs.CouponCountChangedEvent != null)
		{
			PlayerDefs.CouponCountChangedEvent(App.saveData.player.coupons);
		}
		if (resultAmount != amount)
		{
			resultText = TextDefs.COUPONS_MAX;
			return false;
		}
		resultText = null;
		return true;
	}

	public static void RemoveCoupon(int amount)
	{
		App.saveData.player.coupons = Mathf.Max(0, App.saveData.player.coupons - amount);
		if (PlayerDefs.CouponCountChangedEvent != null)
		{
			PlayerDefs.CouponCountChangedEvent(App.saveData.player.coupons);
		}
	}

	public static void GachaPulled()
	{
		App.saveData.player.gachaPullsSinceUpdate++;
	}

	public static void AddBadge(int ID)
	{
		if (App.saveData.player.badges.Count < 99999999)
		{
			App.saveData.player.badges.Add(ID);
			App.upsight.CustomAnalyticValueEvent(eSubType1.LEVEL, eSubType2.LEVEL_COUNT, "badge_level", 1, BadgeCount);
		}
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQBA", (float)BadgeCount / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQBA") * 100f);
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQBQ", (float)BadgeCount / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQBQ") * 100f);
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQBg", (float)BadgeCount / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQBg") * 100f);
	}

	public static int GetBadgeID(int index)
	{
		if (index > -1 && index < App.saveData.player.badges.Count)
		{
			return App.saveData.player.badges[index];
		}
		if (TestDefs.BADGES_COUNT_OVERRIDE > -1 && index < BadgeCount)
		{
			return TrainerDefs.GetRandomGymLeaderBadgeID();
		}
		return 0;
	}

	public static void AddTrophy(string ID)
	{
		App.saveData.player.trophies.Add(ID);
	}

	public static string GetTrophyID(int index)
	{
		if (index > -1 && index < App.saveData.player.trophies.Count)
		{
			return App.saveData.player.trophies[index];
		}
		return null;
	}

	public static void AddTrainersDefeated(int amount)
	{
		if (App.saveData.player.trainersDefeated < 99999)
		{
			App.saveData.player.trainersDefeated += amount;
		}
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQAQ", (float)DefeatedTrainers / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQAQ") * 100f);
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQAg", (float)DefeatedTrainers / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQAg") * 100f);
		App.social.SubmitAchievement("CgkIjtTv3ccYEAIQAw", (float)DefeatedTrainers / SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQAw") * 100f);
	}

	public static void SetCouncilMemberDefeated(int number)
	{
		App.saveData.player.councilMembersDefeated = number;
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQBw"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQBw");
		}
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQCA"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQCA");
		}
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQCQ"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQCQ");
		}
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQCg"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQCg");
		}
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQCw"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQCw");
		}
		if ((float)DefeatedCouncilMembers >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQDA"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQDA");
		}
	}

	public static bool IsNextCouncilMemberReadyToBattle()
	{
		return NextCouncilMember <= 6 && CurrentBadgesRequired <= 0;
	}

	public static bool IsCouncilStoryComplete()
	{
		return DefeatedCouncilMembers >= 6;
	}

	public static string GetActiveQuestForIndex(int index)
	{
		return App.saveData.player.activeQuests[index];
	}

	public static string GetCompleteQuestForIndex(int index)
	{
		return App.saveData.player.completedQuests[index].id;
	}

	public static string GetCompleteQuestSolutionForIndex(int index)
	{
		return App.saveData.player.completedQuests[index].solutionID;
	}

	public static void SetQuestActive(string id)
	{
		if (!App.saveData.player.activeQuests.Contains(id))
		{
			App.saveData.player.activeQuests.Add(id);
		}
	}

	public static void SetQuestComplete(string id, string solutionID)
	{
		App.saveData.player.activeQuests.Remove(id);
		if (!IsQuestComplete(id))
		{
			App.saveData.player.completedQuests.Add(new QuestCompleteData(id, solutionID));
		}
	}

	public static bool IsQuestActive(string id)
	{
		return App.saveData.player.activeQuests.Where((string e) => e.Equals(id)).Count() > 0;
	}

	public static bool IsQuestComplete(string id)
	{
		return App.saveData.player.completedQuests.Where((QuestCompleteData e) => e.id.Equals(id)).Count() > 0;
	}

	public static List<string> GetAvailableQuestIDs()
	{
		List<string> list = new List<string>(App.saveData.player.activeQuests);
		for (int i = 0; i < QuestDefs.Count; i++)
		{
			if (list.Count >= 3)
			{
				break;
			}
			string iD = QuestDefs.GetID(i);
			if (!IsQuestComplete(iD) && !list.Contains(iD) && BadgeCount >= QuestDefs.BadgeRequirement(iD))
			{
				string text = QuestDefs.CompleteRequirement(iD);
				if (string.IsNullOrEmpty(text) || IsQuestComplete(text))
				{
					list.Add(iD);
				}
			}
		}
		return list;
	}

	public static void SetCraftRecipeLearned(string id)
	{
		if (!App.saveData.player.craftRecipesLearned.Contains(id))
		{
			App.saveData.player.craftRecipesLearned.Add(id);
		}
	}

	public static bool HasRecipedBeenLearned(string id)
	{
		return App.saveData.player.craftRecipesLearned.Contains(id);
	}

	public static void SetLastGymLeaderID(string id)
	{
		if (lastGymLeaderIDs.Count >= 5)
		{
			lastGymLeaderIDs.RemoveRange(0, lastGymLeaderIDs.Count - 4);
		}
		lastGymLeaderIDs.Add(id);
	}

	public static void SetLastSegmentWorldID(string id)
	{
		if (lastSegmentedWorldIDs.Count >= 5)
		{
			lastSegmentedWorldIDs.RemoveRange(0, lastSegmentedWorldIDs.Count - 4);
		}
		lastSegmentedWorldIDs.Add(id);
	}

	public static void GiveReward(RewardData rewardData, string analyticID, out string resultText, Dictionary<string, int> items = null)
	{
		resultText = null;
		int resultAmount = 0;
		string resultText2;
		switch (rewardData.type)
		{
		case eRewardType.BADGE:
			AddBadge(rewardData.value);
			break;
		case eRewardType.COUNCIL_BADGE:
			SetCouncilMemberDefeated(rewardData.value);
			break;
		case eRewardType.COIN:
			AddCoin(rewardData.value, out resultAmount, out resultText);
			break;
		case eRewardType.MORTY:
		{
			MortyData newMortyData = MortyDefs.GetNewMortyData(rewardData.id, rewardData.value, eIVSetUp.RANDOM, eEVSetUp.NONE);
			if (!SetMortyCaught(newMortyData, out resultText))
			{
				AddCoin(5000, out resultAmount, out resultText2);
				if (resultAmount > 0)
				{
					resultText = string.Format(TextDefs.MORTY_CAPTURED_PARTY_AND_STORAGE_FULL_COINS, resultAmount);
				}
			}
			break;
		}
		case eRewardType.ITEM:
		{
			int resultAmount2;
			if (!ItemDefs.ObtainItem((items == null) ? CurrentItems() : items, rewardData.id, rewardData.value, out resultAmount2, out resultText) && rewardData.swappableForCoin)
			{
				int amount = ItemDefs.Cost(rewardData.id) * (rewardData.value - resultAmount2);
				AddCoin(amount, out resultAmount, out resultText2);
				if (resultAmount > 0)
				{
					resultText = string.Format(TextDefs.ITEM_REWARD_NO_SPACE_BUT_COINS, resultAmount);
				}
			}
			break;
		}
		case eRewardType.TROPHY:
			AddTrophy(rewardData.id);
			break;
		case eRewardType.TOURNAMENT_TICKET:
			App.saveData.tournament.ownedTickets.Add(rewardData.id);
			break;
		}
		if (!string.IsNullOrEmpty(analyticID) && resultAmount > 0)
		{
			if (rewardData.type == eRewardType.COIN)
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, analyticID, resultAmount);
			}
			else
			{
				App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, analyticID + "_sub", resultAmount);
			}
		}
	}

	public static void CurrentPartyDebug()
	{
		List<MortyData> list = CurrentParty();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].DebugLog();
		}
	}
}
