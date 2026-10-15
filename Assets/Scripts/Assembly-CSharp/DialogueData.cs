public class DialogueData
{
	public DialogueDataEntry[] entries;

	public DialogueData(string text, string name = null, string assetID = null, eDirection direction = eDirection.LEFT)
	{
		entries = new DialogueDataEntry[1]
		{
			new DialogueDataEntry(text, name, assetID, direction)
		};
	}

	public DialogueData(DialogueDataEntry[] entries)
	{
		this.entries = entries;
	}
}
