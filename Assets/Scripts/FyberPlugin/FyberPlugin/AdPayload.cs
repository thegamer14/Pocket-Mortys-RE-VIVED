using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class AdPayload
	{
		public bool AdStarted { get; set; }

		public string Error { get; set; }

		public string Status { get; set; }
	}
}
