using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.ProductDiscount
{
	[TestFixture]
	public class ProductDiscountOnProductWithVariantTest
	{
		[Test]
		public void PercentageProductDiscountOnProductWithVariant_ShouldGiveDiscountThatTakesVariantEffectIntoAccount()
		{
			IOC.UnitTest();
			var discount = DefaultFactoriesAndSharedFunctionality.CreateProductDiscountPercentage(50);
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, discount);
			var variant = DefaultFactoriesAndSharedFunctionality.CreateProductVariantInfo(-200);
			DefaultFactoriesAndSharedFunctionality.SetVariantsOnProductInfo(productInfo, variant);

			Assert.AreEqual(400, productInfo.PriceInCents);
		}
	}
}