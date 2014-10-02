using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Examine;
using uWebshop.Domain.Model;
using uWebshop.Domain.Services;
using umbraco;
using umbraco.cms.businesslogic.web;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoStoreService : IStoreService
	{
		private readonly IStoreRepository _storeRepository;
		private readonly ICMSApplication _cmsApplication;
		private readonly ICMSEntityRepository _cmsEntityRepository;
		private List<Store> _allStoresCache;

		public UmbracoStoreService(IStoreRepository storeRepository, ICMSApplication cmsApplication, ICMSEntityRepository cmsEntityRepository)
		{
			_storeRepository = storeRepository;
			_cmsApplication = cmsApplication;
			_cmsEntityRepository = cmsEntityRepository;
		}

		public Store GetCurrentStoreNoFallback()
		{
			var uwbsRequest = UwebshopRequest.Current;

			var store = uwbsRequest.CurrentStore; // currently url overrules everything (when doing that differently, do it in CatalogUrlResolveService!)
			if (store != null)
			{
				return store;
			}

			// in feite zou onderstaande code alleen in de backend bereikt kunnen worden of wanneer op node buiten shop tree
			if (HttpContext.Current != null)
			{
			    // get store from cookie (this makes javascript understand what store there is)
                if (!_cmsApplication.RequestIsInCMSBackend(HttpContext.Current))
			    {
			        store = _storeRepository.TryGetStoreFromCookie();

                    if (store != null)
                    {
                        Log.Instance.LogDebug("GetCurrentStoreNoFallback, request outside shop, ABLE to use cookie to determine store");
                    }
			    }

                if(store == null)
			    {
                    Log.Instance.LogDebug("GetCurrentStoreNoFallback, request outside shop, UNable to use cookie to determine store");

			        var storeResult =
			            IO.Container.Resolve<IStoreFromUrlDeterminationService>()
			                .DetermineStoreAndUrlParts(HttpContext.Current.Request.Url.Authority,
			                    HttpContext.Current.Request.Url.AbsolutePath);

			        //todo gewoon altijd via storeResult ipv onderstaande checks? alleen als die null is terugvallen op node/order???
			        // waarom wilden we hier eerst de andere opties en pas daarna terugvallen op wat storeResult bepaald?

			        if (storeResult != null)
			        {
			            store = storeResult.Store;
			        }

			        if (store == null)
			        {
			            store = _storeRepository.TryGetStoreFromCurrentNode();
			        }
			    }

			    //store = storefromOrder ?? storefromCurrentNode ?? storefromDomain;
				//if (store == null && storeResult != null)
				//{
				//	store = storeResult.Store;
				//}
			}

		    if (store != null && !string.IsNullOrEmpty(store.Alias))
			{
				uwbsRequest.CurrentStore = store;
			}
            
			return store;
		}

		public void RegisterStoreChangedEvent(Action<Store> e)
		{
			_storeChangedEvents.Add(e);
		}

		private readonly List<Action<Store>> _storeChangedEvents = new List<Action<Store>>(); 
		public void TriggerStoreChangedEvent(Store store)
		{
			foreach (var e in _storeChangedEvents)
			{
				e(store);
			}
		}

		public void InvalidateCache(int storeId = 0)
		{
			// todo: maybe trigger store changed event
			_allStoresCache = null;
		}

		public ILocalization GetCurrentLocalization()
		{
			var uwebshopRequest = UwebshopRequest.Current;

			if (uwebshopRequest.Localization != null)
			{
				return uwebshopRequest.Localization;
			}

			var store = GetCurrentStore();
			if (store != null)
			{
				if (HttpContext.Current != null)
				{
					var currencyCode = HttpContext.Current.Request["currency"];
					if (currencyCode != null && store.CurrencyCodes.Contains(currencyCode.ToUpperInvariant()))
					{
						uwebshopRequest.Localization = Localization.CreateLocalization(store, currencyCode);
						StoreHelper.SetStoreInfoCookie(store.Alias, currencyCode.ToUpperInvariant());
					}
					else if (HttpContext.Current.Request.Cookies["StoreInfo"] != null)
					{
						var currency = HttpContext.Current.Request.Cookies["StoreInfo"].Values["currency"];
						if (!string.IsNullOrEmpty(currency) && store.CurrencyCodes.Contains(currency.ToUpperInvariant()))
						{
							uwebshopRequest.Localization = Localization.CreateLocalization(store, currency);
						}
					}
				}
				if (uwebshopRequest.Localization == null)
				{
					uwebshopRequest.Localization = Localization.CreateLocalization(store);
				}
			}
            
			return uwebshopRequest.Localization;
		}

		public IEnumerable<ILocalization> GetAllLocalizations()
		{
			var result = new List<ILocalization>();
			foreach (var store in GetAllStores())
			{
				foreach (var currency in store.CurrencyCodes) //new[] {store.DefaultCurrencyCultureSymbol}.Concat(
				{
					result.Add(Localization.CreateLocalization(store, currency));
				}
			}
			return result;
		}

		public Store GetCurrentStore()
		{
			var store = GetCurrentStoreNoFallback();

			if (store == null)
			{
                Log.Instance.LogError("Could not determine current store, fallback cookie");
				store = _storeRepository.TryGetStoreFromCookie();
			}

			if (store == null)
			{
				Log.Instance.LogError("Could not determine current store, fallback to first store");
				store = GetAllStores().OrderBy(x => x.SortOrder).FirstOrDefault(); // fallback
			}

			if (store == null || string.IsNullOrEmpty(store.Alias))
			{
				store = new Store {Alias = "uWebshop", GlobalVat = 21, Id = 1, NodeTypeAlias = "uwbsStoreDummy", CountryCode = "nl-NL", Culture = "nl-NL", CurrencyCulture = "nl-NL", IncompleOrderLifetime = 3600, SortOrder = 1, StoreURL = "/", StoreUrlWithoutDomain = "/", CanonicalStoreURL = "/"};
				Log.Instance.LogWarning("uWebshop had to make a dummy store, please make sure that a published store exists and republish entire site & rebuild examine index, if this doesn't work please reset the IIS application pool");
			}

            Log.Instance.LogDebug("GetCurrentStore() store: " + store.Alias);

			UwebshopRequest.Current.CurrentStore = store;
			return store;
		}

		private static Store TryGetStoreFromQueryString()
		{
			if (HttpContext.Current == null) return null;
			int storeId;
			var storeIdValue = HttpContext.Current.Request.QueryString["storeId"];
			if (!string.IsNullOrWhiteSpace(storeIdValue) && int.TryParse(storeIdValue, out storeId))
			{
				return DomainHelper.StoreById(storeId);
			}
			return null;
		}

		public string CurrentStoreAlias()
		{
			var store = GetCurrentStore();
			return store != null ? store.Alias : string.Empty;
		}

		public IEnumerable<Store> GetAllStores()
		{
			if (_allStoresCache == null || !_allStoresCache.Any())
			{
				_allStoresCache = _storeRepository.GetAll();
				//AllStoresCache = DomainHelper.GetNodeIdForDocumentAlias(Store.NodeAlias).Select(id => new Store(id)).ToList();
				var storeUrlService = IO.Container.Resolve<IStoreUrlService>(); // (circular dependency..)
				storeUrlService.LoadStoreUrls(_allStoresCache);
				foreach (var store in _allStoresCache)
				{
					store.CanonicalStoreURL = storeUrlService.GetCanonicalUrlForStore(store);
				}
			}
			return _allStoresCache;
		}

		public Store GetById(int id, ILocalization localization)
		{
			// todo: wrong, localization ignored left of ??
			return GetAllStores().FirstOrDefault(store => store.Id == id) ?? _storeRepository.GetById(id, localization); // node fallback to be a little bit more sure
		}

		public Store GetByAlias(string storeAlias)
		{
			if (storeAlias == null) return null;
			return GetAllStores().FirstOrDefault(store => store.Alias != null && (store.Alias == storeAlias || store.Alias.ToLowerInvariant() == storeAlias.ToLowerInvariant()));
		}

		public void LoadStoreUrl(Store store)
		{
		}

		private Store GetStoreByAliasOrCurrentStore(string storeAlias)
		{
			var store = GetByAlias(storeAlias) ?? GetCurrentStore();

			if (store == null) throw new Exception("No published stores, please publish");
			return store;
		}

		public void RenameStore(string oldStoreAlias, string newStoreAlias)
		{
			if (string.IsNullOrEmpty(oldStoreAlias) || string.IsNullOrEmpty(newStoreAlias)) return;

			var docTypeList = new List<DocumentType>();

			// get all documenttypes that startswith the store specific doctypes
			foreach (var storeDependantAlias in StoreHelper.StoreDependantDocumentTypeAliasList)
			{
				docTypeList.AddRange(DocumentType.GetAllAsList().Where(x => x.Alias.StartsWith(storeDependantAlias)));
			}

			foreach (var dt in docTypeList)
			{
				// rename properties
				foreach (var property in dt.PropertyTypes)
				{
					property.Alias = property.Alias.Replace("_" + oldStoreAlias, "_" + newStoreAlias);
					property.Save();
				}

				// rename tabs + move properties to new tab
				foreach (var tab in dt.getVirtualTabs)
				{
					if (tab.Caption == oldStoreAlias)
					{
						dt.SetTabName(tab.Id, newStoreAlias);
					}
				}

				dt.Save();
			}

			library.RefreshContent();
			ExamineManager.Instance.IndexProviderCollection[UwebshopConfiguration.Current.ExamineSearcher].RebuildIndex();
		}

		public string GetNiceUrl(int id, int categoryId, ILocalization localization)
		{
			// todo: move to urlservice
			// todo: needs lots of refactoring/checking

			var store = GetByAlias(localization.StoreAlias) ?? GetCurrentStore(); // todo check
			if (store == null) throw new Exception("No published stores, please publish");

			var uwebshopNode = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(id);
			if (uwebshopNode == null) return string.Empty;

			var typeAlias = uwebshopNode.NodeTypeAlias;
			if (Product.IsAlias(typeAlias))
			{
				// todo: test
				// todo: clean
				var product = IO.Container.Resolve<IProductService>().GetById(id, localization);
				if (product == null)
				{
					return string.Empty;
				}
				var category = IO.Container.Resolve<ICategoryService>().GetById(categoryId, localization);

				var productUrlService = IO.Container.Resolve<IProductUrlService>();
				var urlFormatService = IO.Container.Resolve<IUrlFormatService>();
				var urlLocalizationService = IO.Container.Resolve<IUrlLocalizationService>();

				var productUrl = productUrlService.GetUrlInCategoryOrCanonical(product, category);

				return urlFormatService.FormatUrl(urlLocalizationService.LocalizeCatalogUrl(productUrl, localization));
			}
			if (Category.IsAlias(typeAlias))
			{
				var category = IO.Container.Resolve<ICategoryService>().GetById(id, localization);
				
                return IO.Container.Resolve<IUrlService>().CategoryUrlUsingCurrentPath(category, localization);
			}
			return IO.Container.Resolve<IUrlFormatService>().FormatUrl(library.NiceUrl(id));
		}
	}
}