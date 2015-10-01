using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.Core.Macros;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Basket")]
	public class Basket
	{
		/// <summary>
		/// Gets the basket.
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetBasket()
		{
			var basket = API.Basket.GetBasket();

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetBasket", basket, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the basket.
		/// </summary>
		/// <param name="guidAsString">The unique identifier.</param>
		/// <returns></returns>
		public static XPathNavigator GetBasket(string guidAsString)
		{
			var basket = API.Basket.GetBasket(guidAsString);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetBasket", basket, true)).CreateNavigator();
		}

		
		/// <summary>
		/// Gets the current basket or create a new basket if basket was confirmed.
		/// Use case: customer clicks back button on payment provider
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetCurrentOrNewBasket()
		{
			var basket = API.Basket.GetCurrentOrNewBasket();

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCurrentOrNewBasket", basket, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc)
		/// </summary>
		/// <param name="useZone">if set to <c>true</c> use zone/countries.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetFulfillmentProviders(bool useZone = true, string storeAlias = null,string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Basket.GetFulfillmentProviders(useZone, storeAlias, currencyCode);

			dictionary.Add("FillmentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetFulfillmentProviders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the payment providers.
		/// </summary>
		/// <param name="useZone">if set to <c>true</c> [use zone].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetPaymentProviders(bool useZone = true, string storeAlias = null,string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Basket.GetPaymentProviders(useZone, storeAlias, currencyCode);

			dictionary.Add("PaymentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetPaymentProviders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all order discounts.
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetAllOrderDiscounts(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Basket.GetAllOrderDiscounts(storeAlias, currencyCode);

			dictionary.Add("OrderDiscount", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllOrderDiscounts", dictionary, true)).CreateNavigator();
		}
	}
}
