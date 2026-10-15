using System.Collections.Generic;
using System.Linq;

public class MPCraftInfo : MPStateInfo
{
	public Dictionary<string, int> items;

	public string[] slotIds;

	public int slotSelectedIndex;

	public string recipeId;

	public MPCraftInfo(eMPStateOrigin origin, Dictionary<string, int> items)
		: base(origin)
	{
		this.items = items;
		slotIds = new string[3];
		slotSelectedIndex = -1;
	}

	public void SetSelectedSlot(string itemId)
	{
		if (items.ContainsKey(itemId))
		{
			items[itemId]--;
			if (items[itemId] <= 0)
			{
				items.Remove(itemId);
			}
		}
		slotIds[slotSelectedIndex] = itemId;
		recipeId = MPRecipeDefs.GetIDForCombination(slotIds);
	}

	public void ClearSlot(int index)
	{
		if (!string.IsNullOrEmpty(slotIds[index]))
		{
			if (items.ContainsKey(slotIds[index]))
			{
				items[slotIds[index]]++;
			}
			else
			{
				items[slotIds[index]] = 1;
			}
		}
		slotIds[index] = null;
		recipeId = MPRecipeDefs.GetIDForCombination(slotIds);
	}

	public void ClearAllSlots()
	{
		ClearSlot(0);
		ClearSlot(1);
		ClearSlot(2);
	}

	public int SlotsFilled()
	{
		return slotIds.Where((string e) => !string.IsNullOrEmpty(e)).Count();
	}
}
