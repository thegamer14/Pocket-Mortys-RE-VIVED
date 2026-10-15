using UnityEngine;

namespace FyberPlugin
{
	public sealed class PluginBridge
	{
		public static IPluginBridge bridge;

		static PluginBridge()
		{
			Resources.Load("FyberSettings");
		}

		internal static void Start(string json)
		{
			if (bridge != null)
			{
				bridge.StartSDK(json);
			}
		}

		internal static bool Cache(string action)
		{
			if (bridge != null)
			{
				return bridge.Cache(action);
			}
			return false;
		}

		internal static void Request(string json)
		{
			if (bridge != null)
			{
				bridge.Request(json);
			}
		}

		internal static void StartAd(string json)
		{
			if (bridge != null)
			{
				bridge.StartAd(json);
			}
		}

		internal static bool Banner(string json)
		{
			if (bridge != null)
			{
				return bridge.Banner(json);
			}
			return false;
		}

		internal static void Report(string json)
		{
			if (bridge != null)
			{
				bridge.Report(json);
			}
		}

		internal static string Settings(string json)
		{
			if (bridge != null)
			{
				return bridge.Settings(json);
			}
			return null;
		}

		internal static void EnableLogging(bool shouldLog)
		{
			if (bridge != null)
			{
				bridge.EnableLogging(shouldLog);
			}
		}

		internal static void GameObjectStarted()
		{
			if (bridge != null)
			{
				bridge.GameObjectStarted();
			}
		}

		internal static void ApplicationQuit()
		{
			if (bridge != null)
			{
				bridge.ApplicationQuit();
			}
		}

		internal static void ShowTestSuite()
		{
			if (bridge != null)
			{
				bridge.ShowTestSuite();
			}
		}

		internal static void AnalyzeTestSuite()
		{
			if (bridge != null)
			{
				bridge.AnalyzeTestSuite();
			}
		}
	}
}
