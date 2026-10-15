using System;
using System.Collections.Generic;

public class MPBattleActItemData : MPBattleActData
{
	private const string _kDictKeyId = "item_id";

	private const string _kDictKeyIsSuccess = "success";

	private const string _kDictKeyEffectValue = "effect_value";

	private const string _kDictKeyMortyReward = "capture_morty";

	public string id;

	public string mortyNetId;

	public bool isSuccess;

	public int effectValue;

	public MPRewardData mortyRewardData;

	public MPBattleActItemData()
	{
	}

	public MPBattleActItemData(Dictionary<string, object> dict)
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
			switch (item.Key)
			{
			case "item_id":
				id = Convert.ToString(item.Value);
				break;
			case "owned_morty_id":
				mortyNetId = Convert.ToString(item.Value);
				break;
			case "success":
				isSuccess = Convert.ToBoolean(item.Value);
				break;
			case "effect_value":
				effectValue = Convert.ToInt32(item.Value);
				break;
			case "capture_morty":
				mortyRewardData = new MPRewardData((Dictionary<string, object>)item.Value);
				break;
			}
		}
	}
}
