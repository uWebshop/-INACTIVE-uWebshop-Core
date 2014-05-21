using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.API;

namespace uWebshop.Test.Domain.Domain_classes.OrderInfoTests
{
	[TestFixture]
	public class GetAmountTests
	{
		[Test]
		public void UpdatingAnOrderlineShouldBeReflectedInAmount()
		{
			IOC.UnitTest();
			var orderUpdateService = IOC.OrderUpdatingService.Actual().Resolve();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			productInfo.Id = 5678;
			productInfo.Vat = 10;
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var line = order.OrderLines.Single();

			Assert.AreEqual(1000, order.GetAmount(true, false, false));
			Assert.AreEqual(1000, line.GetAmount(true, false, false));
			Assert.AreEqual(909, order.GetAmount(false, false, false));

			var basket = Basket.CreateBasketFromOrderInfo(order);
			Assert.AreEqual(1000, basket.ChargedOrderAmount.ValueInCents);

			orderUpdateService.AddOrUpdateOrderLine(order, 0, 5678, "update", 2, new int[0]);

			Assert.AreEqual(1, order.OrderLines.Count);
			Assert.AreEqual(2, productInfo.Quantity);

			Assert.AreEqual(2, line.SellableUnits.Count());

			Assert.AreEqual(2000, line.GetAmount(true, false, false));
			Assert.AreEqual(2000, order.GetAmount(true, false, false));

			basket = Basket.CreateBasketFromOrderInfo(order);
			Assert.AreEqual(2000, basket.ChargedOrderAmount.ValueInCents);
		}

		[Test]
		public void BasicTests()
		{
			IOC.UnitTest();
			IOC.SettingsService.ExclVat();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			productInfo.Id = 5678;
			productInfo.Vat = 10;
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var line = order.OrderLines.Single();

			Assert.AreEqual(1000, order.GetAmount(false, false, false));
			Assert.AreEqual(1000, line.GetAmount(false, false, false));
			Assert.AreEqual(1100, order.GetAmount(true, false, false));
			Assert.AreEqual(1100, line.GetAmount(true, false, false));

			Assert.AreEqual(1100, order.ChargedAmountInCents);
			Assert.AreEqual(1100, order.GrandtotalInCents);
			Assert.AreEqual(1000, order.SubtotalInCents);

			var basket = Basket.CreateBasketFromOrderInfo(order);
			Assert.AreEqual(1100, basket.ChargedOrderAmount.ValueInCents);
		}
	}
}
