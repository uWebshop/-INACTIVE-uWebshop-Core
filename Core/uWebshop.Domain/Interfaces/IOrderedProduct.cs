using System.Collections.Generic;
using uWebshop.API;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public interface IOrderedProduct
	{
		/// <summary>
		/// Gets the name or alias for the type. (NodeTypeAlias/ContentTypeAlias in Umbraco)
		/// </summary>
		/// <value>
		/// The type alias.
		/// </value>
		string TypeAlias { get; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		string Title { get; }

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>
		/// The price.
		/// </value>
		IDiscountedRangedPrice Price { get; }

		/// <summary>
		/// Gets or sets the sku.
		/// </summary>
		/// <value>
		/// The sku.
		/// </value>
		string SKU { get; }

		/// <summary>
		/// Gets or sets the weight.
		/// </summary>
		/// <value>
		/// The weight.
		/// </value>
		double Weight { get; }

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		double Length { get; }

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>
		/// The height.
		/// </value>
		double Height { get; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>
		/// The width.
		/// </value>
		double Width { get; }

		/// <summary>
		/// Gets or sets the vat.
		/// </summary>
		/// <value>
		/// The vat.
		/// </value>
		decimal Vat { get; }

		/// <summary>
		/// Gets or sets the discount percentage.
		/// </summary>
		/// <value>
		/// The discount percentage.
		/// </value>
		decimal DiscountPercentage { get; }

		/// <summary>
		/// Gets or sets a value indicating whether [discount excluding variants].
		/// </summary>
		/// <value>
		/// <c>true</c> if [discount excluding variants]; otherwise, <c>false</c>.
		/// </value>
		bool DiscountExcludingVariants { get; }

		/// <summary>
		/// Gets or sets the item count.
		/// </summary>
		/// <value>
		/// The item count.
		/// </value>
		int Quantity { get; }

		/// <summary>
		/// Gets the product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		/// <exception cref="System.Exception">ProductInfo without ProductId</exception>
		IProduct CatalogProduct { get; }

		/// <summary>
		/// Gets or sets the ranges.
		/// </summary>
		/// <value>
		/// The ranges.
		/// </value>
		IEnumerable<Range> Ranges { get; }

		/// <summary>
		/// Gets or sets a value indicating whether [is discounted].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is discounted]; otherwise, <c>false</c>.
		/// </value>
		bool IsDiscounted { get; }

		/// <summary>
		/// Gets the original unique identifier.
		/// </summary>
		/// <value>
		/// The original unique identifier.
		/// </value>
		int OriginalId { get; }

		/// <summary>
		/// Gets the variants.
		/// </summary>
		/// <value>
		/// The variants.
		/// </value>
		IEnumerable<IOrderedProductVariant> Variants { get; }
	}
}