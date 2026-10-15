namespace FyberPlugin
{
	public interface VirtualCurrencyCallback : Callback
	{
		void OnError(VirtualCurrencyErrorResponse response);

		void OnSuccess(VirtualCurrencyResponse response);
	}
}
