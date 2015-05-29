using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using VATChecker;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Core;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Test.Mocks;
using uWebshop.Test.Repositories;
using uWebshop.Test.Stubs;
using uWebshop.Umbraco.Interfaces;
using uWebshop.Umbraco.Repositories;
using uWebshop.Umbraco.Services;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test
{
	internal static class IOC
	{
		internal static IoCContainer CurrentContainer;

		public static IOCBuilder<IVatCalculationStrategy> VatCalculationStrategy { get { return Config<IVatCalculationStrategy>(); } }
		

		public static IOCBuilder<ILoggingService> Logger
		{
			get { return Config<ILoggingService>(); }
		}

		public static IOCBuilder<ICMSApplication> CMSApplication
		{
			get { return Config<ICMSApplication>(); }
		}

		public static IOCBuilder<IUwebshopConfiguration> UwebshopConfiguration
		{
			get { return Config<IUwebshopConfiguration>(); }
		}

		public static IOCBuilder<IDiscountCalculationService> DiscountCalculationService
		{
			get { return Config<IDiscountCalculationService>(); }
		}

		public static IOCBuilder<IUrlRewritingService> UrlRewritingService
		{
			get { return Config<IUrlRewritingService>(); }
		}

		public static IOCBuilder<ICatalogUrlResolvingService> CatalogUrlResolvingService
		{
			get { return Config<ICatalogUrlResolvingService>(); }
		}

		public static IOCBuilder<IApplicationCacheManagingService> ApplicationCacheManagingService
		{
			get { return Config<IApplicationCacheManagingService>(); }
		}

		public static IOCBuilder<IOrderUpdatingService> OrderUpdatingService
		{
			get { return Config<IOrderUpdatingService>(); }
		}

		public static IOCBuilder<IVATCheckService> VATCheckService
		{
			get { return Config<IVATCheckService>(); }
		}

		public static IOCBuilder<IProductUrlService> ProductUrlService
		{
			get { return Config<IProductUrlService>(); }
		}

		public static IOCBuilder<ICategoryCatalogUrlService> CategoryCatalogUrlService
		{
			get { return Config<ICategoryCatalogUrlService>(); }
		}

		public static IOCBuilder<IUrlFormatService> UrlFormatService
		{
			get { return Config<IUrlFormatService>(); }
		}

		public static IOCBuilder<IStoreUrlRepository> StoreUrlRepository
		{
			get { return Config<IStoreUrlRepository>(); }
		}
		
		public static IOCBuilder<IUrlLocalizationService> UrlLocalizationService
		{
			get { return Config<IUrlLocalizationService>(); }
		}

		public static IOCBuilder<IUrlService> UrlService
		{
			get { return Config<IUrlService>(); }
		}

		public static IOCBuilder<ISettingsService> SettingsService
		{
			get { return Config<ISettingsService>(); }
		}

		public static IOCBuilder<IUwebshopRequestService> UwebshopRequestService
		{
			get { return Config<IUwebshopRequestService>(); }
		}

		public static IOCBuilder<IStoreService> StoreService
		{
			get { return Config<IStoreService>(); }
		}

		public static IOCBuilder<IOrderService> OrderService
		{
			get { return Config<IOrderService>(); }
		}

		public static IOCBuilder<ICategoryService> CategoryService
		{
			get { return Config<ICategoryService>(); }
		}

		public static IOCBuilder<ICategoryRepository> CategoryRepository
		{
			get { return Config<ICategoryRepository>(); }
		}

		public static IOCBuilder<IProductVariantRepository> ProductVariantRepository
		{
			get { return Config<IProductVariantRepository>(); }
		}

		public static IOCBuilder<IProductService> ProductService
		{
			get { return Config<IProductService>(); }
		}

		public static IOCBuilder<IProductVariantService> ProductVariantService
		{
			get { return Config<IProductVariantService>(); }
		}

		public static IOCBuilder<ICMSDocumentTypeService> CMSDocumentTypeService
		{
			get { return Config<ICMSDocumentTypeService>(); }
		}

		public static IOCBuilder<IStockService> StockService
		{
			get { return Config<IStockService>(); }
		}

		public static IOCBuilder<ICouponCodeService> CouponCodeService
		{
			get { return Config<ICouponCodeService>(); }
		}

		public static IOCBuilder<IOrderDiscountService> OrderDiscountService
		{
			get { return Config<IOrderDiscountService>(); }
		}

		public static IOCBuilder<IProductDiscountService> ProductDiscountService
		{
			get { return Config<IProductDiscountService>(); }
		}

		public static IOCBuilder<IStoreRepository> StoreRepository
		{
			get { return Config<IStoreRepository>(); }
		}

		public static IOCBuilder<ICountryRepository> CountryRepository
		{
			get { return Config<ICountryRepository>(); }
		}

		public static IOCBuilder<IVATCountryRepository> VATCountryRepository
		{
			get { return Config<IVATCountryRepository>(); }
		}

		public static IOCBuilder<ICMSEntityRepository> CMSEntityRepository
		{
			get { return Config<ICMSEntityRepository>(); }
		}

		public static IOCBuilder<IOrderDiscountRepository> OrderDiscountRepository
		{
			get { return Config<IOrderDiscountRepository>(); }
		}

		public static IOCBuilder<IProductDiscountRepository> ProductDiscountRepository
		{
			get { return Config<IProductDiscountRepository>(); }
		}

		public static IOCBuilder<IOrderRepository> OrderRepository
		{
			get { return Config<IOrderRepository>(); }
		}

		public static IOCBuilder<IProductRepository> ProductRepository
		{
			get { return Config<IProductRepository>(); }
		}

		public static IOCBuilder<IStoreFromUrlDeterminationService> StoreFromUrlDeterminationService { get { return Config<IStoreFromUrlDeterminationService>(); } }
		public static IOCBuilder<IStoreUrlService> StoreUrlService { get { return Config<IStoreUrlService>(); } }
		public static IOCBuilder<IHttpContextWrapper> HttpContextWrapper
		{
			get { return Config<IHttpContextWrapper>(); }
		}

		public static IOCBuilder<IUmbracoDocumentTypeInstaller> UmbracoDocumentTypeInstaller
		{
			get { return Config<IUmbracoDocumentTypeInstaller>(); }
		}
		
		// generated
		public static IOCBuilder<IDiscountService> DiscountService
		{
			get { return Config<IDiscountService>(); }
		}

		public static IOCBuilder<IPaymentProviderRepository> PaymentProviderRepository
		{
			get { return Config<IPaymentProviderRepository>(); }
		}

		public static IOCBuilder<IPaymentProviderService> PaymentProviderService
		{
			get { return Config<IPaymentProviderService>(); }
		}

		public static IOCBuilder<IRebuildIndicesService> RebuildIndicesService
		{
			get { return Config<IRebuildIndicesService>(); }
		}

		public static IOCBuilder<IShippingProviderUpdateService> ShippingProviderUpdateService
		{
			get { return Config<IShippingProviderUpdateService>(); }
		}

		public static IOCBuilder<IOrderNumberService> OrderNumberService
		{
			get { return Config<IOrderNumberService>(); }
		}

		public static IOCBuilder<IApplicationCacheService> ApplicationCacheService
		{
			get { return Config<IApplicationCacheService>(); }
		}

		public static IOCBuilder<IProductVariantGroupRepository> ProductVariantGroupRepository
		{
			get { return Config<IProductVariantGroupRepository>(); }
		}
		public static IOCBuilder<IProductVariantGroupService> ProductVariantGroupService
		{
			get { return Config<IProductVariantGroupService>(); }
		}

		public static IOCBuilder<IAuthenticationProvider> AuthenticationProvider
		{
			get { return Config<IAuthenticationProvider>(); }
		}

		public static IOCBuilder<ICMSContentService> CMSContentService
		{
			get { return Config<ICMSContentService>(); }
		}

		public static IOCBuilder<T> Config<T>() where T : class
		{
			return new IOCBuilder<T>();
		}

		public static void UnitTest()
		{
			CurrentContainer = new IoCContainer();
			IO.Container = CurrentContainer;
			SettingsService.InclVat();
			VatCalculationStrategy.OverParts();

			Logger.Mock();

			UwebshopConfiguration.UseType<UwebshopConfiguration>(); // todo no UseType? (slow)
			UwebshopRequestService.Use(MockConstructors.CreateMockUwebshopRequestService());

			OrderDiscountRepository.SetupFake();
			CategoryService.Use(MockConstructors.CreateMockEntityService<ICategoryService, Category>());
			ProductVariantService.Use(MockConstructors.CreateMockEntityService<IProductVariantService, ProductVariant>());
			CouponCodeService.Mock();

			StoreRepository.Use(new TestStoreRepository());
			CMSEntityRepository.Use(new TestCMSEntityRepository());
			CountryRepository.Use(new TestCountryRepository());
			VATCountryRepository.Use(new TestVATCountryRepository());
			OrderRepository.Mock();
			ProductDiscountRepository.Mock();
			ProductRepository.Mock();
			ProductVariantGroupRepository.Mock();
			CategoryRepository.Mock();
			ProductVariantRepository.Mock();

			CMSContentService.SetupNewMock().Setup(m => m.GetReadonlyById(It.IsAny<int>())).Returns(new Mock<IUwebshopReadonlyContent>().Object);
			CMSApplication.Use(new StubCMSApplicationNotInBackend());
			HttpContextWrapper.Mock();

			DiscountService.UseType<FakeDiscountService>();

			StoreService.Use(new TestStoreService());
			OrderService.Use(new TestOrderService());
			ProductService.Use(new TestProductService());
			CMSDocumentTypeService.Use(new StubCMSDocumentTypeService());

			OrderUpdatingService.Mock();
			OrderNumberService.Mock();
			CatalogUrlResolvingService.Mock();
			PaymentProviderService.Mock();
			StoreFromUrlDeterminationService.Mock();
			StockService.Mock();
			ApplicationCacheService.Mock();

			UwebshopRequestService.Use(new StubUwebshopRequestService());

			CurrentContainer.SetDefaultServiceFactory(new MockServiceFactory());

			ModuleFunctionality.Register(CurrentContainer);
			CurrentContainer.RegisterType<IContentTypeAliassesXmlService, StubContentTypeAliassesXmlService>();
			CurrentContainer.RegisterType<IProductVariantGroupService, ProductVariantGroupService>();
			
			InitializeServiceLocators();
		}


		private static void InitializeServiceLocators()
		{
			// todo: this won't work in all situations!
			InitNodeAliasses.Initialize(CurrentContainer.Resolve<IContentTypeAliassesXmlService>().Get());
			Initialize.InitializeServiceLocators(CurrentContainer);

			//uWebshop.Domain.UwebshopConfiguration.Current = IO.Container.Resolve<IUwebshopConfiguration>();
			//UwebshopRequest.Service = IO.Container.Resolve<IUwebshopRequestService>();
			//StoreHelper.StoreService = null;
		}

		internal static void IntegrationTest()
		{
			UnitTest();

			//CatalogUrlResolvingService.Use(new CatalogUrlResolvingService()); //not yet (repo)
			DiscountCalculationService.Actual();
			DiscountService.Actual();
			ProductDiscountService.Actual();
			ProductDiscountRepository.SetupNewMock().Setup(m => m.GetAll(It.IsAny<ILocalization>())).Returns(new List<DiscountProduct>());
			OrderUpdatingService.Actual();
			OrderService.Actual();
			UrlRewritingService.Actual();
			VATCheckService.Use(new ViesVatCheckService());
			//StoreService.Actual(); // todo!
			//CountriesRepository.Use(new UwebshopApplicationCachedCountriesRepository()); // werken nog niet ivm HtppContext.Current
			//VATCountriesRepository.Use(new UwebshopApplicationCachedVATCountriesRepository());
			
			//ProductService.Actual(); // maybe move Localize somewhere else :'(

			StoreFromUrlDeterminationService.Actual();


			OrderDiscountService.Actual();

			UrlFormatService.Actual();
			ProductUrlService.Actual();
			CategoryCatalogUrlService.Actual();
			UrlLocalizationService.Actual();
			UrlService.Actual();

			InitializeServiceLocators();
		}
	}

	internal class MockServiceFactory : IServiceFactory
	{
		public T Build<T>() where T : class
		{
			var mock = new Mock<T>();
			return mock.Object;
		}
	}

	public class IOCBuilder<T> where T : class
	{
		public IOCBuilder<T> Use(T instance)
		{
			IOC.CurrentContainer.RegisterInstance<T, T>(instance);
			return this;
		}

		public IOCBuilder<T> UseType<T1>() where T1 : T
		{
			IOC.CurrentContainer.RegisterType<T, T1>();
			return this;
		}

		public IOCBuilder<T> Use(Func<T> factory)
		{
			return Use(factory());
		}

		public IOCBuilder<T> Mock(out Mock<T> mock)
		{
			mock = SetupNewMock();
			return this;
		}

		public IOCBuilder<T> Mock()
		{
			SetupNewMock();
			return this;
		}

		public Mock<T> SetupNewMock()
		{
			var mock = new Mock<T>();
			Use(mock.Object);
			return mock;
		}

		public T Resolve()
		{
			return IOC.CurrentContainer.Resolve<T>();
		}
	}

	[TestFixture]
	public class Generate
	{
		//[Ignore]
		[Test]
		public void GenerateIOCBoilerplateCode()
		{
			//var type = typeof(string);//IApplicationDependencies);
			//foreach (var propertyInfo in type.GetProperties())
			//	Console.WriteLine("		public " + propertyInfo.PropertyType.Name + " " + propertyInfo.Name + " { get { return " + propertyInfo.Name + "Factory(); } } public Func<" + propertyInfo.PropertyType.Name + "> " + propertyInfo.Name + "Factory { get; set; }");
			//Console.WriteLine();
			//foreach (var propertyInfo in type.GetProperties())
			//	Console.WriteLine("		public static IOCBuilder<" + propertyInfo.PropertyType.Name + "> " + propertyInfo.Name + " { get { return new IOCBuilder<" + propertyInfo.PropertyType.Name + ">(factory => Dependencies." + propertyInfo.Name + "Factory = factory); } }");

			Console.WriteLine();
			foreach (var iType in Assembly.GetAssembly(typeof (OrderInfo)).GetTypes())
			{
				if (!iType.IsInterface || !iType.Name.StartsWith("I")) continue; // eventueel EndsWith("Service") / repo
				Console.WriteLine("		public static IOCBuilder<" + iType.Name + "> " + iType.Name.Substring(1) + " { get { return Config<" + iType.Name + ">(); } }");
			}
		}
	}


	internal static class IOCBuilderExtensions
	{
		//public static IOCBuilder<IUwebshopRequestService> Use(this IOCBuilder<IUwebshopRequestService> iocBuilder, IUwebshopRequestService service)
		//{ werkt niet, andere is sterker
		//	IO.Container.RegisterInstance<IUwebshopRequestService>(service);
		//	UwebshopRequest.Service = service;
		//	return iocBuilder;
		//}

		// specific factory methods
		public static IOCBuilder<IUrlRewritingService> Actual(this IOCBuilder<IUrlRewritingService> iocBuilder)
		{
			iocBuilder.UseType<UrlRewritingService>();
			return iocBuilder;
		}

		public static IOCBuilder<IDiscountCalculationService> Actual(this IOCBuilder<IDiscountCalculationService> iocBuilder)
		{
			iocBuilder.UseType<DiscountCalculationService>();
			return iocBuilder;
		}

		public static IOCBuilder<IStoreFromUrlDeterminationService> Actual(this IOCBuilder<IStoreFromUrlDeterminationService> iocBuilder) { iocBuilder.UseType<StoreFromUrlDeterminationService>(); return iocBuilder; }
		public static IOCBuilder<IOrderService> Actual(this IOCBuilder<IOrderService> iocBuilder)
		{
			iocBuilder.UseType<OrderService>();
			return iocBuilder;
		}

		public static IOCBuilder<IOrderUpdatingService> Actual(this IOCBuilder<IOrderUpdatingService> iocBuilder)
		{
			iocBuilder.UseType<OrderUpdatingService>();
			return iocBuilder;
		}

		public static IOCBuilder<IProductService> Actual(this IOCBuilder<IProductService> iocBuilder)
		{
			iocBuilder.UseType<ProductService>();
			return iocBuilder;
		}

		public static IOCBuilder<IStoreUrlRepository> Actual(this IOCBuilder<IStoreUrlRepository> iocBuilder)
		{
			iocBuilder.UseType<UmbracoStorePickerStoreUrlRepository>();
			return iocBuilder;
		}
		
		public static IOCBuilder<IPaymentProviderService> Actual(this IOCBuilder<IPaymentProviderService> iocBuilder)
		{
			iocBuilder.UseType<PaymentProviderService>();
			return iocBuilder;
		}

		public static IOCBuilder<ICatalogUrlResolvingService> Actual(this IOCBuilder<ICatalogUrlResolvingService> iocBuilder)
		{
			iocBuilder.UseType<CatalogUrlResolvingService>();
			return iocBuilder;
		}

		public static IOCBuilder<IDiscountService> Actual(this IOCBuilder<IDiscountService> iocBuilder)
		{
			iocBuilder.UseType<DiscountService>();
			return iocBuilder;
		}

		public static IOCBuilder<IOrderDiscountService> Actual(this IOCBuilder<IOrderDiscountService> iocBuilder)
		{
			iocBuilder.UseType<OrderDiscountService>();
			return iocBuilder;
		}

		public static IOCBuilder<IProductDiscountService> Actual(this IOCBuilder<IProductDiscountService> iocBuilder)
		{
			iocBuilder.UseType<ProductDiscountService>();
			return iocBuilder;
		}

		public static IOCBuilder<IProductUrlService> Actual(this IOCBuilder<IProductUrlService> iocBuilder)
		{
			iocBuilder.UseType<CatalogCategoryPlusProductUrlService>();
			return iocBuilder;
		}

		public static IOCBuilder<IUrlService> Actual(this IOCBuilder<IUrlService> iocBuilder)
		{
			iocBuilder.UseType<UrlService>();
			return iocBuilder;
		}

		public static IOCBuilder<ICategoryCatalogUrlService> Actual(this IOCBuilder<ICategoryCatalogUrlService> iocBuilder)
		{
			iocBuilder.UseType<CategoryCatalogUrlService>();
			return iocBuilder;
		}

		public static IOCBuilder<IUrlFormatService> Actual(this IOCBuilder<IUrlFormatService> iocBuilder)
		{
			iocBuilder.UseType<UrlFormatService>();
			return iocBuilder;
		}

		public static IOCBuilder<IUrlLocalizationService> Actual(this IOCBuilder<IUrlLocalizationService> iocBuilder)
		{
			iocBuilder.UseType<StoreUrlInFrontBasedOnCurrentNodeUrlLocalizationService>();
			return iocBuilder;
		}

		public static IOCBuilder<IOrderNumberService> Actual(this IOCBuilder<IOrderNumberService> iocBuilder)
		{
			iocBuilder.UseType<OrderNumberService>();
			return iocBuilder;
		}

		public static IOCBuilder<IApplicationCacheManagingService> Actual(this IOCBuilder<IApplicationCacheManagingService> iocBuilder)
		{
			iocBuilder.UseType<UmbracoApplicationCacheManagingService>();
			return iocBuilder;
		}

		public static IOCBuilder<ICategoryService> Actual(this IOCBuilder<ICategoryService> iocBuilder)
		{
			iocBuilder.UseType<CategoryService>();
			return iocBuilder;
		}
		
		// not sure about this
		public static TestCMSEntityRepository GetFake(this IOCBuilder<ICMSEntityRepository> iocBuilder)
		{
			return (TestCMSEntityRepository) iocBuilder.Resolve();
		}

		public static void InclVat(this IOCBuilder<ISettingsService> iocBuilder)
		{
			iocBuilder.Use(StubSettingsService.InclVat());
		}
		public static void ExclVat(this IOCBuilder<ISettingsService> iocBuilder)
		{
			iocBuilder.Use(StubSettingsService.ExclVat());
		}

		public static void OverTotal(this IOCBuilder<IVatCalculationStrategy> iocBuilder) { iocBuilder.UseType<OverTotalVatCalculationStrategy>(); }
		public static void OverParts(this IOCBuilder<IVatCalculationStrategy> iocBuilder) { iocBuilder.UseType<OverSmallestPartsVatCalculationStrategy>(); }
	
		public static StubCMSApplicationNotInBackend StubNotInBackend(this IOCBuilder<ICMSApplication> iocBuilder)
		{
			var stub = new StubCMSApplicationNotInBackend();
			iocBuilder.Use(stub);
			return stub;
		}

		public static void SetupFake(this IOCBuilder<IOrderDiscountRepository> iocBuilder, params IOrderDiscount[] orderDiscounts)
		{
			iocBuilder.Use(new FakeOrderDiscountRepository {Entities = orderDiscounts.ToList()});
		}

		public static Mock<ICMSEntityRepository> SetupFake(this IOCBuilder<ICMSEntityRepository> iocBuilder, params uWebshop.Domain.Helpers.UwbsNode[] uwbsNodes)
		{
			Mock<ICMSEntityRepository> mock = MockConstructors.CreateMockCMSEntityRepository(uwbsNodes);
			iocBuilder.Use(mock.Object);
			return mock;
		}
	}
}