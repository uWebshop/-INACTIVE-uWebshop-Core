using System.Globalization;
using System.Web;
using System.Xml;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	// untestable
	class ConfigFileDefaultCurrencyCultureService : IDefaultCurrencyCultureService
	{
		public CultureInfo GetCultureForCurrency(string currencyCode)
		{
		    if (HttpContext.Current != null)
			// todo: cache, including a file watch
		    {
		        if (
		            !System.IO.File.Exists(
		                HttpContext.Current.Server.MapPath("/App_Plugins/uWebshop/config/CurrencyCultures.config")))
		        {
		            Log.Instance.LogWarning("Could not find CurrencyCultures.config file in /config/uWebshop folder.");
		            return null;
		        }

		        var doc = new XmlDocument();
		        doc.Load(HttpContext.Current.Server.MapPath("/App_Plugins/uWebshop/config/CurrencyCultures.config"));

		        XmlNode providerNode =
		            doc.SelectSingleNode(
		                string.Format(
		                    "//currency[translate(@code, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='{0}']",
		                    currencyCode.ToLower()));

		        if (providerNode == null)
		        {
		            Log.Instance.LogWarning(
		                string.Format(
		                    "Could not find currency with code: {0} in /App_Plugins/uWebshop/config/CurrencyCultures.config",
		                    currencyCode));

		            return null;
		        }

		        if (providerNode.Attributes != null && providerNode.Attributes["culture"] != null)
		            return new CultureInfo(providerNode.Attributes["culture"].Value);
		    }
		    return null;
		}
	}
}