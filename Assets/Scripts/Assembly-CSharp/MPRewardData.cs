using System;
using System.Collections.Generic;

[Serializable]
public class MPRewardData
{
	private const string _kDictKeyType = "type";

	private const string _kDictKeyValueAmount = "amount";

	private const string _kDictKeyValueAmountReceived = "amount_received";

	private const string _kDictKeyQuantity = "quantity";

	private const string _kDictKeyItemId = "item_id";

	private const string _kDictKeyMortyId = "morty_id";

	private const string _kDictKeyMortyLevel = "level";

	private const string _kDictKeyMortyLimit = "owned_morty_limit_reached";

	private const string _kDictKeyAddedToDeck = "added_to_active_deck";

	private const string _kDictKeyOwnedMorty = "owned_morty";

	private const string _kDictKeySubstitute = "substitute";

	public eMPRewardType type;

	public string id;

	public int amount;

	public int amountReceived;

	public int quantity;

	public bool isMortyLimit;

	public bool isAddedToDeck;

	public Dictionary<string, object> ownedMortyDict;

	public MPRewardData substitute;

	public MPRewardData()
	{
	}

	public MPRewardData(Dictionary<string, object> dict)
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
			case "type":
				type = (eMPRewardType)Enum.Parse(typeof(eMPRewardType), Convert.ToString(item.Value));
				break;
			case "item_id":
			case "morty_id":
				id = Convert.ToString(item.Value);
				break;
			case "amount":
			case "level":
				amount = Convert.ToInt32(item.Value);
				break;
			case "amount_received":
				amountReceived = Convert.ToInt32(item.Value);
				break;
			case "quantity":
				quantity = Convert.ToInt32(item.Value);
				break;
			case "owned_morty_limit_reached":
				isMortyLimit = Convert.ToBoolean(item.Value);
				break;
			case "added_to_active_deck":
				isAddedToDeck = Convert.ToBoolean(item.Value);
				break;
			case "owned_morty":
				ownedMortyDict = (Dictionary<string, object>)item.Value;
				break;
			case "substitute":
				substitute = new MPRewardData((Dictionary<string, object>)item.Value);
				break;
			}
		}
	}

	public static MPRewardData[] GetRewardDatasForDictArray(Dictionary<string, object>[] dictArr)
	{
		if (dictArr == null)
		{
			return new MPRewardData[0];
		}
		MPRewardData[] array = new MPRewardData[dictArr.Length];
		for (int i = 0; i < dictArr.Length; i++)
		{
			array[i] = new MPRewardData(dictArr[i]);
		}
		return array;
	}
}
