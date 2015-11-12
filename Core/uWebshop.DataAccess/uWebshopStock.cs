using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using umbraco;
using umbraco.BusinessLogic;
using umbraco.DataLayer;
using uWebshop.DataAccess.Pocos;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;

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

        internal static UmbracoDatabase Database
        {
            get { return UmbracoContext.Current.Application.DatabaseContext.Database; }
        }

        private static IEnumerable<uWebshopStock> LoadAllStockInfo()
        {
            // todo: improve caching, use Dictionary<int, StockInfo>
            IEnumerable<uWebshopStock> stocks;
            if (HttpContext.Current.Items.Contains(AllStockCacheKey))
            {
                stocks = ((IEnumerable<uWebshopStock>) HttpContext.Current.Items[AllStockCacheKey]);
            }
            else
            {
                var sql = Sql.Builder.Select("NodeId, Stock, StoreAlias, OrderCount")
                    .From("uWebshopStock");

                stocks = Database.Query<uWebshopStock>(sql);

                HttpContext.Current.Items[AllStockCacheKey] = stocks;
            }

            return stocks;
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

            var globalFallback =
                stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == string.Empty);

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

            var globalFallback =
                stocks.FirstOrDefault(stock => stock.NodeId == nodeId && stock.StoreAlias == string.Empty);

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
        public static int SubstractStock(int productId, int stockToSubtract, bool updateOrderCount,
            string storeAlias = null)
        {
            storeAlias = storeAlias ?? string.Empty;

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId)
                .Where("StoreAlias @0", storeAlias);

            var stockItem =
                Database.SingleOrDefault<uWebshopStock>(sql);

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
                Database.Update(stockItem);
            }
            else
            {
                stockItem = new uWebshopStock
                {
                    // todo: should this be 1?
                    Stock = 1,
                    NodeId = currentNodeId,
                    UpdateDateTime = DateTime.Now
                };

                if (updateOrderCount)
                {
                    stockItem.OrderCount = orderedCount + stockToSubtract;
                }

                Database.Insert(stockItem);
            }

            return stockItem.Stock;
        }


        public static int ReturnStock(int productId, int stockToReturn, bool updateOrderCount, string storeAlias = null)
        {
            // todo: not thread safe (no transaction)

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId)
                .Where("StoreAlias @0", storeAlias);

            var stockItem =
                Database.SingleOrDefault<uWebshopStock>(sql);

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
                Database.Update(stockItem);
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

                Database.Insert(stockItem);
            }

            return stockItem.Stock;
        }

        /// <summary>
        /// Updates the stock and orderedCount of the product
        /// </summary>
        /// <param name="productId">the nodeId of the pricing this stock applies to</param>
        /// <param name="orderCountToUpdate">the ordercount to add to current oirdercount</param>
        /// <param name="storeAlias"> </param>
        public static int SetOrderCount(int productId, int orderCountToUpdate, string storeAlias = null)
        {
            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId)
                .Where("StoreAlias @0", storeAlias);

            var stockItem =
                Database.SingleOrDefault<uWebshopStock>(sql);

            if (stockItem != null)
            {
                stockItem.UpdateDateTime = DateTime.Now;
                orderedCount = stockItem.OrderCount;

                stockItem.OrderCount = orderedCount + orderCountToUpdate;

                Database.Update(stockItem);
            }
            else
            {
                stockItem = new uWebshopStock
                {
                    NodeId = currentNodeId,
                    UpdateDateTime = DateTime.Now,
                    OrderCount = orderedCount + orderCountToUpdate
                };

                Database.Insert(stockItem);
            }

            return stockItem.OrderCount;
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
            // todo: Transactions?

            var currentStock = 0;
            var orderedCount = 0;
            var currentNodeId = 0;

            var sql = Sql.Builder.Select("*")
                .From("uWebshopStock")
                .Where("NodeId = @0", productId)
                .Where("StoreAlias @0", storeAlias);

            var stockItem =
                Database.SingleOrDefault<uWebshopStock>(sql);

            if (stockItem != null)
            {
                stockItem.UpdateDateTime = DateTime.Now;
                currentStock = stockItem.Stock;
                orderedCount = stockItem.OrderCount;

                stockItem.Stock = newStock;
                if (updateOrderCount)
                {
                    stockItem.OrderCount = orderedCount - (currentStock - newStock);
                }
                Database.Update(stockItem);
            }
            else
            {
                stockItem = new uWebshopStock
                {
                    Stock = newStock,
                    NodeId = currentNodeId,
                    UpdateDateTime = DateTime.Now
                };

                if (updateOrderCount)
                {
                    stockItem.OrderCount = orderedCount - (currentStock - newStock);
                }

                Database.Insert(stockItem);
            }
        }

    }
}