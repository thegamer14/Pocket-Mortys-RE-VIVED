namespace UnityEngine.PlaymodeTestsRunner
{
	internal static class TestPlatformEnumExtensions
	{
		public static bool IsFlagIncluded(this TestPlatform flags, TestPlatform flag)
		{
			return (flags & flag) == flag;
		}
	}
}
