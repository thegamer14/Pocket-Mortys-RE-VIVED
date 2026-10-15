namespace FyberPlugin
{
	public interface RequestCallback : Callback
	{
		void OnAdAvailable(Ad ad);

		void OnAdNotAvailable(AdFormat adFormat);
	}
}
