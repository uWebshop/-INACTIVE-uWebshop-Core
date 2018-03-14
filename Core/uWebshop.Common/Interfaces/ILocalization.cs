namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILocalization
	{
		/// <summary>
		/// Gets the store alias.
		/// </summary>
		/// <value>
		/// The store alias.
		/// </value>
		string StoreAlias { get; }

		/// <summary>
		/// Gets the store.
		/// </summary>
		/// <value>
		/// The store.
		/// </value>
		IStore Store { get; }

		/// <summary>
		/// Gets the currency code.
		/// </summary>
		/// <value>
		/// The currency code.
		/// </value>
		string CurrencyCode { get; }

		/// <summary>
		/// Gets the currency.
		/// </summary>
		/// <value>
		/// The currency.
		/// </value>
		ICurrency Currency { get; }

    }
}