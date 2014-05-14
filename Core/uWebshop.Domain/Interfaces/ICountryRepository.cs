using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICountryRepository
	{
		/// <summary>
		/// Gets all countries.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		List<Country> GetAllCountries(ILocalization localization = null);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IVATCountryRepository : ICountryRepository
	{
	}
}