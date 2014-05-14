using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract(Namespace = "")]
	[Serializable]
	public class ProductInfo // note: mirror of OrderedProduct, NOT of Product!
		//: IOrderedProduct, IOrderedStuff
	{
		internal delegate void ItemCountChangedEventHandler(EventArgs e);

		internal event ItemCountChangedEventHandler ItemCountChanged;

		[XmlIgnore] internal IOrderInfo Order;

		/// <summary>
		/// The product variants
		/// </summary>
		[DataMember] public List<ProductVariantInfo> ProductVariants; // entity - entity relation

		[XmlIgnore] private IProduct _product;

		#region Data fields

		internal int DiscountId;

		/// <summary>
		/// The unique identifier
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[DataMember] public int Id;

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		public int OriginalId { get { return Id; } }
		/// <summary>
		/// Gets the variants.
		/// </summary>
		/// <value>
		/// The variants.
		/// </value>
		public IEnumerable<IOrderedProductVariant> Variants { get { return ProductVariants; } }

		private int? _itemCount;

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[DataMember]
		public string Title { get; set; }


		public IDiscountedRangedPrice Price { get
		{
			return Businesslogic.Price.CreateDiscountedRanged(OriginalPriceInCents, Ranges, Order.PricesAreIncludingVAT, Vat,
				o => OrderTotalItemCount, 
				i => DiscountPercentage > 0 ? (int) ((100 - (long) DiscountPercentage)*i/100) : i - DiscountAmountInCents, 
				Order.Localization);
		}}


		internal IDiscountedRangedPrice PricePlusVariants
		{
			get
			{
				return Businesslogic.Price.CreateDiscountedRanged(RangedOriginalPrice, Ranges, Order.PricesAreIncludingVAT, Vat,
					o => OrderTotalItemCount,
					i => i - ProductDiscountInCents,
					Order.Localization);
			}
		}

		//internal IDiscountedRangedPrice OrderPricePlusVariants
		//{
		//	get
		//	{
		//		var order = Order as OrderInfo;
		//		var triggerDiscountCalculation = order != null ? order.DiscountAmountInCents : 0;
		//		// this can't 
		//		return Businesslogic.Price.CreateDiscountedRanged(PriceInCents, Ranges, Order.PricesAreIncludingVAT, Vat,
		//			o => OrderTotalItemCount,
		//			i => i /*- DiscountInCents */,
		//			Order.Localization);
		//	}
		//}

		/// <summary>
		/// Gets or sets the changed configuration.
		/// </summary>
		/// <value>
		/// The changed configuration.
		/// </value>
		[DataMember]
		public DateTime ChangedOn { get; set; }

		/// <summary>
		/// Gets or sets the sku.
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		[DataMember]
		public string SKU { get; set; }

		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		[DataMember]
		public string[] Tags { get; set; }

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		[DataMember]
		public double Weight { get; set; }

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		[DataMember]
		public double Length { get; set; }

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		[DataMember]
		public double Height { get; set; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		[DataMember]
		public double Width { get; set; }

		/// <summary>
		/// Gets or sets the original price in cents.
		/// </summary>
		/// <value>
		/// The original price in cents.
		/// </value>
		[DataMember]
		public int OriginalPriceInCents { get; set; }

		/// <summary>
		/// Gets or sets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		[DataMember]
		public decimal Vat { get; set; }

		/// <summary>
		/// Gets or sets the discount amount in cents.
		/// </summary>
		/// <value>
		/// The discount amount in cents.
		/// </value>
		[DataMember]
		public int DiscountAmountInCents { get; set; }

		/// <summary>
		/// Gets or sets the discount percentage.
		/// </summary>
		/// <value>
		/// The discount percentage.
		/// </value>
		[DataMember]
		public decimal DiscountPercentage { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [discount excluding variants].
		/// </summary>
		/// <value>
		/// <c>true</c> if [discount excluding variants]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool DiscountExcludingVariants { get; set; }

		/// <summary>
		/// Gets or sets the ranges string.
		/// </summary>
		/// <value>
		/// The ranges string.
		/// </value>
		[DataMember]
		public string RangesString { get; set; }

		/// <summary>
		/// Gets or sets the document type alias.
		/// </summary>
		/// <value>
		/// The document type alias.
		/// </value>
		[DataMember]
		public string DocTypeAlias { get; set; }

		/// <summary>
		/// Gets the type alias.
		/// </summary>
		/// <value>
		/// The type alias.
		/// </value>
		[IgnoreDataMember]
		public string TypeAlias { get { return DocTypeAlias; } }

		/// <summary>
		/// Gets or sets the item count.
		/// </summary>
		/// <value>
		/// The item count.
		/// </value>
		[DataMember]
		public int? ItemCount
		{
			get { return _itemCount; }
			set
			{
				if (ItemCountChanged != null) ItemCountChanged(new EventArgs());
				_itemCount = value;
			}
		}
		public int Quantity { get { return ItemCount.GetValueOrDefault(1); } }

		[XmlIgnore]
		internal int OrderTotalItemCount
		{
			get { return Order != null ? Order.OrderLines.Where(line => line.ProductInfo != null && line.ProductInfo.Id == Id).Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1)) : ItemCount.GetValueOrDefault(1); }
		}

		#endregion

		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		/// <exception cref="System.Exception">ProductInfo without ProductId</exception>
		[XmlIgnore]
		public IProduct CatalogProduct // weird coupling with umbraco dataprovider class
		{
			get
			{
				if (_product == null)
				{
					if (Id == 0)
					{
						return null;
					}
					_product = DomainHelper.GetProductById(Id);
				}
				return _product;
			}
		}

		#region Calculated properties

		internal int RangedPriceWithoutVariants
		{
			get { return Ranges.GetRangeAmountForValue(OrderTotalItemCount) ?? OriginalPriceInCents; }
		}

		/// <summary>
		/// Gets or sets the product range price in cents.
		/// </summary>
		/// <value>
		/// The product range price in cents.
		/// </value>
		[DataMember]
		public int ProductRangePriceInCents
		{
			get { return RangedPriceWithoutVariants + ProductVariants.Sum(variant => variant.DiscountedPriceInCents); }
			set { }
		}
		
		/// <summary>
		/// Gets or sets the ranges.
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		public List<Range> Ranges
		{
			get { return Range.CreateFromString(RangesString); }
			set { RangesString = value.ToRangesString(); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [is discounted].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is discounted]; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsDiscounted
		{
			get { return DiscountPercentage > 0 || DiscountAmountInCents > 0; }
			set { }
		}

		/// <summary>
		/// Gets the price of the SellableUnit (Product + chosen Variants).
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[DataMember]
		public int PriceInCents
		{
			get
			{
				if (DiscountExcludingVariants)
					return (int)(ProductVariants.Sum(variant => variant.DiscountedPriceInCents) + ((100 - (long)DiscountPercentage)*RangedPriceWithoutVariants)/100 - DiscountAmountInCents);
				return (int)((100 - (long)DiscountPercentage)*ProductRangePriceInCents/100 - DiscountAmountInCents);
			}
			set { }
		}

		[IgnoreDataMember]
		internal int ProductDiscountInCents
		{
			get 
			{
				if (DiscountExcludingVariants)
					return (int)(ProductVariants.Sum(variant => variant.ProductDiscountInCents) + (long)DiscountPercentage * RangedPriceWithoutVariants / 100 + DiscountAmountInCents);
				return (int)((long)DiscountPercentage * ProductRangePriceInCents / 100 + DiscountAmountInCents);
			}
		}

		[IgnoreDataMember]
		internal int RangedOriginalPrice
		{
			get { return RangedPriceWithoutVariants + ProductVariants.Sum(variant => variant.RangedPriceInCents); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductInfo"/> class.
		/// </summary>
		public ProductInfo()
		{
			ProductVariants = new List<ProductVariantInfo>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductInfo"/> class.
		/// </summary>
		/// <param name="orderedProduct">The ordered product.</param>
		/// <param name="order">The order.</param>
		public ProductInfo(OrderedProduct orderedProduct, IOrderInfo order) : this()
		{
			Order = order;

			Id = orderedProduct.ProductId;

			Title = orderedProduct.Title;
			SKU = orderedProduct.SKU;
			Weight = orderedProduct.Weight;
			Length = orderedProduct.Length;
			Width = orderedProduct.Width;
			Height = orderedProduct.Height;
			Vat = orderedProduct.Vat;
			ItemCount = orderedProduct.ItemCount.GetValueOrDefault(1);

			ChangedOn = orderedProduct.UpdateDate;

			RangesString = orderedProduct.RangesString;

			OriginalPriceInCents = orderedProduct.PriceInCents;

			DiscountAmountInCents = orderedProduct.OrderedProductDiscountAmount;
			DiscountPercentage = orderedProduct.OrderedProductDiscountPercentage;
			DiscountExcludingVariants = orderedProduct.OrderedProductDiscountExcludingVariants;

			DocTypeAlias = orderedProduct.TypeAlias ?? orderedProduct.DoctypeAlias;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProductInfo"/> class.
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="order">The order.</param>
		/// <param name="itemCount">The item count.</param>
		public ProductInfo(Product product, IOrderInfo order, int itemCount) : this()
		{
			if (product == null)
			{
				return;
				//throw new ArgumentNullException("product null");
			}
			Order = order;
			_product = product;

			Id = product.Id;
			ItemCount = itemCount;
			Title = product.Title;
			SKU = product.SKU;
			Tags = product.Tags;
			Weight = product.Weight;
			Length = product.Length;
			Width = product.Width;
			Height = product.Height;
			Vat = product.Vat;
			ChangedOn = DateTime.Now;

			Ranges = product.Ranges.ToList(); // = localized

			OriginalPriceInCents = product.Price.BeforeDiscount.ValueInCents();

			if (product.IsDiscounted)
			{
				DiscountId = product.ProductDiscount.Id;
			}
			IO.Container.Resolve<IOrderUpdatingService>().UpdateProductInfoDiscountInformation(this);

			DocTypeAlias = product.NodeTypeAlias;
		}

		#endregion

		#region Helper properties (with/withoutVat and Non-cents)

		private int? _productRangePriceWithVatInCentsLegacyDataReadBack;
		private int _discountInCents;

		/// <summary>
		/// Gets or sets the vat amount.
		/// </summary>
		/// <value>
		/// The vat amount.
		/// </value>
		[DataMember]
		public decimal VatAmount
		{
			get { return VatAmountInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product price with vat.
		/// </summary>
		/// <value>
		/// The product price with vat.
		/// </value>
		[DataMember]
		public decimal ProductPriceWithVat
		{
			get { return ProductPriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product price without vat.
		/// </summary>
		/// <value>
		/// The product price without vat.
		/// </value>
		[DataMember]
		public decimal ProductPriceWithoutVat
		{
			get { return ProductPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product range price without vat.
		/// </summary>
		/// <value>
		/// The product range price without vat.
		/// </value>
		[DataMember]
		public decimal ProductRangePriceWithoutVat
		{
			get { return ProductPriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product range price with vat.
		/// </summary>
		/// <value>
		/// The product range price with vat.
		/// </value>
		[DataMember]
		public decimal ProductRangePriceWithVat
		{
			get { return ProductPriceWithVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price with vat.
		/// </summary>
		/// <value>
		/// The discounted price with vat.
		/// </value>
		[DataMember]
		public decimal DiscountedPriceWithVat
		{
			get { return PriceWithVatInCents/100m; }
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
			get { return PriceWithoutVatInCents/100m; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted vat.
		/// </summary>
		/// <value>
		/// The discounted vat.
		/// </value>
		public decimal DiscountedVat
		{
			get { return DiscountedVatInCents/100m; }
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
		/// Gets or sets the product price with vat in cents.
		/// </summary>
		/// <value>
		/// The product price with vat in cents.
		/// </value>
		[DataMember]
		public int ProductPriceWithVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? ProductRangePriceInCents : VatCalculator.WithVat(ProductRangePriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the product price without variants with vat in cents.
		/// </summary>
		/// <value>
		/// The product price without variants with vat in cents.
		/// </value>
		[DataMember]
		public int ProductPriceWithoutVariantsWithVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? RangedPriceWithoutVariants : VatCalculator.WithVat(RangedPriceWithoutVariants, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the product price without vat in cents.
		/// </summary>
		/// <value>
		/// The product price without vat in cents.
		/// </value>
		[DataMember]
		public int ProductPriceWithoutVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(ProductRangePriceInCents, Vat) : ProductRangePriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product price without variants without vat in cents.
		/// </summary>
		/// <value>
		/// The product price without variants without vat in cents.
		/// </value>
		[DataMember]
		public int ProductPriceWithoutVariantsWithoutVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(RangedPriceWithoutVariants, Vat) : RangedPriceWithoutVariants; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product original price with vat in cents.
		/// </summary>
		/// <value>
		/// The product original price with vat in cents.
		/// </value>
		[DataMember]
		public int ProductOriginalPriceWithVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? OriginalPriceInCents : VatCalculator.WithVat(OriginalPriceInCents, Vat); }
			set { }
		}

		/// <summary>
		/// Gets or sets the product original price without vat in cents.
		/// </summary>
		/// <value>
		/// The product original price without vat in cents.
		/// </value>
		[DataMember]
		public int ProductOriginalPriceWithoutVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(OriginalPriceInCents, Vat) : OriginalPriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the product range price with vat in cents.
		/// </summary>
		/// <value>
		/// The product range price with vat in cents.
		/// </value>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[Browsable(false)]
		[Obsolete("Use ProductPriceWithVatInCents")]
		[DataMember]
		public int ProductRangePriceWithVatInCents
		{
			get { return _productRangePriceWithVatInCentsLegacyDataReadBack ?? ProductPriceWithVatInCents; }
			set { _productRangePriceWithVatInCentsLegacyDataReadBack = value; }
		}

		/// <summary>
		/// Gets or sets the product range price without vat in cents.
		/// </summary>
		/// <value>
		/// The product range price without vat in cents.
		/// </value>
		[Browsable(false)]
		[Obsolete("Use ProductPriceWithoutVatInCents")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[DataMember]
		public int ProductRangePriceWithoutVatInCents
		{
			get { return ProductPriceWithoutVatInCents; }
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
			get { return Order.PricesAreIncludingVAT ? PriceInCents : VatCalculator.WithVat(PriceInCents, Vat); }
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
			get { return Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(PriceInCents, Vat) : PriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price with vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price with vat in cents.
		/// </value>
		[Browsable(false)]
		[DataMember]
		[Obsolete("Use PriceWithVatInCents")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int DiscountedPriceWithVatInCents
		{
			get { return PriceWithVatInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted price without vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price without vat in cents.
		/// </value>
		[Browsable(false)]
		[DataMember]
		[Obsolete("Use PriceWithoutVatInCents")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int DiscountedPriceWithoutVatInCents
		{
			get { return Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(PriceInCents, Vat) : PriceInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the discounted vat in cents.
		/// </summary>
		/// <value>
		/// The discounted vat in cents.
		/// </value>
		[DataMember]
		public int DiscountedVatInCents
		{
			get { return VatAmountInCents; }
			set { }
		}

		/// <summary>
		/// Gets or sets the vat amount in cents.
		/// </summary>
		/// <value>
		/// The vat amount in cents.
		/// </value>
		[DataMember]
		public int VatAmountInCents
		{
			get { return PriceWithVatInCents - PriceWithoutVatInCents; }
			set { }
		}

		#endregion

		public int GetAmount(bool ranged)
		{
			return GetAmount(true, ranged);
		}
		public int GetAmount(bool saleDiscounted, bool ranged)
		{
			var price = ranged ? RangedPriceWithoutVariants : OriginalPriceInCents;
			if (saleDiscounted && DiscountExcludingVariants)
			{
				price = (int)((100 - (long)DiscountPercentage) * price) / 100 - DiscountAmountInCents;
			}
			return price;
		}
	}
}