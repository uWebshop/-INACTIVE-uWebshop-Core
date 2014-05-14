using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class NoConditionsTest
	{
		private IDiscountCalculationService _discountCalculationService;

		[SetUp]
		public void SetUp()
		{
			IOC.UnitTest();
			IOC.OrderDiscountService.Actual();
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();
		}

		[Test]
		public void Percentage()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(500, actual);
		}

		[Test]
		public void Percentage100()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(100);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(1000, actual);
		}

		[Test]
		public void Amount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(234, DiscountOrderCondition.None, 0);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(234, actual);
		}

		[Test]
		public void AmountLargerThanOrderAmount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(1234, DiscountOrderCondition.None, 0);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var actual = _discountCalculationService.DiscountAmountForOrder(discount, orderInfo);

			Assert.AreEqual(1000, actual);
		}

		[Test]
		public void CombinedAmountAndPercentage()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(234, DiscountOrderCondition.None, 0), DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50));

			Assert.AreEqual(734, orderInfo.DiscountAmountInCents);
		}

		[Test]
		public void CombinedTooLargeAmountAndPercentage()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(1234, DiscountOrderCondition.None, 0), DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50));
			Assert.AreEqual(1000, orderInfo.DiscountAmountInCents);
		}
		
		[Test]
		public void CombinedTooLargeAmountAndPercentageOrderLineTotalInCentsShouldBeNull()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(1234, DiscountOrderCondition.None, 0), DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50));
			Assert.AreEqual(1000, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(1000, orderInfo.OrderLineTotalWithVatInCents);
			Assert.AreEqual(0, orderInfo.GrandtotalInCents);
			Assert.AreEqual(0, orderInfo.OrderTotalInCents);
		}

		[Test]
		public void RecreateBug20121127()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(21306, 2, 21), DefaultFactoriesAndSharedFunctionality.CreateProductInfo(3995, 1, 0));
			var defaultOrderDiscountWithAmount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(50000, DiscountOrderCondition.None, 0);
			Assert.NotNull(defaultOrderDiscountWithAmount.AffectedProductTags);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, defaultOrderDiscountWithAmount, DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(25));
			Assert.AreEqual(46607, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(46607, orderInfo.DiscountAmountWithVatInCents);
			//Assert.AreEqual(0, orderInfo.OrderLineTotalWithVatInCents);
			// Assert.AreEqual(46607, orderInfo.OrderLineTotalInCents);
			Assert.AreEqual(0, orderInfo.OrderTotalInCents);
		}
	}
}