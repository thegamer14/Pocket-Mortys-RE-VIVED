using System;

[Serializable]
public class QuestGiverData : NodeData
{
	public eQuestProgress progress;

	public QuestGiverData(string id)
		: base(id)
	{
	}
}
