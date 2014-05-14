using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IShippingProviderService
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		IEnumerable<ShippingProvider> GetAll(ILocalization localization);

		/// <summary>
		/// Gets the name of the payment provider with.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		ShippingProvider GetPaymentProviderWithName(string name, ILocalization localization);

		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		ShippingProvider GetById(int id, ILocalization localization);

		///// <summary>
		///// Loads the data.
		///// </summary>
		///// <param name="shippingProvider">The shipping provider.</param>
		///// <param name="localization">The localization.</param>
		//void LoadData(ShippingProvider shippingProvider, ILocalization localization);
	}
}