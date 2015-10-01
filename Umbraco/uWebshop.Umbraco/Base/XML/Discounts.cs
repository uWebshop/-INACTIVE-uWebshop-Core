using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.Core.Macros;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Discounts")]
	public class Discounts
	{
		/// <summary>
		/// Gets all order discounts.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllOrderDiscounts(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Discounts.GetAllOrderDiscounts(storeAlias, currencyCode);

			dictionary.Add("Discount", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllOrderDiscounts", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all discounts for the given order
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetDiscountsForOrder(string orderGuid, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Discounts.GetDiscountsForOrder(orderGuid, storeAlias, currencyCode);

			dictionary.Add("Discount", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetDiscountsForOrder", dictionary, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the discount for product.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetDiscountForProduct(int productId, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Discounts.GetDiscountForProduct(productId, storeAlias, currencyCode);

			dictionary.Add("Discount", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetDiscountForProduct", dictionary, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the discount for product variant.
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetDiscountForProductVariant(int variantId, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Discounts.GetDiscountForProductVariant(variantId, storeAlias, currencyCode);

			dictionary.Add("Discount", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetDiscountForProductVariant", dictionary, true)).CreateNavigator();
		}
	}
}
