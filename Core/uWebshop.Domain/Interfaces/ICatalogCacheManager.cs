namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICatalogCacheManager
	{
		/// <summary>
		/// Loads the original reload product.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void LoadOrReloadProduct(int id);

		/// <summary>
		/// Loads the original reload product variant.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void LoadOrReloadProductVariant(int id);

		/// <summary>
		/// Loads the original reload category.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void LoadOrReloadCategory(int id);

		/// <summary>
		/// Unloads the product.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void UnloadProduct(int id);

		/// <summary>
		/// Unloads the product variant.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void UnloadProductVariant(int id);

		/// <summary>
		/// Unloads the category.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		void UnloadCategory(int id);
	}

	internal class CatalogCacheManager : ICatalogCacheManager
	{
		public void LoadOrReloadProduct(int id)
		{
			// if reload, reload properties into entity from cache
			//	if Categories property changed, update relevant Category.Products & Category.ProductsRecursively

			// if load, load entity
			//	add to relevant Category.Products
			//  add to relevant Category.ProductsRecursively
		}

		public void LoadOrReloadProductVariant(int id)
		{
			// if reload, reload properties into entity from cache

			// if load, load entity, add to cache
			//	add to relevant Product.Variants
			//  update Product.VariantGroups
		}

		public void LoadOrReloadCategory(int id)
		{
			// if reload, reload properties into entity from cache

			// if load, load entity, add to cache
			// add to relevant Product.Categories
			// update Product.Disabled, hmm load Products that might have been left out of the cache?
			// reset upstream Category.ProductsRecursively
			// add to relevant Category.Categories
		}

		public void UnloadProduct(int id)
		{
			// remove from product cache
			// remove from any Category.Products
			// remove from any Category.ProductsRecursively
		}

		public void UnloadProductVariant(int id)
		{
			// remove from variant cache
			// remove from Product.Variants
			// update Product.VariantGroups
		}

		public void UnloadCategory(int id)
		{
			// remove from category cache
			// remove from any Product.Categories
			// update Product.Disabled, remove Product from cache if Disabled?
			// reset upstream Category.ProductsRecursively
			// remove from any Category.Categories
		}
	}
}