namespace FyberPlugin
{
	public sealed class OfferWallRequester : Requester<OfferWallRequester, RequestCallback>
	{
		private const string CLOSE_ON_REDIRECT_KEY = "closeOfferWallOnRedirect";

		private const string SHOW_CLOSE_BUTTON_ON_LOAD_KEY = "showCloseButtonOnLoad";

		private OfferWallRequester()
		{
		}

		public static OfferWallRequester Create()
		{
			return new OfferWallRequester();
		}

		public OfferWallRequester CloseOnRedirect(bool shouldCloseOnRedirect)
		{
			requesterAttributes["closeOfferWallOnRedirect"] = shouldCloseOnRedirect;
			return this;
		}

		public OfferWallRequester ShowCloseButtonOnLoad(bool shouldShowCloseButtonOnLoad)
		{
			requesterAttributes["showCloseButtonOnLoad"] = shouldShowCloseButtonOnLoad;
			return this;
		}

		protected override RequesterType GetRequester()
		{
			return RequesterType.OfferWall;
		}
	}
}
