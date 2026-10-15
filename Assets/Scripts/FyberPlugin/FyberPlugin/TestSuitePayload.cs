using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class TestSuitePayload
	{
		public AnalysisError? AnalysisError { get; set; }

		public IntegrationReport Success { get; set; }
	}
}
