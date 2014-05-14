using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Common;
using uWebshop.Common.Interfaces;

namespace uWebshop.DataAccess
{
	public class OrderData
	{
		public IDatabaseOrder Order;

		public int DatabaseId;
		public Guid UniqueId;
		public string StoreAlias;
		public int? StoreOrderReferenceId;
		public string OrderReferenceNumber;

		public string OrderXML;
		public string OrderStatus;

		public DateTime? CreateDate;
		public DateTime? UpdateDate;

		public int? CustomerId;
		public string CustomerUsername;
		public string CustomerEmail;
		public string CustomerFirstName;
		public string CustomerLastName;

		public string TransactionId;

		public OrderData()
		{
		}

		public OrderData(umbraco.DataLayer.IRecordsReader reader)
		{
			DatabaseId = reader.GetInt("id");
			UniqueId = reader.GetGuid("uniqueID");
			StoreAlias = reader.GetString("storeAlias");
			if (!reader.IsNull("storeOrderReferenceID"))
				StoreOrderReferenceId = reader.GetInt("storeOrderReferenceID");
			OrderReferenceNumber = reader.GetString("orderNumber");

			OrderXML = reader.GetString("orderInfo");
			OrderStatus = reader.GetString("orderStatus");

			if (!reader.IsNull("createDate"))
				CreateDate = reader.GetDateTime("createDate");
			if (!reader.IsNull("updateDate"))
				UpdateDate = reader.GetDateTime("updateDate");

			if (!reader.IsNull("customerID"))
				CustomerId = reader.GetInt("customerID");
			CustomerUsername = reader.GetString("customerUsername");
			CustomerEmail = reader.GetString("customerEmail");
			CustomerFirstName = reader.GetString("customerFirstName");
			CustomerLastName = reader.GetString("customerLastName");

			TransactionId = reader.GetString("transactionID");
		}

		public void SetGeneratedDatabaseId(int id)
		{
			DatabaseId = id;
			if (Order != null) Order.DatabaseId = id;
		}
	}
}