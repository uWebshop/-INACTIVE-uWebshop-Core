namespace uWebshop.Domain.Interfaces
{
	internal interface IProductVariantRepository : IEntityRepository<ProductVariant>, ICachedRepository
	{
	}
}