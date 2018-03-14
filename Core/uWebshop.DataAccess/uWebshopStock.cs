using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using umbraco;
using uWebshop.DataAccess.Pocos;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;
using log4net;
using System.Reflection;
using System.Collections.Concurrent;

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
        protected static ConcurrentDictionary<string, uWebshopStock> _stockCache = new ConcurrentDictionary<string, uWebshopStock>();

        internal static UmbracoDatabase Database
        {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        private static void AddOrUpdateCache(int nodeId, uWebshopStock newCacheItem, string storeAlias = null) {

            string _storeAlias = !string.IsNullOrEmpty(storeAlias) ? storeAlias : string.Empty;

            string cacheKey = nodeId.ToString() + storeAlias;

            //Log.Info("AddOrUpdateCache: " + nodeId + " storeAlias: " + storeAlias + " stock: " + newCacheItem.Stock);

            _stockCache.AddOrUpdate(
                cacheKey,
                newCacheItem,
                (key, oldCacheItem) => newCacheItem);
        }

        private static List<uWebshopStock> LoadAllStockInfo()
        {
            // todo: improve caching, use Dictionary<int, StockInfo>
            List<uWebshopStock> stocks;
            if (HttpContext.Current.Items.Contains(AllStockCacheKey))
            {
                stocks = ((List<uWebshopStock>)HttpContext.Current.Items[AllStockCacheKey]);
            }
            else
            {
                var sql = Sql.Builder.Select("NodeId, Stock, StoreAlias, OrderCount")
                    .From("uWebshopStock");

                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {
                    stocks = db.Fetch<uWebshopStock>(sql);

                    HttpContext.Current.Items[AllStockCacheKey] = stocks;
                }

            }

            return stocks;
        }

        private static uWebshopStock FetchStockByIdAndAliasFromSql(int nodeId, string storeAlias = null) {

            string _storeAlias = !string.IsNullOrEmpty(storeAlias) ? storeAlias : string.Empty;

            string sql = "SELECT * FROM uWebshopStock WHERE NodeId = @0 AND StoreAlias = @1";

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                return db.Fetch<uWebshopStock>(sql, nodeId, _storeAlias).FirstOrDefault();
            }
                
        }

        private static uWebshopStock GetStockInfoByNodeIdAndAlias(int nodeId, string storeAlias = null)
        {
            string _storeAlias = !string.IsNullOrEmpty(storeAlias) ? storeAlias : string.Empty;

            string cacheKey = nodeId.ToString() +  storeAlias;

            var stock = _stockCache.GetOrAdd(cacheKey, alias => FetchStockByIdAndAliasFromSql(nodeId,storeAlias));

            return stock;
        }

        private static readonly ILog Log =
                LogManager.GetLogger(
                    MethodBase.GetCurrentMethod().DeclaringType
                );

        /// <summary>
        /// Returns the current stock of the given pricing
        /// </summary>
        /// <param name="nodeId">the nodeId of the product this stock applies to</param>
        /// <param name="storeAlias">the store alias to get the stock for this node for</param>
        /// <returns></returns>
        public static int GetStock(int nodeId, string storeAlias)
        {

            var stock = GetStockInfoByNodeIdAndAlias(nodeId, storeAlias);

            if (stock != null)
            {
                return stock.Stock;
            }

            var globalFallback = GetStockInfoByNodeIdAndAlias(nodeId, string.Empty);

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
            var stock = GetStockInfoByNodeIdAndAlias(nodeId, storeAlias);

            if (stock != null)
            {
                return stock.OrderCount;
            }

            var globalFallback = GetStockInfoByNodeIdAndAlias(nodeId, string.Empty);

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
        public static int SubstractStock(int productId, int stockToSubtract, bool updateOrderCount, string storeAlias = null)
        {
     
            try
            {
                storeAlias = storeAlias ?? string.Empty;

                var currentStock = 0;
                var orderedCount = 0;

                var sql = Sql.Builder.Select("*")
                    .From("uWebshopStock")
                    .Where("NodeId = @0", productId);

                if (!string.IsNullOrEmpty(storeAlias))
                {
                    sql.Where("StoreAlias = @0", storeAlias);
                }

                using (var db = ApplicationContext.Current.DatabaseContext.Database)
                {

                    var stockItem = db.FirstOrDefault<uWebshopStock>(sql);

                    // Maybe the storeAlias is empty
                    if (stockItem == null && !string.IsNullOrEmpty(storeAlias))
                    {
                        stockItem = db.FirstOrDefault<uWebshopStock>(Sql.Builder.Select("*").From("uWebshopStock").Where("NodeId = @0", productId));
                    }

                    if (stockItem != null)
                    {
                        stockItem.UpdateDateTime = DateTime.Now;
                        currentStock = stockItem.Stock;
                        orderedCount = stockItem.OrderCount;

                        stockItem.Stock = currentStock - stockToSubtract;
                        if (updateOrderCount)
                        {
                            stockItem.OrderCount = orderedCount + stockToSubtract;
                        }

                        db.Update(stockItem);
                    }
                    else
                    {

                        stockItem = new uWebshopStock
                        {
                            // todo: should this be 1?
                            Stock = 1,
                            NodeId = productId,
                            UpdateDateTime = DateTime.Now,
                            CreatDateTime = DateTime.Now
                        };

                        if (updateOrderCount)
                        {
                            stockItem.OrderCount = orderedCount + stockToSubtract;
                        }

                        db.Insert(stockItem);
                    }

                    AddOrUpdateCache(productId, stockItem, storeAlias);

                    try
                    {
                        AfterSubtractStockChanged(new AfterSubtractStockChangedEventArgs
                        {
                            productId = productId,
                            stockItem = stockItem,
                            storeAlias = storeAlias
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error("AfterSubtractStockChanged error!", ex);
                    }

                    return stockItem.Stock;
                }

            } catch (Exception ex)
            {
                Log.Error("SubstractStock Failed!", ex);
                throw;
            }

        }


        public static int ReturnStock(int productId, int stockToReturn, bool updateOrderCount, string storeAlias = null)
        {
            // todo: not thread safe (no transaction)

            storeAlias = storeAlias ?? string.Empty;

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId);

            if (!string.IsNullOrEmpty(storeAlias))
            {
                sql.Where("StoreAlias = @0", storeAlias);
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {

                var stockItem = db.FirstOrDefault<uWebshopStock>(sql);

                if (stockItem != null)
                {
                    stockItem.UpdateDateTime = DateTime.Now;
                    currentStock = stockItem.Stock;
                    orderedCount = stockItem.OrderCount;

                    stockItem.Stock = currentStock + stockToReturn;
                    if (updateOrderCount)
                    {
                        stockItem.OrderCount = orderedCount - stockToReturn;
                    }
                    db.Update(stockItem);
                }
                else
                {
                    stockItem = new uWebshopStock
                    {
                        Stock = stockToReturn,
                        NodeId = currentNodeId,
                        UpdateDateTime = DateTime.Now
                    };

                    if (updateOrderCount)
                    {
                        stockItem.OrderCount = orderedCount - stockToReturn;
                    }

                    db.Insert(stockItem);
                }

                AddOrUpdateCache(productId, stockItem, storeAlias);

                return stockItem.Stock;
            }

        }

        /// <summary>
        /// Updates the stock and orderedCount of the product
        /// </summary>
        /// <param name="productId">the nodeId of the pricing this stock applies to</param>
        /// <param name="orderCountToUpdate">the ordercount to add to current oirdercount</param>
        /// <param name="storeAlias"> </param>
        public static int SetOrderCount(int productId, int orderCountToUpdate, string storeAlias = null)
        {
            storeAlias = storeAlias ?? string.Empty;

            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId);

            if (!string.IsNullOrEmpty(storeAlias))
            {
                sql.Where("StoreAlias = @0", storeAlias);
            }

            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var stockItem = db.FirstOrDefault<uWebshopStock>(sql);

                if (stockItem != null)
                {
                    stockItem.UpdateDateTime = DateTime.Now;
                    orderedCount = stockItem.OrderCount;

                    stockItem.OrderCount = orderedCount + orderCountToUpdate;

                    db.Update(stockItem);
                }
                else
                {
                    stockItem = new uWebshopStock
                    {
                        NodeId = currentNodeId,
                        UpdateDateTime = DateTime.Now,
                        OrderCount = orderedCount + orderCountToUpdate
                    };

                    db.Insert(stockItem);
                }

                AddOrUpdateCache(productId, stockItem, storeAlias);

                return stockItem.OrderCount;
            }

        }

        /// <summary>
        /// Updates the stock to a specific value, replaces the current stock with the given value  
        /// </summary>
        /// <param name="productId">the nodeId of the pricing this stock applies to</param>
        /// <param name="newStock">the stock value to be set (overwrites the current stock value, does not update it!)</param>
        /// <param name="updateOrderCount">Update orderCount; default = true</param>
        /// <param name="storeAlias"></param>
        public static bool ReplaceStock(int productId, int newStock, bool updateOrderCount, string storeAlias = null, Database db = null)
        {
            Database dbb = null;

            if (db != null)
            {
                dbb = db;
            }
            else {
                dbb = Database;
            }

            storeAlias = storeAlias ?? string.Empty;

            bool updated = false;
            // todo: Transactions?

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = productId;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId);

            if (!string.IsNullOrEmpty(storeAlias))
            {
                sql.Where("StoreAlias = @0", storeAlias);
            }

            var stockItem =
                dbb.FirstOrDefault<uWebshopStock>(sql);

            if (stockItem != null)
            {

                if (newStock != stockItem.Stock )
                {
                    stockItem.Stock = newStock;
                    stockItem.UpdateDateTime = DateTime.Now;
                    updated = true;
                }

                if (updateOrderCount)
                {
                    orderedCount = stockItem.OrderCount;
                    currentStock = stockItem.Stock;
                    
                    stockItem.OrderCount = orderedCount - (currentStock - newStock);
                    updated = true;
                }

                if (updated) {
                    dbb.Update(stockItem);
                }

            }
            else
            {
                stockItem = new uWebshopStock
                {
                    Stock = newStock,
                    NodeId = currentNodeId,
                    UpdateDateTime = DateTime.Now,
                    StoreAlias = storeAlias,
                    CreatDateTime = DateTime.Now
                };

                if (updateOrderCount)
                {
                    stockItem.OrderCount = orderedCount - (currentStock - newStock);
                }

                updated = true;
                dbb.Insert(stockItem);
            }

            if (updated) {
                AddOrUpdateCache(productId, stockItem, storeAlias);
                AfterReplaceStockChanged(new AfterReplaceStockChangedEventArgs {
                    productId = productId,
                    stockItem = stockItem,
                    storeAlias = storeAlias
                });
            }        

            return updated;
        }

        public static event AfterReplaceStockChangedEventHandler AfterReplaceStockChanged;

        public delegate void AfterReplaceStockChangedEventHandler(AfterReplaceStockChangedEventArgs e);

        public class AfterReplaceStockChangedEventArgs : EventArgs
        {
            public int productId { get; set; }
            public uWebshopStock stockItem { get; set; }
            public string storeAlias { get; set; }
        }

        public static event AfterSubtractStockChangedEventHandler AfterSubtractStockChanged;

        public delegate void AfterSubtractStockChangedEventHandler(AfterSubtractStockChangedEventArgs e);

        public class AfterSubtractStockChangedEventArgs : EventArgs
        {
            public int productId { get; set; }
            public uWebshopStock stockItem { get; set; }
            public string storeAlias { get; set; }
        }

    }
}