using System.Collections.Generic;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.Discounts
{
	[TestFixture]
	public class PerSetOfXItemsTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.DiscountCalculationService.Actual();
			IOC.SettingsService.InclVat();
			IOC.OrderDiscountService.Actual();
		}

		[TestCase(3, 1, 0)]
		[TestCase(3, 2, 0)]
		[TestCase(3, 3, 3)]
		[TestCase(3, 4, 3)]
		[TestCase(3, 5, 3)]
		[TestCase(3, 6, 6)]
		[TestCase(3, 7, 6)]
		public void ThatPercentDiscountIsAppliedToCorrectNumberOfItemsPercentageDiscount(int setSize, int itemCount, int expectedNumberOfDiscountedItems)
		{
			var productInfo = new ProductInfo();
			productInfo.IsDiscounted = false;
			productInfo.OriginalPriceInCents = 1000;
			productInfo.Ranges = new List<Range>();
			productInfo.ItemCount = itemCount;

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentageList(50, DiscountOrderCondition.PerSetOfXItems, setSize);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount.ToArray());

			Assert.AreEqual(expectedNumberOfDiscountedItems*500 + (itemCount - expectedNumberOfDiscountedItems)*1000, orderInfo.OrderTotalInCents);
		}

		[TestCase(3, 1, 0)]
		[TestCase(3, 2, 0)]
		[TestCase(3, 3, 1)]
		[TestCase(3, 4, 1)]
		[TestCase(3, 5, 1)]
		[TestCase(3, 6, 2)]
		[TestCase(3, 7, 2)]
		public void ThatPercentDiscountIsAppliedToCorrectNumberOfItemsFixedDiscount(int setSize, int itemCount, int expectedNumberOfDiscounts)
		{
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, itemCount);

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(50, DiscountOrderCondition.PerSetOfXItems, setSize);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.AreEqual(itemCount*1000 - expectedNumberOfDiscounts*50, orderInfo.OrderTotalInCents);
		}
	}
}