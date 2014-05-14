using System.ComponentModel;
using uWebshop.Domain;

namespace uWebshop.Test.Stubs
{
	public class StubStore : Store
	{
		public override string Alias { get; protected internal set; }

		public static Store CreateDefaultStore()
		{
			return new StubStore { NodeTypeAlias = "uwbsStore", Alias = "TestStore", DefaultCountryCode = "NL", Id = 1, GlobalVat = 19, 
				CanonicalStoreURL = "http://my.uwebshop.com/", SortOrder = 1, Culture = "nl-NL", CountryCode = "NL", CurrencyCulture = "nl-NL", UseStock = true, 
				UseStoreSpecificStock = false, 
				CurrencyCodes = new[] { "EUR", "USD" }, 
				Currencies = new[]
					{
						new Currency { ISOCurrencySymbol = "EUR", Ratio = 1, CurrencySymbol = "€" }, 
						new Currency { ISOCurrencySymbol = "USD", Ratio = 1.1m, CurrencySymbol = "$" }
					}, };
		}

		private class Currency : ICurrency
		{
			public string ISOCurrencySymbol { get; set; }
			public string CurrencySymbol { get; set; }
			public decimal Ratio { get; set; }
		}
	}
}