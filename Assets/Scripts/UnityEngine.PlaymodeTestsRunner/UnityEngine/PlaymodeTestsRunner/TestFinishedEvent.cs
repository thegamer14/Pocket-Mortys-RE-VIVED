using System;
using UnityEngine.Events;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class TestFinishedEvent : UnityEvent<TestResult>
	{
	}
}
