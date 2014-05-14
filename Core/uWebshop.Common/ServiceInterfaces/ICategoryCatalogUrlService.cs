using System.Collections.Generic;

namespace uWebshop.Common.Interfaces
{
	internal interface ICategoryCatalogUrlService
	{
		string GetCanonicalUrl(ICategory category);
	    string GetUrlForPath(IEnumerable<ICategory> categoryPath);
	}
}