using System.Linq;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using uWebshop.Domain.Model.OrderInfo;
using uWebshop.Domain.Services;

namespace uWebshop.Domain
{
	interface IAmountUnit
	{
		int GetAmount(bool inclVat, bool discounted, bool ranged);
	}
	public class SellableUnit : IDiscountableUnit, IAmountUnit
	{
		public SellableUnit(ProductInfo product)
		{
			Product = product;
			SellableUnitDiscountEffects = new DiscountEffects();
		}
		public ProductInfo Product;
		internal DiscountEffects SellableUnitDiscountEffects; // includes discount from product and all variants

		internal int PriceInCents
		{
			get { return GetAmount(true, false, true); }
		}
		internal int DiscountedPrice
		{
			get { return GetAmount(true, true, true); }
		}
		internal int DiscountedPriceWithoutVat
		{
			get { return GetAmount(false, true, true); }
		}

		public IDiscountedRangedPrice Price
		{
			get { return new SimplePrice(this, Product.Order.Localization); }
		}

		public DiscountEffects GetDiscountEffects()
		{
			return SellableUnitDiscountEffects;
		}

		public int GetAmount(bool inclVat, bool discounted, bool ranged)
		{
			// todo: DIP
			return new SellableUnitPriceService().GetPrice(new ProductInfoSellableUnitPriceSource(Product, this), inclVat, discounted, true, ranged);
		}

		public int GetOriginalAmount(bool discounted, bool ranged)
		{
			return new SellableUnitPriceService().GetOriginalPrice(new ProductInfoSellableUnitPriceSource(Product, this), discounted, true, ranged);
		}
	}
}