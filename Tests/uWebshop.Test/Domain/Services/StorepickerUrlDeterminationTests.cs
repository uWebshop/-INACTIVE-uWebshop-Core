using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;

namespace uWebshop.Test.Domain.Services
{
	[TestFixture]
	public class StorepickerUrlDeterminationTests
	{
		private Mock<IStoreUrlService> _storeUrlService;
		private IStoreFromUrlDeterminationService _service;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_storeUrlService = IOC.StoreUrlService.SetupNewMock();
			_service = IOC.StoreFromUrlDeterminationService.Actual().Resolve();
			SetUrlsWithDomain(Enumerable.Empty<string>());
			SetUrlsWithoutDomain(Enumerable.Empty<string>());
		}

		[Test]
		public void Test()
		{
			SetUrlsWithoutDomain(new[] { "/store/" });
			
			var actual = _service.DetermineStoreAndUrlParts("", "/store/cat1/cat2/");

			Assert.AreEqual("/store/", actual.StoreUrl);
		}

		[Test]
		public void NoMatchGivesNull()
		{
			SetUrlsWithoutDomain(new[] { "/store/" });

			var actual = _service.DetermineStoreAndUrlParts("", "/anotherurl/store/");

			Assert.Null(actual);
		}

		[Test]
		public void TakesFirstUrlThatMatches()
		{
			SetUrlsWithoutDomain(new[] { "/store/cat1/cat2/", "/store/" });

			var actual = _service.DetermineStoreAndUrlParts("", "/store/cat1/cat2/");

			Assert.AreEqual("/store/cat1/cat2/", actual.StoreUrl);
		}

		[Test]
		public void DomainIsPreferredOverNonDomain()
		{
			SetUrlsWithDomain(new[] { "http://uwebshop.com/store/" });//, "http://uwebshop.com/store/cat1/cat2/" });
			SetUrlsWithoutDomain(new[] { "/store/cat1/cat2/", "/store/" });

			var actual = _service.DetermineStoreAndUrlParts("http://uwebshop.com", "/store/cat1/cat2/");

			Assert.AreEqual("http://uwebshop.com/store/", actual.StoreUrl);
		}

		private void SetUrlsWithDomain(IEnumerable<string> urls)
		{
			_storeUrlService.Setup(m => m.GetStoreUrlsWithDomain()).Returns(urls.Select(u => new StoreUrl(u)));
		}
		private void SetUrlsWithoutDomain(IEnumerable<string> urls)
		{
			_storeUrlService.Setup(m => m.GetStoreUrlsWithoutDomain()).Returns(urls.Select(u => new StoreUrl(u)));
		}

		private class StoreUrl : IStoreUrl
		{
			public StoreUrl(string url)
			{
				Url = url;
			}

			public Store Store { get; private set; }
			public string Url { get; private set; }
		}

	}
}
