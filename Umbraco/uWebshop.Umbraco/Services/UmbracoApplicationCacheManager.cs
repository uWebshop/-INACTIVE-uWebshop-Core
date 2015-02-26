using System;
using System.Threading;
using System.Timers;
using uWebshop.Domain.Services;
using umbraco;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Repositories;
using Timer = System.Timers.Timer;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoApplicationCacheManagingService : IApplicationCacheManagingService
	{
		private readonly IProductService _productService;
		private readonly IProductVariantService _productVariantService;
		private readonly IProductVariantGroupService _productVariantGroupService;
		private readonly ICategoryService _categoryService;
		private readonly IOrderDiscountService _orderDiscountService;
		private readonly IProductDiscountService _productDiscountService;
		private readonly IProductRepository _productRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly IProductVariantGroupRepository _productVariantGroupRepository;
		private readonly IProductVariantRepository _productVariantRepository;
		private readonly IStoreService _storeService;
		private readonly IApplicationCacheService _applicationCacheService;

		private Timer _timer;
		private Lazy<bool> _cacheReset;
		private bool _storesDirty;
		private bool _settingsDirty;

		private readonly bool _manageUmbracoXMLCacheWhenLoadBalanced;

		public UmbracoApplicationCacheManagingService(IProductService productService, IProductVariantService productVariantService, IProductVariantGroupService productVariantGroupService, ICategoryService categoryService, 
			IOrderDiscountService orderDiscountService, IProductDiscountService productDiscountService,
			IProductRepository productRepository, ICategoryRepository categoryRepository, IProductVariantGroupRepository productVariantGroupRepository, IProductVariantRepository productVariantRepository,
			IStoreService storeService, IApplicationCacheService applicationCacheService)//, IShippingProviderService shippingProviderService, IPaymentProviderService paymentProviderService)
		{
			_productService = productService;
			_productVariantService = productVariantService;
			_productVariantGroupService = productVariantGroupService;
			_categoryService = categoryService;
			_orderDiscountService = orderDiscountService;
			_productDiscountService = productDiscountService;
			_productRepository = productRepository;
			_categoryRepository = categoryRepository;
			_productVariantGroupRepository = productVariantGroupRepository;
			_productVariantRepository = productVariantRepository;
			_storeService = storeService;
			_applicationCacheService = applicationCacheService;

			_manageUmbracoXMLCacheWhenLoadBalanced = System.Web.Configuration.WebConfigurationManager.
				AppSettings["uWebshopLoadBalancedNoUmbraco"] != "true";
		}

		public void Initialize()
		{
			MakeCacheResetter();
			_timer = new Timer(10000); // todo: config
			_timer.Elapsed += DoFullReset;
		}

		public void ReloadEntityWithGlobalId(int id, string typeName = null)
		{
			//if (nodeTypeAlias == null) nodeTypeAlias = new Document(id).ContentType.Alias;
			//IO.Container.Resolve<IEntityServiceService>().GetByTypeAlias(nodeTypeAlias).ReloadEntityWithId(id);
			
			if (Settings.NodeAlias == typeName)
			{
				Log.Instance.LogDebug("Settings published, requesting rebuild cache");
				_settingsDirty = true;
			}
			else
			{
				if (Store.IsAlias(typeName))
				{
					Log.Instance.LogDebug("Store published, requesting rebuild cache");
					_storesDirty = true;
				}
				Log.Instance.LogDebug("Issueing Full Reset Cache after 'reload entity with id'");
				FullResetTrigger();
			}
		}

		public void UnloadEntityWithGlobalId(int id, string typeName = null)
		{
			//if (typeName == null) typeName = new Document(id).ContentType.Alias;

			//if (Product.IsAlias(typeName))
			//	_productService.UnloadEntityWithId(id);
			//else if (ProductVariant.IsAlias(typeName))
			//	_productVariantService.UnloadEntityWithId(id);

			if (Store.IsAlias(typeName))
			{
				Log.Instance.LogDebug("Store published, requesting rebuild cache");
				_storesDirty = true;
			}
			Log.Instance.LogDebug("Issueing Full Reset Cache after 'unload entity with id'");
			FullResetTrigger();
		}

		public void RebuildTriggeredByRemoteServer()
		{
			if (_manageUmbracoXMLCacheWhenLoadBalanced)
			{
				Log.Instance.LogDebug("Issueing library.RefreshContent() after remote server change");
				library.RefreshContent();

				Thread.Sleep(3000);
			}

			Log.Instance.LogDebug("Issueing Full Reset Cache after remote server change");
			var a = _cacheReset.Value;
		}

		private void MakeCacheResetter()
		{
			_cacheReset = new Lazy<bool>(() => // Lazy, so ignore any new requests
			{
				var t = new Thread(FullResetWorker);
				t.Start();
				return false;
			});
		}

		private void FullResetTrigger()
		{
			Log.Instance.LogDebug("Request to reset cache, (re)setting timer");
			_timer.Stop();
			_timer.Start();
		}
		private void DoFullReset(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			try
			{
				_timer.Stop();
				Log.Instance.LogDebug("Request to reset cache, timer triggered");
				var a = _cacheReset.Value;
				_applicationCacheService.TriggerRemoteRebuild();
			}
			catch (Exception ex)
			{
				Log.Instance.LogError(ex, "There was an exception while rebuilding the cache or triggering the load balanced remote rebuild " + ex);
				throw;
			}
		}

		private void FullResetWorker()
		{
			// todo: reimplement using TPL
			// todo: if new publish detected, don't update cache, but wait untill there is no new publish detected in the timeframe
			Thread.Sleep(GlobalSettings.DebugMode ? 0 : 2000);

			if (_storesDirty)
			{
				Log.Instance.LogDebug("Clearing stores cache after a change to a store");
				_storeService.InvalidateCache();
				_storesDirty = false;
			}
			if (_settingsDirty)
			{
				var newSettings = SettingsLoader.GetSettings();
				var oldSettings = IO.Container.Resolve<ISettingsService>();
				oldSettings.TriggerSettingsChangedEvent(newSettings);
				if (newSettings.IncludingVat != oldSettings.IncludingVat)
				{
					_productService.ReloadWithVATSetting();
				}
				_settingsDirty = false;
			}

			Log.Instance.LogDebug("Rebuilding new cache");
			var allCacheRebuilder = new MultiCastRebuilder(new[]
				{
					_productRepository.GetCacheRebuilder(), _productVariantRepository.GetCacheRebuilder(), _productVariantGroupRepository.GetCacheRebuilder(),_categoryRepository.GetCacheRebuilder(), 
				});
			
			allCacheRebuilder.Rebuild(); // make all the caches prepare
			try
			{
				allCacheRebuilder.Lock();
				Log.Instance.LogDebug("Switching to new cache");
				allCacheRebuilder.SwitchCache();
				Log.Instance.LogDebug("Switched to new cache");
			}
			finally
			{	
				allCacheRebuilder.Unlock();
				OldCacheStuff();
				MakeCacheResetter();
			}
			
		}

		private void OldCacheStuff()
		{
			_productService.FullResetCache();
			_productVariantService.FullResetCache();
			_productVariantGroupService.FullResetCache();
			_categoryService.FullResetCache();
			_orderDiscountService.FullResetCache();
			_productDiscountService.FullResetCache();
			UmbracoStaticCachedEntityRepository.ResetStaticCache();
			UmbracoStaticCachedEntityRepository.ResetEntityCache();
		}
	}

	// idee
	internal interface IEntityServiceService
	{
		IEntityService<T> GetByTypeAlias<T>(string alias);
	}
}