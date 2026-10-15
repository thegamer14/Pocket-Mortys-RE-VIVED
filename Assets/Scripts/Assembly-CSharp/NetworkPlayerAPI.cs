using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

public class NetworkPlayerAPI : MonoBehaviour
{
	public delegate void PlayerMortiesChangedDelegate();

	public delegate void PlayerUseItemDelegate(PlayerUseItemResponse responseData, eUseItemError error);

	public delegate void PlayerAvatarChangedDelegate();

	public delegate void VideoRewardReceivedDelegate(int coinsReceived);

	public delegate void VideoRewardMeeseekReceivedDelegate(string id, MPRewardData[] rewards);

	public delegate void ChallengeGetDelegate(eGetChallengesError error);

	public delegate void ChallengeCompleteDelegate();

	public MPPlayerData data;

	public event PlayerMortiesChangedDelegate MortiesChangedEvent;

	public event PlayerUseItemDelegate UseItemEvent;

	public event PlayerAvatarChangedDelegate AvatarChangedEvent;

	public event VideoRewardReceivedDelegate VideoRewardReceivedEvent;

	public event VideoRewardMeeseekReceivedDelegate VideoRewardMeeseekReceivedEvent;

	public event ChallengeGetDelegate GetChallengeEvent;

	public event ChallengeCompleteDelegate ChallengeCompleteEvent;

	public void Connected()
	{
		DebugLog("Connected()");
		App.net.session.On("fyber:reward", VideoRewardReceivedHandler);
		App.net.session.On("incentive:reward", VideoRewardMeeseekReceivedHandler);
		App.net.session.On("challenge:complete", ChallengeCompleteHandler);
	}

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		App.net.session.Off("fyber:reward", VideoRewardReceivedHandler);
		App.net.session.Off("incentive:reward", VideoRewardMeeseekReceivedHandler);
		App.net.session.Off("challenge:complete", ChallengeCompleteHandler);
		data = null;
	}

	public bool IsRegistered()
	{
		return !string.IsNullOrEmpty(App.saveData.multiplayer.secretID);
	}

	public void Register(string username, string playerAvatarID, List<string> mortyIDs, Action<eRegisterFailedError> callback)
	{
		DebugLog(string.Format("Register(username: {0}, playerAvatarID: {1}, mortyIDs: {2})", username, playerAvatarID, mortyIDs.ToStringElements()));
		List<string> sPAvatarsToMigrate = GetSPAvatarsToMigrate();
		sPAvatarsToMigrate.Remove(playerAvatarID);
		sPAvatarsToMigrate.Insert(0, playerAvatarID);
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/user/register";
		networkHTTPRequest.postData = new
		{
			username = username,
			player_avatar_ids = sPAvatarsToMigrate,
			morty_ids = mortyIDs.ToArray()
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerRegisterResponse responseObject = null;
			eRegisterFailedError eRegisterFailedError2 = NetworkDefs.ParseHTTPResponse<eRegisterFailedError, PlayerRegisterResponse>(request, response, out responseObject);
			if (eRegisterFailedError2 == eRegisterFailedError.NONE && responseObject != null)
			{
				App.saveData.multiplayer.secretID = responseObject.secret;
				App.saveData.multiplayer.username = responseObject.username;
				App.saveData.multiplayer.playerAvatarID = responseObject.player_avatar_id;
				App.saveData.Save(eSaveDataType.MULTIPLAYER);
			}
			if (callback != null)
			{
				DebugLog(string.Format("Register() calling back with error: {0}", eRegisterFailedError2));
				callback(eRegisterFailedError2);
			}
		});
	}

	public void GetDetails(ePlayerDataParams paramFlags, Action<eGetPlayerDataError> callback)
	{
		List<string> list = new List<string>();
		if ((paramFlags & ePlayerDataParams.AVATARS) == ePlayerDataParams.AVATARS)
		{
			list.Add("avatars");
		}
		if ((paramFlags & ePlayerDataParams.CURRENCY) == ePlayerDataParams.CURRENCY)
		{
			list.Add("currency");
		}
		if ((paramFlags & ePlayerDataParams.MORTIES) == ePlayerDataParams.MORTIES)
		{
			list.Add("morties");
		}
		if ((paramFlags & ePlayerDataParams.MORTY_ATTACKS) == ePlayerDataParams.MORTY_ATTACKS)
		{
			list.Add("attacks");
		}
		if ((paramFlags & ePlayerDataParams.DECKS) == ePlayerDataParams.DECKS)
		{
			list.Add("decks");
		}
		if ((paramFlags & ePlayerDataParams.MORTYDEX) == ePlayerDataParams.MORTYDEX)
		{
			list.Add("mortydex");
		}
		if ((paramFlags & ePlayerDataParams.ITEMS) == ePlayerDataParams.ITEMS)
		{
			list.Add("items");
		}
		if ((paramFlags & ePlayerDataParams.RECIPES) == ePlayerDataParams.RECIPES)
		{
			list.Add("recipes");
		}
		if ((paramFlags & ePlayerDataParams.CHALLENGE) == ePlayerDataParams.CHALLENGE)
		{
			list.Add("challenge_reward");
		}
		DebugLog(string.Format("GetPlayerDetails() includeParams = {0}", list.ToStringElements()));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/player-details";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			include = list.ToArray()
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			Dictionary<string, object> responseObject = null;
			eGetPlayerDataError eGetPlayerDataError2 = NetworkDefs.ParseHTTPResponse<eGetPlayerDataError, Dictionary<string, object>>(request, response, out responseObject);
			if (eGetPlayerDataError2 == eGetPlayerDataError.NONE && responseObject != null)
			{
				if (data == null)
				{
					data = new MPPlayerData();
					data.type = eMPOpponentType.PLAYER;
				}
				data.SetFromDict(responseObject, true, true, true);
				App.saveData.multiplayer.username = data.username;
				App.saveData.multiplayer.playerAvatarID = data.avatarId;
				App.saveData.Save(eSaveDataType.MULTIPLAYER);
				if (TestDefs.MP_OVERWRITE_PLAYER_LEVEL > 0)
				{
					data.level = TestDefs.MP_OVERWRITE_PLAYER_LEVEL;
				}
				if (this.MortiesChangedEvent != null && (paramFlags & ePlayerDataParams.DECKS) == ePlayerDataParams.DECKS)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("GetPlayerData() calling back with error: {0}", eGetPlayerDataError2));
				callback(eGetPlayerDataError2);
			}
		});
	}

	public void SetState(eMPOpponentState state)
	{
		DebugLog(string.Format("SetState(state: {0})", state.ToString()));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/state";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			state = state.ToString()
		};
		App.net.http.Request(networkHTTPRequest);
	}

	public void EditDeck(int deckId, List<string> deck, Action<eEditDeckError> callback = null)
	{
		DebugLog(string.Format("SetDeck({0})", deck.ToString()));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/deck/edit";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			deck_id = deckId,
			owned_morty_ids = deck
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerEditDeckResponse responseObject = null;
			eEditDeckError eEditDeckError2 = NetworkDefs.ParseHTTPResponse<eEditDeckError, PlayerEditDeckResponse>(request, response, out responseObject);
			if (eEditDeckError2 == eEditDeckError.NONE)
			{
				data.SetDecks(responseObject.decks);
				data.activeDeckIndex = responseObject.active_deck_id;
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("SetDeck() calling back with error: {0}", eEditDeckError2));
				callback(eEditDeckError2);
			}
		});
	}

	public void EditMortyAttacks(Dictionary<string, SortedList<int, MPAttackData>> newAttacks, bool mortyLabs, Action<eEditAttacksError> callback)
	{
		Dictionary<string, object>[] array = new Dictionary<string, object>[newAttacks.Count];
		string[] array2 = newAttacks.Keys.ToArray();
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = new string[4];
			for (int j = 0; j < array3.Length; j++)
			{
				if (newAttacks[array2[i]].ContainsKey(j))
				{
					array3[j] = newAttacks[array2[i]][j].id;
				}
				else
				{
					array3[j] = null;
				}
			}
			array[i] = new Dictionary<string, object>();
			array[i]["owned_morty_id"] = array2[i];
			array[i]["attack_ids"] = array3;
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/owned_morties/learn-attacks";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			owned_morties = array,
			morty_labs = mortyLabs
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerEditAttacksResponse responseObject = null;
			eEditAttacksError eEditAttacksError2 = NetworkDefs.ParseHTTPResponse<eEditAttacksError, PlayerEditAttacksResponse>(request, response, out responseObject);
			if (eEditAttacksError2 == eEditAttacksError.NONE && responseObject != null)
			{
				data.SetMortiesFromDicts(responseObject.owned_morties, false, true);
				if (mortyLabs)
				{
					data.coupons = responseObject.coupons;
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("EditMortyAttacks() calling back with error: {0}", eEditAttacksError2));
				callback(eEditAttacksError2);
			}
		});
	}

	public void ReleaseMorty(string mortyNetId, Action<eReleaseMortyError> callback = null)
	{
		DebugLog(string.Format("ReleaseMorty(mortyNetId = {0})", mortyNetId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/owned_morties/release";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			owned_morty_id = mortyNetId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerReleaseMortyResponse responseObject = null;
			eReleaseMortyError eReleaseMortyError2 = NetworkDefs.ParseHTTPResponse<eReleaseMortyError, PlayerReleaseMortyResponse>(request, response, out responseObject);
			if (eReleaseMortyError2 == eReleaseMortyError.NONE)
			{
				data.RemoveMorty(mortyNetId);
				data.SetDecks(responseObject.decks);
				data.activeDeckIndex = responseObject.active_deck_id;
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("ReleaseMorty() calling back with error: {0}", eReleaseMortyError2));
				callback(eReleaseMortyError2);
			}
		});
	}

	public void SetActiveDeck(int deckIndex, Action<eSetActiveDeckError> callback = null)
	{
		DebugLog(string.Format("SetActiveDeck(deckIndex = {0})", deckIndex));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/deck/set-active";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			deck_id = deckIndex
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerSetActiveDeckResponse responseObject = null;
			eSetActiveDeckError eSetActiveDeckError2 = NetworkDefs.ParseHTTPResponse<eSetActiveDeckError, PlayerSetActiveDeckResponse>(request, response, out responseObject);
			if (eSetActiveDeckError2 == eSetActiveDeckError.NONE)
			{
				data.SetDecks(responseObject.decks);
				data.activeDeckIndex = responseObject.active_deck_id;
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("SetActiveDeck() calling back with error: {0}", eSetActiveDeckError2));
				callback(eSetActiveDeckError2);
			}
		});
	}

	public void HealActiveMortyDeck(Action<eHealActiveMortyDeckError> callback = null)
	{
		DebugLog(string.Format("HealActiveMortyDeck()"));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/heal";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerHealActiveMortyDeckResponse responseObject = null;
			eHealActiveMortyDeckError eHealActiveMortyDeckError2 = NetworkDefs.ParseHTTPResponse<eHealActiveMortyDeckError, PlayerHealActiveMortyDeckResponse>(request, response, out responseObject);
			if (eHealActiveMortyDeckError2 == eHealActiveMortyDeckError.NONE)
			{
				data.xpMultiplier = responseObject.streak;
				data.SetMortiesFromDicts(responseObject.owned_morties);
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("HealActiveMortyDeck() calling back with error: {0}", eHealActiveMortyDeckError2));
				callback(eHealActiveMortyDeckError2);
			}
		});
	}

	public void UseItem(string mortyNetId, string itemId)
	{
		DebugLog(string.Format("UseItem(mortyNetId: {0}, itemID: {1})", mortyNetId, itemId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/use-item";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			owned_morty_id = mortyNetId,
			item_id = itemId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerUseItemResponse responseObject = null;
			eUseItemError eUseItemError2 = NetworkDefs.ParseHTTPResponse<eUseItemError, PlayerUseItemResponse>(request, response, out responseObject);
			if (eUseItemError2 == eUseItemError.NONE && responseObject != null)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.ITEMS, eSubType2.USE, itemId);
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (MPSharedStateInfo.partyInfo != null)
			{
				MPSharedStateInfo.partyInfo.serverError = eUseItemError2;
				MPSharedStateInfo.partyInfo.serverResponse = responseObject;
			}
			if (this.UseItemEvent != null)
			{
				this.UseItemEvent(responseObject, eUseItemError2);
			}
		});
	}

	public void BuyItem(string itemId, Action<eBuyItemError> callback = null)
	{
		DebugLog(string.Format("BuyItem(itemID: {0})", itemId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/buy-item";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			item_id = itemId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			BuyItemResponse responseObject = null;
			eBuyItemError eBuyItemError2 = NetworkDefs.ParseHTTPResponse<eBuyItemError, BuyItemResponse>(request, response, out responseObject);
			if (eBuyItemError2 == eBuyItemError.NONE && responseObject != null)
			{
				data.coins = responseObject.coins;
				data.SetItemsFromDicts(new Dictionary<string, object>[1] { responseObject.result });
			}
			if (callback != null)
			{
				callback(eBuyItemError2);
			}
		});
	}

	public void LearnRecipe(string recipeId, Action<eLearnRecipeError> callback = null)
	{
		if (string.IsNullOrEmpty(recipeId) || data.recipes.Contains(recipeId))
		{
			return;
		}
		DebugLog(string.Format("LearnRecipe(recipeId: {0}", recipeId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/recipe/learn";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			recipe_id = recipeId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eLearnRecipeError eLearnRecipeError2 = NetworkDefs.ParseHTTPError<eLearnRecipeError>(request, response);
			if (eLearnRecipeError2 == eLearnRecipeError.NONE)
			{
				App.net.player.data.recipes.Add(recipeId);
			}
			if (callback != null)
			{
				callback(eLearnRecipeError2);
			}
		});
	}

	public void CraftRecipe(string recipeId, Action<MPRewardData, eCraftRecipeError> callback)
	{
		DebugLog(string.Format("CraftRecipe(recipeId: {0}", recipeId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/recipe/use";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			recipe_id = recipeId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerCraftRecipeResponse responseObject = null;
			eCraftRecipeError eCraftRecipeError2 = NetworkDefs.ParseHTTPResponse<eCraftRecipeError, PlayerCraftRecipeResponse>(request, response, out responseObject);
			MPRewardData mPRewardData = null;
			if (eCraftRecipeError2 == eCraftRecipeError.NONE && responseObject != null)
			{
				if (!data.recipes.Contains(recipeId))
				{
					data.recipes.Add(recipeId);
				}
				App.net.player.data.SetItemsFromDicts(responseObject.owned_items);
				mPRewardData = new MPRewardData(responseObject.result);
				App.net.player.data.ApplyRewardData(mPRewardData);
			}
			if (callback != null)
			{
				callback(mPRewardData, eCraftRecipeError2);
			}
		});
	}

	public void UseGacha(Action<MPRewardData[], eUseGachaError> callback = null)
	{
		DebugLog("UseGacha()");
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/gacha";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			PlayerUseGachaResponse responseObject = null;
			eUseGachaError eUseGachaError2 = NetworkDefs.ParseHTTPResponse<eUseGachaError, PlayerUseGachaResponse>(request, response, out responseObject);
			MPRewardData[] array = null;
			if (eUseGachaError2 == eUseGachaError.NONE && responseObject != null)
			{
				App.net.player.data.coupons = responseObject.coupons;
				if (responseObject.result != null)
				{
					array = MPRewardData.GetRewardDatasForDictArray(responseObject.result);
				}
				App.net.player.data.ApplyRewardDatas(array);
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("UseGacha() calling back with error: {0}", eUseGachaError2));
				callback(array, eUseGachaError2);
			}
		});
	}

	public void ValidateCouponPurchase(string transactionId, string receipt, object googleData = null, Action<eValidateCouponPurchaseError> callback = null)
	{
		DebugLog(string.Format("ValidateCouponPurchase(transactionId: {0} receipt: {1})", transactionId, receipt));
		if (App.saveData == null || App.saveData.multiplayer == null || App.saveData.multiplayer.secretID == null)
		{
			callback(eValidateCouponPurchaseError.SECRET_INVALID);
			return;
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/iap";
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["signature"] = receipt;
		dictionary["data"] = googleData;
		networkHTTPRequest.postData = new
		{
			secret = App.saveData.multiplayer.secretID,
			platform = "google",
			sessionID = App.net.session.sessionID,
			receipt = dictionary,
			transaction_id = transactionId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			ValidateCouponPurchaseResponse responseObject = null;
			eValidateCouponPurchaseError eValidateCouponPurchaseError2 = NetworkDefs.ParseHTTPResponse<eValidateCouponPurchaseError, ValidateCouponPurchaseResponse>(request, response, out responseObject);
			if (eValidateCouponPurchaseError2 == eValidateCouponPurchaseError.NONE && responseObject != null && data != null)
			{
				App.net.player.data.ApplyRewardDatas(MPRewardData.GetRewardDatasForDictArray(responseObject.result));
			}
			if (callback != null)
			{
				DebugLog(string.Format("ValidateCouponPurchase() calling back with error: {0}", eValidateCouponPurchaseError2));
				callback(eValidateCouponPurchaseError2);
			}
		});
	}

	public void BuyAvatar(string avatarId, Action<eBuyAvatarError> callback)
	{
		DebugLog(string.Format("BuyAvatar(avatarId: {0})", avatarId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/player-avatar/buy";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_avatar_id = avatarId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			BuyAvatarResponse responseObject = null;
			eBuyAvatarError eBuyAvatarError2 = NetworkDefs.ParseHTTPResponse<eBuyAvatarError, BuyAvatarResponse>(request, response, out responseObject);
			if (eBuyAvatarError2 == eBuyAvatarError.NONE && responseObject != null)
			{
				App.net.player.data.SetAvatars(new Dictionary<string, object>[1] { responseObject.result });
				App.net.player.data.avatarId = avatarId;
				App.net.player.data.assetId = MPPlayerAvatarDefs.AssetID(avatarId);
				if (MPPlayerAvatarDefs.CurrencyType(avatarId) == eCurrencyType.COIN)
				{
					App.net.player.data.coins = responseObject.coins;
				}
				else
				{
					App.net.player.data.coupons = responseObject.coupons;
				}
				if (PlayerAvatarDefs.GetAvatarIDs().Contains(avatarId))
				{
					PlayerDefs.SetAvatarOwned(avatarId);
				}
				App.saveData.multiplayer.playerAvatarID = avatarId;
				App.saveData.Save(eSaveDataType.PLAYER | eSaveDataType.MULTIPLAYER);
				if (this.AvatarChangedEvent != null)
				{
					this.AvatarChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("BuyAvatar() calling back with error: {0}", eBuyAvatarError2));
				callback(eBuyAvatarError2);
			}
		});
	}

	public void UseAvatar(string avatarId, Action<eUseAvatarError> callback)
	{
		DebugLog(string.Format("UseAvatar(avatarId: {0})", avatarId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/player-avatar/set-active";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_avatar_id = avatarId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eUseAvatarError eUseAvatarError2 = NetworkDefs.ParseHTTPError<eUseAvatarError>(request, response);
			if (eUseAvatarError2 == eUseAvatarError.NONE)
			{
				App.net.player.data.avatarId = avatarId;
				App.net.player.data.assetId = MPPlayerAvatarDefs.AssetID(avatarId);
				App.saveData.multiplayer.playerAvatarID = avatarId;
				App.saveData.Save(eSaveDataType.MULTIPLAYER);
				if (this.AvatarChangedEvent != null)
				{
					this.AvatarChangedEvent();
				}
			}
			if (callback != null)
			{
				DebugLog(string.Format("UseAvatar() calling back with error: {0}", eUseAvatarError2));
				callback(eUseAvatarError2);
			}
		});
	}

	public void MigrateSPAvatars(Action<eMigrateSPAvatarsError> callback)
	{
		DebugLog("MigrateSPAvatars()");
		List<string> sPAvatarsToMigrate = GetSPAvatarsToMigrate();
		if (sPAvatarsToMigrate.Count == 0)
		{
			DebugLog("MigrateSPAvatars() - no avatars to migrate, calling back.");
			if (callback != null)
			{
				callback(eMigrateSPAvatarsError.NONE);
			}
			return;
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/player-avatar/migrate";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_avatar_ids = sPAvatarsToMigrate
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			Dictionary<string, object>[] responseObject = null;
			eMigrateSPAvatarsError eMigrateSPAvatarsError2 = NetworkDefs.ParseHTTPResponse<eMigrateSPAvatarsError, Dictionary<string, object>[]>(request, response, out responseObject);
			if (eMigrateSPAvatarsError2 == eMigrateSPAvatarsError.NONE && responseObject != null)
			{
				data.SetAvatars(responseObject);
			}
			if (callback != null)
			{
				DebugLog(string.Format("MigrateSPAvatars() calling back with error: {0}", eMigrateSPAvatarsError2));
				callback(eMigrateSPAvatarsError2);
			}
		});
	}

	private List<string> GetSPAvatarsToMigrate()
	{
		List<string> list = new List<string>();
		list.AddRange(App.saveData.player.boughtPlayerAvatarIDs);
		List<string> allMPAvatars = MPPlayerAvatarDefs.GetAvatarIDs();
		list.RemoveAll((string id) => !allMPAvatars.Contains(id));
		if (data != null)
		{
			list.RemoveAll((string id) => data.ownedAvatars.Contains(id));
		}
		return list;
	}

	public void AbsorbMorties(string evolveMortyNetId, List<string> absorbMortyNetIds, Action<eAbsorbMortysError, MPOpponentMortyData, bool, List<string>> callback)
	{
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/owned_morties/absorb";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			owned_morty_id = evolveMortyNetId,
			absorb_owned_morty_ids = absorbMortyNetIds
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			AbsorbMortiesResponse responseObject = null;
			eAbsorbMortysError eAbsorbMortysError2 = NetworkDefs.ParseHTTPResponse<eAbsorbMortysError, AbsorbMortiesResponse>(request, response, out responseObject);
			MPOpponentMortyData arg = null;
			bool flag = false;
			List<string> arg2 = null;
			if (eAbsorbMortysError2 == eAbsorbMortysError.NONE)
			{
				flag = responseObject.created != null;
				arg2 = responseObject.removed;
				Dictionary<string, object> dictionary = ((!flag) ? responseObject.modified : responseObject.created);
				data.SetMortiesFromDicts(new Dictionary<string, object>[1] { dictionary });
				if (responseObject.removed != null)
				{
					foreach (string item in responseObject.removed)
					{
						data.RemoveMorty(item);
					}
				}
				data.SetDecks(responseObject.decks);
				if (this.MortiesChangedEvent != null)
				{
					this.MortiesChangedEvent();
				}
				arg = data.GetMortyData(Convert.ToString(dictionary["owned_morty_id"]));
			}
			if (callback != null)
			{
				DebugLog(string.Format("AbsorbMorties() calling back with error: {0}", eAbsorbMortysError2));
				callback(eAbsorbMortysError2, arg, flag, arg2);
			}
		});
	}

	public void GetChallenges()
	{
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/challenge/list";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			Dictionary<string, object>[] responseObject = null;
			eGetChallengesError eGetChallengesError2 = NetworkDefs.ParseHTTPResponse<eGetChallengesError, Dictionary<string, object>[]>(request, response, out responseObject);
			if (eGetChallengesError2 == eGetChallengesError.NONE && responseObject != null)
			{
				data.SetChallenges(responseObject);
			}
			if (this.GetChallengeEvent != null)
			{
				this.GetChallengeEvent(eGetChallengesError2);
			}
		});
	}

	public void ClaimChallenge(MPChallengeData challengeData, Action<MPRewardData, eClaimChallengeError> callback)
	{
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/challenge/claim-reward";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			challenge_id = challengeData.id
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			ClaimChallengesResponse responseObject = null;
			eClaimChallengeError eClaimChallengeError2 = NetworkDefs.ParseHTTPResponse<eClaimChallengeError, ClaimChallengesResponse>(request, response, out responseObject);
			MPRewardData mPRewardData = null;
			if (eClaimChallengeError2 == eClaimChallengeError.NONE && responseObject != null)
			{
				data.challengeComplete = responseObject.challenge_reward;
				challengeData.SetFromDict(responseObject.challenge);
				mPRewardData = new MPRewardData(responseObject.reward);
				App.net.player.data.ApplyRewardData(mPRewardData);
			}
			if (callback != null)
			{
				callback(mPRewardData, eClaimChallengeError2);
			}
		});
	}

	public void PassVideoReward(string id)
	{
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/incentive/reject";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			incentive_id = id
		};
		App.net.http.Request(networkHTTPRequest);
	}

	private void VideoRewardReceivedHandler(NetworkEventSourceResponse response)
	{
		DebugLog(string.Format("VideoRewardReceivedHandler()"));
		VideoRewardReceivedResponse videoRewardReceivedResponse = JsonReader.Deserialize<VideoRewardReceivedResponse>(response.jsonStr);
		data.coins = videoRewardReceivedResponse.quantity;
		if (this.VideoRewardReceivedEvent != null)
		{
			this.VideoRewardReceivedEvent(videoRewardReceivedResponse.amount_received);
		}
	}

	private void VideoRewardMeeseekReceivedHandler(NetworkEventSourceResponse response)
	{
		DebugLog(string.Format("VideoRewardMeeseekReceivedHandler()"));
		VideoRewardMeeseekReceivedResponse videoRewardMeeseekReceivedResponse = JsonReader.Deserialize<VideoRewardMeeseekReceivedResponse>(response.jsonStr);
		MPRewardData[] rewardDatasForDictArray = MPRewardData.GetRewardDatasForDictArray(videoRewardMeeseekReceivedResponse.rewards);
		data.ApplyRewardDatas(rewardDatasForDictArray);
		if (this.VideoRewardMeeseekReceivedEvent != null)
		{
			this.VideoRewardMeeseekReceivedEvent(videoRewardMeeseekReceivedResponse.incentive_id, rewardDatasForDictArray);
		}
	}

	private void ChallengeCompleteHandler(NetworkEventSourceResponse response)
	{
		DebugLog(string.Format("ChallengeCompleteHandler()"));
		data.challengeComplete = true;
		if (this.ChallengeCompleteEvent != null)
		{
			this.ChallengeCompleteEvent();
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_PLAYER_API)
		{
		}
	}
}
