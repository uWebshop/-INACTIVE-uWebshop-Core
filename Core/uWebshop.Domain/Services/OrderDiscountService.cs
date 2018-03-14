using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class OrderDiscountService : MultiStoreEntityService<IOrderDiscount>, IOrderDiscountService
	{
		private readonly IOrderService _orderService;

		public OrderDiscountService(IOrderDiscountRepository orderDiscountRepository, IStoreService storeService, IOrderService orderService) : base(orderDiscountRepository, storeService)
		{
			_orderService = orderService;
		}

		protected override void AfterEntitiesLoadedFromRepository(List<IOrderDiscount> entities, string storeAlias)
		{
		}

		//public IEnumerable<IOrderDiscount> GetApplicableDiscountsForOrder(OrderInfo orderInfo)
		//{
		//	orderInfo.OrderLines.ForEach(line => line.DiscountInCents = 0); // reset
		//	var orderDiscounts = GetAll(orderInfo.Localization);
		//	var orderLinesAmount = orderInfo.OrderLines.Sum(orderline => orderline.GetOrderLineGrandTotalInCents(orderInfo.PricesAreIncludingVAT));

		//	return orderDiscounts.Where(discount => !discount.Disabled && orderLinesAmount >= discount.MinimumOrderAmount.ValueInCents && (!discount.RequiredItemIds.Any() || _orderService.OrderContainsItem(orderInfo, discount.RequiredItemIds)) && (!discount.CounterEnabled || discount.Counter > 0)).HasDiscountForOrder(orderInfo).ToList();
		//}

		public IEnumerable<IOrderDiscount> GetApplicableDiscountsForOrder(OrderInfo order, ILocalization localization)
		{
			if (order.IsConfirmed())
			{
				Log.Instance.LogError("GetApplicableDiscountsForOrder with NOT incomplete order: " + order.UniqueOrderId + " status: " + order.Status);
				throw new Exception("Error, please contact the webmaster. Mention: Discount Issue");
			}

            var orderDiscounts = GetAll(localization);
			var orderLinesAmount = order.OrderLines.Sum(orderline => orderline.AmountInCents);

            var discounts = orderDiscounts.Where(discount => !discount.Disabled && orderLinesAmount >= discount.MinimumOrderAmount.ValueInCents() 
                && (!discount.RequiredItemIds.Any() || _orderService.OrderContainsItem(order, discount.RequiredItemIds)) 
                && (!discount.CounterEnabled || discount.Counter > 0))
                .HasDiscountForOrder(order)
                .ToList();

            return discounts;
		}
	}
}