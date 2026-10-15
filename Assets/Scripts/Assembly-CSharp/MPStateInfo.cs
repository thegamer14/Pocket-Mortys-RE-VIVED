public class MPStateInfo
{
	public eMPStateOrigin origin;

	public MPStateInfo(eMPStateOrigin origin)
	{
		this.origin = origin;
	}

	public bool IfOrigin(eMPStateOrigin originCompare)
	{
		return (origin & originCompare) == originCompare;
	}
}
