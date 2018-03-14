using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal interface IUrlService
	{
		string CategoryCanonicalUrl(ICategory category, ILocalization localization);
		string CategoryUrlUsingCurrentPath(ICategory category, ILocalization localization);
		string ProductCanonical(IProduct product, ILocalization localization);
		string ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(IProduct product, ILocalization localization);
	}

	internal class UrlService : IUrlService
	{
		// one service to rule them all (and in the darkness bind them)
		private readonly IUrlFormatService _urlFormatService;
		private readonly IUrlLocalizationService _urlLocalizationService;
		private readonly ICategoryCatalogUrlService _categoryCatalogUrlService;
		private readonly IProductUrlService _productUrlService;
		private readonly IUwebshopRequestService _requestService;
		private readonly ICategoryService _categoryService;
		private readonly IProductService _productService;

		public UrlService(IUrlFormatService urlFormatService, IUrlLocalizationService urlLocalizationService, ICategoryCatalogUrlService categoryCatalogUrlService, IProductUrlService productUrlService, IUwebshopRequestService requestService, ICategoryService categoryService, IProductService productService)
		{
			_urlFormatService = urlFormatService;
			_urlLocalizationService = urlLocalizationService;
			_categoryCatalogUrlService = categoryCatalogUrlService;
			_productUrlService = productUrlService;
			_requestService = requestService;
			_categoryService = categoryService;
			_productService = productService;
		}

		public string CategoryCanonicalUrl(ICategory category, ILocalization localization)
		{
			category = _categoryService.Localize(category, localization);
			return _categoryCatalogUrlService.GetCanonicalUrl(category);
		}

		public string CategoryUrlUsingCurrentPath(ICategory category, ILocalization localization)
		{
			var path = GetLocalizedCurrentPath(localization);

            category = _categoryService.Localize(category, localization);

			if (path != null && !category.ParentCategories.Contains(path.LastOrDefault()))
			{
				path = null;
			}

			string url;
			if (path != null && path.Any())
			{
				path = path.Concat(new[] {category});
                url = _categoryCatalogUrlService.GetUrlForPath(path);
            }
			else
			{
				url = _categoryCatalogUrlService.GetCanonicalUrl(category);
            }

            return _urlFormatService.FormatUrl(_urlLocalizationService.LocalizeCatalogUrl(url, localization));
		}

		private IEnumerable<ICategory> GetLocalizedCurrentPath(ILocalization localization)
		{

			var path = _requestService.Current.CategoryPath ?? Enumerable.Empty<ICategory>();

            if (!localization.Equals(_requestService.Current.Localization))
			{
				path = path.Select(c => _categoryService.Localize(c, localization));
				if (path.Any(c => c == null))
				{
					return null;
				}
			}
			return path;
		}

		public string ProductCanonical(IProduct product, ILocalization localization)
		{
			product = _productService.Localize(product, localization);

			var productUrl = _productUrlService.GetCanonicalUrl(product);

            return _urlFormatService.FormatUrl(_urlLocalizationService.LocalizeCatalogUrl(productUrl, localization));
		}

		public string ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(IProduct product, ILocalization localization)
		{
			var path = (GetLocalizedCurrentPath(localization) ?? Enumerable.Empty<ICategory>()).ToArray();

			product = _productService.Localize(product, localization);
			
			var currentCategoryId = path.Select(c => c.Id).LastOrDefault();
			string productUrl;
			if (path.Any() && product.Categories.Any(c => c.Id == currentCategoryId))
			{
                productUrl = _productUrlService.GetUrlUsingCategoryPathOrCanonical(product, path);
            }
			else
			{
				var category = _categoryService.Localize(_requestService.Current.Category, localization);
				productUrl = _productUrlService.GetUrlInCategoryOrCanonical(product, category);
            }

            return _urlFormatService.FormatUrl(_urlLocalizationService.LocalizeCatalogUrl(productUrl, localization));
		}
	}
}
