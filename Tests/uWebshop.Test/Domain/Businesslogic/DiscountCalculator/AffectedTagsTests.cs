using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculator
{
	[TestFixture]
	public class AffectedTagsTests
	{
		private IDiscountCalculationService _discountCalculationService;
		private ProductInfo _product;
		private OrderInfo _order;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();

			_product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			_product.Tags = new [] { "schoen" };
			_order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(_product);

		}

		[Test]
		public void DiscountAmountForOrder_TagsMatching_GivesDiscount()
		{
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			discount.AffectedProductTags = new[]{"schoen"};

			var amount = _discountCalculationService.DiscountAmountForOrder(discount, _order);

			Assert.AreEqual(500, amount);
		}

		[Test]
		public void DiscountAmountForOrder_TagsNotMatching_GivesZeroDiscount()
		{
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			discount.AffectedProductTags = new[] { "jas" };

			var amount = _discountCalculationService.DiscountAmountForOrder(discount, _order);

			Assert.AreEqual(0, amount);
		}

		[Test]
		public void Integration()
		{
			var discountMock = new Mock<IOrderDiscount>();
			discountMock.SetupGet(m => m.MemberGroups).Returns(new List<string>());
			discountMock.SetupGet(m => m.AffectedOrderlines).Returns(new List<int>());

			_discountCalculationService.DiscountAmountForOrder(discountMock.Object, _order);

			discountMock.VerifyGet(m => m.AffectedProductTags);
		}
	}
}
