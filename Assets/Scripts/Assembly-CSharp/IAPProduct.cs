using Prime31;

public class IAPProduct
{
	public string productId { get; private set; }

	public string title { get; private set; }

	public string priceWithSymbol { get; private set; }

	public string description { get; private set; }

	public string currencyCode { get; private set; }

	public string currencySymbol { get; private set; }

	public float price { get; private set; }

	public IAPProduct(GoogleSkuInfo prod)
	{
		productId = prod.productId;
		title = prod.title;
		priceWithSymbol = prod.price;
		description = prod.description;
		currencyCode = prod.priceCurrencyCode;
		price = prod.priceAmountMicros;
	}

	public override string ToString()
	{
		return string.Format("[IAPProduct[: productId: {0}, title: {1}, priceWithSymbol: {2}, description: {3}, currencyCode: {4}, currencySymbol: {5}]", productId, title, priceWithSymbol, description, currencyCode, currencySymbol);
	}
}
