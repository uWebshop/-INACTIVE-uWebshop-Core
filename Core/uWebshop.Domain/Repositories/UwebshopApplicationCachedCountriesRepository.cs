using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Repositories
{
	/// <summary>
	/// 
	/// </summary>
	internal class UwebshopApplicationCachedCountriesRepository : ICountryRepository
	{
		private readonly ConcurrentDictionary<string, List<Country>> _cache = new ConcurrentDictionary<string, List<Country>>();

		protected virtual string BaseXMLFileName
		{
			get { return "countries"; }
		}

		/// <summary>
		/// Gets all countries.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		public List<Country> GetAllCountries(ILocalization localization = null)
		{
			// todo: multicurrency maybe?
			var storeAlias = "none";
			if (localization != null && localization.Store != null)
			{
				storeAlias = localization.StoreAlias;
			}
			if (string.IsNullOrEmpty(storeAlias)) storeAlias = "none";
			return _cache.GetOrAdd(storeAlias, s =>
				{
					// future todo: make file location configurable (web.config or through code)
					var path = HttpContext.Current.Server.MapPath(string.Format("/scripts/uWebshop/" + BaseXMLFileName + "_{0}.xml", storeAlias));
					if (!System.IO.File.Exists(path))
					{
						// eventueel zou je in dit geval eigenlijk niet de storeAlias willen aanpassen naar none?
						path = HttpContext.Current.Server.MapPath("/scripts/uWebshop/" + BaseXMLFileName + ".xml");
					}
					if (!System.IO.File.Exists(path))
						return DotNETFrameworkFallback(storeAlias);

					XDocument doc;
					using (var streamReader = new StreamReader(path, new UTF8Encoding()))
					{
						doc = XDocument.Load(streamReader);
					}
					return doc.Descendants("country").Select(country => new Country {Name = country.Value, Code = country.Attribute("code").Value}).ToList();
				});
		}

		/// <summary>
		/// Dots the net framework fallback.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		protected virtual List<Country> DotNETFrameworkFallback(string storeAlias)
		{
			Store store = StoreHelper.GetByAlias(storeAlias);

			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;

			if (store != null)
			{
				Thread.CurrentThread.CurrentCulture = store.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = store.CultureInfo;
			}

			var cultureList = new Dictionary<string, string>();

			foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			{
				var region = new RegionInfo(culture.LCID);

				if (!(cultureList.ContainsKey(region.TwoLetterISORegionName)))
					cultureList.Add(region.TwoLetterISORegionName, region.DisplayName);
			}

			List<Country> countries = cultureList.Select(culture => new Country {Name = culture.Value, Code = culture.Key}).Where(country => !string.IsNullOrEmpty(country.Name)).OrderBy(country => country.Name).ToList();

			if (store != null)
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}

			return countries;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class UwebshopApplicationCachedVATCountriesRepository : UwebshopApplicationCachedCountriesRepository, IVATCountryRepository
	{
		/// <summary>
		/// Gets the name of the base XML file.
		/// </summary>
		/// <value>
		/// The name of the base XML file.
		/// </value>
		protected override string BaseXMLFileName
		{
			get { return "VATcountries"; }
		}

		/// <summary>
		/// Dots the net framework fallback.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">For VAT countries a configuration file named VATcountries.xml is required</exception>
		protected override List<Country> DotNETFrameworkFallback(string storeAlias)
		{
			throw new Exception("For VAT countries a configuration file named VATcountries.xml is required");
		}
	}
}