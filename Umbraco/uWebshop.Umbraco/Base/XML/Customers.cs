using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using umbraco;
using uWebshop.Domain.NewtonsoftJsonNet;

namespace uWebshop.API.XML
{
	[XsltExtension("Customers")]
	public class Customers
	{
		/// <summary>
		/// Gets all customers.
		/// </summary>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetAllCustomers()
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Customers.GetAllCustomers();

			dictionary.Add("Customer", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetAllCustomers", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the customers.
		/// </summary>
		/// <param name="group">The group.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <param name="currencyCode">The currency code.</param>
		/// <returns></returns>
		public static XPathNavigator GetCustomers(string group)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Customers.GetCustomers(group);

			dictionary.Add("Customer", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCustomers", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the customers by spending.
		/// </summary>
		/// <param name="amountInCents">The amount in cents.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetCustomersBySpending(int amountInCents, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Customers.GetCustomersBySpending(amountInCents, storeAlias);

			dictionary.Add("Customer", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetCustomersBySpending", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the orders.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetOrders(string userName, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Customers.GetOrders(userName, storeAlias);

			dictionary.Add("Order", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetOrders", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Gets the wishlist for the current membership user, or the current guest
		/// </summary>
		/// <param name="userName">Name of the usern.</param>
		/// <param name="wishlistName">Name of the wishlist.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetWishlist(string wishlistName = null, string storeAlias = null)
		{
			var item = API.Customers.GetWishlist(wishlistName, storeAlias);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetWishlist", item, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the wishlist.
		/// </summary>
		/// <param name="userName">Name of the usern.</param>
		/// <param name="wishlistName">Name of the wishlist.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetWishlist(string userName, string wishlistName = null, string storeAlias = null)
		{
			var item = API.Customers.GetWishlist(userName, wishlistName, storeAlias);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetWishlist", item, true)).CreateNavigator();
		}
		
		/// <summary>
		/// Gets the wishlists.
		/// </summary>
		/// <param name="userName">Name of the usern.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static XPathNavigator GetWishlists(string userName, string storeAlias = null)
		{
			var dictionary = new Dictionary<string, object>();

			var item = API.Customers.GetWishlists(userName, storeAlias);

			dictionary.Add("Wishlist", item);

			return XDocument.Parse(JSONXMLRender.RenderOutput("GetWishlists", dictionary, true)).CreateNavigator();
		}

		/// <summary>
		/// Get the customer field value from from session
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		/// <returns></returns>
		public static string GetCustomerValueFromSession(string fieldName)
		{
			return API.Customers.GetCustomerValueFromSession(fieldName);
		}

		/// <summary>
		/// Get the customer field value from from membership
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		/// <returns></returns>
		public static string GetCustomerValueFromProfile(string fieldName)
		{
			return API.Customers.GetCustomerValueFromProfile(fieldName);
		}


		/// <summary>
		/// Get the customer field value first from session then from membership
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		/// <returns></returns>
		public static string GetCustomerValueFromSessionOrProfile(string fieldName)
		{
			return API.Customers.GetCustomerValueFromSessionOrProfile(fieldName);
		}

		/// <summary>
		/// Get the customer field value first from from session, then from the current saved basket and finally from membership
		/// </summary>
		/// <param name="fieldName">the property field name</param>
		/// <returns></returns>
		public static string GetCustomerValueFromSessionOrBasketOrProfile(string guidAsString, string fieldName)
		{
			return API.Customers.GetCustomerValueFromSessionOrBasketOrProfile(guidAsString, fieldName);
		}
	}
}
