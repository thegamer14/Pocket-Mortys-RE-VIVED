namespace FyberPlugin
{
	public interface AdCallback
	{
		void OnAdStarted(Ad ad);

		void OnAdFinished(AdResult result);
	}
}
