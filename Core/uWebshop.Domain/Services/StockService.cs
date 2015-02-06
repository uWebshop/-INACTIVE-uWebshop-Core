using System;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class StockService : IStockService
	{
		public int GetStockForUwebshopEntityWithId(int id, string storeAlias = null)
		{
			return UWebshopStock.GetStock(id, storeAlias);
		}

		public int GetOrderCount(int id, string storeAlias = null)
		{
			return UWebshopStock.GetOrderCount(id, storeAlias);
		}

		[Obsolete("use SubstractStock")]
		public void SetStock(int itemId, int delta, bool updateOrderCount = true, string storeAlias = null)
		{
			UWebshopStock.SubstractStock(itemId, delta, updateOrderCount, storeAlias);
		}

		public void SubstractStock(int itemId, int delta, bool updateOrderCount = true, string storeAlias = null)
		{
			UWebshopStock.SubstractStock(itemId, delta, updateOrderCount, storeAlias);
		}

		public void SetOrderCount(int itemId, int orderCountToUpdate, string storeAlias = null)
		{
			UWebshopStock.SetOrderCount(itemId, orderCountToUpdate, storeAlias);
		}

		public void ReplaceStock(int productId, int newStock, bool updateOrderCount, string storeAlias = null)
		{
			UWebshopStock.ReplaceStock(productId, newStock, updateOrderCount, storeAlias);
		}

		public void ReturnStock(int itemId, int stockToReturn, bool updateOrderCount = true, string storeAlias = null)
		{
			UWebshopStock.ReturnStock(itemId, stockToReturn, updateOrderCount, storeAlias);
		}
	}
}