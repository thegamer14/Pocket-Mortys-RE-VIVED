namespace FyberPlugin
{
	public sealed class FyberLogger
	{
		public static void EnableLogging(bool shouldLog)
		{
			PluginBridge.EnableLogging(shouldLog);
		}
	}
}
