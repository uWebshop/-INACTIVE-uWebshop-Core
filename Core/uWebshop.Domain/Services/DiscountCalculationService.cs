using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using uWebshop.Domain.Model.OrderInfo;

namespace uWebshop.Domain.Services
{
	internal class DiscountCalculationService : IDiscountCalculationService
	{
		private readonly IOrderService _orderService;
		private readonly ICouponCodeService _couponCodeService;

		public DiscountCalculationService(IOrderService orderService, ICouponCodeService couponCodeService)
		{
			_orderService = orderService;
			_couponCodeService = couponCodeService;
		}

		public int RangedDiscountValueForOrder(Interfaces.IOrderDiscount discount, OrderInfo order)
		{
			var ranges = discount.Ranges;
			if (ranges != null && ranges.Any())
			{
				var affectedOrderLines = discount.AffectedOrderlines.Any() || discount.RequiredItemIds.Any() ? _orderService.GetApplicableOrderLines(order, discount.AffectedOrderlines.Any() ? discount.AffectedOrderlines : discount.RequiredItemIds) : order.OrderLines;

				var itemCount = affectedOrderLines.Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1));

				return ranges.GetRangeAmountForValue(itemCount) ?? discount.DiscountValue;
			}
			return discount.DiscountValue;
		}

		public int DiscountAmountForOrder(IOrderDiscount discount, OrderInfo orderInfo, bool applyDiscountEffects = false)//, IAuthenticationProvider authenticationProvider = null)
		{
			if (!string.IsNullOrEmpty(discount.CouponCode) && !orderInfo.CouponCodes.Contains(discount.CouponCode))
			{
				return 0;
			}

			if (orderInfo.IsNotConfirmed())
			{
				var coupons = _couponCodeService.GetAllForDiscount(discount.Id);
				if (coupons.Any()) //
				{
					var availableCoupons = coupons.Where(c => c.NumberAvailable > 0).Select(c => c.CouponCode);
					if (!availableCoupons.Intersect(orderInfo.CouponCodes).Any())
						return 0;
				}
			}

			var authenticationProvider = IO.Container.Resolve<IAuthenticationProvider>();

			if (discount.MemberGroups.Any() && !discount.MemberGroups.Intersect(authenticationProvider.RolesForCurrentUser).Any())
			{
				return 0;
			}

			if (discount.OncePerCustomer && !string.IsNullOrEmpty(authenticationProvider.CurrentLoginName))
			{
				var ordersforCurrentMember = OrderHelper.GetOrdersForCustomer(authenticationProvider.CurrentLoginName).Where(x => x.IsConfirmed() && x.Status != OrderStatus.Cancelled && x.Status != OrderStatus.Returned);

				if (ordersforCurrentMember.Any(x => x.Discounts.Any(d => d.OriginalId == discount.Id)))
				{
					return 0;
				}
			}
			
			var applicableOrderLines = !discount.AffectedOrderlines.Any() ? orderInfo.OrderLines : _orderService.GetApplicableOrderLines(orderInfo, discount.AffectedOrderlines);

			if (discount.AffectedProductTags != null && discount.AffectedProductTags.Any())
			{
				applicableOrderLines = applicableOrderLines.Where(line => line.ProductInfo.Tags.Intersect(discount.AffectedProductTags).Any()).ToList();
			}
			var isSellableUnitDiscount = discount.AffectedOrderlines.Any() || (discount.AffectedProductTags != null && discount.AffectedProductTags.Any());

			var orderSellableUnits = new List<SellableUnit>();
			foreach (var line in applicableOrderLines)
				//for (var i = 0; i < line.ProductInfo.ItemCount; i++)
					orderSellableUnits.AddRange(line.SellableUnits); // maak een lijst met de prijzen van alle (losse) items van de order

			var numberOfItemsLeftOutOfSets = discount.NumberOfItemsCondition == 0 ? 0 : applicableOrderLines.Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1))%discount.NumberOfItemsCondition;
			var applicableSellableUnits = orderSellableUnits.OrderBy(item => item.PriceInCents).Take(orderSellableUnits.Count - numberOfItemsLeftOutOfSets).ToList();

			var rangedDiscountValue = RangedDiscountValueForOrder(discount, orderInfo); // todo: not localized
			var discountAmount = rangedDiscountValue;
			var maximumDiscountableAmount = 0L;
			var timesApplicable = 1;
			if (discount.Condition == DiscountOrderCondition.None)
			{
				maximumDiscountableAmount = applicableOrderLines.Sum(orderline => orderline.AmountInCents - (orderline.GetOriginalAmount(false, true) -  orderline.GetOriginalAmount(true, true)));
				timesApplicable = applicableOrderLines.Sum(orderLine => orderLine.ProductInfo.ItemCount.GetValueOrDefault(1));
			}
			else if (discount.Condition == DiscountOrderCondition.OnTheXthItem && discount.NumberOfItemsCondition > 0)
			{
				isSellableUnitDiscount = true;
				// todo: test
				discountAmount = discountAmount*applicableSellableUnits.Count()/discount.NumberOfItemsCondition;
				applicableSellableUnits = applicableSellableUnits.Take(orderSellableUnits.Count / discount.NumberOfItemsCondition).ToList();
				maximumDiscountableAmount = applicableSellableUnits.Sum(su => su.PriceInCents); // bereken de korting over de x goedkoopste items, waarbij x het aantal sets in de order is
			}
			else if (discount.Condition == DiscountOrderCondition.PerSetOfXItems && discount.NumberOfItemsCondition > 0)
			{
				isSellableUnitDiscount = true;
				timesApplicable = applicableSellableUnits.Count() / discount.NumberOfItemsCondition;
				if (discount.DiscountType == DiscountType.Amount)
				{
					applicableSellableUnits = applicableSellableUnits.Take(orderSellableUnits.Count / discount.NumberOfItemsCondition).ToList();
				}
				discountAmount = discountAmount*applicableSellableUnits.Count()/discount.NumberOfItemsCondition;
				maximumDiscountableAmount = applicableSellableUnits.Sum(su => su.PriceInCents);
			}

			if (discount.IncludeShippingInOrderDiscountableAmount)
			{
				maximumDiscountableAmount += orderInfo.ShippingProviderAmountInCents;
			}

			if (discount.DiscountType == DiscountType.Amount)
			{
				// currently not on SellableUnits, because that would break existing functionality

				if (applyDiscountEffects)
				{
					var amountDiscountEffect = new AmountDiscountEffect {Amount = rangedDiscountValue};
					if (isSellableUnitDiscount)
					{
						foreach (var su in applicableSellableUnits)
						{
							su.SellableUnitDiscountEffects.AddEffect(amountDiscountEffect);
						}
					}
					else
					{
						orderInfo.OrderDiscountEffects.AddEffect(amountDiscountEffect);
					}
				}
				
				return (int)Math.Min(discountAmount, maximumDiscountableAmount);
			}
			if (discount.DiscountType == DiscountType.Percentage)
			{
				// wanneer SellableUnit/OrderLine/Order
				if (applyDiscountEffects)
				{
					var percentageDiscountEffect = new PercentageDiscountEffect {Percentage = rangedDiscountValue/10000m};
					if (isSellableUnitDiscount)
					{
						foreach (var su in applicableSellableUnits)
						{
							su.SellableUnitDiscountEffects.AddEffect(percentageDiscountEffect);
						}
					}
					else
					{
						orderInfo.OrderDiscountEffects.AddEffect(percentageDiscountEffect);
					}
				}
				return DiscountHelper.PercentageCalculation(rangedDiscountValue, maximumDiscountableAmount);
			}
			if (discount.DiscountType == DiscountType.NewPrice)
			{
				if (applyDiscountEffects)
				{
					var newPriceDiscountEffect = new NewPriceDiscountEffect {NewPrice = rangedDiscountValue};
					foreach (var applicableSellableUnit in applicableSellableUnits)
					{
						applicableSellableUnit.SellableUnitDiscountEffects.AddEffect(newPriceDiscountEffect);
					}
				}
				if (discount.Condition == DiscountOrderCondition.OnTheXthItem && discount.NumberOfItemsCondition > 0)
				{
					return applicableSellableUnits.Take(orderSellableUnits.Count/discount.NumberOfItemsCondition).Select(su => Math.Max(0, su.PriceInCents - discountAmount)).Sum();
				}

				return (int)Math.Max(0, maximumDiscountableAmount - rangedDiscountValue*timesApplicable);
			}

			return 0;
		}
	}
}