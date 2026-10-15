public class MPVideoRewardMeeseekInfo : MPStateInfo
{
	public MPVideoRewardData data;

	public MPVideoRewardMeeseekInfo(eMPStateOrigin origin, MPVideoRewardData data)
		: base(origin)
	{
		this.data = data;
	}
}
