using System.Collections.Generic;

public class MPBattleTurnOpponentData
{
	private const string _kDictKeyPlayerXpData = "player_xp_datas";

	private const string _kDictKeyMortyXpDatas = "xp_datas";

	private const string _kDictKeyAttacksToLearn = "attacks_to_learn";

	private const string _kDictKeyAvailableAttacks = "attacks_available";

	private const string _kDictKeyRewardDatas = "rewards";

	private const string _kDictKeyVideoRewardDatas = "incentive";

	public Dictionary<string, object>[] itemsArr;

	public MPPlayerXpData playerXpData;

	public MPMortyXpData[] mortyXpDatas;

	public MPAttacksToLearnData attacksToLearnData;

	public MPRewardData[] rewardDatas;

	public MPVideoRewardData videoRewardMeeseeksData;

	public MPBattleTurnOpponentData()
	{
	}

	public MPBattleTurnOpponentData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public void SetFromDict(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "owned_items":
				itemsArr = ConvertExt.ToDictionaryArray(item.Value);
				break;
			case "player_xp_datas":
				playerXpData = new MPPlayerXpData((Dictionary<string, object>)item.Value);
				break;
			case "xp_datas":
				SetMortyXpDatasFromDicts(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "attacks_to_learn":
				attacksToLearnData = new MPAttacksToLearnData(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "rewards":
				rewardDatas = MPRewardData.GetRewardDatasForDictArray(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "incentive":
				videoRewardMeeseeksData = new MPVideoRewardData(ConvertExt.ToDictionary(item.Value));
				break;
			}
		}
	}

	public void SetMortyXpDatasFromDicts(Dictionary<string, object>[] dictArr)
	{
		if (dictArr != null)
		{
			mortyXpDatas = new MPMortyXpData[dictArr.Length];
			for (int i = 0; i < dictArr.Length; i++)
			{
				mortyXpDatas[i] = new MPMortyXpData(dictArr[i]);
			}
		}
	}

	public bool IsAttacksToLearnStateRequired()
	{
		return attacksToLearnData != null && attacksToLearnData.morties.Count > 0;
	}

	public bool IsPlayerLevelStateRequired()
	{
		return playerXpData != null && (playerXpData.xpEarned > 0 || playerXpData.xpMultiplier != playerXpData.xpMultiplierOld);
	}

	public bool IsRewardStateRequired()
	{
		return App.net.battle.turnPlayerData.rewardDatas != null && App.net.battle.turnPlayerData.rewardDatas.Length > 0;
	}

	public bool IsVideoRewardStateRequired()
	{
		return videoRewardMeeseeksData != null && App.fyber.isVideoAvailable;
	}
}
