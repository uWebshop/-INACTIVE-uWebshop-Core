using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Test.Stubs;
using uWebshop.Umbraco.Test;

namespace uWebshop.Test.Services.StoreServiceTests
{
	[Ignore] // todo: urgent! failing because of refactoring, fix the tests
	[TestFixture]
	public class LoadStoreUrlTests
	{
		private const int StoreId = 7351;
		private IStoreService _storeService;
		private Store _store;
		private Mock<ICMSApplication> _cmsApplicationMock;
		private Mock<ICMSEntityRepository> _cmsEntityRepositoryMock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.CMSApplication.Mock(out _cmsApplicationMock);
			IOC.StoreUrlService.UseType<UmbracoStorePickerStoreUrlService>();
			IOC.StoreUrlRepository.Actual();
			_store = new Store {Id = StoreId};
		}

		[Test]
		public void LoadStoreUrl_StorePickerNotFound_SetsRoot()
		{
			IOC.CMSEntityRepository.Mock();
			//_storeService = IOC.StoreService.Actual().Resolve();

			var url = IOC.StoreUrlService.Resolve().GetCanonicalUrlForStore(_store);

			Assert.AreEqual("/", url);
		}

		[Test]
		public void LoadStoreUrl_OnRootNodeWithHideTopLevelAndDomainNotFound_SetsRoot()
		{
			IOC.CMSEntityRepository.Mock(out _cmsEntityRepositoryMock);
			_cmsEntityRepositoryMock.Setup(m => m.GetNodeWithStorePicker(StoreId)).Returns(new UwbsNode {Level = 1});
			_cmsApplicationMock.Setup(m => m.HideTopLevelNodeFromPath).Returns(true);
			_cmsApplicationMock.Setup(m => m.GetDomainForNodeId(It.IsAny<int>())).Returns(() => null);
			_storeService = IOC.StoreService.Actual().Resolve();

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnRootNodeWithHideTopLevel_SetsDomain()
		{
			ConfigureNodeChainWithHideTopLevelNode("http://mijndomain.nl/", "ignored");

			_storeService.LoadStoreUrl(_store);
			var url = IOC.StoreUrlService.Resolve().GetCanonicalUrlForStore(StoreHelper.GetCurrentStore());

			Assert.AreEqual("http://mijndomain.nl/", url);
			Assert.AreEqual("http://mijndomain.nl/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnRootNodeWithHideTopLevel_SetsDomainSlashAdded()
		{
			ConfigureNodeChainWithHideTopLevelNode("http://mijndomain.nl", "ignored");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("http://mijndomain.nl/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnLevel2Node_SetsBothUrls()
		{
			ConfigureNodeChainWithUrls("part1", "part2");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("/part1/part2/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnLevel3Node_SetsThreeUrls()
		{
			ConfigureNodeChainWithUrls("part1", "part2", "part3");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("/part1/part2/part3/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnLevel2NodeWithHideTopLevelShortDomainWithSlash_SetsDomainPlusUrl()
		{
			ConfigureNodeChainWithHideTopLevelNode("http://mijndomain.nl/", "ignored", "part2");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("http://mijndomain.nl/part2/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnLevel2NodeWithHideTopLevelFullDomainName_SetsDomainPlusUrl()
		{
			ConfigureNodeChainWithHideTopLevelNode("http://mijndomain.nl/", "ignored", "part2");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("http://mijndomain.nl/part2/", _store.StoreURL);
		}

		[Test]
		public void LoadStoreUrl_OnRootNode_SetsUrlName()
		{
			ConfigureNodeChainWithUrls("store");

			_storeService.LoadStoreUrl(_store);

			Assert.AreEqual("/store/", _store.StoreURL);
		}

		class FakeCategoryService : ICategoryService
		{
			public Category GetById(int id, ILocalization localization, bool includeDisabled = false)
			{
				throw new System.NotImplementedException();
			}

			public IEnumerable<Category> GetAll(ILocalization localization, bool includeDisabled = false)
			{
				throw new System.NotImplementedException();
			}

			public void ReloadEntityWithId(int id)
			{
				throw new System.NotImplementedException();
			}

			public void UnloadEntityWithId(int id)
			{
				throw new System.NotImplementedException();
			}

			public void FullResetCache()
			{
				throw new System.NotImplementedException();
			}

			public List<Category> GetAllRootCategories(ILocalization localization)
			{
				throw new System.NotImplementedException();
			}

			public ICategory Localize(ICategory category, ILocalization localization)
			{
				return category;
			}
		}

		[Test]
		public void CategoryNiceUrl_NoStorePickerSet_ReturnsSlashCategoryUrl()
		{
			IOC.IntegrationTest();
			IOC.CategoryService.UseType<FakeCategoryService>();
			IOC.StoreService.Actual();

			IOC.CMSEntityRepository.Mock(out _cmsEntityRepositoryMock);
			_cmsEntityRepositoryMock.Setup(m => m.GetByGlobalId(1)).Returns(new UwbsNode {NodeTypeAlias = "uwbsCategory"});
			_cmsEntityRepositoryMock.Setup(m => m.GetObjectsByAliasUncached<Store>(Store.NodeAlias, It.IsAny<ILocalization>(), It.IsAny<int>())).Returns(new List<Store> { StoreHelper.GetCurrentStore() });
			Mock<ICategoryService> catalogServiceMock;
			IOC.CategoryService.Mock(out catalogServiceMock);
			var category = new Category {UrlName = "shoes", ParentId = -1, SubCategories = Enumerable.Empty<Category>()};
			catalogServiceMock.Setup(m => m.GetById(1, It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns(category);
			catalogServiceMock.Setup(m => m.Localize(category, It.IsAny<ILocalization>())).Returns(category); // bah bah

			uWebshop.Domain.Core.Initialize.InitializeServiceLocators(IOC.CurrentContainer); // todo hackish, find a better solution

			Assert.AreEqual(IOC.StoreService.Resolve(), StoreHelper.StoreService);

			var actual = StoreHelper.GetNiceUrl(1);

			Assert.AreEqual("/shoes", actual);
		}


		private void ConfigureNodeChainWithUrls(params string[] urls)
		{
			var nodes = UwbsNodes(urls);

			_cmsEntityRepositoryMock = IOC.CMSEntityRepository.SetupFake(nodes.ToArray());
			_storeService = IOC.StoreService.Actual().Resolve();

			_cmsEntityRepositoryMock.Setup(m => m.GetNodeWithStorePicker(StoreId)).Returns(nodes.Last());
			_cmsApplicationMock.Setup(m => m.HideTopLevelNodeFromPath).Returns(false);
		}

		private static List<UwbsNode> UwbsNodes(string[] urls)
		{
			var nodes = urls.Select(url => new UwbsNode {UrlName = url}).ToList();
			int parentId = -1;
			int level = 1;
			foreach (var node in nodes)
			{
				node.Id = level;
				node.Level = level;
				node.ParentId = parentId;
				parentId = level;
				level++;
			}
			return nodes;
		}

		private void ConfigureNodeChainWithHideTopLevelNode(string domainForTopLevelNode, params string[] urls)
		{
			var nodes = UwbsNodes(urls);

			_cmsEntityRepositoryMock = IOC.CMSEntityRepository.SetupFake(nodes.ToArray());
			IOC.StoreUrlService.UseType<UmbracoStorePickerStoreUrlService>();
			IOC.StoreUrlRepository.Actual();
			_storeService = IOC.StoreService.Actual().Resolve();

			_cmsEntityRepositoryMock.Setup(m => m.GetNodeWithStorePicker(StoreId)).Returns(nodes.Last());
			_cmsApplicationMock.Setup(m => m.HideTopLevelNodeFromPath).Returns(true);
			_cmsApplicationMock.Setup(m => m.GetDomainForNodeId(nodes.First().Id)).Returns(domainForTopLevelNode);
		}
	}
}
