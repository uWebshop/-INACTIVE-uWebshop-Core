using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco;
using uWebshop.Domain;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Catalog")]
	public class Catalog
	{

		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllCategories(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetAllCategories(storeAlias, currencyCode);

			dictionary.Add("Category", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllCategories", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all products.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllProducts(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetAllProducts(storeAlias, currencyCode);

			dictionary.Add("Product", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllProducts", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all product variants.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllProductVariants(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetAllProductVariants(storeAlias, currencyCode);

			dictionary.Add("Variant", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllProductVariants", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetCategory(int categoryId = 0, string storeAlias = null, string currencyCode = null)
		{
			var item = API.Catalog.GetCategory(categoryId, storeAlias, currencyCode);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCategory", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the categories recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetCategoriesRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetCategoriesRecursive(categoryId, storeAlias, currencyCode);

			dictionary.Add("Category", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCategoriesRecursive", dictionary, true)).CreateNavigator();
		}


		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetProduct(int productId = 0, string storeAlias = null, string currencyCode = null)
		{
			var item = API.Catalog.GetProduct(productId, storeAlias, currencyCode);

			return XDocument.Parse(JSONXMLRender.RenderOutput("productId", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the products recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetProductsRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetProductsRecursive(categoryId, storeAlias, currencyCode);

			dictionary.Add("Product", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetProductsRecursive", dictionary, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the product variant.
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetProductVariant(int variantId, string storeAlias = null, string currencyCode = null)
		{
			var item = API.Catalog.GetProductVariant(variantId, storeAlias, currencyCode);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetProductVariant", item, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the products variants recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetProductsVariantsRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Catalog.GetProductsVariantsRecursive(categoryId, storeAlias, currencyCode);

			dictionary.Add("Variant", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetProductsVariantsRecursive", dictionary, true)).CreateNavigator();
		}


		/// <summary>
		/// Generate paging based on the itemcount and the items per page
		/// </summary>
		/// <param name="itemCount">The item count.</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>
		/// Paging Object
		/// </returns>
		public static XPathNavigator GetPages(int itemCount, int itemsPerPage)
		{
			// todo: test is this even possible?
			var item = Paging.GetPages(itemCount, itemsPerPage);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetPages", item, true)).CreateNavigator();
		}
	}
}
