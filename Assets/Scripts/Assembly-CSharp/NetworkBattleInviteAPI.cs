using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

public class NetworkBattleInviteAPI : MonoBehaviour
{
	public delegate void BattleInviteQueueChangedDelegate(List<MPBattleInviteData> invites, bool friendInvitePending);

	public delegate void SentInviteDelegate();

	private List<MPBattleInviteData> _receivedInvites;

	public bool friendInvitePending { get; private set; }

	public bool sentInvitePending { get; private set; }

	public MPBattleInviteData sentInvite { get; private set; }

	public event BattleInviteQueueChangedDelegate InviteQueueChangeEvent;

	public event SentInviteDelegate SentInviteExpiredEvent;

	public event SentInviteDelegate SentInviteDeclinedEvent;

	private void Awake()
	{
		_receivedInvites = new List<MPBattleInviteData>();
	}

	private void Start()
	{
		AddEvents();
	}

	private void OnDestroy()
	{
		RemoveEvents();
	}

	public void Connected()
	{
		App.net.session.On("battle-invite:received", InviteReceivedHandler);
		App.net.session.On("battle-invite:declined", InviteDeclinedHandler);
		App.net.session.On("battle-invite:revoked", InviteRevokedHandler);
		App.net.session.On("battle:start", BattleStartHandler);
	}

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		App.net.session.Off("battle-invite:received", InviteReceivedHandler);
		App.net.session.Off("battle-invite:declined", InviteDeclinedHandler);
		App.net.session.Off("battle-invite:revoked", InviteRevokedHandler);
		App.net.session.Off("battle:start", BattleStartHandler);
		_receivedInvites.Clear();
		InviteQueueChanged();
	}

	private void AddEvents()
	{
		App.net.room.UserRemovedEvent += RoomUserRemovedHandler;
	}

	private void RemoveEvents()
	{
		App.net.room.UserRemovedEvent -= RoomUserRemovedHandler;
	}

	private void Update()
	{
		CheckForExpiredReceivedInvites();
		CheckForExpiredSentInvite();
	}

	public void SendInvite(string enemyUserID, string battleType, Action<eSendInviteError> callback = null)
	{
		DebugLog(string.Format("SendInvite(enemyUserID = {0})", enemyUserID));
		if (sentInvitePending)
		{
			if (callback != null)
			{
				callback(eSendInviteError.SENT_INVITE_PENDING);
			}
			return;
		}
		for (int num = _receivedInvites.Count - 1; num >= 0; num--)
		{
			if (_receivedInvites[num].type == battleType)
			{
				RespondToInvite(_receivedInvites[num], false);
			}
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/battle-invite/create";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			opponent_player_id = enemyUserID,
			battle_type = battleType
		};
		sentInvitePending = true;
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			SendInviteResponse responseObject = null;
			eSendInviteError eSendInviteError2 = NetworkDefs.ParseHTTPResponse<eSendInviteError, SendInviteResponse>(request, response, out responseObject);
			if (eSendInviteError2 == eSendInviteError.NONE && responseObject != null)
			{
				MPUserData userData = new MPUserData
				{
					netId = enemyUserID
				};
				sentInvite = new MPBattleInviteData(responseObject.battle_invite_id, responseObject.battle_type, userData, responseObject.battle_invite_ttl, responseObject.token);
			}
			else
			{
				sentInvitePending = false;
			}
			if (callback != null)
			{
				DebugLog(string.Format("SendInvite() calling back with error: {0}", eSendInviteError2));
				callback(eSendInviteError2);
			}
		});
	}

	public void SendInviteWildMorty(string wildMortyId)
	{
		DebugLog(string.Format("SendInviteWildMorty(wildMortyId = {0})", wildMortyId));
		for (int num = _receivedInvites.Count - 1; num >= 0; num--)
		{
			RespondToInvite(_receivedInvites[num], false);
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/battle-wild-morty";
		networkHTTPRequest.postData = new
		{
			wild_morty_id = wildMortyId,
			session_id = App.net.session.sessionID
		};
		sentInvitePending = true;
		App.net.http.Request(networkHTTPRequest);
	}

	public void RespondToInvite(MPBattleInviteData data, bool accept, Action<eRespondToInviteError> callback = null)
	{
		DebugLog(string.Format("RespondToInvite(battleInviteID = {0}, accept = {1})", data.id, accept));
		_receivedInvites.Remove(data);
		InviteQueueChanged();
		if (accept)
		{
			for (int num = _receivedInvites.Count - 1; num >= 0; num--)
			{
				RespondToInvite(_receivedInvites[num], false);
			}
		}
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/battle-invite/response";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			token = data.token,
			accept = accept
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eRespondToInviteError eRespondToInviteError2 = NetworkDefs.ParseHTTPError<eRespondToInviteError>(request, response);
			if (callback != null)
			{
				DebugLog(string.Format("RespondToInvite() calling back with error: {0}", eRespondToInviteError2));
				callback(eRespondToInviteError2);
			}
		});
	}

	public void CancelPendingSentInvite()
	{
		DebugLog("CancelPendingSentInvite()");
		if (sentInvite != null)
		{
			NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
			networkHTTPRequest.path = "/session/battle-invite/revoke";
			networkHTTPRequest.postData = new
			{
				session_id = App.net.session.sessionID,
				token = sentInvite.token
			};
			App.net.http.Request(networkHTTPRequest);
			sentInvite = null;
			sentInvitePending = false;
		}
	}

	private void InviteQueueChanged()
	{
		friendInvitePending = _receivedInvites.FirstOrDefault((MPBattleInviteData e) => e.type == "PvF") != null;
		if (this.InviteQueueChangeEvent != null)
		{
			this.InviteQueueChangeEvent(_receivedInvites.ToList(), friendInvitePending);
		}
	}

	public List<MPBattleInviteData> GetReceivedInvites()
	{
		return _receivedInvites.ToList();
	}

	private void CheckForExpiredReceivedInvites()
	{
		for (int num = _receivedInvites.Count - 1; num >= 0; num--)
		{
			MPBattleInviteData mPBattleInviteData = _receivedInvites[num];
			mPBattleInviteData.ManualUpdateTtl();
			if (mPBattleInviteData.ttl == 0f)
			{
				DebugLog("CheckForExpiredReceivedInvites() - found expired received invite: " + mPBattleInviteData.id);
				RespondToInvite(mPBattleInviteData, false);
			}
		}
	}

	private void CheckForExpiredSentInvite()
	{
		if (sentInvite == null)
		{
			return;
		}
		sentInvite.ManualUpdateTtl();
		if (sentInvite.ttl == 0f)
		{
			DebugLog("CheckForExpiredSentInvite() - found expired sent invite: " + sentInvite.id);
			CancelPendingSentInvite();
			if (this.SentInviteExpiredEvent != null)
			{
				this.SentInviteExpiredEvent();
			}
		}
	}

	private void InviteReceivedHandler(NetworkEventSourceResponse response)
	{
		DebugLog("InviteReceivedHandler()");
		BattleInviteReceivedResponse battleInviteReceivedResponse = JsonReader.Deserialize<BattleInviteReceivedResponse>(response.jsonStr);
		Dictionary<string, object> from_player = battleInviteReceivedResponse.from_player;
		string userNetId = Convert.ToString(from_player["player_id"]);
		MPUserData mPUserData = App.net.room.users.FirstOrDefault((MPUserData e) => e.netId == userNetId);
		if (mPUserData != null)
		{
			mPUserData.SetFromDict(from_player);
		}
		else
		{
			mPUserData = new MPUserData(from_player);
		}
		MPBattleInviteData item = new MPBattleInviteData(battleInviteReceivedResponse.battle_invite_id, battleInviteReceivedResponse.battle_type, mPUserData, battleInviteReceivedResponse.battle_invite_ttl, battleInviteReceivedResponse.token);
		_receivedInvites.Add(item);
		InviteQueueChanged();
	}

	private void InviteDeclinedHandler(NetworkEventSourceResponse response)
	{
		DebugLog("InviteDeclinedHandler()");
		InviteDeclinedResponse inviteDeclinedResponse = JsonReader.Deserialize<InviteDeclinedResponse>(response.jsonStr);
		if (sentInvite != null && sentInvite.id.Equals(inviteDeclinedResponse.battle_invite_id))
		{
			sentInvite = null;
			sentInvitePending = false;
			if (this.SentInviteDeclinedEvent != null)
			{
				this.SentInviteDeclinedEvent();
			}
		}
	}

	private void InviteRevokedHandler(NetworkEventSourceResponse response)
	{
		DebugLog("InviteRevokedHandler()");
		InviteRevokedResponse responseData = JsonReader.Deserialize<InviteRevokedResponse>(response.jsonStr);
		MPBattleInviteData mPBattleInviteData = _receivedInvites.FirstOrDefault((MPBattleInviteData i) => i.id.Equals(responseData.battle_invite_id));
		if (mPBattleInviteData != null)
		{
			_receivedInvites.Remove(mPBattleInviteData);
			InviteQueueChanged();
		}
	}

	private void BattleStartHandler(NetworkEventSourceResponse response)
	{
		DebugLog("BattleStartHandler()");
		sentInvite = null;
		sentInvitePending = false;
		for (int num = _receivedInvites.Count - 1; num >= 0; num--)
		{
			RespondToInvite(_receivedInvites[num], false);
		}
	}

	private void RoomUserRemovedHandler(MPUserData userData)
	{
		if (sentInvite != null && userData.netId == sentInvite.userData.netId)
		{
			CancelPendingSentInvite();
			if (this.SentInviteDeclinedEvent != null)
			{
				this.SentInviteDeclinedEvent();
			}
		}
		for (int num = _receivedInvites.Count - 1; num >= 0; num--)
		{
			bool flag = false;
			if (userData.netId == _receivedInvites[num].userData.netId)
			{
				_receivedInvites.RemoveAt(num);
				flag = true;
			}
			if (flag)
			{
				InviteQueueChanged();
			}
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_BATTLE_INVITE_API)
		{
		}
	}
}
