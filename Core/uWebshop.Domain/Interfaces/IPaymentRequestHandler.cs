namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IPaymentRequestHandler
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <returns></returns>
		string GetName();

		/// <summary>
		/// Creates the payment request.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		PaymentRequest CreatePaymentRequest(OrderInfo orderInfo);

		// todo: incorrect name
		/// <summary>
		/// Gets the payment URL.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		string GetPaymentUrl(OrderInfo orderInfo);
	}
}