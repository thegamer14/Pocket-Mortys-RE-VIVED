using System;
using System.Collections.Generic;

public class MPBattleActSwitchData : MPBattleActData
{
	private const string _kDictKeyMortyNetID = "owned_morty_id";

	public string mortyNetId;

	public MPBattleActSwitchData()
	{
	}

	public MPBattleActSwitchData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public override void SetFromDict(Dictionary<string, object> dict)
	{
		base.SetFromDict(dict);
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			string key = item.Key;
			if (key != null && key == "owned_morty_id")
			{
				mortyNetId = Convert.ToString(item.Value);
			}
		}
	}
}
