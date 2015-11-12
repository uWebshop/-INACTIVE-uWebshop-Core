using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess;
using umbraco.DataLayer;

namespace uWebshop.Domain.Model
{
	/// <summary>
	/// 
	/// </summary>
	internal class Coupon : ICoupon
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Coupon"/> class.
		/// </summary>
		/// <param name="discountId">The discount unique identifier.</param>
		/// <param name="couponCode">The coupon code.</param>
		/// <param name="numberAvailable">The number available.</param>
		public Coupon(int discountId, string couponCode, int numberAvailable)
		{
			DiscountId = discountId;
			CouponCode = couponCode;
			NumberAvailable = numberAvailable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Coupon"/> class.
		/// </summary>
		/// <param name="discountId">The discount unique identifier.</param>
		/// <param name="displayString">The display string.</param>
		internal Coupon(int discountId, string displayString)
		{
			int numberAvailable = 1;
			int.TryParse(displayString.Split('|')[1], out numberAvailable);

			DiscountId = discountId;
			CouponCode = displayString.Split('|')[0];
			NumberAvailable = numberAvailable;
		}

		/// <summary>
		/// Gets or sets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int DiscountId { get; set; }

		/// <summary>
		/// Gets or sets the coupon code.
		/// </summary>
		/// <value>
		/// The coupon code.
		/// </value>
		public string CouponCode { get; set; }

		/// <summary>
		/// Gets or sets the number available.
		/// </summary>
		/// <value>
		/// The number available.
		/// </value>
		public int NumberAvailable { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}|{1}", CouponCode, NumberAvailable);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Coupon) obj);
		}

		/// <summary>
		/// Equalses the specified other.
		/// </summary>
		/// <param name="other">The other.</param>
		/// <returns></returns>
		protected bool Equals(Coupon other)
		{
			return CouponCode == other.CouponCode && NumberAvailable == other.NumberAvailable;
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = CouponCode.GetHashCode();
				hashCode = (hashCode*397) ^ DiscountId;
				return hashCode;
			}
		}
	}
}