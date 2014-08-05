using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

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
		public void PercentageDiscountIncludingVat()
		{
			IOC.IntegrationTest(); 
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 10);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			IOC.OrderDiscountRepository.SetupFake(discount);
			order.OrderDiscountsFactory = () => IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(order, order.Localization).ToList();
			order.ResetDiscounts();

			Assert.AreEqual(1000, order.GetAmount(true, false, true));
			Assert.AreEqual(909, order.GetAmount(false, false, true));
			Assert.AreEqual(500, order.GetAmount(true, true, true));
			Assert.AreEqual(455, order.GetAmount(false, true, true));

			var price = new SimplePrice(order, order.Localization);
			Assert.AreEqual(1000, price.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(909, price.BeforeDiscount.WithoutVat.ValueInCents);
			Assert.AreEqual(91, price.BeforeDiscount.Vat.ValueInCents);
			Assert.AreEqual(500, price.WithVat.ValueInCents);
			Assert.AreEqual(455, price.WithoutVat.ValueInCents);
			Assert.AreEqual(45, price.Vat.ValueInCents);

			Assert.AreEqual(500, price.Discount.WithVat.ValueInCents);
			Assert.AreEqual(454, price.Discount.WithoutVat.ValueInCents);
		}

		[Test]
		public void PercentageDiscountExncludingVat()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 10);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			IOC.OrderDiscountRepository.SetupFake(discount);
			order.OrderDiscountsFactory = () => IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(order, order.Localization).ToList();
			order.ResetDiscounts();

			Assert.AreEqual(1100, order.GetAmount(true, false, true));
			Assert.AreEqual(1000, order.GetAmount(false, false, true));
			Assert.AreEqual(550, order.GetAmount(true, true, true));
			Assert.AreEqual(500, order.GetAmount(false, true, true));

			var price = new SimplePrice(order, order.Localization);
			Assert.AreEqual(1100, price.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(1000, price.BeforeDiscount.WithoutVat.ValueInCents);
			Assert.AreEqual(100, price.BeforeDiscount.Vat.ValueInCents);
			Assert.AreEqual(550, price.WithVat.ValueInCents);
			Assert.AreEqual(500, price.WithoutVat.ValueInCents);
			Assert.AreEqual(50, price.Vat.ValueInCents);

			Assert.AreEqual(550, price.Discount.WithVat.ValueInCents);
			Assert.AreEqual(500, price.Discount.WithoutVat.ValueInCents);
		}

		[Test]
		public void AmountDiscountIncludingVat()
		{
			IOC.IntegrationTest();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 10);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(500, DiscountOrderCondition.None, 0);
			IOC.OrderDiscountRepository.SetupFake(discount);
			order.OrderDiscountsFactory = () => IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(order, order.Localization).ToList();
			order.ResetDiscounts();

			var absoluteDiscountAmount = 500;
			var AverageOrderVatPercentage = 10m;
			var vatAmountFromWithVat = VatCalculator.VatAmountFromWithVat(absoluteDiscountAmount, AverageOrderVatPercentage);
			//var vatAmountFromWithoutVat = VatCalculator.VatAmountFromWithoutVat(absoluteDiscountAmount, AverageOrderVatPercentage);
			Assert.AreEqual(45, vatAmountFromWithVat);

			// berekende discount vat = 45,45454545..
			// correct zou zijn 500 - 454 = 46 (zie hieronder)

			// full amount: 909
			// discounted amount: 455
			// discount: 909 - 455 = 454
			//  'discount vat': 500 - 454 = 46

			Assert.AreEqual(1000, order.GetAmount(true, false, true));
			Assert.AreEqual(909, order.GetAmount(false, false, true));
			Assert.AreEqual(500, order.GetAmount(true, true, true));
			Assert.AreEqual(455, order.GetAmount(false, true, true));

			var price = new SimplePrice(order, order.Localization);
			Assert.AreEqual(1000, price.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(909, price.BeforeDiscount.WithoutVat.ValueInCents);
			Assert.AreEqual(91, price.BeforeDiscount.Vat.ValueInCents);
			Assert.AreEqual(500, price.WithVat.ValueInCents);
			Assert.AreEqual(455, price.WithoutVat.ValueInCents);
			Assert.AreEqual(45, price.Vat.ValueInCents);

			Assert.AreEqual(500, price.Discount.WithVat.ValueInCents);
			Assert.AreEqual(454, price.Discount.WithoutVat.ValueInCents);
		}

		[Test]
		public void AmountDiscountExcludingVat()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 10);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(500, DiscountOrderCondition.None, 0);
			IOC.OrderDiscountRepository.SetupFake(discount);
			order.OrderDiscountsFactory = () => IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(order, order.Localization).ToList();
			order.ResetDiscounts();

			Assert.AreEqual(1100, order.GetAmount(true, false, true));
			Assert.AreEqual(1000, order.GetAmount(false, false, true));
			Assert.AreEqual(550, order.GetAmount(true, true, true));
			Assert.AreEqual(500, order.GetAmount(false, true, true));

			var price = new SimplePrice(order, order.Localization);
			Assert.AreEqual(1100, price.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(1000, price.BeforeDiscount.WithoutVat.ValueInCents);
			Assert.AreEqual(100, price.BeforeDiscount.Vat.ValueInCents);
			Assert.AreEqual(550, price.WithVat.ValueInCents);
			Assert.AreEqual(500, price.WithoutVat.ValueInCents);
			Assert.AreEqual(50, price.Vat.ValueInCents);

			Assert.AreEqual(550, price.Discount.WithVat.ValueInCents);
			Assert.AreEqual(500, price.Discount.WithoutVat.ValueInCents);
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
