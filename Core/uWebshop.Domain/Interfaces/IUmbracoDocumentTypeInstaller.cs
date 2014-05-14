namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUmbracoDocumentTypeInstaller
	{
		/// <summary>
		/// Installs the store.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		void InstallStore(string storeAlias);

		/// <summary>
		/// Uninstalls the store.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		void UnInstallStore(string storeAlias);

	    /// <summary>
	    /// Creates the order document.
	    /// </summary>
	    /// <param name="order">The order.</param>
	    void CreateOrderDocument(OrderInfo order);

        /// <summary>
        /// Returns the Order Id, or creates ordernode when Id == 0
        /// </summary>
        /// <param name="order">The order.</param>
        int GetOrCreateOrderContent(OrderInfo order);
	}
}