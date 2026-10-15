using System;
using System.Collections.Generic;

namespace FyberPlugin
{
	internal sealed class FyberCallbacksManager
	{
		private static FyberCallbacksManager instance;

		private Dictionary<string, object> callbacks;

		private Dictionary<string, BannerAdCallback> bannerCallbacks;

		private Dictionary<string, Ad> ads;

		public static FyberCallbacksManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FyberCallbacksManager();
				}
				return instance;
			}
		}

		private FyberCallbacksManager()
		{
			callbacks = new Dictionary<string, object>();
			bannerCallbacks = new Dictionary<string, BannerAdCallback>();
			ads = new Dictionary<string, Ad>();
		}

		internal static void AddCallback(string guid, object callback)
		{
			Instance.callbacks[guid] = callback;
		}

		public static void AddBannerCallback(string guid, BannerAdCallback bannerAdCallback)
		{
			Instance.bannerCallbacks[guid] = bannerAdCallback;
		}

		internal static void AddAd(string guid, Ad ad)
		{
			Instance.ads[guid] = ad;
		}

		internal void Process(NativeMessage message)
		{
			if (callbacks.ContainsKey(message.Id) || message.Id.Equals("testsuite", StringComparison.Ordinal))
			{
				switch (message.Type)
				{
				case 0:
					ProcessRequestCallback(message);
					break;
				case 1:
					ProcessAdCallback(message);
					break;
				case 2:
					ProcessBannerAdCallback(message);
					break;
				case 3:
					ProcessTestSuiteResponse(message);
					break;
				default:
					ClearCallbacks(message.Id);
					FyberCallback.Instance.OnNativeError("An unknown error occurred.");
					break;
				}
			}
			else
			{
				ClearCallbacks(message.Id);
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
			}
		}

		private void ProcessAdCallback(NativeMessage message)
		{
			if (message.AdPayload == null)
			{
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				ClearCallbacks(message.Id);
				return;
			}
			switch (message.Origin)
			{
			case 0:
				FireAdCallback(message, AdFormat.OFFER_WALL);
				break;
			case 1:
				FireAdCallback(message, AdFormat.REWARDED_VIDEO);
				break;
			case 2:
				FireAdCallback(message, AdFormat.INTERSTITIAL);
				break;
			case 3:
				FireAdCallback(message, AdFormat.BANNER);
				break;
			default:
				ClearCallbacks(message.Id);
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				break;
			}
		}

		private void ProcessBannerAdCallback(NativeMessage message)
		{
			BannerAdPayload bannerAdPayload = message.BannerAdPayload;
			BannerAd bannerAd = ads[message.Id] as BannerAd;
			BannerAdCallback bannerAdCallback = bannerCallbacks[message.Id];
			if (bannerAdPayload == null || bannerAd == null)
			{
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				ClearCallbacks(message.Id);
				ClearBannerCallback(message.Id);
			}
			else
			{
				if (bannerAdCallback == null)
				{
					return;
				}
				int? num = bannerAdPayload.Event;
				if (num.HasValue)
				{
					switch (num.Value)
					{
					case 0:
						bannerAdCallback.OnAdError(bannerAd, bannerAdPayload.ErrorMessage);
						return;
					case 1:
						bannerAdCallback.OnAdLoaded(bannerAd);
						return;
					case 2:
						bannerAdCallback.OnAdClicked(bannerAd);
						return;
					case 3:
						bannerAdCallback.OnAdLeftApplication(bannerAd);
						return;
					case 4:
						bannerAdCallback.OnAdWillPresentModalView(bannerAd);
						return;
					case 5:
						bannerAdCallback.OnAdDidDismissModalView(bannerAd);
						return;
					}
				}
				ClearBannerCallback(message.Id);
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
			}
		}

		private void ProcessTestSuiteResponse(NativeMessage message)
		{
			TestSuitePayload testSuitePayload = message.TestSuitePayload;
			AnalysisError? analysisError = testSuitePayload.AnalysisError;
			if (analysisError.HasValue)
			{
				FyberCallback.Instance.OnAnalysisFailed(analysisError);
				return;
			}
			IntegrationReport success = testSuitePayload.Success;
			FyberCallback.Instance.OnAnalysisSucceeded(success);
		}

		internal void ClearCallbacks(string id)
		{
			if (ads.ContainsKey(id))
			{
				Ad ad = ads[id];
				if (ad.AdFormat != AdFormat.BANNER)
				{
					ads.Remove(id);
				}
				callbacks.Remove(id);
			}
		}

		internal void ClearBannerCallback(string id)
		{
			ads.Remove(id);
			bannerCallbacks.Remove(id);
		}

		private void FireAdCallback(NativeMessage message, AdFormat adFormat)
		{
			AdPayload adPayload = message.AdPayload;
			AdCallback callback = GetCallback<AdCallback>(message.Id, !adPayload.AdStarted);
			if (adPayload.AdStarted)
			{
				if (ads.ContainsKey(message.Id))
				{
					Ad ad = ads[message.Id];
					if (ad.AdFormat != AdFormat.BANNER)
					{
						ads.Remove(message.Id);
					}
					callback.OnAdStarted(ad);
				}
				else
				{
					ClearCallbacks(message.Id);
					FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				}
			}
			else
			{
				AdResult adResult = new AdResult();
				if (string.IsNullOrEmpty(adPayload.Error))
				{
					adResult.Status = AdStatus.OK;
					adResult.Message = adPayload.Status;
				}
				else
				{
					adResult.Status = AdStatus.Error;
					adResult.Message = adPayload.Error;
				}
				adResult.AdFormat = adFormat;
				callback.OnAdFinished(adResult);
			}
		}

		private void ProcessRequestCallback(NativeMessage message)
		{
			if (message.RequestPayload == null)
			{
				ClearCallbacks(message.Id);
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				return;
			}
			if (message.RequestPayload.RequestError.HasValue)
			{
				Callback callback = GetCallback<Callback>(message.Id);
				callback.OnRequestError(RequestError.FromNative(message.RequestPayload.RequestError.Value));
				return;
			}
			switch (message.Origin)
			{
			case 0:
				FireRequestCallback(message, AdFormat.OFFER_WALL);
				break;
			case 1:
				FireRequestCallback(message, AdFormat.REWARDED_VIDEO);
				break;
			case 2:
				FireRequestCallback(message, AdFormat.INTERSTITIAL);
				break;
			case 3:
				FireRequestCallback(message, AdFormat.BANNER);
				break;
			case 4:
			{
				VirtualCurrencyCallback callback2 = GetCallback<VirtualCurrencyCallback>(message.Id);
				if (message.RequestPayload.CurrencyResponse != null)
				{
					callback2.OnSuccess(message.RequestPayload.CurrencyResponse);
				}
				else
				{
					callback2.OnError(message.RequestPayload.CurrencyErrorResponse);
				}
				break;
			}
			default:
				ClearCallbacks(message.Id);
				FyberCallback.Instance.OnNativeError("An unknown error occurred. Please, request Ads again.");
				break;
			}
		}

		private void FireRequestCallback(NativeMessage message, AdFormat format)
		{
			RequestCallback callback = GetCallback<RequestCallback>(message.Id);
			if (message.RequestPayload.AdAvailable == true)
			{
				Ad ad;
				if (format == AdFormat.BANNER)
				{
					ad = new BannerAd(message.Id);
				}
				else
				{
					ad = new Ad(message.Id);
					ad.AdFormat = format;
				}
				if (!string.IsNullOrEmpty(message.RequestPayload.PlacementId))
				{
					ad.PlacementId = message.RequestPayload.PlacementId;
				}
				callback.OnAdAvailable(ad);
			}
			else
			{
				callback.OnAdNotAvailable(format);
			}
		}

		private T GetCallback<T>(string id, bool remove = true) where T : class
		{
			T result = callbacks[id] as T;
			if (remove)
			{
				callbacks.Remove(id);
			}
			return result;
		}
	}
}
