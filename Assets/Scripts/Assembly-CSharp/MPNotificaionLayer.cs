using System.Collections.Generic;
using UnityEngine;

public class MPNotificaionLayer : MonoBehaviour
{
	private static MPNotificaionLayer _instance;

	private NetworkErrorIcon _netErrorIcon;

	private NetworkWaitingIcon _netWaitingIcon;

	private NetworkFriendBattleIcon _netFriendBattleIcon;

	public static void Instantiate()
	{
		if (!(_instance != null))
		{
			_instance = UI.Instantiate("UI/MPNotifications/Notifications", App.states.mainCanvas.transform).GetComponent<MPNotificaionLayer>();
		}
	}

	public static void Destroy()
	{
		if (!(_instance == null))
		{
			_instance.DestroySelf();
		}
	}

	private void Awake()
	{
		_netErrorIcon = base.transform.Find("NetworkErrorIcon").GetComponent<NetworkErrorIcon>();
		_netWaitingIcon = base.transform.Find("NetworkWaitingIcon").GetComponent<NetworkWaitingIcon>();
		_netFriendBattleIcon = base.transform.Find("NetworkFriendBattleIcon").GetComponent<NetworkFriendBattleIcon>();
	}

	private void Start()
	{
		_netErrorIcon.Hide();
		_netWaitingIcon.Hide();
		_netFriendBattleIcon.Hide();
	}

	private void OnEnable()
	{
		AddEvents();
	}

	private void OnDisable()
	{
		RemoveEvents();
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		_instance = null;
	}

	private void AddEvents()
	{
		App.net.http.RequestErrorEvent += HTTPRequestErrorHandler;
		App.net.http.RequestDelayedStateChangedEvent += RequestDelayedStateChangedHandler;
		App.net.battleInvite.InviteQueueChangeEvent += NetBattleInviteQueueChangeHandler;
	}

	private void RemoveEvents()
	{
		App.net.http.RequestErrorEvent -= HTTPRequestErrorHandler;
		App.net.http.RequestDelayedStateChangedEvent -= RequestDelayedStateChangedHandler;
		App.net.battleInvite.InviteQueueChangeEvent -= NetBattleInviteQueueChangeHandler;
	}

	private void Update()
	{
		base.transform.SetAsLastSibling();
	}

	private void HTTPRequestErrorHandler()
	{
		_netErrorIcon.ShowWithAnim();
	}

	private void RequestDelayedStateChangedHandler(bool isDelayed)
	{
		if (isDelayed)
		{
			_netWaitingIcon.ShowWithAnim();
		}
		else
		{
			_netWaitingIcon.HideWithAnim();
		}
	}

	private void NetBattleInviteQueueChangeHandler(List<MPBattleInviteData> invites, bool friendInvitePending)
	{
		if (friendInvitePending)
		{
			_netFriendBattleIcon.ShowWithAnim();
		}
		else
		{
			_netFriendBattleIcon.HideWithAnim();
		}
	}
}
