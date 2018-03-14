using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Common;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// Class representing the catalog in  webshop, containing a group of categories
	/// </summary>
	[ContentType(Name = "Catalog", Description = "#CatalogDescription", Alias = "uwbsCatalog", IconClass = IconClass.stack, Icon = ContentIcon.ClipboardList, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(CategoryRepositoryContentType), typeof(ProductRepositoryContentType) }, ParentContentType = typeof(UwebshopRootContentType))]
	[DataContract(Namespace = "")]
	public static class Catalog
	{
		/// <summary>
		/// The category repository node alias
		/// </summary>
		public static string CategoryRepositoryNodeAlias { get { return CategoryRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// The product repository node alias
		/// </summary>
		public static string ProductRepositoryNodeAlias { get { return ProductRepositoryContentType.NodeAlias; } }

		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		internal static IEnumerable<Category> GetAllRootCategories(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<ICategoryService>().GetAllRootCategories(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		internal static IEnumerable<uWebshopEntity> GetCategoryRepositoryNodes()
		{
			return DomainHelper.GetObjectsByAlias<uWebshopEntity>(CategoryRepositoryNodeAlias, Constants.NonMultiStoreAlias);
		}

		internal static uWebshopEntity GetCategoryRepositoryNode()
		{
			return GetCategoryRepositoryNodes().FirstOrDefault();
		}

		internal static uWebshopEntity GetProductRepositoryNode()
		{
			return DomainHelper.GetObjectsByAlias<uWebshopEntity>(ProductRepositoryNodeAlias, Constants.NonMultiStoreAlias).FirstOrDefault();
		}
	}
}