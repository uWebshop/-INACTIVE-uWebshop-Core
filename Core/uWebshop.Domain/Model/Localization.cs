using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Model
{
	internal class Localization : ILocalization
	{
		private Localization()
		{
		}

		public string StoreAlias
		{
			get { return Store.Alias; }
		}

		public IStore Store { get; private set; }

		public string CurrencyCode
		{
			get { return Currency != null ? Currency.ISOCurrencySymbol : string.Empty; }
		}

		public ICurrency Currency { get; private set; }

		public static ILocalization CreateLocalization(IStore store, string currencyCode = null)
		{
			currencyCode = currencyCode != null ? currencyCode.ToUpperInvariant() : store.DefaultCurrencyCultureSymbol.ToUpperInvariant();

		    if (store.Currencies != null)
		    {
		        var currenciesOnStore = store.Currencies.FirstOrDefault(c => c.ISOCurrencySymbol == currencyCode);
		        return new Localization {Store = store, Currency = currenciesOnStore};
		    }
            
		    var fallbackCurrency = new DefaultStoreCurrency
		    {
		        ISOCurrencySymbol = store.DefaultCurrencyCultureSymbol,
		        Ratio = 1,
		        CurrencySymbol = API.Store.GetCurrencySymbol(store.DefaultCurrencyCultureSymbol)
		    };

		    return new Localization {Store = store, Currency = fallbackCurrency };
			
		}
		// todo: name/location check
		internal static ILocalization ForceCreateLocalization(Store store, string currencyCode = null)
		{
			var symbol = string.IsNullOrEmpty(currencyCode) ? "?" : API.Store.GetCurrencySymbol(currencyCode);
			return new Localization { Store = store, Currency = new DefaultStoreCurrency { ISOCurrencySymbol = currencyCode, Ratio = 1, CurrencySymbol = symbol } };
		}

		private class DefaultStoreCurrency : ICurrency
		{
			public string ISOCurrencySymbol { get; set; }
			public string CurrencySymbol { get; set; }
			public decimal Ratio { get; set; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as ILocalization;
			if (other == null) return false;
			return CurrencyCode == other.CurrencyCode && StoreAlias == other.StoreAlias;
		}
		public override int GetHashCode()
		{
			return CurrencyCode.GetHashCode() * 7 + StoreAlias.GetHashCode();
		}

		//public static ILocalization CreateLocalization(string storeAlias, string currencyCode = null)
		//{
		//	if (currencyCode != null) currencyCode = currencyCode.ToUpperInvariant();
		//	var store = StoreHelper.GetByAlias(storeAlias);
		//	if (store == null || string.IsNullOrEmpty(store.Alias)) throw new Exception("Store with alias " + storeAlias +	" not found");
		//	return new Localization { Store = store, CurrencyCode = currencyCode ?? store.CurrencyCultureSymbol, StoreAlias = store.Alias, };
		//}
	}
}