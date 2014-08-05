using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IOrderService
	{
		/// <summary>
		/// Creates the order.
		/// </summary>
		/// <returns></returns>
		OrderInfo CreateOrder();

		/// <summary>
		/// Creates the order.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns></returns>
		OrderInfo CreateOrder(Store store);

		/// <summary>
		/// Creates the copy of order.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		OrderInfo CreateCopyOfOrder(OrderInfo orderInfo);

		/// <summary>
		/// Orders the contains out of stock item.
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <returns></returns>
		bool OrderContainsOutOfStockItem(OrderInfo orderinfo);

		/// <summary>
		/// Gets the applicable order lines.
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <param name="itemIdsToCheck">The item ids automatic check.</param>
		/// <returns></returns>
		List<OrderLine> GetApplicableOrderLines(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck);

		/// <summary>
		/// Orders the contains item.
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <param name="itemIdsToCheck">The item ids automatic check.</param>
		/// <returns></returns>
		bool OrderContainsItem(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck);

		List<OrderValidationError> ValidateOrder(OrderInfo orderInfo, bool confirmValidation = false);
		List<OrderValidationError> ValidateGlobalValidations(OrderInfo orderInfo);
		/// <summary>
		/// Validates the customer.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <param name="writeToOrderValidation">if set to <c>true</c> [write automatic order validation].</param>
		/// <returns></returns>
		bool ValidateCustomer(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true);
		List<OrderValidationError> ValidateCustomer(OrderInfo orderInfo);

		/// <summary>
		/// Validates the stock.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <param name="writeToOrderValidation">if set to <c>true</c> [write automatic order validation].</param>
		/// <returns></returns>
		bool ValidateStock(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true);
		List<OrderValidationError> ValidateStock(OrderInfo orderInfo);

		/// <summary>
		/// Validates the orderlines.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <param name="writeToOrderValidation">if set to <c>true</c> [write automatic order validation].</param>
		/// <returns></returns>
		bool ValidateOrderlines(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true);
		List<OrderValidationError> ValidateOrderlines(OrderInfo orderInfo);

		/// <summary>
		/// Validates the custom validations.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="writeToOrderValidation">if set to <c>true</c> [write automatic order validation].</param>
		/// <returns></returns>
		bool ValidateCustomValidations(OrderInfo orderInfo, bool writeToOrderValidation);
		List<OrderValidationError> ValidateCustomValidations(OrderInfo orderInfo);

		List<OrderValidationError> ValidatePayment(OrderInfo order);
		List<OrderValidationError> ValidateShipping(OrderInfo order);

		/// <summary>
		/// Uses the stored discounts.
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="discounts">The discounts.</param>
		void UseStoredDiscounts(OrderInfo order, List<IOrderDiscount> discounts);

		/// <summary>
		/// Uses the database discounts.
		/// </summary>
		/// <param name="order">The order.</param>
		void UseDatabaseDiscounts(OrderInfo order);


		Guid GetOrderIdFromOrderIdCookie();
	}
}