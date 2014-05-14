using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.ProductDiscount
{
	[TestFixture]
	public class ProductDiscountCalculations
	{
		[Test]
		public void Amount()
		{
			IOC.UnitTest();
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			product.DiscountAmountInCents = 100;

			Assert.AreEqual(900, product.PriceInCents);
			Assert.AreEqual(100, product.ProductDiscountInCents);
			Assert.AreEqual(1000, product.RangedOriginalPrice);
		}

		[Test]
		public void Percentage()
		{
			IOC.UnitTest();
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			product.DiscountPercentage = 10;

			Assert.AreEqual(900, product.PriceInCents);
			Assert.AreEqual(100, product.ProductDiscountInCents);
			Assert.AreEqual(1000, product.RangedOriginalPrice);
		}

		[Test]
		public void AmountIncludingVariant()
		{
			IOC.UnitTest();
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			product.ProductVariants = new List<ProductVariantInfo> { DefaultFactoriesAndSharedFunctionality.CreateProductVariantInfo(500) };
			product.DiscountAmountInCents = 100;

			Assert.AreEqual(1400, product.PriceInCents);
			Assert.AreEqual(100, product.ProductDiscountInCents);
			Assert.AreEqual(1500, product.RangedOriginalPrice);
		}

		[Test]
		public void PercentageIncludingVariant()
		{
			IOC.UnitTest();
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			product.ProductVariants = new List<ProductVariantInfo> { DefaultFactoriesAndSharedFunctionality.CreateProductVariantInfo(500) };
			product.DiscountAmountInCents = 100;

			Assert.AreEqual(1400, product.PriceInCents);
			Assert.AreEqual(100, product.ProductDiscountInCents);
			Assert.AreEqual(1500, product.RangedOriginalPrice);
		}
	}
}