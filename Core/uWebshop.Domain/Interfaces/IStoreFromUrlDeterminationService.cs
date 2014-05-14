namespace uWebshop.Domain.Interfaces
{
	internal interface IStoreFromUrlDeterminationService
	{
		StoreUrlDeterminationResult DetermineStoreAndUrlParts(string domain, string url);
	}
	class StoreUrlDeterminationResult
	{
		public StoreUrlDeterminationResult(Store store, string storeUrl, string catalogUrl)
		{
			Store = store;
			StoreUrl = storeUrl;
			CatalogUrl = catalogUrl;
		}
		public Store Store;
		public string StoreUrl;
		public string CatalogUrl;
	}
}