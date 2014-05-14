using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Domain.Businesslogic.DiscountService
{
	[Ignore]
	[TestFixture]
	public class GetApplicableDiscountsForOrderTest
	{
		private OrderInfo _orderInfo;
		private Mock<IOrderDiscount> _mockDiscount;
		private IOrderDiscountService _discountService;
		private Mock<IOrderService> _mockOrderService;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.OrderService.Mock(out _mockOrderService);
			_mockOrderService.Setup(m => m.OrderContainsItem(It.IsAny<OrderInfo>(), It.IsAny<List<int>>())).Returns(true);

			IOC.DiscountService.Actual();
			IOC.OrderDiscountService.Actual();
			IOC.DiscountCalculationService.SetupNewMock().Setup(m => m.DiscountAmountForOrder(It.IsAny<IOrderDiscount>(), It.IsAny<OrderInfo>(), It.IsAny<bool>())).Returns(1);

			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));

			_mockDiscount = new Mock<IOrderDiscount>();
			_mockDiscount.SetupGet(m => m.CounterEnabled).Returns(false);
			_mockDiscount.SetupGet(m => m.RequiredItemIds).Returns(new List<int>());

			// todo: fix
			//IOC.OrderDiscountRepository.SetupFake(_mockDiscount.Object);

			_discountService = IOC.OrderDiscountService.Actual().Resolve();
		}
		class FakePrice : IVatPrice, IPrice
		{
			private readonly int _price;

			public FakePrice(int price)
			{
				_price = price;
			}

			public decimal Value { get; private set; }
			public int ValueInCents { get { return _price; } }
			public string ToCurrencyString()
			{
				return string.Empty;
			}

			public IPrice WithVat { get { return this; } }
			public IPrice WithoutVat { get { return this; } }
			public IPrice Vat { get { return this; } }
		}
		[Test]
		public void OrderAmountAboveMinimum_ShouldReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.MinimumOrderAmount).Returns(new FakePrice(500));

			Assert.AreEqual(_mockDiscount.Object, _discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Single());
		}

		[Test]
		public void OrderAmountBelowMinimum_ShouldNotReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.MinimumOrderAmount).Returns(new FakePrice(1500));

			Assert.False(_discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Any());
		}

		[Test]
		public void CounterEnabledFalse_ShouldReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.CounterEnabled).Returns(false);

			Assert.AreEqual(_mockDiscount.Object, _discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Single());
			_mockDiscount.VerifyGet(m => m.CounterEnabled, Times.AtLeastOnce());
			_mockDiscount.VerifyGet(m => m.Counter, Times.Never());
		}

		[Test]
		public void CounterEnabledTrueAndCounterAboveZero_ShouldReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.CounterEnabled).Returns(true);
			_mockDiscount.SetupGet(m => m.Counter).Returns(1);

			Assert.AreEqual(_mockDiscount.Object, _discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Single());
			_mockDiscount.VerifyGet(m => m.CounterEnabled, Times.AtLeastOnce());
			_mockDiscount.VerifyGet(m => m.Counter, Times.AtLeastOnce());
		}

		[Test]
		public void CounterEnabledTrueAndCounterZero_ShouldNotReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.CounterEnabled).Returns(true);
			_mockDiscount.SetupGet(m => m.Counter).Returns(0);

			Assert.False(_discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Any());
			_mockDiscount.VerifyGet(m => m.CounterEnabled, Times.AtLeastOnce());
			_mockDiscount.VerifyGet(m => m.Counter, Times.AtLeastOnce());
		}

		[Test]
		public void RequiredItemIdsEmpty_ShouldReturnDiscount()
		{
			_mockDiscount.SetupGet(m => m.RequiredItemIds).Returns(new List<int>());

			Assert.AreEqual(_mockDiscount.Object, _discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Single());
			_mockDiscount.VerifyGet(m => m.RequiredItemIds, Times.AtLeastOnce());
		}

		[Test]
		public void RequiredItemIdsWithOrderContainsItemTrue_ShouldReturnDiscount()
		{
			var itemIds = new List<int> {1234};
			_mockDiscount.SetupGet(m => m.RequiredItemIds).Returns(itemIds);

			var discount = _discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Single();

			Assert.AreEqual(_mockDiscount.Object, discount);
			_mockDiscount.VerifyGet(m => m.RequiredItemIds, Times.AtLeastOnce());
			_mockOrderService.Verify(m => m.OrderContainsItem(_orderInfo, itemIds), Times.Once());
		}

		[Test]
		public void RequiredItemIdsWithOrderContainsItemFalse_ShouldNotReturnDiscount()
		{
			var itemIds = new List<int> {1234};
			_mockDiscount.SetupGet(m => m.RequiredItemIds).Returns(itemIds);
			_mockOrderService.Setup(m => m.OrderContainsItem(It.IsAny<OrderInfo>(), It.IsAny<List<int>>())).Returns(false);

			Assert.False(_discountService.GetApplicableDiscountsForOrder(_orderInfo, _orderInfo.Localization).Any());
			_mockDiscount.VerifyGet(m => m.RequiredItemIds, Times.AtLeastOnce());
			_mockOrderService.Verify(m => m.OrderContainsItem(_orderInfo, itemIds), Times.Once());
		}
	}
}