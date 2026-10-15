using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.PlaymodeTestsRunner.TestListBuilder
{
	internal abstract class TestListElement
	{
		protected TestListElement parent;

		protected List<TestListElement> children = new List<TestListElement>();

		public string name;

		public string fullName;

		public string id;

		public TestExecutorBase testExecutor;

		protected TestListElement(string id, string name, string fullName)
		{
			this.id = id;
			this.name = name;
			this.fullName = fullName;
		}

		public virtual TestExecutorBase GetTestExecutor()
		{
			throw new NotImplementedException();
		}

		public void AddChildren(TestListElement child)
		{
			AddChildren(new List<TestListElement> { child });
		}

		public void AddChildren(List<TestListElement> children)
		{
			foreach (TestListElement child in children)
			{
				child.parent = this;
			}
			this.children.AddRange(children);
		}

		public virtual IEnumerable<TestListElement> GetFlattenedHierarchy()
		{
			return children.SelectMany((TestListElement e) => e.GetFlattenedHierarchy());
		}

		public IEnumerable<TestListElement> GetChildren()
		{
			return children;
		}
	}
}
