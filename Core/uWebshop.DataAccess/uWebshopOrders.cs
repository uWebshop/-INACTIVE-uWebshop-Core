using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.DataAccess.Pocos;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using static Umbraco.Core.Persistence.Sql;

namespace uWebshop.DataAccess
{
	public class uWebshopOrders
	{
		public static string ConnectionString;
        
        internal static UmbracoDatabase Database
        {
            get { return UmbracoContext.Current.Application.DatabaseContext.Database; } 
        }
        
        public static IEnumerable<uWebshopOrderData> GetAllOrderInfos(Sql sqlToAppend = null)
		{
            if (sqlToAppend == null)
            {
                sqlToAppend = Builder
                    .Where("not orderStatus = @0", "Incomplete")
                    .Where("not orderStatus = @0", "Wishlist");
            }

            var sql = Builder.Select("*")
                .From("uWebshopOrders")
                .LeftOuterJoin("uWebshopOrderSeries")
                .On("seriesID= uWebshopOrderSeries.id");

            if (sqlToAppend != null)
            {
                sql.Append(sqlToAppend);
            }
            
            return
		        Database.Query<uWebshopOrderData>(sql);

        }

	    public static uWebshopOrderData GetOrderInfo(Guid orderId)
	    {
	        var sqlToAppend = Builder
	            .Where("uniqueID = @0", orderId);

            var orderData = GetAllOrderInfos(sqlToAppend);
          
	        if (orderData != null && orderData.FirstOrDefault() != null)
	        {
	            return orderData.FirstOrDefault();
	        }
            
	        LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
	            DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
	            " uWebshopOrders FAIL to get/read from Database with uniqueId: " + orderId);
	        return null;

	    }

	    public static uWebshopOrderData GetOrderInfo(string transactionId)
	    {
            var sqlToAppend = Builder
                .Where("transactionID = @0", transactionId);

            var orderData = GetAllOrderInfos(sqlToAppend);

            if (orderData != null && orderData.FirstOrDefault() != null)
            {
                return orderData.FirstOrDefault();
            }

            LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
                " uWebshopOrders FAIL to get/read from Database with transactionId: " + transactionId);
            return null;
        }

		public static uWebshopOrderData GetOrderInfo(int id)
		{
            var sqlToAppend = Builder
                .Where("id = @0", id);

            var orderData = GetAllOrderInfos(sqlToAppend);

            if (orderData != null && orderData.FirstOrDefault() != null)
            {
                return orderData.FirstOrDefault();
            }

            LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
                " uWebshopOrders FAIL to get/read from Database with  uWebshopOrders.id: " + id);
            return null;
        }

	    public static IEnumerable<uWebshopOrderData> GetOrdersFromCustomer(int customerId, bool includeIncomplete = false)
	    {
	        if (customerId == 0)
	        {
	            return new List<uWebshopOrderData>();
	        }

            var sqlToAppend = Builder
                .Where("customerID = @0", customerId);

	        if (!includeIncomplete)
	        {
                var notIncompleteSql = Builder
                    .Where("not orderStatus = @0", "Incomplete")
                    .Where("not orderStatus = @0", "Wishlist");
	            sqlToAppend.Append(notIncompleteSql);
	        }

            var orderData = GetAllOrderInfos(sqlToAppend);

	        return orderData;
	    }

	    public static IEnumerable<uWebshopOrderData> GetOrdersFromCustomer(string customerUsername, bool includeIncomplete = false)
		{
            if (string.IsNullOrEmpty(customerUsername))
            {
                return new List<uWebshopOrderData>();
            }

            var sqlToAppend = Builder
               .Where("customerUsername = @0", customerUsername);

            if (!includeIncomplete)
            {
                var notIncompleteSql = Builder
                    .Where("not orderStatus = @0", "Incomplete")
                    .Where("not orderStatus = @0", "Wishlist");
                sqlToAppend.Append(notIncompleteSql);
            }

            var orderData = GetAllOrderInfos(sqlToAppend);

            return orderData;
		}

		public static IEnumerable<uWebshopOrderData> GetWishlistsFromCustomer(int customerId)
		{
            if (customerId == 0)
            {
                return new List<uWebshopOrderData>();
            }

		    var sqlToAppend = Builder
		        .Where("customerID = @0", customerId)
		        .Where("orderStatus = @0", "Wishlist");

            var orderData = GetAllOrderInfos(sqlToAppend);

            return orderData;
		}

		public static IEnumerable<uWebshopOrderData> GetWishlistsFromCustomer(string customerUsername)
		{

            if (string.IsNullOrEmpty(customerUsername))
            {
                return new List<uWebshopOrderData>();
            }

            var sqlToAppend = Builder
              .Where("customerUsername = @0", customerUsername)
              .Where("orderStatus = @0", "Wishlist");

            var orderData = GetAllOrderInfos(sqlToAppend);

            return orderData;
		}

	    public static IEnumerable<uWebshopOrderData> GetOrdersDeliveredBetweenTimes(DateTime? startTime, DateTime? endTime)
	    {
	        if (startTime >= endTime)
	        {
	            return new List<uWebshopOrderData>();
	        }
            
	        var sql = Builder
	            .Where("not orderStatus = @0", "Incomplete")
	            .Where("not orderStatus = @0", "Wishlist")
	            .Where("deliveryDate >= @0", startTime)
	            .Where("deliveryDate <= @0", endTime);

            return GetAllOrderInfos(sql);
	    }

	    public static void StoreOrder(uWebshopOrderData orderData)
		{
            
			if (!string.IsNullOrWhiteSpace(orderData.SeriesCronInterval))
			{
                // todo create or update
            }
            
            orderData.UpdateDate = DateTime.Now;

	        if (GetOrderInfo(orderData.UniqueId) == null)
	        {
	            Database.Insert(orderData);
	        }
            else { 
	        Database.Update(orderData);
	        }
		}


        public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, OrderStatus orderStatus)
		{
			SetOrderInfo(orderId, serializedOrderInfoObject, orderStatus.ToString());
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, string orderStatus)
		{
            var order = GetOrderInfo(orderId);
            var dateTime = DateTime.Now;

		    if (order != null)
		    {
		        order.OrderInfo = serializedOrderInfoObject;
		        order.OrderStatus = orderStatus;
		        order.UpdateDate = dateTime;
		        Database.Update(order);
		    }
		    else
		    {
		        var newOrder = new uWebshopOrderData
		        {
		            OrderInfo = serializedOrderInfoObject,
		            OrderStatus = orderStatus,
		            CreateDate = dateTime,
		            UpdateDate = dateTime
		        };
		        Database.Insert(newOrder);
            }
		}


	    public static void ChangeOrderStatus(Guid orderId, OrderStatus orderStatus)
	    {
	        var order = GetOrderInfo(orderId);

	        order.OrderStatus = orderStatus.ToString();

	        Database.Update("uWebshopOrders", "id", order);
	    }

	    public static void SetTransactionId(Guid orderId, string transactionId)
	    {
	        var order = GetOrderInfo(orderId);

	        order.TransactionId = transactionId;

	        Database.Update(order);
	    }

	    /// <summary>
	    /// Set the Umbraco Member Id (when using umbraco members)
	    /// </summary>
	    /// <param name="orderId">The order unique identifier.</param>
	    /// <param name="customerId">The customer unique identifier.</param>
	    public static void SetCustomerId(Guid orderId, int customerId)
	    {
	        var order = GetOrderInfo(orderId);

	        order.CustomerId = customerId;

	        Database.Update(order);
	    }

	    /// <summary>
	    /// Set the .Net Membership Loginname
	    /// </summary>
	    /// <param name="orderId">The order unique identifier.</param>
	    /// <param name="userName">Name of the user.</param>
	    public static void SetCustomer(Guid orderId, string userName)
	    {
	        var order = GetOrderInfo(orderId);

            order.UpdateDate = DateTime.Now;
            order.CustomerUsername = userName;

	        Database.Update(order);
	    }

	    public static void UpdateCustomerId(int oldCustomerId, int newCustomerId)
	    {
            Database.Update<uWebshopOrderData>("SET customerID=@0 WHERE customerID=@1", newCustomerId, oldCustomerId);
        }

	    public static void UpdateCustomerUsername(string oldCustomerUsername, string newCustomerUsername)
	    {
	        Database.Update<uWebshopOrderData>("SET customerUsername=@0 WHERE customerUsername=@1", newCustomerUsername,
	            oldCustomerUsername);
	    }
        
		public static void SetCustomerInfo(Guid orderId, XElement element)
		{
            var order = GetOrderInfo(orderId);
		    order.UpdateDate = DateTime.Now;

            if (element.Name == "customerEmail")
            {
                order.CustomerEmail = element.Value;
			}
		    if (element.Name == "customerFirstName")
		    {
		        order.CustomerFirstName = element.Value;
		    }
		    if (element.Name == "customerLastName")
		    {
		        order.CustomerLastName = element.Value;
		    }
		    Database.Update(order);
		}
		public static void SetCustomerInfo(Guid orderId, string customerEmail, string customerFirstName, string customerLastName)
		{
            customerFirstName = customerFirstName ?? string.Empty;
            customerLastName = customerLastName ?? string.Empty;
            customerEmail = customerEmail ?? string.Empty;

            var order = GetOrderInfo(orderId);
            order.UpdateDate = DateTime.Now;

		    order.CustomerFirstName = customerFirstName;
		    order.CustomerLastName = customerLastName;
		    order.CustomerEmail = customerEmail;

            Database.Update(order);
        }

		public static string GetHighestOrderNumberForStore(string storeAlias, ref int referenceId)
		{
            var sql = Builder.Select("*")
                        .From("uWebshopOrders")
                        .Where("StoreAlias = @0", storeAlias)
                        .Where("orderNumber NOT like @0", "[INCOMPLETE]%")
                        .Where("orderNumber NOT like @0", "[SCHEDULED]%")
                        .OrderBy("id DESC");

            var orderResult = Database.FirstOrDefault<uWebshopOrderData>(sql);
		    return orderResult != null ? orderResult.OrderNumber : null;
		}

		public static string GetHighestOrderNumber(ref int referenceId)
		{
            var sql = Builder.Select("*")
                         .From("uWebshopOrders")
                         .Where("orderNumber NOT like @0", "[INCOMPLETE]%")
                         .Where("orderNumber NOT like @0", "[SCHEDULED]%")
                         .OrderBy("id DESC");

            var orderResult = Database.FirstOrDefault<uWebshopOrderData>(sql);
            return orderResult != null ? orderResult.OrderNumber : null;
        }

		public static void SetOrderNumber(Guid uniqueOrderId, string orderNumber, string storeAlias, int storeOrderReferenceID)
		{
            LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "SetOrderNumber orderNumber: " + orderNumber + " storeOrderReferenceID: " + storeOrderReferenceID);
            
            var order = GetOrderInfo(uniqueOrderId);
            order.UpdateDate = DateTime.Now;
		    order.OrderNumber = orderNumber;
		    order.StoreOrderReferenceId = storeOrderReferenceID;
		    order.StoreAlias = storeAlias;

		    Database.Update(order);
		}

	    public static int AssignNewOrderNumberToOrder(int databaseId, string storeAlias, int orderNumberStartNumber)
	    {
	        using (var referenceIdUpdate = Database.GetTransaction())
	        {
	            var referenceId = Database.ExecuteScalar<int>(@"declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders WHERE StoreAlias = @storeAlias ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end
SELECT @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id",
	                new
	                {
	                    id = databaseId,
	                    orderNumberStartNumber = orderNumberStartNumber,
	                    storeAlias = storeAlias,
	                    updateDate = DateTime.Now
	                });

	            referenceIdUpdate.Complete();

                return referenceId;
            }
            
	    }

	    public static int AssignNewOrderNumberToOrderSharedBasket(int databaseId, string storeAlias,
	        int orderNumberStartNumber)
	    {
	        // todo: PetaPoco
	        if (databaseId <= 0) throw new Exception("No valid database id");
            
	        using (var referenceIdUpdate = Database.GetTransaction())
	        {
	            var referenceId = Database.ExecuteScalar<int>(@"declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id

select @storeOrderReferenceID",
	                new
	                {
	                    id = databaseId,
	                    orderNumberStartNumber = orderNumberStartNumber,
	                    storeAlias = storeAlias,
	                    updateDate = DateTime.Now
	                });

	            referenceIdUpdate.Complete();

	            return referenceId;
	        }
	    }

        /// <summary>
        /// Used to determine new incomplete order number
        /// </summary>
        /// <returns></returns>
	    public static int DetermineLastOrderId()
		{ 
			var lastOrderNumber = 0;

			var reader = Database.Fetch<uWebshopOrderData>("SELECT TOP(1) orderNumber FROM uWebshopOrders where OrderStatus = 'Incomplete' ORDER BY orderNumber DESC");

		    var firstOrDefault = reader.FirstOrDefault();
	        if (firstOrDefault != null)
	        {
	            var incompleteOrderNumber = firstOrDefault.OrderNumber;

	            if (incompleteOrderNumber.StartsWith("[INCOMPLETE]-"))
	            {
                    incompleteOrderNumber = incompleteOrderNumber.Replace("[INCOMPLETE]-", string.Empty);
	            }

	            int.TryParse(incompleteOrderNumber, out lastOrderNumber);
	        }

	        return lastOrderNumber;
		}
        
		public static void Delete(IEnumerable<uWebshopOrderData> orders)
		{
			foreach (var order in orders)
		    {
		        Database.Delete<uWebshopOrderData>(order);
		    }
		}


        public static void Delete(IEnumerable<Guid> orderGuids)
        {
            // todo: optimize!
            foreach (var orderGuid in orderGuids)
            {
                Database.Delete(GetOrderInfo(orderGuid));
            }
        }

        public static void RemoveScheduledOrdersWithSeriesId(int seriesId)
	    {
	        if (seriesId <= 0) throw new ArgumentOutOfRangeException("seriesId");
            
            var sqlToAppend = Builder
              .Where("seriesID = @0", seriesId)
              .Where("orderStatus = @0", "Scheduled");

            var orders = GetAllOrderInfos(sqlToAppend);

	        foreach (var order in orders)
	        {
	            Database.Delete(order);
	        }
	    }
	}
}