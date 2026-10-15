namespace GooglePlayGames
{
	public static class GameInfo
	{
		private const string UnescapedApplicationId = "APP_ID";

		private const string UnescapedIosClientId = "IOS_CLIENTID";

		private const string UnescapedWebClientId = "WEB_CLIENTID";

		private const string UnescapedNearbyServiceId = "NEARBY_SERVICE_ID";

		private const string UnescapedRequireGooglePlus = "REQUIRE_GOOGLE_PLUS";

		public const string ApplicationId = "843889502734";

		public const string IosClientId = "";

		public const string WebClientId = "843889502734-5nifgtkt4aou2ofr34qqenft4nn0997t.apps.googleusercontent.com";

		public const string NearbyConnectionServiceId = "";

		public static bool RequireGooglePlus()
		{
			return false;
		}

		public static bool ApplicationIdInitialized()
		{
			return !string.IsNullOrEmpty("843889502734") && !"843889502734".Equals(ToEscapedToken("APP_ID"));
		}

		public static bool IosClientIdInitialized()
		{
			return !string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("IOS_CLIENTID"));
		}

		public static bool WebClientIdInitialized()
		{
			return !string.IsNullOrEmpty("843889502734-5nifgtkt4aou2ofr34qqenft4nn0997t.apps.googleusercontent.com") && !"843889502734-5nifgtkt4aou2ofr34qqenft4nn0997t.apps.googleusercontent.com".Equals(ToEscapedToken("WEB_CLIENTID"));
		}

		public static bool NearbyConnectionsInitialized()
		{
			return !string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("NEARBY_SERVICE_ID"));
		}

		private static string ToEscapedToken(string token)
		{
			return string.Format("__{0}__", token);
		}
	}
}
