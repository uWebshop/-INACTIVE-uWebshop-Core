using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal interface IStoreUrlRepository
	{
		UrlsWithAndWithoutDomain GetUrls(int storeId);
	}
	class UrlsWithAndWithoutDomain
	{
		public IEnumerable<string> WithDomain;
		public IEnumerable<string> WithoutDomain;
	}
	internal class UmbracoStorePickerStoreUrlRepository : IStoreUrlRepository
	{
		private readonly ICMSEntityRepository _cmsEntityRepository;
		private readonly ICMSApplication _cmsApplication;

		public UmbracoStorePickerStoreUrlRepository(ICMSEntityRepository cmsEntityRepository, ICMSApplication cmsApplication)
		{
			_cmsEntityRepository = cmsEntityRepository;
			_cmsApplication = cmsApplication;
		}


		public UrlsWithAndWithoutDomain GetUrls(int storeId)
		{
			if (_cmsEntityRepository == null) throw new NullReferenceException("_cmsEntityRepository");
			if (_cmsApplication == null) throw new NullReferenceException("_cmsApplication");
			if (Log.Instance == null) throw new NullReferenceException("Log.Instance");

			var urlsWithoutDomain = new List<string>();
			var urlsWithDomain = new List<string>();
			var nodes = _cmsEntityRepository.GetNodesWithStorePicker(storeId);

			if (nodes.Any(n => n == null)) throw new NullReferenceException("nodes.Any(n => n == null)");

			foreach (var storepickerNode in nodes.OrderByDescending(n => n.Level)) // order by langste path eerst
			{
				var node = storepickerNode;

				var path = string.Empty;
				var rootHasDomain = false;
				do
				{
					var domains = _cmsApplication.GetDomainsForNodeId(node.Id);
					if (domains.Any())
					{
						foreach (var domain in domains)
						{
							Log.Instance.LogDebug("LoadStoreUrls domains.Any() domain: " + domain);
							Log.Instance.LogDebug("LoadStoreUrls domains.Any() path: " + path);
						}

						var pathToUse = "/";

						if (!string.IsNullOrEmpty(path))
						{
							pathToUse = "/" + path.Trim('/') + "/";
						}

						urlsWithDomain.AddRange(domains.Select(d => d.TrimEnd('/') + pathToUse));
						// als level 1 stop
						if (node.Level == 1)
						{
							rootHasDomain = true;
						}
					}
					
					if (!(node.Level == 1 && _cmsApplication.HideTopLevelNodeFromPath && IsFirstNode(node)))
					{
						path = string.Format("{0}/{1}", node.UrlName.Trim('/'), path);
					}
					node = node.Parent;
				} while (node != null);

				if (path == string.Empty)
				{
					urlsWithoutDomain.Add("/");
				}
				else if (!rootHasDomain)
				{
					if (path.StartsWith("http"))
					{
						var pathToUse = path.TrimEnd('/') + "/";

						Log.Instance.LogDebug("path.StartsWith(http) pathToUse: " + pathToUse);

						//todo: maybe never needed...
						urlsWithDomain.Add(pathToUse);
					}
					else
					{
						var pathToUse = "/" + path.Trim('/') + "/";

						Log.Instance.LogDebug("NOT path.StartsWith(http) pathToUse: " + pathToUse);

						urlsWithoutDomain.Add(pathToUse);
					}
				}

			}
			return new UrlsWithAndWithoutDomain() {WithDomain = urlsWithDomain, WithoutDomain = urlsWithoutDomain};
		}

		private bool IsFirstNode(UwbsNode node)
		{
			return IO.Container.Resolve<ICMSContentService>().GetAllRootNodes().Min(n => n.SortOrder) == node.SortOrder;
		}
	}

	internal class UmbracoStorePickerStoreUrlService : IStoreUrlService
	{
		private readonly IStoreService _storeService;
		private readonly IStoreUrlRepository _storeUrlRepository;
		
		public UmbracoStorePickerStoreUrlService(IStoreService storeService, IStoreUrlRepository storeUrlRepository)
		{
			_storeService = storeService;
			_storeUrlRepository = storeUrlRepository;
			storeService.RegisterStoreChangedEvent(s => ReloadAllUrls());
			//ReloadAllUrls(); can't do this!
		}

		private bool _firstTimeInitialized;

		private IStoreUrl[] _storeUrlsWithDomain;
		public IEnumerable<IStoreUrl> GetStoreUrlsWithDomain()
		{
			if (!_firstTimeInitialized) {ReloadAllUrls(); _firstTimeInitialized = true; }
			if (_storeUrlsWithDomain == null) throw new Exception("StoreUrls not loaded");
			return _storeUrlsWithDomain;
		}

		private IStoreUrl[] _storeUrlsWithoutDomain;
		public IEnumerable<IStoreUrl> GetStoreUrlsWithoutDomain()
		{
			if (!_firstTimeInitialized) { ReloadAllUrls(); _firstTimeInitialized = true; }
			if (_storeUrlsWithoutDomain == null) throw new Exception("StoreUrls not loaded");
			return _storeUrlsWithoutDomain;
		}

		public string GetCanonicalUrlForStore(IStore store)
		{
			var storeUrl = GetStoreUrlsWithDomain().FirstOrDefault(u => u.Store.Alias == store.Alias)
						?? GetStoreUrlsWithoutDomain().FirstOrDefault(u => u.Store.Alias == store.Alias);
			return storeUrl != null ? storeUrl.Url : "/";
		}

		public void LoadStoreUrls(IEnumerable<Store> allStores)
		{
			var urlsWithoutDomain = new List<IStoreUrl>();
			var urlsWithDomain = new List<IStoreUrl>();
			var stores = allStores.ToArray();
			foreach (var store in stores)
			{
				var urls = _storeUrlRepository.GetUrls(store.Id);
				if (!urls.WithDomain.Any() && !urls.WithoutDomain.Any() && stores.Length == 1)
				{
					Log.Instance.LogDebug("StoreURL: could not find node with StorePicker:" + store.Id + " falling back to root url");
					urlsWithoutDomain.Add(new StoreUrl(store, "/"));
					break;
				}

				urlsWithDomain.AddRange(urls.WithDomain.Select(url => new StoreUrl(store, url)));
				urlsWithoutDomain.AddRange(urls.WithoutDomain.Select(url => new StoreUrl(store, url)));
			}

			if (!urlsWithDomain.Any() && !urlsWithoutDomain.Any())
			{
				Log.Instance.LogWarning("Determining Store Urls: no storepicker but multiple stores? ");
			}
			_storeUrlsWithDomain = urlsWithDomain.ToArray();
			_storeUrlsWithoutDomain = urlsWithoutDomain.ToArray();
		}

		private void ReloadAllUrls()
		{
			LoadStoreUrls(_storeService.GetAllStores());
		}

		private class StoreUrl : IStoreUrl
		{
			public StoreUrl(Store store, string url)
			{
				Store = store;
				Url = url;
			}

			public Store Store { get; private set; }
			public string Url { get; private set; }
		}
	}
}