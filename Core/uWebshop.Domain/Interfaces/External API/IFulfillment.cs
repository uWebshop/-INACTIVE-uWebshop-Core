using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFulfillment
	{
		/// <summary>
		/// Gets a value indicating whether [fulfilled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [fulfilled]; otherwise, <c>false</c>.
		/// </value>
		bool Fulfilled { get; }
		/// <summary>
		/// Gets the providers.
		/// </summary>
		/// <value>
		/// The providers.
		/// </value>
		IEnumerable<IChosenFulfillmentProvider> Providers {get;} 
	}
}