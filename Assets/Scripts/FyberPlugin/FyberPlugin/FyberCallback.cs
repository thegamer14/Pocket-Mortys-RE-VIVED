using System;

namespace FyberPlugin
{
	public class FyberCallback : RequestCallback, VirtualCurrencyCallback, AdCallback, BannerAdCallback, AnalysisCallback, Callback
	{
		private static FyberCallback instance;

		public static FyberCallback Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FyberCallback();
				}
				return instance;
			}
		}

		public static event Action<Ad> AdAvailable;

		public static event Action<AdFormat> AdNotAvailable;

		public static event Action<VirtualCurrencyResponse> VirtualCurrencySuccess;

		public static event Action<VirtualCurrencyErrorResponse> VirtualCurrencyError;

		public static event Action<RequestError> RequestFail;

		public static event Action<Ad> AdStarted;

		public static event Action<AdResult> AdFinished;

		public static event Action<string> NativeError;

		public static event Action<BannerAd, string> BannerAdError;

		public static event Action<BannerAd> BannerAdLoaded;

		public static event Action<BannerAd> BannerAdClicked;

		public static event Action<BannerAd> BannerAdLeftApplication;

		public static event Action<bool> VideoCached;

		public static event Action<IntegrationReport> AnalysisSucceeded;

		public static event Action<AnalysisError?> AnalysisFailed;

		public static event Action<BannerAd> BannerAdWillPresentModalView;

		public static event Action<BannerAd> BannerAdDidDismissModalView;

		internal FyberCallback()
		{
		}

		public void OnAdAvailable(Ad ad)
		{
			if (FyberCallback.AdAvailable != null)
			{
				FyberCallback.AdAvailable(ad);
			}
		}

		public void OnAdNotAvailable(AdFormat adFormat)
		{
			if (FyberCallback.AdNotAvailable != null)
			{
				FyberCallback.AdNotAvailable(adFormat);
			}
		}

		public void OnError(VirtualCurrencyErrorResponse response)
		{
			if (FyberCallback.VirtualCurrencyError != null)
			{
				FyberCallback.VirtualCurrencyError(response);
			}
		}

		public void OnSuccess(VirtualCurrencyResponse response)
		{
			if (FyberCallback.VirtualCurrencySuccess != null)
			{
				FyberCallback.VirtualCurrencySuccess(response);
			}
		}

		public void OnRequestError(RequestError error)
		{
			if (FyberCallback.RequestFail != null)
			{
				FyberCallback.RequestFail(error);
			}
		}

		public void OnAdStarted(Ad ad)
		{
			if (FyberCallback.AdStarted != null)
			{
				FyberCallback.AdStarted(ad);
			}
		}

		public void OnAdFinished(AdResult result)
		{
			if (FyberCallback.AdFinished != null)
			{
				FyberCallback.AdFinished(result);
			}
		}

		public void OnNativeError(string message)
		{
			if (FyberCallback.NativeError != null)
			{
				FyberCallback.NativeError(message);
			}
		}

		public void OnAdError(BannerAd ad, string error)
		{
			if (FyberCallback.BannerAdError != null)
			{
				FyberCallback.BannerAdError(ad, error);
			}
		}

		public void OnAdLoaded(BannerAd ad)
		{
			if (FyberCallback.BannerAdLoaded != null)
			{
				FyberCallback.BannerAdLoaded(ad);
			}
		}

		public void OnAdClicked(BannerAd ad)
		{
			if (FyberCallback.BannerAdClicked != null)
			{
				FyberCallback.BannerAdClicked(ad);
			}
		}

		public void OnAdLeftApplication(BannerAd ad)
		{
			if (FyberCallback.BannerAdLeftApplication != null)
			{
				FyberCallback.BannerAdLeftApplication(ad);
			}
		}

		public void OnVideoCached(bool videosAvailable)
		{
			if (FyberCallback.VideoCached != null)
			{
				FyberCallback.VideoCached(videosAvailable);
			}
		}

		public void OnAnalysisSucceeded(IntegrationReport integrationReport)
		{
			if (FyberCallback.AnalysisSucceeded != null)
			{
				FyberCallback.AnalysisSucceeded(integrationReport);
			}
		}

		public void OnAnalysisFailed(AnalysisError? analysisError)
		{
			if (FyberCallback.AnalysisFailed != null)
			{
				FyberCallback.AnalysisFailed(analysisError);
			}
		}

		public void OnAdWillPresentModalView(BannerAd ad)
		{
			if (FyberCallback.BannerAdWillPresentModalView != null)
			{
				FyberCallback.BannerAdWillPresentModalView(ad);
			}
		}

		public void OnAdDidDismissModalView(BannerAd ad)
		{
			if (FyberCallback.BannerAdDidDismissModalView != null)
			{
				FyberCallback.BannerAdDidDismissModalView(ad);
			}
		}
	}
}
