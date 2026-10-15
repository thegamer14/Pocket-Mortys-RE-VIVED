using System;
using System.Collections;
using JsonFx.Json;
using UnityEngine;

public class NetworkSessionAPI : MonoBehaviour
{
	public delegate void SessionConnectedDelegate();

	public delegate void SessionDisconnectedDelegate(eNetSessionDisconnectType clientDisconnectType);

	public delegate void SessionEmoteDelegate(string emoteId, string playerId);

	private const float _kESStartHandlerTimeout = 15f;

	public const string kKeepAliveTypeDefaultId = "DEFAULT";

	public const string kKeepAliveTypePreAdId = "PRE_AD";

	public string sessionID;

	private NetworkEventSource _eventSource;

	private string _pingUrl;

	private float _pingInterval;

	private bool _waitingForSessionStart;

	private Action<eNetSessionConnectError> _connectCallback;

	private eNetSessionDisconnectType _clientDisconnectType;

	public bool isConnected
	{
		get
		{
			return _eventSource != null && !_eventSource.IsClosed();
		}
	}

	public event SessionConnectedDelegate ConnectedEvent;

	public event SessionDisconnectedDelegate DisconnectedEvent;

	public event SessionEmoteDelegate EmoteEvent;

	private void Awake()
	{
		_eventSource = base.gameObject.AddComponent<NetworkEventSource>();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus && isConnected)
		{
			StopPingCoroutine();
			StartCoroutine(SendRepeatingPingCo(_pingUrl, _pingInterval, true));
		}
	}

	private void AddEventSourceEvents()
	{
		_eventSource.KeepaliveTimeoutEvent += ESKeepaliveTimeoutHandler;
		_eventSource.OpenedEvent += ESOpenedHandler;
		_eventSource.ClosedEvent += ESClosedHandler;
		_eventSource.SourceError += ESErrorHandler;
		_eventSource.On("session:start", ESStartHandler);
		_eventSource.On("session:end", ESEndedHandler);
		_eventSource.On("emote:p2p", ESEmoteDirectHandler);
		_eventSource.On("emote:room", ESEmoteHandler);
	}

	private void RemoveEventSourceEvents()
	{
		_eventSource.KeepaliveTimeoutEvent -= ESKeepaliveTimeoutHandler;
		_eventSource.OpenedEvent -= ESOpenedHandler;
		_eventSource.ClosedEvent -= ESClosedHandler;
		_eventSource.SourceError -= ESErrorHandler;
		_eventSource.Off("session:start", ESStartHandler);
		_eventSource.Off("session:end", ESEndedHandler);
		_eventSource.Off("emote:p2p", ESEmoteDirectHandler);
		_eventSource.Off("emote:room", ESEmoteHandler);
	}

	public void GetServerMessage(Action<MPServerMessageData, eServerMessageError> callback = null)
	{
		DebugLog("GetServerMessage()");
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.endPoint = "http://static.bps-pmnet.com/init";
		networkHTTPRequest.path = string.Format("{0}.json", "applecrumble");
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			ServerMessageResponse responseObject = null;
			eServerMessageError eServerMessageError2 = NetworkDefs.ParseHTTPResponse<eServerMessageError, ServerMessageResponse>(request, response, out responseObject);
			MPServerMessageData arg = null;
			if (eServerMessageError2 == eServerMessageError.NONE && responseObject != null)
			{
				arg = new MPServerMessageData(responseObject.message);
			}
			callback(arg, eServerMessageError2);
		});
	}

	public void Connect(string secretId, Action<eNetSessionConnectError> callback = null)
	{
		DebugLog(string.Format("Connect(secretId: {0})", secretId));
		Disconnect(eNetSessionDisconnectType.PRE_CONNECT);
		_connectCallback = callback;
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/user/login";
		networkHTTPRequest.postData = new
		{
			secret = secretId
		};
		App.net.http.Request(networkHTTPRequest, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			LoginResponse responseObject = null;
			eNetSessionConnectError eNetSessionConnectError2 = NetworkDefs.ParseHTTPResponse<eNetSessionConnectError, LoginResponse>(request, response, out responseObject);
			if (eNetSessionConnectError2 == eNetSessionConnectError.NONE && responseObject != null)
			{
				StartCoroutine(ConnectToEventSourceCo(responseObject.session_url));
			}
			else if (callback != null)
			{
				DebugLog(string.Format("Connect() calling back with error: {0}", eNetSessionConnectError2));
				callback(eNetSessionConnectError2);
			}
		});
	}

	private IEnumerator ConnectToEventSourceCo(string url)
	{
		DebugLog("ConnectToEventSourceCo()");
		AddEventSourceEvents();
		_clientDisconnectType = eNetSessionDisconnectType.NONE;
		_waitingForSessionStart = true;
		_eventSource.Open(url);
		float timeout = 15f;
		while (_waitingForSessionStart && timeout > 0f)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				DebugLog("ConnectToEventSoureCo(): ESStartHandler timout!");
				Disconnect(eNetSessionDisconnectType.ES_START_TIMEOUT);
				_waitingForSessionStart = false;
				if (_connectCallback != null)
				{
					_connectCallback(eNetSessionConnectError.SESSION_START_TIMEOUT);
					_connectCallback = null;
				}
			}
			else
			{
				yield return null;
			}
		}
	}

	public void ESStartHandler(NetworkEventSourceResponse response)
	{
		DebugLog("ESStartHandler() response: " + response.jsonStr);
		_waitingForSessionStart = false;
		ESStartResponse eSStartResponse;
		try
		{
			eSStartResponse = JsonReader.Deserialize<ESStartResponse>(response.jsonStr);
		}
		catch (Exception ex)
		{
			DebugLog(string.Format("ESStartHandler() Failed to deserialize response. jsonStr: {0} Error: ", response.jsonStr, ex.Message));
			if (_connectCallback != null)
			{
				_connectCallback(eNetSessionConnectError.INVALID_ES_START_RESPONSE);
				_connectCallback = null;
			}
			return;
		}
		sessionID = eSStartResponse.session_id;
		_eventSource.SetDisconnectAfterTimeout(eSStartResponse.keep_alive * 3f);
		StartCoroutine(SendRepeatingPingCo(eSStartResponse.ping_url, eSStartResponse.ping_interval));
		App.net.player.GetDetails(ePlayerDataParams.ALL, delegate(eGetPlayerDataError getDetailsError)
		{
			if (getDetailsError == eGetPlayerDataError.NONE)
			{
				App.net.player.MigrateSPAvatars(delegate(eMigrateSPAvatarsError migrateError)
				{
					if (migrateError == eMigrateSPAvatarsError.NONE)
					{
						App.net.friend.RefreshData(delegate(eFriendRefreshDataError friendsError)
						{
							if (friendsError == eFriendRefreshDataError.NONE)
							{
								if (_connectCallback != null)
								{
									_connectCallback(eNetSessionConnectError.NONE);
									_connectCallback = null;
								}
							}
							else if (_connectCallback != null)
							{
								_connectCallback(eNetSessionConnectError.GET_FRIEND_DATA_ERROR);
								_connectCallback = null;
							}
						});
					}
					else if (_connectCallback != null)
					{
						_connectCallback(eNetSessionConnectError.MIGRATE_SP_AVATARS_ERROR);
						_connectCallback = null;
					}
				});
			}
			else if (_connectCallback != null)
			{
				_connectCallback(eNetSessionConnectError.GET_PLAYER_DATA_ERROR);
				_connectCallback = null;
			}
		});
	}

	public void Disconnect(eNetSessionDisconnectType disconnectType)
	{
		DebugLog(string.Format("Disconnect({0})", disconnectType));
		_clientDisconnectType = disconnectType;
		if (_eventSource != null)
		{
			_eventSource.Close();
		}
	}

	public void SendPingNow(string type = "DEFAULT")
	{
		StartCoroutine(SendPingCo(0f, _pingUrl, type));
	}

	private IEnumerator SendRepeatingPingCo(string url, float interval, bool skipInterval = false)
	{
		_pingUrl = url;
		_pingInterval = interval;
		float currentInterval = ((!skipInterval) ? interval : 0f);
		while (true)
		{
			yield return new WaitForSeconds(currentInterval);
			currentInterval = interval;
			StartCoroutine(SendPingCo(0f, url, "DEFAULT", delegate(bool success)
			{
				if (!success)
				{
					StartCoroutine(SendPingCo(currentInterval / 4f, url));
				}
			}));
		}
	}

	private IEnumerator SendPingCo(float delay, string url, string type = "DEFAULT", Action<bool> callback = null)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		NetworkHTTPRequest requestDesc = new NetworkHTTPRequest
		{
			endPoint = url,
			postData = new
			{
				session_id = sessionID,
				type = type
			}
		};
		App.net.http.Request(requestDesc, delegate(NetworkHTTPRequest request, NetworkHTTPResponse response)
		{
			if (callback != null)
			{
				callback(response != null && response.isSuccess);
			}
		});
	}

	public void StopPingCoroutine()
	{
		StopAllCoroutines();
	}

	public void EmoteToUser(string emote, string otherUserId)
	{
		DebugLog(string.Format("Emote(emote: {0}, otherUserId: {1})", emote, otherUserId));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/emote/p2p";
		networkHTTPRequest.postData = new
		{
			session_id = sessionID,
			player_id = otherUserId,
			emote = emote
		};
		App.net.http.Request(networkHTTPRequest);
	}

	public void EmoteToRoom(string emote)
	{
		DebugLog(string.Format("Emote(emote: {0})", emote));
		NetworkHTTPRequest networkHTTPRequest = new NetworkHTTPRequest();
		networkHTTPRequest.path = "/session/emote/room";
		networkHTTPRequest.postData = new
		{
			session_id = sessionID,
			emote = emote
		};
		App.net.http.Request(networkHTTPRequest);
	}

	public void On(string eventName, NetworkEventSource.EventResponseDelegate handler)
	{
		_eventSource.On(eventName, handler);
	}

	public void Off(string eventName, NetworkEventSource.EventResponseDelegate handler)
	{
		_eventSource.Off(eventName, handler);
	}

	private void ESKeepaliveTimeoutHandler()
	{
		DebugLog("ESKeepaliveTimeoutHandler()");
		Disconnect(eNetSessionDisconnectType.KEEPALIVE_TIMEOUT);
	}

	private void ESOpenedHandler()
	{
		DebugLog("ESOpenedHandler()");
		if (this.ConnectedEvent != null)
		{
			this.ConnectedEvent();
		}
	}

	private void ESClosedHandler()
	{
		DebugLog("ESClosedHandler()");
		RemoveEventSourceEvents();
		StopPingCoroutine();
		if (_clientDisconnectType == eNetSessionDisconnectType.NONE)
		{
			_clientDisconnectType = eNetSessionDisconnectType.ES_CLOSED;
		}
		if (this.DisconnectedEvent != null)
		{
			this.DisconnectedEvent(_clientDisconnectType);
		}
		if (_waitingForSessionStart)
		{
			_waitingForSessionStart = false;
			if (_connectCallback != null)
			{
				_connectCallback(eNetSessionConnectError.UNKNOWN);
				_connectCallback = null;
			}
		}
	}

	private void ESEndedHandler(NetworkEventSourceResponse response)
	{
		_clientDisconnectType = eNetSessionDisconnectType.SERVER_ENDED;
	}

	private void ESErrorHandler(string errorMessage = null)
	{
		DebugLog(string.Format("ESErrorHandler(): {0}", errorMessage));
		Disconnect(eNetSessionDisconnectType.ES_ERROR);
	}

	private void ESEmoteDirectHandler(NetworkEventSourceResponse response)
	{
		DebugLog("ESEmoteDirectHandler()");
		EmoteResponse emoteResponse = JsonReader.Deserialize<EmoteResponse>(response.jsonStr);
		if (this.EmoteEvent != null)
		{
			this.EmoteEvent(emoteResponse.emote, emoteResponse.player_id);
		}
	}

	private void ESEmoteHandler(NetworkEventSourceResponse response)
	{
		DebugLog("ESEmoteHandler()");
		EmoteResponse emoteResponse = JsonReader.Deserialize<EmoteResponse>(response.jsonStr);
		if (this.EmoteEvent != null)
		{
			this.EmoteEvent(emoteResponse.emote, emoteResponse.player_id);
		}
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_SESSION_API)
		{
		}
	}
}
