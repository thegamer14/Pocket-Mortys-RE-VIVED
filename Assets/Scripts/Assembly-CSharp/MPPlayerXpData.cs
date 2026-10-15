using System;
using System.Collections.Generic;

public class MPPlayerXpData
{
	private const string _kDictKeyXpMultiplier = "streak";

	private const string _kDictKeyXpMultiplierOld = "streak_old";

	private const string _KDictKeyXpEarned = "xp_earned";

	private const string _kDictKeyXpDatas = "player_datas";

	public int xpMultiplier;

	public int xpMultiplierOld;

	public int xpEarned;

	public MPPlayerXpIncrementData[] increments;

	public MPPlayerXpData()
	{
		increments = null;
	}

	public MPPlayerXpData(Dictionary<string, object> dict)
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
			case "streak":
				xpMultiplier = Convert.ToInt32(item.Value);
				break;
			case "streak_old":
				xpMultiplierOld = Convert.ToInt32(item.Value);
				break;
			case "xp_earned":
				xpEarned = Convert.ToInt32(item.Value);
				break;
			case "player_datas":
				SetIncrementArrayFromDict(ConvertExt.ToDictionaryArray(item.Value));
				break;
			}
		}
	}

	public void SetIncrementArrayFromDict(Dictionary<string, object>[] dictArr)
	{
		increments = new MPPlayerXpIncrementData[dictArr.Length];
		for (int i = 0; i < dictArr.Length; i++)
		{
			increments[i] = new MPPlayerXpIncrementData(dictArr[i]);
		}
	}
}
