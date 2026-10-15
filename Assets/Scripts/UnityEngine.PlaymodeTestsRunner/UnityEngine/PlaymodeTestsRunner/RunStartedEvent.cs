using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class RunStartedEvent : UnityEvent<string, List<string>>
	{
	}
}
