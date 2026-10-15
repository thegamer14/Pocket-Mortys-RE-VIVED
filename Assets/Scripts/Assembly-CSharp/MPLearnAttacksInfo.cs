public class MPLearnAttacksInfo : MPStateInfo
{
	public string backgroundId;

	private MPAttacksToLearnData _attacksToLearnData;

	public MPBattleTurnOpponentData battlePlayerData;

	public MPLearnAttacksInfo(eMPStateOrigin origin, string backgroundId, MPAttacksToLearnData attacksToLearnData)
		: base(origin)
	{
		this.backgroundId = backgroundId;
		_attacksToLearnData = attacksToLearnData;
	}

	public MPLearnAttacksInfo(eMPStateOrigin origin, string backgroundId, MPBattleTurnOpponentData battlePlayerData)
		: base(origin)
	{
		this.backgroundId = backgroundId;
		this.battlePlayerData = battlePlayerData;
	}

	public MPAttacksToLearnData GetAttackToLearnData()
	{
		return (battlePlayerData == null) ? _attacksToLearnData : battlePlayerData.attacksToLearnData;
	}
}
