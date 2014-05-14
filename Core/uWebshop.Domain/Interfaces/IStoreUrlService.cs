using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	interface IStoreUrlService
	{
		IEnumerable<IStoreUrl> GetStoreUrlsWithDomain();
		IEnumerable<IStoreUrl> GetStoreUrlsWithoutDomain();
		string GetCanonicalUrlForStore(IStore store);
		void LoadStoreUrls(IEnumerable<Store> allStores);
	}
}