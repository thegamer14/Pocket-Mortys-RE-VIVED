using System;
using System.Collections.Generic;
using System.Linq;

public class MPPlayerData : MPUserData
{
	public delegate void CurrencyChangedDelegate(int newValue);

	private const string _kDictKeyCoins = "coins";

	private const string _kDictKeyCoupons = "coupons";

	private const string _kDictKeyXP = "xp";

	private const string _kDictKeyXpMultiplier = "streak";

	private const string _kOwnedAvatars = "owned_avatars";

	private const string _kDictKeyDecks = "decks";

	private const string _kDictKeyActiveDeck = "active_deck_id";

	private const string _kDictKeyDeck_Id = "deck_id";

	private const string _kDictKeyMortyIds = "owned_morty_ids";

	private const string _kDictKeyMortydex = "mortydex";

	private const string _kDictKeyMortydexIsCaught = "caught";

	private const string _kDictKeyRecipes = "owned_recipes";

	private const string _kDictKeyRecipeId = "recipe_id";

	private const string _kDictKeyChallengeComplete = "challenge_reward";

	private int _coins;

	private int _coupons;

	public int xp;

	public int xpMultiplier;

	public List<string>[] decks;

	public int activeDeckIndex;

	public List<string> mortydexSeen;

	public List<string> mortydexCaught;

	public List<string> recipes;

	public List<string> ownedAvatars;

	public bool challengeComplete;

	public List<MPChallengeData> challenges;

	public int coins
	{
		get
		{
			return _coins;
		}
		set
		{
			if (value != _coins)
			{
				_coins = value;
				if (this.CoinsChangedEvent != null)
				{
					this.CoinsChangedEvent(_coins);
				}
			}
		}
	}

	public int coupons
	{
		get
		{
			return _coupons;
		}
		set
		{
			if (value != _coupons)
			{
				_coupons = value;
				if (this.CouponsChangedEvent != null)
				{
					this.CouponsChangedEvent(_coupons);
				}
			}
		}
	}

	public event CurrencyChangedDelegate CoinsChangedEvent;

	public event CurrencyChangedDelegate CouponsChangedEvent;

	public MPPlayerData()
	{
		decks = new List<string>[3]
		{
			new List<string>(),
			new List<string>(),
			new List<string>()
		};
		mortydexSeen = new List<string>();
		mortydexCaught = new List<string>();
		recipes = new List<string>();
		ownedAvatars = new List<string>();
		challenges = new List<MPChallengeData>();
	}

	public MPPlayerData(Dictionary<string, object> dict)
		: this()
	{
		SetFromDict(dict, true, true, true);
	}

	public void SetFromDict(Dictionary<string, object> dict, bool clearMorties = false, bool clearItems = false, bool clearAvatars = false)
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
			case "coins":
				coins = Convert.ToInt32(item.Value);
				break;
			case "coupons":
				coupons = Convert.ToInt32(item.Value);
				break;
			case "xp":
				xp = Convert.ToInt32(item.Value);
				break;
			case "streak":
				xpMultiplier = Convert.ToInt32(item.Value);
				break;
			case "owned_avatars":
				SetAvatars(ConvertExt.ToDictionaryArray(item.Value), clearAvatars);
				break;
			case "decks":
				SetDecks(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "active_deck_id":
				activeDeckIndex = Convert.ToInt32(item.Value);
				break;
			case "mortydex":
				SetMortydex(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "owned_recipes":
				SetRecipes(ConvertExt.ToDictionaryArray(item.Value));
				break;
			case "challenge_reward":
				challengeComplete = Convert.ToBoolean(item.Value);
				break;
			}
		}
	}

	public override MPOpponentMortyData GetPartyMortyData(int index)
	{
		return morties.FirstOrDefault((MPOpponentMortyData e) => e.netId == decks[activeDeckIndex][index]);
	}

	public override List<MPOpponentMortyData> GetAllPartyMortyDatas()
	{
		List<MPOpponentMortyData> list = new List<MPOpponentMortyData>();
		for (int i = 0; i < GetActiveDeckCount(); i++)
		{
			list.Add(GetPartyMortyData(i));
		}
		return list;
	}

	public override MPOpponentMortyData GetPartyFirstAliveMortyData()
	{
		for (int i = 0; i < GetActiveDeckCount(); i++)
		{
			MPOpponentMortyData partyMortyData = GetPartyMortyData(i);
			if (!partyMortyData.isFainted)
			{
				return partyMortyData;
			}
		}
		return null;
	}

	public bool IsMortyInParty(MPOpponentMortyData mortyData)
	{
		return decks[activeDeckIndex].Contains(mortyData.netId);
	}

	public void SetDecks(Dictionary<string, object>[] deckDict)
	{
		if (deckDict != null)
		{
			for (int i = 0; i < decks.Length; i++)
			{
				decks[i].Clear();
			}
			for (int j = 0; j < deckDict.Length; j++)
			{
				int num = Convert.ToInt32(deckDict[j]["deck_id"]);
				decks[num] = new List<string>((string[])deckDict[j]["owned_morty_ids"]);
			}
		}
	}

	public List<string> GetDeckMortyIDs(int index)
	{
		return decks[index];
	}

	public List<MPOpponentMortyData> GetDeckMortyDatas(int deckIndex)
	{
		List<MPOpponentMortyData> list = new List<MPOpponentMortyData>();
		for (int mortyIndex = 0; mortyIndex < decks[deckIndex].Count; mortyIndex++)
		{
			list.Add(morties.FirstOrDefault((MPOpponentMortyData e) => e.netId == decks[deckIndex][mortyIndex]));
		}
		return list;
	}

	public List<string> GetActiveDeckIDs()
	{
		return GetDeckMortyIDs(activeDeckIndex);
	}

	public int GetActiveDeckCount()
	{
		return GetDeckCount(activeDeckIndex);
	}

	public int GetActiveDeckAliveCount()
	{
		int num = 0;
		for (int i = 0; i < GetActiveDeckCount(); i++)
		{
			MPOpponentMortyData partyMortyData = GetPartyMortyData(i);
			if (!partyMortyData.isFainted)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDeckCount(int deckIndex)
	{
		return decks[deckIndex].Count;
	}

	public int GetDeckAliveCount(int deckIndex)
	{
		int num = 0;
		List<MPOpponentMortyData> deckMortyDatas = GetDeckMortyDatas(deckIndex);
		for (int i = 0; i < deckMortyDatas.Count; i++)
		{
			MPOpponentMortyData mPOpponentMortyData = deckMortyDatas[i];
			if (!mPOpponentMortyData.isFainted)
			{
				num++;
			}
		}
		return num;
	}

	public void SetMortydex(Dictionary<string, object>[] mortydexDictArr)
	{
		if (mortydexDictArr == null)
		{
			return;
		}
		mortydexSeen.Clear();
		mortydexCaught.Clear();
		for (int i = 0; i < mortydexDictArr.Length; i++)
		{
			string item = Convert.ToString(mortydexDictArr[i]["morty_id"]);
			bool flag = Convert.ToBoolean(mortydexDictArr[i]["caught"]);
			mortydexSeen.Add(item);
			if (flag)
			{
				mortydexCaught.Add(item);
			}
		}
	}

	public void SetRecipes(Dictionary<string, object>[] recipesDictArr)
	{
		recipes.Clear();
		if (recipesDictArr != null)
		{
			for (int i = 0; i < recipesDictArr.Length; i++)
			{
				recipes.Add(Convert.ToString(recipesDictArr[i]["recipe_id"]));
			}
		}
	}

	public void SetAvatars(Dictionary<string, object>[] avatarsDictArr, bool clearAvatars = false)
	{
		if (clearAvatars)
		{
			ownedAvatars.Clear();
		}
		if (avatarsDictArr == null)
		{
			return;
		}
		for (int i = 0; i < avatarsDictArr.Length; i++)
		{
			string item = Convert.ToString(avatarsDictArr[i]["player_avatar_id"]);
			if (!ownedAvatars.Contains(item))
			{
				ownedAvatars.Add(item);
			}
		}
	}

	public bool IsAvatarOwned(string id)
	{
		return ownedAvatars.Contains(id);
	}

	public void ApplyRewardDatas(MPRewardData[] rewardDatas)
	{
		if (rewardDatas != null)
		{
			for (int i = 0; i < rewardDatas.Length; i++)
			{
				ApplyRewardData(rewardDatas[i]);
			}
		}
	}

	public void ApplyRewardData(MPRewardData rewardData)
	{
		switch (rewardData.type)
		{
		case eMPRewardType.COIN:
			coins = rewardData.quantity;
			break;
		case eMPRewardType.COUPON:
			coupons = rewardData.quantity;
			break;
		case eMPRewardType.ITEM:
			items[rewardData.id] = rewardData.quantity;
			break;
		case eMPRewardType.MORTY:
			ApplyMortyRewardData(rewardData.ownedMortyDict, rewardData.isAddedToDeck);
			break;
		}
		if (rewardData.substitute != null)
		{
			ApplyRewardData(rewardData.substitute);
		}
	}

	public void ApplyMortyRewardData(Dictionary<string, object> ownedMortyDict, bool addToDeck)
	{
		string item = Convert.ToString(ownedMortyDict["owned_morty_id"]);
		string item2 = Convert.ToString(ownedMortyDict["morty_id"]);
		SetMortiesFromDicts(new Dictionary<string, object>[1] { ownedMortyDict });
		if (addToDeck)
		{
			decks[activeDeckIndex].Add(item);
		}
		if (!App.net.player.data.mortydexSeen.Contains(item2))
		{
			App.net.player.data.mortydexSeen.Add(item2);
		}
		if (!App.net.player.data.mortydexCaught.Contains(item2))
		{
			App.net.player.data.mortydexCaught.Add(item2);
		}
	}

	public void SetChallenges(Dictionary<string, object>[] dictArr)
	{
		challenges.Clear();
		if (dictArr != null)
		{
			for (int i = 0; i < dictArr.Length; i++)
			{
				challenges.Add(new MPChallengeData(dictArr[i]));
			}
		}
	}
}
