using UnityEngine;

public class RewardState : StateBase
{
	[HideInInspector]
	public bool goToInterstitialWhenFinished;

	private RewardUI _rewardUI;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/Reward/{0}", "Reward");
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
		_rewardUI = _ui as RewardUI;
		RewardData rewardData = SharedStatInfo.rewardInfo.rewards[0];
		string rewardOutcomeText = SharedStatInfo.rewardInfo.GetRewardOutcomeText(rewardData);
		SharedStatInfo.rewardInfo.RemoveReward(0);
		_rewardUI.SetUp(rewardData, rewardOutcomeText);
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
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_rewardUI.TweetEvent -= UITweetHandler;
		_rewardUI.ContinueEvent -= UIContinueHandler;
	}

	private void UITweetHandler(RewardData rewardData)
	{
		string text = null;
		switch (rewardData.type)
		{
		default:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_DEF, "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.BADGE:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_BADGE, "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.COUNCIL_BADGE:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_COUNCIL_BADGE, "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.PORTAL_GUN:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_PORTAL_GUN, "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.COIN:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_COIN, rewardData.value, "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.MORTY:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_MORTY, MortyDefs.Name(rewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.ITEM:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_ITEM, ItemDefs.Name(rewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.TROPHY:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_TROPHY, TournamentDefs.TournamentName(rewardData.id), "http://hyperurl.co/zrn5s4");
			break;
		case eRewardType.TOURNAMENT_TICKET:
			text = string.Format(TextDefs.SOCIAL_TWEET_REWARD_TOURNAMENT_TICKET, "http://hyperurl.co/zrn5s4");
			break;
		}
		App.social.Share(text, true);
	}

	private void UIContinueHandler()
	{
		if (SharedStatInfo.rewardInfo.rewards.Count > 0)
		{
			App.states.Set<RewardState>(new FadeTransInfo
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
		SharedStatInfo.rewardInfo = null;
		if (goToInterstitialWhenFinished)
		{
			goToInterstitialWhenFinished = false;
			App.states.Get<InterstitialState>().nextStateIsBack = true;
			App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
			App.states.Set<InterstitialState>(new FadeTransInfo());
		}
		else
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}
}
