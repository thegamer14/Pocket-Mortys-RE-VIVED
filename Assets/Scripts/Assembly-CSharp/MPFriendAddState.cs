using UnityEngine;

public class MPFriendAddState : StateBase
{
	private MPFriendAddUI _friendAddUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPFriendAdd/{0}", "MPFriendAdd");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_friendAddUI = _ui as MPFriendAddUI;
		_friendAddUI.SetUp();
	}

	protected override void Destroy()
	{
		base.Destroy();
		_friendAddUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_friendAddUI.SearchForUserEvent += UISearchforUserHandler;
		_friendAddUI.SendFriendRequestEvent += UISendFriendRequestHandler;
		_friendAddUI.InviteEvent += UIInviteHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_friendAddUI.SearchForUserEvent -= UISearchforUserHandler;
		_friendAddUI.SendFriendRequestEvent -= UISendFriendRequestHandler;
		_friendAddUI.InviteEvent -= UIInviteHandler;
	}

	private void UISearchforUserHandler(string username)
	{
		ActivateInput(false);
		App.net.friend.SearchForUser(username, delegate(MPFriendData friendData, eFriendSearchForUserError searchError)
		{
			_friendAddUI.SearchForUserComplete(friendData, searchError);
			ActivateInput(true);
		});
	}

	private void UISendFriendRequestHandler(string userId)
	{
		if (!App.net.friend.IsFriendListFull())
		{
			ActivateInput(false);
			App.net.friend.SendRequest(userId, delegate(eFriendSendRequestError sendError)
			{
				_friendAddUI.SendFriendRequestComplete(sendError);
				ActivateInput(true);
			});
		}
		else
		{
			_friendAddUI.SendFriendRequestComplete(eFriendSendRequestError.FRIEND_LIMIT_SELF);
		}
	}

	private void UIInviteHandler()
	{
		App.social.Share(string.Format(MPTextDefs.MP_FRIEND_ADD_INVITE_SHARE, App.net.player.data.username, "http://hyperurl.co/zrn5s4"));
	}
}
