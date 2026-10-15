using System;
using System.IO;
using System.Reflection;
using UnityEngine.PlaymodeTestsRunner;

namespace UnityEngine.PlaymodeTests
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class TestAttribute : Attribute
	{
		internal const float kDefaultTimeout = 5f;

		private string m_SceneName;

		private float m_Timeout = 5f;

		private TestPlatform m_TestPlatform = TestPlatform.All;

		public TestAttribute()
		{
		}

		public TestAttribute(string scene)
			: this(TestPlatform.All, scene, 5f)
		{
		}

		public TestAttribute(float timeout)
			: this(TestPlatform.All, null, timeout)
		{
		}

		public TestAttribute(TestPlatform testPlatform)
			: this(testPlatform, null, 5f)
		{
		}

		public TestAttribute(TestPlatform testPlatform, string scene, float timeout)
		{
			m_TestPlatform = testPlatform;
			m_Timeout = timeout;
			if (!string.IsNullOrEmpty(scene))
			{
				if (scene.EndsWith(".unity"))
				{
					scene = scene.Substring(0, scene.Length - ".unity".Length);
				}
				m_SceneName = scene;
			}
		}

		internal float GetTimeout()
		{
			return m_Timeout;
		}

		internal bool IncludeOnPlatform(TestPlatform testPlatform)
		{
			return (m_TestPlatform & testPlatform) == testPlatform;
		}

		internal bool IncludeOnScene(string sceneName)
		{
			if (string.IsNullOrEmpty(sceneName))
			{
				return false;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sceneName);
			return m_SceneName == fileNameWithoutExtension;
		}

		internal bool IsSceneBased()
		{
			return !string.IsNullOrEmpty(m_SceneName);
		}

		internal static PlayModeTestAttribute GetAttribute(MethodInfo method)
		{
			if (!Attribute.IsDefined(method, typeof(PlayModeTestAttribute)))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(method, typeof(PlayModeTestAttribute)) as PlayModeTestAttribute;
		}

		internal static PlayModeTestAttribute GetAttribute(Type type)
		{
			if (!Attribute.IsDefined(type, typeof(PlayModeTestAttribute)))
			{
				return null;
			}
			return Attribute.GetCustomAttribute(type, typeof(PlayModeTestAttribute)) as PlayModeTestAttribute;
		}
	}
}
