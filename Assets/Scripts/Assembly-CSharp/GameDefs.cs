public class GameDefs
{
	public const string kVersion = "v2.0.3";

	public const float kNodeSize = 1f;

	public const float kNodeSizeHalf = 0.5f;

	public const int kLockMortyLabsCouncilBadge = 1;

	public const int kLockTournamentsCouncilBadge = 2;

	public const float kTextReadDelay = 2f;

	public const float kBATextReadDelay = 4f;

	public const float kHCTextReadDelay = 4f;

	public const float kCFTextReadDelay = 5.5f;

	public const float kDisobeyTextReadDelay = 4f;

	public const string kServerTimeURL = "http://www.adultswim.com/videos/api/v1/_server_time?a=pm_{0}";

	public const string APP_STORE_URL = "http://hyperurl.co/zrn5s4";

	public static void SetUpDefs()
	{
		WorldDefs.SetUp();
		InteractionDefs.SetUp();
		SignPostDefs.SetUp();
		PlayerAvatarDefs.SetUp();
		MortyDefs.SetUp();
		AttackDefs.SetUp();
		TrainerDefs.SetUp();
		NPCDefs.SetUp();
		ItemDefs.SetUp();
		QuestDefs.SetUp();
		RecipeDefs.SetUp();
		TournamentDefs.SetUp();
	}

	public static void RemoveDefs()
	{
		WorldDefs.Remove();
		InteractionDefs.Remove();
		SignPostDefs.Remove();
		PlayerAvatarDefs.Remove();
		MortyDefs.Remove();
		AttackDefs.Remove();
		TrainerDefs.Remove();
		NPCDefs.Remove();
		ItemDefs.Remove();
		QuestDefs.Remove();
		RecipeDefs.Remove();
		TournamentDefs.Remove();
	}

	public static ePlaceholderType GetPlaceholderTypeForNodeType(eNodeType type)
	{
		switch (type)
		{
		default:
			return ePlaceholderType.NONE;
		case eNodeType.ITEM:
			return ePlaceholderType.ITEM_PART_OR_CRATE;
		case eNodeType.MORTY:
			return ePlaceholderType.MORTY;
		case eNodeType.NPC:
			return ePlaceholderType.NPC;
		case eNodeType.TRAINER:
			return ePlaceholderType.TRAINER;
		case eNodeType.PORTAL:
			return ePlaceholderType.PORTAL;
		case eNodeType.SIGN_POST:
			return ePlaceholderType.SIGN_POST;
		case eNodeType.GYM:
			return ePlaceholderType.GYM;
		case eNodeType.GACHA_MACHINE:
			return ePlaceholderType.GACHA_MACHINE;
		case eNodeType.DIALOGUE_TRIGGER:
			return ePlaceholderType.DIALOGUE_TRIGGER;
		case eNodeType.QUEST_GIVER:
			return ePlaceholderType.QUEST_GIVER;
		case eNodeType.CRAFT_TABLE:
			return ePlaceholderType.CRAFT_TABLE;
		}
	}

	public static void StartGame()
	{
		if (!PlayerDefs.GetStorySeen("GarageIntro"))
		{
			string resultText;
			PlayerDefs.SetMortyCaught(MortyDefs.GetNewMortyData("MortyDefault", 5, eIVSetUp.RANDOM, eEVSetUp.NONE), out resultText);
			App.states.Get<StoryState>().storyIDToLoad = "GarageIntro";
			App.states.Set<StoryState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else if (PlayerDefs.BadgeCount == 0 && !WorldDefs.isSegmentedWorldSaved)
		{
			SharedStatInfo.worldInfo.worldID = "Tutorial";
			SharedStatInfo.worldInfo.entranceID = "SegmentedWorldPortal";
			App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else if (!PlayerDefs.GetStorySeen("CouncilChambersIntro") && !WorldDefs.isSegmentedWorldSaved)
		{
			App.states.Get<StoryState>().storyIDToLoad = "CouncilChambersIntro";
			App.states.Set<StoryState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			PlayerDefs.ReDisplayCurrentAlertNotification();
			App.states.Set<WorldState>(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
