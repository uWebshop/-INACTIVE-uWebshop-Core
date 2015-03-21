using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;

namespace uWebshop.Test.Public_API.BasketTests
{
	[TestFixture]
	public class OrderAmountTests
	{
		private OrderInfo _source;

		[Test]
		public void DiscountAmountCalculationRegressionTest()
		{
			IOC.IntegrationTest();
			_source = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(_source, discount);
			var price = Price.CreateDiscountedRanged(BasketTotalBeforeDiscount(), null, _source.PricesAreIncludingVAT, _source.AverageOrderVatPercentage, null, i => i - _source.DiscountAmountInCents, _source.Localization);

			Assert.AreEqual(500, IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(discount, _source));
			Assert.AreEqual(500, _source.DiscountAmountInCents);
			Assert.AreEqual(500, price.WithVat.ValueInCents);

			Assert.AreEqual(500, price.Discount.WithVat.ValueInCents);
		}
		private int BasketTotalBeforeDiscount()
		{
			return _source.OrderLines.Sum(orderline => orderline.AmountInCents) + _source.ShippingProviderAmountInCents + _source.PaymentProviderPriceInCents;
		}

		[Test]
		public void DiscountAmountRegressionTest20140116()
		{
			IOC.IntegrationTest();
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(6000, 1, 19, new DiscountProduct { DiscountType = DiscountType.Amount, DiscountValue = 2000 });
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);
			var oldLine = order.OrderLines.Single();
			var basket = Basket.CreateBasketFromOrderInfo(order);

			Assert.AreEqual(4000, order.OrderLineTotalInCents);
			Assert.AreEqual(4000, basket.OrderAmount.ValueInCents());
			Assert.AreEqual(4000, basket.OrderAmount.BeforeDiscount.ValueInCents());
			Assert.AreEqual(0, basket.OrderAmount.Discount.ValueInCents());


			IOC.IntegrationTest();
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(25);
			IOC.OrderDiscountRepository.SetupFake(discount);
			//DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(order, discount);
			order.OrderDiscountsFactory = () => IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(order, order.Localization).ToList();
			order.ResetDiscounts();

			Assert.AreEqual(4000, order.OrderLineTotalInCents);

			Assert.AreEqual(4000, order.OrderLines.Single().ProductInfo.PriceInCents);

			Assert.AreEqual(1000, IOC.DiscountCalculationService.Actual().Resolve().DiscountAmountForOrder(discount, order));
			order.ResetDiscounts();

			Assert.AreEqual(1000, order.DiscountAmountInCents);
			Assert.AreEqual(4000, order.OrderLineTotalInCents);
			Assert.AreEqual(3000, basket.OrderAmount.ValueInCents());
			Assert.AreEqual(4000, basket.OrderAmount.BeforeDiscount.ValueInCents());

			Assert.AreEqual(4000, basket.OrderAmount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(3361, basket.OrderAmount.BeforeDiscount.WithoutVat.ValueInCents);

			Assert.AreEqual(4000, oldLine.Amount.ValueInCents());
			Assert.AreEqual(4000, oldLine.Amount.BeforeDiscount.ValueInCents());
			Assert.AreEqual(0, oldLine.Amount.Discount.WithVat.ValueInCents);
			Assert.AreEqual(0, oldLine.Amount.Discount.ValueInCents());


			var price = new SummedPrice(new[] { oldLine.Amount }, true, 19, StoreHelper.CurrentLocalization, IO.Container.Resolve<IVatCalculationStrategy>());
			Assert.AreEqual(4000, price.ValueInCents);
			price = new SummedPrice(new[] { oldLine.Amount }, true, 19, StoreHelper.CurrentLocalization, IO.Container.Resolve<IVatCalculationStrategy>());
			Assert.AreEqual(4000, price.BeforeDiscount.ValueInCents());
			price = new SummedPrice(new[] { oldLine.Amount }, true, 19, StoreHelper.CurrentLocalization, IO.Container.Resolve<IVatCalculationStrategy>());
			Assert.AreEqual(0, price.Discount.ValueInCents());

			var line = basket.OrderLines.Single();
			Assert.AreEqual(4000, line.Amount.ValueInCents());
			Assert.AreEqual(4000, line.Amount.BeforeDiscount.ValueInCents());
			Assert.AreEqual(0, line.Amount.Discount.ValueInCents());

			Assert.AreEqual(1000, basket.OrderAmount.Discount.ValueInCents());
			Assert.AreEqual(840, basket.OrderAmount.Discount.WithoutVat.ValueInCents); 
			Assert.AreEqual(1000, basket.OrderAmount.Discount.WithVat.ValueInCents);

		}
		[Test]
		public void TestSummedPrice()
		{
			IOC.UnitTest();
			var basePriceMock = new Mock<IDiscountedRangedPrice>();
			var price = new SummedPrice(new[] { basePriceMock.Object }, true, 19, StoreHelper.CurrentLocalization, IOC.VatCalculationStrategy.Resolve());

			var a = price.Discount;

			basePriceMock.VerifyGet(m => m.Discount);
		}

		[Test]
		public void ProductInfo_PriceCalculationRegressionTest()
		{
			IOC.IntegrationTest();
			var product = new ProductInfo { OriginalPriceInCents = 990000, DiscountAmountInCents = 990000 - 100000 };
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);

			Assert.AreEqual(990000, product.Price.BeforeDiscount.ValueInCents());
			Assert.AreEqual(100000, product.Price.ValueInCents());
			Assert.AreEqual(890000, product.Price.Discount.WithVat.ValueInCents);
		}

		[Test]
		public void DiscountAmountRegressionTest20140321()
		{
			IOC.IntegrationTest();
			IOC.VatCalculationStrategy.OverParts();
			var product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(10, 5);
			var product2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(750, 3);
			//var product3 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1, 5, 19);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product1, product2);
			//var basket = Basket.CreateBasketFromOrderInfo(order);

			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(0);
			IOC.OrderDiscountRepository.SetupFake(discount.ToDiscountOrder());
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(order, discount);

			//Assert.AreEqual(2255, basket.OrderAmount.Discount.WithVat.ValueInCents);
			//Assert.AreEqual(0, basket.OrderAmount.WithVat.ValueInCents);

			//var discountAmount = IOC.DiscountCalculationService.Actual().Resolve().DiscountAmountForOrder(discount, order, true);
			//Assert.AreEqual(2255, discountAmount);

			var line = order.OrderLines.First();

			var unit = line.SellableUnits.First();
			Assert.AreEqual(10, unit.PriceInCents);
			Assert.AreEqual(0, unit.DiscountedPrice);

			var newDiscount = line.SellableUnits.Select(su => su.PriceInCents - su.DiscountedPrice).Sum();
			Assert.AreEqual(50, newDiscount);
			
			//OrderLineTotalWithVatInCents, GetAmount(true, false, true) - GetAmount(true, true, true)

			Assert.AreEqual(0, order.OrderLineTotalWithVatInCents);

			var orderNewDiscount = order.DiscountAmountWithVatInCents;
			Assert.AreEqual(2300, orderNewDiscount);

			var orderDiscount = order.DiscountAmountInCents;
			Assert.AreEqual(2300, orderDiscount);

			Assert.AreEqual(50, line.DiscountInCents);
		}

		[Test]
		public void DiscountAmountRegressionTest20140626()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			var product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 0);
			var product2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 20);

			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product1, product2);

			Assert.AreEqual(2000, order.SubtotalInCents);
			Assert.AreEqual(2200, order.GrandtotalInCents);
			Assert.AreEqual(200, order.VatTotalInCents);

			var basket = Basket.CreateBasketFromOrderInfo(order);

			Assert.AreEqual(2000, basket.OrderAmount.WithoutVat.ValueInCents);
			Assert.AreEqual(2200, basket.OrderAmount.WithVat.ValueInCents);
			Assert.AreEqual(200, basket.OrderAmount.Vat.ValueInCents);
		}
		[Test]
		public void DiscountAmountRegressionTest20140627()
		{
			IOC.IntegrationTest();
			IOC.SettingsService.ExclVat();
			var product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(4000, 1, 0);
			var product2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1800, 1, 20);

			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product1, product2);

			Assert.AreEqual(5800, order.SubtotalInCents);
			Assert.AreEqual(6160, order.GrandtotalInCents);
			Assert.AreEqual(360, order.VatTotalInCents);

			var basket = Basket.CreateBasketFromOrderInfo(order);

			Assert.AreEqual(5800, basket.OrderAmount.WithoutVat.ValueInCents);
			Assert.AreEqual(6160, basket.OrderAmount.WithVat.ValueInCents);
			Assert.AreEqual(360, basket.OrderAmount.Vat.ValueInCents);
		}


		[Test]
		public void DiscountAmountRegressionTest20140408()
		{
			IOC.IntegrationTest();
			var product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(400, 10);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product1);

			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(10, DiscountOrderCondition.OnTheXthItem, 2);
			//IOC.OrderDiscountRepository.SetupFake(discount.ToDiscountOrder());
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(order, discount);

			var basket = new BasketOrderInfoAdaptor(order);
			var basketLine = basket.OrderLines.Single();
			Console.WriteLine(basketLine.Amount.Discount.WithVat.ToCurrencyString());
			Console.WriteLine(basketLine.Amount.BeforeDiscount.WithVat.ToCurrencyString());
			Console.WriteLine(basketLine.Amount.WithVat.ToCurrencyString());
			
			var line = order.OrderLines.First();
			Console.WriteLine(line.Amount.Discount.WithVat.ToCurrencyString());
			Console.WriteLine(line.Amount.BeforeDiscount.WithVat.ToCurrencyString());
			Console.WriteLine(line.Amount.WithVat.ToCurrencyString());

			Assert.AreEqual(50, basketLine.Amount.Discount.WithVat.ValueInCents);
			Assert.AreEqual(4000, basketLine.Amount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(3950, basketLine.Amount.WithVat.ValueInCents);

			Assert.AreEqual(50, line.Amount.Discount.WithVat.ValueInCents);
			Assert.AreEqual(4000, line.Amount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(3950, line.Amount.WithVat.ValueInCents);
		}


	}

	[TestFixture]
	public class OrderLineTotal
	{
		[Test]
		public void DiscountAmount()
		{
			IOC.IntegrationTest();
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(5995, 1, 19, new DiscountProduct { DiscountType = DiscountType.Amount, DiscountValue = 2000 }));
			var basket = Basket.CreateBasketFromOrderInfo(order);

			Assert.AreEqual(3995, order.OrderLineTotalInCents);
			Assert.AreEqual(3995, basket.OrderLineTotal.ValueInCents());
			Assert.AreEqual(3995, basket.OrderLineTotal.BeforeDiscount.ValueInCents());
			Assert.AreEqual(0, basket.OrderLineTotal.Discount.ValueInCents());
		}
	}
}
