using System;
using System.Text.RegularExpressions;

namespace FyberPlugin
{
	public sealed class RewardedActionReporter : Reporter<RewardedActionReporter>
	{
		private const string ACTION_ID = "actionId";

		private const string ACTION_ID_PATTERN = "^[A-Z0-9_]+$";

		private RewardedActionReporter(string appId, string actionId)
		{
			ValidateAppId(appId);
			ValidateActionId(actionId);
			nativeDict["appId"] = appId;
			nativeDict["actionId"] = actionId;
		}

		private void ValidateActionId(string actionId)
		{
			if (string.IsNullOrEmpty(actionId))
			{
				throw new ArgumentException("An action ID cannot be null or empty.");
			}
			if (!Regex.IsMatch(actionId, "^[A-Z0-9_]+$"))
			{
				throw new ArgumentException("An action ID can only contain uppercase letters, numbers and the _ underscore symbol.");
			}
		}

		public static RewardedActionReporter Create(string appId, string actionId)
		{
			return new RewardedActionReporter(appId, actionId);
		}

		protected override RewardedActionReporter GetThis()
		{
			return this;
		}
	}
}
