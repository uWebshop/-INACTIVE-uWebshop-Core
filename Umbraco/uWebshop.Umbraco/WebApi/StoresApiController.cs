using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Web.Security;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Services;
using Umbraco.Core.IO;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence.Querying;
using System.Dynamic;
using uWebshop.Umbraco.SaleManager;
using System.Configuration;

namespace uWebshop.Umbraco.WebApi
{
	[PluginController("uWebshop")]
	[KnownType(typeof(BasketOrderInfoAdaptor))]
	public class StoreApiController : UmbracoAuthorizedApiController
	{
		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.HttpGet]
		public bool IsBackendUserLoggedIn()
		{
			return IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated;
		}
		
		public IEnumerable<string> GetAllStoreAliasses()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return StoreHelper.GetAllStores().Select(s => s.Alias);
			}
			return Enumerable.Empty<string>();
		}

		public IEnumerable<IStore> GetAllStores()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return StoreHelper.GetAllStores();
			}
			return Enumerable.Empty<IStore>();
		}

        public object SendOrderEmail(Guid orderId, string status)
        {
            try
            {
                Log.Instance.LogWarning("Starting to send order email for order: " + orderId + " Status: " + status);

                if (!string.IsNullOrEmpty(status))
                {
                    var orderInfo = OrderHelper.GetOrder(orderId);

                    string customerTemplateId = string.Empty;
                    string storeTemplateId = string.Empty;

                    // Add more status here to expand functionality
                    if (status.ToLower() == "dispatched")
                    {
                        customerTemplateId = orderInfo.StoreInfo.Store.DispatchEmailCustomer;
                        storeTemplateId = orderInfo.StoreInfo.Store.DispatchedEmailStore;
                    }

                    int customerEmailNodeId;
                    int.TryParse(customerTemplateId, out customerEmailNodeId);

                    int storeEmailNodeId;
                    int.TryParse(storeTemplateId, out storeEmailNodeId);

                    EmailHelper.SendOrderEmail(storeEmailNodeId, customerEmailNodeId, orderInfo);

                    return new
                    {
                        success = true
                    };

                } else
                {
                    return new
                    {
                        success = false,
                        error = "Status missing."
                    };
                }


            } catch(Exception ex)
            {
                Log.Instance.LogError(ex,"SendOrderEmail Error!");

                return new
                {
                    success = false,
                    error = ex.Message
                };
            }

        }

        public object SendOrderEmailWithTemplate(string orderId, string emailAddress, int templateId)
        {
            try
            {
                Log.Instance.LogWarning("Starting to send order email with template for order: " + orderId + " TemplateId: " + templateId);

                OrderInfo orderInfo = null;

                Guid OrderID = Guid.NewGuid();

                if (Guid.TryParse(orderId, out OrderID))
                {
                    orderInfo = OrderHelper.GetOrder(OrderID);
                }

                if (orderInfo == null)
                {
                    orderInfo = OrderHelper.GetOrder(orderId);
                }

                if (orderInfo != null)
                {
                    EmailHelper.SendSingleOrderEmail(templateId, orderInfo, emailAddress);

                    return new
                    {
                        success = true
                    };
                } else
                {
                    return new
                    {
                        success = false,
                        error = "Order Not Found!"
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Instance.LogError(ex, "SendOrderEmailWithTemplate Error!");

                return new
                {
                    success = false,
                    error = ex.Message
                };
            }

        }

        public object SaveValueForOrder(Guid orderId, string field, string value, string dataType = "customer")
        {
            try
            {
                Log.Instance.LogWarning("Starting to save value for order: " + orderId + " Field: " + field + " Value:" + value);

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    var orderInfo = OrderHelper.GetOrder(orderId);

                    Dictionary<string, string> c = new Dictionary<string, string>();
                    c.Add(field, value);

                    var type = CustomerDatatypes.Customer;

                    if (dataType == "shipping")
                    {
                        type = CustomerDatatypes.Shipping;
                    }
                    if (dataType == "extra")
                    {
                        type = CustomerDatatypes.Extra;
                    }
                    if (dataType == "Repeat")
                    {
                        type = CustomerDatatypes.Repeat;
                    }

                    orderInfo.AddCustomerFields(c, type);

                    IO.Container.Resolve<IOrderRepository>().SaveOrderInfo(orderInfo);

                    return new
                    {
                        success = true
                    };

                }
                else
                {
                    return new
                    {
                        success = false,
                        error = "Value or Field missing."
                    };
                }


            }
            catch (Exception ex)
            {
                Log.Instance.LogError(ex, "Save Value for Order Error!");

                return new
                {
                    success = false,
                    error = ex.Message
                };
            }
        }

        public IEnumerable<string> GetStoreSpecificStockStoreAliasses()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
                //return StoreHelper.GetAllStores().Where(x => x.UseStoreSpecificStock).Select(s => s.Alias);
                return StoreHelper.GetAllStores().Select(s => s.Alias);
            }
			return Enumerable.Empty<string>();
		}

		public BasketOrderInfoAdaptor GetOrderData(string guid)
		{
			var order = Orders.GetOrder(guid);
			var membershipUser = UwebshopRequest.Current.User;
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.UserName == order.Customer.UserName || UwebshopRequest.Current.PaymentProvider != null || OrderHelper.IsCompletedOrderWithinValidLifetime(order))
			{
				return order as BasketOrderInfoAdaptor;
			}
			return null;
		}

        public OrderDataInfo GetOrders(string storeAlias, string toDate, string fromDate, string status, string customerName = null, string customerEmail = null, string ordernumber = null, string uniqueId = null, string zipCode = null, string country = null, string paymentOption = null, string shippingOption = null, string discount = null)
        {

            try
            {
                var orders = new OrderDataInfo();

                string _fromDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.AddDays(-2).ToString("yyyyMMdd") : fromDate;
                string _toDate = string.IsNullOrEmpty(toDate) ? DateTime.Now.AddDays(1).ToString("yyyyMMdd") : toDate;


                if (status.ToLower() == "refundedorders")
                {
                    orders = CustomOrderHelper.GetRefundedOrders(true, false, storeAlias, _fromDate, _toDate);
                }
                else if (status.ToLower() == "abandondbaskets")
                {
                    orders = CustomOrderHelper.GetAbandondOrdersByStore(true, false, storeAlias, _fromDate, _toDate);
                }
                else if (status.ToLower() == "completedorders")
                {
                    orders = CustomOrderHelper.GetCompletedOrders(true, false, storeAlias, _fromDate, _toDate);
                }
                else if (status.ToLower() == "all" || string.IsNullOrEmpty(status))
                {
                    orders = CustomOrderHelper.GetOrdersByStore(true, false, storeAlias, _fromDate, _toDate);
                }
                else
                {
                    orders = CustomOrderHelper.GetOrdersByStatusAndStore(true, false, status, storeAlias, _fromDate, _toDate);
                }

                if (orders.list.Any()) {

                    if (!string.IsNullOrEmpty(customerName))
                    {
                        orders.list = orders.list.Where(x => (!string.IsNullOrEmpty(x.firstName) && x.firstName.ToLower().Contains(customerName.ToLower())) || (!string.IsNullOrEmpty(x.lastName) && x.lastName.ToLower().Contains(customerName.ToLower()))).ToList();
                    }

                    if (!string.IsNullOrEmpty(customerEmail))
                    {
                        orders.list = orders.list.Where(x => (!string.IsNullOrEmpty(x.customer.email) && x.customer.email.ToLower().Contains(customerEmail.ToLower()))).ToList();
                    }

                    if (!string.IsNullOrEmpty(ordernumber))
                    {
                        orders.list = orders.list.Where(x => !string.IsNullOrEmpty(x.refrenceId.ToString()) && x.refrenceId.ToString().ToLower().Contains(ordernumber.ToLower())).ToList();
                    }

                    if (!string.IsNullOrEmpty(uniqueId))
                    {
                        orders.list = orders.list.Where(x => (!string.IsNullOrEmpty(x.uniqueId.ToString()) && x.uniqueId.ToString().ToLower().Contains(uniqueId.ToLower()))).ToList();
                    }

                    if (!string.IsNullOrEmpty(zipCode))
                    {
                        orders.list = orders.list.Where(x => !string.IsNullOrEmpty(x.customer.zipCode) && x.customer.zipCode.Contains(zipCode)).ToList();
                    }

                    if (!string.IsNullOrEmpty(paymentOption))
                    {
                        orders.list = orders.list.Where(x => x.paymentInfo.id == Convert.ToInt32(paymentOption)).ToList();
                    }

                    if (!string.IsNullOrEmpty(shippingOption))
                    {
                        orders.list = orders.list.Where(x => x.shippingInfo.id == Convert.ToInt32(shippingOption)).ToList();
                    }

                    if (!string.IsNullOrEmpty(country))
                    {
                        orders.list = orders.list.Where(x => x.customer != null && !string.IsNullOrEmpty(x.customer.country) && x.customer.country == country).ToList();
                    }

                    if (!string.IsNullOrEmpty(discount))
                    {
                        orders.list = orders.list.Where(x => x.discountInfo != null && x.discountInfo.id == Convert.ToInt32(discount)).ToList();
                    }
                }

                string grandTotal = "";
                string averageAmountInBasket = "";
                string totalShippingCost = "";

                orders.totalQty = orders.list.Sum(x => x.quantity);
                orders.totalOrders = orders.list.Count();

                var allStores = uWebshop.API.Store.GetAllStores();

                if (!string.IsNullOrEmpty(storeAlias) && (status.ToLower() != "all" || !string.IsNullOrEmpty(status)) || allStores.Count() == 1)
                {
                    var cultureInfo = CustomOrderHelper.GetCultureByCurrencyCode(CustomOrderHelper.GetCurrencyCodeByStore(storeAlias));
                    decimal totalCharged = orders.list.Sum(x => x.chargedAmount);
                    decimal totalShipping = orders.list.Sum(x => x.chargedShippingAmount);

                    grandTotal = string.Format(cultureInfo, "{0:C}", orders.list.Sum(x => x.chargedAmount));
                    averageAmountInBasket = "0";
                    totalShippingCost = string.Format(cultureInfo, "{0:C}", totalShipping);

                    try
                    {
                        averageAmountInBasket = string.Format(cultureInfo, "{0:C}", Math.Round(totalCharged / (decimal)orders.totalOrders, 2));
                    } catch { }

                }

                orders.grandTotal = grandTotal;
                orders.averageQtyInBasket = "0";

                try
                {
                    orders.averageQtyInBasket = Math.Round((double)orders.totalQty / (double)orders.totalOrders, 2).ToString();
                } catch { }

                orders.averageAmountInBasket = averageAmountInBasket;
                orders.totalShippingCost = totalShippingCost;

                return orders;
            }
            catch (Exception ex) {
                Log.Instance.LogError(ex, "GetOrder Error!");
                return null;
            }

        }

        public object GetSalesDataForChart(string storeAlias, string toDate, string fromDate, string type = "month")
        {
            try
            {
                var today = DateTime.Today;

                DateTime _fromDate = string.IsNullOrEmpty(fromDate) ? new DateTime(today.Year, 1, 1) : Convert.ToDateTime(fromDate);
                DateTime _toDate = string.IsNullOrEmpty(toDate) ? new DateTime(today.Year, 12, 31) : Convert.ToDateTime(toDate);

                var list = new List<dynamic>();

                var runningDate = _fromDate;
                while (runningDate < _toDate)
                {
                    var nextMonthSeed = runningDate.AddMonths(1);
                    var to = DateHelper.Min(new DateTime(nextMonthSeed.Year, nextMonthSeed.Month, 1), _toDate);

                    var sales = CustomOrderHelper.GetCompletedSales(storeAlias, runningDate, to.AddDays(-1));

                    dynamic o = new ExpandoObject();
                    o.date = runningDate.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    o.total = sales;

                    list.Add(o);

                    runningDate = to;
                }

                return new
                {
                    success = true,
                    list = list
                };

            } catch(Exception ex)
            {

                Log.Instance.LogError(ex, "Get Sales Data Error!");

                return new
                {
                    success = false,
                    error = ex.Message
                };

            }
        }

        public object GetZeroStockVariants(string store)
        {
            // Get All Products
            var allProducts = uWebshop.API.Catalog.GetAllProducts(store);
            var AllEnabledVariants = new List<IProductVariant>();
            
            var list = new List<object>();

            foreach (var product in allProducts)
            {
                var allVariants = product.GetAllVariants();
                var variants = new List<IProductVariant>();

                // Filter out all variants that are disabled or unpublished
                foreach (var variant in allVariants)
                {
                    var path = variant.GetUmbracoPath();

                    var examineItemsFromPath = SmHelper.GetAllCatalogItemsFromPath(path);

                    if (!SmHelper.IsItemDisabled(examineItemsFromPath, store))
                    {
                        AllEnabledVariants.Add(variant);
                        variants.Add(variant);
                    }
                }

                // Filter all variants that have 0 stock and are Variant.
                foreach (IProductVariant variant in variants.Where(x => x.Stock <= 0))
                {

                    dynamic item = new ExpandoObject();

                    item.title = variant.Title;
                    item.id = variant.Id;
                    item.size = variant.GetProperty("size");
                    item.productTitle = product.Title;
                    item.color = variant.Color();
                    item.sku = variant.GetProperty("sku");

                    list.Add(item);
                }
            }

            return new {
                items = list,
                total = AllEnabledVariants.Count()
            };
        }

        public object GetZeroStockProducts(string store)
        {
            var products = uWebshop.API.Catalog.GetAllProducts(store).ToList();

            var productsWithZeroStock = products.Where(x => x.Stock <= 0).Select(x => new { title = x.Title, id = x.Id});

            return new {
                items = productsWithZeroStock,
                total = products.Count()
            };
        }

        public object GetSalesManagerConfig()
        {
            try
            {

                var storeRoot = ConfigurationManager.AppSettings["smStoreRoot"];
                var shippingRoot = ConfigurationManager.AppSettings["smShippingRoot"];
                var paymentRoot = ConfigurationManager.AppSettings["smPaymentRoot"];
                var discountRoot = ConfigurationManager.AppSettings["smDiscountRoot"];

                return new
                {
                    success = true,
                    storeRoot = storeRoot,
                    shippingRoot = shippingRoot,
                    paymentRoot = paymentRoot,
                    discountRoot = discountRoot
                };

            } catch(Exception ex)
            {
                Log.Instance.LogError(ex, "GetSalesManagerConfig Error!");

                return new
                {
                    success = false,
                    error = ex.Message
                };

            }
        }

        public static class DateHelper
        {
            public static DateTime Min(DateTime date1, DateTime date2)
            {
                return (date1 < date2 ? date1 : date2);
            }
        }

        public IEnumerable<OrderCustomer> GetCustomersMembers(string name, string email, bool boughtOnce)
        {

            var ms = Services.MemberService;

            List<IMember> members = new List<IMember>();

            int total = 0;

            if (!string.IsNullOrEmpty(name)) {
                members.AddRange(ms.GetMembersByPropertyValue("customerFirstName", name, StringPropertyMatchType.Contains));
                members.AddRange(ms.GetMembersByPropertyValue("customerLastName", name, StringPropertyMatchType.Contains));
            }

            if (!string.IsNullOrEmpty(email))
            {
                members.AddRange(ms.FindByEmail(email, 0, 99999, out total, StringPropertyMatchType.Contains));
            }

            return members.Distinct(x => x.Id).Select(x => new OrderCustomer()
            {
                firstName = x.GetValue<string>("customerFirstName"),
                lastName = x.GetValue<string>("customerLastName"),
                address1 = x.GetValue<string>("customerAddress1"),
                city = x.GetValue<string>("customerCity"),
                country = x.GetValue<string>("customerCountry"),
                email = x.Email,
                zipCode = x.GetValue<string>("customerZipCode"),
                created = x.CreateDate,
                username = x.Username,
                id = x.Id,
                name = x.GetValue<string>("customerFirstName") + " " + x.GetValue<string>("customerLastName")
            });

        }

        public IEnumerable<OrderCustomer> GetCustomers(string fromDate, string toDate, string name, string email, bool boughtOnce, bool boughtMore)
        {
            try {

                if (boughtMore)
                {
                    boughtOnce = false;
                }

                var customersMapped = new List<OrderCustomer>();

                string _fromDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.AddDays(-3600).ToString("yyyyMMdd") : fromDate;
                string _toDate = string.IsNullOrEmpty(toDate) ? DateTime.Now.AddDays(1).ToString("yyyyMMdd") : toDate;

                var allCustomeres = CustomOrderHelper.GetAllRawCustomers(name, email, _fromDate, _toDate, boughtOnce, boughtMore);

                foreach (var customer in allCustomeres)
                {
                    var customerMapping = CustomOrderHelper.MapCustomer(customer);

                    if (customer != null)
                    {
                        customersMapped.Add(customerMapping);
                    }
                }

                return customersMapped.OrderBy(x => x.email);

            } catch(Exception ex)
            {
                Log.Instance.LogError(ex,"GetCustomers");
                return null;
            }


        }

        public OrderCustomer GetCustomer(string customerEmail)
        {
            var customer = new OrderCustomer();

            var customerOrders = CustomOrderHelper.GetCustomerOrders(-10, customerEmail);

            customer.orders = customerOrders;

            return customer;
        }

        public IEnumerable<IBillingProvider> GetAllPaymentProviders()
        {
            return uWebshop.API.Providers.GetAllPaymentProviders();
        }
        public IEnumerable<IFulfillmentProvider> GetAllFulfillmentProviders()
        {
            return uWebshop.API.Providers.GetAllFulfillmentProviders();
        }

        public IEnumerable<BasketOrderInfoAdaptor> GetAllOrders(string status = "All")
		{
			var orders = Orders.GetAllOrders().Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist && x.Status != OrderStatus.Scheduled);

			if (string.IsNullOrEmpty(status) || status.ToLowerInvariant() == "all" || status.ToLowerInvariant() == "undefined")
			{
				return orders.Select(o => o as BasketOrderInfoAdaptor);
			}

			OrderStatus orderStatus;
			Enum.TryParse(status, out orderStatus);

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return orders.Where(x => x.Status == orderStatus).Select(o => o as BasketOrderInfoAdaptor);
			}
			return null;
		}
		
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDays(int days, string storeAlias = null)
		{
			var orders = Orders.GetOrders(days, storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist && x.Status == OrderStatus.Scheduled);
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersToDeliverToday(string storeAlias = null)
		{
			return OrderHelper.GetOrdersDeliveredBetweenTimes(DateTime.Today.Date, DateTime.Today.AddDays(1).AddSeconds(-1), storeAlias)
				.Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Scheduled)
				.Select(o => Orders.CreateBasketFromOrderInfo(o) as BasketOrderInfoAdaptor);
		}
		
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByStatus(string status, string storeAlias = null)
		{
			OrderStatus orderStatus;
			Enum.TryParse(status, true, out orderStatus);
			return Orders.GetOrders(orderStatus, storeAlias).Select(o => o as BasketOrderInfoAdaptor);
		}
		
		/// <summary>
		/// Get orders based on confirm DateTimeRange
		/// </summary>
		/// <param name="startDateTime">Format: 2015-05-26T16:03:35</param>
		/// <param name="endDateTime">Format: 2015-05-26T16:03:35</param>
		/// <param name="storeAlias"></param>
		/// <returns></returns>
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDateRange(string startDateTime, string endDateTime = null, string storeAlias = null)
		{
			DateTime startDate;
			DateTime.TryParse(startDateTime, out startDate);
			var endDate = DateTime.Now;
		    if (endDateTime != null)
		    {
		        DateTime.TryParse(endDateTime, out endDate);
                if (!endDateTime.Contains(':'))
                {
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                }
            }

		    var orders = Orders.GetOrdersConfirmedBetweenTimes(startDate, endDate, storeAlias);
		    
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDeliveryDateRange(string startDateTime, string endDateTime = null, string storeAlias = null)
		{
			DateTime startDate;
			DateTime.TryParse(startDateTime, out startDate);
			var endDate = DateTime.Now;
			if (endDateTime != null)
			{
				DateTime.TryParse(endDateTime, out endDate);
				// if endDateTime contains a time, there will be a :, if NOT then this check will use the whole day instead of only 00:00:00 (start of day)
				if (!endDateTime.Contains(':'))
				{
					endDate = endDate.Date.AddDays(1).AddSeconds(-1);
				}
			}

			var orders = Orders.GetOrdersDeliveredBetweenTimes(startDate, endDate, storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersToBeDelivered(int daysFromNow = 0, string storeAlias = null)
		{
			var dateToShow = DateTime.Now.Date.AddDays(daysFromNow);
			var orders = Orders.GetOrdersDeliveredBetweenTimes(dateToShow.Date, dateToShow.Date.AddDays(1).AddSeconds(-1), storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<string> GetEmailTemplates()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var files = new List<string>();
				// xslt directory
				if (Directory.Exists(IOHelper.MapPath(SystemDirectories.Xslt)))
				{
					files = Directory.GetFiles(IOHelper.MapPath(SystemDirectories.Xslt), "*.xslt", SearchOption.AllDirectories).Select(file => file.Replace(IOHelper.MapPath(SystemDirectories.Xslt) + @"\", string.Empty)).ToList();
				}
				// macroscripts directory
				if (Directory.Exists(IOHelper.MapPath(SystemDirectories.MacroScripts)))
				{
					files.AddRange(Directory.GetFiles(IOHelper.MapPath(SystemDirectories.MacroScripts), "*.cshtml", SearchOption.AllDirectories).Select(file => file.Replace(IOHelper.MapPath(SystemDirectories.MacroScripts) + @"\", string.Empty)));
				}
				// views directory
				if (Directory.Exists(IOHelper.MapPath(SystemDirectories.MvcViews)))
				{
					files.AddRange(
						Directory.GetFiles(IOHelper.MapPath(SystemDirectories.MvcViews), "*.cshtml", SearchOption.AllDirectories)
							.Select(file => file.Replace(IOHelper.MapPath(SystemDirectories.MvcViews) + @"\", string.Empty)));
				}

				return files.Where(x => x.ToLowerInvariant().Contains("email") || x.ToLowerInvariant().Contains("mail"));
			}
			return null;
		}

		public string GetRenderedTemplate(int nodeId, string template)
		{
			return RazorLibraryExtensions.RenderMacro(template, nodeId);
		}
		
		public int GetOrCreateOrderNode(string uniqueOrderId)
		{
			Guid orderGuid;
			Guid.TryParse(uniqueOrderId, out orderGuid);

            new DocumentTypeInstaller().RunT4Code();

            var order = OrderHelper.GetOrder(orderGuid);

            if (order != null && IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
                return IO.Container.Resolve<IUmbracoDocumentTypeInstaller>().GetOrCreateOrderContent(order);
			}
			return 0;
		}

		public IEnumerable<CustomerGroup> GetAllMemberGroups()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var membershipRoles = Roles.GetAllRoles();

				return membershipRoles.Select(m => new CustomerGroup { Alias = m });
			}
			return Enumerable.Empty<CustomerGroup>();
		}

		public BasketOrderInfoAdaptor GetOrder(string uniqueOrderId)
		{
            try
            {
                Guid guid;
                Guid.TryParse(uniqueOrderId, out guid);

                var order = Orders.GetOrder(guid) as BasketOrderInfoAdaptor;

                return order;

            } catch(Exception ex)
            {
                Log.Instance.LogError(ex, "Get Order Failed!");
            }

			return null;
		}

		public BasketOrderInfoAdaptor GetOrderByNumber(string orderNumber)
		{
			var order = Orders.GetAllOrders().FirstOrDefault(x => x.OrderReference.ToLowerInvariant() == orderNumber.ToLowerInvariant());
			return order as BasketOrderInfoAdaptor;
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByFirstName(string customerFirstName)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.FirstName.ToLowerInvariant() == customerFirstName.ToLowerInvariant());
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByLastName(string customerLastName)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.LastName.ToLowerInvariant() == customerLastName.ToLowerInvariant());
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByEmail(string customerEmail)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.Email.ToLowerInvariant() == customerEmail.ToLowerInvariant());
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByCustomerProperty(string property, string value)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.GetValue<string>(property) != null && x.Customer.GetValue<string>(property).ToLowerInvariant() == value.ToLowerInvariant());
			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public string GetCustomerValueFromOrder(string orderGuid, string property)
		{
			var order = Orders.GetOrder(orderGuid);

			var customerValue = order.Customer.GetValue<string>(property,true);
			if (!string.IsNullOrEmpty(customerValue))
			{
				return customerValue;
			}
			var shippingValue = order.Customer.Shipping.GetValue<string>(property, true);
			if (!string.IsNullOrEmpty(shippingValue))
			{
				return shippingValue;
			}
			var extraValue = order.OrderFields.GetValue<string>(property);

			if (!string.IsNullOrEmpty(extraValue))
			{
				return extraValue;
			}
			return string.Empty;
		}

		public class PostStockRequest
		{
			public int id { get; set; }
			public int stock { get; set; }
			public string storealias { get; set; }
		}

		public int PostStock(PostStockRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				string storeAlias = null;
				if (!string.IsNullOrEmpty(data.storealias) && data.storealias.ToLowerInvariant() != "all stores" && data.storealias.ToLowerInvariant() != "" && data.storealias.ToLowerInvariant() != "global")
				{
					storeAlias = data.storealias;
				}

				IO.Container.Resolve<IStockService>().ReplaceStock(data.id, data.stock, false, storeAlias);
				return data.stock;
			}

			return 0;
		}

		public IEnumerable<Coupon> GetCouponCodes(int discountId)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var couponCodes = IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(discountId);
				return couponCodes.Select(coupon =>
							new Coupon
							{
								CouponCode = coupon.CouponCode,
								DiscountId = discountId,
								NumberAvailable = coupon.NumberAvailable
							}).ToList();
			}
			return null;
		}

		public class PostCouponRequest
		{
			public IEnumerable<PostCoupon> Coupons { get; set; }
			public int DiscountId { get; set; }
            public Guid uniqueID { get; set; }
		}

		public class PostCoupon
		{
			public string couponCode { get; set; }
			public int numberAvailable { get; set; }
		}

		public void PostCouponCodes(PostCouponRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var couponList = data.Coupons.Select(item =>
							new Coupon
							{
								DiscountId = data.DiscountId,
								CouponCode = item.couponCode,
								NumberAvailable = item.numberAvailable,
                                uniqueID = data.uniqueID
							}).Cast<ICoupon>().ToList();

				IO.Container.Resolve<ICouponCodeService>().Save(data.DiscountId, couponList);

			}
		}

		public class CustomerGroup : ICustomerGroup
		{
			public string Alias { get; set; }
		}

		public class Coupon : ICoupon
		{
            public int Id { get; set; }
			public int DiscountId { get; set; }
			public string CouponCode { get; set; }
			public int NumberAvailable { get; set; }
            public Guid uniqueID { get; set; }
		}

		public class PostStatusRequest
		{
			public Guid id { get; set; }
			public string status { get; set; }
			public bool emails { get; set; }
		}

		public string PostOrderStatus(PostStatusRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				OrderStatus status;
				Enum.TryParse(data.status, out status);

				order.SetStatus(status, data.emails);
				order.Save();

				return data.status;
			}
			return null;
		}

		public string PostOrder(PostOrderRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				OrderStatus status;
				Enum.TryParse(data.status, out status);
				
				order.SetStatus(status, data.emails);

				order.Fulfilled = data.fulfilled;
				order.Paid = data.paid;
				
				order.Save();

				return data.status;
			}
			return null;
		}

		public class PostOrderRequest
		{
			public Guid id { get; set; }
			public bool paid { get; set; }
			public bool fulfilled { get; set; }
			public string status { get; set; }
			public bool emails { get; set; }
		}

		public class PostPaidRequest
		{
			public Guid id { get; set; }
			public bool paid { get; set; }
		}

		public bool PostPaid(PostPaidRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				order.Paid = data.paid;
				order.Save();

				return data.paid;
			}
			return false;
		}
        public class PostStatusAndPaidRequest
        {
            public Guid id { get; set; }
            public string status { get; set; }
            public bool emails { get; set; }

            public bool paid { get; set; }
        }

        public bool PostUpdateStatusAndPaid(PostStatusAndPaidRequest data) {

            if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
            {
                var order = OrderHelper.GetOrder(data.id);

                OrderStatus status;
                Enum.TryParse(data.status, out status);

                order.SetStatus(status, data.emails);
                order.Paid = data.paid;
                order.Save();

                return true;
            }

            return false;
        }

		public IEnumerable<string> GetDiscountTypes()
		{
			return Enum.GetNames(typeof(DiscountType));
		}

		public IEnumerable<string> GetDiscountOrderConditions()
		{
			return Enum.GetNames(typeof(DiscountOrderCondition));
		}

		public IEnumerable<string> GetPaymentProviderAmountTypes()
		{
			return Enum.GetNames(typeof(PaymentProviderAmountType));
		}

		public IEnumerable<string> GetPaymentProviderTypes()
		{
			return Enum.GetNames(typeof(PaymentProviderType));
		}

		public IEnumerable<string> GetShippingProviderTypes()
		{
			return Enum.GetNames(typeof(ShippingProviderType));
		}

		public IEnumerable<string> GetShippingRangeTypes()
		{
			return Enum.GetNames(typeof(ShippingRangeType));
		}
	}

	[PluginController("uWebshop")]
	[KnownType(typeof(BasketOrderInfoAdaptor))]
	public class PublicApiController : UmbracoApiController
	{
		public IEnumerable<CountryData> GetCountrySelector()
		{
			var store = API.Store.GetStore();

			if (store != null)
			{
				Thread.CurrentThread.CurrentCulture = store.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = store.CultureInfo;
			}

			return StoreHelper.GetAllCountries().Select(country => new CountryData(country.Name, country.Code));
		}

		public IEnumerable<Product> GetAllProducts()
		{
			var store = API.Store.GetStore();

			if (store != null)
			{
				Thread.CurrentThread.CurrentCulture = store.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = store.CultureInfo;
			}

			return DomainHelper.GetAllProducts().Cast<Product>();
		}

		public Country GetCountryFromCountryCode(string countryCode, string storeAlias, string currencyCode)
		{
			var localization = StoreHelper.GetLocalization(storeAlias, currencyCode);

			var countries = IO.Container.Resolve<ICountryRepository>().GetAllCountries(localization);

			if (countries != null)
			{
				return countries.FirstOrDefault(x => countryCode != null && x.Code.ToLowerInvariant() == countryCode.ToLowerInvariant());
			}
			return null;
		}

		public IEnumerable<Currency> GetCurrencyDataEditor()
		{
			var currencies = GetImportantCurrencyregions().Union(GetAllCurrencyRegions());
			return currencies.Select(c => new Currency { CurrencyEnglishName = c.CurrencyEnglishName, CurrencySymbol = c.CurrencySymbol, ISOCurrencySymbol = c.ISOCurrencySymbol });
		}

		private static IEnumerable<RegionInfo> GetAllCurrencyRegions()
		{
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(c => new RegionInfo(c.LCID)).Distinct(ri => ri.ISOCurrencySymbol).OrderBy(ri => ri.ISOCurrencySymbol);
		}

		private static IEnumerable<RegionInfo> GetImportantCurrencyregions()
		{
			var allRegions = GetAllCurrencyRegions().ToDictionary(c => c.ISOCurrencySymbol, c => c);
			return new[] { "EUR", "USD", "GBP", "DKK", "AUD", "NZD", "CHF", "CAD", "JPY" }.Select(c => allRegions[c]);
		}
		
		public IEnumerable<Language> GetLanguagePicker()
		{

			return umbraco.cms.businesslogic.language.Language.GetAllAsList().Select(language =>
			{
				var culture = new CultureInfo(language.CultureAlias);
				if (!culture.IsNeutralCulture)
				{
					var currencyRegion = new RegionInfo(culture.LCID);

					return new Language { CurrencyEnglishName = currencyRegion.CurrencyEnglishName, CurrencySymbol = currencyRegion.CurrencySymbol, ISOCurrencySymbol = currencyRegion.ISOCurrencySymbol, FriendlyName = language.FriendlyName, LanguageId = language.id.ToString(CultureInfo.InvariantCulture) };
				}
				return null;
			}).Where(c => c != null);
		}

		public IEnumerable<string> GetMemberGroups()
		{
			return Roles.GetAllRoles().ToList();
		}

		public int GetOrderCount(int id, string storeAlias = null)
		{
			return IO.Container.Resolve<IStockService>().GetOrderCount(id, storeAlias);
		}

		public int GetStock(int id, string storeAlias = null)
		{
			string storeAliasValue = null;
			if (!string.IsNullOrEmpty(storeAlias) && storeAlias.ToLowerInvariant() != "all stores" && storeAlias.ToLowerInvariant() != "" && storeAlias.ToLowerInvariant() != "global")
			{
				storeAliasValue = storeAlias;
			}
			return IO.Container.Resolve<IStockService>().GetStockForUwebshopEntityWithId(id, storeAliasValue);
		}

		public IEnumerable<TemplateRequest> GetTemplates(int id)
		{
			var content = Services.ContentService.GetById(id);
			return content.ContentType.AllowedTemplates.Select(template => new TemplateRequest {id = template.Id, name = template.Name, alias = template.Alias}).ToList();
		}

		public IEnumerable<string> GetOrderStatusses()
		{
			return Enum.GetNames(typeof(OrderStatus));
		}

		[System.Web.Http.AcceptVerbs("GET", "POST")]
		[System.Web.Http.HttpGet]
		public string GetCurrentUsername()
		{
            var currentUser = HttpContext.Current.User.Identity.Name;
			return !string.IsNullOrEmpty(currentUser) ? currentUser : string.Empty;
		}

		public string GetDictionaryValue(string key, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			Thread.CurrentThread.CurrentUICulture = localization.Store.CultureInfo;
			return Umbraco.GetDictionaryValue(key);
		}

        public class TemplateRequest
		{
			public int id { get; set; }
			public string alias { get; set; }
			public string name { get; set; }
		}
	}

	[Serializable]
	public class Language
	{
		public string LanguageId;
		public string FriendlyName;
		public string ISOCurrencySymbol;
		public string CurrencySymbol;
		public string CurrencyEnglishName;
	}

	[Serializable]
	public class CountryData
	{
		public string Name;
		public string Code;

		public CountryData(string name, string code)
		{
			Name = name;
			Code = code;
		}
	}

	[Serializable]
	public class Currency
	{
		public string ISOCurrencySymbol;
		public string CurrencySymbol;
		public string CurrencyEnglishName;
	}

	internal static class Extensions
	{
		public static IEnumerable<T> Distinct<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> predicate)
		{
			var distinctKeys = new HashSet<TCompare>();
			foreach (var item in items)
			{
				var key = predicate(item);
				if (distinctKeys.Contains(key)) continue;
				distinctKeys.Add(key);
				yield return item;
			}
		}
	}
}