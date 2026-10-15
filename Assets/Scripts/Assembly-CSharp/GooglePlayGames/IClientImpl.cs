using GooglePlayGames.Native.PInvoke;

namespace GooglePlayGames
{
	internal interface IClientImpl
	{
		PlatformConfiguration CreatePlatformConfiguration();

		TokenClient CreateTokenClient(bool reset);
	}
}
