using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Services.OrderUpdatingsServiceTests
{
	[TestFixture]
	public class AddOrUpdateOrderlineTests
	{
		[Test]
		public void asf()
		{
			IOC.UnitTest();

			var productDiscount = DefaultFactoriesAndSharedFunctionality.CreateProductDiscountPercentage(36);
			productDiscount.RangesString = new List<Range> {new Range {From = 1, To = 20, PriceInCents = 10000}}.ToRangesString();
			//productDiscount.Ranges.Add(new Range{From = 1, To = 20, PriceInCents = 1000});
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(5000, 5, productDiscount);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);

			Assert.IsTrue(productDiscount.Ranges.Any());
			Assert.AreEqual(10000, productDiscount.RangedDiscountValue(15));
			Assert.AreEqual(100, product.DiscountPercentage);

			Assert.AreEqual(0, order.OrderLineTotalInCents);

			IOC.ProductService.SetupNewMock().Setup(m => m.GetById(1234, It.IsAny<ILocalization>(), It.IsAny<bool>())).Returns(new Product { Id = 1234, UseVariantStock = true });

			var orderUpdatingService = IOC.OrderUpdatingService.Actual().Resolve();

			// todo: actually make the update productinfo functionality testable..
			//orderUpdatingService.AddOrUpdateOrderLine(order, 0, 1234, "update", 15, new List<int>());
		}

		[Test]
		public void UpdatingOrderlineItemCountWithoutVariantInformation_ShouldDeleteVariants()
		{
			IOC.UnitTest();
			var variant = DefaultFactoriesAndSharedFunctionality.CreateProductVariantInfo(10);
			variant.Id = 1234;
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 2);
			product.ProductVariants = new List<ProductVariantInfo> {variant};
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);
			order.OrderLines.Single().OrderLineId = 2345;
			var orderUpdatingService = IOC.OrderUpdatingService.Actual().Resolve();

			orderUpdatingService.AddOrUpdateOrderLine(order, 2345, 0, "update", 5, new List<int>());

			Assert.AreEqual(5, order.OrderLines.Single().ProductInfo.ItemCount);
			Assert.False(order.OrderLines.Single().ProductInfo.ProductVariants.Any());
		}
	}
}