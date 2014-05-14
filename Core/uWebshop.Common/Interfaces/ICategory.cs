using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common.Interfaces.Shared;

namespace uWebshop.Common.Interfaces
{
	internal interface ICategory : ICategoryInternalExternalShared
	{
		ICategory Parent { get; }
		IEnumerable<IProduct> Products { get; }
		IEnumerable<ICategory> ParentCategories { get; }
		IEnumerable<ICategory> SubCategories { get; }
	}

	internal interface IProduct : IProductInternalExternalShared
	{
		IEnumerable<ICategory> Categories { get; }
	}

}
namespace uWebshop.Common.Interfaces.Shared
{
	public interface IProductInternalExternalShared : IUwebshopEntity
	{
		string UrlName { get; }
	}

	public interface ICategoryInternalExternalShared : IUwebshopEntity
	{
		/// <summary>
		/// Gets the name of the URL.
		/// </summary>
		/// <value>
		/// The name of the URL.
		/// </value>
		string UrlName { get; }

		/// <summary>
		/// Gets the parent node type alias.
		/// </summary>
		/// <value>
		/// The parent node type alias.
		/// </value>
		string ParentNodeTypeAlias { get; }
	}
}
