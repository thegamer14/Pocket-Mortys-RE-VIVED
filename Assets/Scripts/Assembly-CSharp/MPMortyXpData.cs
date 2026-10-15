using System;
using System.Collections.Generic;

public class MPMortyXpData
{
	private const string _kDictKeyNetId = "owned_morty_id";

	private const string _kDictKeyXpEarned = "xp_earned";

	private const string _kDictKeyMortyDatas = "owned_morty_datas";

	public string netId;

	public int xpEarned;

	public Dictionary<string, object>[] mortyDatasDicts;

	public MPMortyXpData()
	{
	}

	public MPMortyXpData(Dictionary<string, object> dict)
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
			case "owned_morty_id":
				netId = Convert.ToString(item.Value);
				break;
			case "xp_earned":
				xpEarned = Convert.ToInt32(item.Value);
				break;
			case "owned_morty_datas":
				mortyDatasDicts = ConvertExt.ToDictionaryArray(item.Value);
				break;
			}
		}
	}
}
