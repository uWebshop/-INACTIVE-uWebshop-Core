using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace uWebshop.Test.Integration.RegressionTests
{
	[TestFixture]
	public class _20131205_price_VAT_rounding
	{
		[Test]
		public void Test()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			IOC.VatCalculationStrategy.OverParts();
			
			var productDiscountPercentage = DefaultFactoriesAndSharedFunctionality.CreateProductDiscountPercentage(17);
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2456, 5, 20m, productDiscountPercentage);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var line = order.OrderLines.Single();

			Assert.AreEqual(2038, productInfo.PriceInCents);
			Assert.AreEqual(2446, productInfo.PriceWithVatInCents);
			Assert.AreEqual(408, productInfo.VatAmountInCents);
			Assert.AreEqual(2446 * 5, line.AmountInCents);
			Assert.AreEqual(2446 * 5, line.GrandTotalInCents);
			Assert.AreEqual(2038 * 5, line.SubTotalInCents);
			Assert.AreEqual(408 * 5, line.VatAmountInCents);
			Assert.AreEqual(408 * 5, line.OrderLineVatAmountAfterOrderDiscountInCents);
			Assert.AreEqual(408 * 5, order.VatTotalInCents);
			Assert.AreEqual(2446 * 5, order.GrandtotalInCents);
			Assert.AreEqual(2038 * 5, order.SubtotalInCents);
		}

		[Test]
		public void TestDiscount()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			IOC.VatCalculationStrategy.OverParts();
			
			var productDiscountPercentage = DefaultFactoriesAndSharedFunctionality.CreateProductDiscountPercentage(17);
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2456, 5, 20m, productDiscountPercentage);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(order, discount);
			var line = order.OrderLines.Single();

			Assert.AreEqual(2038, productInfo.PriceInCents);
			Assert.AreEqual(2446, productInfo.PriceWithVatInCents);
			Assert.AreEqual(408, productInfo.VatAmountInCents);
			Assert.AreEqual(2446 * 5, line.AmountInCents);
			Assert.AreEqual(2446 * 5, line.GrandTotalInCents);
			Assert.AreEqual(2038 * 5, line.SubTotalInCents);
			Assert.AreEqual(408 * 5, line.VatAmountInCents);

			// todo: commented lines need to be adjusted for discount on order lvl
			//Assert.AreEqual(2038 / 2, productInfo.OrderPriceInCents);
			//Assert.AreEqual(2446 / 2, productInfo.OrderPriceWithVatInCents);
			//Assert.AreEqual(408 / 2, productInfo.OrderVatAmountInCents);
			
			//Assert.AreEqual(2038 * 5 / 2, line.OrderAmountInCents);
			//Assert.AreEqual(2446 * 5 / 2, line.OrderAmountWithVatInCents);
			//Assert.AreEqual(2038 * 5 / 2, line.OrderAmountWithoutInCents);
			//Assert.AreEqual(408 * 5 / 2, line.OrderVatAmountInCents);

			//Assert.AreEqual(408 * 5 / 2, line.OrderLineVatAmountAfterOrderDiscountInCents);
			Assert.AreEqual(408 * 5 / 2, order.VatTotalInCents);
			Assert.AreEqual(2446 * 5 / 2, order.GrandtotalInCents);
			Assert.AreEqual(2038 * 5 / 2, order.SubtotalInCents);
		}
	}
}
