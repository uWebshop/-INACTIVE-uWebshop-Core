using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.web;
using uWebshop.Common;
using Log = uWebshop.Domain.Log;

namespace uWebshop.RazorExtensions
{
	public static class Orders
	{
		/// <summary>
		/// Return the Order from a given unique order Id as GUID
		/// </summary>
		/// <param name="uniqueOrderId"></param>
		/// <returns></returns>
		public static OrderInfo GetOrder(Guid uniqueOrderId)
		{
			return OrderHelper.GetOrder(uniqueOrderId);
		}

		/// <summary>
		/// Return the Order from a given unique order Id as String
		/// </summary>
		/// <param name="uniqueOrderId"></param>
		/// <returns></returns>
		public static OrderInfo GetOrder(string uniqueOrderId)
		{
			Guid guid;
			return Guid.TryParse(uniqueOrderId, out guid) ? GetOrder(guid) : null;
		}
		
		/// <summary>
		/// Return the Order from a given unique order Id as GUID
		/// </summary>
		/// <param name="overruleCopyOrderOnConfirmedOrder"></param>
		/// <returns></returns>
		public static OrderInfo GetOrder(bool overruleCopyOrderOnConfirmedOrder = false)
		{
			return OrderHelper.GetOrder(overruleCopyOrderOnConfirmedOrder);
		}
		

		public static OrderInfo GetCurrentOrder(bool overruleCopyOrderOnConfirmedOrder = false)
		{
			return GetOrder(overruleCopyOrderOnConfirmedOrder);
		}

		/// <summary>
		/// Returns the order for the current customer, with check if the order is incomplete
		/// </summary>
		/// <returns></returns>
		public static OrderInfo GetOrderForCurrentCustomer(bool overruleCopyOrderOnConfirmedOrder = false)
		{
			return GetOrder(overruleCopyOrderOnConfirmedOrder);
		}


		/// <summary>
		/// Get the full country node from a countrycode
		/// </summary>
		/// <param name="countryCode"></param>
		/// <returns></returns>
		public static string GetFullCountryNameFromCountry(string countryCode)
		{
			var country = StoreHelper.GetAllCountries().FirstOrDefault(x => x.Code == countryCode);

			return country != null ? country.Name : string.Empty;
		}

		/// <summary>
		/// Get a list of all the errors in the order
		/// </summary>
		/// <returns></returns>
		public static List<ClientErrorHandling> GetOrderErrors()
		{
			return ClientErrorHandling.GetErrorMessages();
		}

		/// <summary>
		/// Get the customer property types of the order document type.
		/// </summary>
		/// <returns></returns>
		public static List<PropertyType> CustomerPropertyTypes()
		{
			var orderDocType = DocumentType.GetByAlias(Order.NodeAlias);

			var customerTab = orderDocType.getVirtualTabs.FirstOrDefault(x => x.Caption == "Customer");

			return DocumentType.GetByAlias(Order.NodeAlias).PropertyTypes.Where(x => customerTab != null && x.TabId == customerTab.Id).ToList();
		}

		/// <summary>
		/// Return the Order from a given unique order Id
		/// </summary>
		/// <param name="uniqueOrderId"></param>
		/// <returns></returns>
		public static OrderInfo GetOrderFromUniqueOrderId(Guid uniqueOrderId)
		{
			//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " RazorExtensions.Orders.GetOrderFromUniqueOrderId >>>>SQL<<<< SELECT orderInfo");
			return OrderHelper.GetOrderInfo(uniqueOrderId);
		}

		

		/// <summary>
		/// Return latest, non incomplete, order
		/// </summary>
		/// <returns></returns>
		public static OrderInfo GetLatestOrder()
		{
			if (IO.Container.Resolve<ICMSApplication>().RequestIsInCMSBackend(HttpContext.Current))
			{
				return OrderHelper.GetAllOrders(StoreHelper.CurrentStoreAlias).LastOrDefault(x => x.Status != OrderStatus.Incomplete);
			}

			return null;
		}



		/// <summary>
		/// Does the order contain items that are out of stock or without enough stock
		/// </summary>
		/// <param name="uniqueOrderId"></param>
		/// <returns></returns>
		public static bool OrderContainsItemsOutOfStock(Guid uniqueOrderId)
		{
			return OrderHelper.OrderContainsOutOfStockItems(uniqueOrderId);
		}

		/// <summary>
		/// Returns the unique order id for the current customer, with check if order is  incomplete.
		/// </summary>
		/// <returns></returns>
		public static string GetUniqueOrderIdForCurrentCustomer()
		{
			var currentOrder = OrderHelper.GetOrder();

			if (currentOrder != null && currentOrder.Status == OrderStatus.Incomplete || currentOrder != null && currentOrder.Status == OrderStatus.PaymentFailed)
			{
				return currentOrder.UniqueOrderId.ToString();
			}

			return string.Empty;
		}


		/// <summary>
		/// Get completed order for current customer
		/// </summary>
		/// <returns></returns>
		public static OrderInfo GetCompletedOrderForCurrentCustomer()
		{
			var cookieName = OrderHelper.GetOrderCookieName(true);

			var orderIdCookie = HttpContext.Current.Request.Cookies[cookieName];

			if (orderIdCookie != null && !string.IsNullOrEmpty(orderIdCookie.Value))
			{
				Guid uniqueOrderId;
				Guid.TryParse(orderIdCookie.Value, out uniqueOrderId);

				return GetOrderFromUniqueOrderId(uniqueOrderId);
			}

			return null;
		}

		/// <summary>
		/// Copy over the order from a given order to a new one and set that as the current order
		/// </summary>
		/// <param name="orderinfo"></param>
		/// <returns></returns>
		public static OrderInfo CreateNewOrderFromExisting(OrderInfo orderinfo)
		{
			return OrderHelper.CreateNewOrderFromExisting(orderinfo);
		}

		/// Return a list of orders for a customer Id
		/// Return orders based on the customer Id
		/// <param name="customerId"></param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetOrdersForCustomer(string loginName)")]
		public static List<OrderInfo> GetOrdersForCustomer(int customerId)
		{
			return OrderHelper.GetOrdersForCustomer(customerId).ToList();
		}

		/// Return a list of orders for a customer LoginName
		/// Return orders based on the customer LoginName
		/// <param name="loginName"></param>
		/// <returns></returns>
		public static List<OrderInfo> GetOrdersForCustomer(string loginName)
		{
			return OrderHelper.GetOrdersForCustomer(loginName).ToList();
		}

		/// <summary>
		/// Retun a list of orders with a certain order status
		/// </summary>
		/// <returns></returns>
		public static List<OrderInfo> GetAllOrders()
		{
			return OrderHelper.GetAllOrders().ToList();
		}

		/// <summary>
		/// Retun a list of orders with a certain order status
		/// </summary>
		/// <param name="orderStatus"></param>
		/// <returns></returns>
		public static List<OrderInfo> GetOrdersWithStatus(OrderStatus orderStatus)
		{
			return OrderHelper.GetAllOrders().Where(x => x.Status == orderStatus).ToList();
		}

		/// <summary>
		/// Return orders related to current member
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use GetOrdersForMember()")]
		public static List<OrderInfo> GetOrdersForCurrentMember()
		{
			var value = new List<OrderInfo>();
			var currentThread = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = StoreHelper.GetCurrentStore().CurrencyCultureInfo;

			var member = Member.GetCurrentMember();

			if (member != null && member.Id != 0)
			{
				value = OrderHelper.GetOrdersForCustomer(member.Id).ToList();
			}
			Thread.CurrentThread.CurrentCulture = currentThread;

			return value;
		}

		/// <summary>
		/// Return orders related to current member
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetOrdersForCustomer(string loginName)")]
		public static List<OrderInfo> GetOrdersForMember()
		{
			var member = Membership.GetUser();

			return member != null ? GetOrdersForMember(member.UserName) : new List<OrderInfo>();
		}

		/// <summary>
		/// Return orders related to member logiNname
		/// </summary>
		/// <returns></returns>
		public static List<OrderInfo> GetOrdersForMember(string userName)
		{
			var orders = new List<OrderInfo>();

			if (string.IsNullOrEmpty(userName))
			{
				return orders;
			}

			var currentThread = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = StoreHelper.GetCurrentStore().CurrencyCultureInfo;

			var member = Membership.GetUser(userName);

			if (member != null)
			{
				orders = OrderHelper.GetOrdersForCustomer(member.UserName).ToList();
			}

			Thread.CurrentThread.CurrentCulture = currentThread;

			return orders;
		}


		/// <summary>
		/// Returns Shipping Providers for this order
		/// </summary>
		/// <returns></returns>
		public static List<ShippingProvider> ShippingProviders(bool useCountry = true)
		{
			return ShippingProviderHelper.GetShippingProvidersForOrder(useCountry);
		}


		/// <summary>
		/// Returns Shipping Providers for this order
		/// </summary>
		/// <returns></returns>
		public static List<ShippingProvider> GetShippingProvidersForOrder(bool useCountry = true)
		{
			return ShippingProviderHelper.GetShippingProvidersForOrder(useCountry);
		}

		/// <summary>
		/// Get all the shipping providers in uWebshop
		/// </summary>
		/// <returns></returns>
		public static List<ShippingProvider> GetAllShippingProviders()
		{
			return ShippingProviderHelper.GetAllShippingProviders().ToList();
		}


		/// <summary>
		/// Returns Payment Providers for this order
		/// </summary>
		/// <returns></returns>
		public static List<PaymentProvider> PaymentProviders()
		{
			return PaymentProviderHelper.GetPaymentProvidersForOrder();
		}

		/// <summary>
		/// Returns Payment Providers for this order
		/// </summary>
		/// <returns></returns>
		public static List<PaymentProvider> GetPaymentProvidersForOrder(bool useCountry = true)
		{
			return PaymentProviderHelper.GetPaymentProvidersForOrder(useCountry);
		}

		/// <summary>
		/// Get all the payment providers in uWebshop
		/// </summary>
		/// <returns></returns>
		public static List<PaymentProvider> GetAllPaymentProviders()
		{
			return PaymentProviderHelper.GetAllPaymentProviders().ToList();
		}

		/// <summary>
		/// Get the customer information value from the order based on the property alias
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string CustomerInformationValue(OrderInfo orderInfo, string alias)
		{
			return OrderHelper.CustomerInformationValue(orderInfo, alias);
		}

		/// <summary>
		/// Get the shipping information value from the order based on the property alias
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string ShippingInformationValue(OrderInfo orderInfo, string alias)
		{
			return OrderHelper.ShippingInformationValue(orderInfo, alias);
		}

		/// <summary>
		/// Get the extra information value from the order based on the property alias
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string ExtraInformationValue(OrderInfo orderInfo, string alias)
		{
			return OrderHelper.ExtraInformationValue(orderInfo, alias);
		}

		/// <summary>
		/// Get the orderline value from the order based on the property alias and orderline Id
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="orderLineId"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string CustomOrderLineValue(OrderInfo orderInfo, int orderLineId, string alias)
		{
			return OrderHelper.CustomOrderLineValue(orderInfo, orderLineId, alias);
		}

		/// <summary>
		/// Handles the payment provider response. Warning: requires HttpContext.Current.Request["paymentprovider"] to be set
		/// </summary>
		/// <exception cref="System.Exception">
		/// PaymentProvider Not Found
		/// </exception>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void HandlePaymentProviderResponse()
		{
			// todo: old, make obsolete?
			var paymentProvider = HttpContext.Current.Request["paymentprovider"];

			if (string.IsNullOrEmpty(paymentProvider))
			{
				Log.Instance.LogDebug("UwbsPaymentHandler: PaymentProvider Not Found");
				throw new Exception("PaymentProvider Not Found");
			}

			var paymentProviderNode = IO.Container.Resolve<IPaymentProviderService>().GetPaymentProviderWithName(paymentProvider, StoreHelper.CurrentLocalization);

			if (paymentProviderNode == null)
			{
				Log.Instance.LogDebug("UwbsPaymentHandler: PaymentProvider " + paymentProvider + " Not Found");
				throw new Exception("PaymentProvider " + paymentProvider + " Not Found");
			}

			new PaymentRequestHandler().HandleuWebshopPaymentRequest(paymentProviderNode);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// output = .ToString("F)
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <returns></returns>
		public static string CentsToPrice(int priceInCents)
		{
			return CentsToPrice(priceInCents, null, false);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// output = .ToString("F)
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents)
		{
			int price;
			int.TryParse(priceInCents, out price);

			return CentsToPrice(price, null, false);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <returns></returns>
		public static string CentsToPrice(int priceInCents, string cultureInfo)
		{
			return CentsToPrice(priceInCents, cultureInfo, true);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents, string cultureInfo)
		{
			int price;
			int.TryParse(priceInCents, out price);

			return CentsToPrice(price, cultureInfo, true);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="useCurrencySign"></param>
		/// <returns></returns>
		public static string CentsToPrice(int priceInCents, bool useCurrencySign)
		{
			return CentsToPrice(priceInCents, null, useCurrencySign);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="useCurrencySign"></param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents, bool useCurrencySign)
		{
			int price;
			int.TryParse(priceInCents, out price);

			return CentsToPrice(price, null, useCurrencySign);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is null output is .ToString("F")
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <param name="useCurrencySign"> </param>
		/// <returns></returns>
		public static string CentsToPrice(string priceInCents, string cultureInfo, bool useCurrencySign)
		{
			int price;
			int.TryParse(priceInCents, out price);

			return CentsToPrice(price, cultureInfo, useCurrencySign);
		}

		/// <summary>
		/// Converts the price in cents to a full decimal separated price
		/// If culture is null output is .ToString("F")
		/// If culture is given output is .ToString("C")
		/// </summary>
		/// <param name="priceInCents"></param>
		/// <param name="cultureInfo">The cultureInfo to use (ex: 'en-US')</param>
		/// <param name="useCurrencySign"> </param>
		/// <returns></returns>
		public static string CentsToPrice(int priceInCents, string cultureInfo, bool useCurrencySign)
		{
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			var currentUICulture = Thread.CurrentThread.CurrentUICulture;

			var normalPrice = priceInCents/100m;

			string outputString;

			if (cultureInfo == null)
			{
				var outputPrice = Math.Round(normalPrice, 2);

				outputString = outputPrice.ToString("F");
				if (useCurrencySign)
				{
					outputString = outputPrice.ToString("C");
				}

				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
			else
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);

				var outputPrice = Math.Round(normalPrice, 2);

				outputString = outputPrice.ToString("F");
				if (useCurrencySign)
				{
					outputString = outputPrice.ToString("C");
				}

				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}

			return outputString;
		}
	}
}