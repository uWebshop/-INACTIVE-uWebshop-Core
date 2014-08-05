using System;
using System.Runtime.Caching;
using System.Threading;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using System.Linq;

namespace uWebshop.Domain.Services
{
	internal class StoreUrlInFrontBasedOnCurrentNodeUrlLocalizationService : IUrlLocalizationService
	{
		public string LocalizeCatalogUrl(string catalogUrl, ILocalization localization)
		{
			if (catalogUrl == null) throw new ArgumentNullException("catalogUrl");
			if (localization == null) throw new ArgumentNullException("localization");

			var store = (Store)localization.Store;
			//if (store.CanonicalStoreURL == null) throw new Exception("store.CanonicalStoreURL == null");
			//if (store.StoreURL == null) throw new Exception("store.StoreURL == null");

			if (store.CanonicalStoreURL == null || store.StoreURL == null)
			{
				LogWarningOfDelayMaximumOncePer5Minutes(store);
				Thread.Sleep(300);
			}

			var storeURL = localization.Store.StoreURL ?? string.Empty;
			return storeURL.TrimEnd('/') + "/" + catalogUrl.TrimStart('/');
		}

		private void LogWarningOfDelayMaximumOncePer5Minutes(Store store)
		{
			// todo: Reactive Extensions .Throttle would be perfect here..
			var message = "No store url known for store " + store.Alias + " waiting 300ms for possible cache rebuild " + (store.CanonicalStoreURL == null ? "C" : string.Empty) + (store.StoreURL == null ? "S" : string.Empty);
			ObjectCache cache = MemoryCache.Default;
			var orders = cache["3d3b71e0-af59-4798-b40f-fcd39f9acfac" + message];
			if (orders == null)
			{
				cache.Set("3d3b71e0-af59-4798-b40f-fcd39f9acfac" + message, string.Empty, 
					new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5) });
				Log.Instance.LogWarning(message);
			}
		}
	}
}