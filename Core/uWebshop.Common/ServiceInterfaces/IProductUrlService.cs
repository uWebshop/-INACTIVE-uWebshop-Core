using System.Collections.Generic;

namespace uWebshop.Common.Interfaces
{
	internal interface IProductUrlService
	{
		string GetCanonicalUrl(IProduct product);
		string GetUrlInCategoryOrCanonical(IProduct product, ICategory category);
		string GetUrlUsingCategoryPathOrCanonical(IProduct product, IEnumerable<ICategory> categoryPath);
	}
}