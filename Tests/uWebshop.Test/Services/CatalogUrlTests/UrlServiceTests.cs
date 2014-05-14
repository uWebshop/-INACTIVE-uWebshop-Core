using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Services.CatalogUrlTests
{
	[TestFixture]
	public class UrlServiceTests
	{
		private IUrlService _urlService;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			_urlService = IOC.UrlService.Resolve();
			//IOC.ProductRepository
		}

		[Ignore]
		[Test]
		public void Blasf()
		{
			var cat1 = new StubCategory { UrlName = "cat1" };
			var cat2 = new StubCategory { UrlName = "cat2", ParentCategories = new List<ICategory>{cat1}};
			cat1.SubCategories = new List<ICategory>{cat2};
			var product = new Product { UrlName = "prod", Categories = new []{cat2}};

			UwebshopRequest.Current.CategoryPath = new[] {cat1, cat2};
			UwebshopRequest.Current.Category = cat2;
			UwebshopRequest.Current.Localization = StoreHelper.CurrentLocalization;

			Console.WriteLine(_urlService.ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(product, StoreHelper.CurrentLocalization));
			Console.WriteLine(product.NiceUrl());
		}
	}
}
