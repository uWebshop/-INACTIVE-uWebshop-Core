using System.Collections.Generic;
using uWebshop.Common.Interfaces;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	internal interface IProductService : IEntityService<Product>
	{
		/// <summary>
		/// Gets the stock for product.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <returns></returns>
		int GetStockForProduct(int productId);

		/// <summary>
		/// Creates the product information by product unique identifier.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="order">The order.</param>
		/// <param name="localization">The localization.</param>
		/// <param name="itemCount">The item count.</param>
		/// <returns></returns>
		ProductInfo CreateProductInfoByProductId(int productId, OrderInfo order, ILocalization localization, int itemCount);

		/// <summary>
		/// Gets all enabled and with category.
		/// </summary>
		/// <param name="localization">The localization.</param>
		/// <returns></returns>
		List<Product> GetAllEnabledAndWithCategory(ILocalization localization);

		/// <summary>
		/// Reloads the with vat setting.
		/// </summary>
		void ReloadWithVATSetting();

		IProduct Localize(IProduct product, ILocalization localization);
	}
}