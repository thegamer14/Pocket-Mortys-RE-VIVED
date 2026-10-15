using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using UnityEngine;

public class NetworkFriendAPI : MonoBehaviour
{
	public delegate void FriendDelegate(List<MPFriendData> friendRequests, List<MPFriendData> friends, string mortyRequestId, float mortyRequestCountdown, MPFriendData mortyDonatedUser, int pollingInterval, eFriendRefreshDataError error);

	public string mortyRequestId;

	public float mortyRequestCountdown;

	public MPFriendData mortyDonatedUser;

	public int friendListLimit;

	public List<MPFriendData> friendRequests { get; private set; }

	public List<MPFriendData> friends { get; private set; }

	public event FriendDelegate RefreshedDataEvent;

	private void Awake()
	{
		mortyRequestCountdown = -1f;
		friends = new List<MPFriendData>();
		friendRequests = new List<MPFriendData>();
	}

	private void Update()
	{
		if (mortyRequestCountdown > 0f)
		{
			mortyRequestCountdown -= Time.deltaTime;
			if (mortyRequestCountdown < 0f)
			{
				mortyRequestCountdown = 0f;
			}
		}
	}

	public void Connected()
	{
		DebugLog("Connected()");
		App.net.session.On("friend:donation", DonationMortyReceivedHandler);
	}

	public void Disconnected()
	{
		DebugLog("Disconnected()");
		App.net.session.Off("friend:donation", DonationMortyReceivedHandler);
	}

	public void RefreshData(Action<eFriendRefreshDataError> callback = null)
	{
		DebugLog("RefreshData()");
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/list";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			FriendRefreshDataResponse responseObject = null;
			eFriendRefreshDataError eFriendRefreshDataError2 = NetworkDefs.ParseHTTPResponse<eFriendRefreshDataError, FriendRefreshDataResponse>(request, response, out responseObject);
			if (eFriendRefreshDataError2 == eFriendRefreshDataError.NONE && responseObject != null)
			{
				RefreshDataFromResponse(responseObject);
			}
			if (this.RefreshedDataEvent != null)
			{
				this.RefreshedDataEvent(friendRequests, friends, responseObject.donation_request.morty_id, responseObject.donation_request.next_countdown, responseObject.donation_request.donation, responseObject.poll_period, eFriendRefreshDataError2);
			}
			if (callback != null)
			{
				callback(eFriendRefreshDataError2);
			}
		});
	}

	private void RefreshDataFromResponse(FriendRefreshDataResponse responseData)
	{
		mortyRequestId = responseData.donation_request.morty_id;
		mortyRequestCountdown = responseData.donation_request.next_countdown;
		mortyDonatedUser = responseData.donation_request.donation;
		friends.Clear();
		friendRequests.Clear();
		for (int i = 0; i < responseData.requests_received.Length; i++)
		{
			UpdateFriendRequestFromDict(responseData.requests_received[i]);
		}
		for (int j = 0; j < responseData.friends.Length; j++)
		{
			UpdateFriendFromDict(responseData.friends[j], false);
		}
		for (int k = 0; k < responseData.requests_sent.Length; k++)
		{
			UpdateFriendFromDict(responseData.requests_sent[k], true);
		}
		friendListLimit = responseData.limit_friends;
	}

	public void SearchForUser(string username, Action<MPFriendData, eFriendSearchForUserError> callback = null)
	{
		DebugLog(string.Format("SearchForUser({0})", username));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/search";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			query = username
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			FriendSearchForUserResponse responseObject = null;
			eFriendSearchForUserError eFriendSearchForUserError2 = NetworkDefs.ParseHTTPResponse<eFriendSearchForUserError, FriendSearchForUserResponse>(request, response, out responseObject);
			MPFriendData arg = null;
			if (eFriendSearchForUserError2 == eFriendSearchForUserError.NONE && responseObject != null)
			{
				arg = new MPFriendData
				{
					userId = responseObject.player_id,
					username = responseObject.username,
					avatarId = responseObject.player_avatar_id,
					level = responseObject.level
				};
			}
			if (callback != null)
			{
				callback(arg, eFriendSearchForUserError2);
			}
		});
	}

	public void SendRequest(string user_id, Action<eFriendSendRequestError> callback = null)
	{
		DebugLog(string.Format("SendRequest({0})", user_id));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/request";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_id = user_id
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eFriendSendRequestError eFriendSendRequestError2 = NetworkDefs.ParseHTTPError<eFriendSendRequestError>(request, response);
			if (eFriendSendRequestError2 == eFriendSendRequestError.NONE)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.FRIENDS, "friend_request_sent");
			}
			if (callback != null)
			{
				callback(eFriendSendRequestError2);
			}
		});
	}

	public void RespondToRequest(string userId, bool approve, Action<eFriendRespondToRequestError> callback = null)
	{
		DebugLog(string.Format("RespondToRequest({0}, {1})", userId, approve));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/response";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_id = userId,
			approve = approve
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eFriendRespondToRequestError error = NetworkDefs.ParseHTTPError<eFriendRespondToRequestError>(request, response);
			if (error == eFriendRespondToRequestError.NONE)
			{
				RefreshData(delegate(eFriendRefreshDataError refreshError)
				{
					if (refreshError == eFriendRefreshDataError.NONE)
					{
						if (callback != null)
						{
							callback(error);
						}
					}
					else
					{
						App.net.session.Disconnect(eNetSessionDisconnectType.REFRESH_FRIEND_LIST_FAILED);
					}
				});
			}
			else if (callback != null)
			{
				callback(error);
			}
		});
	}

	private MPFriendData UpdateFriendRequestFromDict(Dictionary<string, object> friendRequestDict)
	{
		string userId = Convert.ToString(friendRequestDict["player_id"]);
		MPFriendData mPFriendData = friendRequests.FirstOrDefault((MPFriendData e) => e.userId == userId);
		if (mPFriendData != null)
		{
			mPFriendData.SetFromDict(friendRequestDict);
		}
		else
		{
			mPFriendData = new MPFriendData(friendRequestDict);
			friendRequests.Add(mPFriendData);
		}
		return mPFriendData;
	}

	private MPFriendData RemoveFriendRequestFromDict(Dictionary<string, object> friendRequestDict)
	{
		string userId = Convert.ToString(friendRequestDict["player_id"]);
		MPFriendData mPFriendData = friendRequests.FirstOrDefault((MPFriendData e) => e.userId == userId);
		if (mPFriendData != null)
		{
			friendRequests.Remove(mPFriendData);
		}
		return mPFriendData;
	}

	private MPFriendData UpdateFriendFromDict(Dictionary<string, object> friendDict, bool isPending)
	{
		string userId = Convert.ToString(friendDict["player_id"]);
		MPFriendData mPFriendData = friends.FirstOrDefault((MPFriendData e) => e.userId == userId);
		if (mPFriendData != null)
		{
			mPFriendData.SetFromDict(friendDict);
		}
		else
		{
			mPFriendData = new MPFriendData(friendDict);
			friends.Add(mPFriendData);
		}
		mPFriendData.isPending = isPending;
		return mPFriendData;
	}

	private MPFriendData RemoveFriendFromDict(Dictionary<string, object> friendDict)
	{
		string userId = Convert.ToString(friendDict["player_id"]);
		MPFriendData mPFriendData = friends.FirstOrDefault((MPFriendData e) => e.userId == userId);
		if (mPFriendData != null)
		{
			friends.Remove(mPFriendData);
		}
		return mPFriendData;
	}

	public void Remove(string userId, Action<eFriendRemoveError> callback = null)
	{
		DebugLog(string.Format("Remove({0})", userId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/remove";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_id = userId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eFriendRemoveError error = NetworkDefs.ParseHTTPError<eFriendRemoveError>(request, response);
			if (error == eFriendRemoveError.NONE)
			{
				RefreshData(delegate(eFriendRefreshDataError refreshError)
				{
					if (refreshError == eFriendRefreshDataError.NONE)
					{
						if (callback != null)
						{
							callback(error);
						}
					}
					else
					{
						App.net.session.Disconnect(eNetSessionDisconnectType.REFRESH_FRIEND_LIST_FAILED);
					}
				});
			}
			else if (callback != null)
			{
				callback(error);
			}
		});
	}

	public void RequestMorty(string mortyId, Action<eFriendRequestMortyError> callback = null)
	{
		DebugLog(string.Format("RequestMorty(Morty Id: {0})", mortyId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/request-morty";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			morty_id = mortyId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eFriendRequestMortyError eFriendRequestMortyError2 = NetworkDefs.ParseHTTPError<eFriendRequestMortyError>(request, response);
			if (eFriendRequestMortyError2 == eFriendRequestMortyError.NONE)
			{
				App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.FRIENDS, "friend_request_morty");
			}
			if (callback != null)
			{
				callback(eFriendRequestMortyError2);
			}
		});
	}

	public void DonateMorty(string userId, string mortyNetId, Action<eFriendDonateMortyError> callback = null)
	{
		DebugLog(string.Format("DonateMorty(Username: {0}, Morty Net Id: {1})", userId, mortyNetId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/friend/donate-morty";
		networkHTTPRequest.postData = new
		{
			session_id = App.net.session.sessionID,
			player_id = userId,
			owned_morty_id = mortyNetId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			eFriendDonateMortyError error = NetworkDefs.ParseHTTPError<eFriendDonateMortyError>(request, response);
			if (error == eFriendDonateMortyError.NONE)
			{
				App.net.player.GetDetails(ePlayerDataParams.DONATED_MORTY, delegate(eGetPlayerDataError getDetailsError)
				{
					if (getDetailsError == eGetPlayerDataError.NONE)
					{
						App.upsight.CustomAnalyticEvent(eSubType1.PLAY, eSubType2.FRIENDS, "friend_donated_morty");
						if (callback != null)
						{
							callback(error);
						}
					}
					else
					{
						App.net.session.Disconnect(eNetSessionDisconnectType.PLAYER_GET_DETAILS_FAILED);
					}
				});
			}
			else if (callback != null)
			{
				callback(error);
			}
		});
	}

	public bool IsFriendListFull()
	{
		return friends.Count >= friendListLimit;
	}

	public bool IsFriendUserAlreadyAFriend(string userId)
	{
		MPFriendData mPFriendData = friends.FirstOrDefault((MPFriendData e) => e.userId == userId);
		return mPFriendData != null;
	}

	private void DonationMortyReceivedHandler(NetworkEventSourceResponse response)
	{
		DonationReceivedResponse donationReceivedResponse = JsonReader.Deserialize<DonationReceivedResponse>(response.jsonStr);
		App.net.player.data.ApplyMortyRewardData(donationReceivedResponse.owned_morty, false);
		RefreshData();
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_FRIEND_API)
		{
		}
	}
}
