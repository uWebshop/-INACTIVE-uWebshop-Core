namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICurrency
	{
		/// <summary>
		/// Gets the ISO currency code.
		/// </summary>
		/// <value>
		/// The ISO currency code.
		/// </value>
		string ISOCurrencySymbol { get; }

		/// <summary>
		/// Gets the currency symbol.
		/// </summary>
		/// <value>
		/// The currency symbol.
		/// </value>
		string CurrencySymbol { get; }

		/// <summary>
		/// Gets the ratio.
		/// </summary>
		/// <value>
		/// The ratio.
		/// </value>
		decimal Ratio { get; }
	}
}