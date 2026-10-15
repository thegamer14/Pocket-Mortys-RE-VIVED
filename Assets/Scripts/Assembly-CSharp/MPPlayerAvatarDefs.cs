using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPPlayerAvatarDefs
{
	private static JSONNode _infoJSON;

	private static List<string> _avatarIDs;

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["player_avatar_id"].Value == id);
	}

	public static string AssetID(string id)
	{
		return GetNode(id)["asset_id"];
	}

	public static string Category(string id)
	{
		return (!(GetNode(id)["category"].Value == "null")) ? GetNode(id)["category"] : null;
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.PlayerAvatar, id, "name");
	}

	public static eCurrencyType CurrencyType(string id)
	{
		return ((string)GetNode(id)["currency"]).Equals("coupons") ? eCurrencyType.COUPON : eCurrencyType.COIN;
	}

	public static int Cost(string id)
	{
		return GetNode(id)["cost"].AsInt;
	}

	public static int DisplayOrder(string id)
	{
		return GetNode(id)["sortorder"].AsInt;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "PlayerAvatarInfo")).text);
		_avatarIDs = new List<string>();
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			_avatarIDs.Add(_infoJSON[i]["player_avatar_id"]);
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
	}

	public static List<string> GetAvatarIDs()
	{
		return _avatarIDs.OrderBy(DisplayOrder).ToList();
	}

	public static List<string> GetAvatarsForRegisterSelect()
	{
		List<string> list = new List<string>();
		List<string> avatarIDs = PlayerAvatarDefs.GetAvatarIDs();
		foreach (string item in avatarIDs)
		{
			if (!list.Contains(item) && _avatarIDs.Contains(item))
			{
				if (App.saveData.player.boughtPlayerAvatarIDs.Contains(item))
				{
					list.Add(item);
				}
				else if (PlayerAvatarDefs.IsAvatarLockedByCouncilLevel(item) && !PlayerAvatarDefs.IsAvatarLockedForCouncilLevel(item, App.saveData.player.councilMembersDefeated))
				{
					list.Add(item);
				}
			}
		}
		List<string> list2 = _avatarIDs.FindAll((string id) => Cost(id) == 0);
		foreach (string item2 in list2)
		{
			if (!list.Contains(item2) && !PlayerAvatarDefs.IsAvatarLockedForCouncilLevel(item2, App.saveData.player.councilMembersDefeated))
			{
				list.Add(item2);
			}
		}
		return list.OrderBy(DisplayOrder).ToList();
	}
}
