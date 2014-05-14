using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Domain.Domain_classes.ProductTests
{
	[TestFixture]
	public class ProductIncludingVariantPricesTests
	{
		private ProductVariant _variant;
		private Product _product;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			_product = new Product {OriginalPriceInCents = 1000, Id = 12234, Ranges = new List<Range>(), Vat = 21m};
			_variant = new ProductVariant {OriginalPriceInCents = 100};
			_product.VariantGroups = new List<IProductVariantGroup>{ new ProductVariantGroup("",new List<ProductVariant> {_variant},1)};
			_variant.Product = _product;
		}

		[Test]
		public void PriceIncludingProductPriceInCents_NotDiscounted_GivesProductPlusVariant()
		{
			IOC.ProductDiscountService.SetupNewMock().Setup(m => m.GetAdjustedPriceForProductWithId(12234, It.IsAny<ILocalization>(), 1000, It.IsAny<int>())).Returns(1000);

			Assert.AreEqual(1000, _product.Price.ValueInCents());
			Assert.AreEqual(100, _variant.Price.ValueInCents());

			Assert.AreEqual(1100, _variant.PriceIncludingProduct(_product).ValueInCents());
		}

		[Test]
		public void PriceIncludingProductPriceInCents_ProductDiscountExcludingVariants_GivesDiscountedProductPlusVariant()
		{
			var setupNewMock = IOC.ProductDiscountService.SetupNewMock();
			setupNewMock.Setup(m => m.GetDiscountByProductId(12234, It.IsAny<ILocalization>())).Returns(new DiscountProduct { DiscountValue = 10 * 100, DiscountType = DiscountType.Percentage, ExcludeVariants = true, });
			setupNewMock.Setup(m => m.GetAdjustedPriceForProductWithId(12234, It.IsAny<ILocalization>(), 1000, It.IsAny<int>())).Returns(900);

			//setupNewMock.Verify(m => m.GetAdjustedPriceForProductWithId(12234, It.IsAny<ILocalization>(), 1000, It.IsAny<int>()));
			Assert.AreEqual(900, _product.PriceInCents);
			Assert.AreEqual(100, _variant.PriceInCents);


			Assert.AreEqual(1000, _variant.PriceIncludingProductPriceInCents);
		}

		[Test]
		public void PriceIncludingProductPriceInCents_ProductDiscountIncludingVariants_GivesDiscountedProductPlusVariant()
		{
			var setupNewMock = IOC.ProductDiscountService.SetupNewMock();
			setupNewMock.Setup(m => m.GetDiscountByProductId(12234, It.IsAny<ILocalization>())).Returns(new DiscountProduct { DiscountValue = 10 * 100, DiscountType = DiscountType.Percentage, ExcludeVariants = false, });
			setupNewMock.Setup(m => m.GetAdjustedPriceForProductWithId(12234, It.IsAny<ILocalization>(), 1000, It.IsAny<int>())).Returns(900);

			Assert.AreEqual(1000, _product.OriginalPriceInCents);
			Assert.AreEqual(900, _product.Price.ValueInCents());

			Assert.IsTrue(_variant.Product.IsDiscounted);
			Assert.IsTrue(!_variant.Product.Discount.ExcludeVariants);
			Assert.IsTrue(_variant.Product.Discount.Type == DiscountType.Percentage);
			Assert.AreEqual(990, _variant.PriceIncludingProductPriceInCents);
		}
	}
}