using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;

public static class TimeServer
{
	private const float _kServerTimeout = 10f;

	private static double _serverToLocalDifference;

	public static double lastServerTime { get; private set; }

	public static double GetLocalUnixTime(bool includeServerDifference = true)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (DateTime.UtcNow - dateTime).TotalSeconds + ((!includeServerDifference) ? 0.0 : _serverToLocalDifference);
	}

	public static IEnumerator GetServerTimeURL(string url, bool fallbackToLocalUnix = false)
	{
		double time = -1.0;
		float timeout = 10f;
		WWW www = new WWW(url);
		while (!www.isDone)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield return time;
				yield break;
			}
			yield return www;
		}
		if (string.IsNullOrEmpty(www.error))
		{
			if (double.TryParse(www.text, out time))
			{
				_serverToLocalDifference = time - GetLocalUnixTime(false);
				lastServerTime = time;
			}
			else if (fallbackToLocalUnix)
			{
				time = GetLocalUnixTime();
			}
		}
		else if (fallbackToLocalUnix)
		{
			time = GetLocalUnixTime();
		}
		yield return time;
	}

	public static IEnumerator GetServerTimeJSONURL(string url, string jsonKey, bool isMilliseconds = true, bool fallbackToLocalUnix = false)
	{
		double time = -1.0;
		float timeout = 10f;
		WWW www = new WWW(url);
		while (!www.isDone)
		{
			timeout -= Time.deltaTime;
			if (timeout <= 0f)
			{
				yield return time;
				yield break;
			}
			yield return www;
		}
		if (string.IsNullOrEmpty(www.error))
		{
			JSONNode jSONNode = JSONNode.Parse(www.text);
			string s = jSONNode[jsonKey];
			if (double.TryParse(s, out time))
			{
				if (isMilliseconds)
				{
					time /= 1000.0;
				}
				_serverToLocalDifference = time - GetLocalUnixTime(false);
				lastServerTime = time;
			}
			else if (fallbackToLocalUnix)
			{
				time = GetLocalUnixTime();
			}
		}
		else if (fallbackToLocalUnix)
		{
			time = GetLocalUnixTime();
		}
		yield return time;
	}
}
