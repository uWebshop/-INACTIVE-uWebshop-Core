using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Test.Stubs
{
	internal class StubLocalization : ILocalization
	{
		private static Lazy<ILocalization> Factory = new Lazy<ILocalization>(() => Localization.CreateLocalization(StubStore.CreateDefaultStore(), "EUR"));
		public static ILocalization Default
		{
			get { return Factory.Value; }
		}

		public string StoreAlias { get; set; }
		public IStore Store { get; set; }
		public string CurrencyCode { get; set; }
		public ICurrency Currency { get; set; }
	}
}