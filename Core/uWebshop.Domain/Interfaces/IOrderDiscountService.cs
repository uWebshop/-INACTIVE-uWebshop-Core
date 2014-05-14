using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	internal interface IOrderDiscountService : IEntityService<IOrderDiscount>
	{
		IEnumerable<IOrderDiscount> GetApplicableDiscountsForOrder(OrderInfo order, ILocalization localization);
	}
}