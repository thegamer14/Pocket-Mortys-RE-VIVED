using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class AttackDefs
{
	public const string kAttackIDStruggle = "AttackStruggle";

	private static Dictionary<string, AttackData> _attackDict;

	public static void SetUp()
	{
		JSONNode jSONNode = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "AttackInfo")).text);
		_attackDict = new Dictionary<string, AttackData>();
		for (int i = 0; i < jSONNode.Count; i++)
		{
			_attackDict.Add(jSONNode[i]["id"], new AttackData(jSONNode[i]));
		}
	}

	public static void Remove()
	{
		_attackDict = null;
	}

	public static void Localize()
	{
		if (_attackDict != null)
		{
			string[] array = _attackDict.Keys.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				_attackDict[array[i]].Localize();
			}
		}
	}

	public static AttackData GetData(string id)
	{
		if (_attackDict.ContainsKey(id))
		{
			return _attackDict[id];
		}
		return null;
	}

	public static eAttackCatagory GetAttackCatagory(string id)
	{
		if (!_attackDict.ContainsKey(id))
		{
			return eAttackCatagory.Normal;
		}
		return GetAttackCatagory(_attackDict[id]);
	}

	public static eAttackCatagory GetAttackCatagory(AttackData attackData)
	{
		if (!string.IsNullOrEmpty(attackData.description))
		{
			return eAttackCatagory.Golden;
		}
		if (attackData.effectDatas.Where((AttackEffectData e) => e.type == eAttackEffectType.Stat && e.power > 0f && e.toSelf).Count() > 0)
		{
			return eAttackCatagory.Buff;
		}
		if (attackData.effectDatas.Where((AttackEffectData e) => e.type == eAttackEffectType.Stat && e.power < 0f && !e.toSelf).Count() > 0)
		{
			return eAttackCatagory.Debuff;
		}
		return eAttackCatagory.Normal;
	}

	public static string GetAttackStatEffectTitle(AttackEffectData effectData)
	{
		if (Mathf.Abs(effectData.power) <= 1f)
		{
			return TextDefs.ATTACK_STAT_EFFECTIVENESS_WEAK;
		}
		if (Mathf.Abs(effectData.power) <= 2f)
		{
			return TextDefs.ATTACK_STAT_EFFECTIVENESS_MEDIUM;
		}
		return TextDefs.ATTACK_STAT_EFFECTIVENESS_STRONG;
	}

	public static string GetPopUpValue1(string attackID)
	{
		AttackData data = GetData(attackID);
		switch (GetAttackCatagory(attackID))
		{
		default:
			return null;
		case eAttackCatagory.Normal:
			return data.effectDatas[0].power.ToString();
		case eAttackCatagory.Buff:
		case eAttackCatagory.Debuff:
			return TextDefs.GetMortyStatTextForType(data.effectDatas[0].stat);
		}
	}

	public static string GetPopUpValue2(string attackID)
	{
		AttackData data = GetData(attackID);
		switch (GetAttackCatagory(attackID))
		{
		default:
			return null;
		case eAttackCatagory.Normal:
			return data.effectDatas[0].accuracy * 100f + "%";
		case eAttackCatagory.Buff:
		case eAttackCatagory.Debuff:
			return GetAttackStatEffectTitle(data.effectDatas[0]);
		}
	}
}
