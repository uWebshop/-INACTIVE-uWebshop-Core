using System;
using System.Globalization;
using System.Linq;
using System.Web;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	class CurrencyCultureInfoForLocalizationService : ICurrencyCultureInfoForLocalizationService
	{
		private readonly IDefaultCurrencyCultureService _defaultCurrencyCultureService;
		private readonly IUwebshopRequestService _requestService;

		public CurrencyCultureInfoForLocalizationService(IDefaultCurrencyCultureService	defaultCurrencyCultureService, IUwebshopRequestService requestService)
		{
			_defaultCurrencyCultureService = defaultCurrencyCultureService;
			_requestService = requestService;
		}

		public CultureInfo GetCurrencyCulture(ILocalization localization)
		{
			var request = _requestService.Current;
			CultureInfo result;
			if (request.CurrencyCultures.TryGetValue(localization, out result))
			{
				return result;
			}

			var requestCulture = ResolveCulture();
			if (requestCulture != null && new RegionInfo(requestCulture.LCID).ISOCurrencySymbol == localization.CurrencyCode)
			{
				request.CurrencyCultures.Add(localization, requestCulture);
				return requestCulture;
			}
			var storeCulture = localization.Store.DefaultCurrencyCultureInfo;
			if (new RegionInfo(storeCulture.LCID).ISOCurrencySymbol == localization.CurrencyCode)
			{
				request.CurrencyCultures.Add(localization, storeCulture);
				return storeCulture;
			}

			result = _defaultCurrencyCultureService.GetCultureForCurrency(localization.CurrencyCode) ?? CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(c => new RegionInfo(c.LCID).ISOCurrencySymbol == localization.CurrencyCode);
			if (result == null)
			{
				Log.Instance.LogError("Can't determine culture for currency with code " + localization.CurrencyCode + "");
				result = localization.Store.DefaultCurrencyCultureInfo;
			}
			request.CurrencyCultures.Add(localization, result);
			return result;
		}
		
		internal static CultureInfo ResolveCulture()
		{
			string[] languages = HttpContext.Current.Request.UserLanguages;

			if (languages == null || languages.Length == 0)
				return null;

			try
			{
				var language = languages[0].ToLowerInvariant().Trim();
				return CultureInfo.CreateSpecificCulture(language);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
	}
}