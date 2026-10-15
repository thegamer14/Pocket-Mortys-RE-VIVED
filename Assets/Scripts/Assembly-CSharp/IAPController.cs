using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;
using Prime31;
using UnityEngine;

public class IAPController : MonoBehaviour
{
	public delegate void ProductsEventHandler(List<IAPProduct> products, string error);

	public delegate void PurchaseSuccessHandler(string productId, string token);

	public delegate void PurchaseFailedHandler(string productId, string error, string googleOrderId, bool didCancel);

	public delegate void PurchasesRestoredSuccessHandler();

	public delegate void PurchasesRestoredFailedHanlder(string error);

	public delegate void GoogleInventoryPurchaseFoundHandler(string productId);

	protected const float kFakeEditorIapDelay = 2f;

	protected const float _kServerResponseTimeOut = 15f;

	protected const string _kConsumablePayload = "consumable";

	protected const string _kNonConsumablePayload = "non-consumable";

	private string _androidPublicKey;

	private string[] _productIdentifiers;

	public bool enableDebug;

	public bool failIapPurchasesInEditor;

	public string editorIapProductJson;

	public bool iosVerificationInSandbox;

	public bool testServerValidationFailure;

	private List<IAPProduct> _loadedProducts;

	protected bool _areProductsLoading;

	protected bool _attemptingProductPurchase;

	private List<GooglePurchase> _purchasesPendingConsume;

	public bool billingSupported { get; private set; }

	public event ProductsEventHandler ProductsLoadSuccessfulEvent;

	public event ProductsEventHandler ProductsLoadFailedEvent;

	public event PurchaseSuccessHandler PurchaseSuccessfulEvent;

	public event PurchaseFailedHandler PurchaseFailedEvent;

	public event PurchasesRestoredSuccessHandler PurchasesRestoredSuccessEvent;

	public event PurchasesRestoredFailedHanlder PurchasesRestoredFailedEvent;

	public event GoogleInventoryPurchaseFoundHandler GoogleInventoryPurchaseFoundEvent;

	public void Setup(string androidPublicKey, string[] productIdentifiers)
	{
		_androidPublicKey = androidPublicKey;
		_productIdentifiers = productIdentifiers;
		GoogleIABManager.queryInventorySucceededEvent += QueryInventorySucceededHandler;
		GoogleIABManager.queryInventoryFailedEvent += ProductsLoadFailedHandler;
		GoogleIABManager.purchaseSucceededEvent += AndroidPurchaseSucceededHandler;
		GoogleIABManager.purchaseFailedEvent += AndroidPurchaseFailedHandler;
		GoogleIABManager.consumePurchaseSucceededEvent += ConsumeIAPSuccessfulHandler;
		GoogleIABManager.consumePurchaseFailedEvent += ConsumeIAPFailedHandler;
		GoogleIABManager.billingSupportedEvent += BillingSupportedHandler;
		GoogleIABManager.billingNotSupportedEvent += BillingNotSupportedHandler;
		_purchasesPendingConsume = new List<GooglePurchase>();
		if (enableDebug)
		{
			GoogleIAB.enableLogging(true);
		}
		billingSupported = false;
		InitGoogleIAB();
		if (enableDebug)
		{
			GoogleIAB.enableLogging(true);
		}
	}

	private void OnDestroy()
	{
		GoogleIABManager.queryInventorySucceededEvent -= QueryInventorySucceededHandler;
		GoogleIABManager.queryInventoryFailedEvent -= ProductsLoadFailedHandler;
		GoogleIABManager.purchaseSucceededEvent -= AndroidPurchaseSucceededHandler;
		GoogleIABManager.purchaseFailedEvent -= AndroidPurchaseFailedHandler;
		GoogleIABManager.consumePurchaseSucceededEvent -= ConsumeIAPSuccessfulHandler;
		GoogleIABManager.consumePurchaseFailedEvent -= ConsumeIAPFailedHandler;
		GoogleIABManager.billingSupportedEvent -= BillingSupportedHandler;
		GoogleIABManager.billingNotSupportedEvent -= BillingNotSupportedHandler;
	}

	protected void DebugLog(string str)
	{
		if (enableDebug)
		{
		}
	}

	public void LoadProducts()
	{
		DebugLog("LoadProducts() _areProductsLoading=" + _areProductsLoading + " _products == null? : " + (_loadedProducts == null));
		if (!billingSupported)
		{
			string text = "Billing not supported. Please make sure you are signed in to Google Play and are connected to the internet.";
			DebugLog(text);
			if (this.ProductsLoadFailedEvent != null)
			{
				this.ProductsLoadFailedEvent(null, text);
			}
		}
		else if (_areProductsLoading)
		{
			string text2 = "Cannot begin new load request, products are already loading.";
			DebugLog(text2);
			if (this.ProductsLoadFailedEvent != null)
			{
				this.ProductsLoadFailedEvent(null, text2);
			}
		}
		else if (_loadedProducts != null)
		{
			string text3 = "Aborted loading request, products are already loaded (" + _loadedProducts.Count + " retrieved).";
			DebugLog(text3);
			if (this.ProductsLoadFailedEvent != null)
			{
				this.ProductsLoadFailedEvent(null, text3);
			}
		}
		else
		{
			_areProductsLoading = true;
			DebugLog("calling GoogleIAB.queryInventory()");
			GoogleIAB.queryInventory(_productIdentifiers);
		}
	}

	private IEnumerator LoadTestProductsCoroutine()
	{
		yield return new WaitForSeconds(2f);
	}

	public IAPProduct GetLoadedProductInfo(string productId)
	{
		if (_loadedProducts == null)
		{
			return null;
		}
		return _loadedProducts.FirstOrDefault((IAPProduct p) => p.productId == productId);
	}

	public void PurchaseIAP(string productId, bool consumable = true)
	{
		StartCoroutine(PurchaseIAPCoroutine(productId, consumable));
	}

	private IEnumerator PurchaseIAPCoroutine(string productId, bool consumable = true)
	{
		DebugLog("PurchaseIAP(productId: " + productId + " consumable: " + consumable + ") _areProductsLoading:" + _areProductsLoading + " _billingSupported:" + billingSupported);
		if (!billingSupported)
		{
			string text = "Billing not supported. Please make sure you are signed in to Google Play and are connected to the internet.";
			DebugLog(text);
			if (this.PurchaseFailedEvent != null)
			{
				this.PurchaseFailedEvent(productId, text, null, false);
			}
			yield break;
		}
		while (_areProductsLoading)
		{
			yield return null;
		}
		if (_loadedProducts == null)
		{
			DebugLog("_loadedProducts == null");
			LoadProducts();
			while (_areProductsLoading)
			{
				yield return null;
			}
		}
		if (_loadedProducts == null || _loadedProducts.Count == 0)
		{
			string text2 = "Could not connect to the store to retrieve products.";
			DebugLog("IAPController PurchaseIAP() error: " + text2);
			if (this.PurchaseFailedEvent != null)
			{
				this.PurchaseFailedEvent(productId, text2, null, false);
			}
		}
		else if (_attemptingProductPurchase)
		{
			string text3 = "A purchase is already pending, cannot start a new request";
			DebugLog("IAPController PurchaseIAP() error: " + text3);
			if (this.PurchaseFailedEvent != null)
			{
				this.PurchaseFailedEvent(productId, text3, null, false);
			}
		}
		else
		{
			_attemptingProductPurchase = true;
			string payload = ((!consumable) ? "non-consumable" : "consumable");
			GoogleIAB.purchaseProduct(productId, payload);
		}
	}

	public void RestorePurchases()
	{
		DebugLog("RestorePurchases()");
	}

	protected void PurchaseIAPHandler(string productId, string error, string token, bool didCancel)
	{
		DebugLog("PurchaseIAPHandler(productId: " + productId + "; error: " + error + "; token: " + token + "; didCancel: " + didCancel + ")");
		_attemptingProductPurchase = false;
		if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(productId))
		{
			if (this.PurchaseSuccessfulEvent != null)
			{
				this.PurchaseSuccessfulEvent(productId, token);
			}
		}
		else if (this.PurchaseFailedEvent != null)
		{
			this.PurchaseFailedEvent(productId, error, token, didCancel);
		}
	}

	protected IEnumerator FakeIAPPurchaseDelay(string idStr)
	{
		yield return new WaitForSeconds(2f);
		if (!failIapPurchasesInEditor)
		{
		}
	}

	protected void ProductsLoadComplete(List<IAPProduct> productList)
	{
		DebugLog("ProductsLoadComplete. total products received: " + productList.Count);
		_areProductsLoading = false;
		_loadedProducts = productList;
		if (this.ProductsLoadSuccessfulEvent != null)
		{
			this.ProductsLoadSuccessfulEvent(_loadedProducts, null);
		}
	}

	protected void ProductsLoadFailedHandler(string error)
	{
		DebugLog("ProductsLoadFailedHandler() error: " + error);
		_areProductsLoading = false;
		if (this.ProductsLoadFailedEvent != null)
		{
			this.ProductsLoadFailedEvent(_loadedProducts, "No products found");
		}
	}

	public void InitGoogleIAB()
	{
		GoogleIAB.init(_androidPublicKey);
	}

	private void AndroidPurchaseSucceededHandler(GooglePurchase purchase)
	{
		DebugLog("AndroidPurchaseSucceededHandler() purchase.originalJson: " + purchase.originalJson);
		DebugLog("AndroidPurchaseSucceededHandler() purchase.purchaseToken: " + purchase.purchaseToken);
		DebugLog("AndroidPurchaseSucceededHandler() purchase.signature: " + purchase.signature);
		if (MPIAPDefs.IAP_PRODUCT_LIST.Contains(purchase.productId))
		{
			object googleData = JsonReader.Deserialize(purchase.originalJson);
			App.net.player.ValidateCouponPurchase(purchase.orderId, purchase.signature, googleData, delegate(eValidateCouponPurchaseError error)
			{
				if (error == eValidateCouponPurchaseError.NONE || error == eValidateCouponPurchaseError.IAP_TRANSACTION_ALREADY_APPLIED)
				{
					GoogleIAB.consumeProduct(purchase.productId);
				}
				else
				{
					PurchaseIAPHandler(null, error.ToString(), null, false);
				}
			});
		}
		else
		{
			GoogleIAB.consumeProduct(purchase.productId);
		}
	}

	private void AndroidPurchaseFailedHandler(string error, int code)
	{
		if (string.IsNullOrEmpty(error))
		{
			error = "Error is null";
		}
		bool didCancel = code == 1;
		PurchaseIAPHandler(null, error, null, didCancel);
	}

	protected void QueryInventorySucceededHandler(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		DebugLog("QueryInventorySucceededHandler() purchases.count=" + purchases.Count);
		List<IAPProduct> list = new List<IAPProduct>();
		foreach (GoogleSkuInfo sku in skus)
		{
			list.Add(new IAPProduct(sku));
		}
		ProductsLoadComplete(list);
		if (this.GoogleInventoryPurchaseFoundEvent != null)
		{
			foreach (GooglePurchase purchase in purchases)
			{
				this.GoogleInventoryPurchaseFoundEvent(purchase.productId);
			}
		}
		foreach (GooglePurchase purchase2 in purchases)
		{
			DebugLog("Purchase found: " + purchase2.ToString());
			if (purchase2.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				_purchasesPendingConsume.Add(purchase2);
			}
		}
		if (_purchasesPendingConsume.Count > 0)
		{
			DebugLog("Found " + _purchasesPendingConsume.Count + " purchases that need consuming, starting consume loop");
			ConsumeNextPendingPurchase();
		}
	}

	private void ConsumeNextPendingPurchase()
	{
		if (_purchasesPendingConsume.Count != 0)
		{
			GooglePurchase googlePurchase = _purchasesPendingConsume[0];
			_purchasesPendingConsume.RemoveAt(0);
			GoogleIAB.consumeProduct(googlePurchase.productId);
		}
	}

	protected void ConsumeIAPSuccessfulHandler(GooglePurchase transaction)
	{
		DebugLog("consume successful. product: " + transaction);
		PurchaseIAPHandler(transaction.productId, null, null, false);
		ConsumeNextPendingPurchase();
	}

	protected void ConsumeIAPFailedHandler(string error)
	{
		DebugLog("consume failed with error: " + error);
		PurchaseIAPHandler(null, error, null, false);
		ConsumeNextPendingPurchase();
	}

	protected void BillingSupportedHandler()
	{
		DebugLog("BillingSupportedHandler()");
		billingSupported = true;
		LoadProducts();
	}

	protected void BillingNotSupportedHandler(string val)
	{
		DebugLog("BillingNotSupportedHandler(): " + val);
		billingSupported = false;
	}
}
