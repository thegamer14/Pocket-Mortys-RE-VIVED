public class MPSignPostNode : MPClientNode
{
	public override void SetUp()
	{
		base.SetUp();
	}

	public void SetUpWithId(string id, eDirection direction)
	{
		base.id = id;
		initialDirection = direction;
		SetUp();
	}
}
