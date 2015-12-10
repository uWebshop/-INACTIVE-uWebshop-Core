using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{

	/// <summary>
	/// Variant of a product
	/// Ordered by group
	/// </summary>
	[DataContract(Namespace = "", IsReference = true)]
	[ContentType(ParentContentType = typeof(Catalog), Name = "Product Variant", Description = "#ProductVariantDescription", Alias = "uwbsProductVariant", IconClass = IconClass.link, Icon = ContentIcon.MagnetSmall, Thumbnail = ContentThumbnail.Folder)]
	public class ProductVariant : uWebshopEntity, IProductVariant //, IProductVariant
	{
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		private IProduct _product;
		internal ILocalization Localization;

		#region Helper properties (with/withoutVat and Non-cents)

		/// <summary>
		/// Price of the product variant based on the price value
		/// !NO VAT RULES APPLIED!
		/// </summary>
		/// <value>
		/// The original price.
		/// </value>
		[DataMember]
		public decimal OriginalPrice
		{
			get { return OriginalPriceInCents/100m; }
			set { }
		}

		/// <summary>
		/// Price of the product + this product variant INCLUDING Vat
		/// </summary>
		/// <value>
		/// The price including product price with vat.
		/// </value>
		[DataMember]
		public decimal PriceIncludingProductPriceWithVat
		{
			get { return PriceIncludingProductPriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the original price including product price with vat.
		/// </summary>
		/// <value>
		/// The original price including product price with vat.
		/// </value>
		[DataMember]
		public decimal OriginalPriceIncludingProductPriceWithVat
		{
			get { return OriginalPriceIncludingProductPriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Price of the product + this product variant EXCLUDING Vat
		/// </summary>
		/// <value>
		/// The price including product price without vat.
		/// </value>
		[DataMember]
		public decimal PriceIncludingProductPriceWithoutVat
		{
			get { return PriceIncludingProductPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the original price including product price without vat.
		/// </summary>
		/// <value>
		/// The original price including product price without vat.
		/// </value>
		[DataMember]
		public decimal OriginalPriceIncludingProductPriceWithoutVat
		{
			get { return OriginalPriceIncludingProductPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price with vat.
		/// </summary>
		/// <value>
		/// The price with vat.
		/// </value>
		[DataMember]
		public decimal PriceWithVat
		{
			get { return PriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat.
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		[DataMember]
		public decimal PriceWithoutVat
		{
			get { return PriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Price of the product + this product variant INCLUDING Vat
		/// </summary>
		/// <value>
		/// The price including product price with vat in cents.
		/// </value>
		[DataMember]
		public int PriceIncludingProductPriceWithVatInCents
		{
			get { return Product.PricesIncludingVat ? PriceIncludingProductPriceInCents : VatCalculator.WithVat(PriceIncludingProductPriceInCents, Product.Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the original price including product price with vat in cents.
		/// </summary>
		/// <value>
		/// The original price including product price with vat in cents.
		/// </value>
		[DataMember]
		public int OriginalPriceIncludingProductPriceWithVatInCents
		{
			get { return Product.PricesIncludingVat ? OriginalPriceIncludingProductPriceInCents : VatCalculator.WithVat(OriginalPriceIncludingProductPriceInCents, Product.Vat); }
			set { }
		}

		/// <summary>
		/// Price of the product + this product variant EXCLUDING Vat
		/// </summary>
		/// <value>
		/// The price including product price without vat in cents.
		/// </value>
		[DataMember]
		public int PriceIncludingProductPriceWithoutVatInCents
		{
			get { return Product.PricesIncludingVat ? VatCalculator.WithoutVat(PriceIncludingProductPriceInCents, Product.Vat) : PriceIncludingProductPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the original price including product price without vat in cents.
		/// </summary>
		/// <value>
		/// The original price including product price without vat in cents.
		/// </value>
		public int OriginalPriceIncludingProductPriceWithoutVatInCents
		{
			get { return Product.PricesIncludingVat ? VatCalculator.WithoutVat(OriginalPriceIncludingProductPriceInCents, Product.Vat) : OriginalPriceIncludingProductPriceInCents; }
			set { }
		}

		#endregion

		#region Non-cents

		/// <summary>
		/// The discounted tax amount
		/// </summary>
		/// <value>
		/// The discounted vat amount.
		/// </value>
		[DataMember]
		public decimal DiscountedVatAmount
		{
			get { return DiscountedVatAmountInCents/100m; }
			set { }
		}

		/// <summary>
		/// Tax amount
		/// </summary>
		/// <value>
		/// The original vat amount.
		/// </value>
		[DataMember]
		public decimal OriginalVatAmount
		{
			get { return OriginalVatAmountInCents/100m; }
			set { }
		}

		/// <summary>
		/// The discounted price with tax
		/// </summary>
		/// <value>
		/// The discounted price with vat.
		/// </value>
		[DataMember]
		public decimal DiscountedPriceWithVat
		{
			get { return DiscountedPriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price without vat.
		/// </summary>
		/// <value>
		/// The discounted price without vat.
		/// </value>
		public decimal DiscountedPriceWithoutVat
		{
			get { return DiscountedPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// The price of the pricing without VAT
		/// </summary>
		/// <value>
		/// The original price without vat.
		/// </value>
		[DataMember]
		public decimal OriginalPriceWithoutVat
		{
			get { return OriginalPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// The price of the pricing with VAT
		/// </summary>
		/// <value>
		/// The original price with vat.
		/// </value>
		[DataMember]
		public decimal OriginalPriceWithVat
		{
			get { return OriginalPriceWithVatInCents/100m; }
			set { }
		}

		#endregion

		#region Vat calculations

		/// <summary>
		/// Tax amount
		/// </summary>
		/// <value>
		/// The original vat amount in cents.
		/// </value>
		[DataMember]
		public int OriginalVatAmountInCents
		{
			get { return VatCalculator.VatAmountFromOriginal(Product.PricesIncludingVat, RangedPriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// The discounted tax amount
		/// </summary>
		/// <value>
		/// The discounted vat amount in cents.
		/// </value>
		[DataMember]
		public int DiscountedVatAmountInCents
		{
			get { return VatCalculator.VatAmountFromOriginal(Product.PricesIncludingVat, DiscountedPriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price with vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price with vat in cents.
		/// </value>
		[DataMember]
		public int DiscountedPriceWithVatInCents
		{
			get { return Product.PricesIncludingVat ? DiscountedPriceInCents : VatCalculator.WithVat(DiscountedPriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price without vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price without vat in cents.
		/// </value>
		[DataMember]
		public int DiscountedPriceWithoutVatInCents
		{
			get { return Product.PricesIncludingVat ? VatCalculator.WithoutVat(DiscountedPriceInCents, Vat) : DiscountedPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the price with vat in cents.
		/// </summary>
		/// <value>
		/// The price with vat in cents.
		/// </value>
		[DataMember]
		public int PriceWithVatInCents
		{
			get { return Product.PricesIncludingVat ? PriceInCents : VatCalculator.WithVat(PriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the price without vat in cents.
		/// </summary>
		/// <value>
		/// The price without vat in cents.
		/// </value>
		[DataMember]
		public int PriceWithoutVatInCents
		{
			get { return Product.PricesIncludingVat ? VatCalculator.WithoutVat(PriceInCents, Vat) : PriceInCents; }
			set { }
		}

		/// <summary>
		/// The price of the pricing without VAT
		/// </summary>
		/// <value>
		/// The original price without vat in cents.
		/// </value>
		[DataMember]
		public int OriginalPriceWithoutVatInCents
		{
			get { return Product.PricesIncludingVat ? VatCalculator.WithoutVat(RangedPriceInCents, Vat) : RangedPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the original price with vat in cents.
		/// </summary>
		/// <value>
		/// The original price with vat in cents.
		/// </value>
		[DataMember]
		public int OriginalPriceWithVatInCents
		{
			get { return Product.PricesIncludingVat ? RangedPriceInCents : VatCalculator.WithVat(RangedPriceInCents, Vat); }
			set { }
		}

		#endregion

		/// <summary>
		/// Is this product variant orderable:
		/// stock should be higher then 0
		/// if stock is lower then 0, but backorder is enabled
		/// if the stockstatus is disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [orderable]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool Orderable
		{
			get { return Stock > 0 || Stock <= 0 && BackorderStatus || StockStatus == false; }
			set { }
		}

		public IProductDiscount Discount {
			get { return ProductVariantDiscount; }
		}

		/// <summary>
		/// The group the variant is in
		/// </summary>
		/// <value>
		/// The pricing variant group.
		/// </value>
		[DataMember]
		[Obsolete("Use Group")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string PricingVariantGroup
		{
			get { return Group; }
		}

		[DataMember]
		internal int RangedPriceInCents
		{
			get
			{
				if (Ranges != null)
				{
					var range = Ranges.FirstOrDefault(x => x.From <= 1 && x.PriceInCents != 0);
					if (range != null)
					{
						return range.PriceInCents;
					}
				}
				return OriginalPriceInCents;
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price in cents.
		/// </summary>
		/// <value>
		/// The discounted price in cents.
		/// </value>
		protected int DiscountedPriceInCents
		{
			get { return ProductVariantDiscount.GetAdjustedPrice(RangedPriceInCents); }
			set { }
		}

		/// <summary>
		/// Gets the sale of the pricing
		/// </summary>
		/// <value>
		/// The product variant discount.
		/// </value>
		[DataMember]
		public DiscountProduct ProductVariantDiscount
		{
			get { return IO.Container.Resolve<IProductDiscountService>().GetDiscountByProductVariantId(Id, Localization); }
			set { }
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
			get { return ProductVariantDiscount != null; }
			set { }
		}

		[DataMember]
		internal int PriceIncludingProductPriceInCents
		{
			get
			{
				// todo: broken
				return Product.Price.ValueInCents() + (Product.IsDiscounted && !Product.Discount.ExcludeVariants && Product.Discount.Type == DiscountType.Percentage ? PriceInCents - DiscountHelper.PercentageCalculation(PriceInCents, Product.Discount.DiscountValue) : PriceInCents);
			}
			set { }
		}

		internal int OriginalPriceIncludingProductPriceInCents
		{
			get { return Product.Price.BeforeDiscount.ValueInCents() + OriginalPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		public decimal Vat
		{
			get { return Product.Vat; }
			set { }
		}

		/// <summary>
		/// Gets the product of the variant
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		public IProduct Product
		{
			get
			{
				if (_product != null)
				{
					return _product;
				}

				var variantGroup = DomainHelper.GetProductVariantGroupById(ParentId);
				if (variantGroup != null)
				{
					return _product = DomainHelper.GetProductById(variantGroup.ParentId);
				}
				
				return _product = DomainHelper.GetProductById(ParentId);
			}
			set { _product = value; }
		}

		internal static bool IsAlias(string alias)
		{
			return alias != null && alias.StartsWith(NodeAlias);
		}

		#region global tab

		/// <summary>
		/// Gets the title of the content
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "title", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#Title", Description = "#TitleDescription", Mandatory = true, SortOrder = 1)]
		public string Title { get; set; }

		/// <summary>
		/// ProductVariant SKU
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "sku", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#SKU", Description = "#SKUDescription", SortOrder = 2)]
		public string SKU { get; set; }

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
		[DataMember]
		public string Group { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [required variant].
		/// </summary>
		/// <value>
		///   <c>true</c> if [required variant]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[Obsolete("Use Required")]
		public bool RequiredVariant
		{
			get { return Required; }
			set { Required = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [required variant].
		/// </summary>
		/// <value>
		///   <c>true</c> if [required variant]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool Required { get; set; }

		/// <summary>
		/// Is this content enabled?
		/// </summary>
		/// <value>
		///   <c>true</c> if disabled; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "disable", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#Disable", Description = "#DisableDescription", Mandatory = false, SortOrder = 5)]
		public override bool Disabled { get; set; }

		#endregion

		#region details tab

		/// <summary>
		/// Gets the long description of the content
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "description", DataType = DataType.RichText, Tab = ContentTypeTab.Details, Name = "#Description", Description = "#DescriptionDescription", Mandatory = false, SortOrder = 6)]
		public string Description { get; set; }

		/// <summary>
		/// Length of the product
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "length", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Length", Description = "#LengthDescription", SortOrder = 7)]
		public double Length { get; set; }

		/// <summary>
		/// Width of the product
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "width", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Width", Description = "#WidthDescription", SortOrder = 8)]
		public double Width { get; set; }

		/// <summary>
		/// Height of the product
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "height", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Height", Description = "#HeightDescription", SortOrder = 9)]
		public double Height { get; set; }

		/// <summary>
		/// Weight of the product
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "weight", DataType = DataType.Numeric, Tab = ContentTypeTab.Details, Name = "#Weight", Description = "#WeightDescription", SortOrder = 10)]
		public double Weight { get; set; }

		#endregion

		#region price tab

		[DataMember]
		internal int OriginalPriceInCents { get; set; }

		/// <summary>
		/// Gets or sets the price in cents.
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "price", DataType = DataType.Price, Tab = ContentTypeTab.Price, Name = "#Price", Description = "#PriceDescription", SortOrder = 15)]
		public int PriceInCents
		{
			get { return IsDiscounted ? DiscountedPriceInCents : RangedPriceInCents; }
			set { }
		}

		public IDiscountedRangedPrice Price
		{
			get
			{
				// todo: move? and test
				return Businesslogic.Price.CreateDiscountedRanged(OriginalPriceInCents, Ranges, Product.PricesIncludingVat, Vat,
					order => order.OrderLines.Where(line => line.ProductInfo.ProductVariants.Any(x => x.Id == Id)).Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1)),// todo: move/centralize this
					(unDiscountedPrice, orderTotalItemCount) => IO.Container.Resolve<IProductDiscountService>().GetAdjustedPriceForProductVariantWithId(Id, Localization, unDiscountedPrice, orderTotalItemCount),
					Localization);
			}
		}

		/// <summary>
		/// Gets the price including the product.
		/// </summary>
		/// <value>
		/// The price including the product.
		/// </value>
		public IDiscountedRangedPrice PriceIncludingProduct(API.IProduct product)
		{
			return new CombiPrice((Price) Price, (Price) product.Price, Localization);
		}

		/// <summary>
		/// Ranges from the range data type
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "ranges", DataType = DataType.Ranges, Tab = ContentTypeTab.Price, Name = "#Ranges", Description = "#RangesDescription", SortOrder = 20)]
		public IEnumerable<Range> Ranges { get; set; }

		/// <summary>
		/// Gets the stock of the variant
		/// </summary>
		/// <value>
		/// The stock.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "stock", DataType = DataType.Stock, Tab = ContentTypeTab.Price, Name = "#Stock", Description = "#StockDescription", SortOrder = 25)]
		public int Stock
		{
			// caching here is impossible (breaks with app wide static caching of products), per-request caching is done some levels deeper
			get { return StoreHelper.GetMultiStoreStock(Id); }
			set { }
		}

		/// <summary>
		/// Gets the number of times this ProductVariant is ordered
		/// </summary>
		/// <value>
		/// The order count.
		/// </value>
		[DataMember]
		[ContentPropertyType(Alias = "ordered", DataType = DataType.OrderedCount, Tab = ContentTypeTab.Price, Name = "#Ordered", Description = "#OrderedDescription", SortOrder = 26)]
		public int OrderCount
		{
			get { return UWebshopStock.GetOrderCount(Id); }
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
		[ContentPropertyType(Alias = "stockStatus", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Price, Name = "#StockStatus", Description = "#StockStatusDescription", SortOrder = 27)]
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
		[ContentPropertyType(Alias = "backorderStatus", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Price, Name = "#BackorderStatus", Description = "#BackorderStatusDescription", SortOrder = 28)]
		public bool BackorderStatus { get; set; }

		public string GetProperty(string propertyAlias)
		{
			if (!string.IsNullOrEmpty(propertyAlias))
			{
				var uwebshopReadonlyContent = Node;
				if (uwebshopReadonlyContent != null)
				{
					var property = uwebshopReadonlyContent.GetMultiStoreItem(propertyAlias);
					if (property != null)
					{
						return property.Value;
					}
				}
			}

			return string.Empty;
		}

		#endregion
	}
}