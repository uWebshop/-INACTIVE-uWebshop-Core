using System;
using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	/// <summary>
	/// 
	/// </summary>
	public class Providers
	{
		/// <summary>
		/// Gets all payment providers.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IBillingProvider> GetAllPaymentProviders(string storeAlias = null,
			string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;
			return
				IO.Container.Resolve<IPaymentProviderService>()
					.GetAll(localization)
					.Select(s => new BillingFulfillmentAdaptor(s, inclVat, localization));
		}

		/// <summary>
		/// Gets the payment providers for the given basket 
		/// </summary>
		/// <param name="order">the order to match the payment providers with</param>
		/// <param name="useZone">if set to <c>true</c> [use zone].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IBillingProvider> GetPaymentProvidersForOrder(IBasket order, bool useZone = true,
			string storeAlias = null, string currencyCode = null)
		{
			var orderInfo = Basket.GetOrderInfoFromOrderBasket(order);
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			return
				PaymentProviderHelper.GetBillingProvidersForOrder(orderInfo, useZone, storeAlias, currencyCode)
					.Select(s => new BillingFulfillmentAdaptor(s, orderInfo.PricesAreIncludingVAT, localization));
		}


		/// <summary>
		/// Gets the payment providers for a basket by unique order id (Guid)
		/// </summary>
		/// <param name="guidAsString">The guid for the order as string</param>
		/// <param name="useZone">if set to <c>true</c> [use zone].</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IBillingProvider> GetPaymentProvidersForOrder(string guidAsString, bool useZone = true,
			string storeAlias = null, string currencyCode = null)
		{
			Guid guid;
			Guid.TryParse(guidAsString, out guid);

			if (guid != Guid.Empty)
			{
				var order = Basket.GetBasket(guid);
				if (order != null)
				{
					GetPaymentProvidersForOrder(order, useZone, storeAlias, currencyCode);
				}
			}

			return Enumerable.Empty<IBillingProvider>();
		}

		/// <summary>
		/// Gets the payment providers for a given country
		/// </summary>
		/// <param name="countryCode">the code of the country</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IBillingProvider> GetPaymentProvidersForCountry(string countryCode, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);

			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;

			return PaymentProviderHelper.GetAllPaymentProviders(storeAlias, currencyCode).Where(x => x.Zones.Any(y => y.CountryCodes.Contains(countryCode))).Select(p => new BillingFulfillmentAdaptor(p, inclVat, localization));
		}

		/// <summary>
		/// Gets all fulfillment providers. (ie shipping/pickup/etc)
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IFulfillmentProvider> GetAllFulfillmentProviders(string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;
			var currentOrder = OrderHelper.GetOrder();
			return IO.Container.Resolve<IShippingProviderService>().GetAll(localization)
					.Select(s => new ShippingFulfillmentAdaptor(s, inclVat, localization, currentOrder));
		}

		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc) for a basket
		/// </summary>
		/// <param name="order">the order to match the fulfillment providers with</param>
		/// <param name="useZone">if set to <c>true</c> use zone/countries.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IFulfillmentProvider> GetFulfillmentProvidersForOrder(IBasket order, bool useZone = true, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);
			var orderInfo = Basket.GetOrderInfoFromOrderBasket(order);
			var currentOrder = OrderHelper.GetOrder();
			return ShippingProviderHelper.GetShippingProvidersForOrder(orderInfo, useZone, storeAlias, currencyCode)
					.Select(p => new ShippingFulfillmentAdaptor(p, orderInfo.PricesAreIncludingVAT, localization, currentOrder));
		}


		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc) for a basket by Unique Order Id (Guid)
		/// </summary>
		/// <param name="guidAsString">OrderGuidAsString</param>
		/// <param name="useZone">if set to <c>true</c> use zone/countries.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IFulfillmentProvider> GetFulfillmentProvidersForOrder(string guidAsString, bool useZone = true, string storeAlias = null, string currencyCode = null)
		{
			Guid guid;
			Guid.TryParse(guidAsString, out guid);

			if (guid != Guid.Empty)
			{
				var order = Basket.GetBasket(guid);
				if (order != null)
				{
					GetFulfillmentProvidersForOrder(order, useZone, storeAlias, currencyCode);
				}
			}

			return Enumerable.Empty<IFulfillmentProvider>();
		}

		/// <summary>
		/// Gets the fulfillment providers. (ie shipping/pickup/etc) for a given country
		/// </summary>
		/// <param name="countryCode">the code of the country</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<IFulfillmentProvider> GetFulfillmentProvidersForCountry(string countryCode, string storeAlias = null, string currencyCode = null)
		{
			var localization = StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode);

			var inclVat = IO.Container.Resolve<ISettingsService>().IncludingVat;

			return ShippingProviderHelper.GetAllShippingProviders(storeAlias, currencyCode).Where(x => x.Zone.CountryCodes.Contains(countryCode)).Select(p => new ShippingFulfillmentAdaptor(p, inclVat, localization, null));
		}

	}
}