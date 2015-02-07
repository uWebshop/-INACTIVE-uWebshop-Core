using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Integration.RegressionTests
{
	[TestFixture]
	public class _20150206_order_grand_total_VAT_over_total
	{
		[Test]
		public void OrderWithManyItemsHadRelativelyLargeDeviationWhenCalculatingVatOverPartsInsteadOfOverTotal()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.InclVat();
			IOC.VatCalculationStrategy.OverTotal();

			var productInfo1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(7500, 2, 21m);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(200, 25, 21m);
			var productInfo3 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(200, 50, 21m);
			var productInfo4 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(200, 50, 21m);
			var productInfo5 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(200, 25, 21m);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo1, productInfo2, productInfo3, productInfo4, productInfo5);

			var directlyCalculatedVat = VatCalculator.VatAmountFromWithVat(45000, 21m);
			Assert.AreEqual(7810, directlyCalculatedVat);

			Assert.AreEqual(7810, order.VatTotalInCents);

			Assert.AreEqual(45000, order.GrandtotalInCents);

			Assert.AreEqual(45000 - 7810, order.SubtotalInCents);

			Console.WriteLine(directlyCalculatedVat);
			Console.WriteLine(order.VatTotalInCents);
			Console.WriteLine(order.GrandtotalInCents);


			// todo: this might be a concern:
			// var orderNewDiscount = order.DiscountAmountWithVatInCents;
			// Assert.AreEqual(x, orderNewDiscount); where x should not be 0 but orderNewDiscount is 0
		}
	}
}
