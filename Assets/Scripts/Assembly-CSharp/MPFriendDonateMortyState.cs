using UnityEngine;

public class MPFriendDonateMortyState : StateBase
{
	private MPFriendDonateMortyUI _friendDonateMortyUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPFriendDonateMorty/{0}", "MPFriendDonateMorty");
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
		_friendDonateMortyUI = _ui as MPFriendDonateMortyUI;
		MPFriendData friendData = MPSharedStateInfo.friendDonateMortyInfo.friendData;
		_friendDonateMortyUI.SetUp(friendData, App.net.player.data.GetAllMortyDatasOfId(friendData.requestingMortyId));
	}

	protected override void Destroy()
	{
		base.Destroy();
		_friendDonateMortyUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_friendDonateMortyUI.DonateEvent += UIDonateHandler;
		_friendDonateMortyUI.BackEvent += UIBackHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_friendDonateMortyUI.DonateEvent -= UIDonateHandler;
		_friendDonateMortyUI.BackEvent -= UIBackHandler;
	}

	private void UIDonateHandler(MPOpponentMortyData mortyData)
	{
		ActivateInput(false);
		App.net.friend.DonateMorty(MPSharedStateInfo.friendDonateMortyInfo.friendData.userId, mortyData.netId, delegate(eFriendDonateMortyError error)
		{
			if (error == eFriendDonateMortyError.NONE)
			{
				MPSharedStateInfo.friendDonateMortyInfo = null;
				App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				_friendDonateMortyUI.DisplayError(error);
			}
			ActivateInput(true);
		});
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}
}
