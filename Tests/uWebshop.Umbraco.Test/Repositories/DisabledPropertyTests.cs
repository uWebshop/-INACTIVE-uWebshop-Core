using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using uWebshop.Test;
using uWebshop.Test.Repositories;
using uWebshop.Test.Stubs;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Umbraco.Test.Repositories
{
	[TestFixture]
	public class DisabledPropertyTests
	{
		private UmbracoProductRepository _repository;
		private Product _product;
		private TestPropertyProvider _propertyProvider;
		private ILocalization _localization;
		private string StoreAlias = "TestStoreAlias";

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_repository = (UmbracoProductRepository) IOC.ProductRepository.Actual().Resolve();
			_product = new Product {ParentId = 0};
			_propertyProvider = new TestPropertyProvider();
			//_localization = new StubLocalization {StoreAlias = StoreAlias};
			var store = StoreHelper.GetCurrentStore();
			StoreAlias = store.Alias;
			_localization = Localization.CreateLocalization(store); // todo: not neccessary?
		}

		[Test]
		public void LoadProductProperties_WithoutSetting_ShouldDefaultToDisabledFalse()
		{
			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.False(_product.Disabled);
		}

		[Test]
		public void LoadProductProperties_WithGlobalDisabledTrue_ShouldSetDisabledTrue()
		{
			SetGlobalDisablePropertyTrue();

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.True(_product.Disabled);
		}

		[Test]
		public void LoadProductProperties_WithGlobalDisabledFalseStoreTrue_ShouldSetDisabledTrue()
		{
			SetGlobalDisablePropertyFalse();
			SetStoreDisablePropertyTrue();

			_repository.LoadDataFromPropertiesDictionary(_product, _propertyProvider, _localization);

			Assert.True(_product.Disabled);
		}

		private void SetGlobalDisablePropertyTrue()
		{
			_propertyProvider.Dictionary.Add("disable", "1");
		}

		private void SetGlobalDisablePropertyFalse()
		{
			_propertyProvider.Dictionary.Add("disable", "0");
		}

		private void SetStoreDisablePropertyTrue()
		{
			_propertyProvider.Dictionary.Add(StoreHelper.CreateMultiStorePropertyAlias("disable", StoreAlias), "1");
		}
	}
}