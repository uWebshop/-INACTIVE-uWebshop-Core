using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculator
{
	[TestFixture]
	public class CouponsTests
	{
		[Test]
		public void bla()
		{
			IOC.UnitTest();
			IOC.CouponCodeService.SetupNewMock().Setup(m => m.GetAllForDiscount(1234)).Returns(new List<Coupon> {new Coupon(1234, "code", 7),});

			var a = IOC.DiscountCalculationService.Actual().Resolve();
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(1000, DiscountOrderCondition.None, 0);
			discount.Id = 1234;
			var order = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2199, 2));
			order.CouponCodesData.Add("code");

			var actual = a.DiscountAmountForOrder(discount, order);

			Assert.AreEqual(1000, actual);
		}
	}

	public class Coupon : ICoupon
	{
		public Coupon(int discountId, string couponCode, int numberAvailable)
		{
			DiscountId = discountId;
			CouponCode = couponCode;
			NumberAvailable = numberAvailable;
		}
		public int DiscountId { get; private set; }
		public string CouponCode { get; private set; }
		public int NumberAvailable { get; private set; }
	}
}