using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;
using uWebshop.Test.Repositories;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class CouponCodeTests
	{
		[Test]
		public void bla1()
		{
			IOC.IntegrationTest();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(12990, 1);
			productInfo.Id = TestProductService.ProductId1;
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);

			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(100);
			discount.CouponCode = "coup";

			IO.Container.Resolve<IOrderService>().UseStoredDiscounts(orderInfo, new List<IOrderDiscount> { new OrderDiscount(StoreHelper.CurrentLocalization,discount, orderInfo) });
			orderInfo.SetCouponCode("coup");

			Assert.AreEqual(12990, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(12990, orderInfo.DiscountAmountWithVatInCents);
			Assert.AreEqual(0, orderInfo.OrderTotalInCents);
		}
	}
}