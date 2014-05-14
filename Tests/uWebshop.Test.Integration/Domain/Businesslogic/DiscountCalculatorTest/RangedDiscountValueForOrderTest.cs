using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using Moq;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;
using Range = uWebshop.Domain.Range;

namespace uWebshop.Test.Integration.Domain.Businesslogic.DiscountCalculatorTest
{
	[TestFixture]
	public class RangedDiscountValueForOrderTest
	{
		private IDiscountCalculationService _discountCalculationService;
		private OrderInfo _orderInfo;
		private ProductInfo _product1, _product2;
		private Mock<IOrderDiscount> _discountMock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.OrderService.Actual();

			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();

			_discountMock = MockConstructors.CreateDiscountMock();

			_product1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 7);
			_product1.Id = 156;
			_product2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 4);
			_product2.Id = 289;

			IOC.CMSEntityRepository.SetupFake(new UwbsNode {Id = _product1.Id, NodeTypeAlias = "uwbsProduct"}, new UwbsNode {Id = _product2.Id, NodeTypeAlias = "uwbsProduct"});

			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(_product1, _product2);
		}


		[Test]
		public void DiscountWithoutRanges_ShouldReturnDiscountValue()
		{
			_discountMock.SetupGet(m => m.DiscountValue).Returns(123);
			_discountMock.SetupGet(m => m.Ranges).Returns(new List<Range>());

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			Assert.AreEqual(123, actual);
		}

		[Test]
		public void DiscountWithRangesAndWithoutRequiredItemIdsAndAffectedItems_ShouldReturnApplicableRangeValue()
		{
			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			Assert.AreEqual(MockConstructors.DiscountMockRange3PriceInCents, actual);
		}

		[Test]
		public void DiscountWithRangesAndRequiredItemIds_ShouldReturnApplicableRangeValue()
		{
			_discountMock.SetupGet(m => m.RequiredItemIds).Returns(new List<int> {_product1.Id});

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			Assert.AreEqual(MockConstructors.DiscountMockRange2PriceInCents, actual);
		}

		[Test]
		public void DiscountWithRangesAndRequiredItemIdsAndAffectedOrderlines_ShouldReturnApplicableRangeValue()
		{
			_discountMock.SetupGet(m => m.RequiredItemIds).Returns(new List<int> {_product1.Id});
			_discountMock.SetupGet(m => m.AffectedOrderlines).Returns(new List<int> {_product2.Id});

			var actual = _discountCalculationService.RangedDiscountValueForOrder(_discountMock.Object, _orderInfo);

			Assert.AreEqual(MockConstructors.DiscountMockRange1PriceInCents, actual);
		}
	}
}