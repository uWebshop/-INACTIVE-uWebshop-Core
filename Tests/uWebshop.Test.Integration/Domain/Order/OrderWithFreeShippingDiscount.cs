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
using uWebshop.Test.Stubs;
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
	    public void ExtensiveCreateDiscountedPriceRangeTest_FirstNormal()
	    {
	        // Product 400 euro
	        // 10% Vat
	        // PricesIncludingVat = false
	        // Discount = 1000

		    Assert.AreEqual(42900, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).WithVat.ValueInCents);
	        Assert.AreEqual(39000, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).WithoutVat.ValueInCents);

	        Assert.AreEqual(1100, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).Discount.WithVat.ValueInCents);
	        Assert.AreEqual(1000, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).Discount.WithoutVat.ValueInCents);
	    }

	    [Test]
        public void ExtensiveCreateDiscountedPriceRangeTest_FirstDiscount()
        {
            // Product 400 euro
            // 10% Vat
            // PricesIncludingVat = false
            // Discount = 1000

		    Assert.AreEqual(1100, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).Discount.WithVat.ValueInCents);
            Assert.AreEqual(1000, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).Discount.WithoutVat.ValueInCents);

            Assert.AreEqual(42900, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).WithVat.ValueInCents);
            Assert.AreEqual(39000, Price.CreateDiscountedRanged(40000, null, false, 10m, null, i => i - 1000, null).WithoutVat.ValueInCents);
        }

        private IDiscountCalculationService _discountCalculationService;

	    [Test]
        public void ExtensiveOrderAmountDiscountTest()
	    {
            // Product 400 euro
            // 10% Vat
            // PricesIncludingVat = false
            // Discount = 1000
            // Discount is applied over the order, NOT orderlines

			IOC.SettingsService.ExclVat();
            IOC.OrderDiscountRepository.SetupNewMock()
                .Setup(m => m.GetAll(It.IsAny<ILocalization>()))
                .Returns(new List<IOrderDiscount>
                {
                    new OrderDiscount
                    {
                        DiscountType = DiscountType.Amount,
                        DiscountValue = 1000,
                        Localization = StoreHelper.CurrentLocalization,
                        RequiredItemIds = new List<int>(),
                        MemberGroups = new List<string>(),
                        AffectedOrderlines = new List<int>()
                    }
                });
            var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(
				DefaultFactoriesAndSharedFunctionality.CreateProductInfo(40000, 1, 10m));
            order.PricesAreIncludingVAT = false;
            
            IOC.OrderService.Resolve().UseDatabaseDiscounts(order);
            
            Assert.NotNull(order.OrderDiscountsFactory);

            var iOrder = Orders.CreateBasketFromOrderInfo(order);
			//iOrder.Discounts.Single().DiscountType
            Assert.AreEqual(10, iOrder.AverageOrderVatPercentage);


			Assert.AreEqual(44000, iOrder.OrderAmount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(40000, iOrder.OrderAmount.BeforeDiscount.WithoutVat.ValueInCents);

			Assert.AreEqual(40000, order.GetOrderLineTotalAmount(false, true, false));
			Assert.AreEqual(39000, order.GetAmount(false, true, false));
			Assert.AreEqual(39000, iOrder.OrderAmount.WithoutVat.ValueInCents);
			Assert.AreEqual(39000, iOrder.OrderAmount.ValueInCents());

            Assert.AreEqual(42900, iOrder.OrderAmount.WithVat.ValueInCents); // 43000
            
            Assert.AreEqual(3900, iOrder.OrderAmount.Vat.ValueInCents);

            Assert.AreEqual(1100, iOrder.OrderAmount.Discount.WithVat.ValueInCents);
            Assert.AreEqual(1000, iOrder.OrderAmount.Discount.WithoutVat.ValueInCents);
        }

	    [Test]
		public void HavingAnActiveFreeShippingDiscount_ShouldReflectThatOnOrderParts()
		{
			IOC.OrderDiscountRepository.SetupNewMock().Setup(m => m.GetAll(It.IsAny<ILocalization>())).Returns(new List<IOrderDiscount> { new OrderDiscount { DiscountType = DiscountType.FreeShipping, MinimalOrderAmount =10000, Localization = StoreHelper.CurrentLocalization, RequiredItemIds = new List<int>(), MemberGroups = new List<string>(), AffectedOrderlines  = new List<int>()} });
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(
				DefaultFactoriesAndSharedFunctionality.CreateProductInfo(40000, 1, 10m));
			IOC.OrderService.Resolve().UseDatabaseDiscounts(order);

			//var orderDiscountService = IOC.OrderDiscountService.Resolve();
			//var orderDiscounts = orderDiscountService.GetAll(new StubLocalization());
			//var discount = orderDiscounts.Single();
			//Assert.AreEqual(DiscountType.FreeShipping, discount.Type);
		    
			//var filteredOrderDiscounts = orderDiscounts.Where(orderDiscount => !orderDiscount.Disabled && 40000 >= orderDiscount.MinimumOrderAmount.ValueInCents() && (!orderDiscount.RequiredItemIds.Any() /*|| _orderService.OrderContainsItem(order, orderDiscount.RequiredItemIds)*/) && (!orderDiscount.CounterEnabled || orderDiscount.Counter > 0));
			//Assert.AreEqual(1, filteredOrderDiscounts.Count());

			//var applicableDiscounts = orderDiscountService.GetApplicableDiscountsForOrder(order, order.Localization);
			//Assert.True(applicableDiscounts.Any());
			//Assert.AreEqual(DiscountType.FreeShipping, applicableDiscounts.Single().Type);

			Assert.NotNull(order.OrderDiscountsFactory);
			Assert.True(order.OrderDiscounts.Any());
			//Assert.False(order.OrderDiscountsWithoutFreeShipping.Any());
			Assert.True(order.FreeShipping);

			var shippingProviderMethod = new ShippingProviderMethod {PriceInCents = 1000};
			var adaptor = new ShippingMethodFulfillmentAdaptor(shippingProviderMethod, true, StoreHelper.CurrentLocalization, order);

            // freeshipping so chardgedshippingcosts should be 0
		    Assert.AreEqual(0, order.ChargedShippingCostsInCents);

			Assert.AreEqual(0, adaptor.Amount.WithVat.ValueInCents);
			Assert.AreEqual(1000, adaptor.Amount.BeforeDiscount.WithVat.ValueInCents);
			Assert.AreEqual(1000, adaptor.Amount.Discount.WithVat.ValueInCents);
			Assert.AreEqual(0, adaptor.Amount.WithVat.ValueInCents);
			//Assert.AreEqual(1000, adaptor.Amount.BeforeDiscount.WithVat.ValueInCents);// wtf

			var asf = Price.CreateDiscountedRanged(shippingProviderMethod.PriceInCents, null, true, order.AverageOrderVatPercentage, null, i => (order.FreeShipping) ? 0 : i, StoreHelper.CurrentLocalization);
			Assert.AreEqual(0, asf.WithVat.ValueInCents);
			Assert.AreEqual(1000, asf.BeforeDiscount.WithVat.ValueInCents);
		}

        [Test]
        public void HavingAnActiveFreeShippingDiscount_ShouldGiveDiscountAmountEqualToShippingCosts()
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
