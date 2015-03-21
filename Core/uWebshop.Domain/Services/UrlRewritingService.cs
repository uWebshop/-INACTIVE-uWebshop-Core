using System.Linq;
using System.Web;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Domain.Services
{
	internal class UrlRewritingService : IUrlRewritingService
	{
		private readonly ICatalogUrlResolvingService _catalogUrlResolvingService;
		private readonly ICMSApplication _cmsApplication;
		private readonly IUwebshopConfiguration _configuration;
		private readonly IStoreFromUrlDeterminationService _storeFromUrlDeterminationService;
		private readonly IStoreService _storeService;
		private readonly IApplicationCacheService _applicationCacheService;
		private readonly IHttpContextWrapper _httpContextWrapper;
		private readonly IPaymentProviderService _paymentProviderService;

		public UrlRewritingService(ICatalogUrlResolvingService catalogUrlResolvingService, ICMSApplication cmsApplication, IPaymentProviderService paymentProviderService, IHttpContextWrapper httpContextWrapper, IUwebshopConfiguration configuration, IStoreFromUrlDeterminationService storeFromUrlDeterminationService, IStoreService storeService, IApplicationCacheService applicationCacheService)
		{
			_catalogUrlResolvingService = catalogUrlResolvingService;
			_cmsApplication = cmsApplication;
			_paymentProviderService = paymentProviderService;
			_httpContextWrapper = httpContextWrapper;
			_configuration = configuration;
			_storeFromUrlDeterminationService = storeFromUrlDeterminationService;
			_storeService = storeService;
			_applicationCacheService = applicationCacheService;
		}

		public ResolveUwebshopEntityUrlResult ResolveUwebshopEntityUrl()
		{
			string absolutePath = _httpContextWrapper.AbsolutePath.ToLowerInvariant();
			return ResolveUwebshopEntityUrl(absolutePath);
		}

		public ResolveUwebshopEntityUrlResult ResolveUwebshopEntityUrl(string absolutePath)
		{
			var result = default(ResolveUwebshopEntityUrlResult);
			//var absolutePath = _httpContextWrapper.AbsolutePath.ToLowerInvariant();

			if (absolutePath.ToLowerInvariant().StartsWith("/umbraco") || absolutePath.ToLowerInvariant().StartsWith("/base/") || absolutePath.ToLowerInvariant() == "/umbraco/webservices/legacyajaxcalls.asmx/getsecondsbeforeuserlogout" || absolutePath.ToLowerInvariant() == "/umbraco/ping.aspx")
				return result;

			if (absolutePath.EndsWith(".aspx")) absolutePath = absolutePath.Remove(absolutePath.Length - 5);

			if (absolutePath == "" || absolutePath.EndsWith(".js") || absolutePath.EndsWith(".ico") || absolutePath.EndsWith(".gif") || absolutePath.EndsWith(".css") || absolutePath.EndsWith(".jpg") || absolutePath.EndsWith(".jpeg") || absolutePath.EndsWith(".png") || absolutePath.EndsWith(".axd"))
				return result;

			if (_cmsApplication.IsReservedPathOrUrl(absolutePath))
				return result;
			if (absolutePath != "/" && _httpContextWrapper.PathPointsToPhysicalFile(absolutePath)) // relatief duur
				return result;

			string temp;
			if (UrlPointsToCatalogRepository(absolutePath, out temp))
				return result;

			if (!_storeService.GetAllStores().Any())
				return result;

			// location might seem weird, but this is the clearest entry point for a uWebshop request. Possible todo: move it to a requestStartService
			_applicationCacheService.CheckCacheStateAndRebuildIfNeccessary();
			
			var paymentProviderRepositoryCmsNodeName = _cmsApplication.GetPaymentProviderRepositoryCMSNodeUrlName() ?? "PaymentProviders";
			var paymentProviderSectionCmsNodeName = _cmsApplication.GetPaymentProviderSectionCMSNodeUrlName() ?? "PaymentProviders";
			var paymentProviderPath = string.Format("/{0}/{1}/", paymentProviderRepositoryCmsNodeName, paymentProviderSectionCmsNodeName);
			if (absolutePath.Contains(paymentProviderPath))
			{
				absolutePath = absolutePath.TrimEnd('/');
				if (absolutePath.EndsWith(".aspx"))
					absolutePath = absolutePath.Replace(".aspx", string.Empty);

				var paymentProviderName = absolutePath.Split('/').Last();

				var paymentProviderNode = _paymentProviderService.GetPaymentProviderWithName(paymentProviderName, _storeService.GetCurrentLocalization());
				if (paymentProviderNode == null) return result;
				UwebshopRequest.Current.PaymentProvider = paymentProviderNode;
				result.Entity = paymentProviderNode;
				return result;
			}

			var domain = HttpContext.Current != null ? HttpContext.Current.Request.Url.Authority : string.Empty; // todo: decouple
			var determinationResult = _storeFromUrlDeterminationService.DetermineStoreAndUrlParts(domain, absolutePath);
			
			// todo: clean this up
			if (determinationResult == null)
			{
				var cookie = HttpContext.Current == null ? null : HttpContext.Current.Request.Cookies["StoreInfo"];
				if (cookie == null)
				{
					var s = _storeService.GetAllStores().FirstOrDefault();
					if (s == null) return result;
					determinationResult = new StoreUrlDeterminationResult(s, "/", absolutePath);
				}
				else
				{
					var storeAliasFromCookie = cookie.Values["StoreAlias"];
					var store = _storeService.GetByAlias(storeAliasFromCookie);
					if (store == null) return result;
					determinationResult = new StoreUrlDeterminationResult(store, "/", absolutePath);
				}
			}
			else
			{
				Log.Instance.LogDebug("determinationResult != null: determinationResult.StoreUrl: " + determinationResult.StoreUrl);
				determinationResult.Store.StoreURL = determinationResult.StoreUrl; // set url for this request (todo: different location?)
			}
			
			result.StoreUrl = determinationResult.StoreUrl;
			UwebshopRequest.Current.CurrentStore = determinationResult.Store; // set Store for this request (todo: different location?)

			if (HttpContext.Current != null)
			{
				var determinedStore = UwebshopRequest.Current.CurrentStore;

				Log.Instance.LogDebug("UrlRewriting: determinedStore: " + determinedStore.Alias);

				ILocalization currentLocalization = null;

				var currencyCode = HttpContext.Current.Request["currency"];

				if (currencyCode != null && determinedStore.CurrencyCodes.Contains(currencyCode.ToUpperInvariant()))
				{
					currentLocalization = Localization.CreateLocalization(determinedStore, currencyCode);

				}
				else if (HttpContext.Current.Request.Cookies["StoreInfo"] != null)
				{
					var currency = HttpContext.Current.Request.Cookies["StoreInfo"].Values["currency"];
					if (!string.IsNullOrEmpty(currency) && determinedStore.CurrencyCodes.Contains(currency.ToUpperInvariant()))
					{
						currentLocalization = Localization.CreateLocalization(determinedStore, currency);
					}
				}

				UwebshopRequest.Current.Localization = currentLocalization;
				
				if (currencyCode != null)
				{
					StoreHelper.SetStoreInfoCookie(determinedStore.Alias, currencyCode.ToUpperInvariant());
				}
				else
				{
					StoreHelper.SetStoreInfoCookie(determinedStore.Alias);
				}
			}
			
		 

			//CatalogUrlResolveServiceResult catalogUrlResolveServiceResult = _catalogUrlSplitterService.DetermineCatalogUrlComponents(absolutePath); // relatief duur
			//result.StoreUrl = catalogUrlResolveServiceResult.StoreNodeUrl;

			var catalogUrl = determinationResult.CatalogUrl;
			if (string.IsNullOrEmpty(catalogUrl) || catalogUrl == "/")
				return result;
			
			var urlParts = catalogUrl.Trim('/').ToLower().Split('/').Where(urlvalue => urlvalue.Length > 0).ToList();
			var categoryUrlName = string.Join("/", urlParts) + "/";
			var categoryUrlNoProduct = string.Join("/", urlParts.Take(urlParts.Count - 1)) + "/"; // categoryUrlName.Replace(productUrlName, string.Empty);
			var productUrlName = urlParts.Last();

			var uwebshopRequest = UwebshopRequest.Current;
			// todo: might be a bug in method below, accepting wrong categoryUrlNoProduct
			var product = _catalogUrlResolvingService.GetProductFromUrlName(categoryUrlNoProduct, productUrlName);
			if (product != null)
			{
				//uwebshopRequest.Category = _catalogUrlResolvingService.GetCategoryFromUrlName(categoryUrlNoProduct);
				uwebshopRequest.Product = product;
				// todo: inefficient, same determination in _catalogUrlResolvingService.GetProductFromUrlName
				uwebshopRequest.CategoryPath = _catalogUrlResolvingService.GetCategoryPathFromUrlName(categoryUrlNoProduct).ToList(); 
				uwebshopRequest.Category = uwebshopRequest.CategoryPath.LastOrDefault();
				result.Entity = product;
				result.CategoryUrl = categoryUrlNoProduct;
				result.ProductUrl = productUrlName;
				return result;
			}
			uwebshopRequest.CategoryPath = _catalogUrlResolvingService.GetCategoryPathFromUrlName(categoryUrlName).ToList();
			var category = uwebshopRequest.CategoryPath.LastOrDefault();
			if (category != null)
			{
				uwebshopRequest.Category = category;
				result.Entity = category;
				result.CategoryUrl = categoryUrlName;
			}
			return result;
		}

		public void Rewrite()
		{
			var result = ResolveUwebshopEntityUrl();
			var content = result.Entity;

			if (content is PaymentProvider)
			{
				var rewritePath = _storeService.GetAllStores().First().StoreURL + "?paymentprovider=" + (content as PaymentProvider).Name + _httpContextWrapper.QueryString;

				Log.Instance.LogDebug("Rewrite: StoreHelper.GetAllStores().First().StoreURL: " + _storeService.GetAllStores().First().StoreURL);
				
				_httpContextWrapper.RewritePath(FixUrlForRewrite(rewritePath));

				return;
			}

			if (content is Product)
			{
				// NB: keep &category= for backwards compatibility (bad razor files..)
				_httpContextWrapper.RewritePath(FixUrlForRewrite(result.StoreUrl) + "?resolvedProductId=" + content.Id + "&category=" + result.CategoryUrl + "&product=" + result.ProductUrl + _httpContextWrapper.QueryString);
				return;
			}
			if (content is Category)
			{
				_httpContextWrapper.RewritePath(FixUrlForRewrite(result.StoreUrl) + "?resolvedCategoryId=" + content.Id + "&category=" + result.CategoryUrl + _httpContextWrapper.QueryString);
			}
		}
		internal static string FixUrlForRewrite(string url)
		{
			// todo: to do this properly, we need to store domain and path separately
			if (url.StartsWith("http"))
			{
				var absUrlStart = url.IndexOf('/', 8);
				return url.Substring(absUrlStart);
			}
			return "/" + url.TrimStart('/');
		}

		public void RedirectPermanentOldCatalogUrls()
		{
			if (!_configuration.PermanentRedirectOldCatalogUrls) return;

			string uwbsCategoryUrlIdentifier = _configuration.LegacyCategoryUrlIdentifier + "/";
			string uwbsProductUrlIdentifier = _configuration.LegacyProductUrlIdentifier + "/";

			string absolutePath = _httpContextWrapper.AbsolutePath.ToLowerInvariant();
			if (string.IsNullOrWhiteSpace(absolutePath) || absolutePath == "/")
				return;
			bool redirect = false;
			if (absolutePath.Contains("/" + uwbsCategoryUrlIdentifier))
			{
				redirect = true;
				absolutePath = absolutePath.Replace(uwbsCategoryUrlIdentifier, "");
			}
			if (absolutePath.Contains("/" + uwbsProductUrlIdentifier))
			{
				redirect = true;
				absolutePath = absolutePath.Replace(uwbsProductUrlIdentifier, "");
			}
			if (redirect)
			{
				_httpContextWrapper.RedirectPermanent(absolutePath);
			}
		}

		public bool UrlPointsToCatalogRepository(string absolutePath, out string catalogPath)
		{
			string catalogRepositoryCmsNodeName = _cmsApplication.GetCatalogRepositoryCMSNodeUrlName();
			string categoryRepositoryCmsNodeName = _cmsApplication.GetCategoryRepositoryCMSNodeUrlName();
			string productRepositoryCmsNodeName = _cmsApplication.GetProductRepositoryCMSNodeUrlName();

			if (catalogRepositoryCmsNodeName != null && categoryRepositoryCmsNodeName != null)
			{
				string uWebshopCmsNodeName = _cmsApplication.GetuWebshopCMSNodeUrlName();

				string catalogpath = string.Format("/{0}/{1}", catalogRepositoryCmsNodeName, categoryRepositoryCmsNodeName);
				string catalogpathWithuWebshop = string.Format("/{0}/{1}/{2}", uWebshopCmsNodeName, catalogRepositoryCmsNodeName, categoryRepositoryCmsNodeName);

				if (absolutePath.StartsWith(catalogpathWithuWebshop))
				{
					catalogPath = absolutePath.Replace(catalogpathWithuWebshop, string.Empty);
					return true;
				}
				if (absolutePath.StartsWith(catalogpath))
				{
					catalogPath = absolutePath.Replace(catalogpath, string.Empty);
					return true;
				}
				// redirecting from internal umbraco urls is done in ApplicationEventHandler.UmbracoDefaultBeforeRequestInit
			}
			if (catalogRepositoryCmsNodeName != null && productRepositoryCmsNodeName != null)
			{
				var uWebshopCmsNodeName = _cmsApplication.GetuWebshopCMSNodeUrlName();

				var catalogpath = string.Format("/{0}/{1}", catalogRepositoryCmsNodeName, productRepositoryCmsNodeName);
				var catalogpathWithuWebshop = string.Format("/{0}/{1}/{2}", uWebshopCmsNodeName, catalogRepositoryCmsNodeName, productRepositoryCmsNodeName);

				if (absolutePath.StartsWith(catalogpathWithuWebshop))
				{
					catalogPath = absolutePath.Replace(catalogpathWithuWebshop, string.Empty);
					return true;
				}
				if (absolutePath.StartsWith(catalogpath))
				{
					catalogPath = absolutePath.Replace(catalogpath, string.Empty);
					return true;
				}
			}

			catalogPath = string.Empty;
			return false;
		}

		//private static string GetUrlWithStorePartRemovedAndAddStoreToRequestCacheNONEXAMINEALTERNATIVE(string absolutePath, HttpContext context)
		//{
		//	return ""; // not working, because uQuery doesn't work (at this point?)
		//	var dinges = new Node(1149);
		//	context.Response.Write("aa" + dinges.UrlName);


		//	var urlParts = absolutePath.Trim('/').ToLower().Split('/').Where(urlvalue => urlvalue.Length > 0).ToList();

		//	var firstNode = uQuery.GetNodeByUrl(urlParts.Any() ? urlParts.First() : "/"); // werkt niet, waarsch door gemis umbraco context
		//	if (firstNode == null)
		//	{
		//		firstNode = uQuery.GetNodeByUrl("/");
		//		if (firstNode == null)
		//			throw new Exception("dit kan écht niet");
		//	}
		//	var deepestNode = firstNode;
		//	for (int i = 2; i < urlParts.Count; i++)
		//	{
		//		var node = uQuery.GetNodeByUrl(string.Join("/", urlParts.Take(i)));
		//		if (node == null) break;
		//		deepestNode = node;
		//	}

		//	// find store!
		//	// TODO: recursive!
		//	var prop = deepestNode.GetProperty("storepicker");
		//	if (prop != null)
		//	{
		//		var store = DomainHelper.StoreById(int.Parse(prop.Value));
		//		HttpContext.Current.Items[StoreHelper.CurrentStoreCacheKey] = store;
		//	}

		//	var nodeUrl = library.NiceUrl(deepestNode.Id);
		//	return nodeUrl.Length > 1 ? absolutePath.Replace(nodeUrl, "/") : absolutePath;
		//}
	}
}