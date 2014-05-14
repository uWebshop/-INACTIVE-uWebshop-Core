using System.Globalization;

namespace uWebshop.Domain.Interfaces
{
	interface IDefaultCurrencyCultureService
	{
		CultureInfo GetCultureForCurrency(string currencyCode);
	}
}