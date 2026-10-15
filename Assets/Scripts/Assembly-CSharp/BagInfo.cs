using System.Collections.Generic;

public class BagInfo
{
	public List<MortyData> mortyDatas;

	public Dictionary<string, int> items;

	public string selectedID;

	public eBagFromState fromState;

	public QuestGiverNode questGiverNode;

	public BagInfo(List<MortyData> mortyDatas, Dictionary<string, int> items, eBagFromState fromState, QuestGiverNode questGiverNode)
	{
		this.mortyDatas = mortyDatas;
		this.items = items;
		this.fromState = fromState;
		this.questGiverNode = questGiverNode;
		selectedID = null;
	}
}
