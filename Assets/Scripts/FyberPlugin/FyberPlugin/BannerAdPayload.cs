using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class BannerAdPayload
	{
		public int? Event { get; set; }

		public string ErrorMessage { get; set; }
	}
}
