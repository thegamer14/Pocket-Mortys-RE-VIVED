public class NotificationDefs
{
	public const string kAlertTutorialUseMPadID = "AlertTutorialUseMPadID";

	public const string kAlertTutorialUseItemID = "AlertTutorialUseItemIDKey";

	public const string kAlertTutorialFindRickID = "AlertTutorialFindRickIDKey";

	public const string kAlertTutorialGoThroughPortalID = "AlertTutorialGoThroughPortalIDKey";

	public const string kAlertCitadelFirstTimeID = "AlertCitadelFirstTimeID";

	public const string kAlertFirstSegmentedWorldID = "AlertFirstSegmentedWorldID";

	public const string kAlertSegmentedFirstCouncilMemberID = "AlertSegmentedFirstCouncilMemberID";

	public const string kAlertCitadelFirstCouncilMemberID = "AlertCitadelFirstCouncilMemberID";

	public const string kAlertBadgesRequiredID = "AlertBadgesRequiredID";

	public const string kAlertCouncilMemberReadyID = "AlertCouncilMemberReadyID";

	public const string kAlertEndGameID = "AlertEndGameID";

	public const string kInfoCouponID = "InfoCouponKey";

	public const string kInfoCombineID = "InfoCombineKey";

	public const string kInfoUnlockMortyLabsID = "InfoUnlockMortyLabsKey";

	public const string kInfoUnlockTournamentsID = "InfoUnlockTournamentsKey";

	public static string GetText(string id)
	{
		string result = null;
		switch (id)
		{
		case "AlertTutorialUseMPadID":
			result = TextDefs.NOTIFICATIONS_ALERT_1;
			break;
		case "AlertTutorialUseItemIDKey":
			result = string.Format(TextDefs.NOTIFICATIONS_ALERT_2, ItemDefs.Name("ItemSerum"));
			break;
		case "AlertTutorialFindRickIDKey":
			result = string.Format(TextDefs.NOTIFICATIONS_ALERT_3, TrainerDefs.Name("TrainerTutorialGymLeader"));
			break;
		case "AlertTutorialGoThroughPortalIDKey":
			result = TextDefs.NOTIFICATIONS_ALERT_4;
			break;
		case "AlertCitadelFirstTimeID":
			result = TextDefs.NOTIFICATIONS_ALERT_5;
			break;
		case "AlertFirstSegmentedWorldID":
			result = TextDefs.NOTIFICATIONS_ALERT_6;
			break;
		case "AlertSegmentedFirstCouncilMemberID":
			result = TextDefs.NOTIFICATIONS_ALERT_7;
			break;
		case "AlertCitadelFirstCouncilMemberID":
			result = TextDefs.NOTIFICATIONS_ALERT_8;
			break;
		case "AlertBadgesRequiredID":
			result = string.Format(TextDefs.NOTIFICATIONS_ALERT_9, PlayerDefs.CurrentBadgesRequired.ToString());
			break;
		case "AlertCouncilMemberReadyID":
			result = TextDefs.NOTIFICATIONS_ALERT_10;
			break;
		case "AlertEndGameID":
			result = TextDefs.NOTIFICATIONS_ALERT_11;
			break;
		case "InfoCombineKey":
			result = TextDefs.NOTIFICATIONS_INFO_1;
			break;
		case "InfoCouponKey":
			result = TextDefs.NOTIFICATIONS_INFO_2;
			break;
		case "InfoUnlockMortyLabsKey":
			result = TextDefs.NOTIFICATIONS_INFO_3;
			break;
		case "InfoUnlockTournamentsKey":
			result = TextDefs.NOTIFICATIONS_INFO_4;
			break;
		}
		return result;
	}
}
