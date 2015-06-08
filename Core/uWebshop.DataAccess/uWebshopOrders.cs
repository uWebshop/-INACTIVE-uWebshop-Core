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
		public static string ConnectionString;

		internal static ISqlHelper SQLHelper
		{
			get { return DataLayerHelper.CreateSqlHelper(ConnectionString); } // todo: test if this object can be reused
		}
		
		public static List<OrderData> GetAllOrderInfos(string where = null)
		{
			if (where == null) where = string.Empty;

			var orderInfos = new List<OrderData>();
			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  " + where))
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
			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  WHERE uniqueID = @uniqueId", SQLHelper.CreateParameter("@uniqueId", orderId)))
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
			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id WHERE transactionID = @transactionID", SQLHelper.CreateParameter("@transactionID", transactionId)))
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

		public static OrderData GetOrderInfo(int id)
		{
			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id WHERE uWebshopOrders.id = @orderId", SQLHelper.CreateParameter("@orderId", id)))
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

			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  WHERE customerID = @customerID"
					+ (includeIncomplete ? "" : " and not orderStatus = 'Incomplete' and not orderStatus = 'Wishlist'")
				, SQLHelper.CreateParameter("@customerID", customerId)))
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

			using (var reader = SQLHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  WHERE customerUsername = @customerUsername"
					+ (includeIncomplete ? "" : " and not orderStatus = 'Incomplete' and not orderStatus = 'Wishlist'"),
				SQLHelper.CreateParameter("@customerUsername", customerUsername)))
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

			using (var reader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  WHERE customerID = @customerID and orderStatus = 'Wishlist'",
				SQLHelper.CreateParameter("@customerID", customerId)))
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

			using (var reader = SQLHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id  WHERE customerUsername = @customerUsername and orderStatus = 'Wishlist'",
				SQLHelper.CreateParameter("@customerUsername", customerUsername)))
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

			using (var reader = SQLHelper.ExecuteReader(
				"SELECT * FROM uWebshopOrders left outer join uWebshopOrderSeries on seriesID = uWebshopOrderSeries.id " +
				" WHERE deliveryDate >= @startTime and deliveryDate < @endTime",
				SQLHelper.CreateParameter("@startTime", startTime),
				SQLHelper.CreateParameter("@endTime", endTime)))
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

			if (!string.IsNullOrWhiteSpace(orderData.SeriesCronInterval))
			{
				// todo create or update
			}

			if (SQLHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(ConnectionString) || ConnectionString.ToLower().Contains("mysql"))
			{
				// todo: orderseries on SQLCE

				// SQLCE might get a performance hit (extra query)
				var orderExists = orderData.DatabaseId > 0 || GetOrderInfo(orderData.UniqueId) != null;
				SQLHelper.ExecuteNonQuery(orderExists ? @"UPDATE uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate,
								storeAlias = @storeAlias, customerID = @customerID, customerUsername = @customerUsername,
								customerEmail = @customerEmail, customerFirstName = @customerFirstName, orderNumber = @orderNumber,
								customerLastName = @customerLastName, transactionID = @transactionID, deliveryDate = @deliveryDate, seriesID = @seriesID where uniqueID = @uniqueID" : @"insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate, storeAlias, customerID, customerUsername, customerEmail, customerFirstName, customerLastName, transactionID, orderNumber, storeOrderReferenceID, deliveryDate, seriesID)
							values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate, @storeAlias, @customerID, @customerUsername, @customerEmail, @customerFirstName, @customerLastName, @transactionID, @orderNumber, @storeOrderReferenceID, @deliveryDate, @seriesID)", 
						SQLHelper.CreateParameter("@orderInfo", orderData.OrderXML), 
						SQLHelper.CreateParameter("@uniqueID", orderData.UniqueId),
						CreateParameterFromNullableValue("@storeOrderReferenceID", orderData.StoreOrderReferenceId),
						CreateDbNullStringParameter("@orderNumber", orderData.OrderReferenceNumber),
						CreateDbNullStringParameter("@orderStatus", orderData.OrderStatus), 
						SQLHelper.CreateParameter("@createDate", DateTime.Now), 
						SQLHelper.CreateParameter("@updateDate", DateTime.Now),
						CreateDbNullStringParameter("@storeAlias", orderData.StoreAlias),
						CreateParameterFromNullableValue("@customerID", orderData.CustomerId),
						CreateDbNullStringParameter("@customerUsername", orderData.CustomerUsername),
						CreateDbNullStringParameter("@customerEmail", orderData.CustomerEmail),
						CreateDbNullStringParameter("@customerFirstName", orderData.CustomerFirstName),
						CreateDbNullStringParameter("@customerLastName", orderData.CustomerLastName),
						CreateDbNullStringParameter("@transactionID", orderData.TransactionId),
						CreateParameterFromNullableValue("@deliveryDate", orderData.DeliveryDate),
						SQLHelper.CreateParameter("@seriesID", orderData.SeriesId));
				if (!orderExists)
				{
					// another performance hit for sqlCE, select identity not possible within same command
					var insertedId = SQLHelper.ExecuteScalar<int>("select id from uWebshopOrders where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderData.UniqueId));
					if (orderData.DatabaseId == 0 && insertedId > 0)
					{
						orderData.SetGeneratedDatabaseId(insertedId);
					}
				}
			}
			else
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< combined INSERTorUPDATE orderInfo");
				var insertedId = SQLHelper.ExecuteScalar<int>(
					@"
					IF (@seriesID > 0 and @seriesCronInterval is not null)
					BEGIN
						update uWebshopOrderSeries 
							set cronInterval = @seriesCronInterval, [start] = @seriesStart, [end] = @seriesEnd, endAfterInstances = @seriesEndAfterInstances
							where id = @seriesID
					END 
					ELSE IF (@seriesID = 0 and @seriesCronInterval is not null)
					BEGIN
						insert into uWebshopOrderSeries(cronInterval,[start],[end],endAfterInstances)
						values(@seriesCronInterval,@seriesStart,@seriesEnd,@seriesEndAfterInstances)
						select @seriesID = @@IDENTITY
					END
					ELSE IF (@seriesID > 0)
					BEGIN
						delete from uWebshopOrderSeries where id = @seriesID
						select @seriesID = 0
					END

					IF (SELECT Count(ID) FROM uWebshopOrders WHERE uniqueID = @uniqueId)=0 
					BEGIN 
						insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate, storeAlias, customerID, customerUsername, customerEmail, customerFirstName, customerLastName, transactionID, orderNumber, storeOrderReferenceID, deliveryDate, seriesID)
						values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate, @storeAlias, @customerID, @customerUsername, @customerEmail, @customerFirstName, @customerLastName, @transactionID, @orderNumber, @storeOrderReferenceID, @deliveryDate, @seriesID) 
						select @@IDENTITY
					END 
					ELSE 
					BEGIN 
						update uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate,
							storeAlias = @storeAlias, customerID = @customerID, customerUsername = @customerUsername,
							customerEmail = @customerEmail, customerFirstName = @customerFirstName, orderNumber = @orderNumber, storeOrderReferenceID = @storeOrderReferenceID,
							customerLastName = @customerLastName, transactionID = @transactionID, deliveryDate = @deliveryDate, seriesID = @seriesID	where uniqueID = @uniqueID 
						select -1
					END", 
						SQLHelper.CreateParameter("@orderInfo", orderData.OrderXML), 
						SQLHelper.CreateParameter("@uniqueID", orderData.UniqueId),
						CreateParameterFromNullableValue("@storeOrderReferenceID", orderData.StoreOrderReferenceId),
						//orderData.StoreOrderReferenceId.HasValue ? SQLHelper.CreateParameter("@storeOrderReferenceID", orderData.StoreOrderReferenceId.GetValueOrDefault()) : SQLHelper.CreateParameter("@storeOrderReferenceID", DBNull.Value), 
						CreateDbNullStringParameter("@orderNumber", orderData.OrderReferenceNumber),
						CreateDbNullStringParameter("@orderStatus", orderData.OrderStatus),
						SQLHelper.CreateParameter("@createDate", DateTime.Now), 
						SQLHelper.CreateParameter("@updateDate", DateTime.Now),
						CreateDbNullStringParameter("@storeAlias", orderData.StoreAlias),
						CreateParameterFromNullableValue("@customerID", orderData.CustomerId),
						//orderData.CustomerId == null ? SQLHelper.CreateParameter("@customerID", DBNull.Value) : SQLHelper.CreateParameter("@customerID", orderData.CustomerId),
						CreateDbNullStringParameter("@customerUsername", orderData.CustomerUsername),
						CreateDbNullStringParameter("@customerEmail", orderData.CustomerEmail),
						CreateDbNullStringParameter("@customerFirstName", orderData.CustomerFirstName),
						CreateDbNullStringParameter("@customerLastName", orderData.CustomerLastName),
						CreateDbNullStringParameter("@transactionID", orderData.TransactionId),
						CreateParameterFromNullableValue("@deliveryDate", orderData.DeliveryDate),
						//orderData.DeliveryDate != null ? SQLHelper.CreateParameter("@deliveryDate", orderData.DeliveryDate) : SQLHelper.CreateParameter("@deliveryDate", DBNull.Value), 
						SQLHelper.CreateParameter("@seriesID", orderData.SeriesId),
						CreateDbNullStringParameter("@seriesCronInterval", orderData.SeriesCronInterval),
						CreateParameterFromNullableValue("@seriesStart", orderData.SeriesStart),
						CreateParameterFromNullableValue("@seriesEnd", orderData.SeriesEnd),
						SQLHelper.CreateParameter("@seriesEndAfterInstances", orderData.SeriesEndAfterInstances)
					);
				if (orderData.DatabaseId == 0 && insertedId > 0)
				{
					orderData.SetGeneratedDatabaseId(insertedId);
				}
				// todo: NB, ID of OrderSeries object is not set
				if (!string.IsNullOrWhiteSpace(orderData.SeriesCronInterval))
				{
					var newSeriesId = SQLHelper.ExecuteScalar<int>("select seriesId from uWebshopOrders where id = @orderId", SQLHelper.CreateParameter("@orderId", orderData.DatabaseId));
					if (newSeriesId < 1) throw new ApplicationException("Did not create orderSeries in database or no id");
					orderData.SetGeneratedDatabaseSeriesId(newSeriesId);
				}
			}
		}
		private static IParameter CreateDbNullStringParameter(string parameterName, string field)
		{
			return SQLHelper.CreateParameter(parameterName, string.IsNullOrWhiteSpace(field) ? DBNull.Value : (object)field);
		}
		private static IParameter CreateParameterFromNullableValue<T>(string parameterName, T? value) where T : struct, IComparable
		{
			return SQLHelper.CreateParameter(parameterName, value == null ? DBNull.Value : (object)value.Value);
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, OrderStatus orderStatus)
		{
			SetOrderInfo(orderId, serializedOrderInfoObject, orderStatus.ToString());
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, string orderStatus)
		{
			if (SQLHelper.ConnectionString.Contains("|DataDirectory|") || DataLayerHelper.IsEmbeddedDatabase(ConnectionString) || ConnectionString.ToLower().Contains("mysql"))
			{
				// SQLCE gets a performance hit (extra query, no way around it)
				var orderExists = GetOrderInfo(orderId) != null;
				//if (orderExists)
				//    Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE orderInfo");
				//else
				//    Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< INSERT orderInfo");
				SQLHelper.ExecuteNonQuery(orderExists ? @"UPDATE uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID" : @"INSERT into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate) values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate)", SQLHelper.CreateParameter("@orderInfo", serializedOrderInfoObject), SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@orderStatus", orderStatus), SQLHelper.CreateParameter("@createDate", DateTime.Now), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			else
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< combined INSERTorUPDATE orderInfo");
				SQLHelper.ExecuteNonQuery("IF (SELECT Count(ID) FROM uWebshopOrders WHERE uniqueID = @uniqueId)=0 BEGIN insert into uWebshopOrders(uniqueID, orderInfo, orderStatus, createDate, updateDate) " + "values(@uniqueID, @orderInfo, @orderStatus, @createDate, @updateDate) END ELSE BEGIN update uWebshopOrders set orderInfo = @orderInfo, orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID END", SQLHelper.CreateParameter("@orderInfo", serializedOrderInfoObject), SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@orderStatus", orderStatus), SQLHelper.CreateParameter("@createDate", DateTime.Now), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
			}
		}

		public static void ChangeOrderStatus(Guid orderId, OrderStatus orderStatus)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set orderStatus = @orderStatus, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@orderStatus", orderStatus), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void SetTransactionId(Guid orderId, string transactionId)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set transactionID = @transactionID, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@transactionID", transactionId), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		/// <summary>
		/// Set the Umbraco Member Id (when using umbraco members)
		/// </summary>
		/// <param name="orderId">The order unique identifier.</param>
		/// <param name="customerId">The customer unique identifier.</param>
		public static void SetCustomerId(Guid orderId, int customerId)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @customerID, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerID", customerId), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}
		
		/// <summary>
		/// Set the .Net Membership Loginname
		/// </summary>
		/// <param name="orderId">The order unique identifier.</param>
		/// <param name="userName">Name of the user.</param>
		public static void SetCustomer(Guid orderId, string userName)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @customerUsername, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerUsername", userName), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerUsername(Guid orderId, string newCustomerUsername)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @customerUsername, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerUsername", newCustomerUsername), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		[Browsable(false)]
		[Obsolete("use UpdateCustomerUsername(Guid orderId, string newCustomerUsername)")]
		public static void UpdateCustomerId(Guid orderId, int newCustomerId)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @customerID, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerID", newCustomerId), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerId(int oldCustomerId, int newCustomerId)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerID = @newcustomerID, updateDate = @updateDate where customerID = @oldCustomerID", SQLHelper.CreateParameter("@newcustomerID", newCustomerId), SQLHelper.CreateParameter("@oldCustomerID", oldCustomerId), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void UpdateCustomerUsername(string oldCustomerUsername, string newCustomerUsername)
		{
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerUsername = @newCustomerUserName, updateDate = @updateDate where customerUsername = @oldCustomerUserName", SQLHelper.CreateParameter("@newCustomerUserName", newCustomerUsername), SQLHelper.CreateParameter("@oldCustomerUserName", oldCustomerUsername), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static void InstallOrderTable()
		{
			try
			{
				SQLHelper.ExecuteNonQuery(@"CREATE TABLE 
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
					SQLHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [customerUsername] nvarchar (500) NULL");
				}
				catch
				{
					LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch adding customerUsername column: Already Exists? Exception: " + ex);
				}
				try
				{
					SQLHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [storeOrderReferenceID] int NULL");
				}
				catch
				{
					LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "InstallOrderTable Catch adding storeOrderReferenceID column: Already Exists? Exception: " + ex);
				}
				try
				{
					SQLHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
						ADD [deliveryDate] [datetime] NULL");
					SQLHelper.ExecuteNonQuery(@"ALTER TABLE [uWebshopOrders]
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
			try
			{
				SQLHelper.ExecuteNonQuery(@"CREATE TABLE 
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
			if (element.Name == "customerEmail")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerEmail");
				SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerEmail = @customerEmail, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerEmail", element.Value), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			if (element.Name == "customerFirstName")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerFirstName");
				SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerFirstName = @customerFirstName, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerFirstName", element.Value), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
			}
			if (element.Name == "customerLastName")
			{
				//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " >>>>SQL<<<< UPDATE customerLastName");
				SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerLastName = @customerLastName, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", orderId), SQLHelper.CreateParameter("@customerLastName", element.Value), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
			}
		}
		public static void SetCustomerInfo(Guid orderId, string customerEmail, string customerFirstName, string customerLastName)
		{
			customerFirstName = customerFirstName ?? string.Empty;
			customerLastName = customerLastName ?? string.Empty;
			customerEmail = customerEmail ?? string.Empty;
			SQLHelper.ExecuteNonQuery("update uWebshopOrders set customerEmail = @customerEmail, " +
				"customerFirstName = @customerFirstName, customerLastName = @customerLastName, " +
				"updateDate = @updateDate where uniqueID = @uniqueID",
				SQLHelper.CreateParameter("@uniqueID", orderId),
				SQLHelper.CreateParameter("@customerEmail", customerEmail),
				SQLHelper.CreateParameter("@customerFirstName", customerFirstName),
				SQLHelper.CreateParameter("@customerLastName", customerLastName),
				SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static string GetHighestOrderNumberForStore(string storeAlias, ref int referenceId)
		{
			using (var reader = SQLHelper.ExecuteReader("SELECT orderNumber, storeOrderReferenceID FROM uWebshopOrders WHERE StoreAlias = @storeAlias ORDER BY id DESC", SQLHelper.CreateParameter("@storeAlias", storeAlias)))
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
			using (var reader = SQLHelper.ExecuteReader("SELECT orderNumber, storeOrderReferenceID FROM uWebshopOrders ORDER BY id DESC"))
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

			SQLHelper.ExecuteNonQuery("update uWebshopOrders set orderNumber = @orderNumber, storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where uniqueID = @uniqueID", SQLHelper.CreateParameter("@uniqueID", uniqueOrderId), SQLHelper.CreateParameter("@storeOrderReferenceID", storeOrderReferenceID), SQLHelper.CreateParameter("@orderNumber", orderNumber), SQLHelper.CreateParameter("@storeAlias", storeAlias), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int AssignNewOrderNumberToOrder(int databaseId, string alias, int orderNumberStartNumber)
		{
			if (databaseId <= 0) throw new Exception("No valid database id");

			return SQLHelper.ExecuteScalar<int>(@"begin tran

declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders WHERE StoreAlias = @storeAlias ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id

select @storeOrderReferenceID

commit tran", SQLHelper.CreateParameter("@id", databaseId), SQLHelper.CreateParameter("@orderNumberStartNumber", orderNumberStartNumber), SQLHelper.CreateParameter("@storeAlias", alias), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int AssignNewOrderNumberToOrderSharedBasket(int databaseId, string alias, int orderNumberStartNumber)
		{
			if (databaseId <= 0) throw new Exception("No valid database id");

			return SQLHelper.ExecuteScalar<int>(@"begin tran

declare @storeOrderReferenceID int
set @storeOrderReferenceID =  coalesce((SELECT top 1 storeOrderReferenceID FROM uWebshopOrders ORDER BY storeOrderReferenceID DESC),0) + 1
set @storeOrderReferenceID = case when @orderNumberStartNumber > @storeOrderReferenceID then @orderNumberStartNumber else @storeOrderReferenceID end

update uWebshopOrders set storeOrderReferenceID = @storeOrderReferenceID, storeAlias = @storeAlias, updateDate = @updateDate where id = @id

select @storeOrderReferenceID

commit tran", SQLHelper.CreateParameter("@id", databaseId), SQLHelper.CreateParameter("@orderNumberStartNumber", orderNumberStartNumber), SQLHelper.CreateParameter("@storeAlias", alias), SQLHelper.CreateParameter("@updateDate", DateTime.Now));
		}

		public static int DetermineLastOrderId()
		{
			var lastOrderNumber = 0;

			if (ConnectionString.ToLower().Contains("mysql"))
			{
				// MySQL: SELECT * FROM uWebshopOrders ORDER BY id desc LIMIT 0,1
				var mySqLreader = SQLHelper.ExecuteReader("SELECT * FROM uWebshopOrders ORDER BY id desc LIMIT 0,1");

				while (mySqLreader.Read())
				{
					lastOrderNumber = mySqLreader.GetInt("id");
				}
			}
			else
			{
				var reader = SQLHelper.ExecuteReader("SELECT TOP(1) id FROM uWebshopOrders ORDER BY id DESC");

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

			SQLHelper.ExecuteNonQuery("DELETE from uWebshopOrders where uniqueID in (" + string.Join(",", orderGuids.Select(g => g.ToString()).ToArray()) + ")");
		}

		public static void RemoveScheduledOrdersWithSeriesId(int seriesId)
		{
			if (seriesId <= 0) throw new ArgumentOutOfRangeException("seriesId");
			SQLHelper.ExecuteNonQuery("DELETE from uWebshopOrders where seriesID = @seriesId and orderStatus = 'Scheduled'", SQLHelper.CreateParameter("@seriesId", seriesId));
		}
	}
}