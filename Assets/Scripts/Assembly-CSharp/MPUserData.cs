using System;
using System.Collections.Generic;

public class MPUserData : MPOpponentData
{
	public const string kDictKeyUsername = "username";

	public const string kDictKeyLevel = "level";

	public const string kDictKeyUserAvatarID = "player_avatar_id";

	public string username;

	public string avatarId;

	public int level;

	public string avatarCategory
	{
		get
		{
			return MPPlayerAvatarDefs.Category(avatarId);
		}
	}

	public MPUserData()
	{
	}

	public MPUserData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict, true, true);
	}

	public override void SetFromDict(Dictionary<string, object> dict, bool clearMorties = false, bool clearItems = false)
	{
		if (dict == null)
		{
			return;
		}
		base.SetFromDict(dict, clearMorties, clearItems);
		foreach (KeyValuePair<string, object> item in dict)
		{
			switch (item.Key)
			{
			case "username":
				username = Convert.ToString(item.Value);
				break;
			case "player_avatar_id":
				avatarId = Convert.ToString(item.Value);
				assetId = MPPlayerAvatarDefs.AssetID(avatarId);
				break;
			case "level":
				level = Convert.ToInt32(item.Value);
				break;
			}
		}
	}

	public override string GetUsername()
	{
		return username;
	}
}
