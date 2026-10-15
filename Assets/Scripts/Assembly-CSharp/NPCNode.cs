public class NPCNode : AdvancedNode
{
	public NPCData npcData
	{
		get
		{
			return (NPCData)_data;
		}
		private set
		{
			_data = value;
		}
	}

	protected override void RefreshAssetID()
	{
		if (!string.IsNullOrEmpty(data.id))
		{
			_assetID = NPCDefs.AssetID(data.id);
		}
	}
}
