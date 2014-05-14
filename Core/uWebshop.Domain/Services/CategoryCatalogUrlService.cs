using System.Collections.Generic;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class CategoryCatalogUrlService : ICategoryCatalogUrlService
	{
		public string GetCanonicalUrl(ICategory category)
		{
			var productCategoryUrl = category.UrlName;
			while (category.Parent != null)
			{
				category = category.Parent;
				productCategoryUrl = string.Format("{0}/{1}", category.UrlName, productCategoryUrl);
			}
			return productCategoryUrl;
		}

        public string GetUrlForPath(IEnumerable<ICategory> categoryPath)
        {
            return string.Join("/", categoryPath.Select(c => c.UrlName));
        }
	}
}