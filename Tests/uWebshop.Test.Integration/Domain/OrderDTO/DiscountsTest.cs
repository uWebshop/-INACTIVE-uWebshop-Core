using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Integration.Domain.OrderDTO
{
	[TestFixture]
	public class DiscountsTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
		}

		[Test]
		public void DiscountShouldGiveSameAmountForOrderAfterDeserialization()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(234, DiscountOrderCondition.None, 0);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);
			var orderData = orderInfo.ToOrderData();
			var convertedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(orderData);
			var convertedDiscount = new uWebshop.Domain.OrderDTO.OrderDiscount(StoreHelper.CurrentLocalization, discount, orderInfo);

			//Assert.AreEqual(234, orderInfo.DiscountAmountInCents);
			//Assert.AreEqual(234, convertedOrderInfo.DiscountAmountInCents);
			var discountCalculator = IOC.DiscountCalculationService.Actual().Resolve();
			Assert.AreEqual(234, discountCalculator.DiscountAmountForOrder(discount, orderInfo));
			Assert.AreEqual(234, discountCalculator.DiscountAmountForOrder(discount, convertedOrderInfo));
			Assert.AreEqual(234, discountCalculator.DiscountAmountForOrder(convertedDiscount, orderInfo));
			Assert.AreEqual(234, discountCalculator.DiscountAmountForOrder(convertedDiscount, convertedOrderInfo));
		}
	}
}