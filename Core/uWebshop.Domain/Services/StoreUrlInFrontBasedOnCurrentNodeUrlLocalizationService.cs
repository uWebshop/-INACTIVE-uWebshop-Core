using System;
using System.Runtime.Caching;
using System.Threading;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using System.Linq;
using Umbraco.Core;

namespace uWebshop.Domain.Services
{
	internal class StoreUrlInFrontBasedOnCurrentNodeUrlLocalizationService : IUrlLocalizationService
	{
		public string LocalizeCatalogUrl(string catalogUrl, ILocalization localization)
		{
            try
            {
                if (catalogUrl == null) throw new ArgumentNullException("catalogUrl");
                if (localization == null) throw new ArgumentNullException("localization");

                var store = (Store)localization.Store;
                //if (store.CanonicalStoreURL == null) throw new Exception("store.CanonicalStoreURL == null");
                //if (store.StoreURL == null) throw new Exception("store.StoreURL == null");

                //if (store.CanonicalStoreURL == null || store.StoreURL == null)
                //{
                //	LogWarningOfDelayMaximumOncePer5Minutes(store);
                //	Thread.Sleep(300);
                //}

                var storeURL = localization.Store.StoreURL ?? string.Empty;

                if (string.IsNullOrEmpty(storeURL))
                {
                    var hasStore = StoreCache.StoreDomainCache._cache.Any(x => x.Value.StoreId == store.Id);

                    if (hasStore)
                    {
                        var storeDomain = StoreCache.StoreDomainCache._cache.LastOrDefault(x => x.Value.StoreId == store.Id);

                        var domainUrl = storeDomain.Value.DomainUrl + "/" + catalogUrl.TrimStart('/');

                        return domainUrl;

                    }

                } else
                {

                    return storeURL.TrimEnd('/') + "/" + catalogUrl.TrimStart('/');

                }

                return catalogUrl.TrimStart('/');

            } catch(Exception ex)
            {
                Log.Instance.LogError(ex, "LocalizeCatalogUrl, Store Url. Remove logging later.");
                return "";
            }

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

        public static string GetDomainPrefix(string url)
        {
            // Handle domains w/ scheme
            bool _uriResult = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);

            if (_uriResult)
            {
                return uriResult.AbsolutePath;
            }
            else
            {
                var firstIndexOf = url.IndexOf("/");

                return firstIndexOf > 0 ? url.Substring(firstIndexOf) : string.Empty;
            }
        }
    }
}
