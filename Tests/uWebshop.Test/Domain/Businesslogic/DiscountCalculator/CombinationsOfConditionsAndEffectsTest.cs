using System.Linq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.OrderDTO;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculatorTests
{
	[TestFixture]
	public class CombinationsOfConditionsAndEffectsTest
	{
		private OrderInfo _orderInfo;
		private OrderDiscount _orderDiscount;

		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();

			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));

			_orderDiscount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(195, DiscountOrderCondition.None, 0);
		}

		[Test]
		public void MinimumOrderAmountAndAmountDiscountOnOrderWithSufficientAmount_ShouldAmount()
		{
			_orderDiscount.MinimalOrderAmount = 500;
			IOC.OrderDiscountRepository.SetupFake(_orderDiscount.ToDiscountOrder());

			Assert.AreEqual(195, _orderInfo.DiscountAmountInCents);
		}

		[Test]
		public void MinimumOrderAmountAndAmountDiscountOnOrderWithInsufficientAmount_ShouldNotGiveDiscount()
		{
			_orderDiscount.MinimalOrderAmount = 1500;
			IOC.OrderDiscountRepository.SetupFake(_orderDiscount.ToDiscountOrder());

			Assert.AreEqual(0, _orderInfo.DiscountAmountInCents);
		}
	}
}