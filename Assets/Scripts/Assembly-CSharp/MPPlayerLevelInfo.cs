public class MPPlayerLevelInfo : MPStateInfo
{
	public string backgroundId;

	private MPPlayerXpData _playerXpData;

	public MPBattleTurnOpponentData battlePlayerData;

	public MPPlayerLevelInfo(eMPStateOrigin origin, string backgroundId, MPPlayerXpData playerXpData)
		: base(origin)
	{
		this.backgroundId = backgroundId;
		_playerXpData = playerXpData;
	}

	public MPPlayerLevelInfo(eMPStateOrigin origin, string backgroundId, MPBattleTurnOpponentData battlePlayerData)
		: base(origin)
	{
		this.backgroundId = backgroundId;
		this.battlePlayerData = battlePlayerData;
	}

	public MPPlayerXpData GetPlayerXpData()
	{
		return (battlePlayerData == null) ? _playerXpData : battlePlayerData.playerXpData;
	}
}
