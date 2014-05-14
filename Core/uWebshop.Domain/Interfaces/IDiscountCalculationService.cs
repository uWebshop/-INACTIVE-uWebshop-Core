using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDiscountCalculationService
	{
		/// <summary>
		/// Discounts the amount for order.
		/// </summary>
		/// <param name="discount">The discount.</param>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="applyDiscountEffects">if set to <c>true</c> [apply discount effects].</param>
		/// <returns></returns>
		int DiscountAmountForOrder(IOrderDiscount discount, OrderInfo orderInfo, bool applyDiscountEffects = false);

		/// <summary>
		/// Gets the ranged discount value for order.
		/// </summary>
		/// <param name="orderDiscount">The order discount.</param>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		int RangedDiscountValueForOrder(IOrderDiscount orderDiscount, OrderInfo orderInfo);
	}
}