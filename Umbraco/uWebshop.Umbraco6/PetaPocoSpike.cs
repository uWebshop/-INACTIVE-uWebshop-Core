using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using uWebshop.Common;

namespace uWebshop.Umbraco6
{
	internal class PetaPocoSpike
	{
		public static List<OrderData> GetAllOrderInfos()
		{
			// Get the current database object
			var db = ApplicationContext.Current.DatabaseContext.Database;
			// Fetch a collection of contacts from the db.
			return db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders"));
		}

		public static OrderData GetOrderInfo(Guid orderId)
		{
			// Get the current database object
			var db = ApplicationContext.Current.DatabaseContext.Database;
			// Fetch a collection of contacts from the db.
			return db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault();
		}

		public static OrderData GetOrderInfo(string transactionId)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			return db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.TransactionId == transactionId)).FirstOrDefault();
		}

		public static List<OrderData> GetOrdersFromCustomer(int customerId)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			return db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.CustomerId == customerId));
		}

		public static List<OrderData> GetOrdersFromCustomer(string customerUsername)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			return db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.CustomerUsername == customerUsername));
		}

		public static void StoreOrder(OrderData orderData)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;

			db.Save(orderData);
		}

		public static void SetOrderInfo(Guid orderId, string serializedOrderInfoObject, OrderStatus orderStatus)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault() ?? new OrderData {CreateDate = DateTime.Now};

			fetch.OrderInfo = serializedOrderInfoObject;
			fetch.OrderStatus = orderStatus.ToString();
			fetch.UpdateDate = DateTime.Now;

			db.Save(fetch);
		}

		public static void ChangeOrderStatus(Guid orderId, OrderStatus orderStatus)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault() ?? new OrderData {CreateDate = DateTime.Now};

			fetch.OrderStatus = orderStatus.ToString();
			fetch.UpdateDate = DateTime.Now;

			db.Save(fetch);
		}

		public static void SetTransactionId(Guid orderId, string transactionId)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault() ?? new OrderData {CreateDate = DateTime.Now};

			fetch.TransactionId = transactionId;
			fetch.UpdateDate = DateTime.Now;

			db.Save(fetch);
		}

		public static void SetCustomerId(Guid orderId, int customerId)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault() ?? new OrderData {CreateDate = DateTime.Now};

			fetch.CustomerId = customerId;
			fetch.UpdateDate = DateTime.Now;

			db.Save(fetch);
		}

		public static void SetCustomer(Guid orderId, string userName)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = GetOrCreateOrderWithGuid(orderId, db);

			fetch.CustomerUsername = userName;

			SaveOrder(fetch, db);
		}

		public static void SetCustomerInfo(Guid orderId, XElement element)
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;
			var fetch = GetOrCreateOrderWithGuid(orderId, db);

			if (element.Name == "customerEmail")
			{
				fetch.CustomerEmail = element.Value;
			}
			if (element.Name == "customerFirstName")
			{
				fetch.CustomerFirstName = element.Value;
			}
			if (element.Name == "customerLastName")
			{
				fetch.CustomerLastName = element.Value;
			}

			SaveOrder(fetch, db);
		}

		private static void SaveOrder(OrderData fetch, UmbracoDatabase db)
		{
			fetch.UpdateDate = DateTime.Now;

			db.Save(fetch);
		}

		private static OrderData GetOrCreateOrderWithGuid(Guid orderId, UmbracoDatabase db)
		{
			var fetch = db.Fetch<OrderData>(new Sql().Select("*").From("uWebshopOrders").Where((OrderData o) => o.UniqueId == orderId)).FirstOrDefault() ?? new OrderData {CreateDate = DateTime.Now};
			return fetch;
		}

		public static void InstallOrderTable()
		{
			var db = ApplicationContext.Current.DatabaseContext.Database;

			if (!db.TableExist("uWebshopOrders"))
			{
				db.CreateTable<OrderData>(false);
			}
			else
			{
				try
				{
					db.Execute(@"ALTER TABLE [uWebshopOrders] ADD [customerUsername] nvarchar (500) NULL");
				}
				catch
				{
				}
				try
				{
					db.Execute(@"ALTER TABLE [uWebshopOrders] ADD [storeOrderReferenceID] int NULL");
				}
				catch
				{
				}
			}
		}
	}

	[TableName("uWebshopOrders")]
	public class OrderData
	{
		[PrimaryKeyColumn(AutoIncrement = true)]
		public int Id { get; set; }

		public Guid UniqueId { get; set; }
		public string StoreAlias { get; set; }
		public int? StoreOrderReferenceId { get; set; }
		public string OrderNumber { get; set; }

		public string OrderInfo { get; set; }
		public string OrderStatus { get; set; }

		public DateTime? CreateDate { get; set; }
		public DateTime? UpdateDate { get; set; }

		public int? CustomerId { get; set; }
		public string CustomerUsername { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerFirstName { get; set; }
		public string CustomerLastName { get; set; }

		public string TransactionId { get; set; }
	}
}