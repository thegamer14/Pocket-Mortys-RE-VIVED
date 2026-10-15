using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TrainerData : NodeData
{
	public eTrainerType type;

	public List<MortyData> morties;

	public Dictionary<string, int> items;

	public bool isDefeated;

	public TrainerData(string id)
		: base(id)
	{
		type = TrainerDefs.Type(id);
		SetMorties();
		SetItems();
	}

	protected virtual void SetMorties()
	{
		morties = TrainerDefs.Morties(id);
		if (morties.Count == 0)
		{
			eEVSetUp evSetUp = ((type == eTrainerType.GYM_LEADER || type == eTrainerType.COUNCIL_MEMBER) ? eEVSetUp.RANDOM : eEVSetUp.NONE);
			bool flag = PlayerDefs.BadgeCount > 2 && UnityEngine.Random.Range(0, 2) == 0;
			bool isLowerLevel = PlayerDefs.BadgeCount < 17 && flag;
			int balancedMortyCount = MortyDefs.GetBalancedMortyCount(flag);
			Range<int> statTotalLock = new Range<int>(160 + (Mathf.Clamp(PlayerDefs.BadgeCount, 19, 24) - 19) * 10, 1000);
			for (int i = 0; i < balancedMortyCount; i++)
			{
				int balancedMortyLevelForTrainerType = MortyDefs.GetBalancedMortyLevelForTrainerType(type, isLowerLevel);
				MortyData newRandomMortyData = MortyDefs.GetNewRandomMortyData(balancedMortyLevelForTrainerType, eIVSetUp.RANDOM_RESTRICTED, evSetUp, new RandomMortyParams
				{
					badgeLevelLock = true,
					rarityLock = eMortyRarityCheck.NOT_RARE,
					statTotalLock = statTotalLock
				}, true);
				morties.Add(newRandomMortyData);
			}
			morties = morties.OrderBy((MortyData e) => e.level + UnityEngine.Random.Range(2, 3)).ToList();
		}
	}

	protected virtual void SetItems()
	{
		items = TrainerDefs.Items(id);
		if (items.Count == 0)
		{
			items = ItemDefs.GetBalancedTrainerItems();
		}
	}
}
