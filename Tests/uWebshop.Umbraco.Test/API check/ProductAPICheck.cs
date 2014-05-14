using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.API;
using uWebshop.Domain.Interfaces;
using Catalog = uWebshop.RazorExtensions.Catalog;

namespace uWebshop.Umbraco.Test.API_check
{
	class ProductAPICheck
	{
		void Check()
		{
			var product = Catalog.Product() as IProductLegacy230;
#pragma warning disable 612,618
			var bla = product.DiscountedPriceWithVat + product.DiscountedPriceWithoutVat + product.DiscountedVatAmount + product.OriginalPriceWithVat +
				product.OriginalPriceWithoutVat + product.OriginalVatAmount + product.PriceWithVat + product.PriceWithoutVat + product.VatAmount;
			var intje = product.DiscountedPriceWithVatInCents + product.DiscountedPriceWithoutVatInCents + product.DiscountedVatAmountInCents +
				product.OriginalPriceInCents + product.OriginalPriceWithVatInCents + product.OriginalPriceWithoutVatInCents + product.OriginalVatAmountInCents +
				product.PriceInCents + product.PriceWithVatInCents + product.PriceWithoutVatInCents + product.VatAmountInCents;
#pragma warning restore 612,618
		}
	}
}
