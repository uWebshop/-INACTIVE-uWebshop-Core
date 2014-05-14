using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public static class Store
	{
		private static ConcurrentDictionary<string, string> _isoCurrencySymbolMapping = new ConcurrentDictionary<string, string>();

		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <returns></returns>
		public static IStore GetStore()
		{
			return new BasketStore(StoreHelper.GetCurrentStore());
		}

		public static bool SetStore(string storeAlias)
		{
			return StoreHelper.ChangeStore(storeAlias);
		}

		/// <summary>
		/// Gets all stores.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<IStore> GetAllStores()
		{
			return StoreHelper.GetAllStores().Select(s => new BasketStore(s));
		}

		/// <summary>
		/// Get the current localization
		/// </summary>
		/// <returns></returns>
		public static ILocalization GetCurrentLocalization()
		{
			return StoreHelper.GetLocalizationOrCurrent(null, null);
		}

		/// <summary>
		/// Get all the countries from country.xml or country_storealias.xml
		/// If not found a fallback list will be used
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Country> GetAllCountries(string storeAlias = null, string currencyCode = null)
		{
			return
				IO.Container.Resolve<ICountryRepository>()
					.GetAllCountries(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}


		/// <summary>
		/// Get the full country name based on the country code country.xml or country_storealias.xml
		/// returns the given countrycode if no match can be made
		/// </summary>
		/// <returns></returns>
		public static string GetCountryNameFromCountryCode(string countryCode)
		{
            var localisation = GetCurrentLocalization();

            if (localisation != null)
            {
                var localCountries = IO.Container.Resolve<ICountryRepository>().GetAllCountries(localisation);

                if (localCountries != null)
                {
                    var country = localCountries.FirstOrDefault(x => x.Code == countryCode);
            
                    return country != null ? country.Name : countryCode;
                }
            }

            return null;
		}

		/// <summary>
		/// Get currency symbol from ISOcurrencyCode
		/// </summary>
		/// <param name="ISOCurrencySymbol"></param>
		/// <returns></returns>
		public static string GetCurrencySymbol(string ISOCurrencySymbol)
		{
			return _isoCurrencySymbolMapping.GetOrAdd(ISOCurrencySymbol, isoSymbol => CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => !c.IsNeutralCulture).Select(culture =>
				{
					try
					{
						return new RegionInfo(culture.LCID);
					}
					catch
					{
						return null;
					}
				}).Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol).Select(ri => ri.CurrencySymbol).FirstOrDefault() ?? string.Empty);
		}


		/// <summary>
		/// Create a basic IPrice object
		/// </summary>
		/// <param name="price"></param>
		/// <param name="storeAlias"></param>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public static IPrice CreateBasicPrice(int price, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			return Price.CreateBasicPrice(price, localization);
		}

	}
}
