using System.Collections.Generic;
using uWebshop.Common;

namespace uWebshop.Domain.Interfaces
{
	public interface IOrderDiscount : IOrderDiscountInternalExternalShared
	{
		/// <summary>
		/// Code of the coupon
		/// </summary>
		/// <value>
		/// The coupon code.
		/// </value>
		string CouponCode { get; }

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		int OriginalId { get; }

		/// <summary>
		/// Gets the counter.
		/// </summary>
		/// <value>
		/// The counter.
		/// </value>
		int Counter { get; }
	}

	public interface IOrderDiscountAppliedShared : IDiscount
	{
		/// <summary>
		/// List of products this discount applies to
		/// </summary>
		/// <value>
		/// The required item ids.
		/// </value>
		List<int> RequiredItemIds { get; }

		/// <summary>
		/// Discount condition (None, OnTheXthItem, PerSetOfItems)
		/// </summary>
		/// <value>
		/// The condition.
		/// </value>
		DiscountOrderCondition Condition { get; }

		/// <summary>
		/// Number of items required for OnTheXthItem and PerSetOfItems conditions
		/// </summary>
		/// <value>
		/// The number of items condition.
		/// </value>
		int NumberOfItemsCondition { get; }

		/// <summary>
		/// The discount value in cents or percentage
		/// </summary>
		/// <value>
		/// The discount value.
		/// </value>
		int DiscountValue { get; }

		/// <summary>
		/// Information on the ranges for this discount
		/// </summary>
		/// <value>
		/// The ranges string.
		/// </value>
		//string RangesString { get; }
		List<Range> Ranges { get; }

		/// <summary>
		/// Type of discount (Percentage, Amount, Free shipping)
		/// </summary>
		/// <value>
		/// The type of the discount.
		/// </value>
		DiscountType DiscountType { get; }

		/// <summary>
		/// Membergroups this discount is valid for
		/// </summary>
		/// <value>
		/// The member groups.
		/// </value>
		List<string> MemberGroups { get; }

		/// <summary>
		/// Gets the affected orderlines.
		/// </summary>
		/// <value>
		/// The affected orderlines.
		/// </value>
		List<int> AffectedOrderlines { get; }

		/// <summary>
		/// Gets the affected product tags.
		/// </summary>
		/// <value>
		/// The affected product tags.
		/// </value>
		IEnumerable<string> AffectedProductTags { get; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets the minimum order amount.
		/// </summary>
		/// <value>
		/// The minimum order amount.
		/// </value>
		IVatPrice MinimumOrderAmount { get; }

		/// <summary>
		/// Gets a value indicating whether [include shipping information order discountable amount].
		/// </summary>
		/// <value>
		/// <c>true</c> if [include shipping information order discountable amount]; otherwise, <c>false</c>.
		/// </value>
		bool IncludeShippingInOrderDiscountableAmount { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IOrderDiscountInternalExternalShared : IOrderDiscountAppliedShared, IDiscount, IUwebshopSortableEntity
	{

	}
}