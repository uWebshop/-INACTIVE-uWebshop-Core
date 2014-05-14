using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPaymentProviderService
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		IEnumerable<PaymentProvider> GetAll(ILocalization localization);

		/// <summary>
		/// Gets the name of the payment provider with.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		PaymentProvider GetPaymentProviderWithName(string name, ILocalization localization);

		/// <summary>
		/// Gets the by unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		PaymentProvider GetById(int id, ILocalization localization);

		/// <summary>
		/// Loads the data.
		/// </summary>
		/// <param name="paymentProvider">The payment provider.</param>
		/// <param name="localization">The localization.</param>
		void LoadData(PaymentProvider paymentProvider, ILocalization localization);
	}
}