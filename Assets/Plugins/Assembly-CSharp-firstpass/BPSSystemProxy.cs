using UnityEngine;

public class BPSSystemProxy
{
	public static string GetProxyName()
	{
		if (!Application.isEditor)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.System"))
			{
				return androidJavaClass.CallStatic<string>("getProperty", new object[1] { "http.proxyHost" });
			}
		}
		return null;
	}

	public static int GetProxyPort()
	{
		if (!Application.isEditor)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.System"))
			{
				string text = androidJavaClass.CallStatic<string>("getProperty", new object[1] { "http.proxyPort" });
				return (!string.IsNullOrEmpty(text)) ? int.Parse(text) : 0;
			}
		}
		return 0;
	}
}
