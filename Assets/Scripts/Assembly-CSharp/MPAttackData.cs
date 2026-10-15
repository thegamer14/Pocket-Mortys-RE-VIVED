using System;
using System.Collections.Generic;

public class MPAttackData
{
	public const string kDictKeyId = "attack_id";

	public const string kDictKeyPP = "pp";

	public string id;

	public int PP;

	public MPAttackData()
	{
	}

	public MPAttackData(string id, int PP)
	{
		this.id = id;
		this.PP = PP;
	}

	public MPAttackData(Dictionary<string, object> dict)
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
			case "attack_id":
				id = Convert.ToString(item.Value);
				break;
			case "pp":
				PP = Convert.ToInt32(item.Value);
				break;
			}
		}
	}

	public void ResetBattleValues()
	{
	}

	public void RestorePPFull()
	{
		PP = MPAttackDefs.PP(id);
	}
}
