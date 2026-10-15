using System;
using System.Collections.Generic;

public class MPBattleActParalysedData : MPBattleActData
{
	private const string _kDictKeyAttackId = "attack_id";

	public string attackId;

	public MPBattleActParalysedData()
	{
	}

	public MPBattleActParalysedData(Dictionary<string, object> dict)
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
			if (key != null && key == "attack_id")
			{
				attackId = Convert.ToString(item.Value);
			}
		}
	}
}
