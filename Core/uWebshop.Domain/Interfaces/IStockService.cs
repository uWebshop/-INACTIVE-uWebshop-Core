using System;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IStockService
	{
		/// <summary>
		/// Gets the stock for uwebshop entity with unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		int GetStockForUwebshopEntityWithId(int id, string storeAlias = null);

        [Obsolete("Use SubstractStock")]
        void SetStock(int productId, int delta, bool updateOrderCount = true, string storeAlias = null);

        /// <summary>
        /// Sets the stock.
        /// </summary>
        /// <param name="productId">The product unique identifier.</param>
        /// <param name="delta">The delta.</param>
        /// <param name="updateOrderCount">if set to <c>true</c> [update order count].</param>
        /// <param name="storeAlias">The store alias.</param>
        void SubstractStock(int productId, int delta, bool updateOrderCount = true, string storeAlias = null);

		/// <summary>
		/// Sets the order count.
		/// </summary>
		/// <param name="itemId">The item unique identifier.</param>
		/// <param name="orderCountToUpdate">The order count automatic update.</param>
		/// <param name="storeAlias">The store alias.</param>
		void SetOrderCount(int itemId, int orderCountToUpdate, string storeAlias = null);
        
        /// <summary>
        /// Replaces the current stock with the given value
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="newStock"></param>
        /// <param name="updateOrderCount"></param>
        /// <param name="storeAlias"></param>
	    void ReplaceStock(int productId, int newStock, bool updateOrderCount, string storeAlias = null);

		/// <summary>
		/// Returns the stock.
		/// </summary>
		/// <param name="itemId">The item unique identifier.</param>
		/// <param name="stockToReturn">The stock automatic return.</param>
		/// <param name="updateOrderCount">if set to <c>true</c> [update order count].</param>
		/// <param name="storeAlias">The store alias.</param>
		void ReturnStock(int itemId, int stockToReturn, bool updateOrderCount = true, string storeAlias = null);

		/// <summary>
		/// Returns the ordercount.
		/// </summary>
		int GetOrderCount(int id, string storeAlias = null);
	}
}