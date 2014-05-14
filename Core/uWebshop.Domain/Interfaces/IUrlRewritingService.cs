using uWebshop.Common.Interfaces;
using uWebshop.Domain.BaseClasses;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	interface IUrlRewritingService
	{
		/// <summary>
		/// Rewrites this instance.
		/// </summary>
		void Rewrite();

		/// <summary>
		/// Redirects the permanent old catalog urls.
		/// </summary>
		void RedirectPermanentOldCatalogUrls();

		/// <summary>
		/// URLs the points automatic catalog repository.
		/// </summary>
		/// <param name="absolutePath">The absolute path.</param>
		/// <param name="catalogPart">The catalog part.</param>
		/// <returns></returns>
		bool UrlPointsToCatalogRepository(string absolutePath, out string catalogPart);

		/// <summary>
		/// Resolves the uwebshop entity URL.
		/// </summary>
		/// <returns></returns>
		ResolveUwebshopEntityUrlResult ResolveUwebshopEntityUrl();

		/// <summary>
		/// Resolves the uwebshop entity URL.
		/// </summary>
		/// <param name="absolutePath">The absolute path.</param>
		/// <returns></returns>
		ResolveUwebshopEntityUrlResult ResolveUwebshopEntityUrl(string absolutePath);
	}

	/// <summary>
	/// 
	/// </summary>
	public struct ResolveUwebshopEntityUrlResult
	{
		/// <summary>
		/// The category URL
		/// </summary>
		public string CategoryUrl;

		/// <summary>
		/// The entity
		/// </summary>
		public IUwebshopEntity Entity;

		/// <summary>
		/// The product URL
		/// </summary>
		public string ProductUrl;

		/// <summary>
		/// The store URL
		/// </summary>
		public string StoreUrl;
	}
}