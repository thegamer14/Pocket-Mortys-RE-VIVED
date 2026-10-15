using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SignPostDefs
{
	private static JSONNode _infoJSON;

	private static List<string> _pooledIDs;

	public static int idCount
	{
		get
		{
			return _infoJSON.Count;
		}
	}

	public static bool IsRandomPool(string id)
	{
		return _infoJSON[id]["israndompool"].AsBool;
	}

	public static int BadgeReq(string id)
	{
		return _infoJSON[id]["badgereq"].AsInt;
	}

	public static string Dialogue(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.SignPost, id, "dialogue");
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "SignPostInfo")).text);
		_pooledIDs = new List<string>();
		for (int i = 0; i < idCount; i++)
		{
			string iD = GetID(i);
			if (IsRandomPool(iD))
			{
				_pooledIDs.Add(iD);
			}
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
		_pooledIDs = null;
	}

	public static bool IsIDActive(string id)
	{
		return id != null && _infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return _infoJSON[index]["id"];
	}

	public static List<string> GetListOfRandomPooledIDs(bool isBadgeLocked = false)
	{
		List<string> list = new List<string>(_pooledIDs);
		if (isBadgeLocked)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (BadgeReq(list[num]) > PlayerDefs.BadgeCount)
				{
					list.RemoveAt(num);
				}
			}
		}
		list.Shuffle();
		return list;
	}
}
