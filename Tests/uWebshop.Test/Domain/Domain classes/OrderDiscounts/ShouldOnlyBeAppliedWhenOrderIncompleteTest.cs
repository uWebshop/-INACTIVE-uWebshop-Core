using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Test.Repositories;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class ShouldOnlyBeAppliedWhenOrderIncompleteTest
	{
		[SetUp]
		public void SetUp()
		{
			IOC.IntegrationTest();
		}

		[Test]
		public void RecreateBug20130117_()
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(12990, 1);
			productInfo.Id = TestProductService.ProductId1;
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(10000, DiscountOrderCondition.None, 0);
			discount.CouponCode = "coup";
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);
			orderInfo.SetCouponCode("coup");

			Assert.AreEqual(10000, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(10000, orderInfo.DiscountAmountWithVatInCents);
			Assert.AreEqual(2990, orderInfo.OrderTotalInCents);

			IOC.OrderUpdatingService.Resolve().AddOrUpdateOrderLine(orderInfo, 0, TestProductService.ProductId1, "update", 1, new List<int>());

			Assert.AreEqual(10000, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(10000, orderInfo.DiscountAmountWithVatInCents);
			Assert.AreEqual(2990, orderInfo.OrderTotalInCents);
		}
	}
}