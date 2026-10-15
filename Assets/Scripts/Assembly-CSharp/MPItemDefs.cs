using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPItemDefs : MonoBehaviour
{
	public const string kCaptureSuccessAnimationID = "CaptureSuccess";

	public const string kCaptureFailedAnimationID = "CaptureFailed";

	public const int kMaxIndividualItemCountInBag = 10;

	public const string kCouponID = "ItemCouponMP";

	public const string kMortyChipID = "ItemMortyChip";

	public const string kPoisonCureID = "ItemPoisonCure";

	public const string kParalysisCureID = "ItemParalysisCure";

	public const string kMrMeeseekID = "ItemMrMeeseek";

	public const string kEffectTypeCapture = "Capture";

	public const string kEffectTypeRestoreHP = "RestoreHP";

	public const string kEffectTypeDamageHP = "DamageHP";

	public const string kEffectTypeCure = "Cure";

	public const string kEffectTypeRestorePP = "RestorePP";

	public const string kEffectTypeRevive = "Revive";

	public const string kEffectTypeStatIncrease = "StatIncrease";

	public const string kEffectTypeCoupon = "Coupon";

	private static JSONNode _infoJSON;

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["item_id"].Value == id);
	}

	public static string AssetID(string id)
	{
		return GetNode(id)["asset_id"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Item, id, "name");
	}

	public static eMPItemType Type(string id)
	{
		return (eMPItemType)Enum.Parse(typeof(eMPItemType), GetNode(id)["type"]);
	}

	public static int Cost(string id)
	{
		return GetNode(id)["cost"].AsInt;
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Item, id, "description");
	}

	public static int DisplayOrder(string id)
	{
		return GetNode(id)["sortorder"].AsInt;
	}

	public static int Rarity(string id)
	{
		return GetNode(id)["rarity"].AsInt;
	}

	public static string EffectType(string id)
	{
		return GetNode(id)["effect_type"];
	}

	public static string EffectStatType(string id)
	{
		return GetNode(id)["effect_stat"];
	}

	public static float EffectValue(string id)
	{
		return GetNode(id)["effect_value"].AsFloat;
	}

	public static bool UsableInWorld(string id)
	{
		return GetNode(id)["use_world"].AsBool;
	}

	public static bool UsableInCraft(string id)
	{
		return GetNode(id)["use_craft"].AsBool;
	}

	public static bool UsableInBattle(string id)
	{
		return GetNode(id)["use_battle"].AsBool;
	}

	public static bool UseOnSelf(string id)
	{
		return GetNode(id)["use_self"].AsBool;
	}

	public static string AttackerAnimation(string id)
	{
		return (!(GetNode(id)["attacker_animation"].Value == "null")) ? GetNode(id)["attacker_animation"] : null;
	}

	public static string DefenderAnimation(string id)
	{
		return (!(GetNode(id)["defender_animation"].Value == "null")) ? GetNode(id)["defender_animation"] : null;
	}

	public static int PlayerLevel(string id)
	{
		return GetNode(id)["store_level"].AsInt;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "ItemInfo")).text);
	}

	public static void Remove()
	{
		_infoJSON = null;
	}

	public static bool CanItemBeApplyedToMorty(string itemID, MPOpponentMortyData mortyData, out string resultText)
	{
		bool result = true;
		resultText = null;
		switch (EffectType(itemID))
		{
		case "RestoreHP":
			if (mortyData.isFainted)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_HP_FAILED_FAINTED;
			}
			else if (mortyData.hp >= mortyData.hpStat)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_HP_FAILED_FULL;
			}
			break;
		case "Cure":
			if (mortyData.isFainted)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_CURE_FAILED_FAINTED;
			}
			else if (EffectStatType(itemID) == "All" && mortyData.hp >= mortyData.hpStat && !mortyData.isPoisoned && !mortyData.isParalysed)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_CURE_FAILED_ALL;
			}
			else if (EffectStatType(itemID) == "Poison" && !mortyData.isPoisoned)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_CURE_FAILED_POISON;
			}
			else if (EffectStatType(itemID) == "Paralysis" && !mortyData.isParalysed)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_CURE_FAILED_PARALYSIS;
			}
			break;
		case "RestorePP":
			if (mortyData.isFainted)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_PP_FAILED_FAINTED;
			}
			else if (mortyData.GetTotalRemainingPP() >= mortyData.GetTotalMaxPP())
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_PP_FAILED_FULL;
			}
			break;
		case "Revive":
			if (!mortyData.isFainted)
			{
				result = false;
				resultText = TextDefs.ITEM_APPLY_FAILED_ALIVE;
			}
			break;
		}
		return result;
	}

	public static string GetItemUsedResultText(string itemID, bool isSuccess, int itemEffectValue, string attackerMortyID, string defenderMortyID, string userPronounYours, MPRewardData rewardData)
	{
		string result = null;
		switch (EffectType(itemID))
		{
		case "RestoreHP":
			result = string.Format(TextDefs.ITEM_APPLY_HP, userPronounYours, MPMortyDefs.Name(attackerMortyID), itemEffectValue);
			break;
		case "Cure":
			switch (EffectStatType(itemID))
			{
			case "All":
				result = string.Format(TextDefs.ITEM_APPLY_CURE_FULL, userPronounYours, MPMortyDefs.Name(attackerMortyID));
				break;
			case "Poison":
				result = string.Format(TextDefs.ITEM_APPLY_POISONED, userPronounYours, MPMortyDefs.Name(attackerMortyID));
				break;
			case "Paralysis":
				result = string.Format(TextDefs.ITEM_APPLY_PARALYSED, userPronounYours, MPMortyDefs.Name(attackerMortyID));
				break;
			}
			break;
		case "RestorePP":
			result = string.Format(TextDefs.ITEM_APPLY_PP, userPronounYours, MPMortyDefs.Name(attackerMortyID), string.Format("{0}%", itemEffectValue));
			break;
		case "Revive":
			result = string.Format(TextDefs.ITEM_APPLY_REVIVE, userPronounYours, MPMortyDefs.Name(attackerMortyID), itemEffectValue);
			break;
		case "StatIncrease":
		{
			string text = EffectStatType(itemID);
			result = ((text != null && text == "Level") ? ((!isSuccess) ? string.Format(TextDefs.MORTY_MAX_LEVEL_REACHED, MPMortyDefs.Name(attackerMortyID)) : string.Format(TextDefs.BATTLE_REWARD_LEVEL, userPronounYours, MPMortyDefs.Name(attackerMortyID), itemEffectValue)) : ((itemEffectValue <= 0) ? MPTextDefs.MP_ITEM_APPLY_STAT_FAIL : string.Format(TextDefs.ITEM_APPLY_STAT_INCREASE, userPronounYours, MPMortyDefs.Name(attackerMortyID), TextDefs.GetMortyStatTextForType(EffectStatType(itemID)))));
			break;
		}
		case "Capture":
			if (isSuccess)
			{
				result = ((rewardData == null || rewardData.isMortyLimit) ? string.Format(TextDefs.MORTY_CAPTURED_PARTY_AND_STORAGE_FULL, MPMortyDefs.Name(defenderMortyID)) : ((!rewardData.isAddedToDeck) ? string.Format(TextDefs.MORTY_CAPTURED_STORAGE, MPMortyDefs.Name(defenderMortyID)) : string.Format(TextDefs.MORTY_CAPTURED_PARTY, MPMortyDefs.Name(defenderMortyID))));
			}
			break;
		}
		return result;
	}

	public static SortedList<int, string> GetOrderedListOfItems(Dictionary<string, int> items, eMPItemType type)
	{
		SortedList<int, string> sortedList = new SortedList<int, string>();
		foreach (KeyValuePair<string, int> item in items)
		{
			if (Type(item.Key) == type)
			{
				sortedList.Add(DisplayOrder(item.Key), item.Key);
			}
		}
		return sortedList;
	}

	public static int ItemsIndividualCount(Dictionary<string, int> items, string itemID)
	{
		if (!string.IsNullOrEmpty(itemID) && items.ContainsKey(itemID))
		{
			return items[itemID];
		}
		return 0;
	}

	public static SortedList<int, string> GetPurchaseableItems(int forPlayerLevel)
	{
		SortedList<int, string> sortedList = new SortedList<int, string>();
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			string value = _infoJSON[i]["item_id"].Value;
			int num = PlayerLevel(value);
			if (Type(value) == eMPItemType.ITEM && num > -1 && num <= forPlayerLevel)
			{
				sortedList.Add(DisplayOrder(value), value);
			}
		}
		return sortedList;
	}
}
