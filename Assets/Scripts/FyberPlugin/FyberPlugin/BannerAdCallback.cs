namespace FyberPlugin
{
	public interface BannerAdCallback
	{
		void OnAdError(BannerAd ad, string error);

		void OnAdLoaded(BannerAd ad);

		void OnAdClicked(BannerAd ad);

		void OnAdLeftApplication(BannerAd ad);

		void OnAdWillPresentModalView(BannerAd ad);

		void OnAdDidDismissModalView(BannerAd ad);
	}
}
