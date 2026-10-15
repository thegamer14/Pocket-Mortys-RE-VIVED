using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.PlaymodeTestsRunner
{
	internal class TestResultRenderer
	{
		private static class Styles
		{
			public static readonly GUIStyle SucceedLabelStyle;

			public static readonly GUIStyle FailedLabelStyle;

			public static readonly GUIStyle FailedMessagesStyle;

			static Styles()
			{
				SucceedLabelStyle = new GUIStyle("label");
				SucceedLabelStyle.normal.textColor = Color.green;
				SucceedLabelStyle.fontSize = 48;
				FailedLabelStyle = new GUIStyle("label");
				FailedLabelStyle.normal.textColor = Color.red;
				FailedLabelStyle.fontSize = 32;
				FailedMessagesStyle = new GUIStyle("label");
				FailedMessagesStyle.wordWrap = false;
				FailedMessagesStyle.richText = true;
			}
		}

		private readonly List<TestResult> m_TestCollection = new List<TestResult>();

		private readonly List<TestResult> m_FailedTestCollection = new List<TestResult>();

		private bool m_ShowResults;

		private Vector2 m_ScrollPosition;

		private int m_FailureCount;

		public void ShowResults()
		{
			m_ShowResults = true;
			Cursor.visible = true;
		}

		internal void AddResults(TestResult result)
		{
			m_TestCollection.Add(result);
			if (result.resultType != TestResult.ResultType.Success)
			{
				m_FailureCount++;
				m_FailedTestCollection.Add(result);
			}
		}

		public void Draw()
		{
			if (!m_ShowResults)
			{
				return;
			}
			if (m_FailedTestCollection.Count == 0)
			{
				GUILayout.Label("All test succeeded", Styles.SucceedLabelStyle, GUILayout.Width(600f));
			}
			else
			{
				int count = m_FailedTestCollection.Count;
				GUILayout.Label(count + " tests failed!", Styles.FailedLabelStyle);
				m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUILayout.ExpandWidth(true));
				string text = "";
				text += "<b><size=18>Code-based tests</size></b>\n";
				text += string.Join("\n", m_FailedTestCollection.Select((TestResult result) => string.Concat(result.name, " ", result.resultType, "\n", result.messages)).ToArray());
				GUILayout.TextArea(text, Styles.FailedMessagesStyle);
				GUILayout.EndScrollView();
			}
			if (GUILayout.Button("Close"))
			{
				Application.Quit();
			}
		}

		public int FailureCount()
		{
			return m_FailureCount;
		}
	}
}
