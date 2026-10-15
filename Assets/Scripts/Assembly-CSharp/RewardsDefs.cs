using System.Collections.Generic;
using UnityEngine;

public class RewardsDefs
{
	private const int _kRewardCoinBaseWildMorty = 10;

	private const int _kRewardCoinBaseTrainer = 15;

	public const int kRewardItemCount = 4;

	public const int kRewardMortyRarity = 70;

	public const int kRewardNewMortysAfterUpdate = 3;

	public static List<RewardData> GetWildMortyReward(MortyData defeatedMortyData)
	{
		List<RewardData> list = new List<RewardData>();
		list.Add(new RewardData(eRewardType.COIN, null, 100f, defeatedMortyData.level * 15));
		list.Add(new RewardData(eRewardType.ITEM, ItemDefs.GetRandomBadgeLockedRewardID(eItemType.PART, true, true), 100f));
		return list;
	}

	public static List<RewardData> GetTrainerReward(string trainerID, int defeatedLevel)
	{
		List<RewardData> possibleRewards = TrainerDefs.Rewards(trainerID);
		return GetTrainerCalculatedReward(possibleRewards, defeatedLevel);
	}

	public static List<RewardData> GetTournamentTrainerReward(string trainerID, int defeatedLevel)
	{
		List<RewardData> possibleRewards = TournamentDefs.TrainerRewards(trainerID);
		return GetTrainerCalculatedReward(possibleRewards, defeatedLevel);
	}

	private static List<RewardData> GetTrainerCalculatedReward(List<RewardData> possibleRewards, int defeatedLevel)
	{
		List<RewardData> list = new List<RewardData>();
		for (int i = 0; i < possibleRewards.Count; i++)
		{
			RewardData rewardData = possibleRewards[i];
			if ((float)Random.Range(0, 100) > rewardData.chance)
			{
				continue;
			}
			if (string.IsNullOrEmpty(rewardData.id))
			{
				switch (rewardData.type)
				{
				case eRewardType.MORTY:
					rewardData.id = MortyDefs.GetRandomID(new RandomMortyParams
					{
						badgeLevelLock = true,
						rarityLock = eMortyRarityCheck.NOT_RARE
					}, true);
					break;
				case eRewardType.ITEM:
					rewardData.id = ItemDefs.GetRandomBadgeLockedRewardID(eItemType.ALL, true, true);
					break;
				}
			}
			else if (rewardData.type == eRewardType.ITEM)
			{
				eItemType eItemType2 = rewardData.id.ToEnum(eItemType.ALL);
				if (eItemType2 != eItemType.ALL)
				{
					rewardData.id = ItemDefs.GetRandomBadgeLockedRewardID(eItemType2, true, true);
				}
			}
			if (rewardData.value < 1)
			{
				switch (rewardData.type)
				{
				case eRewardType.BADGE:
					rewardData.value = TrainerDefs.GetRandomGymLeaderBadgeID();
					break;
				case eRewardType.COIN:
					rewardData.value = defeatedLevel * 15;
					break;
				case eRewardType.MORTY:
					rewardData.value = MortyDefs.GetBalancedMortyLevel(eBalanceType.REWARD);
					break;
				case eRewardType.ITEM:
					rewardData.value = 1;
					break;
				}
			}
			list.Add(possibleRewards[i]);
		}
		return list;
	}

	public static List<RewardData> GetItemCrateReward()
	{
		if ("Tutorial".Equals(SharedStatInfo.worldInfo.worldID))
		{
			List<RewardData> list = new List<RewardData>();
			list.Add(new RewardData(eRewardType.ITEM, "ItemMortyChip", 100f));
			return list;
		}
		string id = ((!(Random.value <= 0.05f)) ? ItemDefs.GetRandomBadgeLockedRewardID(eItemType.ITEM, true, true) : "ItemCoupon");
		List<RewardData> list2 = new List<RewardData>();
		list2.Add(new RewardData(eRewardType.COIN, null, 100f, 100 + 10 * PlayerDefs.BadgeCount));
		list2.Add(new RewardData(eRewardType.ITEM, id, 100f));
		list2.Add(new RewardData(eRewardType.ITEM, ItemDefs.GetRandomBadgeLockedRewardID(eItemType.PART, true, true), 100f));
		return list2;
	}

	public static List<RewardData> GetCouponReward()
	{
		List<RewardData> list = new List<RewardData>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>(PlayerDefs.CurrentItems());
		for (int i = 0; i < 4; i++)
		{
			string text = null;
			switch (i)
			{
			case 0:
				text = ItemDefs.GetRandomBadgeLockedRewardID(eItemType.PART, true, true);
				break;
			case 1:
				text = ItemDefs.GetRandomBracketLockedRewardableItemID(75, 100, dictionary);
				break;
			case 2:
				text = ItemDefs.GetRandomBracketLockedRewardableItemID(25, 75, dictionary);
				break;
			case 3:
				text = ((!(Random.value <= 0.05f)) ? ItemDefs.GetRandomBracketLockedRewardableItemID(0, 24, dictionary) : "ItemCoupon");
				break;
			}
			if (dictionary.ContainsKey(text))
			{
				dictionary[text]++;
			}
			else
			{
				dictionary.Add(text, 1);
			}
			list.Add(new RewardData(eRewardType.ITEM, text, 100f, 1, true));
		}
		string text2 = null;
		if (App.saveData.player.gachaPullsSinceUpdate <= 3)
		{
			RandomMortyParams randomMortyParams = new RandomMortyParams();
			randomMortyParams.isNewOnly = true;
			randomMortyParams.lockToUnobtained = true;
			randomMortyParams.evolutionTierLock = new Range<int>(2, 10);
			text2 = MortyDefs.GetRandomID(randomMortyParams);
		}
		if (text2 == null)
		{
			RandomMortyParams randomMortyParams = new RandomMortyParams();
			randomMortyParams.evolutionTierLock = new Range<int>(2, 10);
			text2 = MortyDefs.GetRandomID(randomMortyParams, true);
		}
		list.Add(new RewardData(eRewardType.MORTY, text2, 100f, MortyDefs.GetBalancedMortyLevel(eBalanceType.REWARD)));
		return list;
	}
}
