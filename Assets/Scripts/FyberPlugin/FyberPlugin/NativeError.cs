using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class NativeError
	{
		public string Error { get; set; }

		public string Id { get; set; }
	}
}
