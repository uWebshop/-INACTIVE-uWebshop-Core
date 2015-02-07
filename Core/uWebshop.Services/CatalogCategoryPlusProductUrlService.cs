using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Core;

namespace uWebshop.Domain.Services
{
	internal class Module : SimpleAddon
	{
		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 15;
		}
		public override string Name()
		{
			return "";
		}
		public override void DependencyRegistration(IRegistrationControl control)
		{
			control.RegisterType<IProductUrlService, CatalogCategoryPlusProductUrlService>();
		}
	}
	internal class CatalogCategoryPlusProductUrlService : IProductUrlService
	{
		private readonly ICategoryCatalogUrlService _categoryCatalogUrlService;
		private readonly ILoggingService _log;

		public CatalogCategoryPlusProductUrlService(ICategoryCatalogUrlService categoryCatalogUrlService, ILoggingService log)
		{
			_categoryCatalogUrlService = categoryCatalogUrlService;
			_log = log;
		}

		public string GetCanonicalUrl(IProduct product) 
		{
			return GetUrlInCategoryOrCanonical(product, null);
		}

		public string GetUrlInCategoryOrCanonical(IProduct product, ICategory category)
		{
			if (product == null) throw new ArgumentNullException("product");
			if (product.Categories == null) throw new Exception("Product with null Categories");
			if (_categoryCatalogUrlService == null) throw new Exception("Some serious configuration error occured");

			if (category == null || !product.Categories.Any(c => c.Id == category.Id))
			{
				category = product.Categories.FirstOrDefault();
			}

			// todo: products can get their own url using storeUrl/productUrlName, but resolving and name conflicts need to be fixed
			if (category == null)
			{
				return product.UrlName;
			}

			return _categoryCatalogUrlService.GetCanonicalUrl(category) + "/" + product.UrlName;
		}

		public string GetUrlUsingCategoryPathOrCanonical(IProduct product, IEnumerable<ICategory> categoryPath)
		{
			var category = categoryPath.LastOrDefault();
			if (category != null && product.Categories.Any(c => c.Id == category.Id))
			{
				return _categoryCatalogUrlService.GetUrlForPath(categoryPath) + "/" + product.UrlName;
			}
			
			category = product.Categories.FirstOrDefault();

			// todo: products can get their own url using storeUrl/productUrlName, but resolving and name conflicts need to be fixed
			if (category == null)
			{
				return product.UrlName;
			}

			return _categoryCatalogUrlService.GetCanonicalUrl(category) + "/" + product.UrlName;
		}
	}
}