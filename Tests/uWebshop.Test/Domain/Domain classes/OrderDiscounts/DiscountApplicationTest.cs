using NUnit.Framework;
using uWebshop.Common;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class DiscountApplicationTest
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			IOC.UnitTest();
			IOC.OrderDiscountService.Actual();
			IOC.DiscountCalculationService.Actual();
			IOC.SettingsService.InclVat();
		}

		[Test]
		public void AddingACouponToAnIncompleteOrderAfterDeserialization_ShouldApplyCouponDiscount()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(100, DiscountOrderCondition.None, 0);
			discount.CouponCode = "coupon";
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.AreEqual(0, orderInfo.DiscountAmountInCents);

			Assert.AreEqual(0, orderInfo.DiscountAmountInCents);

			Assert.False(orderInfo.LegacyDataReadBackMode);
			orderInfo.SetCouponCode("coupon");

			//Assert.False(orderInfo._discountAmountInCents.HasValue);
			Assert.AreEqual(100, orderInfo.DiscountAmountInCents);
		}
	}
}