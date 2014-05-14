using System;
using System.ComponentModel;
using System.Linq;
using uWebshop.Domain.Helpers;

namespace uWebshop.RazorExtensions
{
	public static class Navigation
	{
		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <returns></returns>
		public static string NiceUrl(int id)
		{
			return StoreHelper.GetNiceUrl(id);
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId)
		{
			return StoreHelper.GetNiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id 
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeId">the id of the store you want to link to</param>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId, int storeId)
		{
			var store = StoreHelper.GetById(storeId);
			return store != null ? StoreHelper.GetNiceUrl(id, categoryId, store.Alias) : StoreHelper.GetNiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id 
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeAlias">the id of the store you want to link to</param>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId, string storeAlias)
		{
			return StoreHelper.GetNiceUrl(id, categoryId, storeAlias);
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use NiceUrl()")]
		public static string GetLocalizedUrl(int id)
		{
			//var storeAlias = StoreHelper.GetCurrentStore();

			return NiceUrl(id);
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use NiceUrl()")]
		public static string GetLocalizedUrl(int id, int categoryId)
		{
			//var storeAlias = StoreHelper.GetCurrentStore();

			return NiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id 
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeId">the id of the store you want to link to</param>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use NiceUrl()")]
		public static string GetLocalizedUrl(int id, int categoryId, int storeId)
		{
			return NiceUrl(id, categoryId, storeId);
		}
	}
}