public class MPFriendsInfo : MPStateInfo
{
	public MPFriendData selectedFriendData;

	public MPFriendsInfo(eMPStateOrigin origin, MPFriendData selectedFriendData)
		: base(origin)
	{
		this.selectedFriendData = selectedFriendData;
	}
}
