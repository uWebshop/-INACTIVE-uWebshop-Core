using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPayment
	{
		/// <summary>
		/// Gets the providers.
		/// </summary>
		/// <value>
		/// The providers.
		/// </value>
		IEnumerable<IChosenPaymentProvider> Providers { get; } 
	}
}