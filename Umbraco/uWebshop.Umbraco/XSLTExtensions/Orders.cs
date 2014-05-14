using System;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using uWebshop.Domain.Helpers;
using umbraco;
using System.Globalization;
using umbraco.BusinessLogic;

namespace uWebshop.XSLTExtensions
{
	[XsltExtension("uWebshop.Orders")]
	public class Orders
	{
		public static string GetFullCountryNameFromCountry(string country)
		{
			var Country = StoreHelper.GetAllCountries().FirstOrDefault(x => country != null && x.Code == country);

			if (Country != null) return country != null ? Country.Name : string.Empty;

			return string.Empty;
		}

		public static string GetFullCountryNameFromCountry(string country, string storeAlias)
		{
			var Country = StoreHelper.GetAllCountries(storeAlias).FirstOrDefault(x => country != null && x.Code == country);

			if (Country != null) return country != null ? Country.Name : string.Empty;

			return string.Empty;
		}

		public static string ReplaceCharacters(string value, string toReplace, string replaceWith)
		{
			return value.Replace(toReplace, replaceWith);
		}

		public static XPathNavigator GetOrder(string uniqueOrderId)
		{
			if (!string.IsNullOrEmpty(uniqueOrderId))
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " XSLTExtensions.GetOrder >>>>SQL<<<< SELECT orderInfo");

				var orderInfo = OrderHelper.GetOrderInfo(Guid.Parse(uniqueOrderId));

				var currentCulture = Thread.CurrentThread.CurrentCulture;
				var currentUICulture = Thread.CurrentThread.CurrentUICulture;

				Thread.CurrentThread.CurrentCulture = orderInfo.StoreInfo.CurrencyCultureInfo;
				Thread.CurrentThread.CurrentUICulture = orderInfo.StoreInfo.CurrencyCultureInfo;

				var serializedOrderInfo = DomainHelper.SerializeObjectToXmlString(orderInfo);

				var orderXml = new XmlDocument();

				orderXml.LoadXml(serializedOrderInfo);

				var xmlDoc = orderXml.CreateNavigator();

				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;

				return xmlDoc;
			}

			return new XmlDocument().CreateNavigator();
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// output = .ToString("F)
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents)
		{
			return CentsToPrice(priceInCents, null, false);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents, string cultureInfo)
		{
			return CentsToPrice(priceInCents, cultureInfo, true);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is null output is .ToString("F")
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <param name="useCurrencySign"> </param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents, string cultureInfo, bool useCurrencySign)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			var currentUICulture = Thread.CurrentThread.CurrentUICulture;

			if (!string.IsNullOrEmpty(priceInCents))
			{
				decimal price = 0;

				decimal.TryParse(priceInCents, out price);

				var normalPrice = price/100m;

				if (cultureInfo != null)
				{
					Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

					var outputPrice = Math.Round(normalPrice, 2);

					var outputString = outputPrice.ToString("F");
					if (useCurrencySign)
					{
						outputString = outputPrice.ToString("C");
					}

					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;

					return outputString;
				}
				else
				{
					var outputPrice = Math.Round(normalPrice, 2);

					var outputString = outputPrice.ToString("F");

					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;

					return outputString;
				}
			}

			return priceInCents;
		}
	}
}