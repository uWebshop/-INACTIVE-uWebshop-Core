using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain.Helpers;

namespace uWebshop.Domain.Services
{
	class SellableUnitPriceService
	{
		public int GetOriginalPrice(ISellableUnitPriceSource unit, bool discounted = true, bool sale = true, bool ranged = true)
		{
			var solden = unit.GetPrice(sale, ranged);

			return discounted ? unit.ApplyOrderDiscount(solden) : solden;
		}

		public int GetPrice(ISellableUnitPriceSource unit, bool vat, bool discounted = true, bool sale = true, bool ranged = true)
		{
			var unitDiscounted = GetOriginalPrice(unit, discounted, sale, ranged);

			if (vat && !unit.PricesAreIncludingVAT)
			{
				return VatCalculator.WithVat(unitDiscounted, unit.Vat);
			}
			if (!vat && unit.PricesAreIncludingVAT)
			{
				return VatCalculator.WithoutVat(unitDiscounted, unit.Vat);
			}
			return unitDiscounted;
		}
	}

	internal interface ISellableUnitPriceSource
	{
		int GetPrice(bool saleDiscounted, bool ranged);
		int ApplyOrderDiscount(int price);
		bool PricesAreIncludingVAT { get; }
		decimal Vat { get; }
	}

	class ProductInfoSellableUnitPriceSource : ISellableUnitPriceSource
	{
		private readonly ProductInfo _product;
		private readonly SellableUnit _sellableUnit;

		public ProductInfoSellableUnitPriceSource(ProductInfo product, SellableUnit sellableUnit)
		{
			_product = product;
			_sellableUnit = sellableUnit;
		}

		public int GetPrice(bool saleDiscounted, bool ranged)
		{
			var suBasePrice = _product.GetAmount(saleDiscounted, ranged) + _product.ProductVariants.Sum(v => v.GetAmount(saleDiscounted, ranged));
			var hasSellableUnitSaleDiscount = !_product.DiscountExcludingVariants;
			return hasSellableUnitSaleDiscount ? 
				(int) ((100 - (long) _product.DiscountPercentage)*suBasePrice/100 - _product.DiscountAmountInCents) : suBasePrice;
		}

		public int ApplyOrderDiscount(int price)
		{
			return _sellableUnit.GetDiscountEffects().GetDiscountedPrice(price);
		}

		public bool PricesAreIncludingVAT { get { return _product.Order.PricesAreIncludingVAT; } }
		public decimal Vat { get { return _product.Vat; } }
		

	}
}
