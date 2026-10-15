using System.Collections.Generic;

public class PartyInfo
{
	public List<MortyData> mortyDatas;

	public MortyData startingMortyData;

	public MortyData selectedMortyData;

	public bool forceSwitch;

	public BattleMove switchMove;

	public Dictionary<string, int> items;

	public PartyInfo(List<MortyData> mortyDatas, Dictionary<string, int> items, MortyData selectedMortyData = null, bool forceSwitch = false)
	{
		this.mortyDatas = mortyDatas;
		this.items = items;
		startingMortyData = selectedMortyData;
		this.selectedMortyData = selectedMortyData;
		this.forceSwitch = forceSwitch;
	}
}
