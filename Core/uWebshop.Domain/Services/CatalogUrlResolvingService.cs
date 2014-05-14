using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class CatalogUrlResolvingService : ICatalogUrlResolvingService
	{
		private readonly ICategoryService _categoryService;
		private readonly IProductService _productService;
		private readonly IStoreService _storeService;

		public CatalogUrlResolvingService(ICategoryService categoryService, IProductService productService, IStoreService storeService)
		{
			_categoryService = categoryService;
			_productService = productService;
			_storeService = storeService;
		}

		public IProduct GetProductFromUrlName(string categoryUrlName, string productUrlName)
		{
			if (string.IsNullOrEmpty(productUrlName))
			{
				return null;
			}

			var categoryNode = GetCategoryFromUrlName(categoryUrlName);

			if (productUrlName.EndsWith("/"))
			{
				productUrlName = productUrlName.Remove(productUrlName.Length - 1);
			}

			if (categoryNode != null && categoryNode.Products != null)
			{
				// throw new Exception("asf " + categoryUrlName + " " + productUrlName + " " + (categoryNode == null) + " " + categoryNode.Products.Where(p => p.LocalizedUrl.ToLowerInvariant() == "toy-boat").Select(p => p.Id).FirstOrDefault());
				return
					categoryNode.Products.FirstOrDefault(
						product => product != null && product.UrlName.ToLowerInvariant() == productUrlName.ToLowerInvariant());
			}
			return _productService.GetAll(_storeService.GetCurrentLocalization())
					.FirstOrDefault(x => x.UrlName.ToLowerInvariant() == productUrlName.ToLowerInvariant());
		}

		public IEnumerable<ICategory> GetCategoryPathFromUrlName(string categoryUrlName)
		{
			if (string.IsNullOrEmpty(categoryUrlName) || categoryUrlName == "/")
				return Enumerable.Empty<ICategory>();

			categoryUrlName = categoryUrlName.TrimEnd('/');

			var catalogUrls = categoryUrlName.ToLower().Split('/').ToList();
			catalogUrls.Reverse();

			var categories = _categoryService.GetAll(_storeService.GetCurrentLocalization());
			var matchingcategories = categories.Where(x => x.UrlName.ToLower() == catalogUrls.FirstOrDefault()); //.ToList();
			var possibilities = matchingcategories.Select(x => new CategoryTreeWalker(x)); //.ToList();

			//Nike/Shoes/Running
			//Running/Shoes/Nike
			//Log.Instance.LogDebug( "foreach (var url in catalogUrls.Skip(1)) start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
			foreach (var url in catalogUrls.Skip(1)) // 0ms want leeg
			{
				var urlVariable = url;

				var temp = possibilities.SelectMany(currentLevelCategory => currentLevelCategory.GetParentCategories()).ToList();
				var temp2 = temp.Where(c => c.CurrentCategory.UrlName.ToLowerInvariant() == urlVariable).ToList();

				possibilities = possibilities //.Where(x =>// Category.IsAlias(x.ParentNodeTypeAlias) && x.ParentNodeTypeAlias != Catalog.CategoryRepositoryNodeAlias && 
					//x.ParentCategory.UrlName.ToLower() == urlVariable
					//&& !x.ParentCategory.Disabled
					.SelectMany(currentLevelCategory => currentLevelCategory.GetParentCategories()).Where(c => c.CurrentCategory.UrlName.ToLowerInvariant() == urlVariable);
				// TODO!!!!!!!! test
			}

			var categoryTreeWalkers = possibilities;

			if (categoryTreeWalkers.Skip(1).Any())
			{
				categoryTreeWalkers = categoryTreeWalkers.Where(x => x.ParentNodeTypeAlias.ToLowerInvariant() == Catalog.CategoryRepositoryNodeAlias.ToLowerInvariant());
			}

			var chosenOne = categoryTreeWalkers.FirstOrDefault();
			var pointer = chosenOne;
			var path = new List<ICategory>();

			while (pointer != null)
			{
				path.Add(pointer.CurrentCategory);
				pointer = pointer.Parent;
			}

			return path;
		}

		public ICategory GetCategoryFromUrlName(string categoryUrlName)
		{
			return GetCategoryPathFromUrlName(categoryUrlName).LastOrDefault();
		}

		private class CategoryTreeWalker
		{
			public ICategory Category;
			public ICategory ParentCategory;
			public string ParentNodeTypeAlias;
			public ICategory CurrentCategory;
			public CategoryTreeWalker Parent;

			private CategoryTreeWalker()
			{
			}

			public CategoryTreeWalker(ICategory category)
			{
				Category = category;
				CurrentCategory = category;
				ParentCategory = category.Parent;
				ParentNodeTypeAlias = category.ParentNodeTypeAlias;
			}

			public IEnumerable<CategoryTreeWalker> GetParentCategories()
			{
				var list = Category.ParentCategories.Select(c => new CategoryTreeWalker {Category = Category, CurrentCategory = c, ParentCategory = c.Parent, 
					ParentNodeTypeAlias = c.ParentNodeTypeAlias, Parent = this}).ToList();
				if (ParentCategory != null)
				{
					list.Add(new CategoryTreeWalker { Category = Category, CurrentCategory = ParentCategory, ParentCategory = ParentCategory.Parent, 
						ParentNodeTypeAlias = ParentCategory.ParentNodeTypeAlias, Parent = this });
				}
				return list;
			}
		}
	}
}