namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAppliedOrderDiscount : IOrderDiscountAppliedShared
	{
		/// <summary>
		/// Gets the amount for order.
		/// </summary>
		/// <value>
		/// The amount for order.
		/// </value>
		IVatPrice AmountForOrder { get; }

		/// <summary>
		/// The used coupon code.
		/// </summary>
		/// <value>
		/// The used coupon code.
		/// </value>
		string CouponCode { get; }

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		int OriginalId { get; }
	}


}