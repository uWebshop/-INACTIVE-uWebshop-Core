using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public static class Orders
	{
		/// <summary>
		/// Gets the current order.
		/// </summary>
		/// <returns></returns>
		public static IOrder GetOrder()
		{
			var order = OrderHelper.GetOrder();

			return order != null ? CreateBasketFromOrderInfo(order) : null;
		}

		/// <summary>
		/// Gets the order.
		/// </summary>
		/// <param name="guid">The unique identifier as string</param>
		/// <returns></returns>
		public static IOrder GetOrder(string guid)
		{
			Guid guidFromString;
			Guid.TryParse(guid, out guidFromString);
			
			return guidFromString != Guid.Empty ? GetOrder(guidFromString) : null;
		}

		/// <summary>
		/// Gets the order.
		/// </summary>
		/// <param name="guid">The unique identifier.</param>
		/// <returns></returns>
		public static IOrder GetOrder(Guid guid)
		{
			var order = OrderHelper.GetOrder(guid);

			var membershipUser = Membership.GetUser();
		  
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || (membershipUser != null && membershipUser.UserName == order.CustomerInfo.LoginName || UwebshopRequest.Current.PaymentProvider != null || OrderHelper.IsCompletedOrderWithinValidLifetime(order)) || order.Status == OrderStatus.Wishlist)
			{
				return CreateBasketFromOrderInfo(order);
			}

			return null;
		}

		/// <summary>
		/// Gets the order by transaction unique identifier.
		/// </summary>
		/// <param name="transactionId">The transaction unique identifier.</param>
		/// <returns></returns>
		public static IOrder GetOrderByTransactionId(string transactionId)
		{
			var order = OrderHelper.GetOrder(transactionId);

			var membershipUser = Membership.GetUser();

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.UserName == order.CustomerInfo.LoginName || UwebshopRequest.Current.PaymentProvider != null || OrderHelper.IsCompletedOrderWithinValidLifetime(order))
			{
				return CreateBasketFromOrderInfo(order);
			}

			return null;
		}

		/// <summary>
		/// Gets all orders.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetAllOrders(string storeAlias = null)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetAllOrders(storeAlias).Select(CreateBasketFromOrderInfo);
			}

			return Enumerable.Empty<IOrder>();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="status">The status.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrders(OrderStatus status, string storeAlias = null)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetAllOrders(storeAlias).Where(o => o.Status == status).Select(CreateBasketFromOrderInfo);
			}

			return Enumerable.Empty<IOrder>();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="days">The days.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrders(int days, string storeAlias = null)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				return GetOrders(DateTime.Now.AddDays(-days), DateTime.Now, storeAlias);
			}

			return Enumerable.Empty<IOrder>();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="startDate">The start date.</param>
		/// <param name="endDate">The end date.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrders(DateTime startDate, DateTime endDate, string storeAlias = null)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				return GetAllOrders(storeAlias).Where(o => o.ConfirmDate >= startDate && o.ConfirmDate <= endDate);
			}

			return Enumerable.Empty<IOrder>();
		}

		/// <summary>
		/// Gets the orders for customer.
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrdersForCustomer(int customerId, string storeAlias = null)
		{
			var membershipUser = Membership.GetUser();
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.ProviderUserKey == customerId.ToString() || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetOrdersForCustomer(customerId, storeAlias).Select(CreateBasketFromOrderInfo);
			}

			return Enumerable.Empty<IOrder>();

		}

		/// <summary>
		/// Gets the orders for customer.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrdersForCustomer(string userName, string storeAlias = null)
		{
			var membershipUser = Membership.GetUser();
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.UserName == userName || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetOrdersForCustomer(userName, storeAlias).Select(CreateBasketFromOrderInfo);
			}

			return Enumerable.Empty<IOrder>();
		}

		internal static IOrder CreateBasketFromOrderInfo(OrderInfo order)
		{
			return new BasketOrderInfoAdaptor(order);
		}

		/// <summary>
		/// Get completed order for current customer
		/// </summary>
		/// <returns></returns>
		public static IOrder GetCompletedOrder()
		{
			var orderIdCookie = OrderHelper.GetCompletedOrderCookie();

			return orderIdCookie != null ? GetOrder(orderIdCookie) : null;
		}
	}
}
