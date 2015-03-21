using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	internal class CustomerNetMembershipAdaptor  : ICustomer
	{
		private readonly ProfileBase _profile;

		public CustomerNetMembershipAdaptor(MembershipUser user)
		{
			_profile = ProfileBase.Create(user.UserName);
			UserId = user.ProviderUserKey == null ? string.Empty : user.ProviderUserKey.ToString();
			UserName = user.UserName;
			Email = user.Email;
		}

		public string FirstName { get { return GetValue("customerFirstName"); } }
		public string Name { get { return GetValue("customerName"); } }
		public string LastName { get { return GetValue("customerLastName"); } }
		public string Address1 { get { return GetValue("customerAddress1"); } }
		public string Address2 { get { return GetValue("customerAddress2"); } }
		public string Street { get { return GetValue("customerStreet"); } }
		public string StreetNumber { get { return GetValue("customerStreetNumber"); } }
		public string StreetNumberAddition { get { return GetValue("customerStreetNumberAddition"); } }
		public string City { get { return GetValue("customerCity"); } }
		public string Country { get { return GetValue("customerCountry"); } } // todo?
		public string CountryCode { get { return GetValue("customerCountry"); } }
		public string Region { get { return GetValue("customerRegion"); } }
		public string ZipCode { get { return GetValue("customerZipCode"); } }
		public string Company { get { return GetValue("customerCompany"); } }
		public string Phone { get { return GetValue("customerPhone"); } }
		private string _email;
		public string Email { get { return string.IsNullOrEmpty(_email) ? GetValue("customerEmail") : _email; } set { _email = value; } }
		public T GetValue<T>(string fieldName, bool ignoreCustomerIsShipping = false)
		{
			// todo: implement customer is shipping?
			return (T)_profile.GetPropertyValue(fieldName);
		}
		private string GetValue(string property)
		{
			var value = string.Empty;

			if(ProfileBase.Properties.Cast<SettingsProperty>().Any(x => x.Name.ToLowerInvariant() == property.ToLowerInvariant()))
			{
				var propertyField = _profile.GetPropertyValue(property);

				if (propertyField != null)
				{
					value = propertyField.ToString();
				}
			}

			return value;
		}

		public IAddress Shipping { get; private set; }

		public bool AcceptsMarketing
		{
			get
			{
				var value = GetValue("customerAcceptsMarketing");
				return value == "true" || value == "1";
			}
		}

		public string UserName { get; private set; }
		public string UserId { get; private set; }

		private int? _totalSpending;
		public int TotalSpendingInCents
		{
			get
			{
				if (_totalSpending == null)
				{
					var orders = Orders.GetOrdersForCustomer(UserName);
					//if (!string.IsNullOrEmpty(storeAlias))
					//{
					//	orders = orders.Where(x => x.Store.Alias.ToUpperInvariant() == storeAlias.ToUpperInvariant());
					//}
					_totalSpending = (int)orders.Sum(x => x.ChargedOrderAmount.ValueInCents / x.Localization.Currency.Ratio);
				}
				return _totalSpending.GetValueOrDefault();
			}
			internal set { _totalSpending = value; }
		}

		public bool CustomerIsShipping
		{
			get
			{
				//todo: implement!
				return false;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class Customers
	{
		/// <summary>
		/// Gets all customers.
		/// </summary>
		public static IEnumerable<ICustomer> GetAllCustomers()
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				return (from MembershipUser user in Membership.GetAllUsers() select new CustomerNetMembershipAdaptor(user)).Cast<ICustomer>().ToList();
			}

			return Enumerable.Empty<ICustomer>();
		}

		/// <summary>
		/// Gets the customers.
		/// </summary>
		/// <param name="group">The group.</param>
		public static IEnumerable<ICustomer> GetCustomers(string group)
		{
			if ((IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null) && Roles.RoleExists(@group))
			{
				return Roles.GetUsersInRole(@group).Select(u => new CustomerNetMembershipAdaptor(Membership.GetUser(u)));
			}

			return Enumerable.Empty<ICustomer>();
		}

		/// <summary>
		/// Gets the customers by spending.
		/// </summary>
		/// <param name="amountInCents">The amount in cents.</param>
		/// <param name="storeAlias">The store alias.</param>
		public static IEnumerable<ICustomer> GetCustomersBySpending(int amountInCents, string storeAlias = null)
		{
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || UwebshopRequest.Current.PaymentProvider != null)
			{
				var customers = new List<ICustomer>();
				foreach (var customer in GetAllCustomers())
				{
					var orders = Orders.GetOrdersForCustomer(customer.UserName); // todo!!!!: fix sql in loop
					if (!string.IsNullOrEmpty(storeAlias))
					{
						orders = orders.Where(x => x.Store.Alias.ToUpperInvariant() == storeAlias.ToUpperInvariant());
					}

					var totalspending = orders.Sum(x => x.ChargedOrderAmount.ValueInCents/x.Localization.Currency.Ratio);
					var customerNetMembershipAdaptor = customer as CustomerNetMembershipAdaptor;
					// maybe todo: fix sql in loop (customerNetMembershipAdaptor.TotalSpendingInCents) 
					if (customerNetMembershipAdaptor != null) customerNetMembershipAdaptor.TotalSpendingInCents = (int)totalspending;
					if (totalspending >= amountInCents)
					{
						customers.Add(customer);
					}
				}

				return customers.OrderByDescending(x => x.TotalSpendingInCents);
			}

			return Enumerable.Empty<ICustomer>();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		public static IEnumerable<IOrder> GetOrders(int customerId, string storeAlias = null)
		{
			return Orders.GetOrdersForCustomer(customerId, storeAlias);
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IEnumerable<IOrder> GetOrders(string userName, string storeAlias = null)
		{
			return Orders.GetOrdersForCustomer(userName, storeAlias);
		}

		/// <summary>
		/// Gets the wishlist for the current membership user or the current guest
		/// </summary>
		/// <param name="wishlistName">Name of the wishlist.</param>
		/// <param name="storeAlias">The store alias.</param>
		public static IWishlist GetWishlist(string wishlistName = null, string storeAlias = null)
		{
			var member = Membership.GetUser();
			if (member != null)
			{
				return GetWishlist(member.UserName, wishlistName, storeAlias);
			}

			var cookie = HttpContext.Current.Request.Cookies["StoreInfo"];

			if (cookie != null)
			{
				var wishlistId = cookie["Wishlist"];

				Guid wishGuid;
				Guid.TryParse(wishlistId, out wishGuid);

				if (wishGuid != default(Guid) || wishGuid != Guid.Empty)
				{
					var wishlist = OrderHelper.GetOrder(wishGuid);
					if (wishlist != null && wishlist.Status == OrderStatus.Wishlist)
					{
						return new BasketOrderInfoAdaptor(wishlist);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the wishlist.
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="wishlistName">Name of the wishlist.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IWishlist GetWishlist(int customerId, string wishlistName = null, string storeAlias = null)
		{
			var wishlists = GetWishlists(customerId, storeAlias);
			
			if (!string.IsNullOrEmpty(wishlistName))
			{
				return wishlists.FirstOrDefault(x => x.Name == wishlistName);
			}

			return wishlists.FirstOrDefault();
		}

		/// <summary>
		/// Gets the wishlist.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="wishlistName">Name of the wishlist.</param>
		/// <param name="storeAlias">The store alias.</param>
		// todo: problem in API, hidden by overload
		public static IWishlist GetWishlist(string userName, string wishlistName = null, string storeAlias = null)
		{
			var wishlists = GetWishlists(userName, storeAlias);

			return !string.IsNullOrEmpty(wishlistName) ? wishlists.FirstOrDefault(x => x.Name == wishlistName) : wishlists.FirstOrDefault();
		}

		/// <summary>
		/// Gets the wishlists.
		/// </summary>
		/// <param name="customerId">The customer unique identifier.</param>
		/// <param name="storeAlias">The store alias.</param>
		public static IEnumerable<IWishlist> GetWishlists(int customerId, string storeAlias = null)
		{
			var membershipUser = Membership.GetUser();
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.ProviderUserKey as string == customerId.ToString() || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetWishlistsForCustomer(customerId, storeAlias).Select(Orders.CreateBasketFromOrderInfo).Cast<IWishlist>();
			}

			return Enumerable.Empty<IWishlist>();
		}

		/// <summary>
		/// Gets the wishlists.
		/// </summary>
		/// <param name="userName">Name of the usern.</param>
		/// <param name="storeAlias">The store alias.</param>
		public static IEnumerable<IWishlist> GetWishlists(string userName, string storeAlias = null)
		{
			var membershipUser = Membership.GetUser();
			if (IO.Container.Resolve<ICMSApplication>().IsBackendUserAuthenticated || membershipUser != null && membershipUser.UserName == userName || UwebshopRequest.Current.PaymentProvider != null)
			{
				return OrderHelper.GetWishlistsForCustomer(userName, storeAlias).Select(Orders.CreateBasketFromOrderInfo).Cast<IWishlist>();
			}

			return Enumerable.Empty<IWishlist>();			
		}
		
		/// <summary>
		/// Get the customer field value from from session
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		public static string GetCustomerValueFromSession(string fieldName)
		{
			if (HttpContext.Current != null && HttpContext.Current.Session != null)
			{
				var postedFields = HttpContext.Current.Session[Constants.PostedFieldsKey];

				if (postedFields != null)
				{
					var fields = postedFields as Dictionary<string, string>;

					if (fields == null) return string.Empty;

					var customerFieldFromSession = fields.FirstOrDefault(x => string.Equals(x.Key, fieldName, StringComparison.InvariantCultureIgnoreCase)).Value;
					return !string.IsNullOrEmpty(customerFieldFromSession) ? customerFieldFromSession : string.Empty;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Get the customer field value from from membership
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		public static string GetCustomerValueFromProfile(string fieldName)
		{
			var member = Membership.GetUser();

			if (member == null)
			{
				return string.Empty;
			}

			// todo make configurable?
			if (fieldName.ToLowerInvariant() == "customeremail" || fieldName.ToLowerInvariant() == "email")
			{
				return member.Email;
			}

			var profile = ProfileBase.Create(member.UserName);

			foreach (SettingsProperty prop in ProfileBase.Properties)
			{
				if (!string.Equals(prop.Name, fieldName, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				var value = profile[prop.Name];

				if (value != null)
				{
					return value.ToString();
				}
			}

			return string.Empty;
		}
		
		/// <summary>
		/// Get the customer field value first from session then from membership
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		public static string GetCustomerValueFromSessionOrProfile(string fieldName)
		{
			var sessionValue = GetCustomerValueFromSession(fieldName);

			if (!string.IsNullOrEmpty(sessionValue))
			{
				return sessionValue;
			}

			var memberValue = GetCustomerValueFromProfile(fieldName);

			return !string.IsNullOrEmpty(memberValue) ? memberValue : string.Empty;
		}

		/// <summary>
		/// Get the customer field value first from from session, then from the current saved basket and finally from membership
		/// </summary>
		public static string GetCustomerValueFromSessionOrBasketOrProfile(IBasket basket, string fieldName)
		{
			var sessionValue = GetCustomerValueFromSession(fieldName);

			if (!string.IsNullOrEmpty(sessionValue))
			{
				return sessionValue;
			}

			var basketValue = basket.Customer.GetValue<string>(fieldName);

			if (!string.IsNullOrEmpty(basketValue))
			{
				return basketValue;
			}

			var memberValue = GetCustomerValueFromProfile(fieldName);


			if (!string.IsNullOrEmpty(memberValue))
			{
				return memberValue;
			}

			return string.Empty;
		}
		
		/// <summary>
		/// Get the customer field value first from from session, then from the current saved basket and finally from membership
		/// </summary>
		/// <param name="guidAsString">the basket guid as a string</param>
		/// <param name="fieldName">the property field name</param>
		public static string GetCustomerValueFromSessionOrBasketOrProfile(string guidAsString, string fieldName)
		{
			Guid guid;
			Guid.TryParse(guidAsString, out guid);

			var order = Basket.GetBasket(guid);

			return GetCustomerValueFromSessionOrBasketOrProfile(order, fieldName);
		}
	}
}
