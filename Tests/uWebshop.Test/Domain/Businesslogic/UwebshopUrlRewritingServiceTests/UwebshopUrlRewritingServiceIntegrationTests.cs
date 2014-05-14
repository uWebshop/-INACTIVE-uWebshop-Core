using System;
using Moq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Domain.Businesslogic.UwebshopUrlRewritingServiceTests
{
	[TestFixture]
	public class UwebshopUrlRewritingServiceIntegrationTests
	{
		private Mock<ICatalogUrlResolvingService> _catalogUrlResolvingServiceMock;
		private Mock<IStoreFromUrlDeterminationService> _mock;
		private MockHttpContextWrapper _wrapper;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_mock = new Mock<IStoreFromUrlDeterminationService>();
			IOC.Config<IStoreFromUrlDeterminationService>().Use(_mock.Object);
		}

		[TestCase("/", "?safl=1")]
		[TestCase("/", "")]
		[TestCase("/shopnode/", "?safl=1")]
		[TestCase("/someotherpage/category/", "")]
		[TestCase("/someotherpage/category/bla.aspx", "")]
		public void UrlWithoutCategory_ShouldNotRewrite(string storeUrl, string querystring)
		{
			_wrapper = new MockHttpContextWrapper(storeUrl + querystring);
			IOC.HttpContextWrapper.Use(_wrapper);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();
			
			Assert.IsNull(_wrapper.RewritePathCalledValue);
		}

		[TestCase("/", "category/", "?safl=1")]
		[TestCase("/shopnode/", "category/", "?safl=1")]
		public void CategoryUrls(string storeUrl, string categoryUrl, string querystring)
		{
			SetupResolverAndSplitter(storeUrl, categoryUrl, "", querystring);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			Assert.AreEqual(storeUrl + "?resolvedCategoryId=123&category=" + categoryUrl + querystring.Replace('?', '&'), _wrapper.RewritePathCalledValue);
		}

		[TestCase("/", "category/", "product/", "?safl=1")]
		[TestCase("/", "category/", "product/", "")]
		[TestCase("/shopnode/", "category/", "product/", "?safl=1")]
		[TestCase("/shopnode/", "category/", "product/", "")]
		public void ProductUrls(string storeUrl, string categoryUrl, string productUrl, string querystring)
		{
			SetupResolverAndSplitter(storeUrl, categoryUrl, productUrl, querystring);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			Assert.AreEqual(storeUrl + "?resolvedProductId=456&category=" + categoryUrl + "&product=" + productUrl.Replace("/", "") + querystring.Replace('?', '&'), _wrapper.RewritePathCalledValue);
		}

		//[TestCase("/catalog/categories/")]
		[TestCase("")]
		[TestCase("/")]
		[TestCase("any.ico")]
		[TestCase("any.jpg")]
		[TestCase("any.jpeg")]
		[TestCase("any.png")]
		[TestCase("any.js")]
		[TestCase("any.css")]
		public void ExpensiveChecksShouldNotBeCalledFor(string url)
		{
			_wrapper = new MockHttpContextWrapper(url);
			IOC.HttpContextWrapper.Use(_wrapper);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			Assert.AreEqual(0, _wrapper.PathPointsToPhysicalFileTimesCalled);
			DetermineCatalogUrlComponentsShouldNotBeCalled(_mock);
		}

		[Test]
		public void IfIsCMSReservedUrl_PathPointsToPhysicalFileAndDetermineCatalogUrlComponentsShouldNotBeCalled()
		{
			_wrapper = new MockHttpContextWrapper("/someurl/");
			IOC.HttpContextWrapper.Use(_wrapper);
			IOC.Config<ICMSApplication>().SetupNewMock().Setup(m => m.IsReservedPathOrUrl("/someurl/")).Returns(true);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			Assert.AreEqual(0, _wrapper.PathPointsToPhysicalFileTimesCalled);
			DetermineCatalogUrlComponentsShouldNotBeCalled(_mock);
		}

		[Test]
		public void IfPathPointsToPhysicalFile_DetermineCatalogUrlComponentsShouldNotBeCalled()
		{
			_wrapper = new MockHttpContextWrapper("/someurl/") {PathPointsToPhysicalFileValue = true};
			IOC.HttpContextWrapper.Use(_wrapper);

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			DetermineCatalogUrlComponentsShouldNotBeCalled(_mock);
		}

		[Test]
		public void PaymentProvider()
		{
			var cmsApp = IOC.CMSApplication.StubNotInBackend();
			cmsApp.PaymentProviderRepositoryCMSNodeName = "payment-repo";
			cmsApp.PaymentProviderSectionCMSNodeName = "payment-section";
			_wrapper = new MockHttpContextWrapper("/" + cmsApp.GetPaymentProviderRepositoryCMSNodeUrlName() + "/" + cmsApp.GetPaymentProviderSectionCMSNodeUrlName() + "/something.aspx/?qs=1");
			IOC.HttpContextWrapper.Use(_wrapper);
			IOC.Config<IPaymentProviderService>().SetupNewMock().Setup(m => m.GetPaymentProviderWithName("something", It.IsAny<ILocalization>())).Returns(new PaymentProvider {Name = "something"});

			IOC.UrlRewritingService.Actual().Resolve().Rewrite();

			Assert.AreEqual("/?paymentprovider=something&qs=1", _wrapper.RewritePathCalledValue);
		}

		private void DetermineCatalogUrlComponentsShouldNotBeCalled(Mock<IStoreFromUrlDeterminationService> mock)
		{
			mock.Verify(m => m.DetermineStoreAndUrlParts(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void ResolvingProductUrl_ShouldReturnProductAndSetCurrentProduct()
		{
			SetupResolverAndSplitter("/shopnode/", "category/", "product/");

			var actual = IOC.UrlRewritingService.Actual().Resolve().ResolveUwebshopEntityUrl();

			Assert.NotNull(actual);
			Assert.NotNull(UwebshopRequest.Current.Product);
		}

		[Test]
		public void Speed()
		{
			SetupResolverAndSplitter("/shopnode/", "category/", "product/");

			var startTime = DateTime.Now;
			var urlRewritingService = IOC.UrlRewritingService.Actual().Resolve();
			Console.WriteLine((DateTime.Now - startTime).TotalMilliseconds);

			IOC.CatalogUrlResolvingService.Resolve().GetCategoryFromUrlName("");

			startTime = DateTime.Now;
			//for (int i = 0; i < 1000; i++)
			urlRewritingService.ResolveUwebshopEntityUrl();
			Console.WriteLine((DateTime.Now - startTime).TotalMilliseconds/1);

			startTime = DateTime.Now;
			for (int i = 0; i < 1000; i++)
				urlRewritingService.ResolveUwebshopEntityUrl();
			Console.WriteLine((DateTime.Now - startTime).TotalMilliseconds/1000);
		}

		private void SetupResolverAndSplitter(string storeUrl, string categoryUrl, string productUrl, string querystring = "")
		{
			IOC.StoreFromUrlDeterminationService.Actual();
			IOC.StoreUrlService.SetupNewMock().Setup(m => m.GetStoreUrlsWithoutDomain()).Returns(new[] {new StoreUrl {Store = StoreHelper.GetCurrentStore(), Url = storeUrl}});
			var productName = productUrl.Replace("/", "");
			IOC.CatalogUrlResolvingService.Mock(out _catalogUrlResolvingServiceMock); // a fake
			_catalogUrlResolvingServiceMock.Setup(m => m.GetCategoryPathFromUrlName(categoryUrl)).Returns(new []{ new Category { Id = 123 }});
			_catalogUrlResolvingServiceMock.Setup(m => m.GetProductFromUrlName(categoryUrl, productName)).Returns(new Product {Id = 456});
			_wrapper = new MockHttpContextWrapper(storeUrl + categoryUrl + productUrl + querystring);
			IOC.HttpContextWrapper.Use(_wrapper);
		}
	}

	internal class StoreUrl : IStoreUrl
	{
		public Store Store { get; set; }
		public string Url { get; set; }
	}
}
