using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Repositories;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// Helper class with order related functions
	/// </summary>
	public static class OrderHelper
	{
		/*
		 * Legacy API
		 * todo: too much business logic
		 */

		private static IOrderRepository OrderRepository
		{
			get { return IO.Container.Resolve<IOrderRepository>(); }
		}
		private static IOrderService OrderService
		{
			get { return IO.Container.Resolve<IOrderService>(); }
		}

		/// <summary>
		/// Generates the Order cookie name
		/// </summary>
		/// <param name="isOrderComplete">Whether the order is complete or not</param>
		public static string GetOrderCookieName(bool isOrderComplete = false)
		{
			var uwebshopConfiguration = UwebshopConfiguration.Current;
			string cookiePrefix = (isOrderComplete ? "CompletedOrderId" : "OrderId");
			return cookiePrefix + (uwebshopConfiguration == null || uwebshopConfiguration.ShareBasketBetweenStores ? string.Empty : StoreHelper.GetCurrentStore().UrlName);
		}

		/// <summary>
		/// Sets the order cookie.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public static void SetOrderCookie(OrderInfo orderInfo)
		{
			var cookieName = GetOrderCookieName();

			var minutesToAdd = 360;

			var incompleteOrderLifetime = IO.Container.Resolve<ISettingsService>().IncompleteOrderLifetime;
			if (incompleteOrderLifetime != 0)
			{
				minutesToAdd = incompleteOrderLifetime;
			}

			var expireDateTime = DateTime.Now;
			expireDateTime = expireDateTime.AddMinutes(minutesToAdd);

			var cookie = new HttpCookie(cookieName, orderInfo.UniqueOrderId.ToString())
			{
				Expires = expireDateTime
			};

			HttpContext.Current.Response.Cookies.Set(cookie);
		}

		/// <summary>
		/// Get the completed order cookie.
		/// </summary>
		public static string GetCompletedOrderCookie()
		{
			// todo: use UwebshopRequest for cache
			var cookieName = GetOrderCookieName(true);

			var orderIdCookie = HttpContext.Current.Request.Cookies[cookieName];

			return orderIdCookie != null ? orderIdCookie.Value : null;
		}

		/// <summary>
		/// Sets the order cookie.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public static void SetCompletedOrderCookie(OrderInfo orderInfo)
		{
			var cookieName = GetOrderCookieName(true);

			var cookie = new HttpCookie(cookieName, orderInfo.UniqueOrderId.ToString()) {Expires = DateTime.Now.AddMinutes(10)};

			HttpContext.Current.Response.Cookies.Set(cookie);
		}

		/// <summary>
		/// Removes the order cookie.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public static void RemoveOrderCookie(OrderInfo orderInfo)
		{
			var cookieName = GetOrderCookieName();

			var cookie = new HttpCookie(cookieName, string.Empty) {Expires = DateTime.Now.AddMinutes(-1)};

			HttpContext.Current.Response.Cookies.Set(cookie);
		}

		private static int _logCounter;
		internal static void LogThis(string message)
		{
			//Log.Instance.LogDebug(_logCounter++ + " " + message);
		}

		/// <summary>
		/// Get order based on the uniqueOrderId
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns></returns>
		public static OrderInfo GetOrder(Guid uniqueOrderId)
		{
			LogThis("GetOrder(" + uniqueOrderId.ToString() + ")");
			if (uniqueOrderId == default(Guid) || uniqueOrderId == Guid.Empty) return null;
			return OrderRepository.GetOrderInfo(uniqueOrderId);
		}
		
		/// <summary>
		/// Get order based on the transaction Id of the payment provider
		/// </summary>
		/// <param name="transactionId">The transaction unique identifier.</param>
		/// <returns></returns>
		public static OrderInfo GetOrder(string transactionId)
		{
			LogThis("GetOrder(" + transactionId + ")");
			return OrderRepository.GetOrderInfo(transactionId);
		}

		internal static OrderInfo GetCurrentBasketOrNewIfNotIncomplete()
		{
			var order = GetUnmodifiedCurrentOrder();

			if (order != null && order.Status != OrderStatus.Incomplete)
			{
				order = null;
			}

			return order ?? CreateOrder();
		}

		internal static OrderInfo GetUnmodifiedCurrentOrder()
		{
			LogThis("GetUnmodifiedCurrentOrder()");
			var uwebshopRequest = UwebshopRequest.Current;
			var orderInfo = uwebshopRequest.OrderInfo;

			if (orderInfo == null)
			{
				LogThis("GetUnmodifiedCurrentOrder() get cookie");
				var cookieGuid = OrderService.GetOrderIdFromOrderIdCookie();

				if (cookieGuid == Guid.Empty)
				{
					return null;
				}
				
				orderInfo = GetOrder(cookieGuid);

				if (IsCompletedOrderWithinValidLifetime(orderInfo) &&
				  orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OnlinePayment &&
				  orderInfo.Paid == true)
				{
					return null;
				}

				if (IsCompletedOrderWithinValidLifetime(orderInfo) &&
				  orderInfo.PaymentInfo.PaymentType != PaymentProviderType.OnlinePayment)
				{
					return null;
				}

				uwebshopRequest.OrderInfo = orderInfo;
			}
			else
			{
				// order from request
				// is order marked as completed?
				// AND is order set to use online payment provider?
				// AND is payment not failed?
				if (IsCompletedOrderWithinValidLifetime(orderInfo) && 
					orderInfo.PaymentInfo.PaymentType == PaymentProviderType.OnlinePayment &&
					orderInfo.Paid == true)
				{
					// the return null, so a new order should be made instead of having access to this one...
					return null;
				}
				if (IsCompletedOrderWithinValidLifetime(orderInfo) &&
				  orderInfo.PaymentInfo.PaymentType != PaymentProviderType.OnlinePayment)
				{
					return null;
				}
			}
			return orderInfo;
		}

		/// <summary>
		/// Get Current Order based on the cookie value of the customer
		/// if status is NOT incomplete and NOT waiting for paymentprovider and NOT paid it means the order is in 'payment
		/// progress'
		/// action: create a new COPY of the current order so the customer can't change the original order.
		/// if status is NOT incomplete, and status is PAID or status is OfflinePayment, return null.
		/// </summary>
		/// <param name="overruleCopyOrderOnConfirmedOrder">if set to <c>true</c> don't make a new order when the current one is confirmed.</param>
		/// <returns></returns>
		public static OrderInfo GetOrder(bool overruleCopyOrderOnConfirmedOrder = false)
		{
			var orderInfo = GetUnmodifiedCurrentOrder();
			return GetOrder(orderInfo, overruleCopyOrderOnConfirmedOrder);
		}

		/// <summary>
		/// Get Current Order based on the given order. If there is no given order, try get orderInfo from cookie
		/// if status is NOT incomplete and NOT waiting for paymentprovider and NOT paid it means the order is in 'payment
		/// progress'
		/// action: create a new COPY of the current order so the customer can't change the original order.
		/// if status is NOT incomplete, and status is PAID or status is OfflinePayment, return null.
		/// </summary>
		/// <param name="orderInfo"></param>
		/// <param name="overruleCopyOrderOnConfirmedOrder">if set to <c>true</c> don't make a new order when the current one is confirmed.</param>
		/// <returns></returns>
		public static OrderInfo GetOrder(OrderInfo orderInfo, bool overruleCopyOrderOnConfirmedOrder = false)
		{
			if (orderInfo == null)
			{
				return null;
			}
			LogThis("GetOrder(OrderInfo, bool) " + overruleCopyOrderOnConfirmedOrder + "  " + orderInfo.UniqueOrderId);
			if (overruleCopyOrderOnConfirmedOrder)
			{
				return orderInfo;
			}

			// if status is NOT incomplete, and status is PAID or status is OfflinePayment, or if status is confirmed without a payment provider (only possible if there are no payment providers created) return null.
			if (orderInfo.Status == OrderStatus.OfflinePayment || orderInfo.Status != OrderStatus.Incomplete && orderInfo.Paid == true || orderInfo.Status == OrderStatus.Confirmed && orderInfo.PaymentInfo.Id == 0)
			{
				return null;
			}
			
			// if status is NOT incomplete and NOT waiting for paymentprovider and NOT paid it means the order is in 'payment progress'
			// action: create a new COPY of the current order so the customer can't change the original order.
			if (orderInfo.Status != OrderStatus.OfflinePayment && orderInfo.Status != OrderStatus.Incomplete && orderInfo.Status != OrderStatus.WaitingForPaymentProvider && orderInfo.Paid != true)
			{
				orderInfo = OrderService.CreateCopyOfOrder(orderInfo);
			}

			return orderInfo;
		}

		/// <summary>
		/// Get Current Order based on the cookie value of the customer
		/// if status is NOT incopmlete and NOT waiting for paymentprovider and NOT paid it means the order is in 'payment
		/// progress'
		/// action: create a new COPY of the current order so the customer can't change the original order.
		/// if status is NOT incomplete, and status is PAID or status is OfflinePayment, create a new order
		/// </summary>
		/// <returns></returns>
		public static OrderInfo GetOrCreateOrder()
		{
			LogThis("GetOrCreateOrder()");
			var orderInfo = GetOrder() ?? CreateOrder();

			UwebshopRequest.Current.OrderInfo = orderInfo;
			return orderInfo;
		}
		
		/// <summary>
		/// Get order for current customer
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns></returns>
		public static XmlDocument GetOrderXML(Guid uniqueOrderId)
		{
			return OrderRepository.GetOrderInfoXML(uniqueOrderId);
		}

		/// <summary>
		/// Returns all the orders from a customer (member) Id;
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="includeIncomplete">if set to <c>true</c> [include incomplete].</param>
		/// <returns></returns>
		public static IEnumerable<OrderInfo> GetOrdersForCustomer(int customerId, string storeAlias = null, bool includeIncomplete = false)
		{
			return OrderRepository.GetOrdersFromCustomer(customerId, storeAlias, includeIncomplete); 
		}

		/// <summary>
		/// Returns all the order from a customer (member) Login name;
		/// </summary>
		/// <param name="userName">Name of the login.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="includeIncomplete">if set to <c>true</c> [include incomplete].</param>
		/// <returns></returns>
		public static IEnumerable<OrderInfo> GetOrdersForCustomer(string userName, string storeAlias = null, bool includeIncomplete = false)
		{
			return OrderRepository.GetOrdersFromCustomer(userName, storeAlias, includeIncomplete);
		}


		/// <summary>
		/// Returns all the orders from a customer (member) Id;
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<OrderInfo> GetWishlistsForCustomer(int customerId, string storeAlias = null)
		{
			return OrderRepository.GetWishlistsFromCustomer(customerId, storeAlias);
		}

		/// <summary>
		/// Returns all the order from a customer (member) Login name;
		/// </summary>
		/// <param name="userName">Name of the login.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="includeIncomplete">if set to <c>true</c> [include incomplete].</param>
		/// <returns></returns>
		public static IEnumerable<OrderInfo> GetWishlistsForCustomer(string userName, string storeAlias = null)
		{
			return OrderRepository.GetWishlistsFromCustomer(userName, storeAlias);
		}
		
		/// <summary>
		/// Gets all orders.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<OrderInfo> GetAllOrders(string storeAlias = null)
		{
			return storeAlias != null
				? OrderRepository.GetAllOrders()
					.Where(x => x.StoreInfo.Alias.ToLowerInvariant() == storeAlias.ToLowerInvariant())
				: OrderRepository.GetAllOrders();
		}

		/// <summary>
		/// Check if VAT should be applied for this order
		/// </summary>
		/// <returns>
		/// The amount with or without vat
		/// </returns>
		public static decimal GetTotalAmountUsingVatCheck()
		{
			return GetOrder().ChargedAmountInCents/100m;
		}

		/// <summary>
		/// Check if VAT should be applied for this order
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns>
		/// The amount with or without vat
		/// </returns>
		public static decimal GetTotalAmountUsingVatCheck(Guid uniqueOrderId)
		{
			return GetTotalAmountUsingVatCheckInCents(uniqueOrderId)/100m;
		}

		/// <summary>
		/// Gets the total amount using vat check in cents.
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns></returns>
		public static int GetTotalAmountUsingVatCheckInCents(Guid uniqueOrderId)
		{
			return GetOrder(uniqueOrderId).ChargedAmountInCents;
		}

		/// <summary>
		/// Returns customer orderline value from a specific order and orderline
		/// </summary>
		/// <param name="orderInfo">The order</param>
		/// <param name="orderLineId">Id of the orderline</param>
		/// <param name="alias">the fieldalias</param>
		/// <returns></returns>
		public static string CustomOrderLineValue(OrderInfo orderInfo, int orderLineId, string alias)
		{
			var orderline = orderInfo.OrderLines.FirstOrDefault(x => x.OrderLineId == orderLineId);
			return orderline == null ? string.Empty : ExtraInformationValueHelper(alias, orderline.CustomData);
		}

		/// <summary>
		/// Returns the value based
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static string CustomerInformationValue(OrderInfo orderInfo, string alias)
		{
			return ExtraInformationValueHelper(alias, orderInfo.CustomerInfo.CustomerInformation);
		}

		/// <summary>
		/// Shippings the information value.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static string ShippingInformationValue(OrderInfo orderInfo, string alias)
		{
			return ExtraInformationValueHelper(alias, orderInfo.CustomerInfo.ShippingInformation);
		}

		/// <summary>
		/// Gets the extra information value with given alias.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="alias">The alias.</param>
		/// <returns></returns>
		public static string ExtraInformationValue(OrderInfo orderInfo, string alias)
		{
			return ExtraInformationValueHelper(alias, orderInfo.CustomerInfo.ExtraInformation);
		}

		private static string ExtraInformationValueHelper(string alias, XElement data)
		{
			if (data != null)
			{
				var element = data.Element(alias);
				if (element != null)
				{
					var value = element.Value;
					if (!string.IsNullOrWhiteSpace(value)) return value;
				}
			}
			if (alias == "customerEmail")
			{
				var member = Membership.GetUser(); // todo: decouple
				if (member != null) return member.Email;
			}
			var profile = HttpContext.Current.Profile;

			if (profile != null && !profile.IsAnonymous)
			{
				try
				{
					var profileAlias = profile[alias];

					if (profileAlias != null) return profileAlias.ToString();
				}
				catch
				{
					return string.Empty;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Orders the contains item.
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <param name="itemIdsToCheck">The item ids automatic check.</param>
		/// <returns></returns>
		public static bool OrderContainsItem(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			return GetApplicableOrderLines(orderinfo, itemIdsToCheck).Any();
		}

		/// <summary>
		/// Gets the applicable order lines.
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <param name="itemIdsToCheck">The item ids automatic check.</param>
		/// <returns></returns>
		public static List<OrderLine> GetApplicableOrderLines(OrderInfo orderinfo, IEnumerable<int> itemIdsToCheck)
		{
			var applicableOrderLines = OrderService.GetApplicableOrderLines(orderinfo, itemIdsToCheck);
			return orderinfo.OrderLines.Where(line => applicableOrderLines.Any(l => l.OrderLineId == line.OrderLineId)).ToList();
		}

		[Obsolete("Use HandlePaymentRequest")]
		public static string GetRedirectUrlAfterConfirmation(OrderInfo orderInfo, int confirmedNodeId)
		{
			return HandlePaymentRequest(orderInfo, confirmedNodeId);
		}

		/// <summary>
		/// Gets the redirect URL after confirmation.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="confirmedNodeId">The confirmed node unique identifier.</param>
		/// <returns></returns>
		public static string HandlePaymentRequest(OrderInfo orderInfo, int confirmedNodeId)
		{
			var paymentProvider = PaymentProvider.GetPaymentProvider(orderInfo.PaymentInfo.Id);

			var currentDomain = HttpContext.Current.Request.Url.Authority;

			if (paymentProvider != null)
			{
				Log.Instance.LogDebug("HandlePaymentRequest paymentProvider: " + paymentProvider.Title + " " + paymentProvider.Type);

				switch (paymentProvider.Type)
				{
					case PaymentProviderType.OfflinePaymentAtCustomer:
					case PaymentProviderType.OfflinePaymentInStore:
						if (!string.IsNullOrEmpty(paymentProvider.SuccesNodeId))
						{
							int succesNodeId;

							int.TryParse(paymentProvider.SuccesNodeId, out succesNodeId);

							if (succesNodeId != 0)
							{
								var urlToReturn = IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(succesNodeId);

								urlToReturn = string.Format("//{0}{1}", currentDomain, urlToReturn);
								Log.Instance.LogDebug("HandlePaymentRequest SuccesNodeId: " + urlToReturn);

								return urlToReturn;
							}
						}
						break;
					case PaymentProviderType.OnlinePayment:
						var iPaymentProvider = PaymentProviderHelper.GetAllIPaymentProviders().FirstOrDefault(x => x.GetName().ToLowerInvariant() == paymentProvider.Name.ToLowerInvariant());

						if (iPaymentProvider != null)
						{
							var handler = PaymentProviderHelper.GetAllPaymentRequestHandlers().FirstOrDefault(x => x.GetName().ToLowerInvariant() == paymentProvider.Name.ToLowerInvariant());

							if (handler != null)
							{
								try
								{
									orderInfo.PaymentInfo.TransactionMethod = iPaymentProvider.GetParameterRenderMethod();
									var handlerResult = handler.CreatePaymentRequest(orderInfo);
									orderInfo.Save();

									if (handlerResult == null)
									{
										Log.Instance.LogError("HandlePaymentRequest handler.CreatePaymentRequest(orderInfo) == null");
										return "failed";
									}
								}
								catch (Exception ex)
								{
									Log.Instance.LogError("HandlePaymentRequest handler.CreatePaymentRequest(orderInfo) FAILED " + ex);
									return "failed";
								}
							}

							switch (iPaymentProvider.GetParameterRenderMethod())
							{
								case PaymentTransactionMethod.Form:
								case PaymentTransactionMethod.Custom:
									if (!string.IsNullOrEmpty(paymentProvider.ControlNodeId))
									{
										int controlNodeId;

										int.TryParse(paymentProvider.ControlNodeId, out controlNodeId);

										if (controlNodeId != 0)
										{
											string urlToReturn = IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(controlNodeId);

											urlToReturn = string.Format("//{0}{1}", currentDomain, urlToReturn);

											Log.Instance.LogDebug("HandlePaymentRequest ControlNodeId: " + urlToReturn);
											return urlToReturn;
										}
									}
									Log.Instance.LogWarning("HandlePaymentRequest PaymentTransactionMethod.Form/Custom FAILED - paymentProvider.ControlNodeId not found: try fallback to orderInfo.PaymentInfo.Url");
									if (!string.IsNullOrWhiteSpace(orderInfo.PaymentInfo.Url))
									{
										string value = string.Format("{0}{1}", orderInfo.PaymentInfo.Url, "?" + orderInfo.PaymentInfo.Parameters);

										if (string.IsNullOrEmpty(orderInfo.PaymentInfo.Parameters))
										{
											value = orderInfo.PaymentInfo.Url;
										}

										Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.QueryString: " + value);

										return value;
									}
									Log.Instance.LogWarning("HandlePaymentRequest PaymentTransactionMethod.Form/Custom FAILED");
									return "failed";
								case PaymentTransactionMethod.QueryString:
									if (!string.IsNullOrWhiteSpace(orderInfo.PaymentInfo.Url))
									{
										string value = string.Format("{0}{1}", orderInfo.PaymentInfo.Url, "?" + orderInfo.PaymentInfo.Parameters);

										if (string.IsNullOrEmpty(orderInfo.PaymentInfo.Parameters))
										{
											value = orderInfo.PaymentInfo.Url;
										}

										Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.QueryString: " + value);

										return value;
									}
									Log.Instance.LogError("HandlePaymentRequest PaymentTransactionMethod.QueryString FAILED");
									return "failed";
								case PaymentTransactionMethod.ServerPost:

									if (handler != null)
									{
										Log.Instance.LogDebug("HandlePaymentRequest paymentRequestHandlers.GetName(): " + handler.GetName());

										string nextURL = handler.GetPaymentUrl(orderInfo);

										Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.ServerPost nextURL: " + nextURL);


										if (!string.IsNullOrEmpty(nextURL))
										{
											Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.ServerPost nextURL: " + nextURL);

											return nextURL;
										}

										Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.ServerPost OrderInfo.PaymentInfo.Url: " + orderInfo.PaymentInfo.Url);

										if (!string.IsNullOrEmpty(orderInfo.PaymentInfo.Url))
										{
											Log.Instance.LogDebug("HandlePaymentRequest PaymentTransactionMethod.ServerPost OrderInfo.PaymentInfo.Url: " + orderInfo.PaymentInfo.Url);

											return orderInfo.PaymentInfo.Url;
										}
									}
									Log.Instance.LogError("HandlePaymentRequest PaymentTransactionMethod.ServerPost FAILED: " + paymentProvider.Name);
									return "failed";
								case PaymentTransactionMethod.WebClient:
									return "webclient";
								case PaymentTransactionMethod.Inline:

									if (handler != null)
									{
										return handler.GetPaymentUrl(orderInfo);
									}
									return "inline";
							}
						}
						Log.Instance.LogError("HandlePaymentRequest With Online Payment FAILED");
						return "failed";
				}
			}

			if (confirmedNodeId != 0)
			{
				string confirmNodeId = confirmedNodeId != 0 ? IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(confirmedNodeId) : string.Empty;

				confirmNodeId = string.Format("//{0}{1}", currentDomain, confirmNodeId);

				return confirmNodeId;
			}

			string fallback = string.Format("//{0}", currentDomain);

			//Log.Instance.LogDebug( "HandlePaymentRequest fallback: " + fallback);

			return fallback;
		}

		/// <summary>
		/// Update the Stock of all products variants and discounts of an order
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public static void UpdateStock(OrderInfo orderInfo)
		{
			Log.Instance.LogDebug("UpdateStock Start");

			if (orderInfo.StockUpdated)
			{
				Log.Instance.LogDebug("UpdateStock: Stock Already Updated: Return");
				return;
			}

			var stockService = IO.Container.Resolve<IStockService>();

			// todo: extremely inefficient (many queries) and not safe (no transaction)
			if (orderInfo.StoreInfo.Store != null && orderInfo.StoreInfo.Store.UseStock)
			{
				Log.Instance.LogDebug("UpdateStock: START stock enabled, store not null");

				if (orderInfo.OrderLines == null)
				{
					Log.Instance.LogDebug("UpdateStock: OrderLines NOT Found");
					return;
				}

				foreach (var orderLine in orderInfo.OrderLines)
				{
					Log.Instance.LogDebug("UpdateStock: OrderLinesFound");
					// if UseVariantStock, only update variant stock
					if (orderLine.ProductInfo.CatalogProduct == null)
					{
						Log.Instance.LogDebug("UpdateStock: orderLine.ProductInfo.Product == NULL");
					}
					else if (orderLine.ProductInfo.CatalogProduct.UseVariantStock && orderLine.ProductInfo.CatalogProduct.GetAllVariants().Any())
					{
						Log.Instance.LogDebug("UpdateStock: start product usevariantstock");
						// if variant stockstatus is enabled update stockstatus + ordercount
						foreach (var variant in orderLine.ProductInfo.ProductVariants.Where(x => x.Variant.StockStatus))
						{
							Log.Instance.LogDebug("UpdateStock (and ordercount): start product usevariantstock - variant.id: " + variant.Id);
							stockService.SetStock(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), true, orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}

						// if variant stockstatus is disabled update ordercount
						foreach (var variant in orderLine.ProductInfo.ProductVariants.Where(x => !x.Variant.StockStatus))
						{
							Log.Instance.LogDebug("UpdateOrderCount (not stock): start product usevariantstock - variant.id: " + variant.Id);
							stockService.SetOrderCount(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}
					}
						// if UseVariantStock FALSE, update variant stock + product stock
						//if (orderLine.ProductInfo.Product != null && !orderLine.ProductInfo.Product.UseVariantStock)
					else
					{
						Log.Instance.LogDebug("UpdateStock: orderLine.ProductInfo.Product.UseVariantStock == false");
						// if product stockstatus is enabled update stockstatus + ordercount
						if (orderLine.ProductInfo.CatalogProduct.StockStatus)
						{
							stockService.SetStock(orderLine.ProductInfo.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), true, orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}
							// if product stockstatus is disabled update ordercount
						else
						{
							stockService.SetOrderCount(orderLine.ProductInfo.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}

						// if variant stockstatus is enabled update stockstatus + ordercount
						foreach (var variant in orderLine.ProductInfo.ProductVariants.Where(x => x.Variant.StockStatus))
						{
							stockService.SetStock(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), true, orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}

						// if variant stockstatus is disabled update ordercount
						foreach (var variant in orderLine.ProductInfo.ProductVariants.Where(x => !x.Variant.StockStatus))
						{
							stockService.SetOrderCount(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}
					}
				}
				Log.Instance.LogDebug("UpdateStock: END stock enabled, store not null");
			}
				// if stock is disabled on the store 
			else if (orderInfo.StoreInfo.Store != null && !orderInfo.StoreInfo.Store.UseStock)
			{
				Log.Instance.LogDebug("UpdateStock: START stock disabled, store not null");
				foreach (var orderLine in orderInfo.OrderLines)
				{
					// if UseVariantStock, only update variant ordercount
					if (orderLine.ProductInfo.CatalogProduct.UseVariantStock)
					{
						foreach (var variant in orderLine.ProductInfo.ProductVariants)
						{
							stockService.SetOrderCount(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}
					}
						// if UseVariantStock FALSE, update variant ordercount + product ordercount
					else
					{
						stockService.SetOrderCount(orderLine.ProductInfo.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);

						foreach (var variant in orderLine.ProductInfo.ProductVariants)
						{
							stockService.SetOrderCount(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
						}
					}
				}

				Log.Instance.LogDebug("UpdateStock: END stock disabled, store not null");
			}

			// preserve the discounts before stock is substracted
			var orderDiscounts = orderInfo.Discounts.ToList();
			orderInfo.OrderDiscountsFactory = () => orderDiscounts;

			// update the discount stock
			foreach (var orderDiscount in orderInfo.Discounts.Where(discount => discount.CounterEnabled))
			{
				Log.Instance.LogDebug("UpdateStock: start discount stock");
				stockService.SetStock(orderDiscount.OriginalId, 1, false, orderInfo.StoreInfo.Store != null && orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
			}
			foreach (var discountProduct in orderInfo.OrderLines.Select(l => DiscountHelper.GetProductDiscount(l.ProductInfo.DiscountId)).Where(discount => discount != null && discount.CounterEnabled))
			{
				Log.Instance.LogDebug("UpdateStock: start discount stock");
				stockService.SetStock(discountProduct.Id, 1, false, orderInfo.StoreInfo.Store != null && orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
			}

			var couponCodeService = IO.Container.Resolve<ICouponCodeService>();
			var discounts = orderInfo.Discounts.Select(d => d.Id).Concat(orderInfo.OrderLines.Select(l => l.ProductInfo.DiscountId));
			foreach (var discountId in discounts)
			{
				var coupons = couponCodeService.GetAllForDiscount(discountId).Where(c => c.NumberAvailable > 0).Where(c => orderInfo.CouponCodes.Contains(c.CouponCode));
				couponCodeService.DecreaseCountByOneFor(coupons);
			}

			Log.Instance.LogDebug("before stockupdated");
			orderInfo.StockUpdated = true;
			Log.Instance.LogDebug("after stockupdated");
			orderInfo.Save();

			Log.Instance.LogDebug("UpdateStock End (after save)");
		}

		/// <summary>
		/// Return the Stock of all products and variants and discounts of an order
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public static void ReturnStock(OrderInfo orderInfo)
		{
			if (!orderInfo.StockUpdated) return;

			var stockService = IO.Container.Resolve<IStockService>();

			// todo: extremely inefficient (many queries) and not safe (no transaction)
			if (orderInfo.StoreInfo.Store != null && orderInfo.StoreInfo.Store.UseStock)
			{
				foreach (var orderLine in orderInfo.OrderLines)
				{
					stockService.ReturnStock(orderLine.ProductInfo.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), true, orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);

					foreach (var variant in orderLine.ProductInfo.ProductVariants.Where(x => x.Variant.StockStatus))
					{
						stockService.ReturnStock(variant.Id, orderLine.ProductInfo.ItemCount.GetValueOrDefault(1), true, orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
					}
				}
			}

			foreach (var orderDiscount in orderInfo.Discounts.Where(discount => discount.CounterEnabled))
			{
				stockService.ReturnStock(orderDiscount.OriginalId, 1, false, orderInfo.StoreInfo.Store != null && orderInfo.StoreInfo.Store.UseStoreSpecificStock ? orderInfo.StoreInfo.Alias : string.Empty);
			}

			orderInfo.StockUpdated = false;
			orderInfo.Save();
		}

		internal static void CopyCustomerToShipping(OrderInfo orderInfo)
		{
			if (orderInfo.CustomerInfo.ShippingInformation == null || (orderInfo.CustomerInfo.ShippingInformation != null && !orderInfo.CustomerInfo.ShippingInformation.Descendants().Any()))
			{
				var xDoc = new XDocument(new XElement(CustomerDatatypes.Shipping.ToString()));
				if (orderInfo.CustomerInfo.customerInformation != null && orderInfo.CustomerInfo.customerInformation.Root != null)
				{
					foreach (var node in orderInfo.CustomerInfo.customerInformation.Root.Elements())
					{
						if (xDoc.Root != null)
							xDoc.Root.Add(new XElement(node.Name.ToString().Replace("customer", "shipping"), new XCData(node.Value)));
					}

					orderInfo.CustomerInfo.shippingInformation = xDoc;
				}

				orderInfo.Save();
			}
		}

		#region OrderService forwarding

		/// <summary>
		/// Is the order orderable?
		/// </summary>
		/// <param name="orderinfo">The orderinfo.</param>
		/// <returns></returns>
		public static bool Orderable(OrderInfo orderinfo)
		{
			return !OrderService.OrderContainsOutOfStockItem(orderinfo);
		}

		/// <summary>
		/// Check if the order contains items out of stock
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns></returns>
		public static bool OrderContainsOutOfStockItems(Guid uniqueOrderId)
		{
			return OrderService.OrderContainsOutOfStockItem(GetOrder(uniqueOrderId));
		}

		/// <summary>
		/// Creates the new order from existing.
		/// </summary>
		/// <param name="orderInfoToCopyFrom">The order information automatic copy from.</param>
		/// <returns></returns>
		public static OrderInfo CreateNewOrderFromExisting(OrderInfo orderInfoToCopyFrom)
		{
			return OrderService.CreateCopyOfOrder(orderInfoToCopyFrom);
		}

		/// <summary>
		/// Create Order, using the current store
		/// </summary>
		/// <returns></returns>
		public static OrderInfo CreateOrder()
		{
			LogThis("CreateOrder()");
			return OrderService.CreateOrder();
		}

		/// <summary>
		/// Returns the order Guid based on the "OrderId" cookie of the customer
		/// </summary>
		/// <returns></returns>
		public static Guid OrderIdFromOrderIdCookie()
		{
			return OrderService.GetOrderIdFromOrderIdCookie();
		}

		/// <summary>
		/// Create Order
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns></returns>
		public static OrderInfo CreateOrder(Store store)
		{
			var order = OrderService.CreateOrder(store);
			OrderRepository.LegacyStoreOrder(order);
			return order;
		}

		/// <summary>
		/// Validates the order.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <returns></returns>
		public static bool ValidateOrder(OrderInfo orderInfo)
		{
			if (orderInfo == null) throw new ArgumentNullException("orderInfo");

			Log.Instance.LogDebug("ValidateOrder orderInfo.TermsAccepted: " + orderInfo.TermsAccepted);

			return !OrderService.ValidateOrder(orderInfo, true).Any();
		}

		/// <summary>
		/// Validates the customer.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <returns></returns>
		public static bool ValidateCustomer(OrderInfo orderInfo, bool clearValidation)
		{
			return OrderService.ValidateCustomer(orderInfo, clearValidation);
		}

		/// <summary>
		/// Validates the stock.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <returns></returns>
		public static bool ValidateStock(OrderInfo orderInfo, bool clearValidation)
		{
			return OrderService.ValidateStock(orderInfo, clearValidation);
		}

		/// <summary>
		/// Validates the order lines.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <returns></returns>
		public static bool ValidateOrderLines(OrderInfo orderInfo, bool clearValidation)
		{
			return OrderService.ValidateOrderlines(orderInfo, clearValidation);
		}

		/// <summary>
		/// Validates the custom validation.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="clearValidation">if set to <c>true</c> [clear validation].</param>
		/// <returns></returns>
		public static bool ValidateCustomValidation(OrderInfo orderInfo, bool clearValidation)
		{
			return OrderService.ValidateCustomValidations(orderInfo, clearValidation);
		}

		#endregion


		#region Obsolete public API

		/// <summary>
		/// Get order for current customer
		/// </summary>
		/// <param name="uniqueOrderId">The unique order unique identifier.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrderXML()")]
		public static XmlDocument GetOrderInfoXML(Guid uniqueOrderId)
		{
			return GetOrderXML(uniqueOrderId);
		}

		/// <summary>
		/// Get order based on the customer Id
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrdersForCustomer")]
		public static IEnumerable<OrderInfo> GetOrdersFromCustomer(int customerId)
		{
			return GetOrdersForCustomer(customerId);
		}

		/// <summary>
		/// Get order based on the customer Id
		/// </summary>
		/// <param name="customerUserName">The customer username</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrdersForCustomer")]
		public static IEnumerable<OrderInfo> GetOrdersFromCustomer(string customerUserName)
		{
			return GetOrdersForCustomer(customerUserName);
		}

		/// <summary>
		/// Gets the original create order information.
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrCreateOrder")]
		public static OrderInfo GetOrCreateOrderInfo()
		{
			return GetOrCreateOrder();
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrder()")]
		public static OrderInfo GetOrderInfo(Guid uniqueOrderId)
		{
			return GetOrder(uniqueOrderId);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrder()")]
		public static OrderInfo GetOrderInfo(string transactionId)
		{
			return GetOrder(transactionId);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrder()")]
		public static OrderInfo GetOrderInfo(OrderInfo orderInfo, bool overruleCopyOrderOnConfirmedOrder = false)
		{
			return GetOrder(orderInfo, overruleCopyOrderOnConfirmedOrder);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetOrder()")]
		public static OrderInfo GetOrderInfo(bool overruleCopyOrderOnConfirmedOrder = false)
		{
			return GetOrder(overruleCopyOrderOnConfirmedOrder);
		}

		#endregion

		/// <summary>
		/// Returns true if the order is complete, with a valid guid in the completed order Id and the confirm datetime is maximum 10 minutes old.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public static bool IsCompletedOrderWithinValidLifetime(OrderInfo order)
		{
			var completedOrderCookie = GetCompletedOrderCookie();
			return completedOrderCookie != null && (completedOrderCookie == order.UniqueOrderId.ToString() &&
													order.ConfirmDate.GetValueOrDefault().AddMinutes(10) >= DateTime.Now);
		}

		/// <summary>
		/// Returns true if the order is complete, with a valid guid in the completed order Id and the confirm datetime is maximum 10 minutes old.
		/// </summary>
		/// <param name="order"></param>
		/// <returns></returns>
		public static bool IsCompletedOrderWithinValidLifetime(IOrder order)
		{
			var completedOrderCookie = GetCompletedOrderCookie();
			return completedOrderCookie != null && (completedOrderCookie == order.UniqueId.ToString() &&
													order.ConfirmDate.AddMinutes(10) >= DateTime.Now);
		}
	}


}