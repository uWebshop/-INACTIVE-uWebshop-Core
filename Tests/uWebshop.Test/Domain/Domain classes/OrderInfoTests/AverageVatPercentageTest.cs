using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class AverageVatPercentageTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
		}
		[Test]
		public void SingleLineOneProductShouldGiveEqual()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 19);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			Assert.AreEqual(19, orderInfo.AverageOrderVatPercentage);
		}

		[Test]
		public void SingleLineTwoProductsShouldGiveEqual()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 2, 19);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			Assert.AreEqual(19, orderInfo.AverageOrderVatPercentage);
		}

		[Test]
		public void TwoLinesTwoProductsSameVatShouldGiveEqual()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 19);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 19);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo, productInfo2);


			Assert.AreEqual(19, orderInfo.AverageOrderVatPercentage);
		}

		[Test]
		public void TwoLinesTwoProductsDifferentVatShouldGiveAverage()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(100000, 10, 19);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(100000, 10, 6);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo, productInfo2);

			Assert.AreEqual(12.5, orderInfo.AverageOrderVatPercentage);
		}

		[Test]
		public void TwoLinesThreeProductsDifferentVatShouldGiveWeightedAverage()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 2, 19);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 6);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo, productInfo2);

			Assert.AreEqual((2m*19 + 6)/3, orderInfo.AverageOrderVatPercentage);
		}

		[Test]
		public void TwoLinesThreeProductsDifferentVatShouldGiveWeightedAverageWithVatExempt()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 2, 19);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 0);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo, productInfo2);

			Assert.AreEqual((2m*19)/3, orderInfo.AverageOrderVatPercentage);
		}
	}
}