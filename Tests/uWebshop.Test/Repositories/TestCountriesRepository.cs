using System.Collections.Generic;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Repositories
{
	public class TestCountryRepository : ICountryRepository
	{
		public List<Country> GetAllCountries()
		{
			return new List<Country> {new Country {Code = "NL", Name = "Netherlands"}, new Country {Code = "GB", Name = "United Kingdom"}, new Country {Code = "US", Name = "United States"}, new Country {Code = "MX", Name = "Mexico"},};
		}

		public List<Country> GetAllCountries(ILocalization storeAlias)
		{
			return GetAllCountries();
		}
	}

	public class TestVATCountryRepository : IVATCountryRepository
	{
		public List<Country> GetAllCountries()
		{
			return new List<Country> {new Country {Code = "NL", Name = "Netherlands"}, new Country {Code = "GB", Name = "United Kingdom"},};
		}

		public List<Country> GetAllCountries(ILocalization storeAlias)
		{
			return GetAllCountries();
		}
	}
}