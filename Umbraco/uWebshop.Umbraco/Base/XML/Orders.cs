using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.NewtonsoftJsonNet;
using Order = uWebshop.Domain.OrderDTO.Order;

namespace uWebshop.API.XML
{
	[XsltExtension("Orders")]
	public class Orders
	{
		/// <summary>
		/// Gets the current order.
		/// </summary>
		/// <returns></returns>
		public static XPathNavigator GetOrder()
		{
			var item = API.Orders.GetOrder();

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrder", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the order.
		/// </summary>
		/// <param name="guidAsString">The unique identifier.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrder(string guidAsString)
		{
			var item = API.Orders.GetOrder(guidAsString);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrder", item, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the order by transaction unique identifier.
		/// </summary>
		/// <param name="transactionId">The transaction unique identifier.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrderByTransactionId(string transactionId)
		{
			var item = API.Orders.GetOrderByTransactionId(transactionId);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrderByTransactionId", item, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets all orders.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllOrders(string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Orders.GetAllOrders(storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllOrders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="status">The status.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrdersByStatus(string status, string storeAlias = null)
		{
			var orderstatus = OrderStatus.Confirmed;

			Enum.TryParse(status, out orderstatus);

			var dictionary = new Dictionary<string, object>();

			var item = API.Orders.GetOrders(orderstatus, storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="days">The days.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrdersByDays(int days, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Orders.GetOrders(days, storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrdersByDateTime(DateTime startDate, DateTime endDate, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Orders.GetOrders(startDate, endDate, storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrders", dictionary, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the orders for customer.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrdersForCustomer(string userName, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Orders.GetOrdersForCustomer(userName, storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrdersForCustomer", dictionary, true)).CreateNavigator();
		}
	}
}
