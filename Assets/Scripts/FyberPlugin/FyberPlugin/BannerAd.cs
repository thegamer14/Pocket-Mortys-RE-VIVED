using System.Collections.Generic;
using FyberPlugin.LitJson;
using UnityEngine;

namespace FyberPlugin
{
	public class BannerAd : Ad
	{
		private int? position;

		public const int POSITION_TOP = 1;

		public const int POSITION_BOTTOM = 0;

		private BannerAdCallback bannerAdCallback;

		internal BannerAd(string guid)
			: base(guid)
		{
			base.AdFormat = AdFormat.BANNER;
		}

		public BannerAd WithBannerAdCallback(BannerAdCallback callback)
		{
			if (!Started)
			{
				bannerAdCallback = callback;
			}
			return this;
		}

		public BannerAd WithPosition(int position)
		{
			if (!Started)
			{
				this.position = position;
			}
			return this;
		}

		public BannerAd DisplayAtTop()
		{
			return WithPosition(1);
		}

		public BannerAd DisplayAtBottom()
		{
			return WithPosition(0);
		}

		public void Hide()
		{
			if (Started)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(3);
				dictionary["id"] = guid;
				dictionary["ad"] = base.AdFormat;
				dictionary["action"] = "hide";
				PluginBridge.Banner(JsonMapper.ToJson(dictionary));
			}
			else
			{
				Debug.Log("This instance has not been started yet, the \"Hide()\" method will not do anything");
			}
		}

		public void Show()
		{
			if (Started)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(3);
				dictionary["id"] = guid;
				dictionary["ad"] = base.AdFormat;
				dictionary["action"] = "show";
				PluginBridge.Banner(JsonMapper.ToJson(dictionary));
			}
			else
			{
				Debug.Log("This instance has not been started yet, the \"Show()\" method will not do anything");
			}
		}

		public void Destroy()
		{
			if (Started)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>(3);
				dictionary["id"] = guid;
				dictionary["ad"] = base.AdFormat;
				dictionary["action"] = "destroy";
				PluginBridge.Banner(JsonMapper.ToJson(dictionary));
				FyberCallbacksManager.Instance.ClearBannerCallback(guid);
			}
			else
			{
				Debug.Log("This instance has not been started yet, the \"Destroy()\" method will not do anything");
			}
		}

		protected override bool CanStart()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(3);
			dictionary["id"] = guid;
			dictionary["ad"] = base.AdFormat;
			dictionary["action"] = "canStart";
			return PluginBridge.Banner(JsonMapper.ToJson(dictionary));
		}

		protected override void RegisterCallbacks(string guid)
		{
			base.RegisterCallbacks(guid);
			if (bannerAdCallback == null)
			{
				bannerAdCallback = FyberCallback.Instance;
			}
			FyberCallbacksManager.AddBannerCallback(guid, bannerAdCallback);
		}

		protected override string GetGuid()
		{
			Debug.Log("GetGuid - " + guid);
			return guid;
		}

		protected override void AddExtraInfo(Dictionary<string, object> dict)
		{
			int? num = position;
			if (num.HasValue)
			{
				dict["position"] = position;
			}
		}
	}
}
