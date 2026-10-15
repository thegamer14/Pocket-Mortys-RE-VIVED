using System.Collections.Generic;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal interface TestRunnerListener
	{
		void RunStarted(string platform, List<string> testsToRun);

		void RunFinished(List<TestResult> testResults);

		void TestStarted(string testName);

		void TestFinished(TestResult test);
	}
}
