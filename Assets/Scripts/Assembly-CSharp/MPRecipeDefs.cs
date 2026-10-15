using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class MPRecipeDefs : MonoBehaviour
{
	public const int kCraftItemsMaxSpaces = 3;

	private static JSONNode _infoJSON;

	private static List<string[]> _slotCombinations;

	public static int Count
	{
		get
		{
			return _infoJSON.Count;
		}
	}

	public static JSONNode GetNode(string id)
	{
		return _infoJSON.Childs.First((JSONNode e) => e["recipe_id"].Value == id);
	}

	public static int Number(string id)
	{
		return GetNode(id)["sortorder"].AsInt;
	}

	public static string Slot1(string id)
	{
		return (!(GetNode(id)["item_id_1"].Value == "null")) ? GetNode(id)["item_id_1"] : null;
	}

	public static string Slot2(string id)
	{
		return (!(GetNode(id)["item_id_2"].Value == "null")) ? GetNode(id)["item_id_2"] : null;
	}

	public static string Slot3(string id)
	{
		return (!(GetNode(id)["item_id_3"].Value == "null")) ? GetNode(id)["item_id_3"] : null;
	}

	public static string ResultID(string id)
	{
		return GetNode(id)["item_id_result"];
	}

	public static bool IsIDActive(string id)
	{
		return id != null && _infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return _infoJSON[index]["recipe_id"];
	}

	public static void SetUp()
	{
		_infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/Multiplayer/{0}", "RecipeInfo")).text);
		_slotCombinations = new List<string[]>();
		for (int i = 0; i < Count; i++)
		{
			string iD = GetID(i);
			string[] array = new string[3]
			{
				Slot1(iD),
				Slot2(iD),
				Slot3(iD)
			};
			Array.Sort(array);
			_slotCombinations.Add(array);
		}
	}

	public static void Remove()
	{
		_infoJSON = null;
		_slotCombinations = null;
	}

	public static SortedList<int, string> GetOrderedIds()
	{
		SortedList<int, string> sortedList = new SortedList<int, string>();
		for (int i = 0; i < _infoJSON.Count; i++)
		{
			sortedList.Add(_infoJSON[i]["sortorder"].AsInt, _infoJSON[i]["recipe_id"].Value);
		}
		return sortedList;
	}

	public static string GetIDForCombination(string[] combination)
	{
		string[] array = new string[combination.Length];
		Array.Copy(combination, array, array.Length);
		Array.Sort(array);
		int num = -1;
		for (int i = 0; i < _slotCombinations.Count; i++)
		{
			if (_slotCombinations[i].SequenceEqual(array))
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			return GetID(num);
		}
		return null;
	}

	public static List<string> GetCombinationForRecipeID(string id)
	{
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(Slot1(id)))
		{
			list.Add(Slot1(id));
		}
		if (!string.IsNullOrEmpty(Slot2(id)))
		{
			list.Add(Slot2(id));
		}
		if (!string.IsNullOrEmpty(Slot3(id)))
		{
			list.Add(Slot3(id));
		}
		return list;
	}

	public static int GetCraftableQuantity(string recipeID, int slot1Quant, int slot2Quant, int slot3Quant)
	{
		int count = GetCombinationForRecipeID(recipeID).Count;
		int num = Mathf.Max(slot1Quant, slot2Quant, slot3Quant);
		if (count > 0 && slot1Quant < num)
		{
			num = slot1Quant;
		}
		if (count > 1 && slot2Quant < num)
		{
			num = slot2Quant;
		}
		if (count > 2 && slot3Quant < num)
		{
			num = slot3Quant;
		}
		return num;
	}

	public static string Craft(string[] combination)
	{
		return GetIDForCombination(combination);
	}
}
