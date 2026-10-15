using UnityEngine;

public class MPFriendRequestMortyState : StateBase
{
	private MPFriendRequestMortyUI _friendRequestMortyUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPFriendRequestMorty/{0}", "MPFriendRequestMorty");
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
		_friendRequestMortyUI = _ui as MPFriendRequestMortyUI;
		_friendRequestMortyUI.SetUp(App.net.player.data.mortydexSeen);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_friendRequestMortyUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_friendRequestMortyUI.RequestEvent += UIRequestHandler;
		_friendRequestMortyUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_friendRequestMortyUI.RequestEvent -= UIRequestHandler;
		_friendRequestMortyUI.BackEvent -= UIBackHandler;
	}

	private void UIRequestHandler(string mortyId)
	{
		ActivateInput(false);
		App.net.friend.RequestMorty(mortyId, delegate(eFriendRequestMortyError error)
		{
			if (error == eFriendRequestMortyError.NONE)
			{
				App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				_friendRequestMortyUI.DisplayRequestMortyError(error);
				ActivateInput(true);
			}
		});
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
