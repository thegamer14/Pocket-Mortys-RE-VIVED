using System.Collections.Generic;

public class CraftInfo
{
	public Dictionary<string, int> items;

	public string[] slotIDs;

	public CraftInfo(Dictionary<string, int> items)
	{
		this.items = items;
		ClearSlots();
	}

	public void ClearSlots()
	{
		slotIDs = new string[3];
	}
}
