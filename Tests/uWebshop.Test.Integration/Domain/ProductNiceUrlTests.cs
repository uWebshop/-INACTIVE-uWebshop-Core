using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using Moq;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Integration.Domain
{
	[TestFixture]
	public class ProductNiceUrlTests
	{
		private Product _product;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			// at some point we will need to specify here that we want to use the 'storeUrl localization' module and 'catalog recursive categories url' (oid) module
			_product = new Product {UrlName = "prod", Categories = new ICategory[] {new StubCategory {UrlName = "cat2", ParentCategory = new StubCategory {UrlName = "cat1"}}}};
		}

		[Ignore]
		[Test]
		public void Test()
		{
			//wtf todo: non deterministic results (side effect from another test)
			//var lo = StoreHelper.CurrentLocalization;
			//Assert.NotNull(lo);
			//Assert.NotNull(lo.Store);
			//Assert.NotNull(lo.Store.StoreURL);

			Assert.AreEqual("http://my.uwebshop.com/cat1/cat2/prod", _product.NiceUrl());
		}
	}
}