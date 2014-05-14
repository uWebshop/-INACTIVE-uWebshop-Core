using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using Moq;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculatorTests
{
	[TestFixture]
	public class HasDiscountForOrderExtension
	{
		private Mock<IOrderDiscount> _orderDiscountMock;
		private Mock<IDiscountCalculationService> _discountCalculationServiceMock;
		private OrderInfo _orderInfo;

		[SetUp]
		public void Setup()
		{
			IOC.DiscountCalculationService.Mock(out _discountCalculationServiceMock);
			_orderDiscountMock = new Mock<IOrderDiscount>();
			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
		}

		[Test]
		public void CallingHasDiscountForOrderOnListWithDiscount_ShouldCallDiscountAmountForOrderOnOrderCalculatorOnce()
		{
			var discounts = new List<IOrderDiscount> {_orderDiscountMock.Object};

			discounts.HasDiscountForOrder(_orderInfo).ToList();

			_discountCalculationServiceMock.Verify(m => m.DiscountAmountForOrder(_orderDiscountMock.Object, _orderInfo, It.IsAny<bool>()), Times.Once());
		}

		[Test]
		public void HasDiscountForOrderOnList_ShouldReturnDiscountsForWichDiscountCalculatorGivesAmountAboveZero()
		{
			var orderDiscountWithAmountForOrderMock = new Mock<IOrderDiscount>();
			_discountCalculationServiceMock.Setup(m => m.DiscountAmountForOrder(_orderDiscountMock.Object, It.IsAny<OrderInfo>(), It.IsAny<bool>())).Returns(0);
			_discountCalculationServiceMock.Setup(m => m.DiscountAmountForOrder(orderDiscountWithAmountForOrderMock.Object, It.IsAny<OrderInfo>(), It.IsAny<bool>())).Returns(1000);
			var discounts = new List<IOrderDiscount> {_orderDiscountMock.Object, orderDiscountWithAmountForOrderMock.Object};

			var filteredDiscountList = discounts.HasDiscountForOrder(_orderInfo).ToList();

			_discountCalculationServiceMock.Verify(m => m.DiscountAmountForOrder(It.IsAny<IOrderDiscount>(), _orderInfo, It.IsAny<bool>()), Times.Exactly(2));
			Assert.AreEqual(1, filteredDiscountList.Count);
			Assert.AreEqual(orderDiscountWithAmountForOrderMock.Object, filteredDiscountList.Single());
		}
	}
}