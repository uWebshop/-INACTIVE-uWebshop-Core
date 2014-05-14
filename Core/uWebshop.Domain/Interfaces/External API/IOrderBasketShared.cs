using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderBasketShared : IOrderBasketWishlistShared
	{
		// essential stuff
		/// <summary>
		/// Gets a value indicating whether [is vat charged].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is vat charged]; otherwise, <c>false</c>.
		/// </value>
		bool IsVatCharged { get; }
		/// <summary>
		/// Gets the regional vat.
		/// </summary>
		/// <value>
		/// The regional vat.
		/// </value>
		IPrice RegionalVat { get; }

		/// <summary>
		/// Gets the discounts.
		/// </summary>
		/// <value>
		/// The discounts.
		/// </value>
		IEnumerable<IAppliedOrderDiscount> Discounts { get; }
		/// <summary>
		/// Gets the used coupon codes.
		/// </summary>
		/// <value>
		/// The used coupon codes.
		/// </value>
		IEnumerable<string> UsedCouponCodes { get; }
		/// <summary>
		/// Gets the fulfillment.
		/// </summary>
		/// <value>
		/// The fulfillment.
		/// </value>
		IFulfillment Fulfillment { get; }
		/// <summary>
		/// Gets the payment.
		/// </summary>
		/// <value>
		/// The payment.
		/// </value>
		IPayment Payment { get; }

		// helpers
		/// <summary>
		/// Gets the charged order amount.
		/// </summary>
		/// <value>
		/// The charged order amount.
		/// </value>
		IPrice ChargedOrderAmount { get; }
		/// <summary>
		/// Gets the charged shipping amount.
		/// </summary>
		/// <value>
		/// The charged shipping amount.
		/// </value>
		IPrice ChargedShippingAmount { get; }
		/// <summary>
		/// Gets the charged payment amount.
		/// </summary>
		/// <value>
		/// The charged payment amount.
		/// </value>
		IPrice ChargedPaymentAmount { get; }
		/// <summary>
		/// Gets the payment provider amount.
		/// </summary>
		/// <value>
		/// The payment provider amount.
		/// </value>
		IVatPrice PaymentProviderAmount { get; }
		/// <summary>
		/// Gets the shipping provider amount.
		/// </summary>
		/// <value>
		/// The shipping provider amount.
		/// </value>
		IDiscountedPrice ShippingProviderAmount { get; }

		/// <summary>
		/// Gets the order amount.
		/// </summary>
		/// <value>
		/// The order amount.
		/// </value>
		new IDiscountedPrice OrderAmount { get; }

		/// <summary>
		/// Gets the order line total.
		/// </summary>
		/// <value>
		/// The order line total.
		/// </value>
		new IDiscountedPrice OrderLineTotal { get; }
	}
}