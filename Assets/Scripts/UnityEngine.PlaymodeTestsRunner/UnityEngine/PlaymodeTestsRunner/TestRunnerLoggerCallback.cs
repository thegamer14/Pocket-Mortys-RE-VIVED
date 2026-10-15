using System.Collections.Generic;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class TestRunnerLoggerCallback : MonoBehaviour, TestRunnerListener
	{
		public void RunStarted(string platform, List<string> testsToRun)
		{
			Debug.Log("Starting run");
		}

		public void RunFinished(List<TestResult> testResults)
		{
			Debug.Log("Finishing run");
		}

		public void TestStarted(string testName)
		{
			Debug.Log("Starting test: " + testName);
		}

		public void TestFinished(TestResult test)
		{
			Debug.Log("Finishing test: " + test.name);
		}
	}
}
