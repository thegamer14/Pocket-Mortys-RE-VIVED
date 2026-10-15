using System;
using System.Collections.Generic;

public class MPBattleActAttackData : MPBattleActData
{
	private const string _kDictKeyId = "attack_id";

	private const string _kDictKeyElementModifier = "element_modifier";

	private const string _kDictKeyEffectDatas = "effect_datas";

	public string id;

	public float elementModifier;

	public List<MPBattleActAttackEffectData> effectDatas;

	public MPBattleActAttackData()
	{
	}

	public MPBattleActAttackData(Dictionary<string, object> dict)
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
			case "attack_id":
				id = Convert.ToString(item.Value);
				break;
			case "element_modifier":
				elementModifier = Convert.ToSingle(item.Value);
				break;
			case "effect_datas":
				SetEffectDatasFromDicts(ConvertExt.ToDictionaryArray(item.Value));
				break;
			}
		}
	}

	public void SetEffectDatasFromDicts(Dictionary<string, object>[] dictArr)
	{
		if (dictArr != null)
		{
			effectDatas = new List<MPBattleActAttackEffectData>();
			for (int i = 0; i < dictArr.Length; i++)
			{
				effectDatas.Add(new MPBattleActAttackEffectData(dictArr[i]));
			}
		}
	}
}
