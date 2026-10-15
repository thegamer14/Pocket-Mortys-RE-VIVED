using System;

namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
	internal static class TestsConstraintsExtensions
	{
		internal static TestsConstraint GetTestsConstraint(this TestPlatform testPlatform)
		{
			switch (testPlatform)
			{
			case TestPlatform.EditMode:
				return new EditModeTestsConstraints();
			case TestPlatform.PlayMode:
				return new PlayModeTestsConstraints();
			default:
				throw new NotImplementedException();
			}
		}
	}
}
