namespace FyberPlugin
{
	public class MediationBundleInfo
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public BundleError? BundleError { get; set; }

		public override string ToString()
		{
			return "\tName: " + Name + "\n\tVersion: " + Version + ((!BundleError.HasValue) ? string.Empty : ("\n\tBundleError: " + BundleError.ToString()));
		}
	}
}
