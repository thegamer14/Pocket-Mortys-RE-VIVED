using UnityEngine;

public class MPWorldSelectState : StateBase
{
	private MPWorldSelectUI _worldSelectUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPWorldSelect/{0}", "MPWorldSelect");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		App.sound.PlaySoundtrack("Audio/Soundtrack/World/RaM202m06.Roy.01033907.Ver1.0");
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		base.Deactivate(transOutInfo);
		App.sound.StopSoundtrack((transOutInfo != null) ? transOutInfo.duration : 0f);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_worldSelectUI = _ui as MPWorldSelectUI;
		_worldSelectUI.SetUp(App.saveData.multiplayer.lastWorldId, App.net.player.data.level);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_worldSelectUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_worldSelectUI.WorldSelectedEvent += UIWorldSelectedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_worldSelectUI.WorldSelectedEvent -= UIWorldSelectedHandler;
	}

	private void UIWorldSelectedHandler(string selectedWorldId)
	{
		_worldSelectUI.SetStateJoining();
		JoinRoom(selectedWorldId);
	}

	private void JoinRoom(string selectedWorldId)
	{
		App.net.room.Join(selectedWorldId, delegate(eJoinError error, string worldId)
		{
			if (error == eJoinError.NONE)
			{
				App.net.player.data.placementData.walkIn = true;
				App.redirectOnMPSessionDisconnect = true;
				MPSharedStateInfo.worldInfo = new MPWorldInfo(worldId);
				App.states.Set<MPWorldState>(new FadeTransInfo(), new FadeTransInfo());
			}
			else
			{
				_worldSelectUI.SetStateError(string.Format(MPTextDefs.MP_REGISTER_JOIN_ROOM_ERROR, error));
			}
		});
	}
}
