using System.Diagnostics;
using UnityEngine;

namespace FyberPlugin
{
	public class Utils
	{
		public static void printWarningMessage()
		{
			UnityEngine.Debug.Log("WARNING: Fyber plugin is not available on this platform.");
			UnityEngine.Debug.Log("WARNING: the \"" + GetMethodName() + "\" method does not do anything");
		}

		private static string GetMethodName()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame frame = stackTrace.GetFrame(2);
			return frame.GetMethod().Name;
		}
	}
}
