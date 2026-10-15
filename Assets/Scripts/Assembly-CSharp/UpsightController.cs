using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine;

public class UpsightController : MonoBehaviour
{
	private const string UPSIGHT_IOS_TOKEN = "ead0e39df0ea41b9a7d40ea9d2b3f370";

	private const string UPSIGHT_IOS_SECRET = "a7202a99072b4efeba70ac24b71b64ff";

	private const string UPSIGHT_ANDROID_GAME_ID = "843889502734";

	private const string UPSIGHT_ANDROID_TOKEN = "a448be245fb64df797104a4d77681533";

	private const string UPSIGHT_ANDROID_SECRET = "ff875d6a94314438a7f4581b1236b3f5";

	public const string ANALYTIC_NAME_GET_COINS = "get_schmeckles";

	public const string ANALYTIC_NAME_GAME_CENTER = "game_center";

	public const string ANALYTIC_NAME_STORE_MENU = "store_menu";

	public const string ANALYTIC_NAME_SETTINGS_MENU = "settings_menu";

	public const string ANALYTIC_NAME_VISIT_COUPON_MACHINE = "visit_coupon_machine";

	public const string ANALYTIC_NAME_VIEW_IAP = "view_IAP";

	public const string ANALYTIC_NAME_USE_HEALING_CENTER = "use_healing_center";

	public const string ANALYTIC_NAME_CAMPAIGN = "campaign_mode";

	public const string ANALYTIC_NAME_MULTIPLAYER = "multiplayer_mode";

	public const string ANALYTIC_NAME_REGISTER_AVATAR = "picked_avatar";

	public const string ANALYTIC_NAME_REGISTER_USERNAME = "picked_username";

	public const string ANALYTIC_NAME_REGISTER_MORTIES = "selected_mortys";

	public const string ANALYTIC_NAME_REGISTER_COMPLETE = "onboarding_process_complete";

	public const string ANALYTIC_NAME_LOST_RICK_BATTLE = "lost_rick_battle";

	public const string ANALYTIC_NAME_WON_RICK_BATTLE = "won_rick_battle";

	public const string ANALYTIC_NAME_LOST_COUNCIL_BATTLE = "lost_council_battle";

	public const string ANALYTIC_NAME_WON_COUNCIL_BATTLE = "won_council_battle";

	public const string ANALYTIC_NAME_DEPOSITED_MORTY = "deposited_morty";

	public const string ANALYTIC_NAME_WITHDRAW_MORTY = "withdraw_morty";

	public const string ANALYTIC_NAME_COMBINED_MORTY = "combined_morty";

	public const string ANALYTIC_NAME_IAP_1_COUPON = "1_coupon";

	public const string ANALYTIC_NAME_IAP_5_COUPON = "5_coupon";

	public const string ANALYTIC_NAME_IAP_15_COUPON = "15_coupon";

	public const string ANALYTIC_NAME_IAP_40_COUPON = "40_coupon";

	public const string ANALYTIC_NAME_IAP_125_COUPON = "125_coupon";

	public const string ANALYTIC_NAME_IAP_300_COUPON = "300_coupon";

	public const string ANALYTIC_NAME_WATCH_VIDEO_AD = "watch_video_ad";

	public const string ANALYTIC_NAME_WATCH_VIDEO_AD_PLACEMENT = "watch_video_ad_{0}";

	public const string ANALYTIC_NAME_BATTLE_OVER = "battle_over";

	public const string ANALYTIC_NAME_PLAYER_LEVEL = "player_level";

	public const string ANALYTIC_NAME_WON_BATTLE = "won_battle";

	public const string ANALYTIC_NAME_ITEM_CRATE = "item_crate";

	public const string ANALYTIC_NAME_QUEST_COMPLETE = "quest_complete";

	public const string ANALYTIC_NAME_COUPON_MACHINE = "coupon_machine";

	public const string ANALYTIC_NAME_CRAFT = "craft";

	public const string ANALYTIC_NAME_TOURNAMENT_ITEM_CRATE = "tournament_item_crate";

	public const string ANALYTIC_NAME_TOURNAMENT_EVENT = "tournament_event";

	public const string ANALYTIC_NAME_TOURNAMENT_TICKET = "tournament_ticket";

	public const string ANALYTIC_NAME_CHALLENGE = "Challenge";

	public const string ANALYTIC_NAME_VIDEO_REWARDS = "video_rewards";

	public const string ANALYTIC_NAME_BADGE_LEVEL = "badge_level";

	public const string ANALYTIC_NAME_BADGE_LEVEL_START = "badge_level_start";

	public const string ANALYTIC_NAME_BADGE_WIN = "badge_win";

	public const string ANALYTIC_NAME_BADGE_FAIL = "badge_fail";

	public const string ANALYTIC_NAME_TUT_BEGIN_SCENE = "begin_scene";

	public const string ANALYTIC_NAME_TUT_END_SCENE = "end_scene";

	public const string ANALYTIC_NAME_TUT_BEGIN_MORTY_BATTLE = "begin_morty_battle";

	public const string ANALYTIC_NAME_TUT_END_MORTY_BATTLE = "end_morty_battle";

	public const string ANALYTIC_NAME_TUT_BEGIN_TRAINER_BATTLE = "begin_trainer_battle";

	public const string ANALYTIC_NAME_TUT_END_TRAINER_BATTLE = "end_trainer_battle";

	public const string ANALYTIC_NAME_TUT_BEGIN_RICK_BATTLE = "begin_rick_battle";

	public const string ANALYTIC_NAME_TUT_END_RICK_BATTLE = "end_rick_battle";

	public const string ANALYTIC_NAME_TUT_PICK_UP_CHIP = "pick_up_chip";

	public const string ANALYTIC_NAME_TUT_LAUNCH_MPAD = "launch_mpad";

	public const string ANALYTIC_NAME_TUT_USE_SERUM = "use_serum";

	public const string ANALYTIC_NAME_RICK_OR_TREAT_PURCHASED = "rick_or_treat_purchased";

	public const string ANALYTIC_NAME_FRIEND_REQUEST_SENT = "friend_request_sent";

	public const string ANALYTIC_NAME_FRIEND_REQUEST_MORTY = "friend_request_morty";

	public const string ANALYTIC_NAME_FRIEND_DONATED_MORTY = "friend_donated_morty";

	public bool enableDebug;

	private string _kAndroidAppToken = ((!TestDefs.USE_UPSIGHT_DEMO_KEY) ? "a448be245fb64df797104a4d77681533" : "a0ae2f5cd6dc465aaeaf4b05692ac3be");

	private string _kAndroidAppSecret = ((!TestDefs.USE_UPSIGHT_DEMO_KEY) ? "ff875d6a94314438a7f4581b1236b3f5" : "0e6deaf6ff0040c0963848d534bf1cb3");

	private string _kGcmProjectNumber = "843889502734";

	private string _kIosAppToken = ((!TestDefs.USE_UPSIGHT_DEMO_KEY) ? "ead0e39df0ea41b9a7d40ea9d2b3f370" : "39da36bd4be24e18a54b367dc5407647");

	private string _kIosAppSecret = ((!TestDefs.USE_UPSIGHT_DEMO_KEY) ? "a7202a99072b4efeba70ac24b71b64ff" : "8c52361b93664202a783487944cb7477");

	private string _kAndroidUpsightAnalyticsKey = ((!TestDefs.USE_UPSIGHT_ANALYTICS_ANDROID_QA_KEY) ? "d27e96a9b52944b2841a9f8fab9047ac" : "6e66e3f4767d41789990f9e5d4c6732b");

	public bool isUpsightOpen { get; private set; }

	private void Awake()
	{
		AddEventHandlers(true);
	}

	private void Start()
	{
		Upsight.init(_kAndroidAppToken, _kAndroidAppSecret, _kGcmProjectNumber);
		KontagentBinding.startSession(_kAndroidUpsightAnalyticsKey, TestDefs.USE_UPSIGHT_ANALYTICS_ANDROID_QA_KEY);
		if (enableDebug)
		{
			KontagentBinding.enableDebug();
			Upsight.setLogLevel(UpsightLogLevel.DEBUG);
		}
		DebugLog("Sending Device Information");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("v_maj", "v2.0.3");
		KontagentBinding.sendDeviceInformation(dictionary);
		AttemptUpsightConnection();
		Upsight.registerForPushNotifications();
	}

	private void AddEventHandlers(bool add)
	{
		if (add)
		{
			UpsightManager.openRequestSucceededEvent += OpenRequestSucceededHandler;
			UpsightManager.openRequestFailedEvent += OpenRequestFailedHandler;
			UpsightManager.trackInAppPurchaseSucceededEvent += TrackInAppPurchaseSucceededHandler;
			UpsightManager.trackInAppPurchaseFailedEvent += TrackInAppPurchaseFailedEvent;
			UpsightManager.reportCustomEventSucceededEvent += ReportCustomEventSucceededHandler;
			UpsightManager.reportCustomEventFailedEvent += ReportCustomEventFailedHandler;
			UpsightManager.unlockedRewardEvent += UnlockedRewardHandler;
		}
		else
		{
			UpsightManager.openRequestSucceededEvent -= OpenRequestSucceededHandler;
			UpsightManager.openRequestFailedEvent -= OpenRequestFailedHandler;
			UpsightManager.trackInAppPurchaseSucceededEvent -= TrackInAppPurchaseSucceededHandler;
			UpsightManager.trackInAppPurchaseFailedEvent -= TrackInAppPurchaseFailedEvent;
			UpsightManager.reportCustomEventSucceededEvent -= ReportCustomEventSucceededHandler;
			UpsightManager.reportCustomEventFailedEvent -= ReportCustomEventFailedHandler;
			UpsightManager.unlockedRewardEvent -= UnlockedRewardHandler;
		}
	}

	protected void DebugLog(string str)
	{
		if (enableDebug)
		{
		}
	}

	private bool OfflineBlocked()
	{
		return Application.platform != RuntimePlatform.Android;
	}

	public void AttemptUpsightConnection()
	{
		if (OfflineBlocked()) { return; }
		DebugLog("AttemptUpsightConnection()");
		Upsight.requestAppOpen();
	}

	private void OnDestroy()
	{
		AddEventHandlers(false);
	}

	public void CustomAnalyticEvent(eSubType1 subType1, eSubType2 subType2, string name, int gameNumber = -1)
	{
		if (OfflineBlocked()) { return; }
		CustomAnalyticValueEvent(subType1, subType2, name, 0, gameNumber);
	}

	public void CustomAnalyticValueEvent(eSubType1 subType1, eSubType2 subType2, string name, int value, int gameNumber = -1)
	{
		if (OfflineBlocked()) { return; }
		eSubType3 eSubType4 = eSubType3.NONE;
		if (App.currentMode == eGameMode.CAMPAIGN)
		{
			eSubType4 = eSubType3.CAMPAIGN;
		}
		else if (App.currentMode == eGameMode.MULTIPLAYER)
		{
			eSubType4 = eSubType3.MULTIPLAYER;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("GameNumber", App.saveData.player.dimensionCount);
		dictionary.Add("SessionNumber", App.saveData.player.sessionCount);
		dictionary.Add("SessionTime", Mathf.RoundToInt(App.sessionTime));
		dictionary.Add("TotalGameTime", Mathf.RoundToInt(App.saveData.player.totalGameTime));
		dictionary.Add("BadgeNumber", PlayerDefs.BadgeCount);
		dictionary.Add("SchmeckleBalance", PlayerDefs.CoinCount);
		dictionary.Add("CouponBalance", PlayerDefs.CouponCount);
		dictionary.Add("PartyCount", PlayerDefs.PlayerPartyCount);
		dictionary.Add("CaughtCount", PlayerDefs.MortyCaughtCount);
		gameNumber = GetBucketGameNumber((gameNumber != -1) ? gameNumber : App.saveData.player.dimensionCount);
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		if (subType1 != eSubType1.NONE)
		{
			dictionary2.Add("st1", subType1.ToString().ToLower());
		}
		if (subType2 != eSubType2.NONE)
		{
			dictionary2.Add("st2", subType2.ToString().ToLower());
		}
		if (eSubType4 != eSubType3.NONE)
		{
			dictionary2.Add("st3", eSubType4.ToString().ToLower());
		}
		dictionary2.Add("v", value.ToString());
		dictionary2.Add("l", gameNumber.ToString());
		dictionary2.Add("data", Json.Serialize(dictionary));
		DebugLog("KontagentBinding.customEvent() name:" + name + "\n" + Json.Serialize(dictionary2));
		KontagentBinding.customEvent(name, dictionary2);
	}

	private int GetBucketGameNumber(int gameNumber)
	{
		if (gameNumber > 150)
		{
			gameNumber = 151 + Mathf.FloorToInt((float)gameNumber / 50f);
		}
		if (gameNumber > 255)
		{
			gameNumber = 255;
		}
		return gameNumber;
	}

	public void TrackIapPurchase(string productID, UpsightAndroidPurchaseResolution resType, int cost = 0, string googleOrderId = null)
	{
		if (OfflineBlocked()) { return; }
		DebugLog(string.Concat("TrackIapPurchase() for resType:", resType, " cost", cost));
		if (resType == UpsightAndroidPurchaseResolution.Bought)
		{
			string text = null;
			if (App.currentMode == eGameMode.CAMPAIGN)
			{
				text = IAPDefs.GetAnalyticIDForIAP(productID);
			}
			else if (App.currentMode == eGameMode.MULTIPLAYER)
			{
				text = MPIAPDefs.GetAnalyticIDForIAP(productID);
			}
			Upsight.trackInAppPurchase(productID, 1, resType, cost, googleOrderId, "Google");
			CustomAnalyticValueEvent(eSubType1.IAP, eSubType2.PURCHASE, text, cost);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("productID", productID);
		}
		else
		{
			Upsight.trackInAppPurchase(productID, 1, resType, cost, googleOrderId, "Google");
		}
	}

	private byte[] StringToByteArray(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public void SendContentRequest(eUpsightPlacementID placementID)
	{
		if (OfflineBlocked()) { return; }
		if (!isUpsightOpen)
		{
			DebugLog("Skipping conent request because upsight is not open. Requesting upsight open instead");
			Upsight.requestAppOpen();
		}
		DebugLog("Sending content request for placement: " + placementID.ToString().ToLower());
		Upsight.sendContentRequest(placementID.ToString().ToLower(), false);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			Upsight.requestAppOpen();
			KontagentBinding.startSession(_kAndroidUpsightAnalyticsKey, TestDefs.USE_UPSIGHT_ANALYTICS_ANDROID_QA_KEY);
			DebugLog("Sending Device Information");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("v_maj", "v2.0.3");
			KontagentBinding.sendDeviceInformation(dictionary);
			if (enableDebug)
			{
				KontagentBinding.enableDebug();
			}
		}
		else
		{
			KontagentBinding.stopSession();
		}
	}

	private void OpenRequestSucceededHandler(Dictionary<string, object> data)
	{
		DebugLog("OpenRequestSucceededHandler");
		isUpsightOpen = true;
	}

	private void OpenRequestFailedHandler(string error)
	{
		DebugLog("OpenSuccessFailedHandler - " + error);
		isUpsightOpen = false;
	}

	private void TrackInAppPurchaseSucceededHandler()
	{
		DebugLog("TrackInAppPurchaseSucceededHandler");
	}

	private void TrackInAppPurchaseFailedEvent(string error)
	{
		DebugLog("TrackInAppPurchaseFailedEvent - " + error);
	}

	private void ReportCustomEventSucceededHandler()
	{
		DebugLog("ReportCustomEventSucceededHandler");
	}

	private void ReportCustomEventFailedHandler(string error)
	{
		DebugLog("ReportCustomEventFailedHandler - " + error);
	}

	private void UnlockedRewardHandler(UpsightReward reward)
	{
		DebugLog("unlockedRewardEvent: " + reward);
		List<string> list = new List<string>(App.saveData.settings.upsightRewardReceipts);
		DebugLog("current reward receipt list:\n");
		foreach (string item in list)
		{
			DebugLog(item + "\n");
		}
		if (list.Contains(reward.receipt))
		{
			DebugLog("this reward has already been received by the user");
			return;
		}
		string text = reward.name;
		if (text != null && text == "coupons")
		{
			int resultAmount;
			string resultText;
			PlayerDefs.AddCoupon(reward.quantity, out resultAmount, out resultText);
			DebugLog(string.Format("Attempted to add {0} coupons, new value is {1}", reward.quantity.ToString(), resultAmount.ToString()));
		}
		list.Add(reward.receipt);
		App.saveData.settings.upsightRewardReceipts = list.ToArray();
		App.saveData.Save(eSaveDataType.SETTINGS | eSaveDataType.PLAYER);
		DebugLog("new reward receipt list:\n");
		foreach (string item2 in list)
		{
			DebugLog(item2 + "\n");
		}
	}
}
