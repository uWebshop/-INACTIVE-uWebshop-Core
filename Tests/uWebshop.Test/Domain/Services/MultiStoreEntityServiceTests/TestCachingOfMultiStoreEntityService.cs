using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Services.OrderUpdatingsServiceTests;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Domain.Services.MultiStoreEntityServiceTests
{
	[TestFixture]
	public class TestEntityServiceToRepoCoupling
	{
		private Mock<IProductRepository> _productRepositoryMock;
		private IProductService _productService;
		private ILocalization _localization;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.ProductRepository.Mock(out _productRepositoryMock);
			_productService = IOC.ProductService.Actual().Resolve();
			_localization = StubLocalization.Default;
			_productRepositoryMock.Setup(m => m.GetAll(_localization)).Returns(new List<Product>());
		}

		[Test]
		public void GetAll_CallingTwice_ShouldCallRepoTwice()
		{
			_productService.GetAll(_localization);
			_productService.GetAll(_localization);

			_productRepositoryMock.Verify(m => m.GetAll(_localization), Times.Exactly(2));
		}

		[Test]
		public void GetById_CallingTwice_ShouldCallRepoTwice()
		{
			_productService.GetById(1, _localization);
			_productService.GetById(1, _localization);

			_productRepositoryMock.Verify(m => m.GetById(1, _localization), Times.Exactly(2));
		}
	}
}