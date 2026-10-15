using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class RecipeDefs
{
	public const int kCraftItemsMaxSpaces = 3;

	private static JSONNode infoJSON;

	private static List<string[]> _slotCombinations;

	public static int Count
	{
		get
		{
			return infoJSON.Count;
		}
	}

	public static void SetUp()
	{
		infoJSON = JSONNode.Parse(Resources.Load<TextAsset>(string.Format("JSONs/SinglePlayer/{0}", "RecipeInfo")).text);
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
		infoJSON = null;
		_slotCombinations = null;
	}

	public static int Number(string id)
	{
		return infoJSON[id]["number"].AsInt;
	}

	public static string Slot1(string id)
	{
		return infoJSON[id]["slot1"];
	}

	public static string Slot2(string id)
	{
		return infoJSON[id]["slot2"];
	}

	public static string Slot3(string id)
	{
		return infoJSON[id]["slot3"];
	}

	public static string ResultID(string id)
	{
		return infoJSON[id]["resultid"];
	}

	public static bool IsIDActive(string id)
	{
		return id != null && infoJSON[id] != null;
	}

	public static string GetID(int index)
	{
		return infoJSON[index]["id"];
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
