using System.Configuration;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class UwebshopConfiguration : IUwebshopConfiguration
	{
		//private static readonly Lazy<IUwebshopConfiguration> _current = new Lazy<IUwebshopConfiguration>(() => IO.Container.Resolve<IUwebshopConfiguration>());

		/// <summary>
		/// Initializes a new instance of the <see cref="UwebshopConfiguration"/> class.
		/// </summary>
		public UwebshopConfiguration()
		{
			PermanentRedirectOldCatalogUrls = ConfigurationManager.AppSettings["uwbsPermanentRedirectOldCatalogUrls"] == "true";
			LegacyCategoryUrlIdentifier = ConfigurationManager.AppSettings["uwbsLegacyCategoryUrlIdentifier"] ?? "category";
			LegacyProductUrlIdentifier = ConfigurationManager.AppSettings["uwbsLegacyProductUrlIdentifier"] ?? "product";
			CategoryUrl = ConfigurationManager.AppSettings["uwbsCategoryUrl"];
			ProductUrl = ConfigurationManager.AppSettings["uwbsProductUrl"];
			ExamineSearcher = ConfigurationManager.AppSettings["uwbsExamineSearcher"] ?? "ExternalSearcher";
			ExamineIndexer = ConfigurationManager.AppSettings["uwbsExamineIndexer"] ?? "ExternalIndexer";
			ShareBasketBetweenStores = ConfigurationManager.AppSettings["uwbsShareBasketBetweenStores"] == "true";
			OrdersCacheTimeoutMilliseconds = Common.Helpers.ParseInt(ConfigurationManager.AppSettings["uwbsOrdersCacheTimeoutMilliseconds"], 2000);
            DisableDateFolders = ConfigurationManager.AppSettings["uwbsDisableDatFolders"] == "true";
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		public static IUwebshopConfiguration Current { get; internal set; }

		/// <summary>
		/// Gets the examine indexer.
		/// </summary>
		/// <value>
		/// The examine indexer.
		/// </value>
		public string ExamineIndexer { get; private set; }

		/// <summary>
		/// Gets the setting indicating whether the basket is shared between all stores.
		/// </summary>
		/// <value>
		/// The setting indicating whether the basket is shared between all stores.
		/// </value>
		public bool ShareBasketBetweenStores { get; private set; }

		/// <summary>
		/// Gets a value indicating whether [permanent redirect old catalog urls].
		/// </summary>
		/// <value>
		/// <c>true</c> if [permanent redirect old catalog urls]; otherwise, <c>false</c>.
		/// </value>
		public bool PermanentRedirectOldCatalogUrls { get; private set; }

        /// <summary>
        /// Gets a value indicating whether opening orders in the backend should create datefolder structure
        /// </summary>
        /// <value>
        /// <c>true</c> if [permanent redirect old catalog urls]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableDateFolders { get; private set; }

		/// <summary>
		/// Gets the legacy category URL identifier.
		/// </summary>
		/// <value>
		/// The legacy category URL identifier.
		/// </value>
		public string LegacyCategoryUrlIdentifier { get; private set; }

		/// <summary>
		/// Gets the legacy product URL identifier.
		/// </summary>
		/// <value>
		/// The legacy product URL identifier.
		/// </value>
		public string LegacyProductUrlIdentifier { get; private set; }

		/// <summary>
		/// Gets the category URL.
		/// </summary>
		/// <value>
		/// The category URL.
		/// </value>
		public string CategoryUrl { get; private set; }

		/// <summary>
		/// Gets the product URL.
		/// </summary>
		/// <value>
		/// The product URL.
		/// </value>
		public string ProductUrl { get; private set; }

		/// <summary>
		/// Gets the examine searcher.
		/// </summary>
		/// <value>
		/// The examine searcher.
		/// </value>
		public string ExamineSearcher { get; private set; }

		/// <summary>
		/// Gets the orders cache timeout in milliseconds.
		/// </summary>
		/// <value>
		/// The orders cache timeout in milliseconds.
		/// </value>
		public int OrdersCacheTimeoutMilliseconds { get; private set; }
	}
}