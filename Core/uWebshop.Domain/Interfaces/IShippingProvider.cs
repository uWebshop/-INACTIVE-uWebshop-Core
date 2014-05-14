using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IShippingProvider
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		string GetName();

		/// <summary>
		/// Gets all shipping methods.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		IEnumerable<ShippingProviderMethod> GetAllShippingMethods(int id);
	}
}