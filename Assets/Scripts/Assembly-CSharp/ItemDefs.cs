using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class ItemDefs
{
	public const string kCaptureSuccessAnimationID = "CaptureSuccess";

	public const string kCaptureFailedAnimationID = "CaptureFailed";

	public const string kMortyChipID = "ItemMortyChip";

	public const string kSeriumID = "ItemSerum";

	public const string kGreatSeriumID = "ItemGreatSerum";

	public const string kSensationalSeriumID = "ItemSensationalSerum";

	public const string kPureSeriumID = "ItemPureSerum";

	public const string kHalzingerID = "ItemHalzinger";

	public const string kPureHalzingerID = "ItemHalzingerPure";

	public const string kPoisonCureID = "ItemPoisonCure";

	public const string kParalysisCureID = "ItemParalysisCure";

	public const string kFullRecoverID = "ItemFullRecover";

	public const string kMrMeeseekID = "ItemMrMeeseek";

	public const string kCouponID = "ItemCoupon";

	public const string kTournamentTicketID = "ItemTournamentTicket";

	public const string kCourierFlapID = "ItemCourierFlap";

	public const string kEffectTypeCapture = "Capture";

	public const string kEffectTypeRestoreHP = "RestoreHP";

	public const string kEffectTypeDamageHP = "DamageHP";

	public const string kEffectTypeCure = "Cure";

	public const string kEffectTypeRestorePP = "RestorePP";

	public const string kEffectTypeRevive = "Revive";

	public const string kEffectTypeStatIncrease = "StatIncrease";

	public const string kEffectTypeCoupon = "Coupon";

	public const string kEffectTypePortal = "Portal";

	public const float kCouponChance = 0.05f;

	private static JSONNode _infoJSON;

	private static Dictionary<eItemType, List<string>> _idDict;

	public static int idCount
	{
		get
		{
			return _infoJSON.Count;
		}
	}

	public static string AssetID(string id)
	{
		return _infoJSON[id]["assetid"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Item, id, "name");
	}

	public static eItemType Type(string id)
	{
		return (eItemType)Enum.Parse(typeof(eItemType), _infoJSON[id]["type"]);
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Item, id, "description");
	}

	public static int Cost(string id)
	{
		return _infoJSON[id]["cost"].AsInt;
	}

	public static int DisplayOrder(string id)
	{
		return _infoJSON[id]["displayorder"].AsInt;
	}

	public static int PurchaseBadgeReq(string id)
	{
		return _infoJSON[id]["purchasebadgereq"].AsInt;
	}

	public static int RewardBadgeReq(string id)
	{
		return _infoJSON[id]["rewardbadgereq"].AsInt;
	}

	public static int Rarity(string id)
	{
		return _infoJSON[id]["rarity"].AsInt;
	}

	public static string EffectType(string id)
	{
		return _infoJSON[id]["effecttype"];
	}

	public static float EffectValue(string id)
	{
		return _infoJSON[id]["effectvalue"].AsFloat;
	}

	public static string EffectStatType(string id)
	{
		return _infoJSON[id]["effectstattype"];
	}

	public static bool UsableInWorld(string id)
	{
		return _infoJSON[id]["usableinworld"].AsBool;
	}

	public static bool UsableInCraft(string id)
	{
		return _infoJSON[id]["usableincraft"].AsBool;
	}

	public static bool UsableInBattle(string id)
	{
		return _infoJSON[id]["usableinbattle"].AsBool;
	}

	public static bool UseOnSelf(string id)
	{
		return _infoJSON[id]["useonself"].AsBool;
	}

	public static string AttackerAnimation(string id)
	{
		return _infoJSON[id]["attackeranimation"];
	}

	public static string DefenderAnimation(string id)
	{
		return _infoJSON[id]["defenderanimation"];
	}

	public static int BattlePoints(string id)
	{
		return _infoJSON[id]["battlepoints"].AsInt;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "ItemInfo")).text);
		Dictionary<eItemType, List<string>> dictionary = new Dictionary<eItemType, List<string>>();
		dictionary.Add(eItemType.ALL, new List<string>());
		_idDict = dictionary;
		for (int i = 0; i < idCount; i++)
		{
			string iD = GetID(i);
			eItemType key = Type(iD);
			if (_idDict.ContainsKey(key))
			{
				_idDict[key].Add(iD);
			}
			else
			{
				_idDict.Add(key, new List<string> { iD });
			}
			_idDict[eItemType.ALL].Add(iD);
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
		_idDict = null;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && _infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return _infoJSON[index]["id"];
	}

	public static List<string> GetBadgeLockedPurchasableItemIDs()
	{
		int badgeCount = PlayerDefs.BadgeCount;
		List<string> list = new List<string>();
		for (int i = 0; i < idCount; i++)
		{
			string iD = GetID(i);
			int num = PurchaseBadgeReq(iD);
			if (num != -1 && num <= badgeCount)
			{
				list.Add(iD);
			}
		}
		return list;
	}

	public static string GetRandomBadgeLockedRewardID(eItemType definedType, bool applyPlayerBadgeCheck, bool applyRarityCheck, List<string> excludeIDs = null)
	{
		string text = GetBadgeLockedRewardableItemIDs(definedType, applyPlayerBadgeCheck, applyRarityCheck, excludeIDs).PickRandom();
		if (string.IsNullOrEmpty(text))
		{
			text = GetBadgeLockedRewardableItemIDs(definedType, applyPlayerBadgeCheck, applyRarityCheck, null).PickRandom();
		}
		return text;
	}

	private static List<string> GetBadgeLockedRewardableItemIDs(eItemType definedType, bool applyPlayerBadgeCheck, bool applyRarityCheck, List<string> excludeIDs)
	{
		List<string> list = _idDict[definedType];
		List<string> list2 = new List<string>();
		int badgeCount = PlayerDefs.BadgeCount;
		int num = UnityEngine.Random.Range(0, 100);
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			int num2 = RewardBadgeReq(text);
			if ((excludeIDs == null || !excludeIDs.Contains(text)) && num2 != -1 && (!applyPlayerBadgeCheck || num2 <= badgeCount) && (!applyRarityCheck || num <= Rarity(text)))
			{
				list2.Add(text);
			}
		}
		return list2;
	}

	public static string GetRandomBracketLockedRewardableItemID(int rarityBracketLow, int rarityBracketHigh, Dictionary<string, int> itemCountChecks = null)
	{
		string text = GetBracketLockedRewardableItemIDs(rarityBracketLow, rarityBracketHigh, itemCountChecks).PickRandom();
		if (string.IsNullOrEmpty(text) && itemCountChecks != null)
		{
			text = GetBracketLockedRewardableItemIDs(rarityBracketLow, rarityBracketHigh).PickRandom();
		}
		return text;
	}

	private static List<string> GetBracketLockedRewardableItemIDs(int rarityBracketLow, int rarityBracketHigh, Dictionary<string, int> itemCountChecks = null)
	{
		List<string> list = _idDict[eItemType.ITEM];
		List<string> list2 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			int num = Rarity(text);
			if (RewardBadgeReq(text) >= 0 && (itemCountChecks == null || !itemCountChecks.ContainsKey(text) || itemCountChecks[text] < 10) && num >= rarityBracketLow && num <= rarityBracketHigh)
			{
				list2.Add(text);
			}
		}
		return list2;
	}

	public static List<string> GetItemsOfType(eItemType type, Dictionary<string, int> items)
	{
		List<string> list = items.Keys.ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (Type(list[num]) != type)
			{
				list.RemoveAt(num);
			}
		}
		return list;
	}

	public static bool CheckCanApplyItemToMorty(string itemID, MortyData mortyData, out string resultText)
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
		case "Portal":
			result = false;
			resultText = TextDefs.ITEM_APPLY_FAILED_COURIER_FLAP;
			break;
		}
		return result;
	}

	public static bool ApplyItemToMorty(string itemID, Dictionary<string, int> itemsList, MortyData usingMortyData, MortyData defendingMortyData, string UserPronounYours, out string resultText)
	{
		bool flag = false;
		string text = EffectType(itemID);
		switch (text)
		{
		default:
			resultText = null;
			break;
		case "Capture":
			flag = ApplyCaptureItemToMorty(itemID, defendingMortyData, out resultText);
			break;
		case "RestoreHP":
			flag = ApplyRestoreHPItemToMorty(itemID, usingMortyData, UserPronounYours, out resultText);
			break;
		case "DamageHP":
			flag = ApplyDamageHPItemToMorty(itemID, defendingMortyData, out resultText);
			break;
		case "Cure":
			flag = ApplyCureItemToMorty(itemID, usingMortyData, UserPronounYours, out resultText);
			break;
		case "RestorePP":
			flag = ApplyPPItemToMorty(itemID, usingMortyData, UserPronounYours, out resultText);
			break;
		case "Revive":
			flag = ApplyReviveItemToMorty(itemID, usingMortyData, UserPronounYours, out resultText);
			break;
		case "StatIncrease":
			flag = ApplyStatIncreaseItemToMorty(itemID, usingMortyData, UserPronounYours, out resultText);
			break;
		case "Portal":
			flag = ApplyPortal(out resultText);
			break;
		}
		if (flag || text.Equals("Capture"))
		{
			itemsList[itemID]--;
			if (itemsList[itemID] <= 0)
			{
				itemsList.Remove(itemID);
			}
		}
		return flag;
	}

	private static bool ApplyCaptureItemToMorty(string itemID, MortyData defendingMortyData, out string resultText)
	{
		resultText = null;
		if (SharedStatInfo.battleInfo != null && SharedStatInfo.battleInfo.isTutorial)
		{
			return true;
		}
		int num = UnityEngine.Random.Range(0, 255);
		int num2 = Mathf.FloorToInt((float)(defendingMortyData.hpStat * 255 * 4) / ((float)defendingMortyData.hp * EffectValue(itemID)));
		return num2 >= num;
	}

	private static bool ApplyRestoreHPItemToMorty(string itemID, MortyData mortyData, string UserPronounYours, out string resultText)
	{
		int num = (int)EffectValue(itemID);
		int num2 = Mathf.Clamp(mortyData.hpStat - mortyData.hp, 0, (num <= 0) ? mortyData.hpStat : num);
		mortyData.IncrementHP(num2);
		resultText = string.Format(TextDefs.ITEM_APPLY_HP, UserPronounYours, MortyDefs.Name(mortyData.id), num2);
		return true;
	}

	private static bool ApplyDamageHPItemToMorty(string itemID, MortyData mortyData, out string resultText)
	{
		int num = (int)EffectValue(itemID);
		int num2 = ((num <= 0) ? mortyData.hp : Mathf.Clamp(0, mortyData.hp, num));
		mortyData.IncrementHP(-num2);
		resultText = null;
		return true;
	}

	private static bool ApplyCureItemToMorty(string itemID, MortyData mortyData, string UserPronoun, out string resultText)
	{
		if (EffectStatType(itemID) == "All")
		{
			mortyData.ResetAilments();
			mortyData.SetHP(mortyData.hpStat);
			resultText = string.Format(TextDefs.ITEM_APPLY_CURE_FULL, UserPronoun, MortyDefs.Name(mortyData.id));
			return true;
		}
		if (EffectStatType(itemID) == "Poison" && mortyData.isPoisoned)
		{
			mortyData.isPoisoned = false;
			resultText = string.Format(TextDefs.ITEM_APPLY_POISONED, UserPronoun, MortyDefs.Name(mortyData.id));
			return true;
		}
		if (EffectStatType(itemID) == "Paralysis" && mortyData.isParalysed)
		{
			mortyData.isParalysed = false;
			resultText = string.Format(TextDefs.ITEM_APPLY_PARALYSED, UserPronoun, MortyDefs.Name(mortyData.id));
			return true;
		}
		resultText = null;
		return false;
	}

	private static bool ApplyPPItemToMorty(string itemID, MortyData mortyData, string UserPronoun, out string resultText)
	{
		float num = EffectValue(itemID);
		mortyData.RestoreAllPP(num / 100f);
		resultText = string.Format(TextDefs.ITEM_APPLY_PP, UserPronoun, MortyDefs.Name(mortyData.id), num + "%");
		return true;
	}

	private static bool ApplyReviveItemToMorty(string itemID, MortyData mortyData, string UserPronoun, out string resultText)
	{
		float num = EffectValue(itemID);
		mortyData.SetHP(Mathf.FloorToInt((float)mortyData.hpStat / 100f * num));
		resultText = string.Format(TextDefs.ITEM_APPLY_REVIVE, UserPronoun, MortyDefs.Name(mortyData.id), num);
		return true;
	}

	private static bool ApplyStatIncreaseItemToMorty(string itemID, MortyData mortyData, string UserPronounYours, out string resultText)
	{
		int amount = Mathf.FloorToInt(EffectValue(itemID));
		string text = EffectStatType(itemID);
		resultText = string.Format(TextDefs.ITEM_APPLY_STAT_INCREASE, UserPronounYours, MortyDefs.Name(mortyData.id), text);
		bool result = true;
		switch (text)
		{
		case "Level":
			result = MortyDefs.IncrementLevel(mortyData, true, UserPronounYours, out resultText);
			break;
		case "Attack":
			mortyData.AddStateBonus("Attack", amount);
			break;
		case "Defence":
			mortyData.AddStateBonus("Defence", amount);
			break;
		case "Speed":
			mortyData.AddStateBonus("Speed", amount);
			break;
		}
		return result;
	}

	private static bool ApplyPortal(out string resultText)
	{
		App.saveData.Delete(eSaveDataType.SEGMENTED_WORLD);
		resultText = null;
		return true;
	}

	public static int ItemsIndividualCount(Dictionary<string, int> items, string itemID)
	{
		if (!string.IsNullOrEmpty(itemID) && items.ContainsKey(itemID))
		{
			return items[itemID];
		}
		return 0;
	}

	public static int ItemsObtainableCount(Dictionary<string, int> items, string itemID, int amount)
	{
		string text = EffectType(itemID);
		if (!string.IsNullOrEmpty(text) && text.Equals("Coupon"))
		{
			return Mathf.Min(99999 - App.saveData.player.coupons, amount);
		}
		return Mathf.Min(10 - ItemsIndividualCount(items, itemID), amount);
	}

	public static bool ObtainItem(Dictionary<string, int> items, string itemID, int amount, out int resultAmount, out string resultText)
	{
		string text = EffectType(itemID);
		if (!string.IsNullOrEmpty(text) && text.Equals("Coupon"))
		{
			PlayerDefs.AddCoupon(amount, out resultAmount, out resultText);
			if (resultAmount < amount)
			{
				return false;
			}
		}
		else
		{
			resultAmount = ItemsObtainableCount(items, itemID, amount);
			if (items.ContainsKey(itemID))
			{
				items[itemID] += resultAmount;
			}
			else
			{
				items.Add(itemID, resultAmount);
			}
			if (resultAmount < amount)
			{
				resultText = string.Format(TextDefs.ITEM_OBTAIN_NO_SPACE, Name(itemID));
				return false;
			}
		}
		resultText = null;
		return true;
	}

	public static bool PurchaseItem(Dictionary<string, int> items, string itemID, out string resultText)
	{
		int num = Cost(itemID);
		if (App.saveData.player.coins >= num)
		{
			int resultAmount;
			if (ObtainItem(items, itemID, 1, out resultAmount, out resultText))
			{
				PlayerDefs.RemoveCoin(num);
				resultText = string.Format(TextDefs.ITEM_BOUGHT, Name(itemID));
				return true;
			}
			return false;
		}
		resultText = string.Format(TextDefs.ITEM_BUY_NO_COIN, Name(itemID));
		return false;
	}

	public static void RemoveItem(Dictionary<string, int> items, string id)
	{
		if (ItemsIndividualCount(items, id) > 0)
		{
			items[id]--;
			if (ItemsIndividualCount(items, id) == 0)
			{
				items.Remove(id);
			}
		}
	}

	public static Dictionary<string, int> GetBalancedTrainerItems()
	{
		if (UnityEngine.Random.value < 0.25f)
		{
			if (PlayerDefs.BadgeCount > 13)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				dictionary.Add("ItemGreatSerum", 1);
				return dictionary;
			}
			if (PlayerDefs.BadgeCount > 6)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				dictionary.Add("ItemSerum", 1);
				return dictionary;
			}
		}
		return new Dictionary<string, int>();
	}
}
