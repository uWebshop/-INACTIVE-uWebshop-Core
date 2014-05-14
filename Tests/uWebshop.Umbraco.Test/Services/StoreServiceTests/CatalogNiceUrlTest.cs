using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Test;
using uWebshop.Test.Stubs;

namespace uWebshop.Umbraco.Test.Services.StoreServiceTests
{
	[TestFixture]
	public class CatalogNiceUrlTest
	{
		private IStoreService _storeService;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_storeService = IOC.StoreService.Actual().Resolve();

			//_product = new Product { UrlName = "prod", Categories = new API.ICategory[] { new StubCategory() } };
			//IOC.CategoryCatalogUrlService.SetupNewMock().Setup(m => m.GetCanonicalUrl(It.IsAny<ICategory>())).Returns("[catname]");
			//_productUrlService = IOC.ProductUrlService.Actual().Resolve();
		}

		[Test]
		public void ag()
		{
			_storeService.GetNiceUrl(1, 2, _storeService.GetCurrentLocalization());

		}
	}
}
