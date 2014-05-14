using System;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test;

namespace uWebshop.Umbraco.Test.Model.CategoryTests
{
	[TestFixture]
	public class NiceUrlTests
	{
		private Category _category;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			IOC.StoreService.Actual();

			_category = new Category { UrlName = "cat", Id = 1234, ParentId = 0, Localization = StoreHelper.CurrentLocalization };
			Assert.IsTrue(_category.Localization.Equals(StoreHelper.CurrentLocalization));
			var setupNewMock = IOC.CategoryService.SetupNewMock();
			setupNewMock.Setup(m => m.GetById(1234, It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns(_category);
			setupNewMock.Setup(m => m.Localize(_category, It.IsAny<ILocalization>())).Returns(_category);
			uWebshop.Domain.Core.Initialize.InitializeServiceLocators(IOC.CurrentContainer); // hackish, find a better solution
		}

		[Test]
		public void SingleLevelIncludingDomainAndHideTopLevel()
		{
			Console.WriteLine(IOC.StoreService.Resolve().GetCurrentStore().CanonicalStoreURL);

			var actual = _category.NiceUrl();

			Assert.AreEqual("http://my.uwebshop.com/cat", actual);
		}

		[Test]
		public void SingleLevelNoHideTopLevel()
		{
			IOC.StoreService.Resolve().GetCurrentStore().CanonicalStoreURL = "/nl/";

			Assert.NotNull(StoreHelper.GetCurrentStore().CanonicalStoreURL);

			Assert.AreEqual(StoreHelper.GetCurrentStore(), StoreHelper.CurrentLocalization.Store);
			Assert.IsTrue(StoreHelper.GetCurrentStore() == StoreHelper.CurrentLocalization.Store);
			Assert.IsTrue(_category.Localization.StoreAlias == StoreHelper.CurrentLocalization.StoreAlias);
			Assert.AreEqual(_category.Localization, StoreHelper.CurrentLocalization);
			var actual = _category.NiceUrl();

			Assert.AreEqual("/nl/cat", actual);
		}
	}
}