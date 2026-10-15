using System;
using System.Collections.Generic;

public class MPVideoRewardData
{
	public const string kDictKeyInsentiveId = "incentive_id";

	public const string kDictKeyRewardsId = "rewards";

	public const string kDictKeyTokenId = "token";

	public string Id;

	public MPRewardData[] rewards;

	public string token;

	public MPVideoRewardData()
	{
	}

	public MPVideoRewardData(Dictionary<string, object> dict)
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
			case "incentive_id":
				Id = Convert.ToString(item.Value);
				break;
			case "rewards":
				rewards = MPRewardData.GetRewardDatasForDictArray(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "token":
				token = Convert.ToString(item.Value);
				break;
			}
		}
	}

	public bool CanRewardsBeDisplayed()
	{
		return rewards != null && rewards[0].type != eMPRewardType.STREAK;
	}

	public string GetMeeseekPlacementId()
	{
		switch (Id)
		{
		default:
			return null;
		case "PvPWinAd":
			return "multiplayer_battle_won";
		case "PvPLoseAd":
			return "multiplayer_battle_lost";
		case "multiplayer_npc":
			return "multiplayer_npc";
		}
	}
}
