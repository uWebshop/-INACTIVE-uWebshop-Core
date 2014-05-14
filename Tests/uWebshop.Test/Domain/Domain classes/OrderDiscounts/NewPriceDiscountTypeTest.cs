using System.Linq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class NewPriceDiscountTypeTest
	{
		private IDiscountCalculationService _discountCalculationService;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithSingleProductAndApplicableNewPriceDiscount_ShouldGiveProductPriceMinusDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(1995 - 1000, actual);
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithOneLineTwoProductsAndApplicableNewPriceDiscount_ShouldGiveOrderLineTotalMinusTwoTimesTheDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 2));
			var orderLine = orderInfo.OrderLines.Single();
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(orderLine.GrandTotalInCents - 1000*2, actual);
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithTwoLinesTwoProductsAndApplicableNewPriceDiscount_ShouldGiveOrderLineTotalMinusFourTimesTheDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 2), DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1899, 2));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(orderInfo.OrderLineTotalInCents - 1000*4, actual);
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithOneLineTwoProductsAndApplicableNewPriceDiscountWithPerSetOfItemsCondition_ShouldGiveOrderLineTotalMinusTheDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 2));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000, DiscountOrderCondition.PerSetOfXItems, 2);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(orderInfo.OrderLineTotalInCents - 1000, actual);
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithTwoLinesTwoProductsAndApplicableNewPriceDiscountWithPerSetOfItemsCondition_ShouldGiveOrderLineTotalMinusTwoTimesTheDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 2), DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1899, 2));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000, DiscountOrderCondition.PerSetOfXItems, 2);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(orderInfo.OrderLineTotalInCents - 2000, actual);
		}

		[Test]
		public void DiscountAmountForOrderOnOrderWithTwoOrderLinesAndApplicableNewPriceDiscountWithOnTheXthItemCondition_ShouldGiveCheapestProductPriceTotalMinusTheDiscountAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 1), DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1899, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithNewPrice(1000, DiscountOrderCondition.OnTheXthItem, 2);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(1899 - 1000, actual);
		}
	}
}