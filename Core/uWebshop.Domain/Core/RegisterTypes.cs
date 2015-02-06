using uWebshop.Common.Interfaces;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Repositories;
using uWebshop.Domain.Services;
using VATChecker;

namespace uWebshop.Domain
{
	internal class CoreServices : SimpleAddon
	{
		public override int DependencyRegistrationOrder()
		{
			return base.DependencyRegistrationOrder() + 13;
		}

		public override string Name()
		{
			return "Uwebshop Core and default services";
		}
		public override void DependencyRegistration(IRegistrationControl container)
		{
			// CMS unaware services (uwebshop default)
			container.RegisterType<IDiscountCalculationService, DiscountCalculationService>();
			container.RegisterType<IUrlRewritingService, UrlRewritingService>();
			container.RegisterType<ICatalogUrlResolvingService, CatalogUrlResolvingService>();
			container.RegisterType<IVATCheckService, ViesVatCheckService>();
			container.RegisterType<IOrderUpdatingService, OrderUpdatingService>();
			container.RegisterType<IOrderNumberService, OrderNumberService>();
			container.RegisterType<IStockService, StockService>();
			container.RegisterType<IUwebshopRequestService, UwebshopHttpContextRequestService>();
			container.RegisterType<IInstaller, UwebshopDefaultInstaller>();
			container.RegisterType<ICurrencyCultureInfoForLocalizationService, CurrencyCultureInfoForLocalizationService>();
			container.RegisterType<IStoreFromUrlDeterminationService, StoreFromUrlDeterminationService>();
			container.RegisterType<IDefaultCurrencyCultureService, ConfigFileDefaultCurrencyCultureService>();
			
			if (System.Web.Configuration.WebConfigurationManager.AppSettings["uWebshopLoadBalanced"] == "true")
			{
				container.RegisterType<IApplicationCacheService, LoadBalancedApplicationCacheService>();
			}
			else
			{
				container.RegisterType<IApplicationCacheService, LocalApplicationCacheService>();
			}
			
			container.RegisterType<ICategoryCatalogUrlService, CategoryCatalogUrlService>();
			container.RegisterType<IUrlFormatService, UrlFormatService>();
			container.RegisterType<IUrlLocalizationService, StoreUrlInFrontBasedOnCurrentNodeUrlLocalizationService>();
			container.RegisterType<IUrlService, UrlService>();

			if (System.Web.Configuration.WebConfigurationManager.AppSettings["uWebshopCalculateVatOverTotal"] == "true")
			{
				container.RegisterType<IVatCalculationStrategy, OverTotalVatCalculationStrategy>();
			}
			else
			{
				container.RegisterType<IVatCalculationStrategy, OverSmallestPartsVatCalculationStrategy>(); // default
			}

			container.RegisterType<IStoreUrlRepository, UmbracoStorePickerStoreUrlRepository>();

			// entity services (more registered in Umbraco part)
			container.RegisterType<IProductService, ProductService>();
			container.RegisterType<IProductVariantGroupService, ProductVariantGroupService>();
			container.RegisterType<IProductVariantService, ProductVariantService>();

			container.RegisterType<ICategoryService, CategoryService>();
			container.RegisterType<IOrderService, OrderService>();
			container.RegisterType<IPaymentProviderService, PaymentProviderService>();
			container.RegisterType<IShippingProviderService, ShippingProviderService>();
			container.RegisterType<IDiscountService, DiscountService>();

			container.RegisterType<IOrderDiscountService, OrderDiscountService>();
			container.RegisterType<IProductDiscountService, ProductDiscountService>();

			container.RegisterType<ICountryRepository, UwebshopApplicationCachedCountriesRepository>();
			container.RegisterType<IVATCountryRepository, UwebshopApplicationCachedVATCountriesRepository>();
			container.RegisterType<IOrderRepository, OrderRepository>();

			container.RegisterType<IUwebshopConfiguration, UwebshopConfiguration>();
		}
	}
}