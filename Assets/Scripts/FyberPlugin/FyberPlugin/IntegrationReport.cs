using System.Collections.Generic;

namespace FyberPlugin
{
	public class IntegrationReport
	{
		public string TestSuiteVersion { get; set; }

		public string SdkVersion { get; set; }

		public string AppId { get; set; }

		public string UserId { get; set; }

		public CookiePolicy CookiePolicy { get; set; }

		public List<MediationBundleInfo> StartedBundles { get; set; }

		public List<MediationBundleInfo> UnstartedBundles { get; set; }

		public override string ToString()
		{
			string text = "TestSuiteVersion: " + TestSuiteVersion + "\nSdkVersion: " + SdkVersion + "\nAppId: " + AppId + "\nUserId: " + UserId;
			text = text + "\nCookiePolicy: " + CookiePolicy;
			text += "\nStartedBundles:";
			foreach (MediationBundleInfo startedBundle in StartedBundles)
			{
				text = text + "\n" + startedBundle.ToString();
			}
			text += "\nUnstartedBundles:";
			foreach (MediationBundleInfo unstartedBundle in UnstartedBundles)
			{
				text = text + "\n" + unstartedBundle.ToString();
			}
			return text;
		}
	}
}
