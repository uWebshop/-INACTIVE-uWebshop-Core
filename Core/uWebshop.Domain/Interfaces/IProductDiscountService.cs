namespace uWebshop.Domain.Interfaces
{
	internal interface IProductDiscountService : IEntityService<DiscountProduct>
	{
		DiscountProduct GetDiscountByProductId(int productId, ILocalization localization, OrderInfo order = null);
		DiscountProduct GetDiscountByProductVariantId(int productvariantId, ILocalization localization, OrderInfo order = null);
		int GetAdjustedPriceForProductWithId(int productId, ILocalization localization, int currentProductPrice, int orderTotalItemCount = 0);
		int GetAdjustedPriceForProductVariantWithId(int productvariantId, ILocalization localization, int currentProductPrice, int orderTotalItemCount = 0);
	}
}