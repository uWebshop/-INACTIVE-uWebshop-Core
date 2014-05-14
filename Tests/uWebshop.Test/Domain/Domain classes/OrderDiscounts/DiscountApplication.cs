using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.OrderDTO;
using uWebshop.Test.SharedFunctionality;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class DiscountApplication
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			DefaultFactoriesAndSharedFunctionality.OverrideGlobalSettingsIncludingVat(true);
		}

		[Test]
		public void AddingACouponToAnIncompleteOrderAfterDeserialization_ShouldApplyCouponDiscount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(100, DiscountOrderCondition.None, 0);
			discount.CouponCode = "coupon";
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.AreEqual(0, orderInfo.DiscountAmountInCents);
			// fake deserialization (for sake of speed)
			orderInfo.DiscountService = new DiscountService(new ListRepository<IOrderDiscount>(orderInfo.OrderDiscounts.Select(disc => new OrderDiscount(disc))));

			Assert.AreEqual(0, orderInfo.DiscountAmountInCents);

			orderInfo.CouponCodes.Add("coupon");

			Assert.AreEqual(100, orderInfo.DiscountAmountInCents);
		}
	}
}
