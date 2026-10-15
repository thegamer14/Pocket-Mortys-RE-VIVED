using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPAttackDefs : MonoBehaviour
{
	public const string kAttackIDStruggle = "AttackStruggle";

	private static JSONNode _infoJSON;

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["attack_id"].Value == id);
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Attack, id, "name");
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Attack, id, "description");
	}

	public static string ElementType(string id)
	{
		return GetNode(id)["element"];
	}

	public static int PP(string id)
	{
		return GetNode(id)["pp_stat"].AsInt;
	}

	public static JSONNode EffectsData(string id)
	{
		return GetNode(id)["effects"];
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "AttackInfo")).text);
	}

	public static void Remove()
	{
		_infoJSON = null;
	}

	public static eAttackCatagory GetAttackCatagory(string attackID)
	{
		JSONNode jSONNode = EffectsData(attackID);
		if (!string.IsNullOrEmpty(Description(attackID)))
		{
			return eAttackCatagory.Golden;
		}
		if (jSONNode.Childs.Where((JSONNode e) => e["type"].Value == eAttackEffectType.Stat.ToString() && e["power"].AsFloat > 0f && e["to_self"].AsBool).Count() > 0)
		{
			return eAttackCatagory.Buff;
		}
		if (jSONNode.Childs.Where((JSONNode e) => e["type"].Value == eAttackEffectType.Stat.ToString() && e["power"].AsFloat < 0f && !e["to_self"].AsBool).Count() > 0)
		{
			return eAttackCatagory.Debuff;
		}
		return eAttackCatagory.Normal;
	}

	public static string GetPopUpValue1(string attackID)
	{
		JSONNode jSONNode = EffectsData(attackID);
		switch (GetAttackCatagory(attackID))
		{
		default:
			return null;
		case eAttackCatagory.Normal:
			return jSONNode[0]["power"].Value;
		case eAttackCatagory.Buff:
		case eAttackCatagory.Debuff:
			return TextDefs.GetMortyStatTextForType(jSONNode[0]["stat"].Value);
		}
	}

	public static string GetPopUpValue2(string attackID)
	{
		JSONNode jSONNode = EffectsData(attackID);
		switch (GetAttackCatagory(attackID))
		{
		default:
			return null;
		case eAttackCatagory.Normal:
			return jSONNode[0]["accuracy"].AsFloat * 100f + "%";
		case eAttackCatagory.Buff:
		case eAttackCatagory.Debuff:
			return GetPopupValueStatEffectTitle(jSONNode);
		}
	}

	public static string GetPopupValueStatEffectTitle(JSONNode effectsData)
	{
		if (Mathf.Abs(effectsData[0]["power"].AsFloat) <= 1f)
		{
			return TextDefs.ATTACK_STAT_EFFECTIVENESS_WEAK;
		}
		if (Mathf.Abs(effectsData[0]["power"].AsFloat) <= 2f)
		{
			return TextDefs.ATTACK_STAT_EFFECTIVENESS_MEDIUM;
		}
		return TextDefs.ATTACK_STAT_EFFECTIVENESS_STRONG;
	}
}
