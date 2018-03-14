using Examine;
using Examine.SearchCriteria;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Model;

namespace uWebshop.Domain.StoreCache
{
    public static class StoreDomainCache
    {
        public static ConcurrentDictionary<string, DomainStore> _cache = new ConcurrentDictionary<string, DomainStore>();

        public static void FillCache()
        {
            Log.Instance.LogWarning("Filling Domain Cache...");

            var searcher = ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];

            ISearchCriteria searchCriteria = searcher.CreateSearchCriteria();
            var results = searcher.Search(searchCriteria.RawQuery("uwbsStorePicker:[0* TO 9*]"));

            var ds = ApplicationContext.Current.Services.DomainService;

            foreach (var r in results)
            {
                var domains = ds.GetAssignedDomains(r.Id, false);
                var _storeId = int.TryParse(r.Fields["uwbsStorePicker"], out int storeId);
                var storeAlias = r.Fields["nodeName"];

                if (domains.Any())
                {
                    var domainUrl = GetDomainPrefix(domains.First().DomainName.TrimEnd('/'));

                    Log.Instance.LogWarning("Adding PickerNodeID: " + r.Id + " StoreId: " + storeId + " Domain: " + domainUrl);

                    AddOrUpdateItemCache(storeId, domainUrl, storeAlias);
                } else
                {
                    // Only happens when the store picker node has no domains set.

                    Log.Instance.LogWarning("Adding PickerNodeID: " + r.Id + " StoreId: " + storeId + " Domain: Empty");

                    AddOrUpdateItemCache(storeId, "", storeAlias);
                }
            }


            Log.Instance.LogWarning("Finished Filling Domain Cache...");
        }

        public static void AddOrUpdateItemCache(int storeId, string domainUrl,string storeAlias)
        {
            string cacheKey = storeId.ToString();

            _cache.AddOrUpdate(cacheKey, alias => new DomainStore(storeId, domainUrl, storeAlias), (key, oldValue) => oldValue);

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
