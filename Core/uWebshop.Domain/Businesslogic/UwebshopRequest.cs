using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Security;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	internal class UwebshopRequest
	{
		internal static IUwebshopRequestService Service;
		
		private readonly Dictionary<Store, string> _storeUrls = new Dictionary<Store, string>();
		
		internal readonly Dictionary<ILocalization, CultureInfo> CurrencyCultures = new Dictionary<ILocalization, CultureInfo>();

		/// <summary>
		/// Gets the current request.
		/// </summary>
		/// <value>
		/// The current request.
		/// </value>
		public static UwebshopRequest Current
		{
			get { return Service.Current; }
		}

		/// <summary>
		/// Gets the current store.
		/// </summary>
		/// <value>
		/// The current store.
		/// </value>
		public Store CurrentStore { get; set; }

		/// <summary>
		/// Gets the current category.
		/// </summary>
		/// <value>
		/// The category.
		/// </value>
		public ICategory Category { get; set; }

		public IEnumerable<ICategory> CategoryPath { get; set; }

		/// <summary>
		/// Gets the current product.
		/// </summary>
		/// <value>
		/// The product.
		/// </value>
		public IProduct Product { get; set; }

		/// <summary>
		/// Gets the current payment provider.
		/// </summary>
		/// <value>
		/// The payment provider.
		/// </value>
		public PaymentProvider PaymentProvider { get; set; }

		/// <summary>
		/// Gets the current order.
		/// </summary>
		/// <value>
		/// The order information.
		/// </value>
		public OrderInfo OrderInfo { get; set; }

		/// <summary>
		/// Gets the localization.
		/// </summary>
		/// <value>
		/// The localization.
		/// </value>
		public ILocalization Localization { get; set; }

		private Lazy<MembershipUser> _user = new Lazy<MembershipUser>(Membership.GetUser);
		public MembershipUser User
		{
			get {
                
                const string key = "LazyMembershipUserPerRequest";

                if (HttpContext.Current.Items[key] == null)
                {
                    HttpContext.Current.Items[key] = _user.Value;
                }

                return (MembershipUser)HttpContext.Current.Items[key];
            }
		}

		/// <summary>
		/// Gets the store URL for given store.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <returns></returns>
		public string GetStoreUrl(Store store)
		{
			string url;
			_storeUrls.TryGetValue(store, out url);
			return url;
		}

		internal void SetStoreUrl(Store store, string url)
		{
			_storeUrls[store] = url;
		}
	}
}