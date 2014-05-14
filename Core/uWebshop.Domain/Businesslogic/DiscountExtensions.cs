using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	internal static class DiscountExtensions
	{
		internal static IEnumerable<Interfaces.IOrderDiscount> HasDiscountForOrder(this IEnumerable<Interfaces.IOrderDiscount> discounts, OrderInfo orderInfo)
		{
			return discounts.Where(discount => discount.DiscountType == DiscountType.FreeShipping || IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(discount, orderInfo) > 0);
		}
	}
}