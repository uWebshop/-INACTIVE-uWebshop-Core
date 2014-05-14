using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using uWebshop.Test;
using uWebshop.Test.Repositories;
using uWebshop.Test.Stubs;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Umbraco.Test.Repositories
{
	[TestFixture]
	public class LoadPriceRatioTests
	{
		private UmbracoProductRepository _repository;
		private Product _product;
		private TestPropertyProvider _propertyProvider;
		private ILocalization _localization;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_repository = (UmbracoProductRepository) IOC.ProductRepository.Actual().Resolve();
			_product = new Product {ParentId = 0};
			_propertyProvider = new TestPropertyProvider();
			_localization = new StubLocalization {CurrencyCode = "EUR", StoreAlias = "EN", Currency = new StubCurrency {Ratio = (decimal) 1.1}};
		}

		[Test]
		public void LoadProductPropertyPrice_SpecificCurrency_ShouldNotApplyRatio()
		{
			_propertyProvider.Dictionary.Add("price_EUR", "1000");

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.AreEqual(1000, _product.OriginalPriceInCents);
		}

		[Test]
		public void LoadProductPropertyPrice_OnlyGlobal_ShouldApplyRatio()
		{
			_propertyProvider.Dictionary.Add("price", "1000");

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.AreEqual(1100, _product.OriginalPriceInCents);
		}

		[Test]
		public void LoadProductPropertyPrice_SpecificStoreAndCurrency_ShouldNotApplyRatio()
		{
			_propertyProvider.Dictionary.Add("price_EN_EUR", "1000");

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.AreEqual(1000, _product.OriginalPriceInCents);
		}

		[Test]
		public void LoadProductPropertyPrice_SpecificStore_ShouldApplyRatio()
		{
			_propertyProvider.Dictionary.Add("price_EN", "1000");

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.AreEqual(1100, _product.OriginalPriceInCents);
		}
	}

	public class StubCurrency : ICurrency
	{
		public string ISOCurrencySymbol { get; set; }
		public string CurrencySymbol { get; private set; }
		public decimal Ratio { get; set; }
	}
}