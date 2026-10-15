using System;
using System.Collections.Generic;

public class MPPlayerXpIncrementData
{
	private const string _kDictKeyLevel = "level";

	private const string _kDictKeyXp = "xp";

	private const string _kDictKeyRewards = "rewards";

	public int level;

	public int xp;

	public MPRewardData[] rewards;

	public MPPlayerXpIncrementData()
	{
		rewards = null;
	}

	public MPPlayerXpIncrementData(Dictionary<string, object> dict)
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
			case "level":
				level = Convert.ToInt32(item.Value);
				break;
			case "xp":
				xp = Convert.ToInt32(item.Value);
				break;
			case "rewards":
				rewards = MPRewardData.GetRewardDatasForDictArray(ConvertExt.ToDictionaryArray(item.Value));
				break;
			}
		}
	}
}
