using System.Collections.Generic;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class ResultsRenderer : MonoBehaviour, TestRunnerListener
	{
		private TestResultRenderer resultRenderer = new TestResultRenderer();

		private List<TestResult> results = new List<TestResult>();

		public void RunStarted(string platform, List<string> testsToRun)
		{
		}

		public void RunFinished(List<TestResult> testResults)
		{
			resultRenderer.ShowResults();
		}

		public void TestStarted(string testName)
		{
		}

		public void TestFinished(TestResult test)
		{
			Debug.Log("Test run finished");
			resultRenderer.AddResults(test);
			results.Add(test);
		}

		public void OnGUI()
		{
			resultRenderer.Draw();
		}
	}
}
