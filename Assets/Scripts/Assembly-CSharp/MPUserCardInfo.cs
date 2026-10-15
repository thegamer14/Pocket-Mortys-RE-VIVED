public class MPUserCardInfo : MPStateInfo
{
	public MPUserData userData;

	public MPBattleInviteData receivedInviteData;

	public MPUserCardInfo(eMPStateOrigin origin, MPUserData userData = null, MPBattleInviteData receivedInviteData = null)
		: base(origin)
	{
		this.userData = userData;
		this.receivedInviteData = receivedInviteData;
	}
}
