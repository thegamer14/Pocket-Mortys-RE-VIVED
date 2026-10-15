using System;
using System.Collections.Generic;
using FyberPlugin.LitJson;

namespace FyberPlugin
{
	public abstract class Reporter<T> where T : Reporter<T>
	{
		protected const string APP_ID = "appId";

		protected Dictionary<string, object> nativeDict = new Dictionary<string, object>(3);

		protected abstract T GetThis();

		public T AddParameters(Dictionary<string, string> parameters)
		{
			Dictionary<string, string> reportParameters = GetReportParameters();
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				reportParameters[parameter.Key] = parameter.Value;
			}
			return GetThis();
		}

		public T AddParameter(string key, string value)
		{
			Dictionary<string, string> reportParameters = GetReportParameters();
			reportParameters[key] = value;
			return GetThis();
		}

		public void Report()
		{
			PluginBridge.Report(JsonMapper.ToJson(nativeDict));
		}

		private Dictionary<string, string> GetReportParameters()
		{
			if (nativeDict["parameters"] == null)
			{
				nativeDict["parameters"] = new Dictionary<string, string>();
			}
			return nativeDict["parameters"] as Dictionary<string, string>;
		}

		protected void ValidateAppId(string appId)
		{
			if (string.IsNullOrEmpty(appId))
			{
				throw new ArgumentException("App ID cannot be null nor empty.");
			}
		}
	}
}
