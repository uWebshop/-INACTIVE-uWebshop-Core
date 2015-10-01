using uWebshop.Domain.Helpers;
using Umbraco.Core.Macros;

namespace uWebshop.XSLTExtensions
{
    [XsltExtension("uWebshop.Navigation")]
	public class Navigation
	{
		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <returns></returns>
		public static string GetLocalizedUrl(int id)
		{
			return StoreHelper.GetNiceUrl(id, 0);
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <returns></returns>
		public static string GetLocalizedUrl(int id, int categoryId)
		{
			return StoreHelper.GetNiceUrl(id, categoryId);
		}
	}
}