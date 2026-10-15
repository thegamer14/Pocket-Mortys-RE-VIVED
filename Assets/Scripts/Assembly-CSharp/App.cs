using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class App : MonoBehaviour
{
	public static eGameMode currentMode;

	public static bool redirectOnMPSessionDisconnect;

	public static SaveDataController saveData { get; private set; }

	public static StateController states { get; private set; }

	public static IAPController iap { get; private set; }

	public static UpsightController upsight { get; private set; }

	public static BPSSocialController social { get; private set; }

	public static AudioController sound { get; private set; }

	public static NetworkController net { get; private set; }

	public static FyberController fyber { get; private set; }

	public static bool isPaused { get; private set; }

	public static float sessionTime { get; private set; }

	private void Awake()
	{
		if (!CheckForDuplicates())
		{
			Object.DontDestroyOnLoad(base.gameObject);
			Application.targetFrameRate = 60;
			Application.runInBackground = true;
			DOTween.Init();
			if (TestDefs.DEBUG_FPS_COUNTER)
			{
				GameObject gameObject = new GameObject("FPSCounter");
				gameObject.AddComponent<FPSCounter>();
			}
			saveData = base.gameObject.AddComponent<SaveDataController>();
			saveData.Load();
			saveData.player.sessionCount++;
			saveData.Save(eSaveDataType.PLAYER);
			LocalizationDefs.SetUp();
			CreateControllers();
			AddEvents(true);
		states.SetStartingState();
			SetupIapController();
			SetupSocialController();
			SetupNetworkController();
		}
	}

	private bool CheckForDuplicates()
	{
		App[] array = Object.FindObjectsOfType<App>();
		if (array.Length > 1)
		{
			Object.Destroy(base.gameObject);
			return true;
		}
		return false;
	}

	private void AddEvents(bool value)
	{
		if (value)
		{
			iap.PurchaseSuccessfulEvent += PurchaseSuccessfulEventHandler;
			iap.PurchaseFailedEvent += PurchaseFailedEventHandler;
			net.session.DisconnectedEvent += NetSessionDisconnectedHandler;
		}
		else
		{
			iap.PurchaseSuccessfulEvent -= PurchaseSuccessfulEventHandler;
			iap.PurchaseFailedEvent -= PurchaseFailedEventHandler;
			net.session.DisconnectedEvent -= NetSessionDisconnectedHandler;
		}
	}

	private void Update()
	{
		TimeIndependant.Update();
		sessionTime += Time.deltaTime;
		saveData.player.totalGameTime += Time.deltaTime;
		if (states != null)
		{
			states.ManualUpdate(Time.deltaTime);
		}
	}

	private void OnApplicationQuit()
	{
		AddEvents(false);
	}

	private void CreateControllers()
	{
		GameObject gameObject = new GameObject("StateController");
		gameObject.transform.SetParent(base.transform);
		states = gameObject.AddComponent<StateController>();
		GameObject target = Object.Instantiate(Resources.Load(string.Format("UI/Utils/{0}", "EventSystem"))) as GameObject;
		Object.DontDestroyOnLoad(target);
		iap = base.gameObject.AddComponent<IAPController>();
		upsight = base.gameObject.AddComponent<UpsightController>();
		upsight.enableDebug = TestDefs.DEBUG_UPSIGHT_CONTROLLER;
		social = base.gameObject.AddComponent<BPSSocialController>();
		sound = base.gameObject.AddComponent<AudioController>();
		sound.sfxVolume = saveData.settings.soundFXVolume;
		sound.musicVolume = saveData.settings.musicVolume;
		net = base.gameObject.AddComponent<NetworkController>();
		fyber = base.gameObject.AddComponent<FyberController>();
		fyber.enableDebug = TestDefs.DEBUG_FYBER_CONTROLLER;
	}

	private void SetupIapController()
	{
		iap.enableDebug = TestDefs.DEBUG_IAP_CONTROLLER;
		iap.failIapPurchasesInEditor = TestDefs.FAIL_EDITOR_IAP_PURCHASES;
		iap.editorIapProductJson = ((!TestDefs.EDITOR_LOAD_IAP_PRODUCTS) ? null : TestDefs.EDITOR_IAP_PRODUCTS_JSON);
		iap.iosVerificationInSandbox = TestDefs.USE_IOS_VERIFICATION_SANDBOX;
		iap.testServerValidationFailure = TestDefs.FAIL_IOS_SERVER_VALIDATION;
		List<string> list = new List<string>();
		list.AddRange(IAPDefs.IAP_PRODUCT_LIST);
		list.AddRange(MPIAPDefs.IAP_PRODUCT_LIST);
		iap.Setup("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAi1y2ILGPmjnOszH6VuboTixg40doDDhOOf2JnBvo2Ko1YZNx1cvmlUDwfz+Sc6OBmo5W2LRlA/qafZnsQMLTg0HR28x0dLZDkDVmfYcC7eWw0hhXxhVe9SWTbVFv0iwJ9tMCGyqB494LZp/PPoWTXcfRzvm61J2odu4n65KgXcDcH2aAVKWQTaJVfREZ41z2GNTeLxH3dPsBgppGBAmfHB6ArtIr1waevn46GmzVQWQnwAmkrvZPoou8k2N5G90ac+XGvP6YbYp6/C13MlGvmkFU28cq4DG/Ii3IVx+o1zyyCo/62osN1fFMkB/mDaXTLc+bz65ZSAerNfZhreU1bwIDAQAB", list.ToArray());
	}

	private void SetupSocialController()
	{
		social.enableDebug = TestDefs.DEBUG_BPS_SOCIAL;
		social.AuthenticateEvent += SocailAuthenticateHandler;
	}

	private void SetupNetworkController()
	{
		NetworkDefs.SetEnvironmentEndPoints();
	}

	public static void Pause(bool value)
	{
		if (value)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
		isPaused = value;
	}

	private void PurchaseSuccessfulEventHandler(string productId, string token)
	{
		AwardPurchasedProduct(productId);
		upsight.TrackIapPurchase(productId, UpsightAndroidPurchaseResolution.Bought, IAPDefs.GetIAPCostForProduct(productId), token);
	}

	private void AwardPurchasedProduct(string productId)
	{
		int resultAmount;
		string resultText;
		switch (productId)
		{
		default:
			return;
		case "com.adultswim.pocketmorties.coupon01":
			PlayerDefs.AddCoupon(1, out resultAmount, out resultText);
			break;
		case "com.adultswim.pocketmorties.coupon05":
		case "com.adultswim.pocketmorties.coupon05discount":
			PlayerDefs.AddCoupon(5, out resultAmount, out resultText);
			break;
		case "com.adultswim.pocketmorties.coupon15":
		case "com.adultswim.pocketmorties.coupon15discount":
			PlayerDefs.AddCoupon(15, out resultAmount, out resultText);
			break;
		case "com.adultswim.pocketmorties.coupon40":
		case "com.adultswim.pocketmorties.coupon40discount":
			PlayerDefs.AddCoupon(40, out resultAmount, out resultText);
			break;
		case "com.adultswim.pocketmorties.coupon125":
			PlayerDefs.AddCoupon(125, out resultAmount, out resultText);
			break;
		case "com.adultswim.pocketmorties.coupon300":
			PlayerDefs.AddCoupon(300, out resultAmount, out resultText);
			break;
		}
		saveData.Save(eSaveDataType.SETTINGS | eSaveDataType.PLAYER);
	}

	private void PurchaseFailedEventHandler(string productId, string error, string googleOrderId, bool didCancel)
	{
		UpsightAndroidPurchaseResolution resType = ((!didCancel) ? UpsightAndroidPurchaseResolution.Error : UpsightAndroidPurchaseResolution.Cancelled);
		upsight.TrackIapPurchase(productId, resType, IAPDefs.GetIAPCostForProduct(productId), googleOrderId);
	}

	private void GoogleInventoryPurchaseFoundHandler(string productId)
	{
		AwardPurchasedProduct(productId);
	}

	private void SocailAuthenticateHandler(bool isSuccess)
	{
		if (!isSuccess && !saveData.player.googlePlaySignInDeclined)
		{
			saveData.player.googlePlaySignInDeclined = true;
			saveData.Save(eSaveDataType.PLAYER);
		}
		else if (isSuccess && saveData.player.googlePlaySignInDeclined)
		{
			saveData.player.googlePlaySignInDeclined = false;
			saveData.Save(eSaveDataType.PLAYER);
		}
	}

	public void DeepLinkURLHandler(string url)
	{
		string text = url.ToLower();
		if (text != null && text == "pocketmortys://store")
		{
			states.Get<InterstitialState>().ActivateCouponBuyPopup();
		}
	}

	private void NetSessionDisconnectedHandler(eNetSessionDisconnectType disconnectType)
	{
		if (redirectOnMPSessionDisconnect)
		{
			redirectOnMPSessionDisconnect = false;
			if (disconnectType == eNetSessionDisconnectType.USER_QUIT)
			{
				states.Set<MainMenuState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
				return;
			}
			states.Get<MPUserSignInState>().disconnectType = disconnectType;
			MPSharedStateInfo.serverMessage = new MPServerMessageInfo(typeof(MPUserSignInState));
			states.Set<MPServerMessageState>(new FadeTransInfo(), new FadeTransInfo(), false, eStateAction.DEACTIVATE_ALL);
		}
	}
}
