using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using uWebshop.API;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IProductVariant : IUwebshopSortableEntity
	{
		/// <summary>
		/// Is this product variant orderable:
		/// stock should be higher then 0
		/// if stock is lower then 0, but backorder is enabled
		/// if the stockstatus is disabled
		/// </summary>
		/// <value>
		///   <c>true</c> if [orderable]; otherwise, <c>false</c>.
		/// </value>
		bool Orderable { get; }

		/// <summary>
		/// Gets the sale of the pricing
		/// </summary>
		/// <value>
		/// The product variant discount.
		/// </value>
		IProductDiscount Discount { get; }

		/// <summary>
		/// Returns if the pricing is discounted
		/// </summary>
		/// <value>
		///   <c>true</c> if [is discounted]; otherwise, <c>false</c>.
		/// </value>
		bool IsDiscounted { get; }

		/// <summary>
		/// Gets or sets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		decimal Vat { get; }

		/// <summary>
		/// Gets the title of the content
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// ProductVariant SKU
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		string SKU { get; }

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		/// <value>
		/// The group.
		/// </value>
		string Group { get; }

		/// <summary>
		/// Gets or sets a value indicating whether [required variant].
		/// </summary>
		/// <value>
		///   <c>true</c> if [required variant]; otherwise, <c>false</c>.
		/// </value>
		bool Required { get; }

		/// <summary>
		/// Gets the long description of the content
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description { get; }

        /// <summary>
        /// Color of the product
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        string Color();

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
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		IDiscountedRangedPrice Price { get; }

		/// <summary>
		/// Gets the price including the product.
		/// </summary>
		/// <value>
		/// The price including the product.
		/// </value>
		IDiscountedRangedPrice PriceIncludingProduct(IProduct product);

		/// <summary>
		/// Ranges from the range data type
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		IEnumerable<Range> Ranges { get; }

		/// <summary>
		/// Gets the stock of the variant
		/// </summary>
		/// <value>
		/// The stock.
		/// </value>
		int Stock { get; }

		/// <summary>
		/// Gets the number of times this ProductVariant is ordered
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
		/// Gets the property.
		/// </summary>
		/// <param name="propertyAlias">The property alias.</param>
		/// <returns></returns>
		string GetProperty(string propertyAlias);

        /// <summary>
        /// Gets the propertyValue.
        /// </summary>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns></returns>
        T GetPropertyValue<T>(string propertyAlias);
    }
}