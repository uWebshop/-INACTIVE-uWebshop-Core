using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.DataLayer;

namespace uWebshop.DataAccess
{
	/// <summary>
	/// uWebshopStock dbtable contains the following columns:
	/// id
	/// NodeId = the Id of the pricing node this stockrow if for
	/// Stock = the stock count for this pricing
	/// Ordered = the total order count for this pricing todo: multi-shop support
	/// updateDate = the datetime value when the stock/ordered was last updated
	/// </summary>
	public class UWebshopStock
	{
		public const string AllStockCacheKey = "AllStockCacheKey";

		private static List<StockInfo> LoadAllStockInfo()
		{
			// todo: improve caching, use Dictionary<int, StockInfo>
			List<StockInfo> stocks;
			if (HttpContext.Current.Items.Contains(AllStockCacheKey))
			{
				stocks = ((List<StockInfo>) HttpContext.Current.Items[AllStockCacheKey]);
			}
			else
			{
				stocks = new List<StockInfo>();
				var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);

				var reader = sqlHelper.ExecuteReader("SELECT NodeId, Stock, StoreAlias, OrderCount FROM uWebshopStock");
				while (reader.Read())
				{
					stocks.Add(new StockInfo {NodeId = reader.GetInt("NodeId"), Stock = reader.GetInt("Stock"), StoreAlias = reader.GetString("StoreAlias"), OrderCount = reader.GetInt("OrderCount")});
				}
				HttpContext.Current.Items[AllStockCacheKey] = stocks;
			}
			return stocks;
		}

		public static Guid GetCacheGuid()
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);

			var reader = sqlHelper.ExecuteReader("SELECT cacheGuid FROM uWebshop");
			while (reader.Read())
			{
				reader.GetString("cacheGuid");
				//stocks.Add(new StockInfo { NodeId = reader.GetInt("NodeId"), Stock = reader.GetInt("Stock"), StoreAlias = reader.GetString("StoreAlias"), OrderCount = reader.GetInt("OrderCount") });
			}

			return Guid.Empty;
		}

		/// <summary>
		/// Returns the current stock of the given pricing
		/// </summary>
		/// <param name="nodeId">the nodeId of the product this stock applies to</param>
		/// <param name="storeAlias">the store alias to get the stock for this node for</param>
		/// <returns></returns>
		public static int GetStock(int nodeId, string storeAlias = null)
		{
			var stocks = LoadAllStockInfo();
			var firstOrDefault = stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == storeAlias);

			if (firstOrDefault != null)
			{
				return firstOrDefault.Stock;
			}

			var globalFallback = stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == string.Empty);

			if (globalFallback != null)
			{
				return globalFallback.Stock;
			}

			return 0;
		}

		/// <summary>
		/// Returns the count of how many times the pricing was ordered
		/// </summary>
		/// <param name="nodeId">the nodeId of the pricing this OrderCount applies to</param>
		/// <param name="storeAlias">the store alias to get the orderedcount for this node for</param>
		/// <returns></returns>
		public static int GetOrderCount(int nodeId, string storeAlias = null)
		{
			var stocks = LoadAllStockInfo();
			var firstOrDefault = stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == storeAlias);

			if (firstOrDefault != null)
			{
				return firstOrDefault.OrderCount;
			}

			var globalFallback = stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == string.Empty);

			if (globalFallback != null)
			{
				return globalFallback.OrderCount;
			}

			return 0;
		}


        public static int SubstractStock(int productId, int stockToUpdate, string storeAlias = null)
        {
            return SubstractStock(productId, stockToUpdate, true, storeAlias);
        }

        /// <summary>
        /// substracts the stock and orderedCount of the product
        /// </summary>
        /// <param name="productId">the nodeId of the pricing this stock applies to</param>
        /// <param name="stockToSubtract">the amount of stock to subtract from the current stock</param>
        /// <param name="storeAlias"> </param>
        /// <returns></returns>
	    public static int SubstractStock(int productId, int stockToSubtract, bool updateOrderCount,
	        string storeAlias = null)
	    {
            var setOrderCount = updateOrderCount;

            var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);
            storeAlias = storeAlias ?? string.Empty;

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var currentReader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopStock WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@storeAlias", storeAlias));

            while (currentReader.Read())
            {
                currentStock = currentReader.GetInt("Stock");
                orderedCount = currentReader.GetInt("OrderCount");
                currentNodeId = currentReader.GetInt("NodeId");
            }

            var newStock = currentStock - stockToSubtract;
            var orderCount = orderedCount + stockToSubtract;

            sqlHelper.ExecuteNonQuery(currentNodeId == 0 ? @"INSERT into uWebshopStock(NodeId, Stock, OrderCount, StoreAlias, createDate, updateDate) values(@pricingId, @stock, @orderCount, @storeAlias, @createDate, @updateDate)" : @"UPDATE uWebshopStock set Stock = @stock, OrderCount = @orderCount, StoreAlias = @storeAlias, updateDate = @updateDate WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@stock", newStock), sqlHelper.CreateParameter("@orderCount", setOrderCount ? orderCount : orderedCount), sqlHelper.CreateParameter("@storeAlias", storeAlias), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));

            currentReader.Close();

            return newStock;
	    }


        [Obsolete("use SubstractStock")]
        public static int SetStock(int productId, int stockToUpdate, string storeAlias = null)
        {
            return SubstractStock(productId, stockToUpdate, true, storeAlias);
        }


		/// <summary>
		/// Substracts the given stock and updates the orderedCount of the product/variant with the given stock value
		/// </summary>
		/// <param name="productId">the nodeId of the pricing this stock applies to</param>
		/// <param name="stockToSubtract">the amount of stock to subtract from the current stock</param>
		/// <param name="updateOrderCount">Update orderCount; default = true</param>
		/// <param name="storeAlias"> </param>
		/// <returns></returns>
		[Obsolete("use SubstractStock")]
        public static int SetStock(int productId, int stockToSubtract, bool updateOrderCount, string storeAlias = null)
		{
		    return SubstractStock(productId, stockToSubtract, updateOrderCount, storeAlias);
		}

		public static int ReturnStock(int productId, int stockToReturn, bool updateOrderCount, string storeAlias = null)
		{
			// todo: niet thread safe (geen transaction)

			var setOrderCount = updateOrderCount;

			var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);

			var currentStock = 0;
			var orderedCount = 0;
			var currentNodeId = 0;

			var currentReader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopStock WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@storeAlias", storeAlias));

			while (currentReader.Read())
			{
				currentStock = currentReader.GetInt("Stock");
				orderedCount = currentReader.GetInt("OrderCount");
				currentNodeId = currentReader.GetInt("NodeId");
			}

			var newStock = currentStock + stockToReturn;
			var orderCount = orderedCount - stockToReturn;

			sqlHelper.ExecuteNonQuery(currentNodeId == 0 ? @"INSERT into uWebshopStock(NodeId, Stock, OrderCount, StoreAlias, createDate, updateDate) values(@pricingId, @stock, @orderCount, @storeAlias, @createDate, @updateDate)" : @"UPDATE uWebshopStock set Stock = @stock, OrderCount = @orderCount, StoreAlias = @storeAlias, updateDate = @updateDate WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@stock", newStock), sqlHelper.CreateParameter("@orderCount", setOrderCount ? orderCount : orderedCount), sqlHelper.CreateParameter("@storeAlias", storeAlias), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));

			currentReader.Close();

			return newStock;
		}
		
		/// <summary>
		/// Updates the stock and orderedCount of the product
		/// </summary>
		/// <param name="productId">the nodeId of the pricing this stock applies to</param>
		/// <param name="orderCountToUpdate">the ordercount to add to current oirdercount</param>
		/// <param name="storeAlias"> </param>
		/// <returns></returns>
		public static int SetOrderCount(int productId, int orderCountToUpdate, string storeAlias = null)
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);
			storeAlias = storeAlias ?? string.Empty;

			var orderedCount = 0;
			var currentNodeId = 0;
			var stockCount = 0;

			var currentReader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopStock WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@storeAlias", storeAlias));

			while (currentReader.Read())
			{
				orderedCount = currentReader.GetInt("OrderCount");
				currentNodeId = currentReader.GetInt("NodeId");
				stockCount = currentReader.GetInt("Stock");
			}

			var orderCount = orderedCount + orderCountToUpdate;
			
			sqlHelper.ExecuteNonQuery(currentNodeId == 0 ? @"INSERT into uWebshopStock(NodeId, Stock, OrderCount, StoreAlias, createDate, updateDate) values(@pricingId,  @stock, @orderCount, @storeAlias, @createDate, @updateDate)" : @"UPDATE uWebshopStock set Stock = @stock, OrderCount = @orderCount, StoreAlias = @storeAlias, updateDate = @updateDate WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@stock", stockCount), sqlHelper.CreateParameter("@orderCount", orderCount), sqlHelper.CreateParameter("@storeAlias", storeAlias), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
			
			currentReader.Close();

			return orderCount;
		}


		public static void InstallStockTable()
		{
			var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);

			try
			{
				sqlHelper.ExecuteNonQuery(@"CREATE TABLE 
					[uWebshopStock](
					[id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
					[Stock] [int] NOT NULL,
					[NodeId] [int] NULL,
					[OrderCount] [int] NOT NULL, 
					[StoreAlias] nvarchar (500) NULL, 
					[createDate] [datetime] NULL,
					[updateDate] [datetime] NULL)");
			}
			catch (Exception ex)
			{
				//Log.Instance.LogDebug("InstallStockTable Catch: Already Exists?");
				Log.Add(LogTypes.Debug, 0, "InstallStockTable Catch: Already Exists?");
			}
		}

        /// <summary>
        /// Updates the stock to a specific value, replaces the current stock with the given value  
        /// </summary>
        /// <param name="productId">the nodeId of the pricing this stock applies to</param>
        /// <param name="newStock">the stock value to be set (overwrites the current stock value, does not update it!)</param>
        /// <param name="updateOrderCount">Update orderCount; default = true</param>
        /// <param name="storeAlias"></param>
	    public static void ReplaceStock(int productId, int newStock, bool updateOrderCount, string storeAlias = null)
	    {
            var setOrderCount = updateOrderCount;

            var sqlHelper = DataLayerHelper.CreateSqlHelper(GlobalSettings.DbDSN);

            if (string.IsNullOrEmpty(storeAlias))
            {
                storeAlias = string.Empty;
            }

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var currentReader = sqlHelper.ExecuteReader("SELECT * FROM uWebshopStock WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@storeAlias", storeAlias));

            while (currentReader.Read())
            {
                currentStock = currentReader.GetInt("Stock");
                orderedCount = currentReader.GetInt("OrderCount");
                currentNodeId = currentReader.GetInt("NodeId");
            }

            var orderCount = orderedCount + (newStock - currentStock);

            if (orderCount < 0)
            {
                orderCount = 0;
            }

            sqlHelper.ExecuteNonQuery(currentNodeId == 0 ? @"INSERT into uWebshopStock(NodeId, Stock, OrderCount, StoreAlias, createDate, updateDate) values(@pricingId, @stock, @orderCount, @storeAlias, @createDate, @updateDate)" : @"UPDATE uWebshopStock set Stock = @stock, OrderCount = @orderCount, StoreAlias = @storeAlias, updateDate = @updateDate WHERE NodeId = @pricingId AND StoreAlias = @storeAlias", sqlHelper.CreateParameter("@pricingId", productId), sqlHelper.CreateParameter("@stock", newStock), sqlHelper.CreateParameter("@orderCount", setOrderCount ? orderCount : orderedCount), sqlHelper.CreateParameter("@storeAlias", storeAlias), sqlHelper.CreateParameter("@createDate", DateTime.Now), sqlHelper.CreateParameter("@updateDate", DateTime.Now));
            currentReader.Close();
	    }


	   
        [Obsolete("Use ReplaceStock")]
        public static void UpdateStock(int productId, int newStock, bool updateOrderCount, string storeAlias = null)
	    {
	        ReplaceStock(productId, newStock, updateOrderCount, storeAlias);
	    }
	}

	public class StockInfo
	{
		public int NodeId;
		public string StoreAlias;
		public int Stock;
		public int OrderCount;
	}
}