using System;
using System.Collections.Generic;
using System.Text;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICouponCodeService
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ICoupon> GetAll(string where= null);

		/// <summary>
		/// Gets all for discount.
		/// </summary>
		/// <param name="discountId">The discount unique identifier.</param>
		/// <returns></returns>
		IEnumerable<ICoupon> GetAllForDiscount(int discountId);

		/// <summary>
		/// Gets the specified discount unique identifier.
		/// </summary>
		/// <param name="discountId">The discount unique identifier.</param>
		/// <param name="couponCode">The coupon code.</param>
		/// <returns></returns>
		ICoupon Get(int discountId, string couponCode);

		/// <summary>
		/// Gets all with couponcode.
		/// </summary>
		/// <param name="couponCode">The coupon code.</param>
		/// <returns></returns>
		IEnumerable<ICoupon> GetAllWithCouponcode(string couponCode);

		/// <summary>
		/// Gets all with couponcodes.
		/// </summary>
		/// <param name="couponCodes">The coupon codes.</param>
		/// <returns></returns>
		IEnumerable<ICoupon> GetAllWithCouponcodes(IEnumerable<string> couponCodes);

		/// <summary>
		/// Saves the specified coupon.
		/// </summary>
		/// <param name="coupon">The coupon.</param>
		void Save(ICoupon coupon);

		void Save(int discountId, IEnumerable<ICoupon> coupons);

		void DecreaseCountByOneFor(IEnumerable<ICoupon> coupons);
	}
}
