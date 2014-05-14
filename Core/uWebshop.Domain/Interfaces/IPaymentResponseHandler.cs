namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPaymentResponseHandler
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		string GetName();

		/// <summary>
		/// Handles the payment response.
		/// </summary>
		/// <returns></returns>
		OrderInfo HandlePaymentResponse(PaymentProvider paymentProvider, OrderInfo order);
	}
}