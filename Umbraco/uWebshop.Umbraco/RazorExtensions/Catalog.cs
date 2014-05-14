using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco;
using uWebshop.Umbraco.Businesslogic;
using Helpers = uWebshop.Umbraco.Helpers;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;
using Store = uWebshop.Domain.Store;

namespace uWebshop.RazorExtensions
{
	/// <summary>
	/// 
	/// </summary>
	public class Catalog
	{
		/// <summary>
		/// Return the property value for the current shopalias
		/// </summary>
		/// <param name="nodeId">The node unique identifier.</param>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns></returns>
		public static string GetPropertyValueForCurrentShop(int nodeId, string propertyAlias)
		{
			var store = StoreHelper.GetCurrentStore();

			propertyAlias = StoreHelper.CreateMultiStorePropertyAlias(propertyAlias, store.Alias);

			var node = new umbraco.NodeFactory.Node(nodeId);

			var property = node.GetProperty(propertyAlias);

			return property == null ? string.Empty : property.Value;
		}

		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use AllCategories")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Category> GetAllCategories(string storeAlias = null)
		{
			return AllCategories(storeAlias).Cast<Category>().ToList();
		}

		/// <summary>
		/// Gets all categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> AllCategories(string storeAlias = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias).ToList();
		}

		/// <summary>
		/// Gets all root categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use AllRootCategories")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Category> GetAllRootCategories(string storeAlias = null)
		{
			return Domain.Catalog.GetAllRootCategories(storeAlias);
		}

		/// <summary>
		/// Gets all root categories.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> AllRootCategories(string storeAlias = null)
		{
			return Domain.Catalog.GetAllRootCategories(storeAlias);
		}

		/// <summary>
		/// Returns all the payment zones
		/// </summary>
		/// <returns></returns>
		public static List<Zone> GetAllPaymentZones()
		{
			return IO.Container.Resolve<IZoneService>().GetAllPaymentZones(StoreHelper.CurrentLocalization);
		}

		/// <summary>
		/// Returns all the shipping zones
		/// </summary>
		/// <returns></returns>
		public static List<Zone> GetAllShippingZones()
		{
			return IO.Container.Resolve<IZoneService>().GetAllShippingZones(StoreHelper.CurrentLocalization);
		}

		/// <summary>
		/// Get all the countires from country.xml or country_storealias.xml
		/// If not found a fallback list will be used
		/// </summary>
		/// <returns></returns>
		public static List<Country> GetAllCountries()
		{
			return StoreHelper.GetAllCountries(StoreHelper.CurrentStoreAlias);
		}

		/// <summary>
		/// Returns the full country name from the given countrycode
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		/// <returns></returns>
		public static string CountryNameFromCode(string countryCode)
		{
			var country = StoreHelper.GetAllCountries().FirstOrDefault(x => x.Code == countryCode);
			return country != null ? country.Name : string.Empty;
		}

		/// <summary>
		/// Returns all the products with prices for the given store or current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		[Obsolete("Use AllProducts")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Product> GetAllProducts(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).ToList();
		}

		/// <summary>
		/// Returns all the products with prices for the given store or current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> AllProducts(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		/// <summary>
		/// Get a list of all stores
		/// </summary>
		/// <returns></returns>
		public static List<Store> GetAllStores()
		{
			return StoreHelper.GetAllStores().ToList();
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetAllVariants()")]
		public static List<ProductVariant> GetAllPricingVariants()
		{
			return GetAllVariants();
		}

		/// <summary>
		/// Returns all the pricingvariants for the given store or current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static List<ProductVariant> GetAllVariants(string storeAlias = null)
		{
			return DomainHelper.GetAllProductVariants(false, storeAlias).Where(x => x.Product.Categories.Any()).ToList();
		}


		/// <summary>
		/// Returns a category item based on the URLname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <returns></returns>
		[Obsolete("Use CategoryFromUrlName")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Category GetCategoryFromUrlName(string categoryUrlName)
		{
			return !string.IsNullOrEmpty(categoryUrlName) ? IO.Container.Resolve<ICatalogUrlResolvingService>().GetCategoryFromUrlName(categoryUrlName) as Category : null;
		}

		/// <summary>
		/// Returns a category item based on the URLname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <returns></returns>
		public static ICategory CategoryFromUrlName(string categoryUrlName)
		{
			return !string.IsNullOrEmpty(categoryUrlName) ? IO.Container.Resolve<ICatalogUrlResolvingService>().GetCategoryFromUrlName(categoryUrlName) as ICategory : null;
		}

		/// <summary>
		/// Returns a product item based on the URLname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <param name="productUrlName">Name of the product URL.</param>
		/// <returns></returns>
		[Obsolete("Use ProductFromUrlName")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Product GetProductFromUrlName(string categoryUrlName, string productUrlName)
		{
			if (!string.IsNullOrEmpty(productUrlName) && !string.IsNullOrEmpty(categoryUrlName))
			{
				return IO.Container.Resolve<ICatalogUrlResolvingService>().GetProductFromUrlName(categoryUrlName, productUrlName) as Product;
			}
			return null;
		}

		/// <summary>
		/// Returns a product item based on the URLname
		/// </summary>
		/// <param name="categoryUrlName">Name of the category URL.</param>
		/// <param name="productUrlName">Name of the product URL.</param>
		/// <returns></returns>
		public static IProduct ProductFromUrlName(string categoryUrlName, string productUrlName)
		{
			if (!string.IsNullOrEmpty(productUrlName) && !string.IsNullOrEmpty(categoryUrlName))
			{
				return IO.Container.Resolve<ICatalogUrlResolvingService>().GetProductFromUrlName(categoryUrlName, productUrlName) as IProduct;
			}
			return null;
		}


		/// <summary>
		/// Get image by Image Id
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public static UwbsImage GetImageById(int id)
		{
			return Helpers.GetImageById(id);
		}

		/// <summary>
		/// Get Media values by Id
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <returns></returns>
		public static MediaValues GetUmbracoMedia(int id)
		{
			return Helpers.GetUmbracoMedia(id);
		}

		/// <summary>
		/// Returns the current catagory
		/// </summary>
		/// <param name="ignoreDisabled">if set to <c>true</c> ignore disabled.</param>
		/// <returns></returns>
		[Obsolete("Use Category")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Category GetCategory(bool ignoreDisabled = false)
		{
			var category = (Category) UwebshopRequest.Current.Category;

			if (category == null || (!ignoreDisabled && category.Disabled)) return null;
			return category;
		}

		/// <summary>
		/// Returns the current catagory
		/// </summary>
		/// <param name="ignoreDisabled">if set to <c>true</c> ignore disabled.</param>
		/// <returns></returns>
		public static ICategory Category(bool ignoreDisabled = false)
		{
			var category = UwebshopRequest.Current.Category as ICategory;

			if (category == null || (!ignoreDisabled && category.Disabled)) return null;
			return category;
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use Category")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Category GetCategory(int categoryId, string storeAlias = null)
		{
			var category = DomainHelper.GetCategoryById(categoryId, storeAlias);

			return category == null ? null : (category.Disabled ? null : category);
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static ICategory Category(int categoryId, string storeAlias = null)
		{
			var category = DomainHelper.GetCategoryById(categoryId, storeAlias);

			return category == null ? null : (category.Disabled ? null : category);
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use Category")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Category GetCategory(string categoryId, string storeAlias = null)
		{
			var parsedId = int.Parse(categoryId);
			return GetCategory(parsedId, storeAlias);
		}

		/// <summary>
		/// Gets the category.
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static ICategory Category(string categoryId, string storeAlias = null)
		{
			var parsedId = int.Parse(categoryId);
			return Category(parsedId, storeAlias);
		}

		/// <summary>
		/// Returns the current product
		/// </summary>
		/// <param name="ignoreDisabled">if set to <c>true</c> ignore disabled.</param>
		/// <returns></returns>
		[Obsolete("Use Product")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Product GetProduct(bool ignoreDisabled = false)
		{
			return Product(ignoreDisabled) as Product;
		}

		/// <summary>
		/// Returns the current product
		/// </summary>
		/// <param name="ignoreDisabled">if set to <c>true</c> ignore disabled.</param>
		/// <returns></returns>
		public static IProduct Product(bool ignoreDisabled = false)
		{
			var product = UwebshopRequest.Current.Product as IProduct;

			if (product == null || (!ignoreDisabled && product.Disabled)) return null;
			return product;
		}

		/// <summary>
		/// Returns the product information for the given productId, with prices for the given store or current store
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use Product")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Product GetProduct(int productId, string storeAlias = null)
		{
			var product = DomainHelper.GetProductById(productId, storeAlias);
			return product != null && product.Disabled ? null : product as Product;
		}

		/// <summary>
		/// Returns the product information for the given productId, with prices for the given store or current store
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IProduct Product(int productId, string storeAlias = null)
		{
			var product = DomainHelper.GetProductById(productId, storeAlias);
			return product != null && product.Disabled ? null : product;
		}

		/// <summary>
		/// Returns the product information for the given productId, with prices for the given storeAlias
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use Product")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Product GetProduct(string productId, string storeAlias = null)
		{
			var parsedProductId = int.Parse(productId);
			return GetProduct(parsedProductId);
		}

		/// <summary>
		/// Returns the product information for the given productId, with prices for the given storeAlias
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IProduct Product(string productId, string storeAlias = null)
		{
			var parsedProductId = int.Parse(productId);
			return Product(parsedProductId);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use MatchingTagProducts")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Product> GetMatchingTagProducts(int productId, string storeAlias = null)
		{
			var currentProduct = DomainHelper.GetProductById(productId, storeAlias);

			return GetMatchingTagProducts(currentProduct);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> MatchingTagProducts(int productId, string storeAlias = null)
		{
			var currentProduct = DomainHelper.GetProductById(productId, storeAlias);

			return MatchingTagProducts(currentProduct);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use MatchingTagCategories")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Category> GetMatchingTagCategories(int categoryId, string storeAlias = null)
		{
			var currentCategory = DomainHelper.GetCategoryById(categoryId, storeAlias);

			return GetMatchingTagCategories(currentCategory);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> MatchingTagCategories(int categoryId, string storeAlias = null)
		{
			var currentCategory = DomainHelper.GetCategoryById(categoryId, storeAlias);

			return MatchingTagCategories(currentCategory);
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use MatchingTagProducts")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Product> GetMatchingTagProducts(IProduct product, string storeAlias = null)
		{
			return DomainHelper.GetAllProducts(false, storeAlias).Where(x => x.Tags.Intersect(product.Tags).Any()).Cast<Product>().ToList();
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> MatchingTagProducts(IProduct product, string storeAlias = null)
		{
			return DomainHelper.GetAllProducts(false, storeAlias).Where(x => x.Tags.Intersect(product.Tags).Any()).ToList();
		}

		/// <summary>
		/// Returns unique products witch have one ore more tags in common with the given productId
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use MatchingTagCategories")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Category> GetMatchingTagCategories(Category category, string storeAlias = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias).Where(x => x.Tags.Intersect(category.Tags).Any()).Cast<Category>().ToList();
		}

		/// <summary>
		/// Returns unique categories witch have one ore more tags in common with the given category
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<ICategory> MatchingTagCategories(Category category, string storeAlias = null)
		{
			return DomainHelper.GetAllCategories(false, storeAlias).Where(x => x.Tags.Intersect(category.Tags).Any()).ToList();
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("use GetVariant()")]
		public static ProductVariant GetPricingVariant(int variantId)
		{
			return GetVariant(variantId);
		}

		/// <summary>
		/// Returns the pricingVariant information for the given pricingVariantId
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static ProductVariant GetVariant(int variantId, string storeAlias = null)
		{
			var pricingVariant = DomainHelper.GetProductVariantById(variantId, storeAlias);
			return pricingVariant.Disabled == false ? pricingVariant : null;
		}

		/// <summary>
		/// Returns the pricingVariant information for the given pricingVariantId
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static ProductVariant GetVariant(string variantId, string storeAlias = null)
		{
			var parsedId = int.Parse(variantId);
			var pricingVariant = DomainHelper.GetProductVariantById(parsedId, storeAlias);
			return pricingVariant.Disabled == false ? pricingVariant : null;
		}

		/// <summary>
		/// Returns all the products in this category, including any sublevel
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Obsolete("Use ProductsRecursive")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Product> GetProductsRecursive(int categoryId, string storeAlias = null)
		{
			return DomainHelper.GetCategoryById(categoryId, storeAlias).ProductsRecursive.Cast<Product>().ToList();
		}

		/// <summary>
		/// Returns all the products in this category, including any sublevel
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IProduct> ProductsRecursive(int categoryId, string storeAlias = null)
		{
			return DomainHelper.GetCategoryById(categoryId, storeAlias).ProductsRecursive.ToList();
		}

		/// <summary>
		/// Returns all the categories in this category, including any sublevel
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns>
		/// All subcategories, independent of level of the given categoryId
		/// </returns>
		[Obsolete("Use CategoriesRecursive")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static List<Category> GetCategoriesRecursive(int categoryId, string storeAlias = null)
		{
			return StoreHelper.GetCategoriesRecursive(categoryId, storeAlias);
		}

		/// <summary>
		/// Returns all the categories in this category, including any sublevel
		/// </summary>
		/// <param name="categoryId">The category unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns>
		/// All subcategories, independent of level of the given categoryId
		/// </returns>
		public static IEnumerable<ICategory> CategoriesRecursive(int categoryId, string storeAlias = null)
		{
			return StoreHelper.GetCategoriesRecursive(categoryId, storeAlias);
		}

		/// <summary>
		/// returns a list of payment providers
		/// </summary>
		/// <returns>
		/// All list of all the Payment Providers in uWebshop
		/// </returns>
		public static List<PaymentProvider> GetAllPaymentProviders()
		{
			return PaymentProviderHelper.GetAllPaymentProviders().ToList();
		}

		/// <summary>
		/// Return a list of shipping providers
		/// </summary>
		/// <returns>
		/// All list of all the Shipping Providers in uWebshop
		/// </returns>
		public static List<ShippingProvider> GetAllShippingProviders()
		{
			return ShippingProviderHelper.GetAllShippingProviders().ToList();
		}

		/// <summary>
		/// Returns all the product discounts
		/// </summary>
		/// <returns>
		/// A list of all the Product discounts in uWebshop
		/// </returns>
		public static List<DiscountProduct> GetAllProductDiscounts()
		{
			return IO.Container.Resolve<IProductDiscountService>().GetAll(StoreHelper.CurrentLocalization).ToList();
		}

		/// <summary>
		/// Returns all the order discounts
		/// </summary>
		/// <returns>
		/// A list of all the order discounts in uWebshop
		/// </returns>
		public static List<IOrderDiscount> GetAllOrderDiscounts()
		{
			return IO.Container.Resolve<IOrderDiscountService>().GetAll(StoreHelper.CurrentLocalization).ToList();
		}

		/// <summary>
		/// Gets all order discounts using coupon code from.
		/// </summary>
		/// <param name="couponCodes">The coupon codes.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IEnumerable<DiscountOrder> GetAllOrderDiscountsUsingCouponCodeFrom(IEnumerable<string> couponCodes)
		{
			couponCodes = couponCodes.ToList();
			var coupons = IO.Container.Resolve<ICouponCodeService>().GetAllWithCouponcodes(couponCodes).ToList();
			return IO.Container.Resolve<IOrderDiscountService>().GetAll(StoreHelper.CurrentLocalization).Where(x => x.Disabled == false && 
				(couponCodes.Contains(x.CouponCode) || coupons.Any(c => c.NumberAvailable > 0 && c.DiscountId == x.Id))).Cast<DiscountOrder>().ToList();
		}

		/// <summary>
		/// Gets all order discounts using coupon code from.
		/// </summary>
		/// <param name="couponCode">The coupon code.</param>
		/// <returns></returns>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IEnumerable<DiscountOrder> GetDiscountsUsingCouponCode(string couponCode)
		{
			var coupons = IO.Container.Resolve<ICouponCodeService>().GetAllWithCouponcode(couponCode).ToList();
			return IO.Container.Resolve<IOrderDiscountService>().GetAll(StoreHelper.CurrentLocalization).Where(x => x.Disabled == false && 
				(x.CouponCode == couponCode || coupons.Any(c => c.NumberAvailable > 0 && c.DiscountId == x.Id)
				)).Cast<DiscountOrder>().ToList();
		}

		/// <summary>
		/// </summary>
		/// <returns>
		/// The current store
		/// </returns>
		public static Store GetCurrentStore()
		{
			return StoreHelper.GetCurrentStore();
		}

		/// <summary>
		/// Gets the current currency code.
		/// </summary>
		/// <returns>The current currency code</returns>
		public static string CurrentCurrencyCode()
		{
			return StoreHelper.CurrentLocalization.CurrencyCode;
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