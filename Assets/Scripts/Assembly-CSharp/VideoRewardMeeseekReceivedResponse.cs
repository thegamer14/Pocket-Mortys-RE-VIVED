using System;
using System.Collections.Generic;

[Serializable]
public class VideoRewardMeeseekReceivedResponse
{
	public string incentive_id;

	public Dictionary<string, object>[] rewards;
}
