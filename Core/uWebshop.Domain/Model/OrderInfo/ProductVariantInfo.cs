using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using uWebshop.Common;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Domain
{
#pragma warning disable 1591
	public interface IOrderedProductVariant// : IUwebshopEntity
	{
		/// <summary>
		/// Gets the name or alias for the type. (NodeTypeAlias/ContentTypeAlias in Umbraco)
		/// </summary>
		/// <value>
		/// The type alias.
		/// </value>
		string TypeAlias { get; }

		IDiscountedRangedPrice Price { get; }
		
		bool Required { get; }

		string Title { get; }

		string Group { get; }

		double Weight { get; }

		double Length { get; }

		double Height { get; }

		double Width { get; }

		string SKU { get; }

		decimal Vat { get; }

		//string DocTypeAlias { get; }
		//int DiscountAmountInCents { get; set; }
		//decimal DiscountPercentage { get; set; }

		IProductVariant Variant { get; }

		IEnumerable<Range> Ranges { get; }

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		int OriginalId { get; }
	}
	internal interface IOrderedItem
	{
		string DocTypeAlias { get; }
	}

	[DataContract(Namespace = "")]
	[Serializable]
	public class ProductVariantInfo : IOrderedProductVariant, IOrderedItem
	{
		//[XmlIgnore] internal IOrderInfo Order;
		internal int DiscountId;
		[XmlIgnore] [NonSerialized] internal ProductInfo Product;
		[XmlIgnore] [NonSerialized] private ProductVariant _variant;

		public ProductVariantInfo()
		{
		}

		/// <summary>
		/// Set product variant info based on the variant already in the order
		/// </summary>
		/// <param name="productVariant">The product variant.</param>
		/// <param name="product">The product.</param>
		/// <param name="productVat">The product vat.</param>
		public ProductVariantInfo(OrderedProductVariant productVariant, ProductInfo product, decimal productVat)
		{
			Product = product;
			
			Id = productVariant.VariantId;
			Title = productVariant.Title;
			SKU = productVariant.SKU;
			Group = productVariant.Group;
			Weight = productVariant.Weight;
			Length = productVariant.Length;
			Height = productVariant.Height;
			Width = productVariant.Width;
			PriceInCents = productVariant.PriceInCents;
			RangesString = productVariant.RangesString;
			ChangedOn = DateTime.Now;
			Vat = productVat;
			DiscountAmountInCents = productVariant.DiscountAmount;
			DiscountPercentage = productVariant.DiscountPercentage;

			DocTypeAlias = productVariant.TypeAlias ?? productVariant.DocTypeAlias;
		}

		/// <summary>
		/// Set product variant info based on a product variant in the catalog
		/// </summary>
		/// <param name="productVariant">The product variant.</param>
		/// <param name="product">The product.</param>
		/// <param name="itemCount">The item count.</param>
		public ProductVariantInfo(ProductVariant productVariant, ProductInfo product, int itemCount)
		{
			Product = product;

			Id = productVariant.Id;
			Title = productVariant.Title;
			SKU = productVariant.SKU;

			var groupname = string.Empty;
			if (string.IsNullOrEmpty(productVariant.Group))
			{
				var productVariantGroup = DomainHelper.GetProductVariantGroupById(productVariant.ParentId);

				if (productVariantGroup != null)
				{
					groupname = productVariantGroup.Title;
				}
			}
			else
			{
				groupname = productVariant.Group;
			}

			Group = groupname;

			Weight = productVariant.Weight;
			Length = productVariant.Length;
			Height = productVariant.Height;
			Width = productVariant.Width;

			PriceInCents = productVariant.OriginalPriceInCents;
			Ranges = productVariant.Ranges; // = localized

			ChangedOn = DateTime.Now;
			Vat = productVariant.Vat;

			if (productVariant.IsDiscounted)
			{
				DiscountId = productVariant.ProductVariantDiscount.Id;
				if (productVariant.ProductVariantDiscount.DiscountType == DiscountType.Amount)
					DiscountAmountInCents = productVariant.ProductVariantDiscount.RangedDiscountValue(itemCount);
				else if (productVariant.ProductVariantDiscount.DiscountType == DiscountType.Percentage)
					DiscountPercentage = productVariant.ProductVariantDiscount.RangedDiscountValue(itemCount)/100m;
				else if (productVariant.ProductVariantDiscount.DiscountType == DiscountType.NewPrice)
					PriceInCents = productVariant.ProductVariantDiscount.RangedDiscountValue(itemCount);
			}

			DocTypeAlias = productVariant.NodeTypeAlias;
		}

		[DataMember]
		public int Id { get; set; }

		[IgnoreDataMember]
		public bool Required { get; set; }

		[DataMember]
		public string Title { get; set; }

		[IgnoreDataMember]
		public bool Orderable { get { return true; } set { } }

		[DataMember]
		public DateTime ChangedOn { get; set; }

		[DataMember]
		public string Group { get; set; }

		[DataMember]
		public double Weight { get; set; }

		[DataMember]
		public double Length { get; set; }

		[DataMember]
		public double Height { get; set; }

		[DataMember]
		public double Width { get; set; }

		[DataMember]
		public int PriceInCents { get; set; }

		public IDiscountedRangedPrice Price
		{
			get 
			{ 
				return Businesslogic.Price.CreateDiscountedRanged(PriceInCents, Ranges, Product.Order.PricesAreIncludingVAT, Vat,
					o => OrderTotalItemCount, i => (int)((100 - DiscountPercentage) * i) / 100 - DiscountAmountInCents, Product.Order.Localization);
			}
		}

		[DataMember]
		public string RangesString { get; set; }

		[DataMember]
		public string SKU { get; set; }

		[DataMember]
		public decimal Vat { get; set; }

		[DataMember]
		public string DocTypeAlias { get; set; }

		[IgnoreDataMember]
		public string TypeAlias { get { return DocTypeAlias; } }

		[DataMember]
		public int DiscountAmountInCents { get; set; }

		[DataMember]
		public decimal DiscountPercentage { get; set; }

		[XmlIgnore]
		public int RangedPriceInCents
		{
			get { return Ranges.GetRangeAmountForValue(OrderTotalItemCount) ?? PriceInCents; }
		}

		[XmlIgnore]
		internal int OrderTotalItemCount
		{
			get { return Product != null && Product.Order != null ? Product.Order.OrderLines.Where(line => line.HasVariantWithId(Id)).Sum(line => line.ProductInfo.ItemCount.GetValueOrDefault(1)) : (Product != null ? Product.ItemCount.GetValueOrDefault(1) : 1); }
		}

		[XmlIgnore]
		public IProductVariant Variant
		{
			get { return _variant ?? (_variant = DomainHelper.GetProductVariantById(Id, Product.Order.StoreInfo.Alias)); }
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public IEnumerable<Range> Ranges
		{
			get { return Range.CreateFromString(RangesString); }
			set { RangesString = value.ToRangesString(); }
		}

		public int OriginalId {
			get { return Id; }
		}

		public int DiscountedPriceInCents
		{
			get { return (int) ((100 - DiscountPercentage)*RangedPriceInCents)/100 - DiscountAmountInCents; }
			set { }
		}

		public int GetOriginalAmount(bool ranged)
		{
			return GetAmount(false, ranged);
		}
		public int GetAmount(bool saleDiscounted, bool ranged)
		{
			var basePrice = ranged ? RangedPriceInCents : PriceInCents;
			return saleDiscounted ? ApplyDiscount(basePrice) : basePrice;
		}
		internal int ApplyDiscount(int price)
		{
			return (int)((100 - DiscountPercentage) * price) / 100 - DiscountAmountInCents;
		}
		
		[IgnoreDataMember]
		internal int ProductDiscountInCents
		{
			get { return (int)(DiscountPercentage * RangedPriceInCents) / 100 + DiscountAmountInCents; }
		}

		[DataMember]
		public int PriceWithVatInCents
		{
			get { return Product.Order.PricesAreIncludingVAT ? RangedPriceInCents : VatCalculator.WithVat(RangedPriceInCents, Vat); }
			set { }
		}

		[DataMember]
		public int PriceWithoutVatInCents
		{
			get { return Product.Order.PricesAreIncludingVAT ? VatCalculator.WithoutVat(RangedPriceInCents, Vat) : RangedPriceInCents; }
			set { }
		}

		[DataMember]
		public decimal PriceWithoutVat
		{
			get { return PriceWithoutVatInCents/100m; }
			set { }
		}

		[DataMember]
		public decimal PriceWithVat
		{
			get { return PriceWithVatInCents/100m; }
			set { }
		}
	}
}