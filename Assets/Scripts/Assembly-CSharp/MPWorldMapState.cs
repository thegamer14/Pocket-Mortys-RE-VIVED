using System;
using UnityEngine;

public class MPWorldMapState : StateBase
{
	private MPWorldMapUI _worldMapUI;

	public string _startingDialogueStr;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPWorldMap/{0}", "MPWorldMap");
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
		_worldMapUI = _ui as MPWorldMapUI;
		_worldMapUI.SetUp(MPSharedStateInfo.worldMapInfo.worldId, App.net.room.users);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_worldMapUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		MPWorldMapUI worldMapUI = _worldMapUI;
		worldMapUI.BackEvent = (MPWorldMapUI.UIDelegate)Delegate.Combine(worldMapUI.BackEvent, new MPWorldMapUI.UIDelegate(UIBackHandler));
		App.net.room.nodeReceiver.NodePlacementReceivedEvent += NetNodePlacementReceivedHandler;
		App.net.room.UserAddedEvent += NetRoomUserAddedHandler;
		App.net.room.UserRemovedEvent += NetRoomUserRemovedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MPWorldMapUI worldMapUI = _worldMapUI;
		worldMapUI.BackEvent = (MPWorldMapUI.UIDelegate)Delegate.Remove(worldMapUI.BackEvent, new MPWorldMapUI.UIDelegate(UIBackHandler));
		App.net.room.nodeReceiver.NodePlacementReceivedEvent -= NetNodePlacementReceivedHandler;
		App.net.room.UserAddedEvent -= NetRoomUserAddedHandler;
		App.net.room.UserRemovedEvent -= NetRoomUserRemovedHandler;
	}

	private void UIBackHandler()
	{
		App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
	}

	private void NetNodePlacementReceivedHandler(string netId, MPNodePlacementData placementData, bool fromNewNode)
	{
		_worldMapUI.SetUserPlacementData(netId, placementData);
	}

	private void NetRoomUserAddedHandler(MPUserData userData)
	{
		_worldMapUI.AddUser(userData);
	}

	private void NetRoomUserRemovedHandler(MPUserData userData)
	{
		_worldMapUI.RemoveUser(userData);
	}
}
