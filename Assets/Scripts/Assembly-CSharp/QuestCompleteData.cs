using System;

[Serializable]
public class QuestCompleteData
{
	public string id;

	public string solutionID;

	public QuestCompleteData(string id, string solutionID)
	{
		this.id = id;
		this.solutionID = solutionID;
	}
}
