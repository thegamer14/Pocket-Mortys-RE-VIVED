using System;
using System.Collections.Generic;
using FyberPlugin.LitJson;

namespace FyberPlugin
{
	public class Fyber
	{
		public const string Version = "8.10.0";

		private static Fyber fyber;

		private Dictionary<string, string> startParams;

		private Dictionary<string, object> startOptions;

		private bool initialized { get; set; }

		private Settings settings { get; set; }

		private Fyber()
		{
			startOptions = new Dictionary<string, object>(4);
			settings = new Settings();
		}

		public static Fyber With(string appId)
		{
			if (string.IsNullOrEmpty(appId))
			{
				throw new ArgumentException("App ID cannot be null nor empty");
			}
			if (fyber == null)
			{
				fyber = new Fyber();
			}
			if (!fyber.initialized)
			{
				fyber.startOptions["appId"] = appId;
			}
			return fyber;
		}

		public Fyber WithUserId(string userId)
		{
			if (!initialized)
			{
				startOptions["userId"] = userId;
			}
			return this;
		}

		public Fyber WithManualPrecaching()
		{
			if (!initialized)
			{
				CacheManager.PauseDownloads();
				startOptions["startVideoPrecaching"] = false;
			}
			return this;
		}

		public Fyber WithSecurityToken(string securityToken)
		{
			if (!initialized)
			{
				startOptions["securityToken"] = securityToken;
			}
			return this;
		}

		public Fyber WithParameters(Dictionary<string, string> parameters)
		{
			if (!initialized)
			{
				startParams = new Dictionary<string, string>(parameters);
			}
			return this;
		}

		public Settings Start()
		{
			if (!initialized)
			{
				if (startParams != null)
				{
					startOptions["parameters"] = startParams;
				}
				string json = JsonMapper.ToJson(startOptions);
				PluginBridge.Start(json);
			}
			return settings;
		}
	}
}
