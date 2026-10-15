public class MPNPCNode : MPClientNode
{
	public override void SetUp()
	{
		base.SetUp();
		SetUpCharacterMoveModule(GetAssetId());
	}

	public void SetUpWithId(string id, eDirection direction)
	{
		base.id = id;
		initialDirection = direction;
		SetUp();
	}

	protected override string GetAssetId()
	{
		return MPNPCDefs.AssetID(id);
	}

	public override void SetEditorValues(string id, eDirection initialDirection)
	{
		MPNPCDefs.SetUp();
		base.SetEditorValues(id, initialDirection);
	}
}
