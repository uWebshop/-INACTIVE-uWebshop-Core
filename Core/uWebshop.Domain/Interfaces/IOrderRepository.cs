using System;
using System.Collections.Generic;
using System.Xml;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IOrderRepository
	{
		IEnumerable<OrderInfo> GetAllOrders();

		/// <summary>
		/// Returns a single orderinfo
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns>
		/// Order
		/// </returns>
		OrderInfo GetOrderInfo(Guid uniqueOrderId);

		/// <summary>
		/// Determines the last order unique identifier.
		/// </summary>
		/// <returns></returns>
		int DetermineLastOrderId();

		/// <summary>
		/// Returns a single orderinfo
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns>
		/// Order
		/// </returns>
		XmlDocument GetOrderInfoXML(Guid uniqueOrderId);

		/// <summary>
		/// Get the order based on the transaction ID returned from the Payment Provider
		/// </summary>
		/// <param name="transactionId">The transaction unique identifier.</param>
		/// <returns></returns>
		OrderInfo GetOrderInfo(string transactionId);
		OrderInfo GetOrderInfo(int databaseId);

		/// <summary>
		/// Saves the order.
		/// </summary>
		/// <param name="orderInfo">The order.</param>
		void SaveOrderInfo(OrderInfo orderInfo);

	    /// <summary>
	    /// Gets the orders from customer.
	    /// </summary>
	    /// <param name="customerId">The customer unique identifier.</param>
	    /// <param name="storeAlias">The store alias.</param>
	    /// <param name="includeIncomplete"></param>
	    /// <returns></returns>
	    IEnumerable<OrderInfo> GetOrdersFromCustomer(int customerId, string storeAlias = null, bool includeIncomplete = false);

		/// <summary>
		/// Gets the wishlists from customer.
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		IEnumerable<OrderInfo> GetWishlistsFromCustomer(int customerId, string storeAlias = null);

	    /// <summary>
	    /// Get the orders based on the customer username
	    /// </summary>
	    /// <param name="customerUsername">The customer username.</param>
	    /// <param name="storeAlias">The store alias.</param>
	    /// <param name="includeIncomplete"></param>
	    /// <returns>
	    /// All the orders for the customer
	    /// </returns>
	    IEnumerable<OrderInfo> GetOrdersFromCustomer(string customerUsername, string storeAlias = null, bool includeIncomplete = false);

        /// <summary>
        /// Get the orders based on the customer username or email
        /// </summary>
        /// <param name="customerUsername">The customer username/email</param>
        /// <param name="storeAlias">The store alias.</param>
        /// <param name="includeIncomplete"></param>
        /// <returns>
        /// All the orders for the customer
        /// </returns>
        IEnumerable<OrderInfo> GetOrdersFromCustomerOrEmail(string customerUsername, string storeAlias = null, bool includeIncomplete = false);


        /// <summary>
        /// Get the wishlists based on the customer username
        /// </summary>
        /// <param name="customerUsername">The customer username.</param>
        /// <param name="storeAlias">The store alias.</param>
        /// <returns>
        /// All the orders for the customer
        /// </returns>
        IEnumerable<OrderInfo> GetWishlistsFromCustomer(string customerUsername, string storeAlias = null);
		
		/// <summary>
		/// Sets the order number.
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <param name="orderNumber">The order number.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="id">The unique identifier.</param>
		void SetOrderNumber(Guid uniqueOrderId, string orderNumber, string alias, int id);

		/// <summary>
		/// Gets the highest order number.
		/// </summary>
		/// <param name="lastOrderReferenceNumber">The last order reference number.</param>
		/// <returns></returns>
		string GetHighestOrderNumber(ref int lastOrderReferenceNumber);
		/// <summary>
		/// Gets the highest order number for store.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="lastOrderReferenceNumber">The last order reference number.</param>
		/// <returns></returns>
		string GetHighestOrderNumberForStore(string alias, ref int lastOrderReferenceNumber);
		/// <summary>
		/// Assigns the new order number automatic order shared basket.
		/// </summary>
		/// <param name="databaseId">The database unique identifier.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="orderNumberStartNumber">The order number start number.</param>
		/// <returns></returns>
		int AssignNewOrderNumberToOrderSharedBasket(int databaseId, string alias, int orderNumberStartNumber);
		/// <summary>
		/// Assigns the new order number automatic order.
		/// </summary>
		/// <param name="databaseId">The database unique identifier.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="orderNumberStartNumber">The order number start number.</param>
		/// <returns></returns>
		int AssignNewOrderNumberToOrder(int databaseId, string alias, int orderNumberStartNumber);

		/// <summary>
		/// Stores the order first time hackish refactor PLZ.
		/// </summary>
		/// <param name="order">The order.</param>
		void LegacyStoreOrder(OrderInfo order);

		void SetCustomerId(Guid orderId, int customerId);

		/// <summary>
		/// Set the .Net Membership Loginname
		/// </summary>
		/// <param name="orderId">The order unique identifier.</param>
		/// <param name="userName">Name of the user.</param>
		void SetCustomer(Guid orderId, string userName);

		void SetCustomerInfo(Guid orderId, string customerEmail, string customerFirstName, string customerLastName);

		void RemoveIncompleOrdersBeforeDate(int daysAgo);

		/// <summary>
		/// Removes all orders with the store in testmode: No Undo!
		/// </summary>
		void RemoveTestOrders();

		/// <summary>
		/// Removes orders: No Undo!
		/// </summary>
		/// <param name="orderList">List of orders</param>
		void RemoveOrders(IEnumerable<IOrder> orderList);

		void SetTransactionId(Guid orderId, string transactionId);
	}
}