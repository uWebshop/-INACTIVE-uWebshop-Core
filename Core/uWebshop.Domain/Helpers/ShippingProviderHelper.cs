using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	/// Helper class with payment provider related functions
	/// </summary>
	public static class ShippingProviderHelper
	{
		/// <summary>
		/// Returns a list of all shippping providers
		/// </summary>
		/// <returns>
		/// List of shippping providers
		/// </returns>
		public static List<IShippingProvider> GetAllShippingProvidersIncludingCustomProviders()
		{
			// Get custom providers from dlls
			var toReturn = GetShippingProvidersFromDLLs<IShippingProvider>();

			// Add dummy providers from nodes
			var paymentProviders = GetAllShippingProviders().Where(x => x.Type == ShippingProviderType.Pickup);

			toReturn.AddRange(paymentProviders.Select(paymentProvider => new DummyShippingProvider(paymentProvider.Id)));

			return toReturn;
		}

		/// <summary>
		/// Returns allt he shipping providers in the shop
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static IEnumerable<ShippingProvider> GetAllShippingProviders(string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IShippingProviderService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Where(x => !x.Disabled);
		}

		/// <summary>
		///     Add default shipping provider to an order. (the cheapest)
		/// </summary>
		public static void AutoSelectShippingProvider(OrderInfo orderInfo)
		{
			var shippingProvidersForOrder = GetShippingProvidersForOrder(orderInfo);

			if (shippingProvidersForOrder == null || shippingProvidersForOrder.Count == 0)
			{
				Log.Instance.LogDebug("AutoSelectShippingProvider: No shipping providers for order");
				return;
			}

			var shippingMethods = new List<ShippingProviderMethod>();

			foreach (var prov in shippingProvidersForOrder)
			{
				shippingMethods.AddRange(prov.ShippingProviderMethods);
			}

			var shippingProviderMethod = shippingMethods.OrderBy(x => x.PriceInCents).FirstOrDefault();
			var shippingProviderForOrder = shippingProvidersForOrder.FirstOrDefault(x => x.ShippingProviderMethods.Contains(shippingProviderMethod));

			if (shippingProviderForOrder == null)
			{
				Log.Instance.LogDebug("AutoSelectShippingProvider: shippingProviderForOrder == null");
				return;
			}
			if (shippingProviderMethod == null)
			{
				Log.Instance.LogDebug("AutoSelectShippingProvider: No shipping method for " + shippingProviderForOrder.Title);

				return;
			}

			orderInfo.AddShippingProvider(shippingProviderForOrder.Id, shippingProviderMethod.Id);

			orderInfo.ShippingCostsMightBeOutdated = false;
		}

		private static List<T> GetShippingProvidersFromDLLs<T>()
		{
			var instances = new List<T>();

			var targetType = typeof (T);

			foreach (var shippingProvider in GetAllShippingProviders())
			{
				//Log.Add(LogTypes.System, paymentProvider.Id, "GetInterfaces START");

				var dllName = shippingProvider.DLLName;

				if (string.IsNullOrEmpty(dllName))
				{
					dllName = string.Format("uWebshop.Shipping.{0}.dll", shippingProvider.Name);
				}

				var assemblyPathName = string.Format(@"{0}\{1}", HttpContext.Current.Server.MapPath("/bin"), dllName);

				if (System.IO.File.Exists(assemblyPathName))
				{
					var assembly = Assembly.LoadFrom(assemblyPathName);

					if (assembly != null)
					{
						Type[] types = assembly.GetExportedTypes();

						foreach (var type in types)
						{
							if (!targetType.IsAssignableFrom(type)) continue;
							var operation = (T) Activator.CreateInstance(type);

							instances.Add(operation);
						}
					}
				}
			}

			return instances;
		}

		/// <summary>
		/// Gets the shipping zones.
		/// </summary>
		/// <param name="countryCode">The country code.</param>
		/// <returns></returns>
		public static List<Zone> GetShippingZones(string countryCode)
		{
			return DomainHelper.GetObjectsByAlias<Zone>(Zone.ShippingZoneNodeAlias).Where(x => x.CountryCodes.Contains(countryCode)).ToList();
		}

		/// <summary>
		/// Returns all shipping providers that match the current order
		/// </summary>
		/// <param name="useCountry">if set to <c>true</c> [use country].</param>
		/// <returns></returns>
		public static List<ShippingProvider> GetShippingProvidersForOrder(bool useCountry = true)
		{
			var orderInfo = OrderHelper.GetOrder();

			return orderInfo != null ? GetShippingProvidersForOrder(orderInfo, useCountry) : new List<ShippingProvider>();
		}

		/// <summary>
		/// Returns all shipping providers that match the current order
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="useCountry">Use the country information from the order and/or store</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static List<ShippingProvider> GetShippingProvidersForOrder(OrderInfo orderInfo, bool useCountry = true, string storeAlias = null, string currencyCode = null)
		{
			//var providers = Providers.GetFulfillmentProvidersForOrder(Basket.CreateBasketFromOrderInfo(orderInfo), useCountry, storeAlias, currencyCode);
			//return GetAllShippingProviders(storeAlias, currencyCode).Where(a => providers.Any(p => p.Id == a.Id)).ToList();
// todo: move logic and update API
			if (orderInfo == null)
			{
				Log.Instance.LogError("GetShippingProvidersForOrder OrderInfo == null");
				return new List<ShippingProvider>();
			}

            var shippingProviders = GetAllShippingProviders(storeAlias, currencyCode).Where(shippingProvider => shippingProvider.IsApplicableToOrder(orderInfo)).ToList();

			if (useCountry)
			{
				var shippingCountryCode = orderInfo.CustomerInfo.ShippingCountryCode;
				if (string.IsNullOrEmpty(shippingCountryCode))
					shippingCountryCode = orderInfo.CustomerInfo.CountryCode;
				shippingProviders = shippingProviders.Where(shippingProvider => shippingProvider.Type == ShippingProviderType.Pickup || ShippingProviderHasCountyCodeInZone(shippingProvider, shippingCountryCode)).ToList();
			}
			
			// remove all other shipping providers if overrule shipping providers are found
			if (shippingProviders.Any(x => x.Overrule))
			{
				shippingProviders = shippingProviders.Where(shippingProvider => shippingProvider.Overrule).ToList();

			}
			else if (shippingProviders.Any(x => x.TypeOfRange == ShippingRangeType.Weight))
			{
				// Remove all non weight shipping providers if a weight shipping provider is found
				shippingProviders = shippingProviders.Where(shippingProvider => shippingProvider.Type == ShippingProviderType.Pickup || shippingProvider.TypeOfRange == ShippingRangeType.Weight).ToList();
			}

			// Sort payment providers by node sort order in Umbraco backend
			return shippingProviders.OrderBy(a => a.SortOrder).ToList();
		}

		internal static bool ShippingProviderHasCountyCodeInZone(ShippingProvider shippingProvider, string shippingCountryCode)
		{
			return shippingProvider.Zone != null && shippingProvider.Zone.CountryCodes != null && shippingProvider.Zone.CountryCodes.Contains(shippingCountryCode);
		}

		/// <summary>
		/// Gets the shipping provider.
		/// </summary>
		/// <param name="id">The unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static ShippingProvider GetShippingProvider(int id, string storeAlias = null, string currencyCode = null)
		{
			return IO.Container.Resolve<IShippingProviderService>().GetById(id, StoreHelper.GetLocalization(storeAlias, currencyCode) ?? StoreHelper.CurrentLocalization);
		}

		public static void ClearValidationResult(OrderInfo order)
		{
			if (HttpContext.Current.Session[Constants.ShippingValidationResultsKey + order.UniqueOrderId] != null)
				HttpContext.Current.Session.Remove(Constants.ShippingValidationResultsKey + order.UniqueOrderId);
		}

		public static void AddValidationResult(OrderInfo order, int id, string key, string value, string alias = null, string name = null)
		{
			var results = GetPaymentValidationResults(order);
			results.Add(new OrderValidationError { Id = id, Key = key, Value = value, Alias = alias, Name = name });
			HttpContext.Current.Session.Add(Constants.ShippingValidationResultsKey + order.UniqueOrderId, results);
		}
		internal static List<OrderValidationError> GetPaymentValidationResults(OrderInfo order)
		{
			if (HttpContext.Current.Session[Constants.ShippingValidationResultsKey + order.UniqueOrderId] != null)
				return (List<OrderValidationError>)HttpContext.Current.Session[Constants.ShippingValidationResultsKey + order.UniqueOrderId];
			return new List<OrderValidationError>();
		}
	}
}