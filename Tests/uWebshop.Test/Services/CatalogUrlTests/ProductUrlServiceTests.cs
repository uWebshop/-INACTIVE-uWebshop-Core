using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Services.CatalogUrlTests
{
	[TestFixture]
	public class ProductUrlServiceTests
	{
		private IProductUrlService _productUrlService;
		private Product _product;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_product = new Product {UrlName = "prod", Categories = new API.ICategory[] {new StubCategory()}};
			IOC.CategoryCatalogUrlService.SetupNewMock().Setup(m => m.GetCanonicalUrl(It.IsAny<ICategory>())).Returns("[catname]");
			_productUrlService = IOC.ProductUrlService.Actual().Resolve();
		}

		//[Test]
		//public void TestGetUrl()
		//{
		//	var actual = _productUrlService.GetUrl(_product);

		//	Assert.AreEqual("[catname]/prod", actual);
		//}

		[Test]
		public void TestGetCanonicalUrl()
		{
			var actual = _productUrlService.GetCanonicalUrl(_product);

			Assert.AreEqual("[catname]/prod", actual);
		}

		[Test]
		public void asgadg()
		{
			//API.Catalog.NiceUrl()
		}
	}
}