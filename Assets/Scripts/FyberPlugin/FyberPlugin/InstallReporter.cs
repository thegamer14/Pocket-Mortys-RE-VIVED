namespace FyberPlugin
{
	public sealed class InstallReporter : Reporter<InstallReporter>
	{
		private InstallReporter(string appId)
		{
			ValidateAppId(appId);
			nativeDict["appId"] = appId;
		}

		public static InstallReporter Create(string appId)
		{
			return new InstallReporter(appId);
		}

		protected override InstallReporter GetThis()
		{
			return this;
		}
	}
}
