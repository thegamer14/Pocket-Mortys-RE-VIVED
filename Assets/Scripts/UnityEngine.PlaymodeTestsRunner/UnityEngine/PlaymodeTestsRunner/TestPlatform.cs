using System;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Flags]
	public enum TestPlatform : ulong
	{
		All = ulong.MaxValue,
		EditMode = 2uL,
		PlayMode = 4uL
	}
}
