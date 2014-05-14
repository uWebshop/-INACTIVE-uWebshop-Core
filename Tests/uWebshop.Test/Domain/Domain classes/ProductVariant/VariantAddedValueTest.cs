using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;
using Moq;

namespace uWebshop.Test.Domain.Domain_classes.ProductVariantTest
{
	[TestFixture]
	public class VariantAddedValueTest
	{
		private Mock<IOrderInfo> _orderInfoMockIncludingVAT;
		private Mock<IOrderInfo> _orderInfoMockExcludingVAT;

		[SetUp]
		public void Setup()
		{
			_orderInfoMockIncludingVAT = MockConstructors.CreateOrderInfoMock(true);
			_orderInfoMockExcludingVAT = MockConstructors.CreateOrderInfoMock(false);
		}

		[Test]
		public void ThatCorrectAmountIsAddedToPriceInclVat()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 19, null, _orderInfoMockIncludingVAT.Object);

			var variantinfo = new ProductVariantInfo();
			variantinfo.PriceInCents = 100;

			product.ProductVariants = new List<ProductVariantInfo> {variantinfo};

			Assert.AreEqual(1100, product.PriceWithVatInCents);
		}

		[Test]
		public void ThatCorrectAmountIsSubractedFromPriceInclVat()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 19, null, _orderInfoMockIncludingVAT.Object);

			var variantinfo = new ProductVariantInfo();
			variantinfo.PriceInCents = -100;

			product.ProductVariants = new List<ProductVariantInfo> {variantinfo};

			Assert.AreEqual(900, product.PriceWithVatInCents);
		}

		[Test]
		public void ThatCorrectAmountIsAddedToPrice()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 6, null, _orderInfoMockExcludingVAT.Object);

			var variantinfo = new ProductVariantInfo();
			variantinfo.PriceInCents = 100;

			product.ProductVariants = new List<ProductVariantInfo> {variantinfo};

			Assert.AreEqual(1100, product.PriceWithoutVatInCents);
		}

		[Test]
		public void ThatCorrectAmountIsSubractedFromPrice()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 6, null, _orderInfoMockExcludingVAT.Object);

			var variantinfo = new ProductVariantInfo();
			variantinfo.PriceInCents = -100;

			product.ProductVariants = new List<ProductVariantInfo> {variantinfo};

			Assert.AreEqual(900, product.PriceWithoutVatInCents);
		}
	}
}