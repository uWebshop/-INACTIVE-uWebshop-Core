namespace uWebshop.Domain.Interfaces
{
	internal interface IProductDiscountService : IEntityService<DiscountProduct>
	{
		DiscountProduct GetDiscountByProductId(int productId, ILocalization localization);
		int GetAdjustedPriceForProductWithId(int productId, ILocalization localization, int currentProductPrice, int orderTotalItemCount = 0);
	}
}