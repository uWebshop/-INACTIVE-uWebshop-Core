using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// 
	/// </summary>
	public class DiscountHelper
	{
		/// <summary>
		/// Returns a single product discount of a product
		/// </summary>
		/// <param name="itemId">The item unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns>
		/// Sale
		/// </returns>
		public static DiscountProduct GetProductDiscount(int itemId, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductDiscountService>().GetDiscountByProductId(itemId, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Returns a single pricing discount of a product
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns>
		/// Sale
		/// </returns>
		public static IEnumerable<API.IOrderDiscount> GetOrderDiscounts(Guid uniqueOrderId)
		{
			// todo: bugged, no ranges, is this used by external code?
			var orderinfo = OrderHelper.GetOrder(uniqueOrderId);

			var orderDiscounts = IO.Container.Resolve<IOrderDiscountService>().GetAll(orderinfo.Localization);

			if (orderDiscounts != null && orderDiscounts.Any())
			{
				var discountsForOrder = new List<IOrderDiscount>();

				foreach (var discount in orderDiscounts.Where(x => x.MinimumOrderAmount.WithVat.ValueInCents <= orderinfo.Grandtotal))
				{
					if (!discount.Disabled && (discount.CounterEnabled && discount.Counter > 0 || discount.CounterEnabled == false))
					{
						discountsForOrder.Add(discount);
					}
				}

				discountsForOrder.Add(orderDiscounts.First(x => x.DiscountType >= DiscountType.FreeShipping));

				return discountsForOrder.Select(d => new DiscountAdaptor(d));
			}

			return Enumerable.Empty<API.IOrderDiscount>();
		}

		/// <summary>
		/// Gets the discount value for order.
		/// </summary>
		/// <param name="discountOrder">The discount order.</param>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		public static int GetDiscountValueForOrder(Interfaces.IOrderDiscount discountOrder, OrderInfo orderInfo)
		{
			// todo: bugged, no ranges, no newprice, is this used by external code?

			if (discountOrder.DiscountType == DiscountType.Percentage)
			{
				// no ranges
				return PercentageCalculation(discountOrder.DiscountValue, orderInfo.GrandtotalInCents);
			}

			if (discountOrder.DiscountType == DiscountType.Amount)
			{
				return discountOrder.DiscountValue;
			}
			// no NewPrice & Shipping
			return 0;
		}

		internal static int PercentageCalculation(int percentageTimesHundred, long originalAmountInCents)
		{
			return (int)Math.Round(percentageTimesHundred * (long)originalAmountInCents / 10000m, MidpointRounding.AwayFromZero);
		}
	}
}