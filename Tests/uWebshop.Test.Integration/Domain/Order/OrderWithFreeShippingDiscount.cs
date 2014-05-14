using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Integration.Domain.Order
{
	[TestFixture]
	public class OrderWithFreeShippingDiscount
	{
		[SetUp]
		public void Setup()
		{
			IOC.IntegrationTest();
		}
		[Test]
		public void HavingAnActiveFreeShippingDiscount_ShouldReflectThatOnOrderParts()
		{
			IOC.OrderDiscountRepository.SetupNewMock().Setup(m => m.GetAll(It.IsAny<ILocalization>())).Returns(new List<IOrderDiscount> { new OrderDiscount { DiscountType = DiscountType.FreeShipping, MinimalOrderAmount = 50000, Localization = StoreHelper.CurrentLocalization, RequiredItemIds = new List<int>(), MemberGroups = new List<string>(), AffectedOrderlines  = new List<int>()} });
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(100000, 1));
			IOC.OrderService.Resolve().UseDatabaseDiscounts(order);

			Assert.NotNull(order.OrderDiscountsFactory);
			//Assert.True(order.OrderDiscounts.Any());
			//Assert.False(order.OrderDiscountsWithoutFreeShipping.Any());
			//Assert.True(order.FreeShipping);

			var shippingProviderMethod = new ShippingProviderMethod {PriceInCents = 100};
			var adaptor = new ShippingMethodFulfillmentAdaptor(shippingProviderMethod, true, StoreHelper.CurrentLocalization, order);
			Assert.AreEqual(0, adaptor.Amount.WithVat.ValueInCents);
			Assert.AreEqual(100, adaptor.Amount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(100, adaptor.Amount.Discount.WithVat.ValueInCents);
			Assert.AreEqual(0, adaptor.Amount.WithVat.ValueInCents);
			Assert.AreEqual(100, adaptor.Amount.BeforeDiscount.WithVat.ValueInCents);

			var asf = Price.CreateDiscountedRanged(shippingProviderMethod.PriceInCents, null, true, order.AverageOrderVatPercentage, null, i => (order.FreeShipping) ? 0 : i, StoreHelper.CurrentLocalization);
			Assert.AreEqual(0, asf.WithVat.ValueInCents);
			Assert.AreEqual(100, asf.BeforeDiscount.WithVat.ValueInCents);
		}
	}
}
