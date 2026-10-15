using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class NativeMessage
	{
		public int Type { get; set; }

		public string Id { get; set; }

		public int Origin { get; set; }

		public RequestPayload RequestPayload { get; set; }

		public AdPayload AdPayload { get; set; }

		public BannerAdPayload BannerAdPayload { get; set; }

		public TestSuitePayload TestSuitePayload { get; set; }
	}
}
