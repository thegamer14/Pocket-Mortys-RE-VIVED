using System;
using System.Collections.Generic;

public class MPBattleActAttackEffectData
{
	private const string _kDictKeyType = "type";

	private const string _kDictKeyToSelf = "to_self";

	private const string _kDictKeyIsAccurate = "is_accurate";

	private const string _kDictKeyIsCritical = "is_critical";

	private const string _kDictKeyStat = "stat";

	private const string _kDictKeyAmount = "amount";

	private const string _kDictKeyAttackerMortyDatas = "attacker_morty_datas";

	private const string _kDictKeyDefenderMortyDatas = "defender_morty_datas";

	public eMPBattleActAttackEffectType type;

	public bool toSelf;

	public bool isAccurate;

	public bool isCritical;

	public string stat;

	public float amount;

	public Dictionary<string, object>[] attackerMortyDataDicts;

	public Dictionary<string, object>[] defenderMortyDataDicts;

	public MPBattleActAttackEffectData()
	{
	}

	public MPBattleActAttackEffectData(Dictionary<string, object> dict)
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
			case "type":
				type = (eMPBattleActAttackEffectType)Enum.Parse(typeof(eMPBattleActAttackEffectType), item.Value.ToString().ToUpper());
				break;
			case "to_self":
				toSelf = Convert.ToBoolean(item.Value);
				break;
			case "is_accurate":
				isAccurate = Convert.ToBoolean(item.Value);
				break;
			case "is_critical":
				isCritical = Convert.ToBoolean(item.Value);
				break;
			case "stat":
				stat = Convert.ToString(item.Value);
				break;
			case "amount":
				amount = Convert.ToSingle(item.Value);
				break;
			case "attacker_morty_datas":
				attackerMortyDataDicts = ConvertExt.ToDictionaryArray(item.Value);
				break;
			case "defender_morty_datas":
				defenderMortyDataDicts = ConvertExt.ToDictionaryArray(item.Value);
				break;
			}
		}
	}
}
