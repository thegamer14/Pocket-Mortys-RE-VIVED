using System;
using System.Collections.Generic;
using UnityEngine;

public class IAPDefs
{
	public const string ANDROID_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAi1y2ILGPmjnOszH6VuboTixg40doDDhOOf2JnBvo2Ko1YZNx1cvmlUDwfz+Sc6OBmo5W2LRlA/qafZnsQMLTg0HR28x0dLZDkDVmfYcC7eWw0hhXxhVe9SWTbVFv0iwJ9tMCGyqB494LZp/PPoWTXcfRzvm61J2odu4n65KgXcDcH2aAVKWQTaJVfREZ41z2GNTeLxH3dPsBgppGBAmfHB6ArtIr1waevn46GmzVQWQnwAmkrvZPoou8k2N5G90ac+XGvP6YbYp6/C13MlGvmkFU28cq4DG/Ii3IVx+o1zyyCo/62osN1fFMkB/mDaXTLc+bz65ZSAerNfZhreU1bwIDAQAB";

	public const string IAP_ID_COUPON_PACK_01 = "com.adultswim.pocketmorties.coupon01";

	public const string IAP_ID_COUPON_PACK_05 = "com.adultswim.pocketmorties.coupon05";

	public const string IAP_ID_COUPON_PACK_15 = "com.adultswim.pocketmorties.coupon15";

	public const string IAP_ID_COUPON_PACK_40 = "com.adultswim.pocketmorties.coupon40";

	public const string IAP_ID_COUPON_PACK_125 = "com.adultswim.pocketmorties.coupon125";

	public const string IAP_ID_COUPON_PACK_300 = "com.adultswim.pocketmorties.coupon300";

	public const string IAP_ID_COUPON_PACK_05_VGP = "com.adultswim.pocketmorties.coupon05discount";

	public const string IAP_ID_COUPON_PACK_15_VGP = "com.adultswim.pocketmorties.coupon15discount";

	public const string IAP_ID_COUPON_PACK_40_VGP = "com.adultswim.pocketmorties.coupon40discount";

	public const int IAP_QNT_COUPON_PACK_01 = 1;

	public const int IAP_QNT_COUPON_PACK_05 = 5;

	public const int IAP_QNT_COUPON_PACK_15 = 15;

	public const int IAP_QNT_COUPON_PACK_40 = 40;

	public const int IAP_QNT_COUPON_PACK_125 = 125;

	public const int IAP_QNT_COUPON_PACK_300 = 300;

	public const int IAP_QNT_COUPON_PACK_05_VGP = 5;

	public const int IAP_QNT_COUPON_PACK_15_VGP = 15;

	public const int IAP_QNT_COUPON_PACK_40_VGP = 40;

	public const int IAP_QNT_COUPON_PACK_MAX = 300;

	public static string[] IAP_PRODUCT_LIST = new string[9] { "com.adultswim.pocketmorties.coupon01", "com.adultswim.pocketmorties.coupon05", "com.adultswim.pocketmorties.coupon15", "com.adultswim.pocketmorties.coupon40", "com.adultswim.pocketmorties.coupon125", "com.adultswim.pocketmorties.coupon300", "com.adultswim.pocketmorties.coupon05discount", "com.adultswim.pocketmorties.coupon15discount", "com.adultswim.pocketmorties.coupon40discount" };

	private static Dictionary<string, int> _iapCostsCents = new Dictionary<string, int>
	{
		{ "com.adultswim.pocketmorties.coupon01", 99 },
		{ "com.adultswim.pocketmorties.coupon05", 399 },
		{ "com.adultswim.pocketmorties.coupon15", 999 },
		{ "com.adultswim.pocketmorties.coupon40", 1999 },
		{ "com.adultswim.pocketmorties.coupon125", 4999 },
		{ "com.adultswim.pocketmorties.coupon300", 9999 },
		{ "com.adultswim.pocketmorties.coupon05discount", 299 },
		{ "com.adultswim.pocketmorties.coupon15discount", 599 },
		{ "com.adultswim.pocketmorties.coupon40discount", 1199 }
	};

	public static int GetIAPCostForProduct(string productID)
	{
		if (!string.IsNullOrEmpty(productID) && _iapCostsCents.ContainsKey(productID))
		{
			return _iapCostsCents[productID];
		}
		return 1;
	}

	public static int GetNumCouponsForPack(string productID)
	{
		switch (productID)
		{
		default:
			return 0;
		case "com.adultswim.pocketmorties.coupon01":
			return 1;
		case "com.adultswim.pocketmorties.coupon05":
		case "com.adultswim.pocketmorties.coupon05discount":
			return 5;
		case "com.adultswim.pocketmorties.coupon15":
		case "com.adultswim.pocketmorties.coupon15discount":
			return 15;
		case "com.adultswim.pocketmorties.coupon40":
		case "com.adultswim.pocketmorties.coupon40discount":
			return 40;
		case "com.adultswim.pocketmorties.coupon125":
			return 125;
		case "com.adultswim.pocketmorties.coupon300":
			return 300;
		}
	}

	public static string GetAnalyticIDForIAP(string id)
	{
		switch (id)
		{
		default:
			return null;
		case "com.adultswim.pocketmorties.coupon01":
			return "1_coupon";
		case "com.adultswim.pocketmorties.coupon05":
		case "com.adultswim.pocketmorties.coupon05discount":
			return "5_coupon";
		case "com.adultswim.pocketmorties.coupon15":
		case "com.adultswim.pocketmorties.coupon15discount":
			return "15_coupon";
		case "com.adultswim.pocketmorties.coupon40":
		case "com.adultswim.pocketmorties.coupon40discount":
			return "40_coupon";
		case "com.adultswim.pocketmorties.coupon125":
			return "125_coupon";
		case "com.adultswim.pocketmorties.coupon300":
			return "300_coupon";
		}
	}

	public static int GetCouponPackSavingPercent(string productID)
	{
		IAPProduct loadedProductInfo = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.coupon01");
		IAPProduct loadedProductInfo2 = App.iap.GetLoadedProductInfo(productID);
		if (loadedProductInfo == null || loadedProductInfo2 == null)
		{
			return 0;
		}
		float num = Convert.ToSingle(loadedProductInfo.price);
		float num2 = Convert.ToSingle(loadedProductInfo2.price);
		if (num == 0f || num2 == 0f)
		{
			return 0;
		}
		float num3 = num / (float)GetNumCouponsForPack("com.adultswim.pocketmorties.coupon01");
		float num4 = num2 / (float)GetNumCouponsForPack(productID);
		float num5 = 1f - num4 / num3;
		return Mathf.CeilToInt(num5 * 100f);
	}
}
