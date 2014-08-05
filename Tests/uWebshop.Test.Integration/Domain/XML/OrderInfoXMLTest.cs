using System;
using NUnit.Framework;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Integration.Domain.XML
{
	[TestFixture]
	public class OrderInfoXMLTest
	{
		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
			IOC.DiscountCalculationService.Actual();
		}

		[Test]
		public void OrderHavingAConfirmDate_ShouldRenderOrderDate()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
			orderInfo.ConfirmDate = DateTime.Now;
			var xml = DomainHelper.SerializeObjectToXmlString(orderInfo);

			Assert.That(xml.Contains("<OrderDate>"));
			Assert.Greater(xml.IndexOf("</OrderDate>") - xml.IndexOf("<OrderDate>"), 11);
		}

		[Test]
		public void OrderHavingDiscount_ShouldRenderDiscountInformation()
		{
			var discount = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(10);
			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(null, discount);
			var product = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(product);
			orderInfo.SetCouponCode("bla");
			var xml = DomainHelper.SerializeObjectToXmlString(orderInfo);
			Console.WriteLine(xml);


			Assert.AreEqual(1, orderInfo.Discounts.Count);

			Assert.That(xml.Contains("<AppliedDiscountsInformation>"));
		}

		[Test]
		public void TestSerialization()
		{
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1995, 2));
			orderInfo.ShippingProviderAmountInCents = 650;
			Assert.NotNull(orderInfo.ChargedShippingCosts);
			Console.WriteLine(DomainHelper.SerializeObjectToXmlString(orderInfo));
		}
	}
}