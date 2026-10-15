using System;
using System.Collections.Generic;

public class MPFriendData
{
	public const string kDictKeyUserId = "player_id";

	public const string kDictKeyUsername = "username";

	public const string kDictKeyAvatarId = "player_avatar_id";

	public const string kDictKeyLevel = "level";

	public const string kDictKeyIsOnline = "online";

	public const string kDictKeyWinCount = "wins";

	public const string kDictKeyLossCount = "losses";

	public const string kDictKeyRequestingMortyId = "donation_request";

	public string userId;

	public string username;

	public string avatarId;

	public int level;

	public bool isOnline;

	public int winCount;

	public int lossCount;

	public string requestingMortyId;

	public bool isPending;

	public MPFriendData()
	{
	}

	public MPFriendData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict);
	}

	public virtual void SetFromDict(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "player_id":
				userId = Convert.ToString(item.Value);
				break;
			case "username":
				username = Convert.ToString(item.Value);
				break;
			case "player_avatar_id":
				avatarId = Convert.ToString(item.Value);
				break;
			case "level":
				level = Convert.ToInt32(item.Value);
				break;
			case "online":
				isOnline = Convert.ToBoolean(item.Value);
				break;
			case "wins":
				winCount = Convert.ToInt32(item.Value);
				break;
			case "losses":
				lossCount = Convert.ToInt32(item.Value);
				break;
			case "donation_request":
			case "owned_morty_id":
				requestingMortyId = Convert.ToString(item.Value);
				break;
			}
		}
	}

	public bool IsAvailableForBattle()
	{
		return isOnline && !isPending;
	}

	public bool IsAvailableForDonating()
	{
		return !string.IsNullOrEmpty(requestingMortyId);
	}
}
