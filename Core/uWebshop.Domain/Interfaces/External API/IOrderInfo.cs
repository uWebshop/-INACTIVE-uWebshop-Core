using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderInfo
	{
		/// <summary>
		/// Gets or sets a value indicating whether the prices are including vat.
		/// </summary>
		/// <value>
		/// <c>true</c> if the prices are including vat; otherwise, <c>false</c>.
		/// </value>
		bool PricesAreIncludingVAT { get; set; }

		/// <summary>
		/// Gets or sets the order lines.
		/// </summary>
		/// <value>
		/// The order lines.
		/// </value>
		List<OrderLine> OrderLines { get; set; }

		/// <summary>
		/// Gets the store information.
		/// </summary>
		/// <value>
		/// The store information.
		/// </value>
		StoreInfo StoreInfo { get; }

		/// <summary>
		/// Gets the localization.
		/// </summary>
		/// <value>
		/// The localization.
		/// </value>
		ILocalization Localization { get; }

		/// <summary>
		/// Gets the vat calculation strategy.
		/// </summary>
		/// <value>
		/// The vat calculation strategy.
		/// </value>
		IVatCalculationStrategy VatCalculationStrategy { get; }
	}
}