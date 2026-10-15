using FyberPlugin;
using UnityEngine;

public class MPVideoRewardMeeseekState : StateBase
{
	private MPVideoRewardMeeseekUI _videoAdUI;

	private float _timeOutTimer;

	private bool _rewardReceived;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPVideoRewardMeeseek/{0}", "MPVideoRewardMeeseek");
	}

	public override void Activate(Transform uiContainerTrans, TransInfo transInInfo)
	{
		base.Activate(uiContainerTrans, transInInfo);
		StartCoroutine(ActivateComplete());
	}

	public override void Deactivate(TransInfo transOutInfo)
	{
		_timeOutTimer = 0f;
		_rewardReceived = false;
		MPSharedStateInfo.videoRewardMeeseekInfo = null;
		base.Deactivate(transOutInfo);
	}

	protected override void CreateUI(Transform containerTrans)
	{
		base.CreateUI(containerTrans);
		if (_ui == null){ UnityEngine.Debug.LogWarning("[State] no UI prefab for " + GetType().Name); return; }
		_videoAdUI = _ui as MPVideoRewardMeeseekUI;
		_videoAdUI.SetUp(MPSharedStateInfo.videoRewardMeeseekInfo.data.Id);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_videoAdUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		_videoAdUI.FinishedEvent += UIFinishedHandler;
		_videoAdUI.WatchEvent += UIWatchHandler;
		FyberCallback.AdAvailable += OnAdAvailable;
		FyberCallback.AdNotAvailable += OnAdNotAvailable;
		FyberCallback.RequestFail += OnRequestFail;
		FyberCallback.AdStarted += OnAdStarted;
		FyberCallback.AdFinished += OnAdFinished;
		App.net.player.VideoRewardMeeseekReceivedEvent += NetPlayerVideoRewardMeeseekReceivedHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		_videoAdUI.FinishedEvent -= UIFinishedHandler;
		_videoAdUI.WatchEvent -= UIWatchHandler;
		FyberCallback.AdAvailable -= OnAdAvailable;
		FyberCallback.AdNotAvailable -= OnAdNotAvailable;
		FyberCallback.RequestFail -= OnRequestFail;
		FyberCallback.AdStarted -= OnAdStarted;
		FyberCallback.AdFinished -= OnAdFinished;
		App.net.player.VideoRewardMeeseekReceivedEvent -= NetPlayerVideoRewardMeeseekReceivedHandler;
	}

	private void Update()
	{
		if (_timeOutTimer > 0f)
		{
			_timeOutTimer -= Time.deltaTime;
			if (_timeOutTimer <= 0f)
			{
				_timeOutTimer = 0f;
				_videoAdUI.SetFailedRewardState();
			}
		}
	}

	private void GoToNextState()
	{
		if (_rewardReceived && MPSharedStateInfo.videoRewardMeeseekInfo.data.CanRewardsBeDisplayed())
		{
			MPSharedStateInfo.rewardInfo = new MPRewardInfo(MPSharedStateInfo.videoRewardMeeseekInfo.origin | eMPStateOrigin.VIDEO_REWARDS, MPSharedStateInfo.videoRewardMeeseekInfo.data.rewards, "video_rewards");
			App.states.Set<MPRewardState>(new FadeTransInfo
			{
				duration = 0.05f,
				color = Color.white
			}, new FadeTransInfo
			{
				duration = 0.2f,
				color = Color.white
			});
		}
		else if (MPSharedStateInfo.videoRewardMeeseekInfo.IfOrigin(eMPStateOrigin.BATTLE))
		{
			App.states.Get<InterstitialState>().nextStateType = typeof(MPWorldState);
			App.states.Get<InterstitialState>().placementID = eUpsightPlacementID.BATTLE_OVER;
			App.states.Set<InterstitialState>(new FadeTransInfo(), null, false, eStateAction.DEACTIVATE_ALL);
		}
		else
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void UIFinishedHandler(bool isSuccess, bool isDismissed)
	{
		if (isDismissed)
		{
			App.net.player.PassVideoReward(MPSharedStateInfo.videoRewardMeeseekInfo.data.Id);
		}
		if (isSuccess && MPSharedStateInfo.videoRewardMeeseekInfo.data.Id == "NPCAd")
		{
			App.net.room.videoRewardMeeseeksData = null;
		}
		GoToNextState();
	}

	private void UIWatchHandler()
	{
		_videoAdUI.SetWaitingVideoState();
		RewardedVideoRequester.Create().AddParameter("pub0", "87610").AddParameter("pub1", NetworkDefs.gameEndPoint)
			.AddParameter("pub2", App.net.player.data.netId)
			.AddParameter("pub3", MPSharedStateInfo.videoRewardMeeseekInfo.data.Id)
			.AddParameter("pub4", MPSharedStateInfo.videoRewardMeeseekInfo.data.token)
			.WithPlacementId(MPSharedStateInfo.videoRewardMeeseekInfo.data.GetMeeseekPlacementId())
			.NotifyUserOnCompletion(false)
			.Request();
	}

	private void OnAdAvailable(Ad ad)
	{
		AdFormat adFormat = ad.AdFormat;
		if (adFormat == AdFormat.REWARDED_VIDEO)
		{
			App.net.session.SendPingNow("PRE_AD");
			App.upsight.CustomAnalyticEvent(eSubType1.ECONOMY, eSubType2.SOURCE, string.Format("watch_video_ad_{0}", MPSharedStateInfo.videoRewardMeeseekInfo.data.GetMeeseekPlacementId()));
			ad.Start();
		}
	}

	private void OnAdNotAvailable(AdFormat adFormat)
	{
		_videoAdUI.SetNoVideoState();
	}

	private void OnRequestFail(RequestError error)
	{
		_videoAdUI.SetNoVideoState();
	}

	private void OnAdStarted(Ad ad)
	{
		App.sound.musicVolume = 0f;
	}

	private void OnAdFinished(AdResult result)
	{
		App.sound.musicVolume = App.saveData.settings.musicVolume;
		if (result.AdFormat != AdFormat.REWARDED_VIDEO)
		{
			return;
		}
		string message = result.Message;
		if (message != null && message == "CLOSE_FINISHED")
		{
			if (!_rewardReceived)
			{
				_videoAdUI.SetWaitingRewardState();
				_timeOutTimer = 15f;
			}
			else
			{
				_videoAdUI.SetRewardReceivedState();
			}
		}
		else
		{
			_videoAdUI.SetFailedVideoState();
		}
	}

	public void NetPlayerVideoRewardMeeseekReceivedHandler(string id, MPRewardData[] rewardDatas)
	{
		if (!(MPSharedStateInfo.videoRewardMeeseekInfo.data.Id != id))
		{
			_rewardReceived = true;
			MPSharedStateInfo.videoRewardMeeseekInfo.data.rewards = rewardDatas;
			if (_timeOutTimer > 0f)
			{
				_timeOutTimer = 0f;
				_videoAdUI.SetRewardReceivedState();
			}
		}
	}
}
