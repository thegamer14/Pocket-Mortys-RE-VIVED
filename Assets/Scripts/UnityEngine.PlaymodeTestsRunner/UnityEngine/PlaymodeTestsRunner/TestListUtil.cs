using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.PlaymodeTests;
using UnityEngine.PlaymodeTestsRunner.TestListBuilder;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class TestListUtil
	{
		private string[] DefaultUserAssemblies = new string[8] { "Assembly-CSharp", "Assembly-UnityScript", "Assembly-CSharp-testable", "Assembly-UnityScript-testable", "Assembly-CSharp-firstpass", "Assembly-UnityScript-firstpass", "Assembly-CSharp-firstpass-testable", "Assembly-UnityScript-firstpass-testable" };

		private string[] DefaultUserEditorAssemblies = new string[8] { "Assembly-CSharp-Editor", "Assembly-UnityScript-Editor", "Assembly-CSharp-Editor-testable", "Assembly-UnityScript-Editor-testable", "Assembly-CSharp-Editor-firstpass", "Assembly-UnityScript-Editor-firstpass", "Assembly-CSharp-Editor-firstpass-testable", "Assembly-UnityScript-Editor-firstpass-testable" };

		private bool isEditModeRunner;

		private TestListUtil()
			: this(false)
		{
		}

		public TestListUtil(bool isEditModeRunner)
		{
			this.isEditModeRunner = isEditModeRunner;
		}

		public TestListElement GetPlaymodeTests()
		{
			return GetPlaymodeTests(null);
		}

		public TestListElement GetPlaymodeTests(TestRunnerFilter filter)
		{
			return GetTests(TestPlatform.PlayMode);
		}

		public TestListElement GetEditmodeTests()
		{
			return GetEditmodeTests(null);
		}

		public TestListElement GetEditmodeTests(TestRunnerFilter filter)
		{
			return GetTests(TestPlatform.EditMode);
		}

		private TestListElement GetTests(TestPlatform testPlatform)
		{
			IEnumerable<Assembly> userAssemblies = GetUserAssemblies(testPlatform.IsFlagIncluded(TestPlatform.EditMode));
			IEnumerable<Type> typesFromAsseblies = GetTypesFromAsseblies(testPlatform, userAssemblies);
			return ParseTypesForTests(testPlatform, typesFromAsseblies);
		}

		internal virtual IEnumerable<Type> GetTypesFromAsseblies(TestPlatform editMode, IEnumerable<Assembly> assemblies)
		{
			return assemblies.SelectMany((Assembly a) => a.GetTypes()).Where(IsTypeATestForPlatform);
		}

		private TestListElement ParseTypesForTests(TestPlatform testPlatform, IEnumerable<Type> types)
		{
			TestListGroup testListGroup = new TestListGroup("Root", "Root", "Root");
			TestsConstraint testsConstraint = testPlatform.GetTestsConstraint();
			foreach (Type type in types)
			{
				if (testsConstraint.IsClassTestSupported() && testsConstraint.IsClassATest(type))
				{
					TestListItem testListItem = new TestListItem(type.FullName, type.Name, type.FullName);
					testListItem.name = "";
					testListItem.fullName = "";
					testListItem.testExecutor = new MonoBehaviourExecutor(type);
					testListGroup.AddChildren(testListItem);
				}
				if (!testsConstraint.IsMethodTestSupported())
				{
					continue;
				}
				IEnumerable<MethodInfo> testMethodList = GetTestMethodList(testPlatform, type);
				TestListGroup testListGroup2 = new TestListGroup(type.FullName, type.Name, type.FullName);
				foreach (MethodInfo item in testMethodList)
				{
					string text = type.FullName + "+" + item.Name;
					TestListItem testListItem2 = new TestListItem(text, item.Name, text);
					if (isEditModeRunner)
					{
						testListItem2.testExecutor = new EditModeExecutor(type, item);
					}
					else
					{
						testListItem2.testExecutor = new MethodExecutor(type, item);
					}
					testListGroup2.AddChildren(testListItem2);
				}
				if (testMethodList.Any())
				{
					testListGroup.AddChildren(testListGroup2);
				}
			}
			return testListGroup;
		}

		private IEnumerable<MethodInfo> GetTestMethodList(TestPlatform testPlatform, Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return methods.Where(delegate(MethodInfo m)
			{
				if (m.GetCustomAttributes(typeof(TestAttribute), true).Length == 0)
				{
					return false;
				}
				TestAttribute testAttribute = (TestAttribute)m.GetCustomAttributes(typeof(TestAttribute), true)[0];
				if (!testAttribute.IncludeOnPlatform(testPlatform))
				{
					return false;
				}
				TestsConstraint testsConstraint = testPlatform.GetTestsConstraint();
				return testsConstraint.IsMethodATest(m) ? true : false;
			});
		}

		private bool IsTypeATestForPlatform(Type type)
		{
			if (type.GetCustomAttributes(typeof(TestAttribute), true).Any())
			{
				return true;
			}
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (methods.Any((MethodInfo m) => m.GetCustomAttributes(typeof(TestAttribute), true).Any()))
			{
				return true;
			}
			return false;
		}

		internal virtual IEnumerable<Assembly> GetUserAssemblies(bool includeEditorAssemblies)
		{
			List<string> assemblyList = DefaultUserAssemblies.ToList();
			if (includeEditorAssemblies)
			{
				assemblyList.AddRange(DefaultUserEditorAssemblies);
			}
			foreach (string userAssembly in assemblyList)
			{
				Assembly a;
				try
				{
					a = Assembly.Load(userAssembly);
				}
				catch (FileNotFoundException)
				{
					continue;
				}
				if (a != null)
				{
					yield return a;
				}
			}
		}
	}
}
