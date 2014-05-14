using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Test.Domain.Domain_classes.OrderDiscounts;
using uWebshop.Test.Mocks;
using Order = uWebshop.Domain.OrderDTO.Order;

namespace uWebshop.Test.Domain.Domain_classes.OrderData
{
	[TestFixture]
	public class PreservationOfDataFromOrderInfoToOrderDataTest
	{
		[Test]
		public void ConversionToOrderDataShouldPreserveValues()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 1));
			orderInfo.Discounts.Add(DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(10));
			var orderData = new Order(orderInfo);

			Assert.AreEqual(orderInfo.ConfirmDate, orderData.ConfirmDate);
			Assert.AreEqual(orderInfo.RegionalVatInCents, orderData.RegionalVatAmount);
			Assert.AreEqual(orderInfo.ShippingProviderAmountInCents, orderData.ShippingProviderPrice);
			Assert.AreEqual(orderInfo.PaymentProviderPriceInCents, orderData.PaymentProviderPrice);
		}
	}
}