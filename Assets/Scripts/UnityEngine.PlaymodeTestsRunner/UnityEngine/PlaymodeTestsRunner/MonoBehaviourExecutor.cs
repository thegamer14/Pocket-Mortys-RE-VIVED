using System;
using System.Collections;
using UnityEngine.PlaymodeTests;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class MonoBehaviourExecutor : TestExecutorBase
	{
		public MonoBehaviourExecutor(Type type)
			: base(type.FullName)
		{
			m_Type = type;
		}

		internal override IEnumerator Execute(PlaymodeTestsController controller)
		{
			m_Controller = controller;
			PlayModeTestAttribute attribute = TestAttribute.GetAttribute(m_Type);
			DateTime start = DateTime.Now;
			GameObject go = new GameObject("Test: " + m_Type.Name);
			go.AddComponent(m_Type);
			Coroutine timer = controller.StartCoroutine(StartTimer(null, attribute.GetTimeout(), delegate
			{
				KillGOTest(go);
			}));
			yield return new WaitUntil(IsMBTestFinished);
			m_Duration = (float)(DateTime.Now - start).TotalSeconds;
			Object.DestroyImmediate(go);
			controller.StopCoroutine(timer);
		}

		protected bool IsMBTestFinished()
		{
			CheckLog();
			return m_MBTestFinished;
		}

		public override TestResult GetResult()
		{
			TestResult testResult = new TestResult(name);
			testResult.resultType = (m_TestFailed ? TestResult.ResultType.Failed : TestResult.ResultType.Success);
			testResult.messages = m_CurrentTestMessageBuilder.ToString();
			testResult.stacktrace = m_CurrentTestStacktraceBuilder.ToString();
			testResult.duration = m_Duration;
			return testResult;
		}
	}
}
