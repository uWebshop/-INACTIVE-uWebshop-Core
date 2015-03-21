using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Test;

namespace uWebshop.Umbraco.Test.Repositories
{
	[TestFixture]
	public class StorePickerStoreUrlRepositoryTests
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			UseNodes(Enumerable.Empty<UwbsNode>());
		}
		private void UseNodes(IEnumerable<UwbsNode> nodes)
		{
			IOC.CMSEntityRepository.SetupNewMock().Setup(m => m.GetNodesWithStorePicker(It.IsAny<int>())).Returns(nodes);
		}

		[Test]
		public void NoStorePickers_ShouldGiveNoUrls()
		{
			var repo = IOC.StoreUrlRepository.Actual().Resolve();
			var store = new Store();

			var urls = repo.GetUrls(store.Id);
			Assert.AreEqual(0, urls.WithDomain.Count());
			Assert.AreEqual(0, urls.WithoutDomain.Count());
		}

		[Test]
		public void NoStorePickers_NodeWithDomain_ShouldGiveNoUrlsWithoutDomain()
		{
			UseNodes(new[]
				{
					new UwbsNode{ Level = 1, Id = 1, SortOrder = 0, UrlName = "unused",},
				});
			var setupNewMock = IOC.CMSApplication.SetupNewMock();
			setupNewMock.Setup(m => m.GetDomainsForNodeId(1)).Returns(new[] { "www.domain.com/en" });
			var repo = IOC.StoreUrlRepository.Actual().Resolve();
			var store = new Store();

			var urls = repo.GetUrls(store.Id);
			Assert.AreEqual("www.domain.com/en/", urls.WithDomain.FirstOrDefault());
			Assert.False(urls.WithoutDomain.Any());
		}
	}
}
