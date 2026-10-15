public static class SharedStatInfo
{
	public static WorldInfo worldInfo;

	public static BattleInfo battleInfo;

	public static PartyInfo partyInfo;

	public static MortyStatsInfo mortyStatsInfo;

	public static MortydexEntryInfo mortydexEntryInfo;

	public static StorageDecksInfo storageDecksInfo;

	public static StorageBootcampInfo storageBootcampInfo;

	public static BagInfo bagInfo;

	public static CraftInfo craftInfo;

	public static RewardInfo rewardInfo;

	public static LabsInfo labsInfo;

	public static TournamentSetUpInfo tournamentSetUpInfo;

	public static void SetUp()
	{
		worldInfo = new WorldInfo();
	}

	public static void ResetAll()
	{
		worldInfo = new WorldInfo();
		battleInfo = null;
		partyInfo = null;
		mortyStatsInfo = null;
		mortydexEntryInfo = null;
		storageDecksInfo = null;
		storageBootcampInfo = null;
		bagInfo = null;
		craftInfo = null;
		rewardInfo = null;
		labsInfo = null;
		tournamentSetUpInfo = null;
	}
}
