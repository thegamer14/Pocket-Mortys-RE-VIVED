namespace FyberPlugin
{
	public class VirtualCurrencyErrorResponse
	{
		public enum ErrorType
		{
			ERROR_INVALID_RESPONSE = 0,
			ERROR_INVALID_RESPONSE_SIGNATURE = 1,
			SERVER_RETURNED_ERROR = 2,
			ERROR_OTHER = 3
		}

		public ErrorType Type { get; set; }

		public string Code { get; set; }

		public string Message { get; set; }
	}
}
