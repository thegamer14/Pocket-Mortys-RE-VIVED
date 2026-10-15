using System;

public class DialogueDataEntry
{
	public string[] text;

	public string name;

	public string assetID;

	public eDirection direction;

	public DialogueDataEntry(string text, string name = null, string assetID = null, eDirection direction = eDirection.LEFT)
	{
		this.text = text.Split(new string[1] { "*" }, StringSplitOptions.None);
		this.name = name;
		this.assetID = ((!(assetID == "{PLAYER}")) ? assetID : PlayerDefs.AssetID);
		this.direction = direction;
	}
}
