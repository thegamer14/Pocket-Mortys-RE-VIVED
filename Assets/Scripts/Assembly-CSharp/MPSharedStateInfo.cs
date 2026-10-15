public static class MPSharedStateInfo
{
	public static MPServerMessageInfo serverMessage;

	public static MPRegistrationInfo registrationInfo;

	public static MPBattleInfo battleInfo;

	public static MPPartyInfo partyInfo;

	public static MPMortydexInfo mortydexInfo;

	public static MPMortydexEntryInfo mortydexEntryInfo;

	public static MPMortyStatsInfo mortyStatsInfo;

	public static MPBagInfo bagInfo;

	public static MPRewardInfo rewardInfo;

	public static MPWorldInfo worldInfo;

	public static MPUserCardInfo userCard;

	public static MPStorageDecksInfo storageDecksInfo;

	public static MPLearnAttacksInfo learnAttacksInfo;

	public static MPPlayerLevelInfo playerLevelInfo;

	public static MPCraftInfo craftInfo;

	public static MPLabsInfo labsInfo;

	public static MPHealingCenterInfo healingCenterInfo;

	public static MPFriendsInfo friendInfo;

	public static MPFriendDonateMortyInfo friendDonateMortyInfo;

	public static MPWorldMapInfo worldMapInfo;

	public static MPVideoRewardMeeseekInfo videoRewardMeeseekInfo;

	public static void SetUp()
	{
	}

	public static void ResetAll(bool excludeWorld = false)
	{
		if (!excludeWorld)
		{
			worldInfo = null;
		}
		serverMessage = null;
		registrationInfo = null;
		battleInfo = null;
		partyInfo = null;
		mortydexInfo = null;
		mortydexEntryInfo = null;
		mortyStatsInfo = null;
		bagInfo = null;
		rewardInfo = null;
		userCard = null;
		storageDecksInfo = null;
		learnAttacksInfo = null;
		playerLevelInfo = null;
		craftInfo = null;
		labsInfo = null;
		healingCenterInfo = null;
		friendInfo = null;
		friendDonateMortyInfo = null;
		worldMapInfo = null;
		videoRewardMeeseekInfo = null;
	}
}
