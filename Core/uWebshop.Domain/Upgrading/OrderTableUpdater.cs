using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Upgrading
{
	internal class OrderTableUpdater
	{
		// for upgrading installations from pre v2.1

		public void AddStoreOrderReferenceIdToExistingOrders()
		{
			var orders = OrderHelper.GetAllOrders().Where(orderinfo => orderinfo != null && orderinfo.Status != OrderStatus.Incomplete);
			var orderRepository = IO.Container.Resolve<IOrderRepository>();

			foreach (var orderInfo in orders.Where(order => !order.StoreOrderReferenceId.HasValue))
			{
				orderInfo.StoreOrderReferenceId = TryParseOrderNumber(orderInfo.OrderNumber);

				if (orderInfo.StoreOrderReferenceId.HasValue)
				{
					orderRepository.SaveOrderInfo(orderInfo);
				}
				else if (!string.IsNullOrWhiteSpace(orderInfo.OrderNumber))
				{
					Log.Instance.LogWarning("Ordernumber could not be parsed, for order with id " + orderInfo.DatabaseId + ", guid " + orderInfo.UniqueOrderId);
				}
				else if (orderInfo.Status != OrderStatus.Incomplete)
				{
					Log.Instance.LogWarning("Order without ordernumber, id " + orderInfo.DatabaseId + ", guid " + orderInfo.UniqueOrderId);
				}
			}
		}

		internal int? TryParseOrderNumber(string orderNumber)
		{
			if (string.IsNullOrWhiteSpace(orderNumber)) return null;
			int orderReferenceNumber;
			if (int.TryParse(Regex.Match(orderNumber, "\\d+$").Value, out orderReferenceNumber))
				return orderReferenceNumber;
			return null;
		}

		public void UpdateXMLAndFieldsOfExistingOrders()
		{
			var orders = OrderHelper.GetAllOrders().Where(orderinfo => orderinfo != null).ToList();
			var orderRepository = IO.Container.Resolve<IOrderRepository>();

			foreach (var orderInfo in orders)
			{
				orderRepository.SaveOrderInfo(orderInfo);
			}
		}
	}
}