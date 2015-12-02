using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Xml;
using uWebshop.Common;
using uWebshop.Common.ServiceInterfaces;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Repositories
{
    internal class OrderRepository : IOrderRepository
    {
        private readonly IUwebshopConfiguration _uwebshopConfiguration;

        public OrderRepository(IUwebshopConfiguration uwebshopConfiguration)
        {
            _uwebshopConfiguration = uwebshopConfiguration;
        }

        public IEnumerable<OrderInfo> GetAllOrders()
        {
            ObjectCache cache = MemoryCache.Default;
            var orders = cache["f24485fb-4e31-4735-ad47-868a3eef0947"] as OrderInfo[];
            if (orders == null)
            {
                Log.Instance.LogDebug("Loading all orders from database");
                orders = uWebshopOrders.GetAllOrderInfos().Select(OrderInfo.CreateOrderInfoFromOrderData).ToArray();
                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMilliseconds(_uwebshopConfiguration.OrdersCacheTimeoutMilliseconds)
                };
                cache.Set("f24485fb-4e31-4735-ad47-868a3eef0947", orders, policy);
            }
            return orders;
        }

        public OrderInfo GetOrderInfo(Guid uniqueOrderId)
        {
            var orderInfoData = uWebshopOrders.GetOrderInfo(uniqueOrderId);

            return orderInfoData != null ? OrderInfo.CreateOrderInfoFromOrderData(orderInfoData) : null;
        }

        public int DetermineLastOrderId()
        {
            return uWebshopOrders.DetermineLastOrderId();
        }

        public XmlDocument GetOrderInfoXML(Guid uniqueOrderId)
        {
            var orderInfo = GetOrderInfo(uniqueOrderId);
            var orderInfoXmlString = DomainHelper.SerializeObjectToXmlString(orderInfo);
            var xmlDoc = new XmlDocument();
            if (orderInfoXmlString != null) xmlDoc.LoadXml(orderInfoXmlString);

            return xmlDoc;
        }

        public OrderInfo GetOrderInfo(string transactionId)
        {
            return OrderInfo.CreateOrderInfoFromOrderData(uWebshopOrders.GetOrderInfo(transactionId));
        }

        public OrderInfo GetOrderInfo(int databaseId)
        {
            return OrderInfo.CreateOrderInfoFromOrderData(uWebshopOrders.GetOrderInfo(databaseId));
        }

        public IEnumerable<OrderInfo> GetOrdersFromCustomer(int customerId, string storeAlias = null, bool includeIncomplete = false)
        {
            return uWebshopOrders.GetOrdersFromCustomer(customerId, includeIncomplete).Select(OrderInfo.CreateOrderInfoFromOrderData);
        }

        public IEnumerable<OrderInfo> GetOrdersFromCustomer(string customerUsername, string storeAlias = null, bool includeIncomplete = false)
        {
            return uWebshopOrders.GetOrdersFromCustomer(customerUsername, includeIncomplete).Select(OrderInfo.CreateOrderInfoFromOrderData);
        }

        public IEnumerable<OrderInfo> GetWishlistsFromCustomer(int customerId, string storeAlias = null)
        {
            return uWebshopOrders.GetWishlistsFromCustomer(customerId).Select(OrderInfo.CreateOrderInfoFromOrderData);
        }

        public IEnumerable<OrderInfo> GetWishlistsFromCustomer(string customerUsername, string storeAlias = null)
        {
            return uWebshopOrders.GetWishlistsFromCustomer(customerUsername).Select(OrderInfo.CreateOrderInfoFromOrderData);
        }

        public void SetOrderNumber(Guid uniqueOrderId, string orderNumber, string alias, int id)
        {
            uWebshopOrders.SetOrderNumber(uniqueOrderId, orderNumber, alias, id);
        }

        public string GetHighestOrderNumber(ref int lastOrderReferenceNumber)
        {
            return uWebshopOrders.GetHighestOrderNumber(ref lastOrderReferenceNumber);
        }

        public string GetHighestOrderNumberForStore(string alias, ref int lastOrderReferenceNumber)
        {
            return uWebshopOrders.GetHighestOrderNumberForStore(alias, ref lastOrderReferenceNumber);
        }

        public int AssignNewOrderNumberToOrderSharedBasket(int databaseId, string alias, int orderNumberStartNumber)
        {
            return uWebshopOrders.AssignNewOrderNumberToOrderSharedBasket(databaseId, alias, orderNumberStartNumber);
        }

        public int AssignNewOrderNumberToOrder(int databaseId, string alias, int orderNumberStartNumber)
        {
            return uWebshopOrders.AssignNewOrderNumberToOrder(databaseId, alias, orderNumberStartNumber);
        }

        public void SaveOrderInfo(OrderInfo orderInfo)
        {
            uWebshopOrders.StoreOrder(orderInfo.ToOrderData());
        }

        public void LegacyStoreOrder(OrderInfo order)
        {
            // a different call for the first time the order is stored
            //todo: why is this different? => refactor to StoreOrder?
            uWebshopOrders.SetOrderInfo(order.UniqueOrderId, DomainHelper.SerializeObjectToXmlString(order), OrderStatus.Incomplete);
        }

        public void SetCustomerId(Guid orderId, int customerId)
        {
            uWebshopOrders.SetCustomerId(orderId, customerId);
        }

        public void SetCustomer(Guid orderId, string userName)
        {
            uWebshopOrders.SetCustomer(orderId, userName);
        }

        public void SetCustomerInfo(Guid orderId, string customerEmail, string customerFirstName, string customerLastName)
        {
            uWebshopOrders.SetCustomerInfo(orderId, customerEmail, customerFirstName, customerLastName);
        }

        public void RemoveIncompleOrdersBeforeDate(int daysAgo)
        {
            var orders = uWebshopOrders.GetAllOrderInfos().Where(x => DateTime.Now.AddDays(-daysAgo) >= x.UpdateDate.GetValueOrDefault() && x.OrderStatus == (int)OrderStatus.Incomplete);

            uWebshopOrders.Delete(orders);
        }

        public void RemoveTestOrders()
        {
            var orders = GetAllOrders().Select(order => order.UniqueOrderId);

            uWebshopOrders.Delete(orders);
        }

        public void RemoveOrders(IEnumerable<IOrder> orderList)
        {
            var orders = orderList.Select(order => order.UniqueId);

            uWebshopOrders.Delete(orders);
        }

        public void SetTransactionId(Guid orderId, string transactionId)
        {
            uWebshopOrders.SetTransactionId(orderId, transactionId);
        }
    }
}