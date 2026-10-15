using System.Collections.Generic;
using UnityEngine.UI;

public class MPCouponBuyPopupUI : UIBase
{
	public delegate void CouponBuyDelegate();

	public delegate void CouponBuyProductClickedDelegate(string productId);

	private Text _firstProductPriceLabel;

	private Text _secondProductPriceLabel;

	private Text _thirdProductPriceLabel;

	private Text _fourthProductPriceLabel;

	private Text _fithProductPriceLabel;

	private Text _sixthProductPriceLabel;

	public event CouponBuyDelegate backClickedEvent;

	public event CouponBuyProductClickedDelegate ProductClickedEvent;

	protected override void LateAwake()
	{
		base.LateAwake();
		_firstProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton1/Price").GetComponent<Text>();
		_secondProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton2/Price").GetComponent<Text>();
		_thirdProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton3/Price").GetComponent<Text>();
		_fourthProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton4/Price").GetComponent<Text>();
		_fithProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton5/Price").GetComponent<Text>();
		_sixthProductPriceLabel = base.transform.Find("Box/ItemMenu/Container/CouponsButton6/Price").GetComponent<Text>();
	}

	public override void SetLabels()
	{
		base.transform.Find("Box/Label").GetComponent<Text>().text = MPTextDefs.MP_COUPONS_BUY_DESCRIPTION;
		SetProductLabels();
		SetPercentOffLabels();
		SetQuantityLabels();
	}

	private void SetProductLabels()
	{
		IAPProduct loadedProductInfo = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon01");
		IAPProduct loadedProductInfo2 = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon05");
		IAPProduct loadedProductInfo3 = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon15");
		IAPProduct loadedProductInfo4 = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon40");
		IAPProduct loadedProductInfo5 = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon125");
		IAPProduct loadedProductInfo6 = App.iap.GetLoadedProductInfo("com.adultswim.pocketmorties.clubrickcoupon300");
		_firstProductPriceLabel.text = ((loadedProductInfo != null) ? loadedProductInfo.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
		_secondProductPriceLabel.text = ((loadedProductInfo2 != null) ? loadedProductInfo2.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
		_thirdProductPriceLabel.text = ((loadedProductInfo3 != null) ? loadedProductInfo3.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
		_fourthProductPriceLabel.text = ((loadedProductInfo4 != null) ? loadedProductInfo4.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
		_fithProductPriceLabel.text = ((loadedProductInfo5 != null) ? loadedProductInfo5.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
		_sixthProductPriceLabel.text = ((loadedProductInfo6 != null) ? loadedProductInfo6.priceWithSymbol : TextDefs.COUPONS_BUY_PRODUCT_LABEL);
	}

	private void SetPercentOffLabels()
	{
		int couponPackSavingPercent = MPIAPDefs.GetCouponPackSavingPercent("com.adultswim.pocketmorties.clubrickcoupon05");
		int couponPackSavingPercent2 = MPIAPDefs.GetCouponPackSavingPercent("com.adultswim.pocketmorties.clubrickcoupon15");
		int couponPackSavingPercent3 = MPIAPDefs.GetCouponPackSavingPercent("com.adultswim.pocketmorties.clubrickcoupon40");
		int couponPackSavingPercent4 = MPIAPDefs.GetCouponPackSavingPercent("com.adultswim.pocketmorties.clubrickcoupon125");
		int couponPackSavingPercent5 = MPIAPDefs.GetCouponPackSavingPercent("com.adultswim.pocketmorties.clubrickcoupon300");
		if (couponPackSavingPercent >= 0)
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton2/Percent").gameObject.SetActive(true);
			base.transform.Find("Box/ItemMenu/Container/CouponsButton2/Percent/Label").GetComponent<Text>().text = ((couponPackSavingPercent != 0) ? string.Format(TextDefs.COUPONS_BUY_PERCENTAGE_STICKER, couponPackSavingPercent.ToString()) : "-");
		}
		else
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton2/Percent").gameObject.SetActive(false);
		}
		if (couponPackSavingPercent2 >= 0)
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton3/Percent").gameObject.SetActive(true);
			base.transform.Find("Box/ItemMenu/Container/CouponsButton3/Percent/Label").GetComponent<Text>().text = ((couponPackSavingPercent2 != 0) ? string.Format(TextDefs.COUPONS_BUY_PERCENTAGE_STICKER, couponPackSavingPercent2.ToString()) : "-");
		}
		else
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton3/Percent").gameObject.SetActive(false);
		}
		if (couponPackSavingPercent3 >= 0)
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton4/Percent").gameObject.SetActive(true);
			base.transform.Find("Box/ItemMenu/Container/CouponsButton4/Percent/Label").GetComponent<Text>().text = ((couponPackSavingPercent3 != 0) ? string.Format(TextDefs.COUPONS_BUY_PERCENTAGE_STICKER, couponPackSavingPercent3.ToString()) : "-");
		}
		else
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton4/Percent").gameObject.SetActive(false);
		}
		if (couponPackSavingPercent4 >= 0)
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton5/Percent").gameObject.SetActive(true);
			base.transform.Find("Box/ItemMenu/Container/CouponsButton5/Percent/Label").GetComponent<Text>().text = ((couponPackSavingPercent4 != 0) ? string.Format(TextDefs.COUPONS_BUY_PERCENTAGE_STICKER, couponPackSavingPercent4.ToString()) : "-");
		}
		else
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton5/Percent").gameObject.SetActive(false);
		}
		if (couponPackSavingPercent5 >= 0)
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton6/Percent").gameObject.SetActive(true);
			base.transform.Find("Box/ItemMenu/Container/CouponsButton6/Percent/Label").GetComponent<Text>().text = ((couponPackSavingPercent5 != 0) ? string.Format(TextDefs.COUPONS_BUY_PERCENTAGE_STICKER, couponPackSavingPercent5.ToString()) : "-");
		}
		else
		{
			base.transform.Find("Box/ItemMenu/Container/CouponsButton6/Percent").gameObject.SetActive(false);
		}
	}

	private void SetQuantityLabels()
	{
		base.transform.Find("Box/ItemMenu/Container/CouponsButton1/Quantity").GetComponent<Text>().text = 1.ToString();
		base.transform.Find("Box/ItemMenu/Container/CouponsButton2/Quantity").GetComponent<Text>().text = 5.ToString();
		base.transform.Find("Box/ItemMenu/Container/CouponsButton3/Quantity").GetComponent<Text>().text = 15.ToString();
		base.transform.Find("Box/ItemMenu/Container/CouponsButton4/Quantity").GetComponent<Text>().text = 40.ToString();
		base.transform.Find("Box/ItemMenu/Container/CouponsButton5/Quantity").GetComponent<Text>().text = 125.ToString();
		base.transform.Find("Box/ItemMenu/Container/CouponsButton6/Quantity").GetComponent<Text>().text = 300.ToString();
	}

	protected override void AddEvents()
	{
		base.AddEvents();
		App.iap.ProductsLoadSuccessfulEvent += ProductsLoadSuccessfulHandler;
	}

	protected override void RemoveEvents()
	{
		base.RemoveEvents();
		App.iap.ProductsLoadSuccessfulEvent -= ProductsLoadSuccessfulHandler;
	}

	public void FirstPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon01");
		}
	}

	public void SecondPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon05");
		}
	}

	public void ThirdPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon15");
		}
	}

	public void ForthPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon40");
		}
	}

	public void FithPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon125");
		}
	}

	public void SixthPackButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.ProductClickedEvent != null)
		{
			this.ProductClickedEvent("com.adultswim.pocketmorties.clubrickcoupon300");
		}
	}

	public void CloseButtonHandler()
	{
		App.sound.PlaySFX("Audio/SFX/GenericButton");
		if (this.backClickedEvent != null)
		{
			this.backClickedEvent();
		}
	}

	private void ProductsLoadSuccessfulHandler(List<IAPProduct> products, string error)
	{
		SetProductLabels();
	}
}
