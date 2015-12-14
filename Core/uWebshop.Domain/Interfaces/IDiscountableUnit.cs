using uWebshop.Domain.Model;
using uWebshop.Domain.Model.OrderInfo;

namespace uWebshop.Domain.Interfaces
{
	interface IDiscountableUnit
	{
		DiscountEffects GetDiscountEffects();
	}
}