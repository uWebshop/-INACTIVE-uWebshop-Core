using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using Moq;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class MembershipTest
	{
		private IDiscountCalculationService _discountCalculationService;
		private OrderDiscount _discount;
		private OrderInfo _orderInfo;
		private Mock<IAuthenticationProvider> _mock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.SettingsService.InclVat();
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();
			_discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			_discount.MemberGroups = new List<string> {"testGroup"};
			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(
				DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(_orderInfo, _discount);
			_mock = IOC.AuthenticationProvider.SetupNewMock();
		}

		[Test]
		public void DiscountWithMembergroupShouldGiveDiscountForUserInThatGroup()
		{
			// Arrange
			_mock.SetupGet(a => a.RolesForCurrentUser).Returns(new List<string> {"testGroup"});

			// Act
			var discountAmount = _discountCalculationService.DiscountAmountForOrder(_discount, _orderInfo);

			// Assert
			Assert.AreEqual(500, discountAmount);
		}

		[Test]
		public void DiscountWithMembergroupShouldGiveNoDiscountForUserNotInThatGroup()
		{
			// Arrange
			_mock.SetupGet(a => a.RolesForCurrentUser).Returns(new List<string> {"anotherGroup"});

			// Act
			var discountAmount = _discountCalculationService.DiscountAmountForOrder(_discount, _orderInfo);

			// Assert
			Assert.AreEqual(0, discountAmount);
		}
	}
}