using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Common.ServiceInterfaces;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Domain.Businesslogic.UwebshopUrlsRewritingServiceTests
{
	[TestFixture(Category = "legacy")]
	public class RedirectPermanentOldCatalogUrlsTests
	{
		private IUrlRewritingService _urlRewritingService;
		private MockHttpContextWrapper _wrapper;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_wrapper = new MockHttpContextWrapper();
			IOC.HttpContextWrapper.Use(_wrapper);
			IOC.UwebshopConfiguration.Use(new TestUwebshopConfiguration { PermanentRedirectOldCatalogUrls = true, LegacyCategoryUrlIdentifier = "categoryidentifier", LegacyProductUrlIdentifier = "productidentifier" });
			_urlRewritingService = IOC.UrlRewritingService.Actual().Resolve();
		}

		[TestCase("")]
		[TestCase("/")]
		[TestCase("/product/")]
		[TestCase("/uwbsCategoryUrlIdentifierSomething/")]
		public void RedirectPermanentOldCatalogUrls_ShouldNotRedirect(string url)
		{
			_wrapper.Configure(url);

			_urlRewritingService.RedirectPermanentOldCatalogUrls();

			Assert.IsNull(_wrapper.RedirectPermanentCalledValue);
		}

		[Test]
		public void UrlWithOldCategoryUrlIdentifierShouldBeRedirected()
		{
			_wrapper.Configure("/categoryidentifier/categoryname/productidentifier/productname/");

			_urlRewritingService.RedirectPermanentOldCatalogUrls();

			Assert.AreEqual("/categoryname/productname/", _wrapper.RedirectPermanentCalledValue);
		}
	}

	public class TestUwebshopConfiguration : IUwebshopConfiguration
	{
		public bool PermanentRedirectOldCatalogUrls { get; set; }
		public string LegacyCategoryUrlIdentifier { get; set; }
		public string LegacyProductUrlIdentifier { get; set; }
		public string CategoryUrl { get; set; }
		public string ProductUrl { get; set; }
		public string ExamineSearcher { get; set; }
		public string ExamineIndexer { get; set; }
		public bool ShareBasketBetweenStores { get; set; }
		public bool DisableDateFolders { get; set; }
		public int OrdersCacheTimeoutMilliseconds { get; set; }
		public string ConnectionString { get; set; }
		public bool UseDeliveryDateAsConfirmDateForScheduledOrders { get; set; }
	}
}