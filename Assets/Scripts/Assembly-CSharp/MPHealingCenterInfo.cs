public class MPHealingCenterInfo : MPStateInfo
{
	public bool isRevive;

	public MPHealingCenterInfo(eMPStateOrigin origin, bool isRevive)
		: base(origin)
	{
		this.isRevive = isRevive;
	}
}
