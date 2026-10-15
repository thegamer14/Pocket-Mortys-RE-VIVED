using System;
using UnityEngine.Events;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class TestStartedEvent : UnityEvent<string>
	{
	}
}
