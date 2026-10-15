using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class PlayerAvatarDefs
{
	private static JSONNode _avatarJSON;

	private static List<string> _avatarIDs;

	public static int idCount
	{
		get
		{
			return _avatarJSON.Count;
		}
	}

	public static string AssetID(string id)
	{
		return _avatarJSON[id]["assetid"];
	}

	public static string Category(string id)
	{
		return _avatarJSON[id]["category"];
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.PlayerAvatar, id, "name");
	}

	public static eCurrencyType CurrencyType(string id)
	{
		return (eCurrencyType)Enum.Parse(typeof(eCurrencyType), ((string)_avatarJSON[id]["currency"]).ToUpper());
	}

	public static int Cost(string id)
	{
		return _avatarJSON[id]["cost"].AsInt;
	}

	public static int DisplayOrder(string id)
	{
		return _avatarJSON[id]["displayorder"].AsInt;
	}

	public static int CouncilRequirement(string id)
	{
		return _avatarJSON[id]["councilmemberreq"].AsInt;
	}

	public static void SetUp()
	{
		_avatarJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "PlayerAvatarInfo")).text);
		_avatarIDs = new List<string>();
		for (int i = 0; i < _avatarJSON.Count; i++)
		{
			_avatarIDs.Add(_avatarJSON[i]["id"]);
		}
	}

	public static void Remove()
	{
		_avatarJSON = null;
	}

	public static List<string> GetAvatarIDs()
	{
		return _avatarIDs;
	}

	public static bool IsAvatarLockedForCouncilLevel(string id, int councilMembersDefeated)
	{
		if (!_avatarIDs.Contains(id))
		{
			return false;
		}
		int num = CouncilRequirement(id);
		return num >= 0 && councilMembersDefeated < num;
	}

	public static bool IsAvatarLockedByCouncilLevel(string id)
	{
		if (!_avatarIDs.Contains(id))
		{
			return false;
		}
		return CouncilRequirement(id) > 0;
	}

	public static List<string> GetAvatarsUnlockedForCouncilLevel(int councilMembersDefeated)
	{
		List<string> list = new List<string>();
		foreach (string avatarID in _avatarIDs)
		{
			if (IsAvatarLockedByCouncilLevel(avatarID) && !IsAvatarLockedForCouncilLevel(avatarID, councilMembersDefeated))
			{
				list.Add(avatarID);
			}
		}
		return list;
	}
}
