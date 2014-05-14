using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	///     Helper class with language related functions
	/// </summary>
	public static class StoreHelper
	{
		/// <summary>
		/// All stores cache key
		/// </summary>
		public const string AllStoresCacheKey = "AllStoresCacheKey";

		/// <summary>
		/// The current store alias cache key
		/// </summary>
		public const string CurrentStoreAliasCacheKey = "CurrentStoreAliasCacheKey";

		/// <summary>
		/// The current store alias URL cache key
		/// </summary>
		public const string CurrentStoreAliasUrlCacheKey = "CurrentStoreAliasUrlCacheKey";

		internal static IStoreService StoreService { get; set; }

		/// <summary>
		/// List of all the default uWebshop document types
		/// </summary>
		/// <value>
		/// The store dependant document type alias list.
		/// </value>
		internal static IEnumerable<string> StoreDependantDocumentTypeAliasList
		{
			get { return new List<string> {Category.NodeAlias, Product.NodeAlias, ProductVariant.NodeAlias, DiscountProduct.NodeAlias, DiscountOrder.NodeAlias, ShippingProvider.NodeAlias, ShippingProviderMethod.NodeAlias, PaymentProvider.NodeAlias, PaymentProviderMethod.NodeAlias}; }
		}

		/// <summary>
		/// Gets the current store alias.
		/// </summary>
		/// <value>
		/// The current store alias.
		/// </value>
		public static string CurrentStoreAlias
		{
			get { return StoreService.CurrentStoreAlias(); }
		}

		/// <summary>
		/// Gets the current localization.
		/// </summary>
		/// <value>
		/// The current localization.
		/// </value>
		public static ILocalization CurrentLocalization
		{
			get { return StoreService.GetCurrentLocalization(); }
		}

		/// <summary>
		/// Get the current store
		/// </summary>
		/// <returns></returns>
		public static Store GetCurrentStore()
		{
			return CurrentLocalization.Store as Store;
		}
		
		/// <summary>
		/// Returns a list of all languages
		/// </summary>
		/// <returns>
		/// List of languages
		/// </returns>
		public static IEnumerable<Store> GetAllStores()
		{
			return StoreService.GetAllStores();
		}

		/// <summary>
		/// Gets the store by alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static Store GetByAlias(string alias)
		{
			return StoreService.GetByAlias(alias);
		}

		/// <summary>
		/// Gets the store by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="currencyCode">The currency code. (currently ignored)</param>
		/// <returns></returns>
		public static Store GetById(int id, string currencyCode = null)
		{
			// todo: null storeAlias, make it work, will always be current localization now, currencyCode has no effect
			return StoreService.GetById(id, CurrentLocalization);// GetLocalizationOrCurrent(null, currencyCode)); 
		}

		/// <summary>
		/// Gets the store alias URL.
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use Store.StoreURL")]
		public static string GetStoreAliasUrl()
		{
			return GetCurrentStore().StoreURL;
		}

		/// <summary>
		/// Get the number formatting for the current store
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public static CultureInfo GetCurrentCulture()
		{
			var currentStore = GetCurrentStore();

			var cultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(culture => new {culture, region = new RegionInfo(culture.LCID)}).Where(@t => string.Equals(t.region.ISOCurrencySymbol, currentStore.CurrencyCulture, StringComparison.InvariantCultureIgnoreCase)).Select(@t => t.culture).FirstOrDefault();

			cultureInfo.NumberFormat.CurrencyPositivePattern = 2;
			cultureInfo.NumberFormat.CurrencyNegativePattern = 2;

			return cultureInfo;
		}

		/// <summary>
		/// Get the culture based on the currency code of the store
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete]
		public static CultureInfo GetCultureByCurrencyCode(Store store)
		{
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).First(culture => string.Equals(new RegionInfo(culture.LCID).ISOCurrencySymbol, store.CurrencyCulture, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Gets the localized URL.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use NiceUrl()")]
		public static string GetLocalizedUrl(int id, int categoryId = 0)
		{
			return GetNiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		public static string GetNiceUrl(int id, int categoryId = 0, string storeAlias = null, string currencyCode = null)
		{
			return StoreService.GetNiceUrl(id, categoryId, GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Renames the store.
		/// </summary>
		/// <param name="oldStoreAlias">The old store alias.</param>
		/// <param name="newStoreAlias">The new store alias.</param>
		public static void RenameStore(string oldStoreAlias, string newStoreAlias)
		{
			StoreService.RenameStore(oldStoreAlias, newStoreAlias);
		}

		/// <summary>
		/// Install a new uWebshop store
		/// </summary>
		/// <param name="storeAlias">the store alias to use</param>
		public static void UnInstallStore(string storeAlias)
		{
			IO.Container.Resolve<IUmbracoDocumentTypeInstaller>().UnInstallStore(storeAlias);
		}

		/// <summary>
		/// Gets the multi store item.
		/// </summary>
		/// <param name="contentId">The content unique identifier.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static string GetMultiStoreItem(int contentId, string propertyAlias, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<ICMSApplication>().GetMultiStoreContentProperty(contentId, propertyAlias, GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Gets the multi store disable.
		/// </summary>
		/// <param name="contentId">The content unique identifier.</param>
		/// <returns></returns>
		public static bool GetMultiStoreDisable(int contentId)
		{
			var value = IO.Container.Resolve<ICMSApplication>().GetMultiStoreContentProperty(contentId, "disable", null, true);
			return value == "1" || value == "true";
		}

		internal static bool ReturnCachedFieldOrLoadValueUsingGetMultiStoreExamineAndUpdateField(int entityId, string fieldName, ref bool? field)
		{
			var newValue = GetMultiStoreItem(entityId, fieldName);
			return field ?? (field = newValue == "enable" || newValue == "1" || newValue == "true" || newValue == string.Empty).GetValueOrDefault();
		}

		internal static bool ReturnCachedFieldOrLoadValueAndUpdateField(string newValue, ref bool? field)
		{
			return field ?? (field = newValue == "enable" || newValue == "1" || newValue == "true" || newValue == string.Empty).GetValueOrDefault();
		}

		internal static bool GetMultiStoreDisableExamine(ILocalization localization, IPropertyProvider fields)
		{
			const string propertyAlias = "disable";
			Func<string, bool> valueCheck = value => value == "1" || value == "true";
			if (fields.ContainsKey(propertyAlias) && valueCheck(fields.GetStringValue(propertyAlias)))
			{
				return true;
			}

			var val = ReadMultiStoreItemFromPropertiesDictionary(propertyAlias, localization, fields);
			return valueCheck(val);
		}

		internal static string CreateMultiStorePropertyAlias(string propertyAlias, string storeAlias)
		{
			return string.Format("{0}_{1}", propertyAlias, storeAlias);
		}

		internal static string MakeRTEItemPropertyAliasIfApplicable(string propertyAlias)
		{
			return propertyAlias.StartsWith("description") ? "RTEItem" + propertyAlias : propertyAlias;
		}

		internal static string ReadMultiStoreItemFromPropertiesDictionary(string propertyAlias, ILocalization localization, IPropertyProvider fields)
		{
			var currencyCode = localization.CurrencyCode;
			var storeAlias = localization.StoreAlias;
			var multiStoreAlias = CreateMultiStorePropertyAlias(propertyAlias, storeAlias);
			if (!string.IsNullOrEmpty(currencyCode))
			{
				if (!string.IsNullOrEmpty(storeAlias))
				{
					var multiStoreMultiCurrenyAlias = CreateFullLocalizedPropertyAlias(propertyAlias, localization);
					if (fields.ContainsKey(multiStoreMultiCurrenyAlias))
					{
						var value = fields.GetStringValue(multiStoreMultiCurrenyAlias);

						if (!string.IsNullOrEmpty(value))
						{
							return value;
						}
					}
				}
				var multiCurrencyAlias = CreateMultiStorePropertyAlias(propertyAlias, currencyCode);
				if (fields.ContainsKey(multiCurrencyAlias))
				{
					var value = fields.GetStringValue(multiCurrencyAlias);

					if (!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
			}

			if (!string.IsNullOrEmpty(storeAlias))
			{
				if (fields.ContainsKey(multiStoreAlias))
				{
					var value = fields.GetStringValue(multiStoreAlias);

					if (!string.IsNullOrEmpty(value))
					{
						return value;
					}
				}
			}
			if (fields.ContainsKey(propertyAlias))
			{
				var value = fields.GetStringValue(propertyAlias);

				return value ?? string.Empty;
			}
			return string.Empty;
		}

		internal static int GetMultiStoreIntValue(string propertyName, ILocalization localization, IPropertyProvider fields, int defaultValue = 0)
		{
			var propertyValue = ReadMultiStoreItemFromPropertiesDictionary(propertyName, localization, fields);
			int intValue;
			return int.TryParse(propertyValue, out intValue) ? intValue : defaultValue;
		}

		internal static double GetMultiStoreDoubleValue(string propertyName, ILocalization localization, IPropertyProvider fields, double defaultValue = 0)
		{
			var propertyValue = ReadMultiStoreItemFromPropertiesDictionary(propertyName, localization, fields);
			double doubleValue;
			return Double.TryParse(propertyValue, out doubleValue) ? doubleValue : defaultValue;
		}

		internal static decimal GetMultiStoreDecimalValue(string propertyName, ILocalization localization, IPropertyProvider fields, decimal defaultValue = 0)
		{
			var propertyValue = ReadMultiStoreItemFromPropertiesDictionary(propertyName, localization, fields);
			decimal decimalValue;
			return Decimal.TryParse(propertyValue, out decimalValue) ? decimalValue : defaultValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static int GetMultiStoreStock(int id)
		{
			// todo: multi store validation
			var currentStore = GetCurrentStore();

			return currentStore != null && currentStore.UseStoreSpecificStock ? UWebshopStock.GetStock(id, currentStore.Alias) : UWebshopStock.GetStock(id);
		}

		/// <summary>
		/// Returns all the products of the category, including any sublevel categories
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static List<API.IProduct> GetProductsRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			return GetProductsRecursive(IO.Container.Resolve<ICategoryService>().GetById(categoryId, GetLocalizationOrCurrent(storeAlias, currencyCode)));
		}

		/// <summary>
		/// Returns all the products of the category, including any sublevel categories
		/// </summary>
		/// <param name="category">The category.</param>
		/// <returns></returns>
		public static List<API.IProduct> GetProductsRecursive(Category category)
		{
			var productList = new List<API.IProduct>();
			GetProductsFromCategory(productList, category);
			return productList;
		}

		private static void GetProductsFromCategory(List<API.IProduct> productList, API.ICategory category)
		{
			productList.AddRange(category.Products);

			foreach (var subCategory in category.SubCategories)
			{
				GetProductsFromCategory(productList, subCategory); // todo: this can loop if the category relations do not form a tree
			}
		}

		/// <summary>
		/// Returns all the categories of the category, including any sublevel categories
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static List<Category> GetCategoriesRecursive(int categoryId, string storeAlias = null)
		{
			var categoryList = new List<Category>();
			GetCategoriesFromCategory(categoryList, DomainHelper.GetCategoryById(categoryId, storeAlias));
			return categoryList;
		}

		private static void GetCategoriesFromCategory(ICollection<Category> categoryList, Category mainCategory)
		{
			foreach (Category category in mainCategory.SubCategories.Where(category => categoryList.All(x => x.Id != category.Id)))
			{
				categoryList.Add(category);
			}

			if (!mainCategory.SubCategories.Any()) return;
			foreach (Category subCategory in mainCategory.SubCategories)
			{
				GetCategoriesFromCategory(categoryList, subCategory); // todo: this can loop if the category relations do not form a tree
			}
		}

		/// <summary>
		/// Returns a list of all countries
		/// </summary>
		/// <returns>
		/// List of countries
		/// </returns>
		public static List<Country> GetAllCountries()
		{
			return IO.Container.Resolve<ICountryRepository>().GetAllCountries();
		}

		/// <summary>
		/// Returns a list of all countries in the provided storeAlias
		/// </summary>
		/// <param name="storeAlias">Alias of current store</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns>
		/// List of countries
		/// </returns>
		public static List<Country> GetAllCountries(string storeAlias, string currencyCode = null)
		{
			return IO.Container.Resolve<ICountryRepository>().GetAllCountries(GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Returns a list of all regions
		/// </summary>
		/// <returns>
		/// List of countries
		/// </returns>
		public static List<Region> GetAllRegions()
		{
			return GetAllRegionsForStore("none");
		}

		/// <summary>
		/// Returns a list of all regions in the provided shopalias
		/// </summary>
		/// <param name="storeAlias">Alias of current language</param>
		/// <returns>
		/// List of countries
		/// </returns>
		public static List<Region> GetAllRegionsForStore(string storeAlias)
		{
			var regions = new List<Region>();

			var regionsXml = "/scripts/regions.xml";
			if (storeAlias != "none")
			{
				regionsXml = string.Format("/scripts/regions_{0}.xml", storeAlias);
			}

			if (System.IO.File.Exists(regionsXml))
			{
				var doc = XDocument.Load(HttpContext.Current.Server.MapPath(regionsXml));

				foreach (var country in doc.Descendants("country"))
				{
					var xAttribute = country.Attribute("code");
					if (xAttribute != null)
						regions.Add(new Region {Name = country.Value, Code = xAttribute.Value});
				}
			}

			return regions;
		}

		/// <summary>
		/// Gets the localization.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static ILocalization GetLocalization(string storeAlias, string currencyCode)
		{
			var store = GetByAlias(storeAlias);
			if (store != null)
			{
				//if (store.CurrencyCodes.Contains(currencyCode))
				return Localization.CreateLocalization(store, currencyCode);
				//else
				//	return Localization.CreateLocalization(store);
			}
			return null;
		}

		/// <summary>
		/// Gets the localization original current.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static ILocalization GetLocalizationOrCurrent(string storeAlias, string currencyCode)
		{
			return GetLocalization(storeAlias, currencyCode) ?? CurrentLocalization;
		}

		internal static string CreateFullLocalizedPropertyAlias(string propertyAlias, ILocalization localization)
		{
			return CreateMultiStorePropertyAlias(CreateMultiStorePropertyAlias(propertyAlias, localization.StoreAlias), localization.CurrencyCode);
		}

		internal static int GetLocalizedPrice(string propertyAlias, ILocalization localization, IPropertyProvider fields)
		{
			int price;
			var fullLocalizedPricePropetryAlias = CreateFullLocalizedPropertyAlias(propertyAlias, localization);
			if (fields.ContainsKey(fullLocalizedPricePropetryAlias) && int.TryParse(fields.GetStringValue(fullLocalizedPricePropetryAlias), out price))
			{
				return price;
			}
			var localizedPricePropertyAlias = CreateMultiStorePropertyAlias(propertyAlias, localization.CurrencyCode);
			if (fields.ContainsKey(localizedPricePropertyAlias) && int.TryParse(fields.GetStringValue(localizedPricePropertyAlias), out price))
			{
				return price;
			}
			price = GetMultiStoreIntValue(propertyAlias, localization, fields);
			return LocalizePrice(price, localization);
		}

		internal static int LocalizePrice(int price, ILocalization localization)
		{
			if (localization != null && localization.Currency != null && localization.Currency.Ratio > 0) return (int) (price*localization.Currency.Ratio);
			return price;
		}

		internal static List<Range> LocalizeRanges(List<Range> ranges, ILocalization localization)
		{
			ranges.ForEach(r => r.PriceInCents = LocalizePrice(r.PriceInCents, localization));
			return ranges;
		}

		/// <summary>
		/// Gets the currency culture.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static CultureInfo GetCurrencyCulture(ILocalization localization)
		{
			return IO.Container.Resolve<ICurrencyCultureInfoForLocalizationService>().GetCurrencyCulture(localization);
		}

		internal static bool ChangeStore(string storeAlias)
		{
			var currencyCode = CurrentLocalization.CurrencyCode;

			var result = GetLocalizationOrCurrent(storeAlias, currencyCode);

			if (result != null)
			{
				var uwebshopRequest = UwebshopRequest.Current;
				uwebshopRequest.Localization = result;
				if (uwebshopRequest.OrderInfo != null && uwebshopRequest.OrderInfo.Status == OrderStatus.Incomplete)
				{
					IO.Container.Resolve<IOrderUpdatingService>().ChangeLocalization(uwebshopRequest.OrderInfo, CurrentLocalization);
				}
				SetStoreInfoCookie(storeAlias, currencyCode);
				return true;
			}
			return false;
		}

		public static bool ChangeCurrency(string currencyCode)
		{
			var storeAlias = CurrentLocalization.StoreAlias;

			var result = GetLocalizationOrCurrent(storeAlias, currencyCode);

			if (result != null)
			{
				UwebshopRequest.Current.Localization = result;
				SetStoreInfoCookie(storeAlias,currencyCode);
				return true;
			}
			return false;
		}

		internal static void SetStoreInfoCookie(string storeAlias, string currency = null, Guid wishlistGuid = default(Guid))
		{
			const string cookieName = "StoreInfo";

			var cookie = HttpContext.Current.Request.Cookies[cookieName] ?? new HttpCookie(cookieName);

			if (!string.IsNullOrEmpty(storeAlias))
			{
				if (string.IsNullOrEmpty(cookie.Values["StoreAlias"]))
				{
					cookie.Values.Add("StoreAlias", storeAlias);
				}
				else
				{
					cookie.Values["StoreAlias"] = storeAlias;
				}
			}

			if (!string.IsNullOrEmpty(currency))
			{
				if (string.IsNullOrEmpty(cookie.Values["StoreAlias"]))
				{
					cookie.Values.Add("Currency", currency);
				}
				else
				{
					cookie.Values["Currency"] = currency;
				}
			}
			if (wishlistGuid != default(Guid))
			{
				if (string.IsNullOrEmpty(cookie.Values["Wishlist"]))
				{
					cookie.Values.Add("Wishlist", wishlistGuid.ToString());
				}
				else
				{
					cookie.Values["Wishlist"] = wishlistGuid.ToString();
				}
				
			}

			HttpContext.Current.Response.Cookies.Set(cookie);
		}
	}
}