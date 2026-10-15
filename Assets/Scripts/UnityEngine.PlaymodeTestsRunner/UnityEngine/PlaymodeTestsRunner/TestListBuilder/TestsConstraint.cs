using System;
using System.Reflection;

namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
	internal abstract class TestsConstraint
	{
		public virtual bool IsMethodATest(MethodInfo method)
		{
			return false;
		}

		public virtual bool IsClassATest(Type type)
		{
			return false;
		}

		public virtual bool IsClassTestSupported()
		{
			return false;
		}

		public virtual bool IsMethodTestSupported()
		{
			return false;
		}
	}
}
