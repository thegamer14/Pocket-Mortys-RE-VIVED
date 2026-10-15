using System;
using System.Collections;
using FyberPlugin.LitJson;
using UnityEngine;

namespace FyberPlugin
{
	public class FyberGameObject : MonoBehaviour
	{
		private const string ObjectName = "FyberGameObject";

		static FyberGameObject()
		{
			Type typeFromHandle = typeof(FyberGameObject);
			try
			{
				MonoBehaviour monoBehaviour = UnityEngine.Object.FindObjectOfType(typeFromHandle) as MonoBehaviour;
				if (!(monoBehaviour != null))
				{
					GameObject gameObject = new GameObject("FyberGameObject");
					gameObject.AddComponent(typeFromHandle);
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
			catch (UnityException)
			{
				Debug.LogWarning(string.Concat("It looks like you have the ", typeFromHandle, " on a GameObject in your scene. Please remove the script from your scene."));
			}
		}

		public static void Init()
		{
		}

		private void OnEnable()
		{
			PluginBridge.GameObjectStarted();
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (focusStatus)
			{
				PluginBridge.GameObjectStarted();
			}
		}

		private void OnNativeMessageReceived(string json)
		{
			NativeMessage nativeMessage = JsonMapper.ToObject<NativeMessage>(json);
			if (string.IsNullOrEmpty(nativeMessage.Id))
			{
				FyberCallback.Instance.OnNativeError("An unknown error occurred while processing the ads. Please request again.");
			}
			else
			{
				FyberCallbacksManager.Instance.Process(nativeMessage);
			}
		}

		private void OnPrecachingFinished(string cachedVideosAvailable)
		{
			if (!string.IsNullOrEmpty(cachedVideosAvailable))
			{
				FyberCallback.Instance.OnVideoCached(bool.Parse(cachedVideosAvailable));
			}
		}

		private void OnNativeErrorOccurred(string json)
		{
			NativeError nativeError = JsonMapper.ToObject<NativeError>(json);
			FyberCallback.Instance.OnNativeError(nativeError.Error);
			if (!string.IsNullOrEmpty(nativeError.Id))
			{
				FyberCallbacksManager.Instance.ClearCallbacks(nativeError.Id);
			}
		}

		private void OnApplicationQuit()
		{
			PluginBridge.ApplicationQuit();
		}

		internal IEnumerator SkipFrameCoroutineWithBlock(Action<int> block)
		{
			yield return null;
			block(0);
		}

		public void SkipFrameWithBlock(Action<int> block)
		{
			StartCoroutine(SkipFrameCoroutineWithBlock(block));
		}
	}
}
