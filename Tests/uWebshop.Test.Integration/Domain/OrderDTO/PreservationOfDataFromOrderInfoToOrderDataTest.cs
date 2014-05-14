using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Integration.Domain.OrderDTO
{
	[TestFixture]
	public class PreservationOfDataFromOrderInfoToOrderDataTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
		}

		[Test]
		public void CalculatedValuesShouldRemainTheSameAfterRoundTripConversion()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 1));
			orderInfo.Discounts.Add(DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(10));
			var orderData = orderInfo.ToOrderData();
			var convertedOrderInfo = OrderInfo.CreateOrderInfoFromOrderData(orderData);

			Assert.AreEqual(orderInfo.Status, convertedOrderInfo.Status);
			Assert.AreEqual(orderInfo.DiscountAmountInCents, convertedOrderInfo.DiscountAmountInCents);
			Assert.AreEqual(orderInfo.GrandtotalInCents, convertedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(orderInfo.SubtotalInCents, convertedOrderInfo.SubtotalInCents);
		}
	}
}