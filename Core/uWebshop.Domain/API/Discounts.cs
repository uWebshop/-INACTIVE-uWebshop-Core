using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public static class Discounts
	{
		/// <summary>
		/// Gets all order discounts.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IOrderDiscount> GetAllOrderDiscounts(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IOrderDiscountService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Select(d => new DiscountAdaptor(d));
		}

		/// <summary>
		/// Gets all discounts for the given order
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<IOrderDiscount> GetDiscountsForOrder(string orderGuid, string storeAlias = null, string currencyCode = null)
		{
			var order = Basket.GetBasket(orderGuid);
			if (order == null)
			{
				return Enumerable.Empty<IOrderDiscount>();
			}
			var orderInfo = Basket.GetOrderInfoFromOrderBasket(order);
			return IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(orderInfo, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Select(d => new DiscountAdaptor(d));
		}

		/// <summary>
		/// Gets all discounts for the given order
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<IOrderDiscount> GetDiscountsForOrder(IBasket order, string storeAlias = null, string currencyCode = null)
		{
			var orderInfo = Basket.GetOrderInfoFromOrderBasket(order);
			return IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(orderInfo, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Select(d => new DiscountAdaptor(d));
		}

		/// <summary>
		/// Gets the discount for product.
		/// </summary>
		/// <param name="productId">The product unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProductDiscount> GetDiscountForProduct(int productId, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductDiscountService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Where(p => p.Products.Any(x => x.Id == productId));
		}

		/// <summary>
		/// Gets the discount for product.
		/// </summary>
		/// <param name="product">The product.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">product</exception>
		public static IEnumerable<IProductDiscount> GetDiscountForProduct(IProduct product, string storeAlias = null, string currencyCode = null)
		{
			if (product == null) throw new ArgumentNullException("product");
			return GetDiscountForProduct(product.Id, storeAlias, currencyCode);
		}

		/// <summary>
		/// Gets the discount for product variant.
		/// </summary>
		/// <param name="variantId">The variant unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IProductDiscount> GetDiscountForProductVariant(int variantId, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IProductDiscountService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Where(p => p.ProductVariants.Any(x => x.Id == variantId));
		}

		/// <summary>
		/// Gets the discount for product variant.
		/// </summary>
		/// <param name="variant">The variant.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">variant</exception>
		public static IEnumerable<IProductDiscount> GetDiscountForProductVariant(IProductVariant variant, string storeAlias = null, string currencyCode = null)
		{
			if (variant == null) throw new ArgumentNullException("variant");
			return GetDiscountForProduct(variant.Id, storeAlias, currencyCode);
		}
	}

	class DiscountAdaptor : IOrderDiscount
	{
		private readonly Domain.Interfaces.IOrderDiscount _orderDiscount;

		public DiscountAdaptor(Domain.Interfaces.IOrderDiscount orderDiscount)
		{
			_orderDiscount = orderDiscount;
		}

		public IEnumerable<string> CouponCodes { get; private set; }
		public int Id { get { return _orderDiscount.Id; } }
		public string TypeAlias { get { return _orderDiscount.TypeAlias; } }
		public bool Disabled { get { return _orderDiscount.Disabled; } }
		public DateTime CreateDate { get { return _orderDiscount.CreateDate; } }
		public DateTime UpdateDate { get { return _orderDiscount.UpdateDate; } }
		public int SortOrder { get { return _orderDiscount.SortOrder; } }
        public string Description { get { return _orderDiscount.Description; } }
	    public bool OncePerCustomer { get { return _orderDiscount.OncePerCustomer; } }
		public bool CounterEnabled { get { return _orderDiscount.CounterEnabled; } }
		public int Counter { get { return _orderDiscount.Id; } }
		public DiscountType Type { get { return _orderDiscount.Type; } }
		public List<int> RequiredItemIds { get { return _orderDiscount.RequiredItemIds; } }
		public DiscountOrderCondition Condition { get { return _orderDiscount.Condition; } }
		public int NumberOfItemsCondition { get { return _orderDiscount.NumberOfItemsCondition; } }
		public int DiscountValue { get { return _orderDiscount.DiscountValue; } }
		public List<Range> Ranges { get { return _orderDiscount.Ranges; } }
		public DiscountType DiscountType { get { return _orderDiscount.DiscountType; } }
		public List<string> MemberGroups { get { return _orderDiscount.MemberGroups; } }
		public List<int> AffectedOrderlines { get { return _orderDiscount.AffectedOrderlines; } }
		public IEnumerable<string> AffectedProductTags { get { return _orderDiscount.AffectedProductTags; } }
		public string Title { get { return _orderDiscount.Title; } }
		public IVatPrice MinimumOrderAmount { get { return _orderDiscount.MinimumOrderAmount; } }
		public bool IncludeShippingInOrderDiscountableAmount { get { return _orderDiscount.IncludeShippingInOrderDiscountableAmount; } }
		//public int OriginalId { get { return _orderDiscount.Id; } }
	}
}
