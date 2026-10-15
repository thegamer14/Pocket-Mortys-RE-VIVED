namespace FyberPlugin
{
	public sealed class CacheManager
	{
		public static void StartPrecaching()
		{
			PluginBridge.Cache("startPrecaching");
		}

		public static void PauseDownloads()
		{
			PluginBridge.Cache("pauseDownloads");
		}

		public static void ResumeDownloads()
		{
			PluginBridge.Cache("resumeDownloads");
		}

		public static bool HasCachedVideos()
		{
			return PluginBridge.Cache("hasCachedVideos");
		}
	}
}
