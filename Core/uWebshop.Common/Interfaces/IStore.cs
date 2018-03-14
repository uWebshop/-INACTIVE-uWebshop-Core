using System.Collections.Generic;
using System.Globalization;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IStore : IUwebshopSortableEntity
	{
		/// <summary>
		/// Gets the store culture
		/// </summary>
		/// <value>
		/// The culture.
		/// </value>
		string Culture { get; }

		/// <summary>
		/// Get the shop alias
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		string Alias { get; }

		/// <summary>
		/// Gets the country code
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		string CountryCode { get; }

		/// <summary>
		/// Gets or sets the default country code.
		/// </summary>
		/// <value>
		/// The default country code.
		/// </value>
		string DefaultCountryCode { get; }

		/// <summary>
		/// Returns the currency culture code (en-US)
		/// </summary>
		/// <value>
		/// The currency culture.
		/// </value>
		string CurrencyCulture { get; }

        /// <summary>
        /// Gets or sets the currencies.
        /// </summary>
        /// <value>
        /// The currencies.
        /// </value>
        IEnumerable<ICurrency> Currencies { get; }

		/// <summary>
		/// Gets the default currency culture information.
		/// </summary>
		/// <value>
		/// The default currency culture information.
		/// </value>
		CultureInfo DefaultCurrencyCultureInfo { get; }

		/// <summary>
		/// Gets the default currency culture symbol.
		/// </summary>
		/// <value>
		/// The default currency culture symbol.
		/// </value>
		string DefaultCurrencyCultureSymbol { get; }

		/// <summary>
		/// Gets a System.Globalization.CultureInfo object that is set to the languagecode and countrycode
		/// </summary>
		/// <value>
		/// The culture information.
		/// </value>
		CultureInfo CultureInfo { get; }

		/// <summary>
		/// Global Vat for all items in the store, when no VAT is specified
		/// </summary>
		/// <value>
		/// The global vat.
		/// </value>
		decimal GlobalVat { get; }

		/// <summary>
		/// Gets or sets a value indicating whether [enable testmode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [enable testmode]; otherwise, <c>false</c>.
		/// </value>
		bool Testmode { get; }

		/// <summary>
		/// Gets or sets the store URL without domain.
		/// </summary>
		/// <value>
		/// The store URL without domain.
		/// </value>
		string StoreUrlWithoutDomain { get; }

		/// <summary>
		/// Email address to sent the orders from
		/// </summary>
		/// <value>
		/// The email address from.
		/// </value>
		string EmailAddressFrom { get; }

		/// <summary>
		/// Name to sent the orders
		/// </summary>
		/// <value>
		/// The name of the email address from.
		/// </value>
		string EmailAddressFromName { get; }

		/// <summary>
		/// Email address to sent the orders from
		/// </summary>
		/// <value>
		/// The email address automatic.
		/// </value>
		string EmailAddressTo { get; }

		/// <summary>
		/// Node the member is redirected to if he needs to change the password
		/// </summary>
		string AccountChangePasswordUrl { get; }
		
		/// <summary>
		/// Gets or sets the store URL.
		/// </summary>
		/// <value>
		/// The store URL.
		/// </value>
		string StoreURL { get; }

		/// <summary>
		/// Gets the nodeIds that have this store picker set
		/// </summary>
		IEnumerable<int> GetConnectedNodes { get; }

	}

	internal interface IStoreInternal : IStore
	{
		
	}
}