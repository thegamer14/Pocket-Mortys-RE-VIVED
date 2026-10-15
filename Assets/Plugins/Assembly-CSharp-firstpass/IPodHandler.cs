using System.Runtime.InteropServices;
using UnityEngine;

public class IPodHandler
{
	public static bool Active
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return IsIPodMusicPlaying();
			}
			return false;
		}
	}

	[DllImport("__Internal")]
	private static extern bool IsIPodMusicPlaying();

	[DllImport("__Internal")]
	private static extern void RegisterUnityIPodCallbackListener(string gameObject, string method);

	public static void RegisterUnityIPodCallbackListener(GameObject targetObj, string functionName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			RegisterUnityIPodCallbackListener(targetObj.name, functionName);
		}
	}
}
