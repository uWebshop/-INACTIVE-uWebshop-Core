using System.Collections.Generic;
using uWebshop.API;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;

namespace uWebshop.RazorExtensions
{
	public static class ExtensionMethods
	{
		public static string NiceUrl(this ICategory category, string storeAlias = null, string currencyCode = null)
		{
            return IO.Container.Resolve<IUrlService>().CategoryUrlUsingCurrentPath(category as Common.Interfaces.ICategory, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
		}

		public static IEnumerable<ICategory> GetSubCategories(this ICategory category)
		{
			return category.SubCategories;
		}
	}

	internal static class IProductExtensions
	{
		//public static int RangedPriceInCents(this IProduct product)
		//{
		//	return product.Price.Ranged.ValueInCents;
		//}

		public static decimal DiscountedPriceWithVat(this IProduct product)
		{
			return product.Price.WithVat.Value;
		}
		//etc

		///// <summary>
		///// The discounted price with tax
		///// </summary>
		///// <value>
		///// The discounted price with vat.
		///// </value>
		//decimal DiscountedPriceWithVat { get; }

		///// <summary>
		///// Gets the discounted price without vat.
		///// </summary>
		///// <value>
		///// The discounted price without vat.
		///// </value>
		//decimal DiscountedPriceWithoutVat { get; }

		///// <summary>
		///// The price of the pricing without VAT
		///// </summary>
		///// <value>
		///// The original price without vat.
		///// </value>
		//decimal OriginalPriceWithoutVat { get; }

		///// <summary>
		///// The active price of the item without tax
		///// Gives the discounted price when the pricing is discounted
		///// Gives the normal price when the pricing isn't discounted
		///// </summary>
		///// <value>
		///// The price without vat.
		///// </value>
		//decimal PriceWithoutVat { get; }

		///// <summary>
		///// The price of the pricing with VAT
		///// </summary>
		///// <value>
		///// The original price with vat.
		///// </value>
		//decimal OriginalPriceWithVat { get; }

		///// <summary>
		///// The active price of the item with tax
		///// Gives the discounted price when the pricing is discounted
		///// Gives the normal price when the pricing isn't discounted
		///// </summary>
		///// <value>
		///// The price with vat.
		///// </value>
		//decimal PriceWithVat { get; }

		///// <summary>
		///// The active tax amount
		///// </summary>
		///// <value>
		///// The vat amount.
		///// </value>
		//decimal VatAmount { get; }

		///// <summary>
		///// Tax amount
		///// </summary>
		///// <value>
		///// The original vat amount in cents.
		///// </value>
		//int OriginalVatAmountInCents { get; }

		///// <summary>
		///// The discounted tax amount
		///// </summary>
		///// <value>
		///// The discounted vat amount in cents.
		///// </value>
		//int DiscountedVatAmountInCents { get; }

		///// <summary>
		///// Gets or sets the discounted price with vat in cents.
		///// </summary>
		///// <value>
		///// The discounted price with vat in cents.
		///// </value>
		//int DiscountedPriceWithVatInCents { get; }

		///// <summary>
		///// Gets or sets the discounted price without vat in cents.
		///// </summary>
		///// <value>
		///// The discounted price without vat in cents.
		///// </value>
		//int DiscountedPriceWithoutVatInCents { get; }

		///// <summary>
		///// Gets or sets the price with vat in cents.
		///// </summary>
		///// <value>
		///// The price with vat in cents.
		///// </value>
		//int PriceWithVatInCents { get; }

		///// <summary>
		///// Gets or sets the price without vat in cents.
		///// </summary>
		///// <value>
		///// The price without vat in cents.
		///// </value>
		//int PriceWithoutVatInCents { get; }

		///// <summary>
		///// The price of the pricing without VAT
		///// </summary>
		///// <value>
		///// The original price without vat in cents.
		///// </value>
		//int OriginalPriceWithoutVatInCents { get; }

		///// <summary>
		///// Gets or sets the original price with vat in cents.
		///// </summary>
		///// <value>
		///// The original price with vat in cents.
		///// </value>
		//int OriginalPriceWithVatInCents { get; }

	}
}
