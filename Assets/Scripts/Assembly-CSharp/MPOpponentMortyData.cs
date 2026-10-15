using System;
using System.Collections.Generic;
using System.Linq;

public class MPOpponentMortyData
{
	public const string kDictKeyNetId = "owned_morty_id";

	public const string kDictKeyId = "morty_id";

	public const string kDictKeyLevel = "level";

	public const string kDictKeyXp = "xp";

	public const string kDictKeyHp = "hp";

	public const string kDictKeyAttacks = "owned_attacks";

	public const string kDictKeyHpStat = "hp_stat";

	public const string kDictKeyAttackStat = "attack_stat";

	public const string kDictKeyDefenceStat = "defence_stat";

	public const string kDictKeySpeedStat = "speed_stat";

	public const string kDictKeyIsPoisoned = "is_poisoned";

	public const string kDictKeyIsParalysed = "is_paralysed";

	public const string kDictKeyEvolutionPoints = "evolution_points";

	public const string kDictKeyAttackIndex = "position";

	public string netId;

	public string id;

	public int level;

	public int xp;

	public int hp;

	public SortedList<int, MPAttackData> attacks;

	public int hpStat;

	public int attackStat;

	public int defenceStat;

	public int speedStat;

	public bool isPoisoned;

	public bool isParalysed;

	public int evolutionPoints;

	public bool isFainted
	{
		get
		{
			return hp <= 0;
		}
	}

	public bool isAilment
	{
		get
		{
			return isPoisoned || isParalysed;
		}
	}

	public MPOpponentMortyData()
	{
		attacks = new SortedList<int, MPAttackData>();
	}

	public MPOpponentMortyData(Dictionary<string, object> dict)
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
			case "morty_id":
				id = ((NetworkTestDefs.OVERRIDE_OPPONENT_MORTY_ID != null) ? NetworkTestDefs.OVERRIDE_OPPONENT_MORTY_ID : Convert.ToString(item.Value));
				break;
			case "level":
				level = Convert.ToInt32(item.Value);
				break;
			case "xp":
				xp = Convert.ToInt32(item.Value);
				break;
			case "hp":
				hp = Convert.ToInt32(item.Value);
				break;
			case "owned_attacks":
				SetAttacksFromDicts(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "hp_stat":
				hpStat = Convert.ToInt32(item.Value);
				break;
			case "attack_stat":
				attackStat = Convert.ToInt32(item.Value);
				break;
			case "defence_stat":
				defenceStat = Convert.ToInt32(item.Value);
				break;
			case "speed_stat":
				speedStat = Convert.ToInt32(item.Value);
				break;
			case "is_poisoned":
				isPoisoned = Convert.ToBoolean(item.Value);
				break;
			case "is_paralysed":
				isParalysed = Convert.ToBoolean(item.Value);
				break;
			case "evolution_points":
				evolutionPoints = Convert.ToInt32(item.Value);
				break;
			}
		}
	}

	public void SetAttacksFromDicts(Dictionary<string, object>[] dictArr)
	{
		if (dictArr == null)
		{
			return;
		}
		for (int i = 0; i < dictArr.Length; i++)
		{
			if (dictArr[i].ContainsKey("position"))
			{
				object obj = dictArr[i]["position"];
				if (obj != null)
				{
					int key = Convert.ToInt32(obj);
					if (attacks.ContainsKey(key))
					{
						attacks[key].SetFromDict(dictArr[i]);
					}
					else
					{
						attacks[key] = new MPAttackData(dictArr[i]);
					}
				}
			}
			else
			{
				string id = Convert.ToString(dictArr[i]["attack_id"]);
				MPAttackData value = attacks.FirstOrDefault((KeyValuePair<int, MPAttackData> e) => e.Value.id == id).Value;
				if (value != null)
				{
					value.SetFromDict(dictArr[i]);
				}
			}
		}
	}

	public void ResetBattleValues()
	{
		isPoisoned = false;
		isParalysed = false;
		foreach (KeyValuePair<int, MPAttackData> attack in attacks)
		{
			attack.Value.ResetBattleValues();
		}
	}

	public int GetTotalRemainingPP()
	{
		int num = 0;
		foreach (KeyValuePair<int, MPAttackData> attack in attacks)
		{
			num += attack.Value.PP;
		}
		return num;
	}

	public int GetTotalMaxPP()
	{
		int num = 0;
		foreach (KeyValuePair<int, MPAttackData> attack in attacks)
		{
			num += MPAttackDefs.PP(attack.Value.id);
		}
		return num;
	}

	public bool HasAttackWithID(string id)
	{
		return attacks.Any((KeyValuePair<int, MPAttackData> pair) => pair.Value.id.Equals(id));
	}

	public void RestoreAllPPFull()
	{
		foreach (KeyValuePair<int, MPAttackData> attack in attacks)
		{
			if (attack.Value != null)
			{
				attack.Value.RestorePPFull();
			}
		}
	}
}
