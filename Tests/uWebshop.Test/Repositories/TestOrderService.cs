using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Repositories
{
	public class TestOrderService : IOrderService
	{
		public OrderInfo CreateOrder()
		{
			return CreateOrder(StoreHelper.GetCurrentStore());
		}

		public OrderInfo CreateOrder(Store store)
		{
			return DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo();
		}

		public OrderInfo CreateCopyOfOrder(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public void StoreOrderFirstTimeHackishRefactorPlz(OrderInfo order)
		{
		}

		public bool OrderContainsOutOfStockItem(OrderInfo orderinfo)
		{
			return false;
		}

	public List<OrderLine> GetApplicableOrderLines(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			return orderinfo.OrderLines;
		}

	public bool OrderContainsItem(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			return false;
		}

		public bool ValidateOrderLegacy(OrderInfo orderInfo)
		{
			throw new NotImplementedException();
		}

		List<OrderValidationError> IOrderService.ValidateOrder(OrderInfo orderInfo, bool confirmValidation)
		{
			throw new NotImplementedException();
		}

		public bool ValidateOrder(OrderInfo orderInfo, bool writeToOrderValidation = true)
		{
			return false;
		}

		public List<OrderValidationError> ValidateOrder(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public List<OrderValidationError> ValidateGlobalValidations(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool ValidateCustomer(OrderInfo orderinfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			return false;
		}

		public List<OrderValidationError> ValidateCustomer(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool ValidateStock(OrderInfo orderinfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			return false;
		}

		public List<OrderValidationError> ValidateStock(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool ValidateOrderlines(OrderInfo orderInfo, bool clearValidation, bool writeToOrderValidation = true)
		{
			return false;
		}

		public List<OrderValidationError> ValidateOrderlines(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool ValidateCustomValidations(OrderInfo orderInfo, bool writeToOrderValidation = true)
		{
			return false;
		}

		public List<OrderValidationError> ValidateCustomValidations(OrderInfo orderInfo)
		{
			throw new System.NotImplementedException();
		}

		public List<OrderValidationError> ValidatePayment(OrderInfo order)
		{
			throw new System.NotImplementedException();
		}

		public List<OrderValidationError> ValidateShipping(OrderInfo order)
		{
			throw new System.NotImplementedException();
		}

		public void UseStoredDiscounts(OrderInfo order, List<IOrderDiscount> discounts)
		{
			order.OrderDiscountsFactory = () => discounts;
		}

		public void UseDatabaseDiscounts(OrderInfo order)
		{
			// hmmm
		}

		public Guid GetOrderIdFromOrderIdCookie()
		{
			return Guid.Empty;
		}
	}
}