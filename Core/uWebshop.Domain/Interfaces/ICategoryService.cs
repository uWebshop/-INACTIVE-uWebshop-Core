using System.Collections.Generic;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface ICategoryService : IEntityService<Category>
	{
		/// <summary>
		/// Gets all root categories.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		IEnumerable<Category> GetAllRootCategories(ILocalization localization);

		ICategory Localize(ICategory category, ILocalization localization);
	}
}