using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;

namespace uWebshop.Test.Domain.Businesslogic.DiscountCalculator
{
    [TestFixture]
    public class CouponsTests
    {
        [Test]
        public void bla()
        {
            IOC.IntegrationTest();
            IOC.CouponCodeService.SetupNewMock().Setup(m => m.GetAllForDiscount(1234)).Returns(new List<Coupon> { new Coupon(1234, "code", 7), });


            var a = IOC.DiscountCalculationService.Actual().Resolve();
            var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(1000, DiscountOrderCondition.None, 0);
            discount.Id = 1234;
            DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(null, discount);
            var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2199, 2));
            order.CouponCodesData.Add("code");

            var actual = a.DiscountAmountForOrder(discount, order);

            Assert.AreEqual(1000, actual);

            Assert.NotNull(order.OrderDiscountsFactory);

            var discountlist = IOC.OrderDiscountService.Resolve().GetApplicableDiscountsForOrder(order, order.Localization);
            Assert.AreEqual(1, discountlist.Count());

            var d = order.Discounts.FirstOrDefault();
            Assert.NotNull(d);


            var basket = Basket.CreateBasketFromOrderInfo(order);
            var disc = basket.Discounts.FirstOrDefault();
            Assert.NotNull(disc);
            Assert.AreEqual("code", disc.CouponCode);
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
        public int Id { get; private set; }
        public int DiscountId { get; private set; }
        public string CouponCode { get; private set; }
        public int NumberAvailable { get; private set; }
        public Guid uniqueID { get; }
    }
}