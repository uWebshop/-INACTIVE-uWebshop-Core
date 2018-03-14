using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Umbraco.Core;

namespace uWebshop.Umbraco
{
    public static class CustomOrderHelper
    {
        private static readonly ILog Log =
                LogManager.GetLogger(
                    MethodBase.GetCurrentMethod().DeclaringType
                );

        public static OrderDataInfo GetOrders(bool includeOrder, bool includeProducts)
        {

            var model = new OrderDataInfo();

            var list = new List<OrderData>();

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {

                var orders = db.Fetch<dynamic>("SELECT * FROM uWebshopOrders"
                        + " WHERE orderStatus = 'Dispatched'"
                        + " OR orderStatus = 'ReadyForDispatch' "
                        + " OR orderStatus = 'OfflinePayment'"
                        + " OR orderStatus = 'Closed'"
                        + " ORDER BY id desc");

                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }

            }

            model.list = list;

            return model;
        }
        public static OrderDataInfo GetOrdersByStatusAndStore(bool includeOrder, bool includeProducts, string orderStatus, string storeAlias, string start, string end)
        {
            var model = new OrderDataInfo();
            var list = new List<OrderData>();

            string storeAliasQuery = "";


            if (!string.IsNullOrEmpty(storeAlias))
            {
                storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
            }
            else {
                storeAlias = "";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
               
                string sql = "SELECT * FROM uWebshopOrders"
                        + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND LOWER(orderStatus) = '" + orderStatus.ToLower() + "'"
                        + storeAliasQuery
                        + " ORDER BY id desc";

                var orders = db.Fetch<dynamic>(sql, start, end);


                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            model.list = list;

            return model;
        }


        public static OrderDataInfo GetOrdersByStore(bool includeOrder, bool includeProducts, string storeAlias, string start, string end)
        {
            var model = new OrderDataInfo();
            var list = new List<OrderData>();

            string storeAliasQuery = "";

            if (!string.IsNullOrEmpty(storeAlias))
            {
                storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
            }
            else {
                storeAlias = "";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                string sql = "SELECT * FROM uWebshopOrders WHERE LOWER(orderStatus) != 'incomplete' AND LOWER(orderStatus) != 'wishlist'"
                        + " AND updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112)"
                        + storeAliasQuery
                        + " ORDER BY id desc";

                var orders = db.Fetch<dynamic>(sql, start, end);

                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            model.list = list;

            return model;

        }

        public static OrderDataInfo GetAbandondOrdersByStore(bool includeOrder, bool includeProducts, string storeAlias, string start, string end)
        {

            var model = new OrderDataInfo();
            var list = new List<OrderData>();

            string storeAliasQuery = "";

            if (!string.IsNullOrEmpty(storeAlias))
            {
                storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = "SELECT * FROM uWebshopOrders"
                        + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND orderStatus = 'Incomplete' AND customerEmail IS NOT NULL AND customerEmail != '' "
                        + storeAliasQuery
                        + " ORDER BY id desc";

                var orders = db.Fetch<dynamic>(sql, start, end);

                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            model.list = list;

            return model;
        }
        public static OrderDataInfo GetCompletedOrders(bool includeOrder, bool includeProducts, string storeAlias, string start, string end)
        {
            try
            {
                var model = new OrderDataInfo();
                var list = new List<OrderData>();

                string storeAliasQuery = "";

                if (!string.IsNullOrEmpty(storeAlias))
                {
                    storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
                }

                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {
                    var sql = "SELECT * FROM uWebshopOrders"
                            + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND (orderStatus = 'ReadyForDispatch' OR orderStatus = 'Dispatched' OR orderStatus = 'OfflinePayment' OR orderStatus = 'Closed')"
                            + storeAliasQuery
                            + " ORDER BY id desc";

                    var orders = db.Fetch<dynamic>(sql, start, end);

                    foreach (var order in orders)
                    {
                        var item = Mapper(order, includeOrder, includeProducts);

                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }

                model.list = list;

                return model;
            } catch (Exception ex)
            {
                Log.Error("GetCompletedOrders Error!", ex);
                return null;
            }

        }

        public static decimal GetCompletedSales(string storeAlias, DateTime start, DateTime end)
        {
            try
            {
                string storeAliasQuery = "";

                if (!string.IsNullOrEmpty(storeAlias))
                {
                    storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
                }

                decimal total = 0;

                uWebshop.Domain.Interfaces.IOrder firstOrder = null; 

                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {
                    var sql = "SELECT * FROM uWebshopOrders"
                            + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND (orderStatus = 'ReadyForDispatch' OR orderStatus = 'Dispatched' OR orderStatus = 'OfflinePayment' OR orderStatus = 'Closed')"
                            + storeAliasQuery
                            + " ORDER BY id desc";

                    var orders = db.Fetch<dynamic>(sql, start, end);

                    foreach (var order in orders)
                    {
                        var orderId = Convert.ToString(order.uniqueID);

                        var o = uWebshop.API.Orders.GetOrder(new Guid((orderId)));

                        if (firstOrder == null)
                        {
                            firstOrder = o;
                        }

                        total += o.ChargedOrderAmount.Value;
                    }
                }


                uWebshop.API.Store.SetStore(firstOrder.Store.Alias);

                var store = uWebshop.API.Store.GetStore();

                if (store != null)
                {
                    Thread.CurrentThread.CurrentCulture = store.CultureInfo;
                    Thread.CurrentThread.CurrentUICulture = store.CultureInfo;
                }

                string currency = uWebshop.API.Store.GetCurrentLocalization().CurrencyCode;

                //return string.Format(GetCultureByCurrencyCode(currency), "{0:C}", total);
                return total;
            }
            catch (Exception ex)
            {
                Log.Error("GetCompletedSales Error!", ex);
                return 0;
            }

        }


        public static OrderDataInfo GetIncompleteOrdersByStore(bool includeOrder, bool includeProducts, string storeAlias, string start, string end)
        {
            var model = new OrderDataInfo();
            var list = new List<OrderData>();

            string storeAliasQuery = "";

            if (!string.IsNullOrEmpty(storeAlias))
            {
                storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
            }
            else
            {
                storeAlias = "";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = "SELECT * FROM uWebshopOrders"
                       + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND orderStatus = 'Incomplete'"
                        + storeAliasQuery
                        + " ORDER BY id desc";

                var orders = db.Fetch<dynamic>(sql, start, end);


                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            model.list = list;

            return model;
        }
        public static OrderDataInfo GetRefundedOrders(bool includeOrder, bool includeProducts, string storeAlias, string from, string start)
        {
            var model = new OrderDataInfo();
            var list = new List<OrderData>();

            string storeAliasQuery = "";

            if (!string.IsNullOrEmpty(storeAlias))
            {
                storeAliasQuery = " AND LOWER(storeAlias) = '" + storeAlias.ToLower() + "'";
            }
            else
            {
                storeAlias = "";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = "SELECT" 
                        + " a.id,[uniqueID],[customerEmail],[customerFirstName],[customerLastName],[orderNumber],[storeOrderReferenceID],[orderInfo],[orderStatus],[transactionID],[storeAlias],[customerID],[customerUsername],[createDate],[updateDate],[deliveryDate],[seriesID],[confirmDate], b.refund as refund"
                        + " FROM uWebshopOrders a"
                        + " LEFT JOIN customRefund b on a.uniqueID = b.orderId"
                        + " WHERE updateDate >= Convert(varchar(30),@0,112) AND updateDate <= Convert(varchar(30),@1,112) AND b.refund IS NOT NULL "
                        + storeAliasQuery
                        + " ORDER BY id desc";

                var orders = db.Fetch<dynamic>(sql, from, start);

                foreach (var order in orders)
                {
                    var item = Mapper(order, includeOrder, includeProducts);

                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }

            model.list = list;

            return model;
        }

        public static List<OrderData> GetCustomerOrders(int customerId, string customerEmail) {

            try
            {

                var list = new List<OrderData>();

                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {
                    var sql = "SELECT"
                            + " id,[uniqueID],[customerEmail],[customerFirstName],[customerLastName],[orderNumber],[storeOrderReferenceID],[orderInfo],[orderStatus],[transactionID],[storeAlias],[customerID],[customerUsername],[createDate],[updateDate],[deliveryDate],[seriesID],[confirmDate]"
                            + " FROM uWebshopOrders"
                            + " WHERE (customerID = @0 OR customerEmail = @1) AND (orderStatus = 'ReadyForDispatch' OR orderStatus = 'Dispatched' OR orderStatus = 'OfflinePayment')"
                            + " ORDER BY id desc";

                    var orders = db.Fetch<dynamic>(sql, customerId, customerEmail);

                    foreach (var order in orders)
                    {
                        var item = Mapper(order, false, false);

                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex) {
                Log.Error("GetCustomerOrders Error!",ex);
                return null;
            }

        }

        public static IEnumerable<OrderRawCustomer> GetAllRawCustomers(string name, string email, string fromDate, string toDate, bool boughtOnce, bool boughtMore) {

            try
            {
                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {

                    string nameQuery = "";
                    string emailQuery = "";
                    string onceQuery = "";
                    string moreQuery = "";
                    string top = " TOP 150 ";

                    if (!string.IsNullOrEmpty(name))
                    {
                        top = "";
                        nameQuery = " AND (customerFirstName LIKE @0 OR customerLastName LIKE @0)";
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        top = "";
                        emailQuery = " AND customerEmail LIKE @1";
                    }

                    if (boughtOnce)
                    {
                        top = "";
                        onceQuery = " AND COUNT(customerEmail) = 1";
                    }

                    if (boughtMore)
                    {
                        top = "";
                        moreQuery = " AND COUNT(customerEmail) > 1";
                    }
                    string dateQuery = " Having min(updateDate) >= Convert(varchar(30),@2,112) AND min(updateDate) <= Convert(varchar(30),@3,112)";

                    string sql = "SELECT " + top + " COUNT(customerEmail) AS orderCount, customerEmail, min(updateDate) as firstOrderDate, min(customerFirstName) as customerFirstName, min(customerLastName) as customerLastName, max(cast(orderInfo as varchar(max))) as orderInfo" +
                        " FROM uWebshopOrders" +
                        " WHERE (customerEmail IS NOT NULL AND  customerEmail != '') AND (orderStatus = 'ReadyForDispatch' OR orderStatus = 'Dispatched' OR orderStatus = 'OfflinePayment') " +
                        nameQuery +
                        emailQuery +
                        " GROUP BY customerEmail" +
                        dateQuery +
                        onceQuery +
                        moreQuery;

                    var customers = db.Query<OrderRawCustomer>(sql, "%" + name + "%", "%" + email + "%", fromDate, toDate);

                    return customers;
                }
            }
            catch (Exception ex) {
                Log.Error("GetAllRawCustomers Error!", ex);
                return null;
            }

        }

        public static OrderData GetOrder(Guid orderId)
        {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var order = db.FirstOrDefault<dynamic>("SELECT * FROM uWebshopOrders"
                        + " WHERE uniqueID = @0"
                        + " ORDER BY id desc", orderId);

                var item = Mapper(order, true, true);

                return item;
            }

        }

        public static OrderCustomer MapCustomer(OrderRawCustomer customer) {
            var o = new OrderCustomer();

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(customer.orderInfo);

            XmlNode orderNode = xml.SelectSingleNode("Order");

            o.email = customer.customerEmail;
            o.firstName = customer.customerFirstName;
            o.lastName = customer.customerLastName;
            o.created = customer.firstOrderDate;
            o.address1 = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerAddress1"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerAddress1"].InnerText : "";
            o.socialId = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerSocialId"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerSocialId"].InnerText : "";
            o.zipCode = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerZipCode"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerZipCode"].InnerText : "";
            o.city = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCity"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCity"].InnerText : "";
            o.email = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerEmail"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerEmail"].InnerText : "";
            o.phone = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerPhone"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerPhone"].InnerText : "";
            o.country = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCountry"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCountry"].InnerText : "";
            o.orderCount = customer.orderCount;

            return o;
        }

        public static OrderData Mapper(dynamic item, bool includeOrder, bool includeProducts)
        {

            var o = new OrderData();

            if (item != null)
            {

                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(item.orderInfo);

                    XmlNode orderNode = xml.SelectSingleNode("Order");

                    string _chargedAmount = orderNode["ChargedAmount"] != null ? orderNode["ChargedAmount"].InnerText : "0";

                    decimal chargedAmount = 0;

                    try
                    {
                        chargedAmount = Convert.ToDecimal(_chargedAmount.Remove(_chargedAmount.Length - 2, 2));
                    }
                    catch
                    {
                    }

                    string currency = orderNode["CurrencyCode"] != null ? orderNode["CurrencyCode"].InnerText : "ISK";

                    XmlNodeList orderLines = xml.SelectNodes("Order/OrderLines/OrderLine");

                    int qty = 0;
                    int lines = 0;

                    try
                    {
                        if (orderLines != null)
                        {

                            foreach (XmlNode node in orderLines)
                            {
                                lines++;
                                int _qty = Convert.ToInt32(node["Quantity"].InnerText);

                                qty = qty + _qty;

                            }

                        }

                    }
                    catch
                    {

                    }

                    DateTime confirmDate = item.createDate;

                    string _confirmDate = orderNode["ConfirmDate"] != null ? orderNode["ConfirmDate"].InnerText : "";

                    try
                    {
                        confirmDate = Convert.ToDateTime(_confirmDate);
                    }
                    catch { }

                    decimal chargedShippingAmount = 0;
                    string _chargedShippingAmount = orderNode["ShippingProviderPrice"] != null ? orderNode["ShippingProviderPrice"].InnerText : "0";
                    try
                    {
                        chargedShippingAmount = Convert.ToDecimal(_chargedShippingAmount.Remove(_chargedShippingAmount.Length - 2, 2));
                    }
                    catch { }

                    bool vatCharged = false;

                    string _vatCharged = orderNode["VATCharged"] != null ? orderNode["VATCharged"].InnerText : "true";

                    if (_vatCharged.ToLower() == "true")
                    {
                        vatCharged = true;
                    }

                    string culture = orderNode.SelectSingleNode("StoreInfo/Culture") != null ? orderNode.SelectSingleNode("StoreInfo/Culture").InnerText : "is-IS";


                    Domain.Interfaces.IOrder order = null;

                    if (includeOrder)
                    {
                        order = uWebshop.API.Orders.GetOrder(item.uniqueID);

                        chargedAmount = order.ChargedOrderAmount.Value;
                    }

                    string orderNodeId = orderNode["CorrespondingOrderDocumentId"] != null ? orderNode["CorrespondingOrderDocumentId"].InnerText : "0";

                    var customer = new OrderCustomer();

                    customer.isShipping = false;

                    if (orderNode["CustomerInfo"]["CustomerInformation"] != null)
                    {

                        customer.firstName = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerFirstName"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerFirstName"].InnerText : "";
                        customer.lastName = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerLastName"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerLastName"].InnerText : "";
                        customer.address1 = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerAddress1"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerAddress1"].InnerText : "";
                        customer.socialId = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerSocialId"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerSocialId"].InnerText : "";
                        customer.zipCode = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerZipCode"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerZipCode"].InnerText : "";
                        customer.city = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCity"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCity"].InnerText : "";
                        customer.email = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerEmail"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerEmail"].InnerText : "";
                        customer.phone = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerPhone"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerPhone"].InnerText : "";
                        customer.country = orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCountry"] != null ? orderNode["CustomerInfo"]["CustomerInformation"]["Customer"]["customerCountry"].InnerText : "";

                        var isShippingNode = orderNode["CustomerInfo"]["CustomerInformation"]["CustomerIsShipping"];

                        if (isShippingNode != null)
                        {
                            string isShipping = isShippingNode.InnerText;

                            if (!string.IsNullOrEmpty(isShipping) && isShipping == "true")
                            {
                                customer.isShipping = true;
                            }
                        }

                    }

                    var shipping = new OrderCustomer();

                    if (orderNode["CustomerInfo"]["ShippingInformation"] != null)
                    {
                        shipping.firstName = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingFirstName"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingFirstName"].InnerText : "";
                        shipping.address1 = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingAddress1"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingAddress1"].InnerText : "";
                        shipping.zipCode = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingZipCode"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingZipCode"].InnerText : "";
                        shipping.city = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingCity"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingCity"].InnerText : "";
                        shipping.phone = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingPhone"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingPhone"].InnerText : "";
                        shipping.country = orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingCountry"] != null ? orderNode["CustomerInfo"]["ShippingInformation"]["Shipping"]["shippingCountry"].InnerText : "";
                    }


                    var products = new List<OrderOrderLineProduct>();

                    if (includeProducts)
                    {

                        XmlNodeList orderlinesProducts = xml.SelectNodes("Order/OrderLines/OrderLine");

                        foreach (XmlNode node in orderlinesProducts)
                        {
                            var productLine = new OrderOrderLineProduct();
                            var variantLines = new List<OrderOrderLineVariant>();

                            int productId = Convert.ToInt32(node["OriginalProductId"].InnerText);
                            int quantity = Convert.ToInt32(node["Quantity"].InnerText);
                            string price = node["OriginalPrice"].InnerText;
                            string title = node["Title"].InnerText;
                            string sku = node["SKU"].InnerText;

                            productLine.productId = productId;
                            productLine.quantity = quantity;
                            productLine.sku = sku;
                            productLine.title = title;
                            productLine.price = price;

                            XmlNodeList variantsXml = node.SelectNodes("ProductVariants/ProductVariantInfo");

                            foreach (XmlNode var in variantsXml)
                            {
                                var variantLine = new OrderOrderLineVariant();

                                int variantId = Convert.ToInt32(var["Id"].InnerText);
                                string variantPrice = var["PriceWithVat"].InnerText;
                                string variantTitle = var["Title"].InnerText;
                                string variantSku = var["SKU"].InnerText;

                                variantLine.variantId = variantId;
                                variantLine.sku = variantSku;
                                variantLine.title = variantTitle;
                                variantLine.price = variantPrice;
                                variantLines.Add(variantLine);
                            }

                            productLine.variants = variantLines;

                            products.Add(productLine);

                        }

                    }

                    var paymentInfo = new OrderPaymentInfo();
                    if (orderNode["PaymentInfo"] != null && orderNode["PaymentInfo"]["Title"] != null && !string.IsNullOrEmpty(orderNode["PaymentInfo"]["Title"].InnerText))
                    {
                        paymentInfo.title = orderNode["PaymentInfo"]["Title"].InnerText;
                        paymentInfo.id = Convert.ToInt32(orderNode["PaymentInfo"]["Id"].InnerText);
                    }

                    var shippingInfo = new OrderPaymentInfo();

                    if (orderNode["ShippingInfo"] != null && orderNode["ShippingInfo"]["Title"] != null && !string.IsNullOrEmpty(orderNode["ShippingInfo"]["Title"].InnerText))
                    {
                        shippingInfo.title = orderNode["ShippingInfo"]["Title"].InnerText;
                        shippingInfo.id = Convert.ToInt32(orderNode["ShippingInfo"]["Id"].InnerText);
                    }

                    var discountInfo = new OrderDiscountInfo();

                    if (orderNode["Discounts"] != null && orderNode["Discounts"]["OrderDiscount"] != null && !string.IsNullOrEmpty(orderNode["Discounts"]["OrderDiscount"]["Id"].InnerText))
                    {
                        
                        if (orderNode["Discounts"]["OrderDiscount"]["CouponCode"] != null && !string.IsNullOrEmpty(orderNode["Discounts"]["OrderDiscount"]["CouponCode"].InnerText))
                        {
                            discountInfo.CouponCode = orderNode["Discounts"]["OrderDiscount"]["CouponCode"].InnerText;
                        }

                        discountInfo.id = Convert.ToInt32(orderNode["Discounts"]["OrderDiscount"]["Id"].InnerText);
                    }

                    o.confirmDate = confirmDate;
                    o.paymentInfo = paymentInfo;
                    o.shippingInfo = shippingInfo;
                    o.products = products;
                    o.createDate = item.createDate;
                    o.customerId = item.customerID;
                    o.email = item.customerEmail;
                    o.firstName = item.customerFirstName;
                    o.lastName = item.customerLastName;
                    o.orderNumber = item.orderNumber;
                    o.orderStatus = item.orderStatus;
                    o.refrenceId = item.storeOrderReferenceID ?? 0;
                    o.transactionId = item.transactionID;
                    o.storeAlias = item.storeAlias;
                    o.uniqueId = item.uniqueID;
                    o.updateDate = item.updateDate;
                    o.chargedAmount = chargedAmount;
                    o.chargedAmountCurrencyFormat = string.Format(GetCultureByCurrencyCode(currency), "{0:C}", chargedAmount);
                    o.currency = currency;
                    o.quantity = qty;
                    o.orderlines = lines;
                    o.vatCharged = vatCharged;
                    o.culture = culture;
                    o.discountInfo = discountInfo;
                    if (includeOrder)
                    {
                        //o.order = order;
                    }
                    o.orderNodeId = orderNodeId;
                    o.customer = customer;
                    o.shipping = shipping;
                    o.chargedShippingAmount = chargedShippingAmount;

                    return o;
                }
                catch (Exception ex)
                {

                    var lineNumber = new System.Diagnostics.StackTrace(ex, true).GetFrame(0).GetFileLineNumber();

                    Log.Error("Map Product Error, orderId: " + item.id + " Line:" + lineNumber, ex);
                }
            }

            return null;
           

        }

        public static XmlDocument GetOrderxml(Guid uniqueID)
        {

            return uWebshop.Domain.Helpers.OrderHelper.GetOrderXML(uniqueID);

        }

        public static void DeleteIncompleteBaskets()
        {

            Log.Info("Deleting all incomplete orders older then today...");

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Execute("DELETE FROM uWebshopOrders WHERE customerEmail IS NULL AND orderStatus = 'Incomplete' AND createDate < CONVERT(varchar, GETDATE(), 112)");
            }


        }

        public static System.Globalization.CultureInfo GetCultureByCurrencyCode(string code)
        {
           

            var culture = "is-IS";

            try
            {
                code = code.ToLower();

                if (code == "gbp")
                {
                    culture = "en-GB";
                }

                if (code == "eur")
                {
                    culture = "nl-NL";
                }

                if (code == "usd")
                {
                    culture = "en-US";
                }

            } catch(Exception ex)
            {
                Log.Error("GetCultureByCurrencyCode Error!",ex);
            }

            return new System.Globalization.CultureInfo(culture);
        }

        public static string GetCurrencyCodeByStore(string store) {

            string code = "ISK";

            try
            {
                store = store.ToLower();

                if (store == "eu")
                {
                    code = "EUR";
                }

                if (store == "de")
                {
                    code = "EUR";
                }

                if (store == "us")
                {
                    code = "USD";
                }

            }
            catch (Exception ex)
            {
                Log.Error("GetCurrencyCodeByStore Error!",ex);
            }
            
            return code;

        }
    }
}
