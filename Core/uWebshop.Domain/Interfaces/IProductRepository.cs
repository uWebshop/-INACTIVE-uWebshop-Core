using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	internal interface IProductRepository : IEntityRepository<Product>, ICachedRepository
	{
	}
	internal interface ICachedRepository
	{
		ICacheRebuilder GetCacheRebuilder();
	}
	internal interface ICacheRebuilder
	{
		void Lock();
		void Rebuild();
		void SwitchCache();
		void Unlock();
	}
}