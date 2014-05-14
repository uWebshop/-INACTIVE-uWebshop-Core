using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IValidationResults
	{
		/// <summary>
		/// Gets all ValidationResults.
		/// </summary>
		/// <value>
		/// All ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> All { get; }

		/// <summary>
		/// Gets the order ValidationResults.
		/// </summary>
		/// <value>
		/// The order ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Order { get; }
		/// <summary>
		/// Gets the stock ValidationResults.
		/// </summary>
		/// <value>
		/// The stockValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Stock { get; }
		/// <summary>
		/// Gets the order line ValidationResults.
		/// </summary>
		/// <value>
		/// The order line ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> OrderLine { get; }
		/// <summary>
		/// Gets the custom ValidationResults.
		/// </summary>
		/// <value>
		/// The custom ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Custom { get; }
		/// <summary>
		/// Gets the customer ValidationResults.
		/// </summary>
		/// <value>
		/// The customer ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Customer { get; }
		/// <summary>
		/// Gets the payment ValidationResults.
		/// </summary>
		/// <value>
		/// The payment ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Payment { get; }
		/// <summary>
		/// Gets the shipping ValidationResults.
		/// </summary>
		/// <value>
		/// The shipping ValidationResults.
		/// </value>
		IEnumerable<IValidationResult> Shipping { get; }
	}
}