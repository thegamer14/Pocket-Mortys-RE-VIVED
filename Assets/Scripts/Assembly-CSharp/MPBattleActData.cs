using System;
using System.Collections.Generic;

public class MPBattleActData
{
	private const string _kDictKeyType = "type";

	private const string _kDictKeyAttackerNetId = "attacker_player_id";

	private const string _kDictKeyDefencerNetId = "defender_player_id";

	private const string _kDictKeyAttackerMortyDatas = "attacker_morty_datas";

	private const string _kDictKeyDefenderMortyDatas = "defender_morty_datas";

	public eMPBattleActType type;

	public string attackerNetId;

	public string defenderNetId;

	public Dictionary<string, object>[] attackerMortyDataDicts;

	public Dictionary<string, object>[] defenderMortyDataDicts;

	public MPBattleActData()
	{
	}

	public MPBattleActData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public virtual void SetFromDict(Dictionary<string, object> dict)
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
				type = (eMPBattleActType)Enum.Parse(typeof(eMPBattleActType), Convert.ToString(item.Value).ToUpper());
				break;
			case "attacker_player_id":
				attackerNetId = Convert.ToString(item.Value);
				break;
			case "defender_player_id":
				defenderNetId = Convert.ToString(item.Value);
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
