public class MPClientNode : MPNode
{
	public string id;

	public virtual void SetEditorValues(string id, eDirection initialDirection)
	{
		this.id = id;
		base.SetEditorValues(initialDirection);
	}
}
