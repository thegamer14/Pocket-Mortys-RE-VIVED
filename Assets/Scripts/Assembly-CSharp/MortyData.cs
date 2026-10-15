using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MortyData : NodeData
{
	private string guid;

	public int dimensionCaught;

	public int level;

	public int xp;

	public List<string> attackIDs;

	public List<int> attackPPs;

	public int hpStat;

	public int attackStat;

	public int defenceStat;

	public int speedStat;

	public int hpIV;

	public int attackIV;

	public int defenceIV;

	public int speedIV;

	public int hpEV;

	public int attackEV;

	public int defenceEV;

	public int speedEV;

	private int attackStatBonus;

	private int defenceStatBonus;

	private int speedStatBonus;

	public float attackModifier;

	public float defenceModifier;

	public float speedModifier;

	public float accuracyModifier;

	public float evadeModifier;

	public bool isPoisoned;

	public bool isParalysed;

	public int hp { get; private set; }

	public float attack
	{
		get
		{
			return MortyDefs.GetBasicStatStepValue(attackModifier);
		}
	}

	public float defence
	{
		get
		{
			return MortyDefs.GetBasicStatStepValue(defenceModifier);
		}
	}

	public int speed
	{
		get
		{
			return Mathf.FloorToInt((float)speedStat * MortyDefs.GetBasicStatStepValue(speedModifier));
		}
	}

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

	public MortyData(string id, int level, eIVSetUp ivSetUp, eEVSetUp evSetUp)
		: base(id)
	{
		level = Mathf.Clamp(level, 1, 100);
		dimensionCaught = UnityEngine.Random.Range(100, 999) + 1;
		guid = Guid.NewGuid().ToString();
		attackIDs = new List<string>(new string[4]);
		attackPPs = new List<int>(new int[4]);
		attackPPs.Populate(-1);
		switch (ivSetUp)
		{
		case eIVSetUp.RANDOM:
			GenerateIVs();
			break;
		case eIVSetUp.RANDOM_RESTRICTED:
			GenerateIVs(0, 10);
			break;
		case eIVSetUp.MAX:
			GenerateIVs(16);
			break;
		}
		GenerateEVs(level, evSetUp);
		xp = MortyDefs.GetXPTarget(level);
		SetLevel(level);
		RandomizeAttacks();
		hp = hpStat;
		ResetBattleModifiers();
	}

	public string GetGuid()
	{
		if (string.IsNullOrEmpty(guid))
		{
			guid = Guid.NewGuid().ToString();
		}
		return guid;
	}

	public void SetGuid(string val)
	{
		guid = val;
	}

	public void SetLevel(int value)
	{
		level = Mathf.Clamp(value, 0, 100);
		hpStat = Mathf.FloorToInt((hpIV + MortyDefs.HPBase(id) + Mathf.FloorToInt(Mathf.Sqrt(hpEV) / 4f) + 50) * level / 50 + 10);
		attackStat = Mathf.FloorToInt((attackIV + MortyDefs.AttackBase(id) + Mathf.FloorToInt(Mathf.Sqrt(attackEV) / 4f)) * level / 50 + 5) + attackStatBonus;
		defenceStat = Mathf.FloorToInt((defenceIV + MortyDefs.DefenceBase(id) + Mathf.FloorToInt(Mathf.Sqrt(defenceEV) / 4f)) * level / 50 + 5) + defenceStatBonus;
		speedStat = Mathf.FloorToInt((speedIV + MortyDefs.SpeedBase(id) + Mathf.FloorToInt(Mathf.Sqrt(speedEV) / 4f)) * level / 50 + 5) + speedStatBonus;
	}

	public void GenerateIVs(int min = 0, int max = 16)
	{
		if (id.Equals("MortyDefault"))
		{
			attackIV = max;
			defenceIV = max;
			speedIV = max;
			hpIV = max;
		}
		else
		{
			attackIV = UnityEngine.Random.Range(min, max + 1);
			defenceIV = UnityEngine.Random.Range(min, max + 1);
			speedIV = UnityEngine.Random.Range(min, max + 1);
			hpIV = (attackIV + defenceIV + speedIV) / 3;
		}
	}

	public void GenerateIVsFromCombine(MortyData highLevel, MortyData lowLevel)
	{
		hpIV = Mathf.Clamp(highLevel.hpIV + Mathf.FloorToInt(lowLevel.hpIV / 2), 0, 16);
		attackIV = Mathf.Clamp(highLevel.attackIV + Mathf.FloorToInt(lowLevel.attackIV / 2), 0, 16);
		defenceIV = Mathf.Clamp(highLevel.defenceIV + Mathf.FloorToInt(lowLevel.defenceIV / 2), 0, 16);
		speedIV = Mathf.Clamp(highLevel.speedIV + Mathf.FloorToInt(lowLevel.speedIV / 2), 0, 16);
	}

	public void GenerateEVs(float level, eEVSetUp setUp)
	{
		int num = Mathf.FloorToInt(20000f * ((level - 5f) / 100f));
		if (num < 0)
		{
			num = 0;
		}
		switch (setUp)
		{
		case eEVSetUp.RANDOM:
			hpEV = UnityEngine.Random.Range(0, num + 1);
			attackEV = UnityEngine.Random.Range(0, num + 1);
			defenceEV = UnityEngine.Random.Range(0, num + 1);
			speedEV = UnityEngine.Random.Range(0, num + 1);
			break;
		case eEVSetUp.MAX:
			hpEV = num;
			attackEV = num;
			defenceEV = num;
			speedEV = num;
			break;
		}
	}

	public void GenerateEVsFromCombine(MortyData highLevel, MortyData lowLevel)
	{
		hpEV = Mathf.Clamp(highLevel.hpEV + Mathf.FloorToInt(lowLevel.hpEV / 2), 0, 65535);
		attackEV = Mathf.Clamp(highLevel.attackEV + Mathf.FloorToInt(lowLevel.attackEV / 2), 0, 65535);
		defenceEV = Mathf.Clamp(highLevel.defenceEV + Mathf.FloorToInt(lowLevel.defenceEV / 2), 0, 65535);
		speedEV = Mathf.Clamp(highLevel.speedEV + Mathf.FloorToInt(lowLevel.speedEV / 2), 0, 65535);
	}

	public void ApplyBattleEV(string defeatedMortyID)
	{
		ApplyHPEV(MortyDefs.HPBase(defeatedMortyID));
		ApplyAttackEV(MortyDefs.AttackBase(defeatedMortyID));
		ApplyDefenceEV(MortyDefs.DefenceBase(defeatedMortyID));
		ApplySpeedEV(MortyDefs.SpeedBase(defeatedMortyID));
	}

	private void ApplyHPEV(int value)
	{
		hpEV = Mathf.Min(65536, hpEV + value);
	}

	private void ApplyAttackEV(int value)
	{
		attackEV = Mathf.Min(65536, attackEV + value);
	}

	private void ApplyDefenceEV(int value)
	{
		defenceEV = Mathf.Min(65536, defenceEV + value);
	}

	private void ApplySpeedEV(int value)
	{
		speedEV = Mathf.Min(65536, speedEV + value);
	}

	public void AddStateBonus(string type, int amount)
	{
		switch (type)
		{
		case "Attack":
			attackStatBonus += amount;
			attackStat += amount;
			break;
		case "Defence":
			defenceStatBonus += amount;
			defenceStat += amount;
			break;
		case "Speed":
			speedStatBonus += amount;
			speedStat += amount;
			break;
		}
	}

	public int GetAvailableAttackSlot()
	{
		return attackIDs.IndexOf(null);
	}

	public void AddAttackToSlot(int index, string id)
	{
		attackIDs[index] = id;
		RestorePPFull(index);
	}

	public void RemoveAttackFromSlot(int index)
	{
		attackIDs[index] = null;
		attackPPs[index] = -1;
	}

	public void RandomizeAttacks()
	{
		List<KeyValuePair<int, string>> list = MortyDefs.Attacks(id);
		for (int i = 0; i < list.Count; i++)
		{
			KeyValuePair<int, string> keyValuePair = list[i];
			if (keyValuePair.Key > level)
			{
				break;
			}
			int index = i % 4;
			AddAttackToSlot(index, keyValuePair.Value);
		}
	}

	public float GetStatModifierValue(string stat)
	{
		switch (stat)
		{
		default:
			return 1f;
		case "Attack":
			return attackModifier;
		case "Defence":
			return defenceModifier;
		case "Speed":
			return speedModifier;
		case "Accuracy":
			return accuracyModifier;
		case "Evade":
			return evadeModifier;
		}
	}

	public int SetHP(int value)
	{
		if (TestDefs.PLAYER_TAKE_NO_DAMAGE && PlayerDefs.CurrentParty().Contains(this) && value < hp)
		{
			return 0;
		}
		int num = Mathf.Clamp(value, 0, hpStat);
		int result = num - hp;
		if (num == 0)
		{
			ResetAilments();
		}
		hp = num;
		return result;
	}

	public int IncrementHP(int value)
	{
		return SetHP(hp + value);
	}

	public float GetHPPercentage()
	{
		return (float)hp / (float)hpStat;
	}

	public int GetTotalRemainingPP()
	{
		int num = 0;
		for (int i = 0; i < attackPPs.Count; i++)
		{
			if (!string.IsNullOrEmpty(attackIDs[i]))
			{
				num += attackPPs[i];
			}
		}
		return num;
	}

	public int GetTotalMaxPP()
	{
		int num = 0;
		for (int i = 0; i < attackIDs.Count; i++)
		{
			if (!string.IsNullOrEmpty(attackIDs[i]))
			{
				num += AttackDefs.GetData(attackIDs[i]).pp;
			}
		}
		return num;
	}

	public void RestorePPFull(int index)
	{
		if (!string.IsNullOrEmpty(attackIDs[index]))
		{
			attackPPs[index] = AttackDefs.GetData(attackIDs[index]).pp;
		}
	}

	public void RestoreAllPP(float percent)
	{
		for (int i = 0; i < attackPPs.Count; i++)
		{
			if (!string.IsNullOrEmpty(attackIDs[i]))
			{
				int num = Mathf.CeilToInt((float)AttackDefs.GetData(attackIDs[i]).pp * percent);
				attackPPs[i] = Mathf.Clamp(attackPPs[i] + num, 0, AttackDefs.GetData(attackIDs[i]).pp);
			}
		}
	}

	public void RestoreAllPPFull()
	{
		for (int i = 0; i < attackPPs.Count; i++)
		{
			if (!string.IsNullOrEmpty(attackIDs[i]))
			{
				attackPPs[i] = AttackDefs.GetData(attackIDs[i]).pp;
			}
		}
	}

	public void ResetBattleModifiers()
	{
		attackModifier = 0f;
		defenceModifier = 0f;
		speedModifier = 0f;
		accuracyModifier = 0f;
		evadeModifier = 0f;
		ResetAilments();
	}

	public void ResetAilments()
	{
		isPoisoned = false;
		isParalysed = false;
	}

	public void ReviveFull()
	{
		ResetBattleModifiers();
		SetHP(hpStat);
		RestoreAllPPFull();
	}

	public void DebugLog()
	{
		string text = string.Format("---{0}({1})---\n", MortyDefs.Name(id), id);
		text += string.Format("Level: {0}\nXP: {1}\nHP: {2}/{3}\n", level, xp, hp, hpStat);
		text += string.Format("\n-Stats-\nAttack: {0}\nDefence: {1}\nSpeed: {2}\n", attackStat, defenceStat, speedStat);
		text += string.Format("\n-IV Stats-\nIV HP: {0}\nIV Attack: {1}\nIV Defence: {2}\nIV Speed: {3}\n", hpIV, attackIV, defenceIV, speedIV);
		text += string.Format("\n-EV Stats-\nEV HP: {0}\nEV Attack: {1}\nEV Defence: {2}\nEV Speed: {3}\n", hpEV, attackEV, defenceEV, speedEV);
		text += string.Format("\n-Bonus Stats-\nAttack: {0}\nDefence: {1}\nSpeed: {2}\n", attackStatBonus, defenceStatBonus, speedStatBonus);
		text += string.Format("\n-Battle Modifiers-\nAttack: {0}\nDefence: {1}\nSpeed: {2}\nAccuracy: {3}\nEvade: {4}\nPoisoned: {5}\nParalysed: {6}\n", attackModifier, defenceModifier, speedModifier, accuracyModifier, evadeModifier, isPoisoned, isParalysed);
		text += string.Format("\n-True Stats-\nAttack: {0}\nDefence: {1}\nSpeed: {2}\n", attack, defence, speed);
		text += "\n-Attacks-\n";
		for (int i = 0; i < attackIDs.Count; i++)
		{
			text += string.Format("{0}: {1}/{2}\n", AttackDefs.GetData(attackIDs[i]).name, attackPPs[i], AttackDefs.GetData(attackIDs[i]).pp);
		}
		text += "\n\n";
	}
}
