using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Security;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using Umbraco.Core.IO;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace uWebshop.Umbraco.WebApi
{

	[PluginController("uWebshop")]
	[KnownType(typeof(BasketOrderInfoAdaptor))]
	public class StoreApiController : UmbracoAuthorizedApiController
	{
		public IEnumerable<string> GetAllStoreAliasses()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return StoreHelper.GetAllStores().Select(s => s.Alias);
			}

			return Enumerable.Empty<string>();
		}

		public IEnumerable<IStore> GetAllStores()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return StoreHelper.GetAllStores();
			}

			return Enumerable.Empty<IStore>();
		}

		public IEnumerable<string> GetStoreSpecificStockStoreAliasses()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return StoreHelper.GetAllStores().Where(x => x.UseStoreSpecificStock).Select(s => s.Alias);
			}

			return Enumerable.Empty<string>();
		}

		public BasketOrderInfoAdaptor GetOrderData(string guid)
		{
			var order = Orders.GetOrder(guid);

			var membershipUser = Membership.GetUser();

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.UserName == order.Customer.UserName || UwebshopRequest.Current.PaymentProvider != null || OrderHelper.IsCompletedOrderWithinValidLifetime(order))
			{
				return order as BasketOrderInfoAdaptor;
			}

			return null;
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetAllOrders(string status = "All")
		{
			var orders = Orders.GetAllOrders().Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);

			if (string.IsNullOrEmpty(status) || status.ToLowerInvariant() == "all" || status.ToLowerInvariant() == "undefined")
			{
				return orders.Select(o => o as BasketOrderInfoAdaptor);
			}

			OrderStatus orderStatus;
			Enum.TryParse(status, out orderStatus);

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return orders.Where(x => x.Status == orderStatus).Select(o => o as BasketOrderInfoAdaptor);
			}

			return null;
		}

		
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDays(int days, string storeAlias = null)
		{
			var orders = Orders.GetOrders(days, storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}
		
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByStatus(string status, string storeAlias = null)
		{
			OrderStatus orderStatus;
			Enum.TryParse(status, true, out orderStatus); 

			var orders = Orders.GetOrders(orderStatus, storeAlias);

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}
		
		/// <summary>
		/// Get orders based on DateTimeRange
		/// </summary>
		/// <param name="startDateTime">Format: 2015-05-26T16:03:35</param>
		/// <param name="endDateTime">Format: 2015-05-26T16:03:35</param>
		/// <param name="storeAlias"></param>
		/// <returns></returns>
		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDateRange(string startDateTime, string endDateTime = null, string storeAlias = null)
		{
			DateTime startDate;
			DateTime.TryParse(startDateTime, out startDate);
			var endDate = DateTime.Now;
			if (endDateTime != null) DateTime.TryParse(endDateTime, out endDate);

			var orders = Orders.GetOrders(startDate, endDate, storeAlias);

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByDeliveryDateRange(string startDateTime, string endDateTime, string storeAlias = null)
		{
			DateTime startDate;
			DateTime.TryParse(startDateTime, out startDate);
			var endDate = DateTime.Now;
			if (endDateTime != null) DateTime.TryParse(endDateTime, out endDate);

			var orders = Orders.GetOrdersDeliveredBetweenTimes(startDate, endDate, storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersToBeDelivered(int days, string storeAlias = null)
		{
			var startDate = DateTime.Now;
			var endDate = DateTime.Now.AddDays(days);

			var orders = Orders.GetOrdersDeliveredBetweenTimes(startDate.Date, endDate.Date, storeAlias).Where(x => x.Status != OrderStatus.Incomplete && x.Status != OrderStatus.Wishlist);

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}


		public IEnumerable<string> GetEmailTemplates()
		{
			var files = Directory.GetFiles(IOHelper.MapPath(SystemDirectories.Xslt), "*.xslt", SearchOption.AllDirectories).Select(file => file.Replace(IOHelper.MapPath(SystemDirectories.Xslt) + @"\", string.Empty)).ToList();

			files.AddRange(Directory.GetFiles(IOHelper.MapPath(SystemDirectories.MacroScripts), "*.cshtml", SearchOption.AllDirectories).Select(file => file.Replace(IOHelper.MapPath(SystemDirectories.MacroScripts) + @"\", string.Empty)));

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return files.Where(x => x.ToLowerInvariant().Contains("email") || x.ToLowerInvariant().Contains("mail"));
			}

			return null;
		}

		public string GetRenderedTemplate(int nodeId, string template)
		{
			return RazorLibraryExtensions.RenderMacro(template, nodeId);
		}
		
		public int GetOrCreateOrderNode(string uniqueOrderId)
		{
			Guid orderGuid;
			Guid.TryParse(uniqueOrderId, out orderGuid);

			var order = OrderHelper.GetOrder(orderGuid);

			if (order != null && IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				return IO.Container.Resolve<IUmbracoDocumentTypeInstaller>().GetOrCreateOrderContent(order);
			}
			return 0;
		}

		public IEnumerable<CustomerGroup> GetAllMemberGroups()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var membershipRoles = Roles.GetAllRoles();

				return membershipRoles.Select(m => new CustomerGroup { Alias = m });
			}

			return Enumerable.Empty<CustomerGroup>();
		}

		public BasketOrderInfoAdaptor GetOrder(string uniqueOrderId)
		{
			Guid guid;
			Guid.TryParse(uniqueOrderId, out guid);
			
			var order = Orders.GetOrder(guid) as BasketOrderInfoAdaptor;

			var membershipUser = Membership.GetUser();

			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated ||
				membershipUser != null && membershipUser.UserName == order.Customer.UserName ||
				UwebshopRequest.Current.PaymentProvider != null ||
				OrderHelper.IsCompletedOrderWithinValidLifetime(order))
			{
				return order;
			}

			return null;
		}


		public BasketOrderInfoAdaptor GetOrderByNumber(string orderNumber)
		{
			var order = Orders.GetAllOrders()
				.FirstOrDefault(x => x.OrderReference.ToLowerInvariant() == orderNumber.ToLowerInvariant());

			return order as BasketOrderInfoAdaptor;
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByFirstName(string customerFirstName)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.FirstName.ToLowerInvariant() == customerFirstName.ToLowerInvariant());

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByLastName(string customerLastName)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.LastName.ToLowerInvariant() == customerLastName.ToLowerInvariant());

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByEmail(string customerEmail)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.Email.ToLowerInvariant() == customerEmail.ToLowerInvariant());

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public IEnumerable<BasketOrderInfoAdaptor> GetOrdersByCustomerProperty(string property, string value)
		{
			var orders = Orders.GetAllOrders().Where(x => x.Customer.GetValue<string>(property) != null && x.Customer.GetValue<string>(property).ToLowerInvariant() == value.ToLowerInvariant());

			return orders.Select(o => o as BasketOrderInfoAdaptor);
		}

		public string GetCustomerValueFromOrder(string orderGuid, string property)
		{
			var order = Orders.GetOrder(orderGuid);

			var customerValue = order.Customer.GetValue<string>(property,true);
			if (!string.IsNullOrEmpty(customerValue))
			{
				return customerValue;
			}
			var shippingValue = order.Customer.Shipping.GetValue<string>(property, true);
			if (!string.IsNullOrEmpty(shippingValue))
			{
				return shippingValue;
			}
			var extraValue = order.OrderFields.GetValue<string>(property);

			if (!string.IsNullOrEmpty(extraValue))
			{
				return extraValue;
			}

			return string.Empty;
		}

		public class PostStockRequest
		{
			public int id { get; set; }
			public int stock { get; set; }

			public string storealias { get; set; }
		}

		public int PostStock(PostStockRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				string storeAlias = null;
				if (!string.IsNullOrEmpty(data.storealias) && data.storealias.ToLowerInvariant() != "all stores")
				{
					storeAlias = data.storealias;
				}

				IO.Container.Resolve<IStockService>().ReplaceStock(data.id, data.stock, false, storeAlias);

				return data.stock;
			}

			return 0;
		}

		public IEnumerable<Coupon> GetCouponCodes(int discountId)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var couponCodes = IO.Container.Resolve<ICouponCodeService>().GetAllForDiscount(discountId);

				return
					couponCodes.Select(
						coupon =>
							new Coupon
							{
								CouponCode = coupon.CouponCode,
								DiscountId = discountId,
								NumberAvailable = coupon.NumberAvailable
							}).ToList();
			}

			return null;
		}

		public class PostCouponRequest
		{
			public IEnumerable<PostCoupon> Coupons { get; set; }
			public int DiscountId { get; set; }
		}

		public class PostCoupon
		{
			public string couponCode { get; set; }
			public int numberAvailable { get; set; }
		}

		public void PostCouponCodes(PostCouponRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var couponList =
					data.Coupons.Select(
						item =>
							new Coupon
							{
								DiscountId = data.DiscountId,
								CouponCode = item.couponCode,
								NumberAvailable = item.numberAvailable
							}).Cast<ICoupon>().ToList();

				if (couponList.Any())
				{
					IO.Container.Resolve<ICouponCodeService>().Save(data.DiscountId, couponList);
				}
			}
		}

		public class CustomerGroup : ICustomerGroup
		{
			public string Alias { get; set; }
		}


		public class Coupon : ICoupon
		{
			public int DiscountId { get; set; }
			public string CouponCode { get; set; }
			public int NumberAvailable { get; set; }
		}

		public class PostStatusRequest
		{
			public Guid id { get; set; }
			public string status { get; set; }
			public bool emails { get; set; }
		}

		public string PostOrderStatus(PostStatusRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				OrderStatus status;
				Enum.TryParse(data.status, out status);

				order.SetStatus(status, data.emails);
				order.Save();

				return data.status;
			}

			return null;
		}

		public string PostOrder(PostOrderRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				OrderStatus status;
				Enum.TryParse(data.status, out status);
				
				order.SetStatus(status, data.emails);

				order.Fulfilled = data.fulfilled;
				order.Paid = data.paid;
				
				order.Save();

				return data.status;
			}

			return null;
		}

		public class PostOrderRequest
		{
			public Guid id { get; set; }
			public bool paid { get; set; }
			public bool fulfilled { get; set; }
			public string status { get; set; }
			public bool emails { get; set; }
		}

		public class PostPaidRequest
		{
			public Guid id { get; set; }
			public bool paid { get; set; }
		}

		public bool PostPaid(PostPaidRequest data)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated)
			{
				var order = OrderHelper.GetOrder(data.id);

				order.Paid = data.paid;
				order.Save();

				return data.paid;
			}

			return false;
		}

		public IEnumerable<string> GetDiscountTypes()
		{
			return Enum.GetNames(typeof(DiscountType));
		}

		public IEnumerable<string> GetDiscountOrderConditions()
		{
			return Enum.GetNames(typeof(DiscountOrderCondition));
		}

		public IEnumerable<string> GetPaymentProviderAmountTypes()
		{
			return Enum.GetNames(typeof(PaymentProviderAmountType));
		}

		public IEnumerable<string> GetPaymentProviderTypes()
		{
			return Enum.GetNames(typeof(PaymentProviderType));
		}

		public IEnumerable<string> GetShippingProviderTypes()
		{
			return Enum.GetNames(typeof(ShippingProviderType));
		}

		public IEnumerable<string> GetShippingRangeTypes()
		{
			return Enum.GetNames(typeof(ShippingRangeType));
		}

	}

	[PluginController("uWebshop")]
	[KnownType(typeof(BasketOrderInfoAdaptor))]
	public class PublicApiController : UmbracoApiController
	{
		public IEnumerable<CountryData> GetCountrySelector()
		{
			return StoreHelper.GetAllCountries().Select(country => new CountryData(country.Name, country.Code));
		}

		public IEnumerable<Product> GetAllProducts()
		{
			return DomainHelper.GetAllProducts().Cast<Product>();

		}

		public Country GetCountryFromCountryCode(string countryCode, string storeAlias, string currencyCode)
		{
			var localization = StoreHelper.GetLocalization(storeAlias, currencyCode);

			var countries =
				IO.Container.Resolve<ICountryRepository>()
					.GetAllCountries(localization);

			if (countries != null)
			{
				var value = countries.FirstOrDefault(x => x.Code.ToLowerInvariant() == countryCode.ToLowerInvariant());

				return value;
			}

			return null;
		}

		public IEnumerable<Currency> GetCurrencyDataEditor()
		{
			var currencies = GetImportantCurrencyregions().Union(GetAllCurrencyRegions());
			return currencies.Select(c => new Currency { CurrencyEnglishName = c.CurrencyEnglishName, CurrencySymbol = c.CurrencySymbol, ISOCurrencySymbol = c.ISOCurrencySymbol });
		}
		private static IEnumerable<RegionInfo> GetAllCurrencyRegions()
		{
			return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(c => new RegionInfo(c.LCID)).Distinct(ri => ri.ISOCurrencySymbol).OrderBy(ri => ri.ISOCurrencySymbol);
		}

		private static IEnumerable<RegionInfo> GetImportantCurrencyregions()
		{
			var allRegions = GetAllCurrencyRegions().ToDictionary(c => c.ISOCurrencySymbol, c => c);

			return new[] { "EUR", "USD", "GBP", "DKK", "AUD", "NZD", "CHF", "CAD", "JPY" }.Select(c => allRegions[c]);
		}
		
		public IEnumerable<Language> GetLanguagePicker()
		{
			return umbraco.cms.businesslogic.language.Language.GetAllAsList().Select(language =>
			{
				var culture = new CultureInfo(language.CultureAlias);
				if (!culture.IsNeutralCulture)
				{
					var currencyRegion = new RegionInfo(culture.LCID);

					return new Language { CurrencyEnglishName = currencyRegion.CurrencyEnglishName, CurrencySymbol = currencyRegion.CurrencySymbol, ISOCurrencySymbol = currencyRegion.ISOCurrencySymbol, FriendlyName = language.FriendlyName, LanguageId = language.id.ToString(CultureInfo.InvariantCulture) };
				}
				return null;
			}).Where(c => c != null);
		}

		public IEnumerable<string> GetMemberGroups()
		{
			return Roles.GetAllRoles().ToList();
		}

		public int GetOrderCount(int id, string storeAlias = null)
		{
			return IO.Container.Resolve<IStockService>().GetOrderCount(id, storeAlias);
		}

		public int GetStock(int id, string storeAlias = null)
		{
			string storeAliasValue = null;
			if (!string.IsNullOrEmpty(storeAlias) && storeAlias.ToLowerInvariant() != "all stores")
			{
				storeAliasValue = storeAlias;
			}
			return IO.Container.Resolve<IStockService>().GetStockForUwebshopEntityWithId(id, storeAliasValue);
		}

		public IEnumerable<TemplateRequest> GetTemplates(int id)
		{
			var content = Services.ContentService.GetById(id);

			return content.ContentType.AllowedTemplates.Select(template => new TemplateRequest {id = template.Id, name = template.Name, alias = template.Alias}).ToList();
		}

		public IEnumerable<string> GetOrderStatusses()
		{
			return Enum.GetNames(typeof(OrderStatus));
		}

		public class TemplateRequest
		{
			public int id { get; set; }
			public string alias { get; set; }
			public string name { get; set; }
		}
	}

	[Serializable]
	public class Language
	{
		public string LanguageId;
		public string FriendlyName;
		public string ISOCurrencySymbol;
		public string CurrencySymbol;
		public string CurrencyEnglishName;
	}

	[Serializable]
	public class CountryData
	{
		public string Name;
		public string Code;

		public CountryData(string name, string code)
		{
			Name = name;
			Code = code;
		}
	}

	[Serializable]
	public class Currency
	{
		public string ISOCurrencySymbol;
		public string CurrencySymbol;
		public string CurrencyEnglishName;
	}

	internal static class Extensions
	{
		public static IEnumerable<T> Distinct<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> predicate)
		{
			var distinctKeys = new HashSet<TCompare>();
			foreach (var item in items)
			{
				var key = predicate(item);
				if (distinctKeys.Contains(key)) continue;
				distinctKeys.Add(key);
				yield return item;
			}
		}
	}
}