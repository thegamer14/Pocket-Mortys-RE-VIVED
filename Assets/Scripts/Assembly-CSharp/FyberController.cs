using FyberPlugin;
using UnityEngine;

public class FyberController : MonoBehaviour
{
	public const string kAppId = "87610";

	public const string kAppToken = "6cabdc8c05d6dc7a5b9c655f213c46a5";

	public bool isVideoAvailable;

	public bool enableDebug;

	private void Awake()
	{
		FyberCallback.NativeError += FyberNativeErrorHandler;
		FyberCallback.RequestFail += FyberRequestFailHandler;
		FyberCallback.AdAvailable += OnAdAvailable;
		FyberCallback.AdNotAvailable += OnAdNotAvailable;
		FyberCallback.AnalysisSucceeded += OnAnalysisSucceeded;
		FyberCallback.AnalysisFailed += OnAnalysisFailed;
		Settings settings = Fyber.With("87610").WithSecurityToken("6cabdc8c05d6dc7a5b9c655f213c46a5").Start();
		settings.NotifyUserOnReward(false);
		if (TestDefs.DEBUG_FYBER_PLUGIN)
		{
			FyberLogger.EnableLogging(true);
		}
	}

	private void Start()
	{
		IntegrationAnalyzer.Analyze();
	}

	public void RequestVideo(MPVideoRewardData data)
	{
		DebugLog("RequestVideo()");
		isVideoAvailable = false;
		RewardedVideoRequester.Create().AddParameter("pub0", "87610").AddParameter("pub1", NetworkDefs.gameEndPoint)
			.AddParameter("pub2", App.net.player.data.netId)
			.AddParameter("pub3", data.Id)
			.AddParameter("pub4", data.token)
			.NotifyUserOnCompletion(false)
			.Request();
	}

	private void OnDestroy()
	{
		FyberCallback.NativeError -= FyberNativeErrorHandler;
		FyberCallback.RequestFail -= FyberRequestFailHandler;
		FyberCallback.AdAvailable -= OnAdAvailable;
		FyberCallback.AdNotAvailable -= OnAdNotAvailable;
		FyberCallback.AnalysisSucceeded -= OnAnalysisSucceeded;
		FyberCallback.AnalysisFailed -= OnAnalysisFailed;
	}

	private void FyberNativeErrorHandler(string error)
	{
		DebugLog("FyberNativeErrorHandler() " + error);
	}

	private void FyberRequestFailHandler(RequestError requestError)
	{
		DebugLog("FyberRequestFailHandler() " + requestError.Description);
	}

	private void OnAdAvailable(Ad ad)
	{
		DebugLog("OnAdAvailable() ad.AdFormat:" + ad.AdFormat);
		AdFormat adFormat = ad.AdFormat;
		if (adFormat == AdFormat.REWARDED_VIDEO)
		{
			isVideoAvailable = true;
		}
	}

	private void OnAdNotAvailable(AdFormat adFormat)
	{
		DebugLog("OnAdNotAvailable()");
		isVideoAvailable = false;
	}

	private void OnAnalysisSucceeded(IntegrationReport integrationReport)
	{
		DebugLog("OnAnalysisSucceeded()");
		foreach (MediationBundleInfo startedBundle in integrationReport.StartedBundles)
		{
			DebugLog("Bundle started: " + startedBundle.ToString());
		}
		foreach (MediationBundleInfo unstartedBundle in integrationReport.UnstartedBundles)
		{
			DebugLog("Bundle not started: " + unstartedBundle.ToString());
		}
	}

	private void OnAnalysisFailed(AnalysisError? analysisError)
	{
		DebugLog("OnAnalysisFailed()");
	}

	protected void DebugLog(string str)
	{
		if (enableDebug)
		{
		}
	}
}
