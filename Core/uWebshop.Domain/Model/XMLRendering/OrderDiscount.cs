using System;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.XMLRendering
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class OrderDiscount
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OrderDiscount"/> class.
		/// </summary>
		public OrderDiscount()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrderDiscount"/> class.
		/// </summary>
		/// <param name="orderDiscount">The order discount.</param>
		/// <param name="orderInfo">The order information.</param>
		internal OrderDiscount(IOrderDiscount orderDiscount, OrderInfo orderInfo)
		{
			var discountOrder = orderDiscount as DiscountOrder;
			if (discountOrder == null)
			{
				discountOrder = IO.Container.Resolve<IOrderDiscountService>().GetAll(orderInfo.Localization).FirstOrDefault(discount => discount.OriginalId == orderDiscount.OriginalId) as DiscountOrder;
			}

			if (discountOrder != null)
			{
				Title = discountOrder.Title;
				Description = discountOrder.Description;
			}
			//orderInfo.OrderLines.ForEach(line => line.OrderDiscountInCents = 0); // reset hmm
			DiscountAmount = new DiscountAmount(IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(orderDiscount, orderInfo), orderInfo.PricesAreIncludingVAT, orderInfo.AverageOrderVatPercentage);

			OriginalDiscountId = orderDiscount.OriginalId;
			DiscountType = orderDiscount.DiscountType;
			DiscountValue = orderDiscount.DiscountValue;
			Condition = orderDiscount.Condition;
			NumberOfItemsCondition = orderDiscount.NumberOfItemsCondition;
			MinimalOrderAmount = orderDiscount.MinimumOrderAmount.ValueInCents();
			CouponCode = orderDiscount.CouponCode;
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the discount amount.
		/// </summary>
		/// <value>
		/// The discount amount.
		/// </value>
		[DataMember]
		// todo: dit kan via een attribute die de naam in XML aanpast, ipv een extra class
		public DiscountAmount DiscountAmount { get; set; }

		/// <summary>
		/// Gets or sets the original discount unique identifier.
		/// </summary>
		/// <value>
		/// The original discount unique identifier.
		/// </value>
		[DataMember]
		public int OriginalDiscountId { get; set; }

		/// <summary>
		/// Gets or sets the type of the discount.
		/// </summary>
		/// <value>
		/// The type of the discount.
		/// </value>
		[DataMember]
		public DiscountType DiscountType { get; set; }

		/// <summary>
		/// Gets or sets the discount value.
		/// </summary>
		/// <value>
		/// The discount value.
		/// </value>
		[DataMember]
		public int DiscountValue { get; set; }

		/// <summary>
		/// Gets or sets the condition.
		/// </summary>
		/// <value>
		/// The condition.
		/// </value>
		[DataMember]
		public DiscountOrderCondition Condition { get; set; }

		/// <summary>
		/// Gets or sets the number of items condition.
		/// </summary>
		/// <value>
		/// The number of items condition.
		/// </value>
		[DataMember]
		public int NumberOfItemsCondition { get; set; }

		/// <summary>
		/// Gets or sets the minimal order amount.
		/// </summary>
		/// <value>
		/// The minimal order amount.
		/// </value>
		[DataMember]
		public int MinimalOrderAmount { get; set; }

		/// <summary>
		/// Gets or sets the coupon code.
		/// </summary>
		/// <value>
		/// The coupon code.
		/// </value>
		[DataMember]
		public string CouponCode { get; set; }
	}
}