using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Store")]
	public class Store
	{
		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetStore()
		{
			var item = API.Store.GetStore();

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetStore", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all stores.
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetAllStores()
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Store.GetAllStores();

			dictionary.Add("Store", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllStores", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Get the current localization
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetCurrentLocalization()
		{
			var item = API.Store.GetCurrentLocalization();

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCurrentLocalization", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Get all the countries from country.xml or country_storealias.xml
		/// If not found a fallback list will be used
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetAllCountries(string storeAlias = null, string currencyCode = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Store.GetAllCountries(storeAlias, currencyCode);

			dictionary.Add("Country", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllCountries", dictionary, true)).CreateNavigator();
		}


		/// <summary>
		/// Get the full country name based on the country code country.xml or country_storealias.xml
		/// returns the given countrycode if no match can be made
		/// </summary>
		/// <returns></returns>
		public static string GetCountryNameFromCountryCode(string countryCode)
		{
			return API.Store.GetCountryNameFromCountryCode(countryCode);
		}

		/// <summary>
		/// Get currency symbol from ISOcurrencyCode
		/// </summary>
		/// <param name="ISOCurrencySymbol"></param>
		/// <returns></returns>
		public static string GetCurrencySymbol(string ISOCurrencySymbol)
		{
			return API.Store.GetCurrencySymbol(ISOCurrencySymbol);
		}
	}
}
