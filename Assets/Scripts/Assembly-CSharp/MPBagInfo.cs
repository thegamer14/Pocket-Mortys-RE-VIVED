using System.Collections.Generic;

public class MPBagInfo : MPStateInfo
{
	public Dictionary<string, int> items;

	public string selectedItemId;

	public MPOpponentMortyData selectedMortyData;

	public MPBagInfo(eMPStateOrigin origin, Dictionary<string, int> items, MPOpponentMortyData selectedMortyData = null)
		: base(origin)
	{
		this.items = items;
		this.selectedMortyData = selectedMortyData;
	}
}
