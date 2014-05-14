using System.Linq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Domain.Domain_classes.OrderDiscounts
{
	[TestFixture]
	public class ShippingsCosts
	{
		private IDiscountCalculationService _discountCalculationService;

		[TestFixtureSetUp]
		public void SetUp()
		{
			IOC.UnitTest();
			IOC.SettingsService.InclVat();
			IOC.OrderDiscountService.Actual();
			//IOC.Config<IDiscountCalculator>().Actual(); 
			//IOC.DiscountCalculator.Actual(); _discountCalculator = IOC.DiscountCalculator.Resolve();
			_discountCalculationService = IOC.DiscountCalculationService.Actual().Resolve();
		}

		[Test]
		public void FreeShippingDiscount_ShouldGiveDiscountAmountEqualToShippingCosts()
		{
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithFreeShipping();
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);
			orderInfo.ShippingProviderAmountInCents = 195;


			//Discounts.Any(discount => discount.DiscountType == DiscountType.FreeShipping);
			Assert.AreEqual(discount.Type, Common.DiscountType.FreeShipping);
			Assert.IsTrue(orderInfo.Discounts.Any(d => d.DiscountType == DiscountType.FreeShipping));

			Assert.IsTrue(orderInfo.FreeShipping);
			Assert.AreEqual(0, orderInfo.ChargedShippingCostsInCents);
			Assert.AreEqual(0, orderInfo.DiscountAmountInCents);
		}
	}
}