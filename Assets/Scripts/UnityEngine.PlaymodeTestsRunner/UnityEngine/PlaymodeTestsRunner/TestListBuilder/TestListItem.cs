using System.Collections.Generic;

namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
	internal class TestListItem : TestListElement
	{
		public TestListItem(string id, string name, string fullName)
			: base(id, name, fullName)
		{
		}

		public override IEnumerable<TestListElement> GetFlattenedHierarchy()
		{
			return new TestListItem[1] { this };
		}
	}
}
