using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class TestExecutorBase
	{
		public string name;

		public string IdName;

		protected float m_Duration;

		protected PlaymodeTestsController m_Controller;

		protected bool m_MBTestFinished;

		protected bool m_MBTestFailed;

		internal bool m_TestFailed;

		protected StringBuilder m_CurrentTestMessageBuilder = new StringBuilder();

		protected StringBuilder m_CurrentTestStacktraceBuilder = new StringBuilder();

		private bool m_AssertionHappened;

		private bool m_ExceptionHappened;

		protected internal Type m_Type;

		protected internal MethodInfo m_MethodInfo;

		public TestExecutorBase(string name)
		{
			this.name = name;
			IdName = name;
		}

		internal virtual IEnumerator Execute(PlaymodeTestsController controller)
		{
			throw new NotImplementedException();
		}

		protected virtual IEnumerator StartTimer(IEnumerator coroutineToBeKilled, float timeout, Action onTimeout)
		{
			yield return new WaitForSeconds(timeout);
			if (coroutineToBeKilled != null)
			{
				m_Controller.StopCoroutine(coroutineToBeKilled);
			}
			if (onTimeout != null)
			{
				onTimeout();
			}
		}

		public virtual TestResult GetResult()
		{
			throw new NotImplementedException();
		}

		public bool IsIncluded(TestRunnerFilter filter)
		{
			return filter == null || filter.Matches(name);
		}

		protected void KillGOTest(GameObject go)
		{
			if (go != null)
			{
				Object.DestroyImmediate(go);
			}
			FailPlaymodeTest();
		}

		internal void FailPlaymodeTest()
		{
			m_MBTestFinished = true;
			m_MBTestFailed = true;
		}

		public virtual void OnPlaymodePassCall()
		{
			m_MBTestFinished = true;
			m_MBTestFailed = false;
		}

		public virtual void OnPlaymodeFailCall()
		{
			m_MBTestFinished = true;
			m_MBTestFailed = true;
		}

		internal void CheckLog()
		{
			if (m_ExceptionHappened || m_AssertionHappened)
			{
				m_TestFailed = true;
			}
			if (m_MBTestFailed)
			{
				m_TestFailed = true;
			}
		}

		public void OnLogCallback(string logString, string stackTrace, LogType type)
		{
			switch (type)
			{
			case LogType.Exception:
				m_ExceptionHappened = true;
				m_CurrentTestMessageBuilder.AppendLine(logString);
				m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
				break;
			case LogType.Assert:
				m_AssertionHappened = true;
				m_CurrentTestMessageBuilder.AppendLine(logString);
				m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
				break;
			case LogType.Error:
				m_ExceptionHappened = true;
				m_CurrentTestMessageBuilder.AppendLine(logString);
				m_CurrentTestStacktraceBuilder.AppendLine(stackTrace);
				break;
			case LogType.Log:
				m_CurrentTestMessageBuilder.AppendLine(logString);
				break;
			case LogType.Warning:
				break;
			}
		}
	}
}
