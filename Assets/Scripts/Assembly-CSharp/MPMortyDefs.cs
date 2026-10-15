using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPMortyDefs : MonoBehaviour
{
	public const string MORTY_NUMBER = "#{0}";

	public const int kOwnedMortyMax = 300;

	public const int kMaxMortiesInParty = 5;

	public const int kAttacksPerMortyMax = 4;

	public const string kMortyMorticiaID = "MortyMorticia";

	public const string kStatIDLevel = "Level";

	public const string kStatIDHP = "HP";

	public const string kStatIDAttack = "Attack";

	public const string kStatIDDefence = "Defence";

	public const string kStatIDSpeed = "Speed";

	public const string kStatIDAccuracy = "Accuracy";

	public const string kStatIDEvade = "Evade";

	public const string kStatIDAll = "All";

	public const string kStatIDPoison = "Poison";

	public const string kStatIDParalysis = "Paralysis";

	private static JSONNode _infoJSON;

	private static JSONNode _attackInfoJSON;

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["morty_id"].Value == id);
	}

	public static string AssetID(string id)
	{
		return GetNode(id)["asset_id"];
	}

	public static int Number(string id)
	{
		return GetNode(id)["number"].AsInt;
	}

	public static string Name(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "name");
	}

	public static string Description(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "description");
	}

	public static string Characteristic(string id)
	{
		return LocalizationDefs.GetLocalizedString(eGooglePage.Morty, id, "characteristic");
	}

	public static string Height(string id)
	{
		return GetNode(id)["height"];
	}

	public static string Weight(string id)
	{
		return GetNode(id)["weight"];
	}

	public static string ElementType(string id)
	{
		return GetNode(id)["element"];
	}

	public static int EvolutionRequirement(string id)
	{
		return GetNode(id)["evolution_req"].AsInt;
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "MortyInfo")).text);
		_attackInfoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "MortyAttacks")).text);
	}

	public static void Remove()
	{
		_infoJSON = null;
	}

	public static string GetMortyNumberText(string id)
	{
		return string.Format("#{0}", Number(id).ToString("D3"));
	}

	public static eMortyGender GetGender(string id)
	{
		if (id == "MortyMorticia")
		{
			return eMortyGender.FEMALE;
		}
		return eMortyGender.MALE;
	}

	public static int GetXPTarget(int level)
	{
		return Mathf.FloorToInt(level * level * level);
	}

	public static List<string> GetUserRegisterMortyIDs()
	{
		List<string> list = (from data in PlayerDefs.GetAllPlayerMorties()
			select data.id).ToList();
		if (list.Count == 0)
		{
			list.Add("MortyDefault");
			list.Add("MortyScruffy");
		}
		list = list.OrderBy(Number).ToList();
		return (from x in list
			group x by x).Select(Enumerable.First).ToList();
	}

	public static SortedList<int, string> GetListOfOrderedMortyIds()
	{
		SortedList<int, string> sortedList = new SortedList<int, string>();
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			string value = _infoJSON[i]["morty_id"].Value;
			sortedList.Add(_infoJSON[i]["number"].AsInt, value);
		}
		return sortedList;
	}

	public static List<string> GetAttacksForMorty(string id, int minLevel, int maxLevel)
	{
		List<string> list = new List<string>();
		JSONNode jSONNode = _attackInfoJSON.Childs.First((JSONNode e) => e["morty_id"].Value == id);
		JSONNode jSONNode2 = jSONNode["attacks"];
		for (int num = 0; num < jSONNode2.Count; num++)
		{
			int asInt = jSONNode2[num]["level"].AsInt;
			if (asInt >= minLevel && asInt <= maxLevel)
			{
				list.Add(jSONNode2[num]["attack_id"].Value);
			}
		}
		return list;
	}
}
