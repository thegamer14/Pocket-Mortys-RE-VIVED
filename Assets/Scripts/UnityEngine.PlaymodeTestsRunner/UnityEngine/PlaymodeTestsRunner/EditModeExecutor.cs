using System;
using System.Collections;
using System.Reflection;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class EditModeExecutor : TestExecutorBase
	{
		private object m_Instance;

		public EditModeExecutor(Type type, MethodInfo methodInfo)
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
			DateTime now = DateTime.Now;
			if (m_MethodInfo.ReturnType == typeof(void))
			{
				return HandleVoidTest(m_Instance, m_MethodInfo);
			}
			if (m_MethodInfo.ReturnType == typeof(IEnumerator))
			{
				return HandleEnumerableTest(m_Instance, m_MethodInfo);
			}
			Debug.Log(string.Concat("Return type ", m_MethodInfo.ReturnType, " of ", m_MethodInfo.Name, " in ", m_Type.FullName, " is not supported."));
			m_Duration = (float)(DateTime.Now - now).TotalSeconds;
			return null;
		}

		private IEnumerator HandleVoidTest(object instance, MethodInfo m)
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
			yield return null;
		}

		public override TestResult GetResult()
		{
			CheckLog();
			TestResult testResult = new TestResult(IdName);
			testResult.name = name;
			testResult.fullName = name;
			testResult.resultType = (m_TestFailed ? TestResult.ResultType.Failed : TestResult.ResultType.Success);
			testResult.messages = m_CurrentTestMessageBuilder.ToString();
			testResult.stacktrace = m_CurrentTestStacktraceBuilder.ToString();
			testResult.duration = m_Duration;
			return testResult;
		}

		private IEnumerator HandleEnumerableTest(object instance, MethodInfo m)
		{
			return m.Invoke(instance, null) as IEnumerator;
		}
	}
}
