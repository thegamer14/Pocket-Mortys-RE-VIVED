public class MPRewardInfo : MPStateInfo
{
	private MPRewardData[] _rewardDatas;

	public eMPRewardConfirmation confirmation;

	public MPBattleTurnOpponentData battlePlayerData;

	public bool fromPlayerLevelState;

	public string analyticId;

	public MPRewardInfo(eMPStateOrigin origin, MPRewardData[] rewards, string analyticId, eMPRewardConfirmation confirmation = eMPRewardConfirmation.SUCCESS)
		: base(origin)
	{
		_rewardDatas = rewards;
		this.confirmation = confirmation;
		this.analyticId = analyticId;
	}

	public MPRewardInfo(eMPStateOrigin origin, MPBattleTurnOpponentData battlePlayerData, string analyticId)
		: base(origin)
	{
		this.battlePlayerData = battlePlayerData;
		confirmation = eMPRewardConfirmation.SUCCESS;
		this.analyticId = analyticId;
	}

	public MPRewardData[] GetRewardDatas()
	{
		return (battlePlayerData == null) ? _rewardDatas : battlePlayerData.rewardDatas;
	}
}
