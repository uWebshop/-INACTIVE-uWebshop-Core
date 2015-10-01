using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal abstract class PlainEntityRepository<T> where T : uWebshopEntity
	{
		protected UmbracoMultiStoreEntityRepository<T, T> _oldRepo;
		private readonly Dictionary<ILocalization, ICache<T>> _caches = new Dictionary<ILocalization, ICache<T>>();		//private IMultiLocaProductCache _cache;
		private ICache<T> GetCache(ILocalization localization)
		{
			ICache<T> cache;
			if (!_caches.TryGetValue(localization, out cache))
			{
				cache = new SafeCache<T>(() => _oldRepo.GetAll(localization));
				_caches.Add(localization, cache);
			}
			return cache;
		}
		public ICacheRebuilder GetCacheRebuilder()
		{
			return new MultiCastRebuilder(_caches.Select(c => c.Value.GetRebuilder()));
		}

		public T GetById(int id, ILocalization localization)
		{
			return GetCache(localization).GetOrAdd(id, i => _oldRepo.GetById(id, localization));
		}

		public List<T> GetAll(ILocalization localization)
		{
			return GetCache(localization).GetAll().ToList();
		}

		public void ReloadData(T entity, ILocalization localization)
		{
			_oldRepo.ReloadData(entity, localization);
		}
	}
	// todo: merge the functionality of these and the old repo's
	internal class PlainCategoryRepository : PlainEntityRepository<Category>, ICategoryRepository
	{
		public PlainCategoryRepository(ICategoryAliassesService aliasses, ICMSContentService cmsContentService)
		{
			_oldRepo = new UmbracoCategoryRepository(aliasses, cmsContentService);
		}
	}
	internal class PlainVariantRepository : PlainEntityRepository<ProductVariant>, IProductVariantRepository
	{
		public PlainVariantRepository(IStoreService storeService, IProductVariantAliassesService aliasses)
		{
			_oldRepo = new UmbracoProductVariantRepository(storeService, aliasses);
		}
	}
	internal class PlainProductRepository : PlainEntityRepository<Product>, IProductRepository
	{
		public PlainProductRepository(ISettingsService settingsService, IStoreService storeService, IProductVariantService variantService, IProductVariantGroupService variantGroupService, IProductAliassesService productAliassesService, ICMSContentService cmsContentService)
		{
			_oldRepo = new UmbracoProductRepository(settingsService, storeService, variantService, variantGroupService, productAliassesService, cmsContentService);
		}
	}

	internal class PlainVariantGroupRepository : PlainEntityRepository<ProductVariantGroup>, IProductVariantGroupRepository
	{
		public PlainVariantGroupRepository(IStoreService storeService, IProductVariantGroupAliassesService aliasses)
		{
			_oldRepo = new UmbracoProductVariantGroupRepository(storeService, aliasses);
		}
	}
}