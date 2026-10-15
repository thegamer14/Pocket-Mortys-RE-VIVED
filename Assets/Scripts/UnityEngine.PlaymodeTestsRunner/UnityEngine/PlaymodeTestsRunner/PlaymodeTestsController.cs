using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.PlaymodeTestsRunner.TestListBuilder;
using UnityEngine.SceneManagement;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class PlaymodeTestsController : MonoBehaviour
	{
		[SerializeField]
		internal TestStartedEvent testStartedEvent = new TestStartedEvent();

		[SerializeField]
		internal TestFinishedEvent testFinishedEvent = new TestFinishedEvent();

		[SerializeField]
		internal RunStartedEvent runStartedEvent = new RunStartedEvent();

		[SerializeField]
		internal RunFinishedEvent runFinishedEvent = new RunFinishedEvent();

		internal const string kPlaymodeTestControllerName = "Code-based tests runner";

		[SerializeField]
		public PlaymodeTestsControllerSettings settings = new PlaymodeTestsControllerSettings();

		private TestExecutorBase m_TestExecutor;

		public IEnumerator Start()
		{
			yield return null;
			yield return null;
			StartCoroutine(Run());
		}

		internal static bool IsControllerOnScene()
		{
			return GameObject.Find("Code-based tests runner") != null;
		}

		internal static PlaymodeTestsController GetController()
		{
			return GameObject.Find("Code-based tests runner").GetComponent<PlaymodeTestsController>();
		}

		private void HandleLogCallback(string logString, string stackTrace, LogType type)
		{
			if (m_TestExecutor != null)
			{
				m_TestExecutor.OnLogCallback(logString, stackTrace, type);
			}
		}

		public IEnumerator Run()
		{
			Application.logMessageReceived += HandleLogCallback;
			runStartedEvent.Invoke(Application.platform.ToString(), null);
			if (settings.sceneBased)
			{
				SceneManager.LoadScene(1, LoadSceneMode.Additive);
				yield return null;
			}
			TestListUtil testListUtil = new TestListUtil(false);
			IEnumerable<TestExecutorBase> tests = from testListElement in testListUtil.GetPlaymodeTests(settings.filter).GetFlattenedHierarchy()
				select testListElement.testExecutor;
			if (settings.filter != null)
			{
				tests = tests.Where((TestExecutorBase e) => settings.filter.Matches(e.name)).ToList();
			}
			foreach (TestExecutorBase item in tests)
			{
				TestExecutorBase t = (m_TestExecutor = item);
				testStartedEvent.Invoke(t.name);
				yield return t.Execute(this);
				TestResult result = t.GetResult();
				testFinishedEvent.Invoke(result);
			}
			runFinishedEvent.Invoke(null);
		}

		internal void PassPlaymodeTest()
		{
			m_TestExecutor.OnPlaymodePassCall();
		}

		internal void FailPlaymodeTest()
		{
			m_TestExecutor.OnPlaymodeFailCall();
		}
	}
}
