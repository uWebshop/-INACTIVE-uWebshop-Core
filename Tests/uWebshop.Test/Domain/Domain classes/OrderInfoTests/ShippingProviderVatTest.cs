using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class ShippingProviderVatTest
	{
		[Test]
		public void ThatVatRelatedPricesAreCorrectlyCalculatedIncludingVat()
		{
			IOC.UnitTest();
			IOC.SettingsService.InclVat();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 6);

			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);
			orderInfo.ShippingProviderAmountInCents = 100;

			Assert.AreEqual(100, orderInfo.ShippingProviderAmountInCents);
			Assert.AreEqual(100, orderInfo.ShippingProviderCostsWithVatInCents);
			Assert.AreEqual(94, orderInfo.ShippingProviderCostsWithoutVatInCents);
			Assert.AreEqual(6, orderInfo.ShippingProviderVatAmountInCents);
		}

		[Test]
		public void ThatVatRelatedPricesAreCorrectlyCalculatedExcludingVat()
		{
			IOC.UnitTest();
			IOC.SettingsService.ExclVat();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1, 6);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);
			orderInfo.ShippingProviderAmountInCents = 94;

			Assert.AreEqual(94, orderInfo.ShippingProviderAmountInCents);
			Assert.AreEqual(100, orderInfo.ShippingProviderCostsWithVatInCents);
			Assert.AreEqual(94, orderInfo.ShippingProviderCostsWithoutVatInCents);
			Assert.AreEqual(6, orderInfo.ShippingProviderVatAmountInCents);
		}
	}
}