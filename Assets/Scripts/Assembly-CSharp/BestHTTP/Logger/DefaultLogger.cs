using System;
using System.Text;
using UnityEngine;

namespace BestHTTP.Logger
{
	public class DefaultLogger : ILogger
	{
		public Loglevels Level { get; set; }

		public string FormatVerbose { get; set; }

		public string FormatInfo { get; set; }

		public string FormatWarn { get; set; }

		public string FormatErr { get; set; }

		public string FormatEx { get; set; }

		public DefaultLogger()
		{
			FormatVerbose = "D [{0}]: {1}";
			FormatInfo = "I [{0}]: {1}";
			FormatWarn = "W [{0}]: {1}";
			FormatErr = "Err [{0}]: {1}";
			FormatEx = "Ex [{0}]: {1} - Message: {2}  StackTrace: {3}";
			Level = ((!UnityEngine.Debug.isDebugBuild) ? Loglevels.Error : Loglevels.Warning);
		}

		public void Verbose(string division, string verb)
		{
			if ((int)Level <= 0)
			{
				try
				{
					UnityEngine.Debug.Log(string.Format(FormatVerbose, division, verb));
				}
				catch
				{
				}
			}
		}

		public void Information(string division, string info)
		{
			if ((int)Level <= 1)
			{
				try
				{
					UnityEngine.Debug.Log(string.Format(FormatInfo, division, info));
				}
				catch
				{
				}
			}
		}

		public void Warning(string division, string warn)
		{
			if ((int)Level <= 2)
			{
				try
				{
					UnityEngine.Debug.LogWarning(string.Format(FormatWarn, division, warn));
				}
				catch
				{
				}
			}
		}

		public void Error(string division, string err)
		{
			if ((int)Level <= 3)
			{
				try
				{
					UnityEngine.Debug.LogError(string.Format(FormatErr, division, err));
				}
				catch
				{
				}
			}
		}

		public void Exception(string division, string msg, Exception ex)
		{
			if ((int)Level > 4)
			{
				return;
			}
			try
			{
				string empty = string.Empty;
				if (ex == null)
				{
					empty = "null";
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					Exception ex2 = ex;
					int num = 1;
					while (ex2 != null)
					{
						stringBuilder.AppendFormat("{0}: {1} {2}", num++.ToString(), ex.Message, ex.StackTrace);
						ex2 = ex2.InnerException;
						if (ex2 != null)
						{
							stringBuilder.AppendLine();
						}
					}
					empty = stringBuilder.ToString();
				}
				UnityEngine.Debug.LogError(string.Format(FormatEx, division, msg, empty, (ex == null) ? "null" : ex.StackTrace));
			}
			catch
			{
			}
		}
	}
}
