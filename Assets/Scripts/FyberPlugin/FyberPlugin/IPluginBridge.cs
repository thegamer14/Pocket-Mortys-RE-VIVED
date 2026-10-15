namespace FyberPlugin
{
	public interface IPluginBridge
	{
		void StartSDK(string json);

		bool Cache(string action);

		void Request(string json);

		void StartAd(string json);

		bool Banner(string json);

		void Report(string json);

		string Settings(string json);

		void EnableLogging(bool shouldLog);

		void GameObjectStarted();

		void ApplicationQuit();

		void ShowTestSuite();

		void AnalyzeTestSuite();
	}
}
