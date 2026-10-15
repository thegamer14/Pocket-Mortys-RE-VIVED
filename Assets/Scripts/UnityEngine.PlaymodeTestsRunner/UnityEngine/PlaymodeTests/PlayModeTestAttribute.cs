using System;
using UnityEngine.PlaymodeTestsRunner;

namespace UnityEngine.PlaymodeTests
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class PlayModeTestAttribute : TestAttribute
	{
		public PlayModeTestAttribute()
			: base(TestPlatform.PlayMode)
		{
		}
	}
}
