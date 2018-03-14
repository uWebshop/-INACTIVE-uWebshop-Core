using System;
using System.Collections.Generic;
using uWebshop.Common;
using uWebshop.Domain.Core;
using uWebshop.Umbraco.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Repositories;
using uWebshop.Umbraco.Services;
using HttpContextWrapper = uWebshop.Domain.Businesslogic.HttpContextWrapper;

namespace uWebshop.Umbraco
{
	public class UmbracoAddon : SimpleAddon
	{
		public static bool VersionSpecificTypesConfiguredInIOCContainer = false;

		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 12;
		}
		public override string Name()
		{
			return "Umbraco integration";
		}
		public override void DependencyRegistration(IRegistrationControl container)
		{
			// entity services (will need to move to Domain)
			container.RegisterType<IStoreService, UmbracoStoreService>(); // todo: move to Domain
			container.RegisterType<IZoneService, UmbracoZoneService>();

			container.RegisterType<IRebuildIndicesService, ExamineRebuildIndicesService>();
			container.RegisterType<ICMSApplication, UmbracoApplication>();
			container.RegisterType<IApplicationCacheManagingService, UmbracoApplicationCacheManagingService>();
			container.RegisterType<ICMSDocumentTypeService, UmbracoDocumentTypeService>();
			container.RegisterType<IAuthenticationProvider, UmbracoDotnetMembershipAuthenticationProvider>();
			container.RegisterType<IStoreUrlService, UmbracoStorePickerStoreUrlService>();

			container.RegisterType<IStoreRepository, UmbracoStoreRepository>();
			container.RegisterType<IPaymentProviderRepository, UmbracoPaymentProviderRepository>();
			container.RegisterType<IPaymentProviderMethodRepository, UmbracoPaymentProviderMethodRepository>();
			container.RegisterType<IShippingProviderRepository, UmbracoShippingProviderRepository>();
			container.RegisterType<IShippingProviderMethodRepository, UmbracoShippingProviderMethodRepository>();
			container.RegisterType<ICMSEntityRepository, UmbracoStaticCachedEntityRepository>();
			container.RegisterType<IOrderDiscountRepository, UmbracoOrderDiscountRepository>();
			container.RegisterType<IProductDiscountRepository, UmbracoProductDiscountRepository>();
			container.RegisterType<IProductRepository, PlainProductRepository>();
			container.RegisterType<IProductVariantGroupRepository, PlainVariantGroupRepository>();
			container.RegisterType<IProductVariantRepository, PlainVariantRepository>();
			container.RegisterType<ICategoryRepository, PlainCategoryRepository>();

			container.RegisterType<ICMSContentService, CMSContentService>();

			container.RegisterType<ISettingsService, SettingsService>();

			container.RegisterType<IHttpContextWrapper, HttpContextWrapper>();
            
            container.RegisterType<IDataTypeDefinitions, DataTypes.DataTypes>();
			container.RegisterType<ILoggingService, UmbracoLoggingService>();
			container.RegisterType<ICMSInstaller, CMSInstaller>();
			container.RegisterType<IUmbracoDocumentTypeInstaller, UmbracoDocumentTypeInstaller>();
			container.RegisterType<ICMSChangeContentService, CMSChangeContentService>();
		}
		public override int StateInitializationOrder()
		{
			return InitializationOrder.Settings;
		}
		public override void StateInitialization(IInitializationControl control)
		{
			try
			{
				var settings = SettingsLoader.GetSettings(); // todo: make examine based fallback
				var settingsService = IO.Container.Resolve<ISettingsService>() as SettingsService;
				if (settingsService != null)
				{
                    settingsService.IncludingVat = settings.IncludingVat;
					settingsService.UseLowercaseUrls = settings.UseLowercaseUrls;
					settingsService.IncompleteOrderLifetime = settings.IncompleteOrderLifetime;
					settingsService.RegisterSettingsChangedEvent(s =>
						{ 
							settingsService.IncludingVat = s.IncludingVat;
							settingsService.IncompleteOrderLifetime = s.IncompleteOrderLifetime;
							settingsService.UseLowercaseUrls = s.UseLowercaseUrls;
						});
				}

				IO.Container.Resolve<IApplicationCacheManagingService>().Initialize();

				control.Done();
			}
			catch (Exception)
			{
				control.NotNow();
				//control.FatalError("Could not load settings");
			}
		}
		class SettingsService : ISettingsService
		{
			public bool IncludingVat { get; set; }
			public bool UseLowercaseUrls { get; set; }
			public int IncompleteOrderLifetime { get; set; }

			public void RegisterSettingsChangedEvent(Action<ISettings> e)
			{
				_settingsChangedEvents.Add(e);
			}

			private readonly List<Action<ISettings>> _settingsChangedEvents = new List<Action<ISettings>>();
			public void TriggerSettingsChangedEvent(ISettings settings)
			{
				foreach (var e in _settingsChangedEvents)
				{
					e(settings);
				}
			}
		}
	}
}