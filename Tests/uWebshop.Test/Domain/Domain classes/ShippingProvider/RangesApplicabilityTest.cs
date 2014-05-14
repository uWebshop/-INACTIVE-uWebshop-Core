using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class ShippingProviderRangesApplicabilityTest
	{
		[Test]
		public void ShippingProviderShouldNotBeApplicableWhenEndRangeEqualsOrderlinesAmount_WhenIncludingVat()
		{
			IOC.SettingsService.InclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);
			var provider = DefaultFactoriesAndSharedFunctionality.CreateShippingProvider(0, 1000, ShippingRangeType.OrderAmount);

			Assert.IsFalse(provider.IsApplicableToOrder(orderInfo));
		}

		[Test]
		public void ShippingProviderShouldNotBeApplicableWhenEndRangeEqualsOrderlinesAmount_WhenExcludingVat()
		{
			IOC.SettingsService.ExclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);
			var provider = DefaultFactoriesAndSharedFunctionality.CreateShippingProvider(0, 1000, ShippingRangeType.OrderAmount);

			Assert.IsFalse(provider.IsApplicableToOrder(orderInfo));
		}

		[Test]
		public void RecreateBugReport20121025()
		{
			IOC.SettingsService.InclVat();
			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(2650, 1);
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo(productInfo);

			var providerFee = DefaultFactoriesAndSharedFunctionality.CreateShippingProvider(1, 6000, ShippingRangeType.OrderAmount);
			var providerGratis = DefaultFactoriesAndSharedFunctionality.CreateShippingProvider(6000, 99900, ShippingRangeType.OrderAmount);

			Assert.IsFalse(providerGratis.IsApplicableToOrder(orderInfo));
			Assert.IsTrue(providerFee.IsApplicableToOrder(orderInfo));
		}
	}
}