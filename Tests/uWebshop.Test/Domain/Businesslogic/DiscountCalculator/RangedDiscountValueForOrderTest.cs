using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Domain;
using Moq;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculatorTests
{
	[TestFixture]
	public class RangedDiscountValueForOrderTest
	{
		private Mock<IOrderService> _orderServiceMock = new Mock<IOrderService>();
		private Mock<IOrderDiscount> _discountMock = new Mock<IOrderDiscount>();
		private IDiscountCalculationService _discountCalculationService;
		private OrderInfo _orderInfo;
		private ProductInfo _product1, _product2;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.OrderService.Mock(out _orderServiceMock);
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();

			_discountMock = MockConstructors.CreateDiscountMock();

			_product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 7);
			_product2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 4);
			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(_product1, _product2);
			_orderServiceMock.Setup(m => m.GetApplicableOrderLines(_orderInfo, It.IsAny<List<int>>())).Returns(_orderInfo.OrderLines);
		}

		[Test]
		public void DiscountWithoutRanges_ShouldReturnDiscountValue()
		{
			_discountMock.SetupGet(m => m.DiscountValue).Returns(123);
			_discountMock.SetupGet(m => m.Ranges).Returns(new List<Range>());

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			_orderServiceMock.Verify(m => m.GetApplicableOrderLines(_orderInfo, It.IsAny<List<int>>()), Times.Never());
			Assert.AreEqual(123, actual);
		}

		[Test]
		public void DiscountWithRangesAndWithoutRequiredItemIdsAndAffectedItems_ShouldReturnApplicableRangeValue()
		{
			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			Assert.AreEqual(MockConstructors.DiscountMockRange3PriceInCents, actual);
		}

		[Test]
		public void DiscountWithRangesAndRequiredItemIds_ShouldCallGetApplicableOrderLinesWithRequiredItemIds()
		{
			_discountMock.SetupGet(m => m.RequiredItemIds).Returns(new List<int> {2342});

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			_orderServiceMock.Verify(m => m.GetApplicableOrderLines(_orderInfo, new List<int> {2342}), Times.Once());
		}

		[Test]
		public void DiscountWithRangesAndRequiredItemIdsAndAffectedOrderlines_ShouldCallGetApplicableOrderLinesWithAffectedOrderlines()
		{
			_discountMock.SetupGet(m => m.RequiredItemIds).Returns(new List<int> {2342});
			_discountMock.SetupGet(m => m.AffectedOrderlines).Returns(new List<int> {4636});

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			_orderServiceMock.Verify(m => m.GetApplicableOrderLines(_orderInfo, new List<int> {2342}), Times.Never());
			_orderServiceMock.Verify(m => m.GetApplicableOrderLines(_orderInfo, new List<int> {4636}), Times.Once());
		}
	}
}