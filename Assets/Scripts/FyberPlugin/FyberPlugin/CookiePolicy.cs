using System.ComponentModel;

namespace FyberPlugin
{
	public enum CookiePolicy
	{
		[Description("Accept all")]
		ACCEPT_ALL = 0,
		[Description("Accept none")]
		ACCEPT_NONE = 1,
		[Description("Accept original server")]
		ACCEPT_ORIGINAL_SERVER = 2
	}
}
