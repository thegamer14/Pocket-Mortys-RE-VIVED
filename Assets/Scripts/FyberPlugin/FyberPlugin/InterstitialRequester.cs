namespace FyberPlugin
{
	public sealed class InterstitialRequester : Requester<InterstitialRequester, RequestCallback>
	{
		private InterstitialRequester()
		{
		}

		public static InterstitialRequester Create()
		{
			return new InterstitialRequester();
		}

		protected override RequesterType GetRequester()
		{
			return RequesterType.Interstitials;
		}
	}
}
