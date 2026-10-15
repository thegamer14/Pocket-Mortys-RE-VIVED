using System.Reflection;

namespace FyberPlugin
{
	[Obfuscation(Exclude = true)]
	internal class RequestPayload
	{
		public int? RequestError { get; set; }

		public string PlacementId { get; set; }

		public bool? AdAvailable { get; set; }

		public VirtualCurrencyResponse CurrencyResponse { get; set; }

		public VirtualCurrencyErrorResponse CurrencyErrorResponse { get; set; }
	}
}
