using NUnit.Framework;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.AppliedDiscountsInformationTest
{
	[TestFixture]
	public class AppliedDiscountsInformationTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.OrderDiscountService.Actual();
			IOC.DiscountCalculationService.Actual();
		}

		[Test]
		public void OrderInfoWithApplicableDiscount_ShouldReturnInformationOnThatDiscount()
		{
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(10);
			IOC.OrderDiscountRepository.SetupFake(new IOrderDiscount[]{discount.ToDiscountOrder()});

			Assert.AreEqual(1, orderInfo.AppliedDiscountsInformation.Count);
		}
	}
}