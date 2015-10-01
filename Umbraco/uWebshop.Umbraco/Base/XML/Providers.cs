using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.Core.Macros;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Providers")]
	public class Providers
	{
		/// <summary>
		/// Gets all payment providers.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllPaymentProviders(string storeAlias = null,string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();
			var item = API.Providers.GetAllPaymentProviders(storeAlias, currencyCode);

			dictionary.Add("PaymentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllPaymentProviders", dictionary, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the payment providers for a basket by unique order id (Guid)
		/// </summary>
		/// <param name="guidAsString">The guid for the order as string</param>
		/// <param name="useZone">if set to <c>true</c> [use zone].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetPaymentProvidersForOrder(string guidAsString, bool useZone = true,string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Providers.GetPaymentProvidersForOrder(guidAsString, useZone, storeAlias, currencyCode);

			dictionary.Add("PaymentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetPaymentProvidersForOrder", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all fulfillment providers. (ie shipping/pickup/etc)
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllFulfillmentProviders(string storeAlias = null,string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Providers.GetAllFulfillmentProviders(storeAlias, currencyCode);

			dictionary.Add("FulfillmentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllFulfillmentProviders", dictionary, true)).CreateNavigator();
		}


		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc) for a basket by Unique Order Id (Guid)
		/// </summary>
		/// <param name="guidAsString">The guid for the order as string</param>
		/// <param name="useZone">if set to <c>true</c> use zone/countries.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetFulfillmentProvidersForOrder(string guidAsString, bool useZone = true, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Providers.GetFulfillmentProvidersForOrder(guidAsString, useZone, storeAlias, currencyCode);

			dictionary.Add("FulfillmentProvider", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetFulfillmentProvidersForOrder", dictionary, true)).CreateNavigator();
		}
	}
}
