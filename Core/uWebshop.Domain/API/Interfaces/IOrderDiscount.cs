using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderDiscount : Domain.Interfaces.IOrderDiscountInternalExternalShared
	{
		/// <summary>
		/// Gets the coupon codes.
		/// </summary>
		/// <value>
		/// The coupon codes.
		/// </value>
		IEnumerable<string> CouponCodes { get; }
	}
}
