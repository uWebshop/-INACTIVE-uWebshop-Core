using System.Linq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Repositories;

namespace uWebshop.Test.Integration.RegressionTests
{
	[TestFixture]
	public class _20130620_orderdiscount_with_coupon_and_hundred_percent_had_eror_in_DiscountamountForOrder
	{
		[Test]
		public void orderdiscount_with_coupon_and_hundred_percent_had_eror_in_DiscountamountForOrder()
		{
			IOC.IntegrationTest();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(12990, 1);
			productInfo.Id = TestProductService.ProductId1;
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);

			var discount1 = DefaultFactoriesAndSharedFunctionality.CreateDefaultOrderDiscountWithPercentage(100);
			discount1.CouponCode = "coup";
			IOC.OrderDiscountRepository.SetupFake(discount1.ToDiscountOrder());

			DefaultFactoriesAndSharedFunctionality.SetDiscountsOnOrderInfo(orderInfo, discount1);
			orderInfo.SetCouponCode("coup");

			Assert.AreEqual(12990, orderInfo.OrderLines.Sum(orderline => orderline.AmountInCents));
			Assert.AreEqual(12990, orderInfo.Discounts.Sum(d => IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(d, orderInfo)));

			Assert.AreEqual(12990, orderInfo.DiscountAmountInCents);
			Assert.AreEqual(0, orderInfo.OrderTotalInCents);

			Assert.NotNull(orderInfo.OrderDiscountsFactory);

			var orderService = IO.Container.Resolve<IOrderService>();
			var discountRepository = IO.Container.Resolve<IOrderDiscountRepository>();

			var orderDiscounts = discountRepository.GetAll(orderInfo.Localization);
			Assert.IsTrue(orderDiscounts.Any());

            var orderLinesAmount = orderInfo.OrderLines.Sum(orderline => orderline.GrandTotalInCents);
			Assert.AreEqual(12990, orderLinesAmount);

			var discount = orderDiscounts.First();

			Assert.IsTrue(!discount.Disabled && orderLinesAmount >= discount.MinimumOrderAmount.ValueInCents() && (!discount.RequiredItemIds.Any() || orderService.OrderContainsItem(orderInfo, discount.RequiredItemIds)) && (!discount.CounterEnabled || discount.Counter > 0));

			Assert.IsFalse(!string.IsNullOrEmpty(discount.CouponCode) && !orderInfo.CouponCodes.Contains(discount.CouponCode));

			var discountService = IO.Container.Resolve<IOrderDiscountService>();

			//var orderDiscounts = GetAll(localization);
			//var orderLinesAmount = order.OrderLines.Sum(orderline => orderline.AmountInCents);

			//return orderDiscounts.Where(discount => !discount.Disabled && orderLinesAmount >= discount.MinimumOrderAmount.ValueInCents && (!discount.RequiredItemIds.Any()
			//	|| _orderService.OrderContainsItem(order, discount.RequiredItemIds)) && (!discount.CounterEnabled || discount.Counter > 0)).HasDiscountForOrder(order).ToList();
			Assert.IsTrue(discountService.GetAll(orderInfo.Localization).Any());
			Assert.AreEqual(12990, orderInfo.OrderLines.Sum(orderline => orderline.AmountInCents));
			var dit = discountService.GetAll(orderInfo.Localization).First();
			Assert.IsTrue(!dit.Disabled);
			Assert.AreEqual(0, dit.MinimumOrderAmount.ValueInCents());
			Assert.IsFalse(dit.RequiredItemIds.Any());
			Assert.IsFalse(dit.CounterEnabled);

			Assert.IsTrue(IO.Container.Resolve<IDiscountCalculationService>().DiscountAmountForOrder(dit, orderInfo) > 0);

			orderInfo.ResetDiscounts();
			Assert.IsTrue(discountService.GetApplicableDiscountsForOrder(orderInfo, orderInfo.Localization).Any());

			Assert.IsTrue(orderInfo.OrderDiscountsFactory().Any());
		}
	}
}