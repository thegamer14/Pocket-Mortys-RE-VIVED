using System;
using System.Collections;
using FyberPlugin;
using UnityEngine;

public class MPVideoRewardState : StateBase
{
	private MPVideoRewardUI _videoRewardUI;

	private bool _playingVideo;

	private bool _rewardReceived;

	private Coroutine _rewardTimeoutCo;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/MPVideoReward/{0}", "MPVideoRewardMenu");
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
		_videoRewardUI = _ui as MPVideoRewardUI;
		_videoRewardUI.SetState(eVideoRewardUIState.WAITING_FOR_VIDEO);
		_videoRewardUI.SetCoins(App.net.player.data.coins);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_videoRewardUI = null;
		_rewardTimeoutCo = null;
		_playingVideo = false;
		_rewardReceived = false;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		MPVideoRewardUI videoRewardUI = _videoRewardUI;
		videoRewardUI.QuitBtnEvent = (MPVideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Combine(videoRewardUI.QuitBtnEvent, new MPVideoRewardUI.UIVideoRewardBtnDelegate(QuitBtnHandler));
		MPVideoRewardUI videoRewardUI2 = _videoRewardUI;
		videoRewardUI2.WatchBtnEvent = (MPVideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Combine(videoRewardUI2.WatchBtnEvent, new MPVideoRewardUI.UIVideoRewardBtnDelegate(WatchBtnHandler));
		FyberCallback.AdAvailable += OnAdAvailable;
		FyberCallback.AdNotAvailable += OnAdNotAvailable;
		FyberCallback.RequestFail += OnRequestFail;
		FyberCallback.AdStarted += OnAdStarted;
		FyberCallback.AdFinished += OnAdFinished;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		MPVideoRewardUI videoRewardUI = _videoRewardUI;
		videoRewardUI.QuitBtnEvent = (MPVideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Remove(videoRewardUI.QuitBtnEvent, new MPVideoRewardUI.UIVideoRewardBtnDelegate(QuitBtnHandler));
		MPVideoRewardUI videoRewardUI2 = _videoRewardUI;
		videoRewardUI2.WatchBtnEvent = (MPVideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Remove(videoRewardUI2.WatchBtnEvent, new MPVideoRewardUI.UIVideoRewardBtnDelegate(WatchBtnHandler));
		FyberCallback.AdAvailable -= OnAdAvailable;
		FyberCallback.AdNotAvailable -= OnAdNotAvailable;
		FyberCallback.RequestFail -= OnRequestFail;
		FyberCallback.AdStarted -= OnAdStarted;
		FyberCallback.AdFinished -= OnAdFinished;
		App.net.player.VideoRewardReceivedEvent -= VideoRewardReceivedHandler;
	}

	private void RequestVideoAvailability()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.ECONOMY, eSubType2.SOURCE, string.Format("watch_video_ad_{0}", "multiplayer_store"));
		RewardedVideoRequester.Create().AddParameter("pub0", "87610").AddParameter("pub1", NetworkDefs.gameEndPoint)
			.AddParameter("pub2", App.net.player.data.netId)
			.WithPlacementId("multiplayer_store")
			.NotifyUserOnCompletion(false)
			.Request();
	}

	private IEnumerator RewardTimeoutCo()
	{
		yield return new WaitForSeconds(15f);
		_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD);
		App.net.player.VideoRewardReceivedEvent -= VideoRewardReceivedHandler;
	}

	protected override void TransitionInComplete()
	{
		base.TransitionInComplete();
		RequestVideoAvailability();
	}

	private void QuitBtnHandler()
	{
		if (!_playingVideo)
		{
			App.states.SetBack(new FadeTransInfo(), new FadeTransInfo());
		}
	}

	private void WatchBtnHandler()
	{
		_videoRewardUI.SetState(eVideoRewardUIState.WAITING_FOR_VIDEO);
		RequestVideoAvailability();
	}

	private void OnAdAvailable(Ad ad)
	{
		AdFormat adFormat = ad.AdFormat;
		if (adFormat == AdFormat.REWARDED_VIDEO)
		{
			App.net.session.SendPingNow("PRE_AD");
			ad.Start();
		}
	}

	private void OnAdNotAvailable(AdFormat adFormat)
	{
		_videoRewardUI.SetState(eVideoRewardUIState.NO_VIDEO_AVAILABLE);
	}

	private void OnRequestFail(RequestError error)
	{
		_videoRewardUI.SetState(eVideoRewardUIState.NO_VIDEO_AVAILABLE);
	}

	private void OnAdStarted(Ad ad)
	{
		_playingVideo = true;
		App.sound.musicVolume = 0f;
		_rewardReceived = false;
		App.net.player.VideoRewardReceivedEvent += VideoRewardReceivedHandler;
	}

	private void OnAdFinished(AdResult result)
	{
		_playingVideo = false;
		App.sound.musicVolume = App.saveData.settings.musicVolume;
		if (result.AdFormat != AdFormat.REWARDED_VIDEO)
		{
			return;
		}
		switch (result.Message)
		{
		case "CLOSE_FINISHED":
			if (!_rewardReceived)
			{
				_videoRewardUI.SetState(eVideoRewardUIState.WAITING_FOR_REWARD);
				_rewardTimeoutCo = StartCoroutine(RewardTimeoutCo());
			}
			break;
		case "CLOSE_ABORTED":
			App.net.player.VideoRewardReceivedEvent -= VideoRewardReceivedHandler;
			_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD);
			break;
		default:
			App.net.player.VideoRewardReceivedEvent -= VideoRewardReceivedHandler;
			_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD);
			break;
		}
	}

	private void VideoRewardReceivedHandler(int coinsReceived)
	{
		App.net.player.VideoRewardReceivedEvent -= VideoRewardReceivedHandler;
		if (_rewardTimeoutCo != null)
		{
			StopCoroutine(_rewardTimeoutCo);
			_rewardTimeoutCo = null;
		}
		_rewardReceived = true;
		App.sound.PlaySFX("Audio/SFX/Coins");
		if (coinsReceived > 0)
		{
			App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, "watch_video_ad", coinsReceived);
		}
		_videoRewardUI.TweenCoins(App.net.player.data.coins);
		_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD, coinsReceived);
	}
}
