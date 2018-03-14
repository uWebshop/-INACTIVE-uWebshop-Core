using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.DataAccess.Pocos;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using System.Reflection;
using log4net;
using Umbraco.Core;
//using static Umbraco.Core.Persistence.Sql;

namespace uWebshop.DataAccess
{
	public class uWebshopOrders
	{
        private static readonly ILog Log =
                LogManager.GetLogger(
                    MethodBase.GetCurrentMethod().DeclaringType
                );


        public static string ConnectionString;
	    private static Sql Builder = Umbraco.Core.Persistence.Sql.Builder;

        internal static UmbracoDatabase Database
        {
            get { return ApplicationContext.Current.DatabaseContext.Database; } 
        }
        
        public static List<uWebshopOrderData> GetAllOrderInfos(Sql sqlToAppend = null)
        {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = Builder.Select("*")
                .From("uWebshopOrders")
                .LeftOuterJoin("uWebshopOrderSeries")
                .On("seriesID= uWebshopOrderSeries.id");

                if (sqlToAppend == null)
                {
                    sql
                        .Where("not orderStatus = @0", OrderStatus.Incomplete.ToString())
                        .Where("not orderStatus = @0", OrderStatus.Wishlist.ToString());
                }
                else
                {
                    sql.Append(sqlToAppend);
                }

                var query = db.Fetch<uWebshopOrderData>(sql);

                return query;
            }

        }

	    public static uWebshopOrderData GetOrderInfo(Guid orderId)
	    {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>("SELECT * FROM uWebshopOrders WHERE uniqueID = '" + orderId + "'");

                if (orderData != null && orderData.Any())
                {

                    return orderData.FirstOrDefault();
                }

                LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
                    " uWebshopOrders FAIL to get/read from Database with uniqueId: " + orderId);
                return null;
            }

	    }

	    public static uWebshopOrderData GetOrderInfo(string transactionId)
	    {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>("SELECT * FROM uWebshopOrders WHERE transactionID = '" + transactionId + "'");

                if (orderData != null && orderData.Any())
                {
                    return orderData.FirstOrDefault();
                }

                LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
                    " uWebshopOrders FAIL to get/read from Database with transactionId: " + transactionId);
                return null;
            }

        }

		public static uWebshopOrderData GetOrderInfo(int id)
		{
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>("SELECT * FROM uWebshopOrders WHERE id = " + id);


                if (orderData != null && orderData.Any())
                {
                    return orderData.FirstOrDefault();
                }

                LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") +
                    " uWebshopOrders FAIL to get/read from Database with  uWebshopOrders.id: " + id);
                return null;
            }


        }

	    public static List<uWebshopOrderData> GetOrdersFromCustomer(int customerId, bool includeIncomplete = false)
	    {
            if (customerId == 0)
	        {
	            return new List<uWebshopOrderData>();
	        }

            string sql = "SELECT * " +
                          "FROM uWebshopOrders " +
                          "LEFT OUTER JOIN uWebshopOrderSeries " +
                          "ON seriesID = uWebshopOrderSeries.id " +
                          "WHERE customerID = @0 ";

            if (!includeIncomplete) {
                sql = sql + "AND (not orderStatus = @1) AND (not orderStatus = @2)";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>(sql, customerId, OrderStatus.Incomplete.ToString(), OrderStatus.Wishlist.ToString());

                return orderData;
            }

	    }

	    public static List<uWebshopOrderData> GetOrdersFromCustomer(string customerUsername, bool includeIncomplete = false)
		{
            if (string.IsNullOrEmpty(customerUsername))
            {
                return new List<uWebshopOrderData>();
            }


            string sql = "SELECT * " +
                          "FROM uWebshopOrders " +
                          "LEFT OUTER JOIN uWebshopOrderSeries " +
                          "ON seriesID = uWebshopOrderSeries.id " +
                          "WHERE customerUsername = @0 ";

            if (!includeIncomplete)
            {
                sql = sql + "AND (not orderStatus = @1) AND (not orderStatus = @2)";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>(sql, customerUsername, OrderStatus.Incomplete.ToString(), OrderStatus.Wishlist.ToString());

                return orderData;
            }

		}
        public static List<uWebshopOrderData> GetOrdersFromCustomerOrEmail(string customerUsername, bool includeIncomplete = false)
        {
            if (string.IsNullOrEmpty(customerUsername))
            {
                return new List<uWebshopOrderData>();
            }

            string sql = "SELECT * " +
                          "FROM uWebshopOrders " +
                          "LEFT OUTER JOIN uWebshopOrderSeries " +
                          "ON seriesID = uWebshopOrderSeries.id " +
                          "WHERE (customerUsername = @0 OR customerEmail = @0) ";

            if (!includeIncomplete)
            {
                sql = sql + "AND (not orderStatus = @1) AND (not orderStatus = @2)";
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>(sql, customerUsername, OrderStatus.Incomplete.ToString(), OrderStatus.Wishlist.ToString());

                return orderData;
            }

        }
        public static List<uWebshopOrderData> GetWishlistsFromCustomer(int customerId)
		{
            if (customerId == 0)
            {
                return new List<uWebshopOrderData>();
            }


			var sql = Builder.Select("*")
				.From("uWebshopOrders")
				.LeftOuterJoin("uWebshopOrderSeries")
				.On("seriesID= uWebshopOrderSeries.id")
				 .Where("customerID = @0", customerId)
				.Where("orderStatus = @0", OrderStatus.Wishlist.ToString());

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>(sql);

                return orderData;
            }

		}

		public static List<uWebshopOrderData> GetWishlistsFromCustomer(string customerUsername)
		{

            if (string.IsNullOrEmpty(customerUsername))
            {
                return new List<uWebshopOrderData>();
            }

			var sql = Builder.Select("*")
				.From("uWebshopOrders")
				.LeftOuterJoin("uWebshopOrderSeries")
				.On("seriesID= uWebshopOrderSeries.id")
				.Where("customerUsername = @0", customerUsername)
			  .Where("orderStatus = @0", OrderStatus.Wishlist.ToString());

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderData = db.Fetch<uWebshopOrderData>(sql);

                return orderData;
            }

		}

	    /// <summary>
	    /// Get orders with confirmdate between starttime and endtime
	    /// </summary>
	    /// <param name="startTime"></param>
	    /// <param name="endTime"></param>
	    /// <param name="storeAlias"></param>
	    /// <returns></returns>
	    public static IEnumerable<uWebshopOrderData> GetOrdersConfirmedBetweenTimes(DateTime? startTime, DateTime? endTime, string storeAlias = null)
        {
            if (endTime == null)
            {
                endTime = DateTime.Now;
            }

	        if (endTime == new DateTime())
	        {
	            endTime = DateTime.Now;
	        }
           
            if (startTime >= endTime)
            {
                return new List<uWebshopOrderData>();
            }
			
			var sql = Builder.Select("*")
				.From("uWebshopOrders")
				.LeftOuterJoin("uWebshopOrderSeries")
				.On("seriesID= uWebshopOrderSeries.id")
				.Where("not orderStatus = @0", OrderStatus.Incomplete.ToString())
                .Where("not orderStatus = @0", OrderStatus.Wishlist.ToString())
                .Where("not orderStatus = @0", OrderStatus.Scheduled.ToString())
                .Where("confirmDate >= @0", startTime)
                .Where("confirmDate <= @0", endTime);

			if (!string.IsNullOrEmpty(storeAlias))
	        {
				sql.Where("StoreAlias = @0", storeAlias);
	        }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                return db.Query<uWebshopOrderData>(sql);
            }
                
		}

	    /// <summary>
	    /// Get orders with deliverydate between starttime and endtime
	    /// </summary>
	    /// <param name="startTime"></param>
	    /// <param name="endTime"></param>
	    /// <param name="storeAlias"></param>
	    /// <returns></returns>
	    public static IEnumerable<uWebshopOrderData> GetOrdersDeliveredBetweenTimes(DateTime? startTime, DateTime? endTime, string storeAlias = null)
	    {
            if (endTime == null)
            {
                endTime = DateTime.Now;
            }

	        if (startTime == null || startTime >= endTime)
	        {
	            return new List<uWebshopOrderData>();
	        }

			var sql = Builder.Select("*")
				.From("uWebshopOrders")
				.LeftOuterJoin("uWebshopOrderSeries")
				.On("seriesID= uWebshopOrderSeries.id")
				.Where("not orderStatus = @0", OrderStatus.Incomplete.ToString())
	            .Where("not orderStatus = @0", OrderStatus.Wishlist.ToString())
	            .Where("deliveryDate >= @0", startTime)
	            .Where("deliveryDate <= @0", endTime);

			if (!string.IsNullOrEmpty(storeAlias))
            {
				sql.Where("StoreAlias = @0", storeAlias);
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                return db.Query<uWebshopOrderData>(sql);
            }
            
		}

	    public static void StoreOrder(uWebshopOrderData orderData)
		{
            // todo: combine into one sql call, but this is problematic with SQL-CE and maybe SQL-AZURE? 
            // http://stackoverflow.com/questions/6595105/bulk-insert-update-with-petapoco

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                orderData.UpdateDate = DateTime.Now;

                if (orderData.SeriesId > 0 && !string.IsNullOrEmpty(orderData.SeriesCronInterval))
                {
                    var orderSerie = new uWebshopOrderSeries
                    {
                        Id = orderData.SeriesId,
                        CronInterval = orderData.SeriesCronInterval,
                        Start = orderData.SeriesStart.GetValueOrDefault(),
                        End = orderData.SeriesEnd.GetValueOrDefault(),
                        EndAfterInstances = orderData.SeriesEndAfterInstances
                    };
                    db.Update(orderSerie);
                }
                else if (orderData.SeriesId == 0 && !string.IsNullOrEmpty(orderData.SeriesCronInterval))
                {
                    var orderSerie = new uWebshopOrderSeries
                    {
                        CronInterval = orderData.SeriesCronInterval,
                        Start = orderData.SeriesStart.GetValueOrDefault(),
                        End = orderData.SeriesEnd.GetValueOrDefault(),
                        EndAfterInstances = orderData.SeriesEndAfterInstances
                    };
                    db.Insert(orderSerie);
                }
                else if (orderData.SeriesId > 0)
                {
                    db.Delete<uWebshopOrderSeries>(orderData.SeriesId);
                }

                if (db.IsNew(orderData))
                {
                    db.Insert(orderData);
                }
                else
                {
                    db.Update(orderData);
                }
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
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                if (order != null)
                {
                    order.OrderInfo = serializedOrderInfoObject;
                    order.OrderStatus = orderStatus;
                    order.UpdateDate = dateTime;
                    db.Update(order);
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
                    db.Insert(newOrder);
                }
            }
                
		}


	    public static void ChangeOrderStatus(Guid orderId, OrderStatus orderStatus)
	    {
	        var order = GetOrderInfo(orderId);

	        order.OrderStatus = orderStatus.ToString();

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update("uWebshopOrders", "id", order);
            }
                
	    }

	    public static void SetTransactionId(Guid orderId, string transactionId)
	    {
	        var order = GetOrderInfo(orderId);

	        order.TransactionId = transactionId;

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update(order);
            }

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

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update(order);
            }
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

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update(order);
            }
        }

	    public static void UpdateCustomerId(int oldCustomerId, int newCustomerId)
	    {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update<uWebshopOrderData>("SET customerID=@0 WHERE customerID=@1", newCustomerId, oldCustomerId);
            }
            
        }

	    public static void UpdateCustomerUsername(string oldCustomerUsername, string newCustomerUsername)
	    {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update<uWebshopOrderData>("SET customerUsername=@0 WHERE customerUsername=@1", newCustomerUsername,
                    oldCustomerUsername);
            }

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

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update(order);
            }
                
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

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                db.Update(order);
            }
        }

		public static string GetHighestOrderNumberForStore(string storeAlias, ref int referenceId)
		{
            var sql = Builder.Select("*")
                        .From("uWebshopOrders")
                        .Where("StoreAlias = @0", storeAlias)
                        .Where("orderNumber NOT like @0", "[INCOMPLETE]%")
                        .Where("orderNumber NOT like @0", "[SCHEDULED]%")
                        .OrderBy("id DESC");

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var orderResult = db.FirstOrDefault<uWebshopOrderData>(sql);
                return orderResult != null ? orderResult.OrderNumber : null;
            }

		}

		public static string GetHighestOrderNumber(ref int referenceId)
		{
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = Builder.Select("*")
                         .From("uWebshopOrders")
                         .Where("orderNumber NOT like @0", "[INCOMPLETE]%")
                         .Where("orderNumber NOT like @0", "[SCHEDULED]%")
                         .OrderBy("id DESC");

                var orderResult = db.FirstOrDefault<uWebshopOrderData>(sql);
                return orderResult != null ? orderResult.OrderNumber : null;
            }

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
	            var referenceId = Database.ExecuteScalar<int>(@"DECLARE @@storeOrderReferenceID int
SET @@storeOrderReferenceID = COALESCE((SELECT TOP 1
  storeOrderReferenceID
FROM uWebshopOrders
WHERE StoreAlias = @storeAlias
ORDER BY storeOrderReferenceID DESC)
, 0) + 1
SET @@storeOrderReferenceID =
                            CASE
                              WHEN @orderNumberStartNumber > @@storeOrderReferenceID THEN @orderNumberStartNumber
                              ELSE @@storeOrderReferenceID
                            END
SELECT
  @@storeOrderReferenceID
UPDATE uWebshopOrders
SET storeOrderReferenceID = @@storeOrderReferenceID,
    storeAlias = @storeAlias,
    updateDate = @updateDate
WHERE id = @id",
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
	            var referenceId = Database.ExecuteScalar<int>(@"DECLARE @@storeOrderReferenceID int
SET @@storeOrderReferenceID = COALESCE((SELECT TOP 1
  storeOrderReferenceID
FROM uWebshopOrders
ORDER BY storeOrderReferenceID DESC)
, 0) + 1
SET @@storeOrderReferenceID =
                            CASE
                              WHEN @orderNumberStartNumber > @@storeOrderReferenceID THEN @orderNumberStartNumber
                              ELSE @@storeOrderReferenceID
                            END
UPDATE uWebshopOrders
SET storeOrderReferenceID = @@storeOrderReferenceID,
    storeAlias = @storeAlias,
    updateDate = @updateDate
WHERE id = @id
SELECT
  @@storeOrderReferenceID",
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

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var reader = db.Fetch<uWebshopOrderData>("SELECT TOP(1) orderNumber FROM uWebshopOrders where OrderStatus = 'Incomplete' ORDER BY orderNumber DESC");

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


		}
        
		public static void Delete(IEnumerable<uWebshopOrderData> orders)
		{
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                foreach (var order in orders)
                {
                    db.Delete<uWebshopOrderData>(order);
                }
            }

		}


        public static void Delete(IEnumerable<Guid> orderGuids)
        {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                // todo: optimize!
                foreach (var orderGuid in orderGuids)
                {
                    db.Delete(GetOrderInfo(orderGuid));
                }
            }

        }

        public static void RemoveScheduledOrdersWithSeriesId(int seriesId)
	    {
	        if (seriesId <= 0) throw new ArgumentOutOfRangeException("seriesId");

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var sql = Builder
              .Where("seriesID = @0", seriesId)
              .Where("orderStatus = @0", OrderStatus.Scheduled.ToString());

                var orders = db.Query<uWebshopOrderData>(sql); ;

                foreach (var order in orders)
                {
                    db.Delete(order);
                }
            }

	    }
	}
}
