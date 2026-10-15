using System;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class TestRunnerFilter
	{
		public string[] names;

		public static TestRunnerFilter empty = new TestRunnerFilter();

		public TestRunnerFilter()
		{
		}

		public TestRunnerFilter(string name)
			: this(new string[1] { name })
		{
		}

		public TestRunnerFilter(string[] names)
		{
			this.names = names;
		}

		public bool Matches(TestResult result)
		{
			return NameMatches(result.fullName);
		}

		public bool Matches(string name)
		{
			return NameMatches(name);
		}

		private bool NameMatches(string name)
		{
			if (names == null || names.Length == 0)
			{
				return true;
			}
			string[] array = names;
			foreach (string value in array)
			{
				if (name.Contains(value))
				{
					return true;
				}
			}
			return false;
		}
	}
}
