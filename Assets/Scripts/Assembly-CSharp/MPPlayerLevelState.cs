using System;
using UnityEngine;

public class MPPlayerLevelState : StateBase
{
	private MPPlayerLevelUI _playerLevelUI;

	public string _startingDialogueStr;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPPlayerLevel/{0}", "MPPlayerLevel");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		_playerLevelUI.TransitionIn();
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_playerLevelUI = _ui as MPPlayerLevelUI;
		_playerLevelUI.SetUp(MPSharedStateInfo.playerLevelInfo.GetPlayerXpData(), MPSharedStateInfo.playerLevelInfo.backgroundId, App.net.player.data.assetId, _startingDialogueStr);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_playerLevelUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		MPPlayerLevelUI playerLevelUI = _playerLevelUI;
		playerLevelUI.RewardEvent = (MPPlayerLevelUI.UIRewardDelegate)Delegate.Combine(playerLevelUI.RewardEvent, new MPPlayerLevelUI.UIRewardDelegate(UIRewardHandler));
		MPPlayerLevelUI playerLevelUI2 = _playerLevelUI;
		playerLevelUI2.FinishedEvent = (MPPlayerLevelUI.UIDelegate)Delegate.Combine(playerLevelUI2.FinishedEvent, new MPPlayerLevelUI.UIDelegate(UIFinishedEventHandler));
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MPPlayerLevelUI playerLevelUI = _playerLevelUI;
		playerLevelUI.RewardEvent = (MPPlayerLevelUI.UIRewardDelegate)Delegate.Remove(playerLevelUI.RewardEvent, new MPPlayerLevelUI.UIRewardDelegate(UIRewardHandler));
		MPPlayerLevelUI playerLevelUI2 = _playerLevelUI;
		playerLevelUI2.FinishedEvent = (MPPlayerLevelUI.UIDelegate)Delegate.Remove(playerLevelUI2.FinishedEvent, new MPPlayerLevelUI.UIDelegate(UIFinishedEventHandler));
	}

	private void UIRewardHandler(MPRewardData[] rewardDatas)
	{
		MPSharedStateInfo.rewardInfo = new MPRewardInfo(eMPStateOrigin.PLAYER_LEVEL, rewardDatas, "player_level");
		MPSharedStateInfo.rewardInfo.fromPlayerLevelState = true;
		App.states.Set<MPRewardState>(new FadeTransInfo
		{
			duration = 0.05f,
			color = Color.white
		}, new FadeTransInfo
		{
			duration = 0.2f,
			color = Color.white
		}, true, eStateAction.ADD_CURRENT_TO_CACHE);
	}

	private void UIFinishedEventHandler()
	{
		if (MPSharedStateInfo.playerLevelInfo.battlePlayerData.IsVideoRewardStateRequired())
		{
			MPSharedStateInfo.videoRewardMeeseekInfo = new MPVideoRewardMeeseekInfo(MPSharedStateInfo.playerLevelInfo.origin | eMPStateOrigin.PLAYER_LEVEL, MPSharedStateInfo.playerLevelInfo.battlePlayerData.videoRewardMeeseeksData);
			MPSharedStateInfo.playerLevelInfo = null;
			App.states.Set<MPVideoRewardMeeseekState>(new FadeTransInfo(), new FadeTransInfo());
		}
		else
		{
			MPSharedStateInfo.playerLevelInfo = null;
			App.states.Get<InterstitialState>().nextStateType = typeof(MPWorldState);
			App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
			App.states.Set<InterstitialState>(new FadeTransInfo(), null, false, eStateAction.DEACTIVATE_ALL);
		}
	}
}
