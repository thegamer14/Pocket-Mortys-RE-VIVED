using System;
using UnityEngine.PlaymodeTestsRunner;

namespace UnityEngine.PlaymodeTests
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class EditModeTestAttribute : TestAttribute
	{
		public EditModeTestAttribute()
			: base(TestPlatform.EditMode)
		{
		}
	}
}
