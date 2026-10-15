public class MPFriendDonateMortyInfo : MPStateInfo
{
	public MPFriendData friendData;

	public MPFriendDonateMortyInfo(eMPStateOrigin origin, MPFriendData friendData)
		: base(origin)
	{
		this.friendData = friendData;
	}
}
