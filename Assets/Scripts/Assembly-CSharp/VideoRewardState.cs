using System;
using FyberPlugin;
using UnityEngine;

public class VideoRewardState : StateBase
{
	private const int _kRewardCoinAmount = 30;

	private GameObject _backgroundContainer;

	private VideoRewardUI _videoRewardUI;

	private bool _playingVideo;

	public override void Awake()
	{
		base.Awake();
		_uiResourcePath = string.Format("UI/VideoReward/{0}", "VideoRewardMenu");
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
		_videoRewardUI = _ui as VideoRewardUI;
		_videoRewardUI.SetState(eVideoRewardUIState.WAITING_FOR_VIDEO);
		_videoRewardUI.SetCoins(PlayerDefs.CoinCount);
	}

	protected override void Destroy()
	{
		base.Destroy();
		_videoRewardUI = null;
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		VideoRewardUI videoRewardUI = _videoRewardUI;
		videoRewardUI.QuitBtnEvent = (VideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Combine(videoRewardUI.QuitBtnEvent, new VideoRewardUI.UIVideoRewardBtnDelegate(QuitBtnHandler));
		VideoRewardUI videoRewardUI2 = _videoRewardUI;
		videoRewardUI2.WatchBtnEvent = (VideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Combine(videoRewardUI2.WatchBtnEvent, new VideoRewardUI.UIVideoRewardBtnDelegate(WatchBtnHandler));
		FyberCallback.AdAvailable += OnAdAvailable;
		FyberCallback.AdNotAvailable += OnAdNotAvailable;
		FyberCallback.RequestFail += OnRequestFail;
		FyberCallback.AdStarted += OnAdStarted;
		FyberCallback.AdFinished += OnAdFinished;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		VideoRewardUI videoRewardUI = _videoRewardUI;
		videoRewardUI.QuitBtnEvent = (VideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Remove(videoRewardUI.QuitBtnEvent, new VideoRewardUI.UIVideoRewardBtnDelegate(QuitBtnHandler));
		VideoRewardUI videoRewardUI2 = _videoRewardUI;
		videoRewardUI2.WatchBtnEvent = (VideoRewardUI.UIVideoRewardBtnDelegate)Delegate.Remove(videoRewardUI2.WatchBtnEvent, new VideoRewardUI.UIVideoRewardBtnDelegate(WatchBtnHandler));
		FyberCallback.AdAvailable -= OnAdAvailable;
		FyberCallback.AdNotAvailable -= OnAdNotAvailable;
		FyberCallback.RequestFail -= OnRequestFail;
		FyberCallback.AdStarted -= OnAdStarted;
		FyberCallback.AdFinished -= OnAdFinished;
	}

	private void RequestVideoAvailability()
	{
		App.upsight.CustomAnalyticEvent(eSubType1.ECONOMY, eSubType2.SOURCE, string.Format("watch_video_ad_{0}", "campaign_store"));
		RewardedVideoRequester.Create().WithPlacementId("campaign_store").NotifyUserOnCompletion(false)
			.Request();
	}

	private void RequestCurrency()
	{
		VirtualCurrencyRequester.Create().NotifyUserOnReward(false).Request();
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
	}

	private void OnAdFinished(AdResult result)
	{
		UnityEngine.Debug.Log("VideoRewardState: OnAdFinished() result.Message: " + result.Message);
		_playingVideo = false;
		App.sound.musicVolume = App.saveData.settings.musicVolume;
		if (result.AdFormat == AdFormat.REWARDED_VIDEO)
		{
			switch (result.Message)
			{
			case "CLOSE_FINISHED":
				RewardClientSide();
				break;
			case "CLOSE_ABORTED":
				_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD);
				break;
			default:
				_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD);
				break;
			}
		}
	}

	private void RewardClientSide()
	{
		UnityEngine.Debug.Log("VideoRewardState: RewardClientSide()");
		App.sound.PlaySFX("Audio/SFX/Coins");
		int resultAmount;
		string resultText;
		PlayerDefs.AddCoin(30, out resultAmount, out resultText);
		if (resultAmount > 0)
		{
			App.upsight.CustomAnalyticValueEvent(eSubType1.ECONOMY, eSubType2.SOURCE, "watch_video_ad", resultAmount);
		}
		App.saveData.Save(eSaveDataType.PLAYER);
		_videoRewardUI.TweenCoins(PlayerDefs.CoinCount);
		_videoRewardUI.SetState(eVideoRewardUIState.DISPLAY_REWARD, 30);
	}
}
