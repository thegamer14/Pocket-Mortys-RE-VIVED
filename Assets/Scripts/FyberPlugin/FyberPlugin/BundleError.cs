using System.ComponentModel;

namespace FyberPlugin
{
	public enum BundleError
	{
		[Description("Bundle is not integrated")]
		NOT_INTEGRATED = 0,
		[Description("Credentials are missing")]
		CREDENTIALS_MISSING = 1,
		[Description("Bundle is not enabled from Fyber Dashboard")]
		NETWORK_NOT_ENABLED = 2,
		[Description("Network is not enabled or Bundle credentials are missing")]
		NETWORK_NOT_ENABLED_OR_CREDENTIALS_MISSING = 3
	}
}
