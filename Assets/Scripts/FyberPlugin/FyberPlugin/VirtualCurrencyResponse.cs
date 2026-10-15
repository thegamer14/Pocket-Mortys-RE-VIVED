namespace FyberPlugin
{
	public class VirtualCurrencyResponse
	{
		public double DeltaOfCoins { get; set; }

		public string LatestTransactionId { get; set; }

		public string CurrencyId { get; set; }

		public string CurrencyName { get; set; }

		public bool DefaultCurrency { get; set; }
	}
}
