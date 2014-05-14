using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Test;

namespace uWebshop.Domain.Domain_classes.OrderInfo
{
	[TestFixture]
	public class ConversionWithDTOsTest
	{
		[Test]
		public void OrderInfoToDTOAndBackShouldPreserveValues()
		{
			IOC.UnitTest();

			var originalOrderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(995, 2));
			var dto = new OrderDTO.Order(originalOrderInfo);
			var convertedOrderInfo = dto.ToOrderInfo();

			Assert.AreEqual(originalOrderInfo.GrandtotalInCents, convertedOrderInfo.GrandtotalInCents);
			Assert.AreEqual(originalOrderInfo.SubtotalInCents, convertedOrderInfo.SubtotalInCents);
			Assert.AreEqual(originalOrderInfo.ShippingProviderAmountInCents, convertedOrderInfo.ShippingProviderAmountInCents);
			Assert.AreEqual(originalOrderInfo.DiscountAmountInCents, convertedOrderInfo.DiscountAmountInCents);
		}
	}
}