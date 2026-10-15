using System;
using BestHTTP;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
	public NetworkSessionAPI session { get; private set; }

	public NetworkPlayerAPI player { get; private set; }

	public NetworkFriendAPI friend { get; private set; }

	public NetworkRoomAPI room { get; private set; }

	public NetworkBattleAPI battle { get; private set; }

	public NetworkBattleInviteAPI battleInvite { get; private set; }

	public NetworkHTTP http { get; private set; }

	private void Awake()
	{
		SetupProxy();
		http = base.gameObject.AddComponent<NetworkHTTP>();
		session = base.gameObject.AddComponent<NetworkSessionAPI>();
		player = base.gameObject.AddComponent<NetworkPlayerAPI>();
		friend = base.gameObject.AddComponent<NetworkFriendAPI>();
		room = base.gameObject.AddComponent<NetworkRoomAPI>();
		battle = base.gameObject.AddComponent<NetworkBattleAPI>();
		battleInvite = base.gameObject.AddComponent<NetworkBattleInviteAPI>();
		AddEvents(true);
	}

	private void SetupProxy()
	{
		string proxyName = BPSSystemProxy.GetProxyName();
		int proxyPort = BPSSystemProxy.GetProxyPort();
		string text = string.Format("http://{0}:{1}", proxyName, proxyPort);
		DebugLog("SetupProxy() uriStr: " + text);
		if (!string.IsNullOrEmpty(proxyName))
		{
			HTTPManager.Proxy = new HTTPProxy(new Uri(text));
			DebugLog("SetupProxy() setting proxy to: " + text);
		}
	}

	private void OnDestroy()
	{
		AddEvents(false);
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			session.DisconnectedEvent += NetSessionDisconnectedHandler;
			session.ConnectedEvent += NetSessionConnectedHandler;
		}
		else
		{
			session.DisconnectedEvent -= NetSessionDisconnectedHandler;
			session.ConnectedEvent -= NetSessionConnectedHandler;
		}
	}

	private void NetSessionConnectedHandler()
	{
		friend.Connected();
		player.Connected();
		battleInvite.Connected();
	}

	private void NetSessionDisconnectedHandler(eNetSessionDisconnectType clientDisconnectType)
	{
		http.Disconnected();
		player.Disconnected();
		friend.Disconnected();
		room.Disconnected();
		battle.Disconnected();
		battleInvite.Disconnected();
	}

	protected void DebugLog(string str)
	{
		if (NetworkTestDefs.DEBUG_CONTROLLER)
		{
		}
	}
}
