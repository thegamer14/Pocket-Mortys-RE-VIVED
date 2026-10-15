using System;
using System.Collections.Generic;

public class MPChallengeData
{
	public const string kDictKeyId = "challenge_id";

	public const string kDictKeyCount = "progress";

	public const string kDictKeyTarget = "target";

	public const string kDictKeyReward = "reward";

	public const string kDictKeyCooldownTimestamp = "cooldown_expires";

	public string id;

	public int count;

	public int target;

	public int reward;

	public double cooldown;

	public MPChallengeData()
	{
	}

	public MPChallengeData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public void SetFromDict(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "challenge_id":
				id = Convert.ToString(item.Value);
				break;
			case "progress":
				count = Convert.ToInt32(item.Value);
				break;
			case "target":
				target = Convert.ToInt32(item.Value);
				break;
			case "reward":
				reward = Convert.ToInt32(item.Value);
				break;
			case "cooldown_expires":
			{
				long num = Convert.ToInt64(item.Value);
				if (num > 0)
				{
					DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(num);
					cooldown = (dateTime - DateTime.UtcNow).TotalSeconds;
				}
				break;
			}
			}
		}
	}

	public bool IsCoolingDown()
	{
		return cooldown > 0.0;
	}

	public bool IsClaimable()
	{
		return count >= target && !IsCoolingDown();
	}
}
