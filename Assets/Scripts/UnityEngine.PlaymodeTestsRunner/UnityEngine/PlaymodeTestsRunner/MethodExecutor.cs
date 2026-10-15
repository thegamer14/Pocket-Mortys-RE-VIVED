using System;
using System.Collections;
using System.Reflection;
using UnityEngine.PlaymodeTests;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class MethodExecutor : TestExecutorBase
	{
		private object m_Instance;

		private bool m_Running;

		private bool m_IsFrameFinished;

		public MethodExecutor(Type type, MethodInfo methodInfo)
			: base(type.FullName + "+" + methodInfo.Name)
		{
			m_Type = type;
			m_MethodInfo = methodInfo;
		}

		private object GetInstanceOfTestObject(MethodInfo m)
		{
			if (m.DeclaringType.IsSubclassOf(typeof(MonoBehaviour)))
			{
				return m_Controller.gameObject.AddComponent(m.DeclaringType);
			}
			return Activator.CreateInstance(m.DeclaringType);
		}

		internal override IEnumerator Execute(PlaymodeTestsController controller)
		{
			m_Controller = controller;
			m_Instance = GetInstanceOfTestObject(m_MethodInfo);
			DateTime start = DateTime.Now;
			if (m_MethodInfo.ReturnType == typeof(void))
			{
				HandleVoidTest(m_Instance, m_MethodInfo);
				yield return null;
			}
			else if (m_MethodInfo.ReturnType == typeof(IEnumerator))
			{
				try
				{
					yield return HandleEnumerableTest(m_Instance, m_MethodInfo, m_Type);
				}
				finally
				{
					if (!m_IsFrameFinished)
					{
						Debug.Log("Test didn't finish OK. Exception?");
					}
					m_IsFrameFinished = false;
				}
			}
			else
			{
				Debug.Log(string.Concat("Return type ", m_MethodInfo.ReturnType, " of ", m_MethodInfo.Name, " in ", m_Type.FullName, " is not supported."));
			}
			m_Duration = (float)(DateTime.Now - start).TotalSeconds;
		}

		private void HandleVoidTest(object instance, MethodInfo m)
		{
			try
			{
				m.Invoke(instance, null);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			CheckLog();
		}

		public override TestResult GetResult()
		{
			CheckLog();
			TestResult testResult = new TestResult(IdName);
			testResult.resultType = (m_TestFailed ? TestResult.ResultType.Failed : TestResult.ResultType.Success);
			testResult.messages = m_CurrentTestMessageBuilder.ToString();
			testResult.stacktrace = m_CurrentTestStacktraceBuilder.ToString();
			testResult.duration = m_Duration;
			return testResult;
		}

		private IEnumerator HandleEnumerableTest(object instance, MethodInfo m, Type type)
		{
			float suiteTimeout = TestAttribute.GetAttribute(type).GetTimeout();
			do
			{
				m_IsFrameFinished = false;
				if (!m_Running)
				{
					m_Running = true;
					float num = TestAttribute.GetAttribute(m).GetTimeout();
					if (num == 5f)
					{
						num = suiteTimeout;
					}
					m_Controller.StartCoroutine(ExMethod(m.Invoke(instance, null) as IEnumerator, num));
				}
				CheckLog();
				m_IsFrameFinished = true;
				if (m_TestFailed)
				{
					break;
				}
				yield return null;
			}
			while (m_Running);
		}

		private IEnumerator ExMethod(IEnumerator e, float timeout)
		{
			m_Running = true;
			Coroutine timer = m_Controller.StartCoroutine(StartTimer(e, timeout, delegate
			{
				m_TestFailed = true;
				m_Running = false;
			}));
			yield return m_Controller.StartCoroutine(e);
			m_Controller.StopCoroutine(timer);
			m_Running = false;
			yield return null;
		}
	}
}
