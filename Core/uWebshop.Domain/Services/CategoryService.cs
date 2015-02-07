using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class CategoryService : EntityService<Category>, ICategoryService
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ConcurrentDictionary<string, List<Category>> _rootCategoriesCache = new ConcurrentDictionary<string, List<Category>>();

		public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		// dit kan (of kon) een IndexReader closed (examine) exception geven, hopelijk fixt de eerste .ToList() dat
		public List<Category> GetAllRootCategories(ILocalization localization)
		{
			if (localization == null) throw new Exception("Trying to load multi-store content without store");
			return _rootCategoriesCache.GetOrAdd(GetCacheKey(localization), alias =>
				{
					var categoryRepositoryNodeIds = Catalog.GetCategoryRepositoryNodes().Select(n => n.Id).ToList();
					return GetAll(localization).Where(category => categoryRepositoryNodeIds.Contains(category.ParentId)).ToList();
				});
		}
		protected string GetCacheKey(ILocalization localization)
		{
			return localization.StoreAlias + localization.CurrencyCode;
		}

		public ICategory Localize(ICategory category, ILocalization localization)
		{
			if (category == null) return null;
			return GetById(category.Id, localization);
		}

		public new void FullResetCache()
		{
			_rootCategoriesCache.Clear();
		}
	}
}