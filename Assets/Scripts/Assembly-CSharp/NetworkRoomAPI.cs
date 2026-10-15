using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

public class NetworkRoomAPI : MonoBehaviour
{
	public delegate void UserDelegate(MPUserData userData);

	public delegate void WildMortyDelegate(MPOpponentData wildMortyData);

	public delegate void PickupDelegate(MPPickupData pickup);

	public delegate void PickupResponseDelegate(MPPickupData pickup, eRequestPickupCollectError error);

	public MPVideoRewardData videoRewardMeeseeksData;

	private UdpChannel _udpChannel;

	private string _UDPHost;

	private int _UDPPort;

	public List<MPUserData> users { get; private set; }

	public List<MPOpponentData> wildMorties { get; private set; }

	public List<MPPickupData> pickups { get; private set; }

	public UdpNodeSender nodeSender { get; private set; }

	public UdpRoomReceiver nodeReceiver { get; private set; }

	public bool isPaused { get; private set; }

	public bool udpSendEnabled
	{
		set
		{
			if (_udpChannel != null)
			{
				_udpChannel.sendEnabled = value;
			}
		}
	}

	public event UserDelegate UserAddedEvent;

	public event UserDelegate UserRemovedEvent;

	public event UserDelegate UserStateChangedEvent;

	public event UserDelegate UserModifiedEvent;

	public event WildMortyDelegate WildMortyAddedEvent;

	public event WildMortyDelegate WildMortyRemovedEvent;

	public event WildMortyDelegate WildMortyStateChangedEvent;

	public event PickupDelegate PickupAddedEvent;

	public event PickupDelegate PickupRemovedEvent;

	public event PickupResponseDelegate PickupCollectEvent;

	private void Awake()
	{
		users = new List<MPUserData>();
		wildMorties = new List<MPOpponentData>();
		pickups = new List<MPPickupData>();
		_udpChannel = base.gameObject.AddComponent<UdpChannel>();
		_udpChannel.channelName = "Room";
		nodeSender = base.gameObject.AddComponent<UdpNodeSender>();
		nodeSender.udpChannel = _udpChannel;
		nodeReceiver = base.gameObject.AddComponent<UdpRoomReceiver>();
		nodeReceiver.udpChannel = _udpChannel;
	}

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		Leave();
	}

	public void Join(string worldId, Action<eJoinError, string> callback = null)
	{
		DebugLog(string.Format("Join(worldId:{0})", worldId));
		isPaused = false;
		App.saveData.multiplayer.lastWorldId = worldId;
		App.saveData.Save(eSaveDataType.MULTIPLAYER);
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/join-room";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			world_id = worldId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			RoomJoinedResponse responseObject = null;
			eJoinError eJoinError2 = NetworkDefs.ParseHTTPResponse<eJoinError, RoomJoinedResponse>(request, response, out responseObject);
			string text = null;
			if (eJoinError2 == eJoinError.NONE && responseObject != null)
			{
				App.net.player.data.placementData = new MPNodePlacementData();
				App.net.player.data.placementData.coord = MPWorldDefs.GetPlayerSpawnCoord(responseObject.world_id);
				users.Add(App.net.player.data);
				Dictionary<string, object>[] array = responseObject.users;
				foreach (Dictionary<string, object> userDict in array)
				{
					UpdateUserFromDict(userDict);
				}
				Dictionary<string, object>[] wild_morties = responseObject.wild_morties;
				foreach (Dictionary<string, object> wildMortyDict in wild_morties)
				{
					UpdateWildMortyFromDict(wildMortyDict);
				}
				Dictionary<string, object>[] array2 = responseObject.pickups;
				foreach (Dictionary<string, object> pickupDict in array2)
				{
					AddPickupFromDict(pickupDict);
				}
				if (responseObject.incentive != null)
				{
					videoRewardMeeseeksData = new MPVideoRewardData(responseObject.incentive);
				}
				text = responseObject.world_id;
				_UDPHost = responseObject.room_udp_host;
				_UDPPort = responseObject.room_udp_port;
				OpenComs();
			}
			else if (eJoinError2 == eJoinError.ROOM_PLAYER_LEVEL && worldId != "1")
			{
				Join("1", callback);
				return;
			}
			if (callback != null)
			{
				DebugLog(string.Format("Connect() calling back with error: {0} worldId: {1}", eJoinError2, text));
				callback(eJoinError2, text);
			}
		});
	}

	public void Leave(Action callback = null)
	{
		DebugLog(string.Format("Leave()"));
		CloseComs();
		users.Clear();
		wildMorties.Clear();
		pickups.Clear();
		_UDPHost = null;
		_UDPPort = 0;
		isPaused = false;
		if (!App.net.session.isConnected)
		{
			return;
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/leave-room";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eLeaveError eLeaveError2 = NetworkDefs.ParseHTTPError<eLeaveError>(request, response);
			if (eLeaveError2 == eLeaveError.NONE)
			{
				if (callback != null)
				{
					callback();
				}
			}
			else
			{
				App.net.session.Disconnect(eNetSessionDisconnectType.FAILED_TO_LEAVE_ROOM);
			}
		});
	}

	public void Pause()
	{
		DebugLog(string.Format("Pause()"));
		isPaused = true;
		CloseComs();
		users.Clear();
		wildMorties.Clear();
		pickups.Clear();
	}

	public void Unpause(Action callback)
	{
		DebugLog("Unpause()");
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/room-details";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			isPaused = false;
			RoomDetailsResponse responseObject = null;
			eUnpauseRoomError eUnpauseRoomError2 = NetworkDefs.ParseHTTPResponse<eUnpauseRoomError, RoomDetailsResponse>(request, response, out responseObject);
			if (eUnpauseRoomError2 == eUnpauseRoomError.NONE && responseObject != null)
			{
				users.Clear();
				wildMorties.Clear();
				pickups.Clear();
				users.Add(App.net.player.data);
				Dictionary<string, object>[] array = responseObject.users;
				foreach (Dictionary<string, object> userDict in array)
				{
					UpdateUserFromDict(userDict);
				}
				Dictionary<string, object>[] wild_morties = responseObject.wild_morties;
				foreach (Dictionary<string, object> wildMortyDict in wild_morties)
				{
					UpdateWildMortyFromDict(wildMortyDict);
				}
				Dictionary<string, object>[] array2 = responseObject.pickups;
				foreach (Dictionary<string, object> pickupDict in array2)
				{
					AddPickupFromDict(pickupDict);
				}
				OpenComs();
				if (this.UserAddedEvent != null)
				{
					for (int l = 0; l < users.Count; l++)
					{
						this.UserAddedEvent(users[l]);
					}
				}
				if (this.WildMortyAddedEvent != null)
				{
					for (int m = 0; m < wildMorties.Count; m++)
					{
						this.WildMortyAddedEvent(wildMorties[m]);
					}
				}
				if (this.PickupAddedEvent != null)
				{
					for (int n = 0; n < pickups.Count; n++)
					{
						this.PickupAddedEvent(pickups[n]);
					}
				}
				if (callback != null)
				{
					callback();
				}
			}
			else
			{
				App.net.session.Disconnect(eNetSessionDisconnectType.FAILED_TO_UNPAUSE_ROOM);
			}
		});
	}

	private void OpenComs()
	{
		App.net.session.On("room:user-added", UserAddedHandler);
		App.net.session.On("room:user-removed", UserRemovedHandler);
		App.net.session.On("room:user-state-changed", UserStateChangedHandler);
		App.net.session.On("room:user-modified", UserModifiedHandler);
		App.net.session.On("room:wild-morty-added", WildMortyAddedHandler);
		App.net.session.On("room:wild-morty-removed", WildMortyRemovedHandler);
		App.net.session.On("room:wild-morty-state-changed", WildMortyStateChangedHandler);
		App.net.session.On("room:pickup-added", PickupAddedHandler);
		App.net.session.On("room:pickup-removed", PickupRemovedHandler);
		nodeReceiver.ClearLastSeqNumDict();
		_udpChannel.Open(_UDPHost, _UDPPort);
	}

	private void CloseComs()
	{
		App.net.session.Off("room:user-added", UserAddedHandler);
		App.net.session.Off("room:user-removed", UserRemovedHandler);
		App.net.session.Off("room:user-state-changed", UserStateChangedHandler);
		App.net.session.Off("room:user-modified", UserModifiedHandler);
		App.net.session.Off("room:wild-morty-added", WildMortyAddedHandler);
		App.net.session.Off("room:wild-morty-removed", WildMortyRemovedHandler);
		App.net.session.Off("room:wild-morty-state-changed", WildMortyStateChangedHandler);
		App.net.session.Off("room:pickup-added", PickupAddedHandler);
		App.net.session.Off("room:pickup-removed", PickupRemovedHandler);
		_udpChannel.Close();
	}

	private MPUserData UpdateUserFromDict(Dictionary<string, object> userDict, bool clearItems = false, bool clearMorties = false)
	{
		string userId = Convert.ToString(userDict["player_id"]);
		try
		{
			if (userDict.ContainsKey("placement"))
			{
				userDict["placement"] = new Dictionary<string, object> { 
				{
					"coord",
					(int[])userDict["placement"]
				} };
			}
		}
		catch
		{
		}
		MPUserData mPUserData = users.FirstOrDefault((MPUserData e) => e.netId == userId);
		if (mPUserData != null)
		{
			mPUserData.SetFromDict(userDict, clearItems, clearMorties);
		}
		else
		{
			mPUserData = new MPUserData(userDict);
			mPUserData.type = eMPOpponentType.USER;
			users.Add(mPUserData);
		}
		return mPUserData;
	}

	private MPUserData RemoveUserFromDict(Dictionary<string, object> userDict)
	{
		string userId = Convert.ToString(userDict["player_id"]);
		MPUserData mPUserData = users.FirstOrDefault((MPUserData e) => e.netId == userId);
		if (mPUserData != null)
		{
			users.Remove(mPUserData);
		}
		return mPUserData;
	}

	private MPOpponentData UpdateWildMortyFromDict(Dictionary<string, object> wildMortyDict, bool clearItems = false, bool clearMorties = false)
	{
		string wildMortyId = Convert.ToString(wildMortyDict["wild_morty_id"]);
		wildMortyDict.Add("player_id", wildMortyId);
		wildMortyDict.Remove("wild_morty_id");
		try
		{
			if (wildMortyDict.ContainsKey("placement"))
			{
				wildMortyDict["placement"] = new Dictionary<string, object> { 
				{
					"coord",
					(int[])wildMortyDict["placement"]
				} };
			}
		}
		catch
		{
		}
		MPOpponentData mPOpponentData = wildMorties.FirstOrDefault((MPOpponentData e) => e.netId == wildMortyId);
		if (mPOpponentData != null)
		{
			mPOpponentData.SetFromDict(wildMortyDict, clearItems, clearMorties);
		}
		else
		{
			mPOpponentData = new MPOpponentData(wildMortyDict);
			mPOpponentData.type = eMPOpponentType.WILD_MORTY;
			wildMorties.Add(mPOpponentData);
		}
		return mPOpponentData;
	}

	private MPOpponentData RemoveWildMortyFromDict(Dictionary<string, object> wildMortyDict)
	{
		string wildMortyId = Convert.ToString(wildMortyDict["wild_morty_id"]);
		MPOpponentData mPOpponentData = wildMorties.FirstOrDefault((MPOpponentData e) => e.netId == wildMortyId);
		if (mPOpponentData != null)
		{
			wildMorties.Remove(mPOpponentData);
		}
		return mPOpponentData;
	}

	public MPPickupData AddPickupFromDict(Dictionary<string, object> pickupDict)
	{
		MPPickupData mPPickupData = new MPPickupData(pickupDict);
		pickups.Add(mPPickupData);
		return mPPickupData;
	}

	public MPPickupData RemovePickupFromDict(string netId)
	{
		MPPickupData mPPickupData = pickups.FirstOrDefault((MPPickupData e) => e.netId == netId);
		if (mPPickupData != null)
		{
			pickups.Remove(mPPickupData);
		}
		return mPPickupData;
	}

	public void RequestPickupCollect(MPPickupData pickupData)
	{
		DebugLog("RequestPickupCollect()");
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/collect-pickup";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			pickup_id = pickupData.netId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			RoomPickupCollectResponse responseObject = null;
			eRequestPickupCollectError eRequestPickupCollectError2 = NetworkDefs.ParseHTTPResponse<eRequestPickupCollectError, RoomPickupCollectResponse>(request, response, out responseObject);
			if (eRequestPickupCollectError2 == eRequestPickupCollectError.NONE && responseObject != null)
			{
				pickupData.rewardDatas = MPRewardData.GetRewardDatasForDictArray(responseObject.contents);
				App.net.player.data.ApplyRewardDatas(pickupData.rewardDatas);
			}
			if (MPSharedStateInfo.rewardInfo != null)
			{
				eMPRewardConfirmation confirmation = ((eRequestPickupCollectError2 != eRequestPickupCollectError.NONE) ? eMPRewardConfirmation.FAIL : eMPRewardConfirmation.SUCCESS);
				MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.rewardInfo.origin, pickupData.rewardDatas, "item_crate", confirmation);
			}
			if (this.PickupCollectEvent != null)
			{
				this.PickupCollectEvent(pickupData, eRequestPickupCollectError2);
			}
		});
	}

	private void UserAddedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> userDict;
		try
		{
			userDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPUserData userData = UpdateUserFromDict(userDict);
		if (this.UserAddedEvent != null)
		{
			this.UserAddedEvent(userData);
		}
	}

	private void UserRemovedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> userDict;
		try
		{
			userDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPUserData userData = RemoveUserFromDict(userDict);
		if (this.UserRemovedEvent != null)
		{
			this.UserRemovedEvent(userData);
		}
	}

	private void UserStateChangedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> userDict;
		try
		{
			userDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPUserData userData = UpdateUserFromDict(userDict);
		if (this.UserStateChangedEvent != null)
		{
			this.UserStateChangedEvent(userData);
		}
	}

	private void UserModifiedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> dictionary;
		try
		{
			dictionary = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		if (!((string)dictionary["player_id"] == App.net.player.data.netId))
		{
			MPUserData userData = UpdateUserFromDict(dictionary, true, true);
			if (this.UserModifiedEvent != null)
			{
				this.UserModifiedEvent(userData);
			}
		}
	}

	private void WildMortyAddedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> wildMortyDict;
		try
		{
			wildMortyDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPOpponentData wildMortyData = UpdateWildMortyFromDict(wildMortyDict);
		if (this.WildMortyAddedEvent != null)
		{
			this.WildMortyAddedEvent(wildMortyData);
		}
	}

	private void WildMortyRemovedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> wildMortyDict;
		try
		{
			wildMortyDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPOpponentData wildMortyData = RemoveWildMortyFromDict(wildMortyDict);
		if (this.WildMortyRemovedEvent != null)
		{
			this.WildMortyRemovedEvent(wildMortyData);
		}
	}

	private void WildMortyStateChangedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> wildMortyDict;
		try
		{
			wildMortyDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		}
		catch (Exception)
		{
			return;
		}
		MPOpponentData wildMortyData = UpdateWildMortyFromDict(wildMortyDict);
		if (this.WildMortyStateChangedEvent != null)
		{
			this.WildMortyStateChangedEvent(wildMortyData);
		}
	}

	public void PickupAddedHandler(NetworkEventSourceResponse response)
	{
		Dictionary<string, object> pickupDict = JsonReader.Deserialize<Dictionary<string, object>>(response.jsonStr);
		MPPickupData pickup = AddPickupFromDict(pickupDict);
		if (this.PickupAddedEvent != null)
		{
			this.PickupAddedEvent(pickup);
		}
	}

	public void PickupRemovedHandler(NetworkEventSourceResponse response)
	{
		RoomPickupRemovedResponse roomPickupRemovedResponse = JsonReader.Deserialize<RoomPickupRemovedResponse>(response.jsonStr);
		MPPickupData pickup = RemovePickupFromDict(roomPickupRemovedResponse.pickup_id);
		if (this.PickupRemovedEvent != null)
		{
			this.PickupRemovedEvent(pickup);
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_ROOM_API)
		{
		}
	}
}
