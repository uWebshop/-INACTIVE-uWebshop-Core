using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPaymentProvider
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		string GetName();

		/// <summary>
		/// Gets the parameter render method.
		/// </summary>
		/// <returns></returns>
		PaymentTransactionMethod GetParameterRenderMethod();

		/// <summary>
		/// Gets all payment methods.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		IEnumerable<PaymentProviderMethod> GetAllPaymentMethods(int id);
	}
}