using System;
using System.Collections.Generic;
using System.Linq;

public class MPOpponentData
{
	public const int kMaxMortiesInParty = 5;

	public const int kMaxMortyDecks = 3;

	public const string kDictKeyNetId = "player_id";

	public const string kDictKeyType = "type";

	public const string kDictKeyState = "state";

	public const string kDictKeyMorties = "owned_morties";

	public const string kDictKeyItems = "owned_items";

	public const string kDictKeyPlacement = "placement";

	public const string kDictKeyBattleStartMortyId = "active_owned_morty";

	public const string kDictKeyWildMortyNetID = "wild_morty_id";

	public const string kDictKeyWildMortyAssetID = "morty_id";

	public const string kDickKeyItemId = "item_id";

	public const string kDickKeyItemQuantity = "quantity";

	public string netId;

	public eMPOpponentType type;

	public eMPOpponentState state;

	public List<MPOpponentMortyData> morties;

	public Dictionary<string, int> items;

	public MPNodePlacementData placementData;

	public string battleStartMortyId;

	public string assetId;

	public MPOpponentData()
	{
		morties = new List<MPOpponentMortyData>();
		items = new Dictionary<string, int>();
		placementData = new MPNodePlacementData();
	}

	public MPOpponentData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict, true, true);
	}

	public virtual void SetFromDict(Dictionary<string, object> dict, bool clearMorties = false, bool clearItems = false)
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
				netId = Convert.ToString(item.Value);
				break;
			case "type":
				type = (eMPOpponentType)Enum.Parse(typeof(eMPOpponentType), Convert.ToString(item.Value));
				break;
			case "state":
				state = (eMPOpponentState)Enum.Parse(typeof(eMPOpponentState), Convert.ToString(item.Value));
				break;
			case "owned_morties":
				SetMortiesFromDicts(ConvertExt.ToDictionaryArray(item.Value), clearMorties);
				break;
			case "owned_items":
				SetItemsFromDicts(ConvertExt.ToDictionaryArray(item.Value), clearItems);
				break;
			case "placement":
				placementData = new MPNodePlacementData(ConvertExt.ToDictionary(item.Value));
				break;
			case "active_owned_morty":
				battleStartMortyId = Convert.ToString(item.Value);
				break;
			case "morty_id":
				assetId = MPMortyDefs.AssetID(Convert.ToString(item.Value));
				break;
			}
		}
	}

	public void SetMortiesFromDicts(Dictionary<string, object>[] dictArr, bool clearMorties = false, bool clearAttacks = false)
	{
		if (clearMorties)
		{
			morties.Clear();
		}
		if (dictArr == null)
		{
			return;
		}
		for (int i = 0; i < dictArr.Length; i++)
		{
			string mortyNetId = Convert.ToString(dictArr[i]["owned_morty_id"]);
			MPOpponentMortyData mPOpponentMortyData = morties.FirstOrDefault((MPOpponentMortyData e) => e.netId == mortyNetId);
			if (mPOpponentMortyData != null)
			{
				if (clearAttacks && mPOpponentMortyData.attacks != null)
				{
					mPOpponentMortyData.attacks.Clear();
				}
				mPOpponentMortyData.SetFromDict(dictArr[i]);
			}
			else
			{
				mPOpponentMortyData = new MPOpponentMortyData(dictArr[i]);
				mPOpponentMortyData.ResetBattleValues();
				morties.Add(mPOpponentMortyData);
			}
		}
	}

	public void SetItemsFromDicts(Dictionary<string, object>[] dictArr, bool clear = false)
	{
		if (clear)
		{
			items.Clear();
		}
		if (dictArr == null)
		{
			return;
		}
		foreach (Dictionary<string, object> dictionary in dictArr)
		{
			string key = Convert.ToString(dictionary["item_id"]);
			int num = Convert.ToInt16(dictionary["quantity"]);
			if (num > 0)
			{
				items[key] = num;
			}
			else
			{
				items.Remove(key);
			}
		}
	}

	public void ResetPartyBattleValues()
	{
		List<MPOpponentMortyData> allPartyMortyDatas = GetAllPartyMortyDatas();
		for (int i = 0; i < allPartyMortyDatas.Count; i++)
		{
			allPartyMortyDatas[i].ResetBattleValues();
		}
	}

	public virtual string GetUsername()
	{
		return null;
	}

	public MPOpponentMortyData GetMortyData(string netId)
	{
		return morties.FirstOrDefault((MPOpponentMortyData e) => e.netId == netId);
	}

	public virtual MPOpponentMortyData GetPartyMortyData(int index)
	{
		return morties[index];
	}

	public virtual List<MPOpponentMortyData> GetAllPartyMortyDatas()
	{
		return morties;
	}

	public virtual MPOpponentMortyData GetPartyFirstAliveMortyData()
	{
		return morties.FirstOrDefault((MPOpponentMortyData e) => !e.isFainted);
	}

	public List<MPOpponentMortyData> GetAllMortyDatasOfId(string mortyId)
	{
		return new List<MPOpponentMortyData>(morties.Where((MPOpponentMortyData e) => e.id == mortyId));
	}

	public MPOpponentMortyData RemoveMorty(string netId)
	{
		MPOpponentMortyData mortyData = GetMortyData(netId);
		morties.Remove(mortyData);
		return mortyData;
	}
}
