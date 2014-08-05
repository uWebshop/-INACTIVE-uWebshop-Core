using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic.VATChecking;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.OrderDTO;
using uWebshop.Test.Stubs;
using OrderLine = uWebshop.Domain.OrderLine;

namespace uWebshop.Test
{
	internal static class DefaultFactoriesAndSharedFunctionality
	{
		public static ShippingProvider CreateShippingProvider(int rangeFrom, int rangeTo, ShippingRangeType typeOfRange)
		{
			var providerFree = new ShippingProvider();
			providerFree.RangeFrom = rangeFrom;
			providerFree.RangeTo = rangeTo;
			providerFree.TypeOfRange = typeOfRange;
			return providerFree;
		}

		public static OrderInfo CreateOrderInfo(params ProductInfo[] productInfo)
		{
			var orderInfo = new OrderInfo();

			orderInfo.PricesAreIncludingVAT = IOC.SettingsService.Resolve().IncludingVat;

			orderInfo.Localization = StoreHelper.CurrentLocalization;
			orderInfo.Status = OrderStatus.Confirmed;
			orderInfo.OrderLines = productInfo.Select(pi => new OrderLine(pi, orderInfo)).ToList();
			orderInfo.SetOrderReferenceOnOrderLinesAndProductInfos();
			orderInfo.VATCheckService = new FixedValueIvatChecker(false);
			orderInfo.EventsOn = true;
			orderInfo.StoreInfo.CountryCode = "NL";
			orderInfo.CustomerInfo.ShippingCountryCode = "NL";
			orderInfo.CustomerInfo.CountryCode = "NL";
			orderInfo.CustomerInfo.VATNumber = "12345NL";
			orderInfo.VatCalculationStrategy = IOC.VatCalculationStrategy.Resolve();
			return orderInfo;
		}

		internal static OrderInfo CreateIncompleteOrderInfo(params ProductInfo[] productInfo)
		{
			var orderInfo = CreateOrderInfo(productInfo);
			orderInfo.Status = OrderStatus.Incomplete;
			orderInfo.OrderDiscountsFactory = () =>
			{
				Console.WriteLine("Factory called");
				return IOC.OrderDiscountService.Resolve().GetApplicableDiscountsForOrder(orderInfo, orderInfo.Localization).ToList();
			};
			//orderInfo.VATChecker = hmm  => mock if needed
			return orderInfo;
		}

		public static ProductInfo CreateProductInfo(int productPriceInCents, int itemCount, DiscountProduct discount)
		{
			return CreateProductInfo(productPriceInCents, itemCount, 19, discount);
		}

		public static ProductInfo CreateProductInfo(int productPriceInCents, int itemCount, decimal vat = 19, DiscountProduct discount = null, IOrderInfo order = null)
		{
			var productInfo = new ProductInfo();
			productInfo.IsDiscounted = discount == null;
			productInfo.OriginalPriceInCents = productPriceInCents;
			productInfo.Ranges = new List<Range>();
			productInfo.Vat = vat;
			productInfo.ItemCount = itemCount;
			productInfo.Tags = new string[0];
			if (order != null)
			{
				order.OrderLines = new List<OrderLine> {new OrderLine(productInfo, order)};
			}
			productInfo.Order = order ?? CreateOrderInfo(productInfo);
			if (discount != null)
			{
				SetProductDiscountOnProductInfo(productInfo, discount);
			}
			return productInfo;
		}

		public static OrderDiscount CreateDefaultOrderDiscountWithPercentage(int percentage)
		{
			return CreateDefaultOrderDiscountWithPercentage(percentage, DiscountOrderCondition.None, 0);
		}

		public static List<OrderDiscount> CreateDefaultOrderDiscountWithPercentageList(int percentage, DiscountOrderCondition orderCondition, int setSize)
		{
			return new List<OrderDiscount> {CreateDefaultOrderDiscountWithPercentage(percentage, orderCondition, setSize)};
		}

		private static OrderDiscount CreateDefaultOrderDiscountWithPercentage(int percentage, DiscountOrderCondition orderCondition, int setSize)
		{
			return new OrderDiscount(StoreHelper.CurrentLocalization) { OriginalId = 1234, DiscountType = DiscountType.Percentage, DiscountValue = percentage * 100, CouponCode = "", NumberOfItemsCondition = setSize, Condition = orderCondition, RequiredItemIds = new List<int> { }, MemberGroups = new List<string>(), MinimalOrderAmount = 0, AffectedOrderlines = new List<int>(), Localization = StoreHelper.CurrentLocalization, AffectedProductTags = Enumerable.Empty<string>(), };
		}

		public static OrderDiscount CreateDefaultOrderDiscountWithAmount(int amountInCents, DiscountOrderCondition orderCondition, int setSize)
		{
			return new OrderDiscount(StoreHelper.CurrentLocalization) { OriginalId = 1235, DiscountType = DiscountType.Amount, DiscountValue = amountInCents, CouponCode = "", NumberOfItemsCondition = setSize, Condition = orderCondition, RequiredItemIds = new List<int> { }, MemberGroups = new List<string>(), MinimalOrderAmount = 0, AffectedOrderlines = new List<int>(), Localization = StoreHelper.CurrentLocalization, AffectedProductTags = Enumerable.Empty<string>(), };
		}

		public static OrderDiscount CreateDefaultOrderDiscountWithFreeShipping(DiscountOrderCondition orderCondition = DiscountOrderCondition.None)
		{
			return new OrderDiscount(StoreHelper.CurrentLocalization) { OriginalId = 1236, DiscountType = DiscountType.FreeShipping, CouponCode = "", Condition = orderCondition, RequiredItemIds = new List<int> { }, MemberGroups = new List<string>(), MinimalOrderAmount = 0, NumberOfItemsCondition = 0, DiscountValue = 0, AffectedOrderlines = new List<int>(), Localization = StoreHelper.CurrentLocalization, AffectedProductTags = Enumerable.Empty<string>(), };
		}

		public static OrderDiscount CreateDefaultOrderDiscountWithNewPrice(int amountInCents, DiscountOrderCondition orderCondition = DiscountOrderCondition.None, int setSize = 0)
		{
			return new OrderDiscount(StoreHelper.CurrentLocalization) { OriginalId = 1237, DiscountType = DiscountType.NewPrice, DiscountValue = amountInCents, CouponCode = "", Condition = orderCondition, RequiredItemIds = new List<int> { }, MemberGroups = new List<string>(), MinimalOrderAmount = 0, NumberOfItemsCondition = setSize, AffectedOrderlines = new List<int>(), Localization = StoreHelper.CurrentLocalization, AffectedProductTags = Enumerable.Empty<string>(), };
		}

		public static void SetDiscountsOnOrderInfo(OrderInfo orderInfo, params OrderDiscount[] discountOrders)
		{
			Console.WriteLine("Repository setup");
			IOC.OrderDiscountRepository.SetupFake(discountOrders.Select(d => d.ToDiscountOrder()).ToArray());
			if (orderInfo != null){var a = orderInfo.DiscountAmountInCents;}
		}

		public static void SetProductDiscountOnProductInfo(ProductInfo productInfo, DiscountProduct discount)
		{
			// todo: NewPrice. Maybe move this functionality elsewhere? (duplicate logic)
			if (discount.DiscountType == DiscountType.Amount)
				productInfo.DiscountAmountInCents = discount.RangedDiscountValue(productInfo.ItemCount.GetValueOrDefault(1));
			else
				productInfo.DiscountPercentage = discount.RangedDiscountValue(productInfo.ItemCount.GetValueOrDefault(1))/100m;
		}

		public static ProductVariantInfo CreateProductVariantInfo(int variantEffect)
		{
			return new ProductVariantInfo {PriceInCents = variantEffect};
		}

		public static void SetVariantsOnProductInfo(ProductInfo productInfo, params ProductVariantInfo[] variants)
		{
			productInfo.ProductVariants = variants.ToList();
			foreach (var variant in variants)
				variant.Product = productInfo;
		}

		public static DiscountProduct CreateProductDiscountPercentage(int percentage)
		{
			return new DiscountProduct {DiscountValue = percentage*100, DiscountType = DiscountType.Percentage};
		}
	}

	public static class Extensions
	{
		public static DiscountOrder ToDiscountOrder(this OrderDiscount orderDiscount)
		{
			//return new OrderDiscount { OriginalId = 1235, DiscountType = DiscountType.Amount, DiscountValue = amountInCents, CouponCode = "", NumberOfItemsCondition = setSize, Condition = orderCondition, RequiredItemIds = new List<int> { }, MemberGroups = new List<string>(), MinimalOrderAmount = 0, AffectedOrderlines = new List<int>() };

			return new DiscountOrder {AffectedOrderlines = orderDiscount.AffectedOrderlines, Id = orderDiscount.Id, Condition = orderDiscount.Condition, CounterEnabled = orderDiscount.CounterEnabled, CouponCode = orderDiscount.CouponCode, DiscountType = orderDiscount.DiscountType, DiscountValue = orderDiscount.DiscountValue, Disabled = orderDiscount.Disabled, NumberOfItemsCondition = orderDiscount.NumberOfItemsCondition, MemberGroups = orderDiscount.MemberGroups, RequiredItemIds = orderDiscount.RequiredItemIds, MinimalOrderAmount = orderDiscount.MinimalOrderAmount, Localization = orderDiscount.Localization};
		}
	}
}