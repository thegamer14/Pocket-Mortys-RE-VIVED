using System;
using System.Collections.Generic;

[Serializable]
public class RoomPickupCollectResponse
{
	public string pickup_id;

	public Dictionary<string, object>[] contents;
}
