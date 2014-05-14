using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using Moq;
using uWebshop.Domain.Services;
using uWebshop.Test.Repositories;

namespace uWebshop.Test.Domain.Domain_classes.ProductTests
{
	[TestFixture]
	public class NiceUrlTests
	{
		private Product _product;
		private Mock<IUrlService> _urlService;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			IOC.UrlService.Mock(out _urlService);
			_product = TestProductService.Product1;
		}

		[Test]
		public void CallingNiceUrl_WithoutArguments_ShouldCallStoreServiceWithFalseCanonical()
		{
			_product.NiceUrl();

			_urlService.Verify(m => m.ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(_product, _product.Localization));
		}

		[Test]
		public void CallingNiceUrl_WithFalseCanonical_ShouldCallStoreServiceWithFalseCanonical()
		{
			_product.NiceUrl(false);

			_urlService.Verify(m => m.ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(_product, _product.Localization));
		}

		[Test]
		public void CallingNiceUrl_WithTrueCanonical_ShouldCallStoreServiceWithTrueCanonical()
		{
			_product.NiceUrl(true);

			_urlService.Verify(m => m.ProductCanonical(_product, _product.Localization));
		}

		[Test]
		public void CallingUrl_ShouldCallStoreServiceWithTrueCanonical()
		{
			var url = _product.Url;

			_urlService.Verify(m => m.ProductCanonical(_product, _product.Localization));
		}
	}
}