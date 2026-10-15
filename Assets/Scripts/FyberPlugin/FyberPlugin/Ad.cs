using System;
using System.Collections.Generic;
using FyberPlugin.LitJson;
using UnityEngine;

namespace FyberPlugin
{
	public class Ad
	{
		protected bool Started;

		protected string guid;

		public AdFormat AdFormat { get; internal set; }

		public string PlacementId { get; internal set; }

		private AdCallback adCallback { get; set; }

		internal Ad(string guid)
		{
			this.guid = guid;
		}

		public Ad WithCallback(AdCallback callback)
		{
			adCallback = callback;
			return this;
		}

		public void Start()
		{
			if (!Started && CanStart())
			{
				Started = true;
				string text = GetGuid();
				Debug.Log("Start - " + text);
				RegisterCallbacks(text);
				Dictionary<string, object> dictionary = new Dictionary<string, object>(3);
				dictionary["id"] = text;
				dictionary["ad"] = AdFormat;
				if (!string.IsNullOrEmpty(PlacementId))
				{
					dictionary["placementId"] = PlacementId;
				}
				Debug.Log("dict - " + dictionary["id"]);
				AddExtraInfo(dictionary);
				PluginBridge.StartAd(JsonMapper.ToJson(dictionary));
			}
			else
			{
				FyberCallback.Instance.OnNativeError("This ad was already shown. You should request a new one.");
			}
		}

		protected virtual bool CanStart()
		{
			return true;
		}

		protected virtual string GetGuid()
		{
			if (guid == null)
			{
				guid = Guid.NewGuid().ToString();
			}
			return guid;
		}

		protected virtual void RegisterCallbacks(string guid)
		{
			if (adCallback == null)
			{
				adCallback = FyberCallback.Instance;
			}
			FyberCallbacksManager.AddCallback(guid, adCallback);
			FyberCallbacksManager.AddAd(guid, this);
		}

		protected virtual void AddExtraInfo(Dictionary<string, object> dict)
		{
		}
	}
}
