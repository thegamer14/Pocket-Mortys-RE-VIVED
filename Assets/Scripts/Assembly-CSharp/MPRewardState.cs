using UnityEngine;

public class MPRewardState : StateBase
{
	private MPRewardUI _rewardUI;

	private int _rewardInfoIndex;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPReward/{0}", "MPReward");
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
		_rewardUI = _ui as MPRewardUI;
		MPRewardData rewardData = MPSharedStateInfo.rewardInfo.GetRewardDatas()[_rewardInfoIndex];
		_rewardUI.SetUp(rewardData, MPSharedStateInfo.rewardInfo.confirmation, MPSharedStateInfo.rewardInfo.fromPlayerLevelState);
		StartCoroutine(_rewardUI.ConfirmReward(rewardData, MPSharedStateInfo.rewardInfo.confirmation, MPSharedStateInfo.rewardInfo.analyticId));
	}

	protected override void Destroy()
	{
		base.Destroy();
		_rewardUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_rewardUI.TweetEvent += UITweetHandler;
		_rewardUI.ContinueEvent += UIContinueHandler;
		App.net.room.PickupCollectEvent += NetRoomPickupCollectHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_rewardUI.TweetEvent -= UITweetHandler;
		_rewardUI.ContinueEvent -= UIContinueHandler;
		App.net.room.PickupCollectEvent -= NetRoomPickupCollectHandler;
	}

	private void UITweetHandler()
	{
		MPRewardData mPRewardData = MPSharedStateInfo.rewardInfo.GetRewardDatas()[_rewardInfoIndex];
		string text = null;
		switch (mPRewardData.type)
		{
		default:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_DEF, "http://hyperurl.co/zrn5s4");
			break;
		case eMPRewardType.COIN:
			text = string.Format(MPTextDefs.MP_SOCIAL_TWEET_REWARD_COIN, mPRewardData.amount, "http://hyperurl.co/zrn5s4");
			break;
		case eMPRewardType.COUPON:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_ITEM, MPItemDefs.Name(mPRewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		case eMPRewardType.MORTY:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_MORTY, MPMortyDefs.Name(mPRewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		case eMPRewardType.ITEM:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_ITEM, MPItemDefs.Name(mPRewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		}
		App.social.Share(text, true);
	}

	private void UIContinueHandler()
	{
		if (_rewardInfoIndex < MPSharedStateInfo.rewardInfo.GetRewardDatas().Length - 1 && MPSharedStateInfo.rewardInfo.confirmation == eMPRewardConfirmation.SUCCESS)
		{
			_rewardInfoIndex++;
			App.states.Set<MPRewardState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			});
			return;
		}
		_rewardInfoIndex = 0;
		if (MPSharedStateInfo.rewardInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			if (!MPSharedStateInfo.rewardInfo.IfOrigin(eMPStateOrigin.VIDEO_REWARDS))
			{
				if (MPSharedStateInfo.rewardInfo.battlePlayerData.IsPlayerLevelStateRequired())
				{
					MPSharedStateInfo.playerLevelInfo = new MPPlayerLevelInfo(MPSharedStateInfo.rewardInfo.origin | eMPStateOrigin.REWARD, MPSharedStateInfo.worldInfo.worldId, MPSharedStateInfo.rewardInfo.battlePlayerData);
					MPSharedStateInfo.rewardInfo = null;
					App.states.Set<MPPlayerLevelState>(new FadeTransInfo
					{
						duration = 0.05f,
						color = Color.white
					}, new FadeTransInfo
					{
						duration = 0.2f,
						color = Color.white
					});
					return;
				}
				if (MPSharedStateInfo.rewardInfo.battlePlayerData.IsVideoRewardStateRequired())
				{
					MPSharedStateInfo.videoRewardMeeseekInfo = new MPVideoRewardMeeseekInfo(MPSharedStateInfo.rewardInfo.origin | eMPStateOrigin.REWARD, MPSharedStateInfo.rewardInfo.battlePlayerData.videoRewardMeeseeksData);
					MPSharedStateInfo.rewardInfo = null;
					App.states.Set<MPVideoRewardMeeseekState>(new FadeTransInfo(), new FadeTransInfo());
					return;
				}
			}
			MPSharedStateInfo.rewardInfo = null;
			App.states.Get<InterstitialState>().nextStateType = typeof(MPWorldState);
			App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
			App.states.Set<InterstitialState>(new FadeTransInfo(), null, false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			MPSharedStateInfo.rewardInfo = null;
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void NetRoomPickupCollectHandler(MPPickupData pickupData, eRequestPickupCollectError error)
	{
		StartCoroutine(_rewardUI.ConfirmReward(MPSharedStateInfo.rewardInfo.GetRewardDatas()[_rewardInfoIndex], MPSharedStateInfo.rewardInfo.confirmation, MPSharedStateInfo.rewardInfo.analyticId));
	}
}
