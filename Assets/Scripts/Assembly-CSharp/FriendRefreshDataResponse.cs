using System;
using System.Collections.Generic;

[Serializable]
public class FriendRefreshDataResponse
{
	public DonationRequestResponse donation_request;

	public Dictionary<string, object>[] requests_received = new Dictionary<string, object>[0];

	public Dictionary<string, object>[] requests_sent = new Dictionary<string, object>[0];

	public Dictionary<string, object>[] friends = new Dictionary<string, object>[0];

	public int limit_friends;

	public int poll_period;
}
