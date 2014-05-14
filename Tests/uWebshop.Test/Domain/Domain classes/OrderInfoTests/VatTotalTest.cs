using System.Linq;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class VatTotalTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.DiscountCalculationService.Actual();
			IOC.OrderDiscountService.Actual();
		}

		[TestCase(true, 0, 1)]
		[TestCase(true, 19, 1)]
		[TestCase(true, 19, 2)]
		[TestCase(false, 0, 1)]
		[TestCase(false, 19, 1)]
		[TestCase(false, 19, 2)]
		public void OrderWithOneLine_ShouldGiveVatAmountOfProduct(bool includingVat, decimal vat, int numberOfItemsInOrderLine)
		{
			IOC.SettingsService.ExclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, numberOfItemsInOrderLine, vat);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			Assert.AreEqual(productInfo.VatAmountInCents*numberOfItemsInOrderLine, orderInfo.TotalVatInCents);
		}

		[TestCase(true, 0, 1)]
		//[TestCase(true, 10, 1)] afrondingsbug in test
		[TestCase(true, 10, 2)]
		[TestCase(false, 0, 1)]
		[TestCase(false, 10, 1)]
		[TestCase(false, 10, 2)]
		public void OrderWithTwoLinesAndDiscount_ShouldGiveVatAmountOfProductMinusOrderDiscount(bool includingVat, decimal vat, int numberOfItemsInOrderLine)
		{
			if (includingVat)
				IOC.SettingsService.InclVat();
			else
				IOC.SettingsService.ExclVat();
			var productInfo1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, numberOfItemsInOrderLine, vat);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(3500, numberOfItemsInOrderLine, vat);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo1, productInfo2);
			var line1 = orderInfo.OrderLines.First();
			var line2 = orderInfo.OrderLines.Skip(1).First();
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var totalOrderDiscount = includingVat ? orderInfo.DiscountAmountWithVatInCents : orderInfo.DiscountAmountWithoutVatInCents;

			Assert.AreEqual(includingVat, orderInfo.PricesAreIncludingVAT);
			Assert.AreEqual(vat, line1.Vat);

			Assert.AreEqual(2250 * numberOfItemsInOrderLine, totalOrderDiscount);
			var expected = includingVat || vat < 1 ? 4500 : 4950;
			Assert.AreEqual(expected * numberOfItemsInOrderLine, line1.Amount.WithVat.ValueInCents + line2.Amount.WithVat.ValueInCents);
		}

		[Test]
		public void MultipleAssertsIncludingVat()
		{
			IOC.SettingsService.InclVat();
			var productInfo1 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1100, 1, 10);
			var productInfo2 = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(3850, 1, 10);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo1, productInfo2);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.NotNull(discount.Localization);

			var d = orderInfo.DiscountAmountInCents;

			var summedParts = orderInfo.OrderLines.Sum(l => l.GetAmount(false, true, true));
			Assert.AreEqual((1000 + 3500), summedParts);

			summedParts = orderInfo.OrderDiscountEffects.GetDiscountedPrice(summedParts); // todo: add inclVat
			Assert.AreEqual((1000 + 3500) / 2, summedParts);


			Assert.AreEqual(2250, orderInfo.DiscountAmountWithoutVatInCents);
			Assert.AreEqual(4950, orderInfo.OrderLines.Sum(orderline => orderline.AmountInCents));

			Assert.AreEqual(50 + 175, orderInfo.TotalVatInCents);
			Assert.AreEqual(2475, orderInfo.OrderTotalInCents);
			Assert.AreEqual(2250, orderInfo.SubtotalInCents);
			Assert.AreEqual(2250 + 225, orderInfo.GrandtotalInCents);
			Assert.AreEqual(50 + 175, orderInfo.TotalVatInCents);
		}
	}
}