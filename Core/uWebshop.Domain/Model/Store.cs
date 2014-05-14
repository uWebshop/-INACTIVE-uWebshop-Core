using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain
{
	/// <summary>
	/// Class representing a language in Umbraco
	/// </summary>
    [ContentType(ParentContentType = typeof(StoreRepositoryContentType), Name = "Store", Description = "#StoreDescription", Alias = "uwbsStore", IconClass = IconClass.store, Icon = ContentIcon.Store, Thumbnail = ContentThumbnail.Folder)]
	public class Store : uWebshopEntity, IStoreInternal
	{
		public Store()
		{
			
		}
		/// <summary>
		/// The node alias
		/// </summary>
		public static string NodeAlias;

		/// <summary>
		/// The store repository node alias
		/// </summary>
		public static string StoreRepositoryNodeAlias { get { return StoreRepositoryContentType.NodeAlias; } }

		private const string AllDomainsCacheKey = "AllDomainsCacheKey";

		private static List<StoreDomain> AllDomains
		{
			get
			{
				if (HttpContext.Current == null) return new List<StoreDomain>();
				var domains = (List<StoreDomain>) HttpContext.Current.Cache[AllDomainsCacheKey];
				if (domains == null)
				{
					// todo: load from table
					domains = new List<StoreDomain>();
				}
				return domains;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return "Store: " + Alias;
		}

		#region properties

		/// <summary>
		/// Gets the store culture
		/// </summary>
		/// <value>
		/// The culture.
		/// </value>
		[ContentPropertyType(Alias = "storeCulture", DataType = DataType.Cultures, Tab = ContentTypeTab.Global, Name = "#StoreCulture", Description = "#StoreCultureDescription")]
		public string Culture { get; set; }

		/// <summary>
		/// Get the shop alias
		/// </summary>
		/// <value>
		/// The alias.
		/// </value>
		public virtual string Alias
		{
			get { return Name; }
			protected internal set { _nodeName = value; }
		}

		/// <summary>
		/// Gets the country code
		/// </summary>
		/// <value>
		/// The country code.
		/// </value>
		[ContentPropertyType(Alias = "countryCode", DataType = DataType.Countries, Tab = ContentTypeTab.Global, Name = "#CountryCode", Description = "#CountryCodeDescription")]
		public string CountryCode { get; set; }

		/// <summary>
		/// Gets or sets the default country code.
		/// </summary>
		/// <value>
		/// The default country code.
		/// </value>
		public virtual string DefaultCountryCode { get; set; }

		/// <summary>
		/// Returns the currency culture code (en-US)
		/// </summary>
		/// <value>
		/// The currency culture.
		/// </value>
		public string CurrencyCulture { get; set; }

		/// <summary>
		/// Gets the currency codes.
		/// </summary>
		/// <value>
		/// The currency codes.
		/// </value>
		[ContentPropertyType(Alias = "currencies", DataType = DataType.Currencies, Tab = ContentTypeTab.Global, Name = "#Currencies", Description = "#CurrenciesDescription")]
		public IEnumerable<string> CurrencyCodes { get; set; }

		/// <summary>
		/// Gets or sets the currencies.
		/// </summary>
		/// <value>
		/// The currencies.
		/// </value>
		public IEnumerable<ICurrency> Currencies { get; set; }

		/// <summary>
		/// Returns the CultureInfo based on the CurrencyCulture
		/// </summary>
		/// <value>
		/// The currency culture information.
		/// </value>
		public CultureInfo CurrencyCultureInfo
		{
			// todo weird but API compatibility
			get { return StoreHelper.GetCurrencyCulture(StoreHelper.CurrentLocalization); }
		}

		/// <summary>
		/// Gets the default currency culture information.
		/// </summary>
		/// <value>
		/// The default currency culture information.
		/// </value>
		public CultureInfo DefaultCurrencyCultureInfo
		{
			//todo
			get { return CurrencyCulture != null && !string.IsNullOrEmpty(Culture) ? new CultureInfo(Culture) : new CultureInfo("en-US"); }
		}

		/// <summary>
		/// Gets the currency culture symbol.
		/// </summary>
		/// <value>
		/// The currency culture symbol.
		/// </value>
		public string CurrencyCultureSymbol
		{
			// todo weird but API compatibility
			get { return new RegionInfo(CurrencyCultureInfo.LCID).ISOCurrencySymbol; }
		}

		/// <summary>
		/// Gets the default currency culture symbol.
		/// </summary>
		/// <value>
		/// The default currency culture symbol.
		/// </value>
		public string DefaultCurrencyCultureSymbol
		{
			get
			{
				var firstOrDefault = Currencies.FirstOrDefault(x => x.Ratio == 1);
				if (firstOrDefault != null)
				{
					return firstOrDefault.ISOCurrencySymbol;
				}

				return CurrencyCodes.FirstOrDefault();
			}
		}

		/// <summary>
		/// Gets a System.Globalization.CultureInfo object that is set to the languagecode and countrycode
		/// </summary>
		/// <value>
		/// The culture information.
		/// </value>
		public CultureInfo CultureInfo
		{
			get {
				return !string.IsNullOrEmpty(Culture) ? new CultureInfo(Culture) : new CultureInfo("en-US");
			}
		}

		/// <summary>
		/// Global Vat for all items in the store, when no VAT is specified
		/// </summary>
		/// <value>
		/// The global vat.
		/// </value>
		[ContentPropertyType(Alias = "globalVat", DataType = DataType.VatPicker, Tab = ContentTypeTab.Global, Name = "#GlobalVat", Description = "#GlobalVatDescription")]
		public decimal GlobalVat { get; set; }

		public bool Testmode { get; private set; }

		/// <summary>
		/// Prefix value to put before the order numbers
		/// </summary>
		/// <value>
		/// The order number prefix.
		/// </value>
		[ContentPropertyType(Alias = "orderNumberPrefix", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#OrderNumberPrefix", Description = "#OrderNumberPrefixDescription")]
		public string OrderNumberPrefix { get; set; }

		/// <summary>
		/// Use a template to generate the order numbers
		/// </summary>
		/// <value>
		/// The order number template.
		/// </value>
		[ContentPropertyType(Alias = "orderNumberTemplate", DataType = DataType.String, Tab = ContentTypeTab.Global, Name = "#OrderNumberTemplate", Description = "#OrderNumberTemplateDescription")]
		public string OrderNumberTemplate { get; set; }

		/// <summary>
		/// Use a template to generate the order numbers
		/// </summary>
		/// <value>
		/// The order number start number.
		/// </value>
		[ContentPropertyType(Alias = "orderNumberStartNumber", DataType = DataType.Numeric, Tab = ContentTypeTab.Global, Name = "#OrderNumberStartNumber", Description = "#OrderNumberStartNumberDescription")]
		public int OrderNumberStartNumber { get; set; }

		/// <summary>
		/// Is stock enabled for this store?
		/// </summary>
		/// <value>
		///   <c>true</c> if [use stock]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "enableStock", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Global, Name = "#EnableStock", Description = "#EnableStockDescription")]
		public bool UseStock { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this store uses variant stock by default.
		/// </summary>
		/// <value>
		///   <c>true</c> if this store uses variant stock by default; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "defaultUseVariantStock", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Global, Name = "#UseVariantStock", Description = "#UseVariantStockDescription")]
		public bool UseVariantStock { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this store uses variant stock by default.
		/// </summary>
		/// <value>
		///   <c>true</c> if this store uses variant stock by default; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "defaultCountdownEnabled", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Global, Name = "#CountdownEnabled", Description = "#CountdownEnabledDescription")]
		public bool UseCountdown { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [enable stock].
		/// </summary>
		/// <value>
		///   <c>true</c> if [enable stock]; otherwise, <c>false</c>.
		/// </value>
		[Obsolete("use UseStock")]
		public bool enableStock
		{
			get { return UseStock; }
			set { }
		}

		/// <summary>
		/// Use the store specific stock _StoreAlias instead of global stock
		/// </summary>
		/// <value>
		/// <c>true</c> if [use store specific stock]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "storeStock", DataType = DataType.TrueFalse, Tab = ContentTypeTab.Global, Name = "#StoreStock", Description = "#StoreStockDescription")]
		public bool UseStoreSpecificStock { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [use store stock].
		/// </summary>
		/// <value>
		///   <c>true</c> if [use store stock]; otherwise, <c>false</c>.
		/// </value>
		[Obsolete("use UseStoreSpecificStock")]
		public bool UseStoreStock
		{
			get { return UseStoreSpecificStock; }
			set { }
		}

		/// <summary>
		/// Use the store specific stock _StoreAlias instead of global stock
		/// </summary>
		/// <value>
		///   <c>true</c> if [use backorders]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "useBackorders", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Global, Name = "#UseBackorders", Description = "#UseBackordersDescription")]
		public bool UseBackorders { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [enable testmode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [enable testmode]; otherwise, <c>false</c>.
		/// </value>
		[ContentPropertyType(Alias = "enableTestmode", DataType = DataType.EnableDisable, Tab = ContentTypeTab.Global, Name = "#EnableTestmode", Description = "#EnableTestmodeDescription")]
		public bool EnableTestmode { get; set; }

		// todo: dit doet niets en staat ook op settings? De bedoeling hier een per store setting van te maken?
		//[ContentPropertyType(Alias = "incompleteOrderLifetime", DataType = DataType.Nummeric, Tab = ContentTypeTab.Global, Name = "#IncompleteOrderLifetime", Description = "#IncompleteOrderLifetimeDescription")]
		/// <summary>
		/// Gets the incomple order lifetime.
		/// </summary>
		/// <value>
		/// The incomple order lifetime.
		/// </value>
		public double IncompleOrderLifetime { get; set; }

		/// <summary>
		/// Gets or sets the store URL without domain.
		/// </summary>
		/// <value>
		/// The store URL without domain.
		/// </value>
		public string StoreUrlWithoutDomain { get; set; }

		#region EmailProperties

		/// <summary>
		/// Email address to sent the orders from
		/// </summary>
		/// <value>
		/// The email address from.
		/// </value>
		[ContentPropertyType(Alias = "storeEmailFrom", DataType = DataType.String, Tab = ContentTypeTab.Email, Name = "#StoreEmailFrom", Description = "#StoreEmailFromDescription")]
		public string EmailAddressFrom { get; set; }

		/// <summary>
		/// Name to sent the orders
		/// </summary>
		/// <value>
		/// The name of the email address from.
		/// </value>
		[ContentPropertyType(Alias = "storeEmailFromName", DataType = DataType.String, Tab = ContentTypeTab.Email, Name = "#StoreEmailFromName", Description = "#StoreEmailFromNameDescription")]
		public string EmailAddressFromName { get; set; }

		/// <summary>
		/// Email address to sent the orders from
		/// </summary>
		/// <value>
		/// The email address automatic.
		/// </value>
		[ContentPropertyType(Alias = "storeEmailTo", DataType = DataType.String, Tab = ContentTypeTab.Email, Name = "#StoreEmailTo", Description = "#StoreEmailToDescription")]
		public string EmailAddressTo { get; set; }

		/// <summary>
		/// Gets the account created email.
		/// </summary>
		/// <value>
		/// The account created email.
		/// </value>
		[ContentPropertyType(Alias = "accountEmailCreated", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#AccountEmailCreated", Description = "#AccountEmailCreatedDescription")]
		public string AccountCreatedEmail { get; set; }

		/// <summary>
		/// Gets the account forgot password email.
		/// </summary>
		/// <value>
		/// The account forgot password email.
		/// </value>
		[ContentPropertyType(Alias = "accountForgotPassword", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#AccountForgotPassword", Description = "#AccountForgotPasswordDescription")]
		public string AccountForgotPasswordEmail { get; set; }

		/// <summary>
		/// Sent and email to the store after the order is confirmed
		/// </summary>
		/// <value>
		/// The confirmation email store.
		/// </value>
		[ContentPropertyType(Alias = "confirmationEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ConfirmationEmailStore", Description = "#ConfirmationEmailStoreDescription")]
		public string ConfirmationEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the order is confirmed
		/// </summary>
		/// <value>
		/// The confirmation email customer.
		/// </value>
		[ContentPropertyType(Alias = "confirmationEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ConfirmationEmailCustomer", Description = "#ConfirmationEmailCustomerDescription")]
		public string ConfirmationEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is succesfully paid online
		/// </summary>
		/// <value>
		/// The online payment email store.
		/// </value>
		[ContentPropertyType(Alias = "onlinePaymentEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#OnlinePaymentEmailStore", Description = "#OnlinePaymentEmailStoreDescription")]
		public string OnlinePaymentEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is succesfully paid online
		/// </summary>
		/// <value>
		/// The online payment email customer.
		/// </value>
		[ContentPropertyType(Alias = "onlinePaymentEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#OnlinePaymentEmailCustomer", Description = "#OnlinePaymentEmailCustomerDescription")]
		public string OnlinePaymentEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to offline payment
		/// </summary>
		/// <value>
		/// The offline payment email store.
		/// </value>
		[ContentPropertyType(Alias = "offlinePaymentEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#OfflinePaymentEmailStore", Description = "#OfflinePaymentEmailStoreDescription")]
		public string OfflinePaymentEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to offline payment
		/// </summary>
		/// <value>
		/// The offline payment email customer.
		/// </value>
		[ContentPropertyType(Alias = "offlinePaymentEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#OfflinePaymentEmailCustomer", Description = "#OfflinePaymentEmailCustomerDescription")]
		public string OfflinePaymentEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to payment failed
		/// </summary>
		/// <value>
		/// The payment failed email store.
		/// </value>
		[ContentPropertyType(Alias = "paymentFailedEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#PaymentFailedEmailStore", Description = "#PaymentFailedEmailStoreDescription")]
		public string PaymentFailedEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to payment failed
		/// </summary>
		/// <value>
		/// The payment failed email customer.
		/// </value>
		[ContentPropertyType(Alias = "paymentFailedEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#PaymentFailedEmailCustomer", Description = "#PaymentFailedEmailCustomerDescription")]
		public string PaymentFailedEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to dispatched
		/// </summary>
		/// <value>
		/// The dispatched email store.
		/// </value>
		[ContentPropertyType(Alias = "dispatchedEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#DispatchedEmailStore", Description = "#DispatchedEmailStoreDescription")]
		public string DispatchedEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to dispatched
		/// </summary>
		/// <value>
		/// The dispatch email customer.
		/// </value>
		[ContentPropertyType(Alias = "dispatchedEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#DispatchedEmailCustomer", Description = "#DispatchedEmailCustomerDescription")]
		public string DispatchEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to cancelled
		/// </summary>
		/// <value>
		/// The cancel email store.
		/// </value>
		[ContentPropertyType(Alias = "cancelEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#CancelEmailStore", Description = "#CancelEmailStoreDescription")]
		public string CancelEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to cancelled
		/// </summary>
		/// <value>
		/// The cancel email customer.
		/// </value>
		[ContentPropertyType(Alias = "cancelEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#CancelEmailCustomer", Description = "#CancelEmailCustomerDescription")]
		public string CancelEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to closed
		/// </summary>
		/// <value>
		/// The closed email store.
		/// </value>
		[ContentPropertyType(Alias = "closedEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ClosedEmailStore", Description = "#ClosedEmailStoreDescription")]
		public string ClosedEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to closed
		/// </summary>
		/// <value>
		/// The closed email customer.
		/// </value>
		[ContentPropertyType(Alias = "closedEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ClosedEmailCustomer", Description = "#ClosedEmailCustomerDescription")]
		public string ClosedEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to pending
		/// </summary>
		/// <value>
		/// The pending email store.
		/// </value>
		[ContentPropertyType(Alias = "pendingEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#PendingEmailStore", Description = "#PendingEmailStoreDescription")]
		public string PendingEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to pending
		/// </summary>
		/// <value>
		/// The pending email customer.
		/// </value>
		[ContentPropertyType(Alias = "pendingEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#PendingEmailCustomer", Description = "#PendingEmailCustomerDescription")]
		public string PendingEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to (temporary) out of stock
		/// </summary>
		/// <value>
		/// The temporary out of stock email store.
		/// </value>
		[ContentPropertyType(Alias = "temporaryOutOfStockEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#TemporaryOutOfStockEmailStore", Description = "#TemporaryOutOfStockEmailStoreDescription")]
		public string TemporaryOutOfStockEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to (temporary) out of stock
		/// </summary>
		/// <value>
		/// The temporary out of stock email customer.
		/// </value>
		[ContentPropertyType(Alias = "temporaryOutOfStockEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#TemporaryOutOfStockEmailCustomer", Description = "#TemporaryOutOfStockEmailCustomerDescription")]
		public string TemporaryOutOfStockEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to undeliverable
		/// </summary>
		/// <value>
		/// The undeliverable email store.
		/// </value>
		[ContentPropertyType(Alias = "undeliverableEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#UndeliverableEmailStore", Description = "#UndeliverableEmailStoreDescription")]
		public string UndeliverableEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to undeliverable
		/// </summary>
		/// <value>
		/// The undeliverable email customer.
		/// </value>
		[ContentPropertyType(Alias = "undeliverableEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#UndeliverableEmailCustomer", Description = "#UndeliverableEmailCustomerDescription")]
		public string UndeliverableEmailCustomer { get; set; }

		/// <summary>
		/// Sent and email to the store after the orderstatus is set to returned
		/// </summary>
		/// <value>
		/// The returned email store.
		/// </value>
		[ContentPropertyType(Alias = "returnEmailStore", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ReturnEmailStore", Description = "#ReturnEmailStoreDescription")]
		public string ReturnedEmailStore { get; set; }

		/// <summary>
		/// Sent and email to the customer after the orderstatus is set to returned
		/// </summary>
		/// <value>
		/// The returned email customer.
		/// </value>
		[ContentPropertyType(Alias = "returnEmailCustomer", DataType = DataType.ContentPicker, Tab = ContentTypeTab.Email, Name = "#ReturnEmailCustomer", Description = "#ReturnEmailCustomerDescription")]
		public string ReturnedEmailCustomer { get; set; }

		#endregion

		/// <summary>
		/// Gets or sets the store URL.
		/// </summary>
		/// <value>
		/// The store URL.
		/// </value>
		public string StoreURL
		{
			get
			{
				var storeUrlCache = UwebshopRequest.Current.GetStoreUrl(this);

				if (string.IsNullOrEmpty(storeUrlCache))
				{
					return CanonicalStoreURL;
				}

				return storeUrlCache;// UwebshopRequest.Current.GetStoreUrl(this);
			}
			set { UwebshopRequest.Current.SetStoreUrl(this, value); }
		}

		internal string CanonicalStoreURL;

		/// <summary>
		/// Gets the domains.
		/// </summary>
		/// <value>
		/// The domains.
		/// </value>
		public List<string> Domains
		{
			get { return new List<string>(); } // AllDomains.Where(d => d.nodeId == Id); }
		}

		#endregion

		internal static bool IsAlias(string alias)
		{
			return alias.StartsWith(NodeAlias);
		}
	}

	internal class StoreDomain
	{
		/// <summary>
		/// The domain
		/// </summary>
		public string domain;

		/// <summary>
		/// The node unique identifier
		/// </summary>
		public int nodeId;
	}
}