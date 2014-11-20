using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using uWebshop.Common.Interfaces.Shared;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public interface IProductLegacy230 : IProduct, IProductLegacyPriceFields, IUwebshopUmbracoEntity
	{
		IEnumerable<ProductVariantGroup> ProductVariantGroups { get; }
	}

	/// <summary>
	/// 
	/// </summary>
    public interface IProduct : IUwebshopSortableEntity, IProductInternalExternalShared
	{
		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		IDiscountedRangedPrice Price { get; }

		/// <summary>
		/// Gets a value indicating whether [has categories].
		/// </summary>
		/// <value>
		///   <c>true</c> if [has categories]; otherwise, <c>false</c>.
		/// </value>
		bool HasCategories { get; }

		/// <summary>
		/// Gets the categories.
		/// </summary>
		/// <value>
		/// The categories.
		/// </value>
		IEnumerable<ICategory> Categories { get; }

		/// <summary>
		/// Gets a value indicating whether [prices including vat].
		/// </summary>
		/// <value>
		///   <c>true</c> if [prices including vat]; otherwise, <c>false</c>.
		/// </value>
		bool PricesIncludingVat { get; }

		/// <summary>
		/// Product Number
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		string SKU { get; }

		/// <summary>
		/// Gets the tags of the product
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		string[] Tags { get; }

		/// <summary>
		/// List of images of the product
		/// </summary>
		/// <value>
		/// The images.
		/// </value>
		IEnumerable<Image> Images { get; }

		/// <summary>
		/// List of files of the product
		/// </summary>
		/// <value>
		/// The files.
		/// </value>
		IEnumerable<File> Files { get; }

		/// <summary>
		/// Length of the product
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		double Length { get; }

		/// <summary>
		/// Width of the product
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		double Width { get; }

		/// <summary>
		/// Height of the product
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		double Height { get; }

		/// <summary>
		/// Weight of the product
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		double Weight { get; }

		/// <summary>
		/// Ranges from the range data type
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		IEnumerable<Range> Ranges { get; }

		/// <summary>
		/// The taxpercentage of the pricing
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		decimal Vat { get; }

		/// <summary>
		/// Stock count
		/// </summary>
		/// <value>
		/// The stock.
		/// </value>
		int Stock { get; }

		/// <summary>
		/// Number of times this item was ordered
		/// </summary>
		/// <value>
		/// The total items ordered.
		/// </value>
		int TotalItemsOrdered { get; }

		/// <summary>
		/// Gets the number of times this Product is ordered
		/// </summary>
		/// <value>
		/// The order count.
		/// </value>
		int OrderCount { get; }

		/// <summary>
		/// The status of the stock
		/// True = enabled
		/// False = disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [stock status]; otherwise, <c>false</c>.
		/// </value>
		bool StockStatus { get; }

		/// <summary>
		/// The status of backorder
		/// True = enabled
		/// False = disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [backorder status]; otherwise, <c>false</c>.
		/// </value>
		bool BackorderStatus { get; }

		/// <summary>
		/// Gets a value indicating whether [use variant stock].
		/// </summary>
		/// <value>
		///   <c>true</c> if [use variant stock]; otherwise, <c>false</c>.
		/// </value>
		bool UseVariantStock { get; }


		/// <summary>
		/// Gets all variants.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IProductVariant> GetAllVariants();

		/// <summary>
		/// Gets the sale of the pricing
		/// </summary>
		/// <value>
		/// The product discount.
		/// </value>
		IProductDiscount Discount { get; }

		/// <summary>
		/// The groups with variants for this product
		/// </summary>
		/// <value>
		/// The product variant groups.
		/// </value>
		IEnumerable<IProductVariantGroup> VariantGroups { get; }

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
		bool Orderable { get; }

		/// <summary>
		/// Returns if the pricing is discounted
		/// </summary>
		/// <value>
		///   <c>true</c> if [is discounted]; otherwise, <c>false</c>.
		/// </value>
		bool IsDiscounted { get; }

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <value>
		/// The url.
		/// </value>
		string Url { get; }

		/// <summary>
		/// Gets the title of the content
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets the short description of the content
		/// </summary>
		/// <value>
		/// The meta description.
		/// </value>
		string MetaDescription { get; }

		/// <summary>
		/// Gets the long description of the content
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description { get; }

		/// <summary>
		/// Gets the template of the product
		/// </summary>
		/// <value>
		/// The template.
		/// </value>
		int Template { get; }

		/// <summary>
		/// The url for this product
		/// </summary>
		/// <returns></returns>
		string NiceUrl();

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		string NiceUrl(string storeAlias);

		/// <summary>
		/// The url for this product based on the current store
		/// </summary>
		/// <param name="getCanonicalUrl">if set to <c>true</c> get the canonical URL.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		string NiceUrl(bool getCanonicalUrl, string storeAlias = null, string currencyCode = null);

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns></returns>
		string GetProperty(string propertyAlias);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IProductLegacyPriceFields
	{
		/// <summary>
		/// Gets the original price in cents.
		/// </summary>
		/// <value>
		/// The original price in cents.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.ValueInCents")]
		int OriginalPriceInCents { get; }

		/// <summary>
		/// Gets the price in cents.
		/// </summary>
		/// <value>
		/// The price in cents.
		/// </value>
		[Obsolete("Use Price.ValueInCents")]
		int PriceInCents { get; }

		/// <summary>
		/// The active tax amount in cents
		/// </summary>
		/// <value>
		/// The vat amount  in  cents.
		/// </value>
		[Obsolete("Use Price.Vat.ValueInCents")]
		int VatAmountInCents { get; }

		/// <summary>
		/// The discounted tax amount
		/// </summary>
		/// <value>
		/// The discounted vat amount.
		/// </value>
		[Obsolete("Use Price.Vat with .Value or .ToCurrencyString")]
		decimal DiscountedVatAmount { get; }

		/// <summary>
		/// Tax amount
		/// </summary>
		/// <value>
		/// The original vat amount.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.Vat with .Value or .ToCurrencyString")]
		decimal OriginalVatAmount { get; }

		/// <summary>
		/// The discounted price with tax
		/// </summary>
		/// <value>
		/// The discounted price with vat.
		/// </value>
		[Obsolete("Use Price.WithVat with .Value or .ToCurrencyString")]
		decimal DiscountedPriceWithVat { get; }

		/// <summary>
		/// Gets the discounted price without vat.
		/// </summary>
		/// <value>
		/// The discounted price without vat.
		/// </value>
		[Obsolete("Use Price.WithoutVat with .Value or .ToCurrencyString")]
		decimal DiscountedPriceWithoutVat { get; }

		/// <summary>
		/// The price of the pricing without VAT
		/// </summary>
		/// <value>
		/// The original price without vat.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.WithoutVat with .Value or .ToCurrencyString")]
		decimal OriginalPriceWithoutVat { get; }

		/// <summary>
		/// The active price of the item without tax
		/// Gives the discounted price when the pricing is discounted
		/// Gives the normal price when the pricing isn't discounted
		/// </summary>
		/// <value>
		/// The price without vat.
		/// </value>
		[Obsolete("Use Price.WithoutVat with .Value or .ToCurrencyString")]
		decimal PriceWithoutVat { get; }

		/// <summary>
		/// The price of the pricing with VAT
		/// </summary>
		/// <value>
		/// The original price with vat.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.WithVat with .Value or .ToCurrencyString")]
		decimal OriginalPriceWithVat { get; }

		/// <summary>
		/// The active price of the item with tax
		/// Gives the discounted price when the pricing is discounted
		/// Gives the normal price when the pricing isn't discounted
		/// </summary>
		/// <value>
		/// The price with vat.
		/// </value>
		[Obsolete("Use Price.WithVat with .Value or .ToCurrencyString")]
		decimal PriceWithVat { get; }

		/// <summary>
		/// The active tax amount
		/// </summary>
		/// <value>
		/// The vat amount.
		/// </value>
		[Obsolete("Use Price.Vat with .Value or .ToCurrencyString")]
		decimal VatAmount { get; }

		/// <summary>
		/// Tax amount
		/// </summary>
		/// <value>
		/// The original vat amount in cents.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.Vat.ValueInCents")]
		int OriginalVatAmountInCents { get; }

		/// <summary>
		/// The discounted tax amount
		/// </summary>
		/// <value>
		/// The discounted vat amount in cents.
		/// </value>
		[Obsolete("Use Price.Vat.ValueInCents")]
		int DiscountedVatAmountInCents { get; }

		/// <summary>
		/// Gets the discounted price with vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price with vat in cents.
		/// </value>
		[Obsolete("Use Price.WithVat.ValueInCents")]
		int DiscountedPriceWithVatInCents { get; }

		/// <summary>
		/// Gets or sets the discounted price without vat in cents.
		/// </summary>
		/// <value>
		/// The discounted price without vat in cents.
		/// </value>
		[Obsolete("Use Price.WithoutVat.ValueInCents")]
		int DiscountedPriceWithoutVatInCents { get; }

		/// <summary>
		/// Gets the price with vat in cents.
		/// </summary>
		/// <value>
		/// The price with vat in cents.
		/// </value>
		[Obsolete("Use Price.WithVat.ValueInCents")]
		int PriceWithVatInCents { get; }

		/// <summary>
		/// Gets the price without vat in cents.
		/// </summary>
		/// <value>
		/// The price without vat in cents.
		/// </value>
		[Obsolete("Use Price.WithoutVat.ValueInCents")]
		int PriceWithoutVatInCents { get; }

		/// <summary>
		/// The price of the pricing without VAT
		/// </summary>
		/// <value>
		/// The original price without vat in cents.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.WithoutVat.ValueInCents")]
		int OriginalPriceWithoutVatInCents { get; }

		/// <summary>
		/// Gets the original price with vat in cents.
		/// </summary>
		/// <value>
		/// The original price with vat in cents.
		/// </value>
		[Obsolete("Use Price.BeforeDiscount.WithVat.ValueInCents")]
		int OriginalPriceWithVatInCents { get; }
	}
}