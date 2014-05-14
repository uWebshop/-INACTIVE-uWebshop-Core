using System.Globalization;

namespace uWebshop.Domain.Interfaces
{
	interface ICurrencyCultureInfoForLocalizationService
	{
		CultureInfo GetCurrencyCulture(ILocalization localization);
	}
}