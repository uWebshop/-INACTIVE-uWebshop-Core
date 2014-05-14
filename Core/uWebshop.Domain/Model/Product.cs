using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uWebshop.API;
using uWebshop.Common.Interfaces.Shared;
using uWebshop.DataAccess;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "", IsReference = true)]
    [ContentType(ParentContentType = typeof(Catalog), Name = "Product", Description = "#ProductDescription", Alias = "uwbsProduct", IconClass = IconClass.box, Icon = ContentIcon.BoxLabel, Thumbnail = ContentThumbnail.Folder, AllowedChildTypes = new[] { typeof(ProductVariantGroup) })]
	public class Product : MultiStoreUwebshopContent, IProductLegacy230, Common.Interfaces.IProduct
	{
		internal ILocalization Localization;
        internal Func<List<IProductVariantGroup>> ProductVariantGroupsFactory;

		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// Determines whether the specified node type alias is this alias.
		/// </summary>
		/// <param name="nodeTypeAlias">The node type alias.</param>
		public static bool IsAlias(string nodeTypeAlias)
		{
			return nodeTypeAlias != null && !nodeTypeAlias.StartsWith(Catalog.ProductRepositoryNodeAlias) && !ProductVariant.IsAlias(nodeTypeAlias) && nodeTypeAlias.StartsWith(NodeAlias);
		}

		/// <summary>
		/// The stock alias
		/// </summary>
		public const string StockAlias = "stock";

		private List<API.ICategory> _categories;
		internal List<int> CategoryIds;

		#region Simple properties

		/// <summary>
		/// Gets a value indicating whether prices are including vat.
		/// </summary>
		/// <value>
		/// <c>true</c> if prices are including vat; otherwise, <c>false</c>.
		/// </value>
		public bool PricesIncludingVat { get; internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether this product has extra categories set.
		/// </summary>
		/// <value>
		/// <c>true</c> if this product has extra categories set; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool HasCategories { get; set; }

		#region global tab

		/// <summary>
		/// Product Number
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "sku", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#SKU", Description = "#SKUDescription", Mandatory = false, SortOrder = 3)]
		public string SKU { get; set; }

		/// <summary>
		/// Gets the tags of the product
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "metaTags", DataType = DataType.Tags, Tab = ContentTypeTab.Global, Name = "#Tags", Description = "#TagsDescription", Mandatory = false, SortOrder = 5)]
		public string[] Tags { get; set; }

		/// <summary>
		/// Gets the category where the product is part of
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		[ContentPropertyType(Alias = "categories", DataType = DataType.MultiContentPickerCategories, Tab = ContentTypeTab.Global, Name = "#ProductCategories", Description = "#ProductCategoriesDescription", Mandatory = false, SortOrder = 7)]
		public IEnumerable<ICategory> Categories
		{
			get { return _categories ?? (_categories = CategoryIds.Select(i => IO.Container.Resolve<ICategoryService>().GetById(i, Localization)).Where(x => x != null).Cast<ICategory>().ToList()); }
			set { _categories = value.ToList(); }
		}

		#endregion

		#region details tab

		/// <summary>
		///     List of images of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "images", DataType = DataType.MultiContentPickerImages, Tab = ContentTypeTab.Details, Name = "#Images", Description = "#ImagesDescription", Mandatory = false, SortOrder = 8)]
		public IEnumerable<Image> Images
		{
			get { return _images ?? (_images = ImageIds.Select(ImageFactory).Where(i => i != null).ToArray()); }
			set { }
		}
		internal static Func<int, Image> ImageFactory;
		private Image[] _images;
		internal int[] ImageIds;

		/// <summary>
		///     List of files of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "files", DataType = DataType.MultiContentPickerFiles, Tab = ContentTypeTab.Details, Name = "#Files", Description = "#FilesDescription", Mandatory = false, SortOrder = 9)]
		public IEnumerable<File> Files
		{
			get { return _files ?? (_files = FileIds.Select(FileFactory).Where(i => i != null).ToArray()); }
			set { }
		}
		internal static Func<int, File> FileFactory;
		private File[] _files;
		internal int[] FileIds;

		/// <summary>
		///     Length of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "length", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Length", Description = "#LengthDescription", Mandatory = false, SortOrder = 10)]
		public double Length { get; set; }

		/// <summary>
		///     Width of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "width", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Width", Description = "#WidthDescription", Mandatory = false, SortOrder = 11)]
		public double Width { get; set; }

		/// <summary>
		///     Height of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "height", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Height", Description = "#HeightDescription", Mandatory = false, SortOrder = 12)]
		public double Height { get; set; }

		/// <summary>
		///     Weight of the product
		/// </summary>
		[DataMember]
		[ContentPropertyType(Alias = "weight", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Weight", Description = "#WeightDescription", Mandatory = false, SortOrder = 13)]
		public double Weight { get; set; }

		#endregion

		#region price tab

		/// <summary>
		/// Gets or sets the original price in cents.
		/// </summary>
		/// <value>
		/// The original price in cents.
		/// </value>
		[ContentPropertyType(Alias = "price", DataType = DataType.Price, Tab = ContentTypeTab.Price, Name = "#Price", Description = "#PriceDescription", Mandatory = false, SortOrder = 14)]
		public int OriginalPriceInCents { get; set; }

		/// <summary>
		/// Ranges from the range data type
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "ranges", DataType = DataType.Ranges, Tab = ContentTypeTab.Price, Name = "#Ranges", Description = "#RangesDescription", Mandatory = false, SortOrder = 15)]
		public IEnumerable<Range> Ranges { get; set; }

		/// <summary>
		/// The taxpercentage of the pricing
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "vat", DataType = DataType.VatPicker, Tab = ContentTypeTab.Price, Name = "#VAT", Description = "#VatDescription", Mandatory = false, SortOrder = 16)]
		public decimal Vat { get; set; }

		/// <summary>
		/// Stock count
		/// </summary>
		/// <value>
		/// The stock.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "stock", DataType = DataType.Stock, Tab = ContentTypeTab.Price, Name = "#Stock", Description = "#StockDescription", Mandatory = false, SortOrder = 17)]
		public int Stock
		{
			// todo: check multistore?!?!?!
			// caching here is impossible (breaks with app wide static caching of products), per-request caching is done some levels deeper
			get { return IO.Container.Resolve<IProductService>().GetStockForProduct(Id); }
			set { }
		}

		/// <summary>
		/// Number of times this item was ordered
		/// </summary>
		/// <value>
		/// The total items ordered.
		/// </value>
		[DataMember]
		public int TotalItemsOrdered { get; set; }

		/// <summary>
		/// Gets the number of times this Product is ordered
		/// </summary>
		/// <value>
		/// The order count.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "ordered", DataType = DataType.OrderedCount, Tab = ContentTypeTab.Price, Name = "#Ordered", Description = "#OrderedDescription", Mandatory = false, SortOrder = 18)]
		public int OrderCount
		{
			get { return UWebshopStock.GetOrderCount(Id, Localization.StoreAlias); }
			set { }
		}

		/// <summary>
		/// The status of the stock
		/// True = enabled
		/// False = disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [stock status]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "stockStatus", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Price, Name = "#StockStatus", Description = "#StockStatusDescription", Mandatory = false, SortOrder = 19)]
		public bool StockStatus { get; set; }

		/// <summary>
		/// The status of backorder
		/// True = enabled
		/// False = disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [backorder status]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "backorderStatus", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Price, Name = "#BackorderStatus", Description = "#BackorderStatusDescription", Mandatory = false, SortOrder = 20)]
		public bool BackorderStatus { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [use variant stock].
		/// </summary>
		/// <value>
		///   <c>true</c> if [use variant stock]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "useVariantStock", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Price, Name = "#UseVariantStock", Description = "#UseVariantStockDescription", Mandatory = false, SortOrder = 21)]
		public bool UseVariantStock { get; set; }

		#endregion

		#endregion

		#region Relations

        //private List<ProductVariantGroup> _productVariantGroups;

        /// <summary>
        /// Gets or sets the variants.
        /// </summary>
        /// <value>
        /// The variants.
        /// </value>
        /// 
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use GetAllVariants")]
        public IEnumerable<ProductVariant> Variants { get; set; }

        /// <summary>
        /// Gets all variants.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IProductVariant> GetAllVariants()
        {
            return VariantGroups.SelectMany(variantgroup => variantgroup.Variants).ToList();
        }

	    public IProductDiscount Discount
		{
			get { return ProductDiscount; }
		}

		/// <summary>
		/// Gets the sale of the pricing
		/// </summary>
		/// <value>
		/// The product discount.
		/// </value>
		[DataMember]
		public DiscountProduct ProductDiscount
		{
			get { return IO.Container.Resolve<IProductDiscountService>().GetDiscountByProductId(Id, Localization); }
			set { }
		}

        ///// <summary>
        ///// The groups with variants for this product
        ///// </summary>
        ///// <value>
        ///// The product variant groups.
        ///// </value>
        //[DataMember]
        //public IEnumerable<ProductVariantGroup> ProductVariantGroups
        //{
        //    get
        //    {
        //        var counter = 0;
        //        return _productVariantGroups ?? (_productVariantGroups = Variants.GroupBy(v => v.Group).Select(g => new ProductVariantGroup(g.Key, g.ToList(), counter++)).ToList());
        //    }
        //}
		
        /// <summary>
        ///     Gets a list of products which belong to the category
        /// </summary>
        [DataMember]
        public IEnumerable<IProductVariantGroup> VariantGroups
        {
            get { return (_productVariantGroups ?? (_productVariantGroups = ProductVariantGroupsFactory())); } //DomainHelper.GetProducts(Id).ToList()); }
            set { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use VariantGroups")]
        public IEnumerable<ProductVariantGroup> ProductVariantGroups {
            // todo!
            get
            {
                return
                    IO.Container.Resolve<IProductVariantGroupService>()
                        .GetAll(Localization)
                        .Where(c => c.ParentId == Id)
                        .ToList();
            }

            set {}
        }

		#endregion

		#region Calculated properties

		private string _localizedUrl;
	    private IEnumerable<IProductVariantGroup> _productVariantGroups;

	    /// <summary>
		/// Is this product orderable
		/// stock should be higher then 0
		/// if stock is lower then 0, but backorder is enabled
		/// if the stockstatus is disabled
		/// if UseVariantStock and product has variants and all variants can be ordered
		/// </summary>
		/// <value>
		///   <c>true</c> if [orderable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool Orderable
		{
			get
			{
				if (VariantGroups.Any(x => x.Required && !x.Variants.Any(variant => variant.Orderable)))
				{
					return false;
				}
                //if (UseVariantStock && Variants.Any() && Variants.All(x => x.Orderable))
                //{
                //    return true;
                //}
                //if (UseVariantStock && Variants.Any() && Variants.All(x => !x.Orderable))
                //{
                //    return false;
                //}

				return Stock > 0 || Stock <= 0 && BackorderStatus || !StockStatus;
			}
			set { }
		}

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		public IDiscountedRangedPrice Price
		{
			get
			{
				return Businesslogic.Price.CreateDiscountedRanged(OriginalPriceInCents, Ranges, PricesIncludingVat, Vat,
					order => order.OrderLines.Where(line => line.ProductInfo.Id == Id).Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1)),// todo: move/centralize this
					(unDiscountedPrice, orderTotalItemCount) => IO.Container.Resolve<IProductDiscountService>().GetAdjustedPriceForProductWithId(Id, Localization, unDiscountedPrice, orderTotalItemCount),
					Localization);
			}
		}

		/// <summary>
		/// Returns if the pricing is discounted
		/// </summary>
		/// <value>
		///   <c>true</c> if [is discounted]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsDiscounted
		{
			get { return ProductDiscount != null; }
			set { }
		}

		/// <summary>
		/// Gets the localized URL.
		/// </summary>
		/// <value>
		/// The localized URL.
		/// </value>
		[Browsable(false)]
		[Obsolete("use UrlName")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string LocalizedUrl
		{
			get { return UrlName; }
		}

		/// <summary>
		/// The localized URL of the product
		/// By default (ID-URL)
		/// can be configured in the web.config
		/// </summary>
		/// <value>
		/// The name of the URL.
		/// </value>
		[DataMember]
		public new string UrlName
		{
			get
			{
				if (_localizedUrl != null) return _localizedUrl;

				var urlFormat = DomainHelper.BuildUrlFromTemplate(UwebshopConfiguration.Current.ProductUrl, this) ?? URL;

				_localizedUrl = IO.Container.Resolve<ICMSApplication>().ApplyUrlFormatRules(urlFormat);

				return _localizedUrl;
			}
			set { _localizedUrl = value; }
		}

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url
		{
			get { return NiceUrl(true); }
			set { }
		}

		/// <summary>
		/// The url for this product
		/// </summary>
		/// <returns></returns>
		public string NiceUrl()
		{
			return NiceUrl(false);
		}

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public string NiceUrl(string storeAlias)
		{
			return NiceUrl(false, storeAlias);
		}

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="getCanonicalUrl">if set to <c>true</c> get the canonical URL.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null)
		{
			// relevant modules:
			//  catalog url rendering (product)		-> IProductUrlService   (using ICategoryCatalogUrlService)
			//  (catalog) url localization			-> IUrlLocalizationService
			//  url formatting						-> IUrlFormatService
			//	localizing this product				-> IProductService
			var localization = storeAlias == null && currencyCode == null ? Localization : StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			if (getCanonicalUrl)
			{
				return IO.Container.Resolve<IUrlService>().ProductCanonical(this, localization);
			}
			else
			{
				return IO.Container.Resolve<IUrlService>().ProductUsingCurrentCategoryPathOrCurrentCategoryOrCanonical(this, localization);
			}
		}

		/// <summary>
		/// Fulls the localized URL.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		[Browsable(false)]
		[Obsolete("Use NiceUrl()")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string FullLocalizedUrl(string storeAlias = null)
		{
			return NiceUrl(storeAlias);
		}

		#endregion
		
		internal void ClearCachedValues()
		{
			_categories = null;
			_productVariantGroups = null;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "Product: " + Name;
		}

		#region Properties replaced by Price

		[DataMember]
		[Obsolete]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int VatAmountInCents
		{
			get { return Price.Vat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int PriceInCents
		{
			get { return (Price as Price).ValueInCents; }
			set { }
		}

		#region Non-cents properties

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal DiscountedVatAmount
		{
			get { return Price.Vat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal OriginalVatAmount
		{
			get { return Price.BeforeDiscount.Vat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal DiscountedPriceWithVat
		{
			get { return Price.WithVat.Value; }
			set { }
		}

		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal DiscountedPriceWithoutVat
		{
			get { return Price.WithoutVat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal OriginalPriceWithoutVat
		{
			get { return Price.BeforeDiscount.WithoutVat.Value; }
			set { }
		}

		/// <summary>
		/// The active price of the item without tax
		/// Gives the discounted price when the pricing is discounted
		/// Gives the normal price when the pricing isn't discounted
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal PriceWithoutVat
		{
			get { return Price.WithoutVat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal OriginalPriceWithVat
		{
			get { return Price.BeforeDiscount.WithVat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal PriceWithVat
		{
			get { return Price.WithVat.Value; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public decimal VatAmount
		{
			get { return Price.Vat.Value; }
			set { }
		}

		#endregion

		#region Vat calculations

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int OriginalVatAmountInCents
		{
			get { return Price.BeforeDiscount.Vat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int DiscountedVatAmountInCents
		{
			get { return Price.Vat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int DiscountedPriceWithVatInCents
		{
			get { return Price.WithVat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int DiscountedPriceWithoutVatInCents
		{
			get { return Price.WithoutVat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int PriceWithVatInCents
		{
			get { return Price.WithVat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int PriceWithoutVatInCents
		{
			get { return Price.WithoutVat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int OriginalPriceWithoutVatInCents
		{
			get { return Price.BeforeDiscount.WithoutVat.ValueInCents; }
			set { }
		}

		[DataMember]
		[Obsolete("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int OriginalPriceWithVatInCents
		{
			get { return Price.BeforeDiscount.WithVat.ValueInCents; }
			set { }
		}

		#endregion

		#endregion

		IEnumerable<Common.Interfaces.ICategory> Common.Interfaces.IProduct.Categories
		{
			get { return Categories.Cast<Common.Interfaces.ICategory>(); }
		}

	    
	}

	internal static class IProductExtensions
	{
		// todo: use TypeAlias instead of this
		internal static string NodeTypeAlias(this IProductInternalExternalShared product)
		{
			var p = product as Product;
			if (p != null && p.NodeTypeAlias != null)
			{
				return p.NodeTypeAlias;
			}

			if (product == null)
			{
				return Product.NodeAlias;
			}

			var entity = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(product.Id);

			if (entity != null && entity.NodeTypeAlias != null)
			{
				return entity.NodeTypeAlias;
			}
			return Product.NodeAlias;
		}
	}
}