using System;
using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class TestResult
	{
		[Serializable]
		public enum ResultType
		{
			Success = 0,
			Failed = 1,
			NotRun = 2
		}

		public string id;

		public string name;

		public string fullName;

		public ResultType resultType = ResultType.NotRun;

		public float duration;

		public string messages;

		public string stacktrace;

		protected Action<TestResult> m_OnResultUpdate;

		internal TestResult(TestListElement test)
		{
			id = test.id;
			fullName = test.fullName;
			name = test.name;
		}

		public TestResult(string id)
		{
			this.id = id;
		}

		public void Update(TestResult result)
		{
			resultType = result.resultType;
			messages = result.messages;
			stacktrace = result.stacktrace;
			if (m_OnResultUpdate != null)
			{
				m_OnResultUpdate(this);
			}
		}

		public void SetResultChangedCallback(Action<TestResult> resultUpdated)
		{
			m_OnResultUpdate = resultUpdated;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", name, fullName);
		}
	}
}
