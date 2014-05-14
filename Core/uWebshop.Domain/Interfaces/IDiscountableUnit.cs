using uWebshop.Domain.Model;

namespace uWebshop.Domain.Interfaces
{
	interface IDiscountableUnit
	{
		DiscountEffects GetDiscountEffects();
	}
}