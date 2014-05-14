using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.ProductInfoTests
{
	[TestFixture]
	public class EffectsOfVariantOnProductPrices
	{
		[Test]
		public void ProductInfoPriceInCents_ShouldIncludeVariantPriceInCents()
		{
			var product = new ProductInfo();
			var variant = new ProductVariantInfo();
			variant.PriceInCents = 5000;
			product.ProductVariants.Add(variant);

			Assert.AreEqual(5000, product.PriceInCents);
		}

		[Test]
		public void ProductInfoPriceInCents_ShouldIncludeDiscountedVariantPriceInCents()
		{
			var product = new ProductInfo();
			var variant = new ProductVariantInfo();
			variant.PriceInCents = 5000;
			variant.DiscountPercentage = 50;
			product.ProductVariants.Add(variant);

			Assert.AreEqual(2500, product.PriceInCents);
		}
	}
}