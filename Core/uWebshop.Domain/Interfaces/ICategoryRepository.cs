namespace uWebshop.Domain.Interfaces
{
	internal interface ICategoryRepository : IEntityRepository<Category>, ICachedRepository
	{
	}
}