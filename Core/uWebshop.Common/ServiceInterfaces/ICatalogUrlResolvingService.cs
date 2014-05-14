using System.Collections.Generic;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface ICatalogUrlResolvingService
	{
		/// <summary>
		/// Return an Product based on the urlname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <param name="productUrlName">Name of the product URL.</param>
		/// <returns></returns>
		uWebshop.Common.Interfaces.IProduct GetProductFromUrlName(string categoryUrlName, string productUrlName);
		
		/// <summary>
		/// Gets the category path from the url.
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <returns></returns>
		IEnumerable<ICategory> GetCategoryPathFromUrlName(string categoryUrlName);

		/// <summary>
		/// Returns the Category based on the URLname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <returns></returns>
		ICategory GetCategoryFromUrlName(string categoryUrlName);
	}
}