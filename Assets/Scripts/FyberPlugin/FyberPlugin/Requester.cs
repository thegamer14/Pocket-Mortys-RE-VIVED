using System;
using System.Collections.Generic;
using FyberPlugin.LitJson;

namespace FyberPlugin
{
	public abstract class Requester<T, V> where T : Requester<T, V> where V : class, Callback
	{
		protected enum RequesterType
		{
			OfferWall = 0,
			RewardedVideos = 1,
			Interstitials = 2,
			Banners = 3,
			VirtualCurrency = 4
		}

		protected const string CUSTOM_PARAMS_KEY = "parameters";

		private const string REQUEST_ID = "requestId";

		private const string REQUESTER = "requester";

		protected const string PLACEMENT_ID_KEY = "placementId";

		protected Dictionary<string, object> requesterAttributes;

		protected V requestCallback;

		public Requester()
		{
			requesterAttributes = new Dictionary<string, object>();
		}

		public virtual void Request()
		{
			GetRequesterAttributes();
			string json = JsonMapper.ToJson(requesterAttributes);
			PluginBridge.Request(json);
		}

		protected abstract RequesterType GetRequester();

		internal Dictionary<string, object> GetRequesterAttributes()
		{
			requesterAttributes["requestId"] = GenerateUidAndAddToQueue();
			requesterAttributes["requester"] = GetRequester();
			return requesterAttributes;
		}

		internal string GenerateUidAndAddToQueue()
		{
			string text = Guid.NewGuid().ToString();
			if (requestCallback == null)
			{
				requestCallback = FyberCallback.Instance as V;
			}
			FyberCallbacksManager.AddCallback(text, requestCallback);
			return text;
		}

		public T WithPlacementId(string placementId)
		{
			requesterAttributes["placementId"] = placementId;
			return this as T;
		}

		public T WithCallback(V requestCallback)
		{
			this.requestCallback = requestCallback;
			return this as T;
		}

		public T AddParameters(Dictionary<string, string> parameters)
		{
			Dictionary<string, string> customParameters = GetCustomParameters();
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				customParameters[parameter.Key] = parameter.Value;
			}
			return this as T;
		}

		public T AddParameter(string key, string value)
		{
			Dictionary<string, string> customParameters = GetCustomParameters();
			customParameters[key] = value;
			return this as T;
		}

		public T ClearParameters()
		{
			GetCustomParameters().Clear();
			return this as T;
		}

		public T RemoveParameter(string key)
		{
			GetCustomParameters().Remove(key);
			return this as T;
		}

		protected Dictionary<string, string> GetCustomParameters()
		{
			if (!requesterAttributes.ContainsKey("parameters"))
			{
				requesterAttributes["parameters"] = new Dictionary<string, string>();
			}
			return requesterAttributes["parameters"] as Dictionary<string, string>;
		}
	}
}
