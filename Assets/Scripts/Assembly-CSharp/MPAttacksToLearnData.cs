using System;
using System.Collections.Generic;

public class MPAttacksToLearnData
{
	private const string _kDictKeyAttacksToLearn = "attacks_to_learn";

	private const string _kDictKeyAvailableAttacks = "attacks_available";

	public List<KeyValuePair<string, List<string>>> morties;

	public MPAttacksToLearnData()
	{
		morties = new List<KeyValuePair<string, List<string>>>();
	}

	public MPAttacksToLearnData(Dictionary<string, object>[] dictArr)
		: this()
	{
		SetFromDict(dictArr);
	}

	public void SetFromDict(Dictionary<string, object>[] dictArr)
	{
		morties.Clear();
		for (int i = 0; i < dictArr.Length; i++)
		{
			string key = Convert.ToString(dictArr[i]["owned_morty_id"]);
			List<string> list = new List<string>();
			Dictionary<string, object>[] array = ConvertExt.ToDictionaryArray(dictArr[i]["attacks_available"]);
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(Convert.ToString(array[j]["attack_id"]));
			}
			morties.Add(new KeyValuePair<string, List<string>>(key, list));
		}
	}
}
