using System;
using System.Configuration;
using uWebshop.Common.ServiceInterfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class UwebshopConfiguration : IUwebshopConfiguration
	{
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
			DisableDateFolders = ConfigurationManager.AppSettings["uwbsDisableDateFolders"] == "true";
			LoadConnectionString();
		}

		private void LoadConnectionString()
		{
			var uwbsconnectionStringSettings = ConfigurationManager.ConnectionStrings["uWebshop"];
			if (uwbsconnectionStringSettings != null && !string.IsNullOrWhiteSpace(uwbsconnectionStringSettings.ConnectionString))
			{
				ConnectionString = uwbsconnectionStringSettings.ConnectionString;
			}
			if (string.IsNullOrWhiteSpace(ConnectionString))
			{
				var umbConnectionStringSettings = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];
				if (umbConnectionStringSettings != null && !string.IsNullOrWhiteSpace(umbConnectionStringSettings.ConnectionString))
				{
					ConnectionString = umbConnectionStringSettings.ConnectionString;
				}
				else
				{
					var appSetting = ConfigurationManager.AppSettings["umbracoDbDSN"];
					if (string.IsNullOrWhiteSpace(appSetting))
					{
						throw new Exception("No connection string for uWebshop, please configure a connection string named uWebshop or umbracoDbDSN");
					}
					ConnectionString = appSetting;
				}
			}
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		public static IUwebshopConfiguration Current { get; internal set; }

		/// <summary>
		/// Gets the examine indexer.
		/// </summary>
		public string ExamineIndexer { get; private set; }

		/// <summary>
		/// Gets the setting indicating whether the basket is shared between all stores.
		/// </summary>
		public bool ShareBasketBetweenStores { get; private set; }

		/// <summary>
		/// Gets a value indicating whether [permanent redirect old catalog urls].
		/// </summary>
		public bool PermanentRedirectOldCatalogUrls { get; private set; }

		/// <summary>
		/// Gets a value indicating whether opening orders in the backend should create datefolder structure
		/// </summary>
		public bool DisableDateFolders { get; private set; }

		/// <summary>
		/// Gets the legacy category URL identifier.
		/// </summary>
		public string LegacyCategoryUrlIdentifier { get; private set; }

		/// <summary>
		/// Gets the legacy product URL identifier.
		/// </summary>
		public string LegacyProductUrlIdentifier { get; private set; }

		/// <summary>
		/// Gets the category URL.
		/// </summary>
		public string CategoryUrl { get; private set; }

		/// <summary>
		/// Gets the product URL.
		/// </summary>
		public string ProductUrl { get; private set; }

		/// <summary>
		/// Gets the examine searcher.
		/// </summary>
		public string ExamineSearcher { get; private set; }

		/// <summary>
		/// Gets the orders cache timeout in milliseconds.
		/// </summary>
		public int OrdersCacheTimeoutMilliseconds { get; private set; }

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		public string ConnectionString { get; private set; }
	}
}