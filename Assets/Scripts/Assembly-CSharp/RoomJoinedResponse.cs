using System;
using System.Collections.Generic;

[Serializable]
public class RoomJoinedResponse
{
	public string room_id;

	public string room_udp_host;

	public int room_udp_port;

	public string world_id = "1";

	public Dictionary<string, object>[] users;

	public Dictionary<string, object>[] wild_morties;

	public Dictionary<string, object>[] pickups;

	public Dictionary<string, object> incentive;
}
