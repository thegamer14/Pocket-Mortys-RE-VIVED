using System;
using System.Collections.Generic;
using UnityEngine;

public class MPIAPDefs
{
	public const string IAP_ID_COUPON_PACK_01 = "com.adultswim.pocketmorties.clubrickcoupon01";

	public const string IAP_ID_COUPON_PACK_05 = "com.adultswim.pocketmorties.clubrickcoupon05";

	public const string IAP_ID_COUPON_PACK_15 = "com.adultswim.pocketmorties.clubrickcoupon15";

	public const string IAP_ID_COUPON_PACK_40 = "com.adultswim.pocketmorties.clubrickcoupon40";

	public const string IAP_ID_COUPON_PACK_125 = "com.adultswim.pocketmorties.clubrickcoupon125";

	public const string IAP_ID_COUPON_PACK_300 = "com.adultswim.pocketmorties.clubrickcoupon300";

	public const string IAP_ID_COUPON_PACK_05_VGP = "com.adultswim.pocketmorties.clubrickcoupon05discount";

	public const string IAP_ID_COUPON_PACK_15_VGP = "com.adultswim.pocketmorties.clubrickcoupon15discount";

	public const string IAP_ID_COUPON_PACK_40_VGP = "com.adultswim.pocketmorties.clubrickcoupon40discount";

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

	public static string[] IAP_PRODUCT_LIST = new string[9] { "com.adultswim.pocketmorties.clubrickcoupon01", "com.adultswim.pocketmorties.clubrickcoupon05", "com.adultswim.pocketmorties.clubrickcoupon15", "com.adultswim.pocketmorties.clubrickcoupon40", "com.adultswim.pocketmorties.clubrickcoupon125", "com.adultswim.pocketmorties.clubrickcoupon300", "com.adultswim.pocketmorties.clubrickcoupon05discount", "com.adultswim.pocketmorties.clubrickcoupon15discount", "com.adultswim.pocketmorties.clubrickcoupon40discount" };

	private static Dictionary<string, int> _iapCostsCents = new Dictionary<string, int>
	{
		{ "com.adultswim.pocketmorties.clubrickcoupon01", 99 },
		{ "com.adultswim.pocketmorties.clubrickcoupon05", 399 },
		{ "com.adultswim.pocketmorties.clubrickcoupon15", 999 },
		{ "com.adultswim.pocketmorties.clubrickcoupon40", 1999 },
		{ "com.adultswim.pocketmorties.clubrickcoupon125", 4999 },
		{ "com.adultswim.pocketmorties.clubrickcoupon300", 9999 },
		{ "com.adultswim.pocketmorties.clubrickcoupon05discount", 299 },
		{ "com.adultswim.pocketmorties.clubrickcoupon15discount", 599 },
		{ "com.adultswim.pocketmorties.clubrickcoupon40discount", 1199 }
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
		case "com.adultswim.pocketmorties.clubrickcoupon01":
			return 1;
		case "com.adultswim.pocketmorties.clubrickcoupon05":
		case "com.adultswim.pocketmorties.clubrickcoupon05discount":
			return 5;
		case "com.adultswim.pocketmorties.clubrickcoupon15":
		case "com.adultswim.pocketmorties.clubrickcoupon15discount":
			return 15;
		case "com.adultswim.pocketmorties.clubrickcoupon40":
		case "com.adultswim.pocketmorties.clubrickcoupon40discount":
			return 40;
		case "com.adultswim.pocketmorties.clubrickcoupon125":
			return 125;
		case "com.adultswim.pocketmorties.clubrickcoupon300":
			return 300;
		}
	}

	public static string GetAnalyticIDForIAP(string id)
	{
		switch (id)
		{
		default:
			return null;
		case "com.adultswim.pocketmorties.clubrickcoupon01":
			return "1_coupon";
		case "com.adultswim.pocketmorties.clubrickcoupon05":
		case "com.adultswim.pocketmorties.clubrickcoupon05discount":
			return "5_coupon";
		case "com.adultswim.pocketmorties.clubrickcoupon15":
		case "com.adultswim.pocketmorties.clubrickcoupon15discount":
			return "15_coupon";
		case "com.adultswim.pocketmorties.clubrickcoupon40":
		case "com.adultswim.pocketmorties.clubrickcoupon40discount":
			return "40_coupon";
		case "com.adultswim.pocketmorties.clubrickcoupon125":
			return "125_coupon";
		case "com.adultswim.pocketmorties.clubrickcoupon300":
			return "300_coupon";
		}
	}

	public static int GetCouponPackSavingPercent(string productID)
	{
		IAPProduct loadedProductInfo = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon01");
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
		float num3 = num / (float)GetNumCouponsForPack("com.adultswim.pocketmorties.clubrickcoupon01");
		float num4 = num2 / (float)GetNumCouponsForPack(productID);
		float num5 = 1f - num4 / num3;
		return Mathf.CeilToInt(num5 * 100f);
	}
}
