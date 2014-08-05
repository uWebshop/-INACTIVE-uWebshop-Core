namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUwebshopConfiguration
	{
		/// <summary>
		/// Gets a value indicating whether permanent redirect old catalog urls.
		/// </summary>
		/// <value>
		/// <c>true</c> if permanent redirect old catalog urls; otherwise, <c>false</c>.
		/// </value>
		bool PermanentRedirectOldCatalogUrls { get; }

		/// <summary>
		/// Gets the legacy category URL identifier.
		/// </summary>
		/// <value>
		/// The legacy category URL identifier.
		/// </value>
		string LegacyCategoryUrlIdentifier { get; }

		/// <summary>
		/// Gets the legacy product URL identifier.
		/// </summary>
		/// <value>
		/// The legacy product URL identifier.
		/// </value>
		string LegacyProductUrlIdentifier { get; }

		/// <summary>
		/// Gets the category URL.
		/// </summary>
		/// <value>
		/// The category URL.
		/// </value>
		string CategoryUrl { get; }

		/// <summary>
		/// Gets the product URL.
		/// </summary>
		/// <value>
		/// The product URL.
		/// </value>
		string ProductUrl { get; }

		/// <summary>
		/// Gets the examine searcher.
		/// </summary>
		/// <value>
		/// The examine searcher.
		/// </value>
		string ExamineSearcher { get; }

		/// <summary>
		/// Gets the examine indexer.
		/// </summary>
		/// <value>
		/// The examine indexer.
		/// </value>
		string ExamineIndexer { get; }

		/// <summary>
		/// Gets the setting indicating whether the basket is shared between all stores.
		/// </summary>
		/// <value>
		/// The setting indicating whether the basket is shared between all stores.
		/// </value>
		bool ShareBasketBetweenStores { get; }

        /// <summary>
        /// Gets the setting indicating whether datefolders should be created when opening orders in the backend
        /// </summary>
        /// <value>
        /// The setting indicating whether the basket is shared between all stores.
        /// </value>
        bool DisableDateFolders { get; }

		/// <summary>
		/// Gets the orders cache timeout in milliseconds.
		/// </summary>
		/// <value>
		/// The orders cache timeout in milliseconds.
		/// </value>
		int OrdersCacheTimeoutMilliseconds { get; }
	}
}