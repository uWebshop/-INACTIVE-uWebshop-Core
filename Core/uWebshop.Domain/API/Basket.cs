using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	public static class Basket
	{
		/// <summary>
		/// Gets the basket.
		/// </summary>
		/// <returns></returns>
		public static IBasket GetBasket()
		{
			OrderHelper.LogThis("ENTRY GetBasket()");
			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				order = OrderHelper.CreateOrder();
			}
			
			return CreateBasketFromOrderInfo(order);
		}

		/// <summary>
		/// Gets the feedback message.
		/// </summary>
		/// <param name="feedbackKey">The feedback key.</param>
		public static string GetFeedbackMessage(string feedbackKey)
		{
			// todo: remove session at some point
			var msg = HttpContext.Current.Session[feedbackKey];
			return msg == null ? null : msg.ToString();
		}
		
		/// <summary>
		/// Gets the basket.
		/// </summary>
		/// <param name="guidAsString">The unique identifier.</param>
		/// <returns></returns>
		public static IBasket GetBasket(string guidAsString)
		{
			OrderHelper.LogThis("ENTRY GetBasket(guidAsString ) " + guidAsString);

			Guid guid;
			Guid.TryParse(guidAsString, out guid);

			return guid != Guid.Empty ? GetBasket(guid) : null;
		}

		/// <summary>
		/// Gets the basket.
		/// </summary>
		/// <param name="guid">The unique identifier.</param>
		/// <returns></returns>
		public static IBasket GetBasket(Guid guid)
		{
			OrderHelper.LogThis("ENTRY GetBasket(Guid ) " + guid);

			var order = OrderHelper.GetOrder(guid);

			if (order.Status == OrderStatus.Incomplete)
			{
				return CreateBasketFromOrderInfo(order);
			}
			return null;
		}

		/// <summary>
		/// Gets the current basket or create a new basket if basket was confirmed.
		/// Use case: customer clicks back button on payment provider
		/// </summary>
		/// <returns></returns>
		public static IBasket GetCurrentOrNewBasket()
		{
			OrderHelper.LogThis("ENTRY GetCurrentOrNewBasket()");
			return CreateBasketFromOrderInfo(OrderHelper.GetCurrentBasketOrNewIfNotIncomplete());
		}
		
		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc)
		/// </summary>
		/// <param name="useZone">if set to <c>true</c> use zone/countries.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IFulfillmentProvider> GetFulfillmentProviders(bool useZone = true, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;
			var currentOrder = OrderHelper.GetOrder();
			return ShippingProviderHelper.GetShippingProvidersForOrder(currentOrder, useZone, storeAlias, currencyCode).Select(s => new ShippingFulfillmentAdaptor(s, inclVat, localization, currentOrder));
		}
		
		/// <summary>
		/// Gets the payment providers.
		/// </summary>
		/// <param name="useZone">if set to <c>true</c> [use zone].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IBillingProvider> GetPaymentProviders(bool useZone = true, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;
			return PaymentProviderHelper.GetBillingProvidersForOrder(OrderHelper.GetOrder(), useZone, storeAlias, currencyCode).Select(s => new BillingFulfillmentAdaptor(s, inclVat, localization));
		}

		/// <summary>
		/// Gets all order discounts.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<IOrderDiscount> GetAllOrderDiscounts(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IOrderDiscountService>().GetApplicableDiscountsForOrder(OrderHelper.GetOrder(), StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Select(d => new DiscountAdaptor(d));
		}

		internal static IBasket CreateBasketFromOrderInfo(OrderInfo order)
		{
			if (order == null) return null;
			return new BasketOrderInfoAdaptor(order);
		}
		internal static OrderInfo GetOrderInfoFromOrderBasket(IOrderBasketWishlistShared orderBasket)
		{
			if (orderBasket == null) return null;

			var basketOrderAdaptor = orderBasket as BasketOrderInfoAdaptor;
			if (basketOrderAdaptor != null)
			{
				return basketOrderAdaptor.GetOrderInfo();
			}
			OrderHelper.LogThis("ENTRY on impossible situation");
			return OrderHelper.GetOrder(orderBasket.UniqueId);
		}
	}

	internal static class Helpers
	{
		// todo: find proper location for this method
		public static T GetValue<T>(string fieldName, XElement source)
		{
			if (source == null) return default(T);
			var element = source.Element(fieldName);
			if (element != null)
			{
				var val = element.Value;
				return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(val);
			}
			return default(T);
		}
	}
	public static class Extensions
	{
		public static bool HasVariantWithId(this IOrderLine orderLine, int id)
		{
			return orderLine.Product.Variants.Any(usedVariant => id == usedVariant.OriginalId);
		}
	}
}
