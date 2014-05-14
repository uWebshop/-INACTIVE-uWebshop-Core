using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Domain.Domain_classes.OrderInfoTests
{
	[TestFixture]
	public class GrandTotalTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			IOC.DiscountCalculationService.Actual();
			IOC.OrderDiscountService.Actual();
		}

		[Test]
		public void ThatVATAmountIsRoundedDown()
		{
			var productInfo = new ProductInfo {IsDiscounted = false, Vat = 19, ItemCount = 2, OriginalPriceInCents = 3500, Ranges = new List<Range>()};

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			Assert.AreEqual(19, orderInfo.AverageOrderVatPercentage);
			Assert.AreEqual(559, productInfo.VatAmountInCents);
			Assert.AreEqual(2941, productInfo.PriceWithoutVatInCents);
			Assert.AreEqual(3500, productInfo.PriceWithVatInCents);
			Assert.AreEqual(7000, orderInfo.OrderTotalInCents);
			Assert.AreEqual(5882, orderInfo.SubtotalInCents);
			Assert.AreEqual(7000, orderInfo.GrandtotalInCents);
		}

		//[Test]
		//public void ThatVATAmountIsRoundedDownDiscounted()
		//{
		//	IOC.SettingsService.ExclVat();
		//	IOC.VatCalculationStrategy.OverTotal();

		//	var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(100, DiscountOrderCondition.None, 0);
		//	var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2942, 2);
		//	var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
		//	DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

		//	Assert.AreEqual(100, orderInfo.DiscountAmountInCents);
		//	Assert.AreEqual(0, orderInfo.OrderLines.Sum(oline => oline.DiscountInCents));

		//	Assert.AreEqual(559, productInfo.VatAmountInCents);
		//	Assert.AreEqual(2942, productInfo.PriceWithoutVatInCents);
		//	Assert.AreEqual(3501, productInfo.PriceWithVatInCents);


		//	Assert.AreEqual(5784, orderInfo.OrderTotalInCents);
		//	Assert.AreEqual(5784, orderInfo.SubtotalInCents);
		//	Assert.AreEqual(6883, orderInfo.GrandtotalInCents);
		//	Assert.AreEqual(1099, orderInfo.TotalVatInCents);
		//}

		[Test]
		public void GrandTotalWithVariants()
		{
			var productInfo = new ProductInfo {IsDiscounted = false, Vat = 6, OriginalPriceInCents = 1000, Ranges = new List<Range>()};
			productInfo.ProductVariants.Add(new ProductVariantInfo {PriceInCents = 100});

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			Assert.IsTrue(orderInfo.PricesAreIncludingVAT);
			Assert.AreEqual(1100, orderInfo.GrandtotalInCents);
		}

		[Test]
		public void GrandTotalWithOrderDiscount()
		{
			var productInfo = new ProductInfo {IsDiscounted = false, Vat = 6, OriginalPriceInCents = 1000, Ranges = new List<Range>()};

			productInfo.ProductVariants.Add(new ProductVariantInfo {PriceInCents = 100});

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.AreEqual(550, orderInfo.OrderTotalInCents);
			Assert.AreEqual(6, orderInfo.AverageOrderVatPercentage);
			Assert.AreEqual(519, orderInfo.SubtotalInCents);
			Assert.AreEqual(550, orderInfo.GrandtotalInCents);

			Assert.AreEqual(1100, orderInfo.OrderLines.First().GrandTotalInCents);
		}

		[Test]
		public void GrandTotalWithOrderDiscoun214t()
		{
			IOC.OrderService.Actual();

			var productInfo = new ProductInfo { Id = 1234, IsDiscounted = false, Vat = 0, OriginalPriceInCents = 1000, Ranges = new List<Range>() };
			IOC.CMSEntityRepository.SetupNewMock().Setup(m => m.GetByGlobalId(1234)).Returns(new UwbsNode {Id = 1234, NodeTypeAlias = Product.NodeAlias});

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithAmount(10, DiscountOrderCondition.None, 0);
			discount.AffectedOrderlines = new List<int>{1234};
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			Assert.AreEqual(10, orderInfo.DiscountAmountInCents);

			Assert.AreEqual(990, orderInfo.GrandtotalInCents);
			Assert.AreEqual(990, orderInfo.OrderTotalInCents);
			Assert.AreEqual(0, orderInfo.AverageOrderVatPercentage);
			Assert.AreEqual(990, orderInfo.SubtotalInCents);
		}


		[Test]
		public void SomeRegressionTest()
		{
			var productInfo = new ProductInfo {IsDiscounted = false, Vat = 6, OriginalPriceInCents = 1000, Ranges = new List<Range>()};

			productInfo.ProductVariants.Add(new ProductVariantInfo {PriceInCents = -100});

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(50);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount);

			var line = orderInfo.OrderLines.Single();
			var sellableUnit = orderInfo.OrderLines.Single().SellableUnits.Single();

			Assert.AreEqual(900, sellableUnit.PriceInCents);
			Assert.AreEqual(51, line.VatAmountInCents);
			Assert.AreEqual(849, line.SubTotalInCents);

			Assert.AreEqual(425, orderInfo.SubtotalInCents);
			
			Assert.AreEqual(450, orderInfo.OrderTotalInCents);
			Assert.AreEqual(6, orderInfo.AverageOrderVatPercentage);
			Assert.AreEqual(450, orderInfo.GrandtotalInCents);

			Assert.AreEqual(900, orderInfo.OrderLines.First().GrandTotalInCents);
			Assert.AreEqual(900, productInfo.PriceWithVatInCents);

		}

		[Test]
		public void Snelheid()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(195, 1));
			var iets = orderInfo.GrandtotalInCents; // 212 ms !!
		}
	}
}