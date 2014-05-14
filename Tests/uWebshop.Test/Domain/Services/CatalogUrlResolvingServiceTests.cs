using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Services
{
	[TestFixture]
	public class CatalogUrlResolvingServiceTests
	{
		private Category _category;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_category = new Category();
			_category.ParentId = 0;
			_category.Localization = StoreHelper.CurrentLocalization;
			var categoryServiceMock = IOC.CategoryService.SetupNewMock();
			categoryServiceMock.Setup(m => m.GetAll(It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns(new List<Category> {_category});
			categoryServiceMock.Setup(m => m.GetById(It.IsAny<int>(), It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns((Category) null);
			IOC.CMSEntityRepository.SetupNewMock().Setup(m => m.GetByGlobalId(It.IsAny<int>())).Returns(new UwbsNode {NodeTypeAlias = Catalog.CategoryRepositoryNodeAlias});
		}

		[Test]
		public void GetCategoryFromUrlName()
		{
			_category.UrlName = "cat";
			var service = IOC.CatalogUrlResolvingService.Actual().Resolve();

			var actual = service.GetCategoryFromUrlName("cat/");

			Assert.AreEqual(_category, actual);
		}

		[Test]
		public void SpecialeKarakters()
		{
			_category.UrlName = "siège-élévateur";
			var service = IOC.CatalogUrlResolvingService.Actual().Resolve();

			var actual = service.GetCategoryFromUrlName("siège-élévateur/");

			Assert.AreEqual(_category, actual);
		}
	}
}