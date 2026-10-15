using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class MortyDefs
{
	public const string MORTY_NUMBER = "#{0}";

	public const int kMortyLevelMax = 100;

	public const int kMortyRarityMax = 100;

	public const int kAttacksPerMortyMax = 4;

	public const int kEvolutionTierMax = 10;

	public const int kStatMax = 256;

	public const int kIVMax = 16;

	public const int kIVRestictedMax = 10;

	public const int kEVMax = 65535;

	public const int kEVGeneratedMax = 20000;

	public const int kBattleModifierMin = -6;

	public const int kBattleModifierMax = 6;

	public const float kAttackCriticalChance = 0.1f;

	public const float kAttackDamageModifierMin = 0.85f;

	public const float kAttackCriticalModifier = 1.75f;

	public const float kAttackWeakTypeModifier = 0.75f;

	public const float kAttackStrongTypeModifier = 1.75f;

	private const float _kPoisonDamageAmount = 10f;

	public const float kParalysisChance = 0.2f;

	public const float kTrainedMortyXPModifier = 1.5f;

	public const int kDimensionNumberMin = 100;

	public const int kDimensionNumberMax = 999;

	public const int kEggHatchAtLevel = 20;

	public const string kMortyDefaultID = "MortyDefault";

	public const string kMortyEggID = "MortyEgg";

	public const string kMortyMorticiaID = "MortyMorticia";

	public const string kTutorialWildID = "MortyScruffy";

	public const string kStatIDLevel = "Level";

	public const string kStatIDHP = "HP";

	public const string kStatIDAttack = "Attack";

	public const string kStatIDDefence = "Defence";

	public const string kStatIDSpeed = "Speed";

	public const string kStatIDAccuracy = "Accuracy";

	public const string kStatIDEvade = "Evade";

	public const string kStatIDAll = "All";

	public const string kStatIDPoison = "Poison";

	public const string kStatIDParalysis = "Paralysis";

	public const string kTypeIDRock = "Rock";

	public const string kTypeIDPaper = "Paper";

	public const string kTypeIDScissors = "Scissors";

	private static JSONNode infoJSON;

	public static int idCount
	{
		get
		{
			return infoJSON.Count;
		}
	}

	public static bool IsNew(string id)
	{
		return infoJSON[id]["isnew"].AsBool;
	}

	public static string AssetID(string id)
	{
		return infoJSON[id]["assetid"];
	}

	public static int Number(string id)
	{
		return infoJSON[id]["number"].AsInt;
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "name");
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "description");
	}

	public static string Characteristic(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "characteristic");
	}

	public static string Height(string id)
	{
		return infoJSON[id]["height"];
	}

	public static string Weight(string id)
	{
		return infoJSON[id]["weight"];
	}

	public static string Evolution(string id)
	{
		return infoJSON[id]["evolution"];
	}

	public static int EvolutionTier(string id)
	{
		return infoJSON[id]["evolutiontier"].AsInt;
	}

	public static int BadgeReq(string id)
	{
		return infoJSON[id]["badgereq"].AsInt;
	}

	public static bool IsRare(string id)
	{
		return infoJSON[id]["israre"].AsBool;
	}

	public static string ElementType(string id)
	{
		return infoJSON[id]["elementtype"];
	}

	public static int DefeatedXP(string id)
	{
		return infoJSON[id]["defeatedxp"].AsInt;
	}

	public static int HPBase(string id)
	{
		return infoJSON[id]["hpbase"].AsInt;
	}

	public static int AttackBase(string id)
	{
		return infoJSON[id]["attackbase"].AsInt;
	}

	public static int DefenceBase(string id)
	{
		return infoJSON[id]["defencebase"].AsInt;
	}

	public static int SpeedBase(string id)
	{
		return infoJSON[id]["speedbase"].AsInt;
	}

	public static int StatTotal(string id)
	{
		return infoJSON[id]["stattotal"].AsInt;
	}

	public static List<KeyValuePair<int, string>> Attacks(string id)
	{
		List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
		string[] array = ((string)infoJSON[id]["attacks"]).Split(new string[1] { ", " }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(':');
			string value = array2[0];
			int key = int.Parse(array2[1]);
			list.Add(new KeyValuePair<int, string>(key, value));
		}
		return list;
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "MortyInfo")).text);
	}

	public static void Remove()
	{
		infoJSON = null;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}

	public static string GetId(int index)
	{
		if (TestDefs.FORCE_ALL_MORTY_IDS != null)
		{
			return TestDefs.FORCE_ALL_MORTY_IDS;
		}
		if (index > -1 && index < infoJSON.Count)
		{
			return infoJSON[index]["id"];
		}
		return null;
	}

	public static string GetRandomID(RandomMortyParams mParams, bool forceOnFail = false)
	{
		string text = GetIDs(mParams).PickRandom();
		if (text != null || !forceOnFail)
		{
			return text;
		}
		return GetId(UnityEngine.Random.Range(1, idCount));
	}

	private static List<string> GetIDs(RandomMortyParams mParams)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < idCount; i++)
		{
			string id = GetId(i);
			if (mParams.isNewOnly && !IsNew(id))
			{
				continue;
			}
			int num = BadgeReq(id);
			if (num < 0 || (mParams.badgeLevelLock && num > PlayerDefs.BadgeCount) || (mParams.lockToUnobtained && PlayerDefs.HasCaughtMorty(id)))
			{
				continue;
			}
			bool flag = IsRare(id);
			if ((mParams.rarityLock == eMortyRarityCheck.RARE && !flag) || (mParams.rarityLock == eMortyRarityCheck.NOT_RARE && flag))
			{
				continue;
			}
			int value = EvolutionTier(id);
			if (mParams.evolutionTierLock == null || flag || mParams.evolutionTierLock.ContainsValue(value))
			{
				int value2 = StatTotal(id);
				if (mParams.statTotalLock == null || flag || mParams.statTotalLock.ContainsValue(value2))
				{
					list.Add(id);
				}
			}
		}
		return list;
	}

	public static List<List<string>> GetIDListsForWorld()
	{
		if (TestDefs.FORCE_ALL_MORTY_IDS != null)
		{
			List<List<string>> list = new List<List<string>>();
			list.Add(new List<string> { TestDefs.FORCE_ALL_MORTY_IDS });
			return list;
		}
		RandomMortyParams randomMortyParams = new RandomMortyParams();
		randomMortyParams.badgeLevelLock = true;
		randomMortyParams.rarityLock = eMortyRarityCheck.NOT_RARE;
		randomMortyParams.evolutionTierLock = new Range<int>(1, 1);
		List<string> iDs = GetIDs(randomMortyParams);
		List<List<string>> list2 = new List<List<string>>();
		for (int i = 0; i < 3; i++)
		{
			string item = iDs.PickRandom();
			list2.Add(new List<string> { item });
			iDs.Remove(item);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			string text = list2[j][0];
			string text2 = text;
			while (true)
			{
				text2 = Evolution(text2);
				if (!string.IsNullOrEmpty(text2) && BadgeReq(text2) <= PlayerDefs.BadgeCount)
				{
					list2[j].Add(text2);
					continue;
				}
				break;
			}
		}
		return list2;
	}

	public static MortyData GetNewMortyData(string id, int level, eIVSetUp ivSetUp, eEVSetUp evSetUp)
	{
		if (infoJSON[id] != null)
		{
			return new MortyData(id, level, ivSetUp, evSetUp);
		}
		return null;
	}

	public static MortyData GetNewRandomMortyData(int level, eIVSetUp ivSetUp, eEVSetUp evSetUp, RandomMortyParams mParams, bool forceOnFail = false)
	{
		string randomID = GetRandomID(mParams, forceOnFail);
		return new MortyData(randomID, level, ivSetUp, evSetUp);
	}

	public static string GetStrongerElementType(string type)
	{
		switch (type)
		{
		default:
			return null;
		case "Rock":
			return "Paper";
		case "Paper":
			return "Scissors";
		case "Scissors":
			return "Rock";
		}
	}

	public static float GetElementTypeModifier(string attackingType, string defendingType)
	{
		if (attackingType != null && defendingType != null)
		{
			if ((attackingType.Equals("Rock") && defendingType.Equals("Scissors")) || (attackingType.Equals("Paper") && defendingType.Equals("Rock")) || (attackingType.Equals("Scissors") && defendingType.Equals("Paper")))
			{
				return 1.75f;
			}
			if ((attackingType.Equals("Rock") && defendingType.Equals("Paper")) || (attackingType.Equals("Paper") && defendingType.Equals("Scissors")) || (attackingType.Equals("Scissors") && defendingType.Equals("Rock")))
			{
				return 0.75f;
			}
		}
		return 1f;
	}

	public static int ApplyDamage(MortyData attacker, MortyData defender, AttackEffectData effectData, ref bool isCritical, ref float typeModifier)
	{
		if (TestDefs.PLAYER_ONE_HIT_KILL && !PlayerDefs.CurrentParty().Contains(defender))
		{
			defender.SetHP(0);
		}
		if (SharedStatInfo.battleInfo != null && SharedStatInfo.battleInfo.isTutorial)
		{
			isCritical = false;
		}
		float num = ((!isCritical) ? ((float)attacker.level) : ((float)attacker.level * 1.75f));
		if (num < (float)defender.level)
		{
			num += ((float)defender.level - num) / ((float)defender.level - num + 0.5f);
		}
		float num2 = (float)attacker.attackStat / (float)defender.defenceStat + (1f - (float)attacker.attackStat / (float)defender.defenceStat) / 2f;
		float num3 = (2f * num + 10f) / 250f * (attacker.attack / defender.defence) * num2;
		float num4 = num3 * effectData.power + 2f;
		float num5 = num4 * UnityEngine.Random.Range(0.85f, 1f);
		int b = Mathf.FloorToInt(num5 * typeModifier);
		b = Mathf.Max(1, b);
		int value = defender.IncrementHP(-b);
		return Math.Abs(value);
	}

	public static void ApplyPoisonDamage(MortyData data)
	{
		int b = Mathf.FloorToInt((float)data.hpStat / 10f);
		b = Mathf.Max(1, b);
		data.IncrementHP(-b);
	}

	public static bool ApplyStatModifier(MortyData data, string stat, float statValue)
	{
		switch (stat)
		{
		default:
			return false;
		case "Attack":
			data.attackModifier = Mathf.Clamp(data.attackModifier + statValue, -6f, 6f);
			return true;
		case "Defence":
			data.defenceModifier = Mathf.Clamp(data.defenceModifier + statValue, -6f, 6f);
			return true;
		case "Speed":
			data.speedModifier = Mathf.Clamp(data.speedModifier + statValue, -6f, 6f);
			return true;
		case "Accuracy":
			data.accuracyModifier = Mathf.Clamp(data.accuracyModifier + statValue, -6f, 6f);
			return true;
		case "Evade":
			data.evadeModifier = Mathf.Clamp(data.evadeModifier + statValue, -6f, 6f);
			return true;
		}
	}

	public static float GetBasicStatStepValue(float step)
	{
		int num = Mathf.Clamp(Mathf.FloorToInt(step), -6, 6);
		float num2 = Mathf.Abs(num);
		if (num > 0)
		{
			return (4f + num2) / 4f;
		}
		if (num < 0)
		{
			return 4f / (4f + num2);
		}
		return 1f;
	}

	public static float GetAccuracyAndEvadeStepValue(float step)
	{
		float num = Mathf.Clamp(Mathf.Floor(step), -6f, 6f);
		float num2 = Mathf.Abs(num);
		if (num > 0f)
		{
			return (15f + num2) / 15f;
		}
		if (num < 0f)
		{
			return 15f / (15f + num2);
		}
		return 1f;
	}

	public static int GetXPEarned(MortyData attackingMorty, MortyData defeatedMorty, bool isWild)
	{
		float num = Mathf.Clamp(1f - 0.25f * (float)(attackingMorty.level - defeatedMorty.level), 0.2f, 1f);
		return Mathf.FloorToInt(((!isWild) ? 1.5f : 1f) * (float)DefeatedXP(defeatedMorty.id) * (float)defeatedMorty.level / 7f * num);
	}

	public static int GetXPTarget(int level)
	{
		return Mathf.FloorToInt(level * level * level);
	}

	public static bool IncrementLevel(MortyData mortyData, bool resetXPToLevel, string userPronounYours, out string resultText)
	{
		if (mortyData.level >= 100)
		{
			resultText = string.Format(TextDefs.MORTY_MAX_LEVEL_REACHED, Name(mortyData.id));
			return false;
		}
		int hpStat = mortyData.hpStat;
		mortyData.SetLevel(mortyData.level + 1);
		if (!mortyData.isFainted)
		{
			mortyData.IncrementHP(mortyData.hpStat - hpStat);
		}
		if (resetXPToLevel)
		{
			mortyData.xp = GetXPTarget(mortyData.level);
		}
		if ((float)mortyData.level >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQDQ"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQDQ");
		}
		if ((float)mortyData.level >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQDg"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQDg");
		}
		if ((float)mortyData.level >= SocialDefs.GetAchievementTarget("CgkIjtTv3ccYEAIQDw"))
		{
			App.social.SubmitAchievement("CgkIjtTv3ccYEAIQDw");
		}
		resultText = string.Format(TextDefs.BATTLE_REWARD_LEVEL, userPronounYours, Name(mortyData.id), mortyData.level);
		return true;
	}

	public static string GetMortyNumberText(string id)
	{
		return string.Format("#{0}", Number(id).ToString("D3"));
	}

	public static int GetBalancedMortyCount(bool isHighCount)
	{
		int num = PlayerDefs.CurrentTotalMortyCount;
		if (PlayerDefs.HasCaughtMorty("MortyEgg"))
		{
			num--;
		}
		if (PlayerDefs.BadgeCount >= 20)
		{
			return 5;
		}
		if (PlayerDefs.BadgeCount >= 15 || num >= 5)
		{
			return isHighCount ? 5 : 4;
		}
		if (PlayerDefs.BadgeCount >= 9 || num >= 4)
		{
			return isHighCount ? 4 : 3;
		}
		if (PlayerDefs.BadgeCount >= 4 || num >= 3)
		{
			return isHighCount ? 3 : 2;
		}
		if (PlayerDefs.BadgeCount >= 2)
		{
			return (!isHighCount) ? 1 : 2;
		}
		return 1;
	}

	public static int GetBalancedMortyLevel(eBalanceType type, bool isLowerLevel = false)
	{
		int num = 5 + Mathf.FloorToInt((float)PlayerDefs.BadgeCount * Mathf.Clamp(1f - 0.01f * (float)PlayerDefs.BadgeCount, 0.85f, 1f));
		num += Mathf.FloorToInt(PlayerDefs.BadgeCount / 10);
		int num2 = Mathf.Clamp(Mathf.FloorToInt((float)PlayerDefs.BadgeCount / 6f), 0, 3);
		int num3 = Mathf.Clamp(Mathf.FloorToInt((float)PlayerDefs.BadgeCount / 10f), 0, 2);
		if (isLowerLevel)
		{
			num -= 1 + num3;
		}
		switch (type)
		{
		case eBalanceType.WILD_MORTY:
			num += UnityEngine.Random.Range(-num3, 1);
			break;
		case eBalanceType.DEFAULT_TRAINER:
			num += UnityEngine.Random.Range(-1 - num3, 1 + num2);
			break;
		case eBalanceType.GYM_LEADER:
			num += UnityEngine.Random.Range(1 - num3, 2 + num2);
			break;
		case eBalanceType.REWARD:
			num += UnityEngine.Random.Range(0, 2);
			break;
		}
		return Mathf.Clamp(num, 4, 100);
	}

	public static int GetBalancedMortyLevelForTrainerType(eTrainerType type, bool isLowerLevel)
	{
		switch (type)
		{
		default:
			return GetBalancedMortyLevel(eBalanceType.NONE, isLowerLevel);
		case eTrainerType.DEFAULT:
			return GetBalancedMortyLevel(eBalanceType.DEFAULT_TRAINER, isLowerLevel);
		case eTrainerType.GYM_LEADER:
			return GetBalancedMortyLevel(eBalanceType.GYM_LEADER, isLowerLevel);
		}
	}
}
