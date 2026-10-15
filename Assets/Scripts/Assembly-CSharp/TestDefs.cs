using System.Collections.Generic;
using UnityEngine;

public class TestDefs
{
	public static bool DEBUG_FPS_COUNTER;

	public static bool DEBUG_GOOGLE_PLAY_GAMES;

	public static bool DEBUG_SAVE_CONTROLLER;

	public static bool DEBUG_STATE_CONTROLLER;

	public static bool DEBUG_IAP_CONTROLLER;

	public static bool DEBUG_BPS_SOCIAL;

	public static bool DEBUG_UPSIGHT_CONTROLLER;

	public static bool DEBUG_FYBER_PLUGIN;

	public static bool DEBUG_INTERSITIAL_STATE;

	public static bool DEBUG_STAT_ICONS;

	public static bool DEBUG_AUDIO;

	public static bool DEBUG_FYBER_CONTROLLER;

	public static bool PLAYER_TAKE_NO_DAMAGE;

	public static bool PLAYER_ONE_HIT_KILL;

	public static bool PLAYER_GUARANTEE_LEVEL_UP;

	public static int BADGES_COUNT_OVERRIDE = -1;

	public static int TRAINERS_DEFEATED_OVERRIDE = -1;

	public static int COUNCIL_MEMBERS_DEFEATED_OVERRIDE = -1;

	public static bool TRAINERS_DISABLE_SIGHT;

	public static bool TOURNAMENT_TRAINERS_EASY;

	public static bool DEBUG_KEYS;

	public static string FORCE_ALL_MORTY_IDS;

	public static string EDITOR_IAP_PRODUCTS_JSON = "[{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon01\", \"localizedTitle\" : \"Coupon Pack 1\", \"localizedDescription\" : \"1 Coupon!\", \"price\" : 0.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 0.99 }{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon05\", \"localizedTitle\" : \"Coupon Pack 2\", \"localizedDescription\" : \"5 Coupons!\", \"price\" : 3.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 3.99 }{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon15\", \"localizedTitle\" : \"Coupon Pack 3\", \"localizedDescription\" : \"15 Coupons!\", \"price\" : 9.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 9.99 }{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon40\", \"localizedTitle\" : \"Coupon Pack 4\", \"localizedDescription\" : \"40 Coupons!\", \"price\" : 19.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 19.99 }{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon125\", \"localizedTitle\" : \"Coupon Pack 5\", \"localizedDescription\" : \"125 Coupons!\", \"price\" : 49.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 49.99 }{ \"productIdentifier\" : \"com.adultswim.pocketmorties.coupon300\", \"localizedTitle\" : \"Coupon Pack 6\", \"localizedDescription\" : \"300 Coupons!\", \"price\" : 99.99, \"currencySymbol\" : \"$\", \"currencyCode\" : \"USD\", \"formattedPrice\" : 99.99 }]";

	public static bool EDITOR_LOAD_IAP_PRODUCTS = true;

	public static bool FAIL_EDITOR_IAP_PURCHASES;

	public static bool USE_IOS_VERIFICATION_SANDBOX;

	public static bool FAIL_IOS_SERVER_VALIDATION;

	public static bool TEST_APP_UPDATE_STATE;

	public static bool USE_UPSIGHT_DEMO_KEY;

	public static bool USE_UPSIGHT_ANALYTICS_ANDROID_QA_KEY;

	public static bool DISABLE_UPSIGHT_INTERSTITIALS;

	public static bool TEST_IAP_PROMO_INTERSTITIAL;

	public static bool FAKE_CLOUD_CONFLICTS;

	public static bool MP_AUTO_SIGN_IN_FROM_MAIN_MENU;

	public static int MP_OVERWRITE_PLAYER_LEVEL;

	public static bool MP_DEBUG_SPAWN_POINTS;

	public static bool MP_DISABLE_MULTIPLAYER_BADGE_REQ;

	public static bool MP_DISABLE_DIMENSION_LEVEL_LOCKS;

	private static int _lastWildMortyIndexBattled = -1;

	public static void OverrideSaveData()
	{
	}

	private static void DebugSkipTutorial()
	{
		if (!PlayerDefs.GetStorySeen("CouncilChambersIntro"))
		{
			PlayerDefs.SetStorySeen("GarageIntro");
			PlayerDefs.SetStorySeen("CouncilChambersIntro");
			PlayerDefs.SetMortySeen(TrainerDefs.GetNewTrainerData("TrainerTutorialGymLeader").morties[0].id);
			PlayerDefs.SetMortySeen(TrainerDefs.GetNewTrainerData("TrainerTutorial").morties[0].id);
			string resultText;
			PlayerDefs.SetMortyCaught(MortyDefs.GetNewMortyData("MortyDefault", 5, eIVSetUp.RANDOM, eEVSetUp.NONE), out resultText);
			PlayerDefs.SetMortyCaught(MortyDefs.GetNewMortyData("MortyScruffy", 6, eIVSetUp.RANDOM, eEVSetUp.NONE), out resultText);
			PlayerDefs.AddBadge(TrainerDefs.GetRandomGymLeaderBadgeID());
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertCitadelFirstTimeID");
			App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
			App.saveData.Save(eSaveDataType.PLAYER);
		}
	}

	public static void OverrideMortiesSeen()
	{
		App.saveData.player.seenMorties = new List<string>();
		for (int i = 0; i < MortyDefs.idCount; i++)
		{
			PlayerDefs.SetMortySeen(MortyDefs.GetId(i));
		}
	}

	public static void OverrideMortiesCaught(int level, int addLoops = 1, int randRange = 0)
	{
		App.saveData.player.partyMorties = new List<MortyData>();
		App.saveData.player.storageMorties = new List<MortyData>();
		App.saveData.player.caughtMorties = new List<string>();
		App.saveData.player.SetDecksToDefault();
		string resultText = null;
		for (int i = 0; i < addLoops; i++)
		{
			for (int j = 0; j < MortyDefs.idCount; j++)
			{
				PlayerDefs.SetMortyCaught(MortyDefs.GetNewMortyData(MortyDefs.GetId(j), Random.Range(level - randRange, level + randRange), eIVSetUp.RANDOM, eEVSetUp.NONE), out resultText);
			}
		}
	}

	public static void OverrideObtainAllItems(int count)
	{
		App.saveData.player.items = new Dictionary<string, int>();
		string resultText = null;
		for (int i = 0; i < ItemDefs.idCount; i++)
		{
			string iD = ItemDefs.GetID(i);
			if (ItemDefs.Type(iD) != eItemType.TOURNAMENT_TICKET)
			{
				int resultAmount;
				ItemDefs.ObtainItem(App.saveData.player.items, iD, count, out resultAmount, out resultText);
			}
		}
	}

	public static void OverrideUnlockAllRecipes()
	{
		for (int i = 0; i < RecipeDefs.Count; i++)
		{
			PlayerDefs.SetCraftRecipeLearned(RecipeDefs.GetID(i));
		}
	}

	public static void OverrideCompleteAllQuests()
	{
		for (int i = 0; i < QuestDefs.Count; i++)
		{
			PlayerDefs.SetQuestComplete(QuestDefs.GetID(i), QuestDefs.SolutionIDs(QuestDefs.GetID(i))[0]);
		}
	}

	public static void OverrideBadges(int count)
	{
		App.saveData.player.badges = new List<int>();
		for (int i = 0; i < count; i++)
		{
			App.saveData.player.badges.Add(TrainerDefs.GetRandomGymLeaderBadgeID());
		}
	}

	public static void OverrideTournamentProgress(int eventsComplete)
	{
		App.saveData.tournament.eventDatas = new Dictionary<string, Dictionary<string, TournamentEventData>>();
		int num = 0;
		List<string> allTournamentIDs = TournamentDefs.GetAllTournamentIDs();
		for (int i = 0; i < allTournamentIDs.Count; i++)
		{
			string[] array = TournamentDefs.TournamentEventIDs(allTournamentIDs[i]);
			for (int j = 0; j < array.Length; j++)
			{
				num++;
				if (num <= eventsComplete)
				{
					TournamentDefs.SetEventComplete(allTournamentIDs[i], array[j]);
				}
			}
		}
	}

	public static void FakePlayerProgress(int badgeCount = 1, int councilMembersDefeated = 0, int mortyCount = 2, int mortyLevelOffset = 1)
	{
		string resultText = null;
		App.saveData.player.storiesSeen = new List<string>();
		PlayerDefs.SetStorySeen("GarageIntro");
		PlayerDefs.SetStorySeen("CouncilChambersIntro");
		PlayerDefs.SetStorySeen("CouncilChambers");
		App.saveData.player.badges = new List<int>();
		for (int i = 0; i < badgeCount; i++)
		{
			App.saveData.player.badges.Add(TrainerDefs.GetRandomGymLeaderBadgeID());
		}
		App.saveData.player.councilMembersDefeated = councilMembersDefeated;
		App.saveData.player.partyMorties = new List<MortyData>();
		App.saveData.player.storageMorties = new List<MortyData>();
		App.saveData.player.caughtMorties = new List<string>();
		App.saveData.player.SetDecksToDefault();
		if (FORCE_ALL_MORTY_IDS == null)
		{
			MortyData newMortyData = MortyDefs.GetNewMortyData("MortyDefault", MortyDefs.GetBalancedMortyLevel(eBalanceType.DEFAULT_TRAINER) + mortyLevelOffset, eIVSetUp.RANDOM, eEVSetUp.RANDOM);
			PlayerDefs.SetMortyCaught(newMortyData, out resultText);
			MortyData newMortyData2 = MortyDefs.GetNewMortyData("MortyScruffy", MortyDefs.GetBalancedMortyLevel(eBalanceType.DEFAULT_TRAINER) + mortyLevelOffset, eIVSetUp.RANDOM, eEVSetUp.RANDOM);
			PlayerDefs.SetMortyCaught(newMortyData2, out resultText);
			mortyCount -= 2;
		}
		for (int j = 0; j < mortyCount; j++)
		{
			RandomMortyParams randomMortyParams = new RandomMortyParams();
			randomMortyParams.badgeLevelLock = true;
			string randomID = MortyDefs.GetRandomID(randomMortyParams, true);
			MortyData newMortyData3 = MortyDefs.GetNewMortyData(randomID, MortyDefs.GetBalancedMortyLevel(eBalanceType.DEFAULT_TRAINER) + mortyLevelOffset, eIVSetUp.RANDOM, eEVSetUp.RANDOM);
			PlayerDefs.SetMortyCaught(newMortyData3, out resultText);
		}
		if (PlayerDefs.IsCouncilStoryComplete())
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertEndGameID");
		}
		else if (PlayerDefs.IsNextCouncilMemberReadyToBattle())
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertCouncilMemberReadyID");
		}
		else
		{
			PlayerDefs.DisplayNotification(eNotificationType.ALERT, "AlertBadgesRequiredID");
		}
	}

	public static void DebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
			SharedStatInfo.worldInfo.worldID = WorldDefs.GetRandomSegmentedWorldID(App.saveData.player.lastSegmentedWorldIDs);
			SharedStatInfo.worldInfo.nodeSetID = null;
			SharedStatInfo.worldInfo.entranceID = "SegmentedWorldPortal";
			App.states.Set<WorldState>();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			_lastWildMortyIndexBattled++;
			_lastWildMortyIndexBattled %= MortyDefs.idCount;
			MortyData newMortyData = MortyDefs.GetNewMortyData(MortyDefs.GetId(_lastWildMortyIndexBattled), 5, eIVSetUp.RANDOM, eEVSetUp.NONE);
			MortyData newMortyData2 = MortyDefs.GetNewMortyData(MortyDefs.GetId(_lastWildMortyIndexBattled), 5, eIVSetUp.RANDOM, eEVSetUp.NONE);
			BattleOpponent player = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, new List<MortyData> { newMortyData2 }, PlayerDefs.CurrentItems());
			BattleOpponentAI enemy = new BattleOpponentAI(eBattleOpponentType.AI_WILD_MORTY, eBattleOpponentSide.RIGHT, new List<MortyData> { newMortyData }, null);
			SharedStatInfo.battleInfo = new BattleInfo(player, enemy);
			App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			TrainerData newRandomTrainerData = TrainerDefs.GetNewRandomTrainerData();
			BattleOpponent player2 = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
			BattleOpponentAI enemy2 = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, newRandomTrainerData.morties, newRandomTrainerData.items, newRandomTrainerData);
			SharedStatInfo.battleInfo = new BattleInfo(player2, enemy2);
			App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			TrainerData newRandomGymLeaderData = TrainerDefs.GetNewRandomGymLeaderData();
			BattleOpponent player3 = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
			BattleOpponentAI enemy3 = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, newRandomGymLeaderData.morties, newRandomGymLeaderData.items, newRandomGymLeaderData);
			SharedStatInfo.battleInfo = new BattleInfo(player3, enemy3);
			App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			TrainerData newTrainerData = TrainerDefs.GetNewTrainerData(PlayerDefs.NextCouncilMemberID);
			BattleOpponent player4 = new BattleOpponent(eBattleOpponentType.PLAYER, eBattleOpponentSide.LEFT, PlayerDefs.AssetID, null, PlayerDefs.CurrentParty(), PlayerDefs.CurrentItems());
			BattleOpponentAI enemy4 = new BattleOpponentAI(eBattleOpponentType.AI_TRAINER, eBattleOpponentSide.RIGHT, newTrainerData.morties, newTrainerData.items, newTrainerData);
			SharedStatInfo.battleInfo = new BattleInfo(player4, enemy4);
			App.states.Set<BattleState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			App.states.Set<MPStorageState>(new FadeTransInfo(), new FadeTransInfo(), true, eStateAction.ADD_CURRENT_TO_CACHE);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			SharedStatInfo.worldInfo.worldID = "TournamentLobby";
			SharedStatInfo.worldInfo.entranceID = "TournamentLobbyCitadel";
			App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			LocalizationDefs.SetLanguage(eLanguage.EN);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			LocalizationDefs.SetLanguage(LocalizationDefs.GetCurrentLanguage() + 1);
		}
		if ((bool)App.net.room)
		{
			App.net.room.udpSendEnabled = !Input.GetKey(KeyCode.Space);
		}
	}
}
