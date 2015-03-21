using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public static class Catalog
	{
		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> GetAllCategories(string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias, currencyCode).Select(CategoryAdaptor.Create);
		}

		/// <summary>
		/// Gets all root categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> GetAllRootCategories(string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias, currencyCode).Where(x => x.ParentNodeTypeAlias == Domain.Catalog.CategoryRepositoryNodeAlias).Select(CategoryAdaptor.Create);
		}

		/// <summary>
		/// Gets all products.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> GetAllProducts(string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllProducts(false, storeAlias, currencyCode).Select(p => new ProductAdaptor(p));
		}

		/// <summary>
		/// Gets all product variants.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProductVariant> GetAllProductVariants(string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllProductVariants(false, storeAlias, currencyCode).Select(v => new VariantAdaptor(v));
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static ICategory GetCategory(int categoryId = 0, string storeAlias = null, string currencyCode = null)
		{
			if (categoryId == 0)
			{
				var currentCat = UwebshopRequest.Current.Category;
				if (storeAlias == null && currencyCode == null || currentCat == null) return (ICategory)currentCat;
				categoryId = currentCat.Id;
			}
			return CategoryAdaptor.Create(DomainHelper.GetCategoryById(categoryId, storeAlias, currencyCode));
		}

		public static IEnumerable<ICategory> GetCurrentCategoryPath()
		{
			return UwebshopRequest.Current.CategoryPath.Select(c => CategoryAdaptor.Create(DomainHelper.GetCategoryById(c.Id)));
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">category</exception>
		public static ICategory GetCategory(ICategory category, string storeAlias = null, string currencyCode = null)
		{
			if (category == null) throw new ArgumentNullException("category");
			return GetCategory(category.Id, storeAlias, currencyCode);
		}

		/// <summary>
		/// Gets the categories recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> GetCategoriesRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var category = DomainHelper.GetCategoryById(categoryId, storeAlias, currencyCode);
			var categoryList = new List<ICategory>();
			GetCategoriesFromCategory(categoryList, category);
			return categoryList;
		}

		/// <summary>
		/// Gets the categories recursive.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">category</exception>
		public static IEnumerable<ICategory> GetCategoriesRecursive(ICategory category, string storeAlias = null, string currencyCode = null)
		{
			if (category == null) throw new ArgumentNullException("category");
			return GetCategoriesRecursive(category.Id, storeAlias, currencyCode);
		}

		private static void GetCategoriesFromCategory(ICollection<ICategory> categoryList, ICategory mainCategory)
		{
			foreach (var category in mainCategory.SubCategories.Where(category => categoryList.All(x => x.Id != category.Id)))
			{
				categoryList.Add(CategoryAdaptor.Create(category));
			}

			if (!mainCategory.SubCategories.Any()) return;
			foreach (var subCategory in mainCategory.SubCategories)
			{
				GetCategoriesFromCategory(categoryList, subCategory); // todo: this can loop
			}
		}

		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IProduct GetProduct(int productId = 0, string storeAlias = null, string currencyCode = null)
		{
			if (productId == 0)
			{
				var product = UwebshopRequest.Current.Product;
				if (storeAlias == null && currencyCode == null || product == null) return (IProduct)product;
				productId = product.Id;
			}
			var productById = DomainHelper.GetProductById(productId, storeAlias, currencyCode);
			if (productById == null) return null;
			return new ProductAdaptor(productById);
		}

		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">product</exception>
		public static IProduct GetProduct(IProduct product, string storeAlias = null, string currencyCode = null)
		{
			if (product == null) throw new ArgumentNullException("product");
			return GetProduct(product.Id, storeAlias, currencyCode);
		}

		/// <summary>
		/// Gets the products recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> GetProductsRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var category = DomainHelper.GetCategoryById(categoryId, storeAlias, currencyCode);
			return category.ProductsRecursive.Select(p => new ProductAdaptor(p));
		}

		/// <summary>
		/// Gets the products recursive.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">category</exception>
		public static IEnumerable<IProduct> GetProductsRecursive(ICategory category, string storeAlias = null, string currencyCode = null)
		{
			if (category == null) throw new ArgumentNullException("category");
			return GetProductsRecursive(category.Id, storeAlias, currencyCode);
		}

		/// <summary>
		/// Gets the product variant.
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IProductVariant GetProductVariant(int variantId, string storeAlias = null, string currencyCode = null)
		{
			return new VariantAdaptor(DomainHelper.GetProductVariantById(variantId, storeAlias, currencyCode));
		}

		/// <summary>
		/// Gets the product variant.
		/// </summary>
		/// <param name="variant">The variant.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">variant</exception>
		public static IProductVariant GetProductVariant(IProductVariant variant, string storeAlias = null, string currencyCode = null)
		{
			if (variant == null) throw new ArgumentNullException("variant");
			return GetProductVariant(variant.Id, storeAlias, currencyCode);
		}

		/// <summary>
		/// Gets the products variants recursive.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProductVariant> GetProductsVariantsRecursive(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var category = DomainHelper.GetCategoryById(categoryId, storeAlias, currencyCode);
			return category.ProductsRecursive.SelectMany(p => p.GetAllVariants()).Select(v => new VariantAdaptor(v));
		}

		/// <summary>
		/// Gets the products variants recursive.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">category</exception>
		public static IEnumerable<IProductVariant> GetProductsVariantsRecursive(ICategory category, string storeAlias = null, string currencyCode = null)
		{
			if (category == null) throw new ArgumentNullException("category");
			return GetProductsVariantsRecursive(category.Id, storeAlias, currencyCode);
		}


		/// <summary>
		/// Returns unique categories witch have one ore more tags in common with the given categoryId
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public static IEnumerable<ICategory> MatchingTagCategories(int categoryId, string storeAlias = null, string currencyCode = null)
		{
			var currentCategory = DomainHelper.GetCategoryById(categoryId, storeAlias, currencyCode);

			return MatchingTagCategories(currentCategory);
		}

		/// <summary>
		/// Returns unique categories witch have one ore more tags in common with the given ICategory
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public static IEnumerable<ICategory> MatchingTagCategories(ICategory category, string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias, currencyCode).Where(x => x.Tags.Intersect(category.Tags).Any()).ToList();
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public static IEnumerable<IProduct> MatchingTagProducts(int productId, string storeAlias = null, string currencyCode = null)
		{
			var currentProduct = DomainHelper.GetProductById(productId, storeAlias, currencyCode);

			return MatchingTagProducts(currentProduct);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given IProduct
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public static IEnumerable<IProduct> MatchingTagProducts(IProduct product, string storeAlias = null, string currencyCode = null)
		{
			return DomainHelper.GetAllProducts(false, storeAlias, currencyCode).Where(x => x.Tags.Intersect(product.Tags).Any()).ToList();
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <returns></returns>
		public static string NiceUrl(int id)
		{
			return StoreHelper.GetNiceUrl(id);
		}

		/// <summary>
		/// Creates the Url for a Catalog item by Id
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId)
		{
			return StoreHelper.GetNiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id 
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeId">the id of the store you want to link to</param>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId, int storeId)
		{
			var store = StoreHelper.GetById(storeId);
			return store != null ? StoreHelper.GetNiceUrl(id, categoryId, store.Alias) : StoreHelper.GetNiceUrl(id, categoryId);
		}

		/// <summary>
		/// Creates the Url for a catalog item by Id 
		/// </summary>
		/// <param name="id">catalog item Id</param>
		/// <param name="categoryId">the Id of the category used to build the url. input 0 will use currentCategory.</param>
		/// <param name="storeAlias">the id of the store you want to link to</param>
		/// If the categoryId is not in the categories list of the product, it will use the first category
		/// <returns></returns>
		public static string NiceUrl(int id, int categoryId, string storeAlias)
		{
			return StoreHelper.GetNiceUrl(id, categoryId, storeAlias);
		}

		/// <summary>
		/// Generate paging based on the itemcount and the items per page
		/// </summary>
		/// <param name="itemCount">The item count.</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>
		/// Paging Object
		/// </returns>
		public static Paging GetPages(int itemCount, int itemsPerPage)
		{
			return Paging.GetPages(itemCount, itemsPerPage);
		}
	}
}
