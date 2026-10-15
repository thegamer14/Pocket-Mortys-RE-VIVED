using System;
using System.Threading;
using BestHTTP.Caching;
using BestHTTP.Cookies;
using UnityEngine;

namespace BestHTTP
{
	[ExecuteInEditMode]
	public sealed class HTTPUpdateDelegator : MonoBehaviour
	{
		public static Func<bool> OnBeforeApplicationQuit;

		private static bool IsSetupCalled;

		public static HTTPUpdateDelegator Instance { get; private set; }

		public static bool IsCreated { get; private set; }

		public static bool IsThreaded { get; set; }

		public static bool IsThreadRunning { get; private set; }

		public static int ThreadFrequencyInMS { get; set; }

		static HTTPUpdateDelegator()
		{
			ThreadFrequencyInMS = 100;
		}

		public static void CheckInstance()
		{
			try
			{
				if (!IsCreated)
				{
					GameObject gameObject = GameObject.Find("HTTP Update Delegator");
					if (gameObject != null)
					{
						Instance = gameObject.GetComponent<HTTPUpdateDelegator>();
					}
					if (Instance == null)
					{
						gameObject = new GameObject("HTTP Update Delegator");
						gameObject.hideFlags = HideFlags.HideAndDontSave;
						Instance = gameObject.AddComponent<HTTPUpdateDelegator>();
					}
					IsCreated = true;
				}
			}
			catch
			{
				HTTPManager.Logger.Error("HTTPUpdateDelegator", "Please call the BestHTTP.HTTPManager.Setup() from one of Unity's event(eg. awake, start) before you send any request!");
			}
		}

		private void Setup()
		{
			HTTPCacheService.SetupCacheFolder();
			CookieJar.SetupFolder();
			CookieJar.Load();
			if (IsThreaded)
			{
				ThreadPool.QueueUserWorkItem(ThreadFunc);
			}
			IsSetupCalled = true;
		}

		private void ThreadFunc(object obj)
		{
			HTTPManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Started");
			try
			{
				IsThreadRunning = true;
				while (IsThreadRunning)
				{
					HTTPManager.OnUpdate();
					Thread.Sleep(ThreadFrequencyInMS);
				}
			}
			finally
			{
				HTTPManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Ended");
			}
		}

		private void Update()
		{
			if (!IsSetupCalled)
			{
				IsSetupCalled = true;
				Setup();
			}
			if (!IsThreaded)
			{
				HTTPManager.OnUpdate();
			}
		}

		private void OnDisable()
		{
			OnApplicationQuit();
		}

		private void OnApplicationQuit()
		{
			if (OnBeforeApplicationQuit != null)
			{
				try
				{
					if (!OnBeforeApplicationQuit())
					{
						HTTPManager.Logger.Information("HTTPUpdateDelegator", "OnBeforeApplicationQuit call returned false, postponing plugin shutdown.");
						return;
					}
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("HTTPUpdateDelegator", string.Empty, ex);
				}
			}
			IsThreadRunning = false;
			if (IsCreated)
			{
				IsCreated = false;
				HTTPManager.OnQuit();
			}
		}
	}
}
