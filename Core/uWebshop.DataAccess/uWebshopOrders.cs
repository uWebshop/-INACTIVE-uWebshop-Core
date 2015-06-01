using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using uWebshop.Common;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.DataLayer;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace uWebshop.DataAccess
{
	public class uWebshopOrders
	{
		public static string ConnectionString = ApplicationContext.Current.DatabaseContext.ConnectionString;

		public static List<OrderData> GetAllOrderInfos(string where = null)
		{
			if (where == null) where = string.Empty;

			var orderInfos = new List<OrderData>();
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< SELECT orderInfo");
			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders " + where))
			{
				while (reader.Read())
				{
					orderInfos.Add(new OrderData(reader));
				}
				return orderInfos;
			}
		}

		public static OrderData GetOrderInfo(Guid orderId)
		{

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			
			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders WHERE uniqueID = @uniqueId", sqlHelper.CreateParameter("@uniqueId", orderId)))
			{
				while (reader.Read())
				{
					
					var orderInfo = reader.GetString("orderInfo");
	
					if (!string.IsNullOrEmpty(orderInfo))
					{
						
						return new OrderData(reader);
					}
				}

				LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " uWebshopOrders FAIL to get/read from Database with uniqueId: " + orderId);
				return null;
			}
		}

		public static OrderData GetOrderInfo(string transactionId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders WHERE transactionID = @transactionID", sqlHelper.CreateParameter("@transactionID", transactionId)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						return new OrderData(reader);
					}
				}
				return null;
			}
		}

		public static List<OrderData> GetOrdersFromCustomer(int customerId, bool includeIncomplete = false)
		{
			var orderInfos = new List<OrderData>();
			if (customerId == 0)
			{
				return orderInfos;
			}

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders WHERE customerID = @customerID"
					+ (includeIncomplete ? "" : " and not orderStatus = 'Incomplete' and not orderStatus = 'Wishlist'")
				, sqlHelper.CreateParameter("@customerID", customerId)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						orderInfos.Add(new OrderData(reader));
					}
				}
			}

			return orderInfos;
		}

		public static List<OrderData> GetOrdersFromCustomer(string customerUsername, bool includeIncomplete = false)
		{
			var orderInfos = new List<OrderData>();

			if (string.IsNullOrEmpty(customerUsername))
			{
				return orderInfos;
			}

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders WHERE customerUsername = @customerUsername"
					+ (includeIncomplete ? "" : " and not orderStatus = 'Incomplete' and not orderStatus = 'Wishlist'"),
				sqlHelper.CreateParameter("@customerUsername", customerUsername)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						orderInfos.Add(new OrderData(reader));
					}
				}
			}
			return orderInfos;
		}

		public static List<OrderData> GetWishlistsFromCustomer(int customerId)
		{
			var orderInfos = new List<OrderData>();
			if (customerId == 0)
			{
				return orderInfos;
			}

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders WHERE customerID = @customerID and orderStatus = 'Wishlist'",
				sqlHelper.CreateParameter("@customerID", customerId)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						orderInfos.Add(new OrderData(reader));
					}
				}
			}

			return orderInfos;
		}

		public static List<OrderData> GetWishlistsFromCustomer(string customerUsername)
		{
			var orderInfos = new List<OrderData>();

			if (string.IsNullOrEmpty(customerUsername))
			{
				return orderInfos;
			}

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders WHERE customerUsername = @customerUsername and orderStatus = 'Wishlist'",
				sqlHelper.CreateParameter("@customerUsername", customerUsername)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						orderInfos.Add(new OrderData(reader));
					}
				}
			}
			return orderInfos;
		}

		public static List<OrderData> GetOrdersDeliveredBetweenTimes(DateTime startTime, DateTime endTime)
		{
			var orderInfos = new List<OrderData>();

			if (startTime >= endTime)
			{
				return orderInfos;
			}

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id " +
				" WHERE deliveryDate >= @startTime and deliveryDate < @endTime",
				sqlHelper.CreateParameter("@startTime", startTime),
				sqlHelper.CreateParameter("@endTime", endTime)))
			{
				while (reader.Read())
				{
					var orderInfo = reader.GetString("orderInfo");
					if (!string.IsNullOrEmpty(orderInfo))
					{
						orderInfos.Add(new OrderData(reader));
					}
				}
			}
			return orderInfos;
		}

		public static void StoreOrder(OrderData orderData)
		{
			if (string.IsNullOrWhiteSpace(orderData.OrderXML))
				throw new Exception("Saving order without XML");
			// id, storeOrderReferenceID and orderNumber are either generated or delicate to manage
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);
			if (sqlHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(ConnectionString) || ConnectionString.ToLower().Contains("mysql"))
			{
				// SQLCE might get a performance hit (extra query)
				var orderExists = orderData.DatabaseId > 0 || GetOrderInfo(orderData.UniqueId) != null;
				sqlHelper.ExecuteNonQuery(orderExists ? @"UPDATE uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate,
								storeAlias = @storeAlias, customerID = @customerID, customerUsername = @customerUsername,
								customerEmail = @customerEmail, customerFirstName = @customerFirstName, orderNumber = @orderNumber,
								customerLastName = @customerLastName, transactionID = @transactionID where uniqueID = @uniqueID" : @"insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate, storeAlias, customerID, customerUsername, customerEmail, customerFirstName, customerLastName, transactionID, orderNumber, storeOrderReferenceID)
							values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate, @storeAlias, @customerID, @customerUsername, @customerEmail, @customerFirstName, @customerLastName, @transactionID, @orderNumber, @storeOrderReferenceID)", sqlHelper.CreateParameter("@orderInfo", orderData.OrderXML), sqlHelper.CreateParameter("@uniqueID", orderData.UniqueId), orderData.StoreOrderReferenceId.HasValue ? sqlHelper.CreateParameter("@storeOrderReferenceID", orderData.StoreOrderReferenceId.GetValueOrDefault()) : sqlHelper.CreateParameter("@storeOrderReferenceID", DBNull.Value), string.IsNullOrWhiteSpace(orderData.OrderReferenceNumber) ? sqlHelper.CreateParameter("@orderNumber", DBNull.Value) : sqlHelper.CreateParameter("@orderNumber", orderData.OrderReferenceNumber), string.IsNullOrWhiteSpace(orderData.OrderStatus) ? sqlHelper.CreateParameter("@orderStatus", DBNull.Value) : sqlHelper.CreateParameter("@orderStatus", orderData.OrderStatus), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now), string.IsNullOrWhiteSpace(orderData.StoreAlias) ? sqlHelper.CreateParameter("@storeAlias", DBNull.Value) : sqlHelper.CreateParameter("@storeAlias", orderData.StoreAlias), orderData.CustomerId == null ? sqlHelper.CreateParameter("@customerID", DBNull.Value) : sqlHelper.CreateParameter("@customerID", orderData.CustomerId), string.IsNullOrWhiteSpace(orderData.CustomerUsername) ? sqlHelper.CreateParameter("@customerUsername", DBNull.Value) : sqlHelper.CreateParameter("@customerUsername", orderData.CustomerUsername), string.IsNullOrWhiteSpace(orderData.CustomerEmail) ? sqlHelper.CreateParameter("@customerEmail", DBNull.Value) : sqlHelper.CreateParameter("@customerEmail", orderData.CustomerEmail), string.IsNullOrWhiteSpace(orderData.CustomerFirstName) ? sqlHelper.CreateParameter("@customerFirstName", DBNull.Value) : sqlHelper.CreateParameter("@customerFirstName", orderData.CustomerFirstName), string.IsNullOrWhiteSpace(orderData.CustomerLastName) ? sqlHelper.CreateParameter("@customerLastName", DBNull.Value) : sqlHelper.CreateParameter("@customerLastName", orderData.CustomerLastName), string.IsNullOrWhiteSpace(orderData.TransactionId) ? sqlHelper.CreateParameter("@transactionID", DBNull.Value) : sqlHelper.CreateParameter("@transactionID", orderData.TransactionId));
				if (!orderExists)
				{
					// another performance hit for sqlCE, select identity not possible within same command
					var insertedId = sqlHelper.ExecuteScalar<int>("select id from uWebshopOrders where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderData.UniqueId));
					if (orderData.DatabaseId == 0 && insertedId > 0)
					{
						orderData.SetGeneratedDatabaseId(insertedId);
					}
				}
			}
			else
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< combined INSERTorUPDATE orderInfo");
				var insertedId = sqlHelper.ExecuteScalar<int>(@"IF (SELECT Count(ID) FROM uWebshopOrders WHERE uniqueID = @uniqueId)=0 
					BEGIN 
						insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate, storeAlias, customerID, customerUsername, customerEmail, customerFirstName, customerLastName, transactionID, orderNumber, storeOrderReferenceID)
						values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate, @storeAlias, @customerID, @customerUsername, @customerEmail, @customerFirstName, @customerLastName, @transactionID, @orderNumber, @storeOrderReferenceID) 
						select @@IDENTITY
					END 
					ELSE 
					BEGIN 
						update uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate,
							storeAlias = @storeAlias, customerID = @customerID, customerUsername = @customerUsername,
							customerEmail = @customerEmail, customerFirstName = @customerFirstName, orderNumber = @orderNumber, storeOrderReferenceID = @storeOrderReferenceID,
							customerLastName = @customerLastName, transactionID = @transactionID	where uniqueID = @uniqueID 
						select -1
					END", sqlHelper.CreateParameter("@orderInfo", orderData.OrderXML), sqlHelper.CreateParameter("@uniqueID", orderData.UniqueId), orderData.StoreOrderReferenceId.HasValue ? sqlHelper.CreateParameter("@storeOrderReferenceID", orderData.StoreOrderReferenceId.GetValueOrDefault()) : sqlHelper.CreateParameter("@storeOrderReferenceID", DBNull.Value), string.IsNullOrWhiteSpace(orderData.OrderReferenceNumber) ? sqlHelper.CreateParameter("@orderNumber", DBNull.Value) : sqlHelper.CreateParameter("@orderNumber", orderData.OrderReferenceNumber), string.IsNullOrWhiteSpace(orderData.OrderStatus) ? sqlHelper.CreateParameter("@orderStatus", DBNull.Value) : sqlHelper.CreateParameter("@orderStatus", orderData.OrderStatus), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now), string.IsNullOrWhiteSpace(orderData.StoreAlias) ? sqlHelper.CreateParameter("@storeAlias", DBNull.Value) : sqlHelper.CreateParameter("@storeAlias", orderData.StoreAlias), orderData.CustomerId == null ? sqlHelper.CreateParameter("@customerID", DBNull.Value) : sqlHelper.CreateParameter("@customerID", orderData.CustomerId), string.IsNullOrWhiteSpace(orderData.CustomerUsername) ? sqlHelper.CreateParameter("@customerUsername", DBNull.Value) : sqlHelper.CreateParameter("@customerUsername", orderData.CustomerUsername), string.IsNullOrWhiteSpace(orderData.CustomerEmail) ? sqlHelper.CreateParameter("@customerEmail", DBNull.Value) : sqlHelper.CreateParameter("@customerEmail", orderData.CustomerEmail), string.IsNullOrWhiteSpace(orderData.CustomerFirstName) ? sqlHelper.CreateParameter("@customerFirstName", DBNull.Value) : sqlHelper.CreateParameter("@customerFirstName", orderData.CustomerFirstName), string.IsNullOrWhiteSpace(orderData.CustomerLastName) ? sqlHelper.CreateParameter("@customerLastName", DBNull.Value) : sqlHelper.CreateParameter("@customerLastName", orderData.CustomerLastName), string.IsNullOrWhiteSpace(orderData.TransactionId) ? sqlHelper.CreateParameter("@transactionID", DBNull.Value) : sqlHelper.CreateParameter("@transactionID", orderData.TransactionId));
				if (orderData.DatabaseId == 0 && insertedId > 0)
				{
					orderData.SetGeneratedDatabaseId(insertedId);
				}
			}
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, OrderStatus orderStatus)
		{
			SetOrderInfo(orderId, serializedOrderInfoObject, orderStatus.ToString());
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, string orderStatus)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			if (sqlHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(ConnectionString) || ConnectionString.ToLower().Contains("mysql"))
			{
				// SQLCE gets a performance hit (extra query, no way around it)
				var orderExists = GetOrderInfo(orderId) != null;
				//if (orderExists)
				//    Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE orderInfo");
				//else
				//    Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< INSERT orderInfo");
				sqlHelper.ExecuteNonQuery(orderExists ? @"UPDATE uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID" : @"INSERT into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate) values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate)", sqlHelper.CreateParameter("@orderInfo", serializedOrderInfoObject), sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@orderStatus", orderStatus), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			else
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< combined INSERTorUPDATE orderInfo");
				sqlHelper.ExecuteNonQuery("IF (SELECT Count(ID) FROM uWebshopOrders WHERE uniqueID = @uniqueId)=0 BEGIN insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate) " + "values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate) END ELSE BEGIN update uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID END", sqlHelper.CreateParameter("@orderInfo", serializedOrderInfoObject), sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@orderStatus", orderStatus), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			}
		}

		public static void ChangeOrderStatus(Guid orderId, OrderStatus orderStatus)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@orderStatus", orderStatus), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void SetTransactionId(Guid orderId, string transactionId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set transactionID = @transactionID, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@transactionID", transactionId), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		/// <summary>
		/// Set the Umbraco Member Id (when using umbraco members)
		/// </summary>
		/// <param name="orderId">The order unique identifier.</param>
		/// <param name="customerId">The customer unique identifier.</param>
		public static void SetCustomerId(Guid orderId, int customerId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @customerID, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerID", customerId), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		/// <summary>
		/// Set the .Net Membership Loginname
		/// </summary>
		/// <param name="orderId">The order unique identifier.</param>
		/// <param name="userName">Name of the user.</param>
		public static void SetCustomer(Guid orderId, string userName)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @customerUsername, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerUsername", userName), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerUsername(Guid orderId, string newCustomerUsername)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @customerUsername, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerUsername", newCustomerUsername), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		[Browsable(false)]
		[Obsolete("use UpdateCustomerUsername(Guid orderId, string newCustomerUsername)")]
		public static void UpdateCustomerId(Guid orderId, int newCustomerId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @customerID, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerID", newCustomerId), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerId(int oldCustomerId, int newCustomerId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @newcustomerID, updateDate = @updateDate where customerID = @oldCustomerID", sqlHelper.CreateParameter("@newcustomerID", newCustomerId), sqlHelper.CreateParameter("@oldCustomerID", oldCustomerId), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerUsername(string oldCustomerUsername, string newCustomerUsername)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @newCustomerUserName, updateDate = @updateDate where customerUsername = @oldCustomerUserName", sqlHelper.CreateParameter("@newCustomerUserName", newCustomerUsername), sqlHelper.CreateParameter("@oldCustomerUserName", oldCustomerUsername), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void InstallOrderTable()
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);
			try
			{
				sqlHelper.ExecuteNonQuery(@"CREATE TABLE 
					[uWebshopOrders](
					[id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
					[uniqueID] [uniqueidentifier] NULL,
					[customerEmail] nvarchar (500) NULL, 
					[customerFirstName] nvarchar (500) NULL, 
					[customerLastName] nvarchar (500) NULL,
					[orderNumber] nvarchar (100) NULL,
					[storeOrderReferenceID] int NULL,
					[orderInfo] [ntext] NULL, 
					[orderStatus] nvarchar (100) NULL,
					[transactionID] nvarchar (100) NULL,
					[storeAlias] nvarchar (500) NULL,
					[customerID] int NULL,
					[customerUsername] nvarchar (500) NULL,
					[createDate] [datetime] NULL,
					[updateDate] [datetime] NULL,
					[deliveryDate] [datetime] NULL,
					[seriesID] [int] NULL)");
			}
			catch (Exception ex)
			{
				LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch: Already Exists? Exception: " + ex);
				try
				{
					sqlHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [customerUsername] nvarchar (500) NULL");
				}
				catch
				{
					LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch adding customerUsername column: Already Exists? Exception: " + ex);
				}
				try
				{
					sqlHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [storeOrderReferenceID] int NULL");
				}
				catch
				{
					LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch adding storeOrderReferenceID column: Already Exists? Exception: " + ex);
				}
				try
				{
					sqlHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [deliveryDate] [datetime] NULL");
					sqlHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [seriesID] [int] NULL");
				}
				catch
				{
					LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch adding deliveryDate,seriesID columns: Already Exists? Exception: " + ex);
				}
			}
		}

		public static void InstallOrderSeriesTable()
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);
			try
			{
				sqlHelper.ExecuteNonQuery(@"CREATE TABLE 
					[uWebshopOrderSeries](
					[id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
					[cronInterval] [varchar](100) NOT NULL,
					[start] [datetime] NOT NULL,
					[end] [datetime] NULL,
					[endAfterInstances] [int] NOT NULL)");
			}
			catch (Exception ex)
			{
				LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderSeriesTable Catch: Already Exists? Exception: " + ex);
			}
		}

		public static void SetCustomerInfo(Guid orderId, XElement element)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			if (element.Name == "customerEmail")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerEmail");
				sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerEmail = @customerEmail, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerEmail", element.Value), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			if (element.Name == "customerFirstName")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerFirstName");
				sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerFirstName = @customerFirstName, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerFirstName", element.Value), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			if (element.Name == "customerLastName")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerLastName");
				sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerLastName = @customerLastName, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", orderId), sqlHelper.CreateParameter("@customerLastName", element.Value), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			}
		}
		public static void SetCustomerInfo(Guid orderId, string customerEmail, string customerFirstName, string customerLastName)
		{
			if (string.IsNullOrEmpty(customerFirstName))
			{
				customerFirstName = string.Empty;
			}
			if (string.IsNullOrEmpty(customerLastName))
			{
				customerLastName = string.Empty;
			}
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set customerEmail = @customerEmail, " +
				"customerFirstName = @customerFirstName, customerLastName = @customerLastName, " +
				"updateDate = @updateDate where uniqueID = @uniqueID",
				sqlHelper.CreateParameter("@uniqueID", orderId),
				sqlHelper.CreateParameter("@customerEmail", customerEmail),
				sqlHelper.CreateParameter("@customerFirstName", customerFirstName),
				sqlHelper.CreateParameter("@customerLastName", customerLastName),
				sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static string GetHighestOrderNumberForStore(string storeAlias, ref int referenceId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT orderNumber, storeOrderReferenceID FROM uWebshopOrders WHERE StoreAlias = @storeAlias ORDER BY id DESC", sqlHelper.CreateParameter("@storeAlias", storeAlias)))
			{
				while (reader.Read())
				{
					referenceId = reader.Get<int>("storeOrderReferenceID");

					var orderNumber = reader.Get<string>("orderNumber");

					if (!string.IsNullOrEmpty(orderNumber) && !orderNumber.StartsWith("[INCOMPLETE]"))
					{
						return orderNumber;
					}
				}
			}
			return null;
		}

		public static string GetHighestOrderNumber(ref int referenceId)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			using (var reader = sqlHelper.ExecuteReader("SELECT orderNumber, storeOrderReferenceID FROM uWebshopOrders ORDER BY id DESC"))
			{
				while (reader.Read())
				{
					referenceId = reader.Get<int>("storeOrderReferenceID");

					var orderNumber = reader.Get<string>("orderNumber");

					if (!string.IsNullOrEmpty(orderNumber) && !orderNumber.StartsWith("[INCOMPLETE]"))
					{
						return orderNumber;
					}
				}
			}
			return null;
		}


		public static void SetOrderNumber(Guid uniqueOrderId, string orderNumber, string storeAlias, int storeOrderReferenceID)
		{
			LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "SetOrderNumber orderNumber: " + orderNumber + " storeOrderReferenceID: " + storeOrderReferenceID);

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("update uWebshopOrders set orderNumber = @orderNumber, storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where uniqueID = @uniqueID", sqlHelper.CreateParameter("@uniqueID", uniqueOrderId), sqlHelper.CreateParameter("@storeOrderReferenceID", storeOrderReferenceID), sqlHelper.CreateParameter("@orderNumber", orderNumber), sqlHelper.CreateParameter("@storeAlias", storeAlias), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int AssignNewOrderNumberToOrder(int databaseId, string alias, int orderNumberStartNumber)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			if (databaseId <= 0) throw new Exception("No valid database id");

			return sqlHelper.ExecuteScalar<int>(@"begin tran

declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders WHERE StoreAlias = @storeAlias ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id

select @storeOrderReferenceID

commit tran", sqlHelper.CreateParameter("@id", databaseId), sqlHelper.CreateParameter("@orderNumberStartNumber", orderNumberStartNumber), sqlHelper.CreateParameter("@storeAlias", alias), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int AssignNewOrderNumberToOrderSharedBasket(int databaseId, string alias, int orderNumberStartNumber)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			if (databaseId <= 0) throw new Exception("No valid database id");

			return sqlHelper.ExecuteScalar<int>(@"begin tran

declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id

select @storeOrderReferenceID

commit tran", sqlHelper.CreateParameter("@id", databaseId), sqlHelper.CreateParameter("@orderNumberStartNumber", orderNumberStartNumber), sqlHelper.CreateParameter("@storeAlias", alias), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int DetermineLastOrderId()
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			var lastOrderNumber = 0;

			if (ConnectionString.ToLower().Contains("mysql"))
			{
				// MySQL: SELECT * FROM uWebshopOrders ORDER BY id desc LIMIT 0,1
				var mySqLreader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopOrders ORDER BY id desc LIMIT 0,1");

				while (mySqLreader.Read())
				{
					lastOrderNumber = mySqLreader.GetInt("id");
				}
			}
			else
			{
				var reader = sqlHelper.ExecuteReader("SELECT TOP(1) id FROM uWebshopOrders ORDER BY id DESC");

				while (reader.Read())
				{
					lastOrderNumber = reader.GetInt("id");
				}
			}
			return lastOrderNumber;
		}

		public static void Delete(IEnumerable<OrderData> orders)
		{
			Delete(orders.Select(order => order.UniqueId));
		}

		public static void Delete(IEnumerable<Guid> orderGuids)
		{
			if (!orderGuids.Any()) return;

			var sqlHelper = DataLayerHelper.CreateSqlHelper(ConnectionString);

			sqlHelper.ExecuteNonQuery("DELETE from uWebshopOrders where uniqueID in (" + string.Join(",", orderGuids.Select(g => g.ToString()).ToArray()) + ")");
		}
	}
}