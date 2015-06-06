using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Linq;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	/// <summary>
	/// 
	/// </summary>
	public class HandleObject
	{
		/// <summary>
		/// Gets or sets the action.
		/// </summary>
		/// <value>
		/// The action.
		/// </value>
		public string Action { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [success].
		/// </summary>
		/// <value>
		///   <c>true</c> if [success]; otherwise, <c>false</c>.
		/// </value>
		public bool Success { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether [validated].
		/// </summary>
		/// <value>
		///   <c>true</c> if [validated]; otherwise, <c>false</c>.
		/// </value>
		public bool Validated { get; set; }
		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>
		/// The item.
		/// </value>
		public Object Item { get; set; }
		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public Uri Url { get; set; }
		/// <summary>
		/// Gets or sets the ExternalURL.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		internal Uri PostConfirmUrl { get; set; }
		/// <summary>
		/// Gets or sets the messages.
		/// </summary>
		/// <value>
		/// The messages.
		/// </value>
		public Dictionary<string, string> Messages { get; set; }
		/// <summary>
		/// Gets posted fields
		/// </summary>
		/// <value>
		/// The messages.
		/// </value>
		public Dictionary<string, object> Fields { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BasketRequestHandler
	{
		/// <summary>
		/// Gets the session.
		/// </summary>
		/// <value>
		/// The session.
		/// </value>
		private static HttpSessionState Session
		{
			get { return HttpContext.Current.Session; }
		}

		/// <summary>
		/// Handles the webshop basket request.
		/// </summary>
		/// <param name="requestParameters">The request parameters.</param>
		/// <returns></returns>
		public bool HandleuWebshopBasketRequest(NameValueCollection requestParameters)
		{
			var request = HttpContext.Current.Request;

			var resultUrl = HandleBasketRequest(requestParameters, request.Url);

			return true;
		}

		/// <summary>
		/// Handles the webshop basket request.
		/// </summary>
		/// <param name="requestParameters">The request parameters.</param>
		/// <param name="rawRequestUrl">The raw request URL.</param>
		/// <returns></returns>
		public List<HandleObject> HandleBasketRequest(NameValueCollection requestParameters, Uri rawRequestUrl)
		{
			ClearFeedbackMessages();

			var fieldsToSession = requestParameters.AllKeys.Where(x => !x.ToLowerInvariant().Contains("password")).ToDictionary(key => key, key => requestParameters[key]);
			Session.Add(Constants.PostedFieldsKey, fieldsToSession);

			var postedFields = new Dictionary<string, object> { { Constants.PostedFieldsKey, fieldsToSession } };
			
			var handleObjectList = new List<HandleObject>();
			
			var currencyQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower().StartsWith("changecurrency"));
			if (currencyQueryStringCollection.Any())
			{
				var result = ChangeCurrency(requestParameters, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> repeatQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && (x.ToLower().StartsWith("repeat") || x.ToLower().StartsWith("shippingdeliverydatetime"))).ToList();

			if (repeatQueryStringCollection.Any())
			{
				var result = AddCustomerInformation(requestParameters, repeatQueryStringCollection, CustomerDatatypes.Repeat, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> shippingQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower().StartsWith("shipping") && x.ToLower() != "shippingprovider").ToList();

			if (shippingQueryStringCollection.Any())
			{
				var result = AddCustomerInformation(requestParameters, shippingQueryStringCollection, CustomerDatatypes.Shipping, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> customerQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower().StartsWith("customer")).ToList();

			if (customerQueryStringCollection.Any())
			{
				var result = AddCustomerInformation(requestParameters, customerQueryStringCollection, CustomerDatatypes.Customer, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> extraQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower().StartsWith("extra")).ToList();

			if (extraQueryStringCollection.Any())
			{
				var result = AddCustomerInformation(requestParameters, extraQueryStringCollection, CustomerDatatypes.Extra, rawRequestUrl);
				handleObjectList.Add(result);
			}

			
			
			List<string> cleanOrderLinesCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "clearbasket").ToList();

			if (cleanOrderLinesCollection.Any())
			{
				var keyValue = requestParameters[cleanOrderLinesCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "clearbasket" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ClearBasket(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> cleanOrderCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "clearorder").ToList();

			if (cleanOrderCollection.Any())
			{
				var keyValue = requestParameters[cleanOrderCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "clearorder" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ClearOrder(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> productQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "productid").ToList();
			List<string> orderlineIdQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "orderlineid").ToList();
			

			if (productQueryStringCollection.Any() || orderlineIdQueryStringCollection.Any())
			{
				var result = AddProduct(requestParameters, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> orderlineDetailsQueryStringCollection = requestParameters.AllKeys.Where(x => x != null).ToList();

			string orderLineIdKey = requestParameters.AllKeys.FirstOrDefault(x => x != null && x.ToLower() == "orderlineid");
			if (orderLineIdKey != null)
			{
				string orderLineIdValue = requestParameters[orderLineIdKey];

				if (orderLineIdValue != null && (orderlineDetailsQueryStringCollection.Any() && !string.IsNullOrEmpty(orderLineIdValue)))
				{
					int orderlineId = 0;

					int.TryParse(orderLineIdValue, out orderlineId);

					if (orderlineId != 0)
					{
						var order = GetCreateOrderOrWishList(requestParameters);

						var orderline = order.OrderLines.FirstOrDefault(x => x.OrderLineId == orderlineId);

						if (orderline != null)
						{
							var result = AddOrderLineDetails(requestParameters, orderlineDetailsQueryStringCollection, orderlineId, rawRequestUrl);
							handleObjectList.Add(result);
						}
					}
				}
			}

			List<string> couponCodeQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "couponcode").ToList();

			if (couponCodeQueryStringCollection.Any())
			{
				var result = AddCoupon(requestParameters, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> shippingProviderQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "shippingprovider").ToList();

			if (shippingProviderQueryStringCollection.Any())
			{
				var result = AddShippingMethod(requestParameters, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> paymentProviderQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "paymentprovider").ToList();

			if (paymentProviderQueryStringCollection.Any())
			{
				var result = AddPaymentMethod(requestParameters, rawRequestUrl);
				handleObjectList.Add(result);
			}

			List<string> createAccountQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "createaccount").ToList();

			if (createAccountQueryStringCollection.Any())
			{
				var keyValue = requestParameters[createAccountQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "createaccount" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = AccountCreate(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> updateAccountQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "updateaccount").ToList();

			if (updateAccountQueryStringCollection.Any())
			{
				var keyValue = requestParameters[updateAccountQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "updateaccount" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = AccountUpdate(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> accountChangePaswordQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "changepassword").ToList();

			if (accountChangePaswordQueryStringCollection.Any())
			{
				var keyValue = requestParameters[accountChangePaswordQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "changepassword" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{

					var result = AccountChangePassword(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> accountSignOutQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "accountsignout").ToList();

			if (accountSignOutQueryStringCollection.Any())
			{
				var keyValue = requestParameters[accountSignOutQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "accountsignout" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = AccountSignOut(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}
			
			List<string> accountSignInQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "accountsignin").ToList();

			if (accountSignInQueryStringCollection.Any())
			{
				var keyValue = requestParameters[accountSignInQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "accountsignin" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = AccountSignIn(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> wishlistCreateQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "createwishlist").ToList();

			if (wishlistCreateQueryStringCollection.Any())
			{
				var keyValue = requestParameters[wishlistCreateQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "createwishlist" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = WishlistCreate(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> wishlistRenameQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "renamewishlist").ToList();

			if (wishlistRenameQueryStringCollection.Any())
			{
				var keyValue = requestParameters[wishlistRenameQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "renamewishlist" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = WishlistRename(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> orderToBasketQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "ordertobasket").ToList();

			if (orderToBasketQueryStringCollection.Any())
			{
				var keyValue = requestParameters[orderToBasketQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "ordertobasket" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = OrderToBasket(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> wishlistToBasketQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "wishlisttobasket").ToList();

			if (wishlistToBasketQueryStringCollection.Any())
			{
				var keyValue = requestParameters[wishlistToBasketQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "wishlisttobasket" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = WishlistToBasket(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}



			List<string> wishlistRemoveQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "removewishlist").ToList();

			if (wishlistRemoveQueryStringCollection.Any())
			{
				var keyValue = requestParameters[wishlistRemoveQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "removewishlist" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = WishlistRemove(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> accountRequestPaswordQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "requestpassword").ToList();

			if (accountRequestPaswordQueryStringCollection.Any())
			{
				var keyValue = requestParameters[accountRequestPaswordQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "requestpassword" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = AccountRequestPassword(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> validateQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "validate").ToList();

			if (validateQueryStringCollection.Any())
			{
				var keyValue = requestParameters[validateQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "validate" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ValidateOrder(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> validateCustomerQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "validatecustomer").ToList();

			if (validateCustomerQueryStringCollection.Any())
			{
				var keyValue = requestParameters[validateCustomerQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "validatecustomer" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ValidateCustomer(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> validateCustomQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "validatecustom").ToList();

			if (validateCustomQueryStringCollection.Any())
			{
				var keyValue = requestParameters[validateCustomQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "validatecustom" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ValidateCustomValidation(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> validateStockQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "validatestock").ToList();

			if (validateStockQueryStringCollection.Any())
			{
				var keyValue = requestParameters[validateStockQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "validatestock" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ValidateStock(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> validateOrderlinesQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "validateorderlines").ToList();

			if (validateOrderlinesQueryStringCollection.Any())
			{
				var keyValue = requestParameters[validateOrderlinesQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "validateorderlines" || keyValue.ToLower() == "on" ||
					keyValue == "1")
				{
					var result = ValidateOrderlines(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			List<string> confirmQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "confirm").ToList();

			if (confirmQueryStringCollection.Any())
			{
				var keyValue = requestParameters[confirmQueryStringCollection.First()];
				if (keyValue.ToLower() == "true" || keyValue.ToLower() == "confirm" || keyValue.ToLower() == "on" || keyValue == "1")
				{
					var result = ConfirmOrder(requestParameters, rawRequestUrl);
					handleObjectList.Add(result);
				}
			}

			foreach (var handleObject in handleObjectList)
			{
				handleObject.Fields = postedFields;
			}

			return handleObjectList;
		}

		internal static void ClearFeedbackMessages()
		{
			Session.Remove(Constants.BasketActionResult);
			Session.Remove(Constants.WishlistActionResult);
			Session.Remove(Constants.WishlistToBasketActionResult);
			Session.Remove(Constants.CouponCodeSessionKey);
			Session.Remove(Constants.CouponCodeResultSessionKey);
			Session.Remove(Constants.CreateMemberSessionKey);
			Session.Remove(Constants.CreateMemberSessionKeyAddition);
			Session.Remove(Constants.ErrorMessagesSessionKey);
			Session.Remove(Constants.PaymentProviderSessionKey);
			Session.Remove(Constants.RequestPasswordSessionKey);
			Session.Remove(Constants.ShippingProviderSessionKey);
			Session.Remove(Constants.SignInMemberSessionKey);
			Session.Remove(Constants.SignOutMemberSessionKey);
			Session.Remove(Constants.UpdateMemberSessionKey);
			Session.Remove(Constants.UpdateMemberSessionKeyAddition);
			Session.Remove(Constants.ChangePasswordSessionKey);
			Session.Remove(Constants.OrderedItemcountHigherThanStockKey);
			Session.Remove(Constants.WishlistActionResult);
			Session.Remove(Constants.WishlistRemoveActionResult);
			Session.Remove(Constants.WishlistRenameActionResult);
			Session.Remove(Constants.WishlistToBasketActionResult);
			Session.Remove(Constants.PostedFieldsKey);
			Session.Remove(Constants.ConfirmOrderKey);
			Session.Remove("ConfirmFailed");
		}

		internal OrderInfo GetWishlist(NameValueCollection requestParameters)
		{
			var wishlistQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "wishlist").ToList();

			OrderInfo wishlist = null;

			if (wishlistQueryStringCollection.Any())
			{
				var wishlistName = requestParameters[wishlistQueryStringCollection.First()];

				if (!string.IsNullOrEmpty(wishlistName))
				{
					var membershipUser = Membership.GetUser();
					if (membershipUser != null)
					{
						var wishlistFromMember = Customers.GetWishlist(membershipUser.UserName, wishlistName,
							StoreHelper.CurrentStoreAlias);

						wishlist = Basket.GetOrderInfoFromOrderBasket(wishlistFromMember);

					}
					else
					{
						var cookie = HttpContext.Current.Request.Cookies["StoreInfo"];

						if (cookie != null)
						{
							var wishlistId = cookie["Wishlist"];

							Guid wishGuid;
							Guid.TryParse(wishlistId, out wishGuid);

							if (wishGuid != default(Guid) || wishGuid != Guid.Empty)
							{
								wishlist = OrderHelper.GetOrder(wishGuid);
							}
						}
					}
				}
			}


			return wishlist;
		}

		internal OrderInfo CreateWishlist(NameValueCollection requestParameters)
		{
			var wishlistQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "wishlist").ToList();

			OrderInfo wishlist = null;

			if (wishlistQueryStringCollection.Any())
			{
				var wishlistName = requestParameters[wishlistQueryStringCollection.First()];

				wishlist = OrderHelper.CreateOrder();

				if (wishlist != null)
				{
					wishlist.Name = wishlistName;
					wishlist.Status = OrderStatus.Wishlist;
					wishlist.Name = requestParameters[wishlistQueryStringCollection.First()];
					

					var member = Membership.GetUser();

					if (member == null)
					{
						var store = StoreHelper.GetCurrentStore();
						StoreHelper.SetStoreInfoCookie(store.Alias, null, wishlist.UniqueOrderId);
					}
					else
					{
						wishlist.CustomerInfo.LoginName = member.UserName;
						if (member.ProviderUserKey != null)
						{
							var userKey = 0;
							int.TryParse(member.ProviderUserKey.ToString(), out userKey);
							if (userKey != 0)
							{
								wishlist.CustomerInfo.CustomerId = userKey;
							}
						}
					}

					wishlist.Save();
				}
			}
			return wishlist;
		}

		internal OrderInfo GetOrCreateWishlist(NameValueCollection requestParameters)
		{
			var wishlistQueryStringCollection = requestParameters.AllKeys.Where(x => x != null && x.ToLower() == "wishlist").ToList();

			OrderInfo wishlist = null;

			if (wishlistQueryStringCollection.Any())
			{
				wishlist = GetWishlist(requestParameters) ?? CreateWishlist(requestParameters);
			}

			return wishlist;
		}

		internal OrderInfo GetCreateOrderOrWishList(NameValueCollection requestParameters)
		{
			return GetOrCreateWishlist(requestParameters) ?? (OrderHelper.GetOrder() ?? OrderHelper.CreateOrder());
		}

		private HandleObject ChangeCurrency(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject { Action = "ChangeCurrency", Url = urlReferrer };
			var result = new Dictionary<string, string>();
			var changeCurrencyKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "changecurrency"); //requestParameters["validate"];
			var changeCurrency = requestParameters[changeCurrencyKey];

			if (string.IsNullOrEmpty(changeCurrency))
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (!StoreHelper.ChangeCurrency(changeCurrency))
			{
				Session.Add(Constants.ChangeCurrencyResult, BasketActionResult.Failed);
				result.Add(Constants.ChangeCurrencyResult, BasketActionResult.Failed.ToString());
				handleObject.Success = false;
				return handleObject;
			}

			handleObject.Success = true;
			handleObject.Url = urlReferrer;
			handleObject.Item = new Dictionary<string, string>{ { "Currency", changeCurrency } };
			Session.Add(Constants.ChangeCurrencyResult, BasketActionResult.Success);
			result.Add(Constants.ChangeCurrencyResult, BasketActionResult.Success.ToString());
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject ValidateOrder(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ValidateOrder", Url = urlReferrer};
			var result = new Dictionary<string, string>();
			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (OrderHelper.ValidateOrder(order))
			{
				// order is valid, ok!
				SetBasket(handleObject, order);
				handleObject.Validated = true;
				Session.Add(Constants.ValidateOrderResult, BasketActionResult.Success);
				result.Add(Constants.ValidateOrderResult, BasketActionResult.Success.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			order.Save(true);
			handleObject.Validated = false;
			SetBasket(handleObject, order);

			Uri redirect = null;
			if (urlReferrer != null)
			{
				redirect = new Uri(urlReferrer, "#validation");
			}

			handleObject.Url = redirect;

			Session.Add(Constants.ValidateOrderResult, BasketActionResult.Failed);
			result.Add(Constants.ValidateOrderResult, BasketActionResult.Failed.ToString());
			handleObject.Messages = result;
			return handleObject;

			//return string.Format("{0}#validation", urlReferrer);
		}

		private HandleObject ValidateCustomer(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ValidateCustomer", Url = urlReferrer};
			var result = new Dictionary<string, string>();
			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (OrderHelper.ValidateCustomer(order, true))
			{
				// order is valid, ok!
				SetBasket(handleObject, order);
				handleObject.Validated = true;
				handleObject.Success = true;
				Session.Add(Constants.ValidateCustomerResult, BasketActionResult.Failed);
				result.Add(Constants.ValidateCustomerResult, BasketActionResult.Failed.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			order.Save(true, ValidateSaveAction.Customer);
			SetBasket(handleObject, order);
			handleObject.Validated = false;

			Uri redirect = null;
			if (urlReferrer != null)
			{
				redirect = new Uri(urlReferrer, "#validation");
			}

			handleObject.Url = redirect;
			Session.Add(Constants.ValidateCustomerResult, BasketActionResult.Failed);
			result.Add(Constants.ValidateCustomerResult, BasketActionResult.Failed.ToString());
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject ValidateCustomValidation(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ValidateCustomValidation", Url = urlReferrer};
			var result = new Dictionary<string, string>();
			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (OrderHelper.ValidateCustomValidation(order, true))
			{
				// order is valid, ok!
				SetBasket(handleObject, order);
				handleObject.Validated = true;
				Session.Add(Constants.ValidateCustomResult, BasketActionResult.Success);
				result.Add(Constants.ValidateCustomResult, BasketActionResult.Success.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			order.Save(true, ValidateSaveAction.CustomValidation);
			// todo: does this create the incomplete order in the order overview?
			SetBasket(handleObject, order);
			handleObject.Validated = false;

			Uri redirect = null;
			if (urlReferrer != null)
			{
				redirect = new Uri(urlReferrer, "#validation");
			}

			handleObject.Url = redirect;

			Session.Add(Constants.ValidateCustomResult, BasketActionResult.Failed);
			result.Add(Constants.ValidateCustomResult, BasketActionResult.Failed.ToString());
			handleObject.Messages = result;

			return handleObject;
		}

		private static void SetBasket(HandleObject handleObject, OrderInfo order)
		{
			var orderInfo = OrderHelper.GetOrder(order.UniqueOrderId);

			handleObject.Item = Basket.CreateBasketFromOrderInfo(orderInfo);
		}

		private HandleObject ValidateStock(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ValidateStock", Url = urlReferrer};
			var result = new Dictionary<string, string>();
			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (OrderHelper.ValidateStock(order, true))
			{
				// order is valid, ok!
				SetBasket(handleObject, order);
				handleObject.Validated = true;
				Session.Add(Constants.ValidateStockResult, BasketActionResult.Success);
				result.Add(Constants.ValidateStockResult, BasketActionResult.Success.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			order.Save(true, ValidateSaveAction.Stock);
			SetBasket(handleObject, order);
			handleObject.Validated = false;

			Uri redirect = null;
			if (urlReferrer != null)
			{
				redirect = new Uri(urlReferrer, "#validation");
			}

			Session.Add(Constants.ValidateStockResult, BasketActionResult.Failed);
			result.Add(Constants.ValidateStockResult, BasketActionResult.Failed.ToString());
			handleObject.Messages = result;

			handleObject.Url = redirect;

			return handleObject;
		}

		private HandleObject ValidateOrderlines(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ValidateOrderlines", Url = urlReferrer};
			var result = new Dictionary<string, string>();

			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				return handleObject;
			}

			if (OrderHelper.ValidateOrderLines(order, true))
			{
				// order is valid, ok!
				SetBasket(handleObject, order);
				handleObject.Validated = true;

				Session.Add(Constants.ValidateOrderlineResult, BasketActionResult.Success);
				result.Add(Constants.ValidateOrderlineResult, BasketActionResult.Success.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			order.Save(true, ValidateSaveAction.Orderlines);
			SetBasket(handleObject, order);
			handleObject.Validated = false;

			Uri redirect = null;
			if (urlReferrer != null)
			{
				redirect = new Uri(urlReferrer, "#validation");
			}

			Session.Add(Constants.ValidateOrderlineResult, BasketActionResult.Failed);
			result.Add(Constants.ValidateOrderlineResult, BasketActionResult.Failed.ToString());
			handleObject.Messages = result;

			handleObject.Url = redirect;
			return handleObject;
		}

		private HandleObject ClearBasket(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "ClearBasket", Url = urlReferrer};
			var result = new Dictionary<string, string>();

			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				handleObject.Messages = result;
				Session.Add(Constants.BasketActionResult, BasketActionResult.Failed);

				result.Add(Constants.BasketActionResult, BasketActionResult.Failed.ToString());
				return handleObject;
			}

			order.OrderLines.Clear();

			order.Save();

			Session.Add(Constants.BasketActionResult, BasketActionResult.Success);
			result.Add(Constants.BasketActionResult, BasketActionResult.Success.ToString());
			SetBasket(handleObject, order);
			handleObject.Messages = result;
			handleObject.Success = true;
			return handleObject;
		}

		private HandleObject ClearOrder(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject { Action = "ClearOrder", Url = urlReferrer };
			var result = new Dictionary<string, string>();

			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				handleObject.Success = false;
				handleObject.Messages = result;
				Session.Add(Constants.BasketActionResult, BasketActionResult.Failed);

				result.Add(Constants.BasketActionResult, BasketActionResult.Failed.ToString());
				return handleObject;
			}

			order = OrderHelper.CreateOrder();

			Session.Add(Constants.BasketActionResult, BasketActionResult.Success);
			result.Add(Constants.BasketActionResult, BasketActionResult.Success.ToString());
			SetBasket(handleObject, order);
			handleObject.Messages = result;
			handleObject.Success = true;
			return handleObject;
		}


		private HandleObject AccountSignOut(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "AccountSignOut", Url = urlReferrer};
			var result = new Dictionary<string, string>();
			FormsAuthentication.SignOut();

			Session.Add(Constants.SignOutMemberSessionKey, AccountActionResult.Success);

			result.Add(Constants.SignOutMemberSessionKey, AccountActionResult.Success.ToString());

			Session.Remove(Constants.SignInMemberSessionKey);
			handleObject.Messages = result;
			handleObject.Success = true;

			return handleObject;
		}

		private HandleObject AccountSignIn(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "AccountSignIn", Url = urlReferrer};
			

			string userNameKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "username") ?? requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "customeremail") ?? string.Empty;
			string passwordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "password") ?? string.Empty;

			string userNameValue = requestParameters[userNameKey];
			string passwordValue = requestParameters[passwordKey];

			MembershipUser user = Membership.GetUser(userNameValue);

			var result = new Dictionary<string, string>();

			if (user == null)
			{
				Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.MemberNotExists);

				result.Add(Constants.SignInMemberSessionKey, AccountActionResult.MemberNotExists.ToString());
				handleObject.Messages = result;
				handleObject.Success = false;
				return handleObject;
			}

			if (!Membership.ValidateUser(userNameValue, passwordValue))
			{
				Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.ValidateUserError);
				result.Add(Constants.SignInMemberSessionKey, AccountActionResult.ValidateUserError.ToString());
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}
			
			FormsAuthentication.SetAuthCookie(userNameValue, true);

			var profile = ProfileBase.Create(userNameValue);
			if (profile["forceChangePassword"] != null)
			{
				var value = profile["forceChangePassword"].ToString();
				if (value == "true" || value == "1" || value == "on")
				{
					var changePasswordUrl = API.Store.GetStore().AccountChangePasswordUrl;

					var strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
					var strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, string.Empty);

					handleObject.Url = new Uri(string.Format("{0}{1}", strUrl, changePasswordUrl));
				}
			}


			Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.Success);
			Session.Remove(Constants.SignOutMemberSessionKey);

			result.Add(Constants.SignInMemberSessionKey, AccountActionResult.Success.ToString());
			handleObject.Messages = result;
			handleObject.Success = true;
			return handleObject;
		}

		private HandleObject AccountCreate(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject { Action = "CreateAccount", Url = urlReferrer };

			var userKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "username") ?? requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "customeremail");

			var passwordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "password");

			var validatePasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "validatepassword");

			var generatePasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "generatepassword");

			// when doing a one-page checkout, you might not want to cancel the confirmation when the member already exists
			var ignoreMemberexistsError = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "ignorememberexistserror");

			var ignoreMemberexistsValue = requestParameters[ignoreMemberexistsError];

			var ignoreMemberexists = ignoreMemberexistsValue != null && (ignoreMemberexistsValue.ToLower() == "true" ||
																		 ignoreMemberexistsValue.ToLower() == "ignorememberexistserror" ||
																		 ignoreMemberexistsValue.ToLower() == "on" ||
																		 ignoreMemberexistsValue == "1") || Membership.GetUser() != null;

			var userName = requestParameters[userKey];

			if (userName == null)
			{
				var order = OrderHelper.GetOrder();

				if (order != null)
				{
					userName = order.CustomerEmail;
				}
			}

			var result = new Dictionary<string, string>();

			if (userName == null)
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.NoUserNameInput);
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.NoUserNameInput.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}

			if (!Regex.IsMatch(userName, "\\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", RegexOptions.IgnoreCase))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.UserNameInvalid);
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.UserNameInvalid.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}

			string passwordValue = requestParameters[passwordKey];
			string validatePasswordValue = requestParameters[validatePasswordKey];
			string generatePasswordValue = requestParameters[generatePasswordKey];

			if (string.IsNullOrEmpty(generatePasswordValue) && !string.IsNullOrEmpty(passwordValue))
			{
				if (passwordValue == validatePasswordValue)
				{
					if (Membership.MinRequiredPasswordLength > 0)
					{
						if (passwordValue.Length < Membership.MinRequiredPasswordLength)
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MinRequiredPasswordLengthError);
							Log.Instance.LogDebug("MinRequiredPasswordLengthError");
							result.Add(Constants.CreateMemberSessionKey,
								AccountActionResult.MinRequiredPasswordLengthError.ToString());
							handleObject.Success = false;
							handleObject.Validated = false;
							handleObject.Messages = result;
							return handleObject;
						}
					}

					if (!string.IsNullOrEmpty(Membership.PasswordStrengthRegularExpression))
					{
						if (!Regex.IsMatch(passwordValue, Membership.PasswordStrengthRegularExpression))
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.PasswordStrengthRegularExpressionError);
							Log.Instance.LogDebug("PasswordStrengthRegularExpression");
							result.Add(Constants.CreateMemberSessionKey,
								AccountActionResult.PasswordStrengthRegularExpressionError.ToString());
							handleObject.Success = false;
							handleObject.Validated = false;
							handleObject.Messages = result;
							return handleObject;
						}
					}

					if (Membership.MinRequiredNonAlphanumericCharacters > 0)
					{
						int num = passwordValue.Where((t, i) => !char.IsLetterOrDigit(passwordValue, i)).Count();

						if (num < Membership.MinRequiredNonAlphanumericCharacters)
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MinRequiredNonAlphanumericCharactersError);
							Log.Instance.LogDebug("MinRequiredNonAlphanumericCharacters");
							result.Add(Constants.CreateMemberSessionKey,
								AccountActionResult.MinRequiredNonAlphanumericCharactersError.ToString());
							handleObject.Success = false;
							handleObject.Validated = false;
							handleObject.Messages = result;
							return handleObject;
						}
					}
				}
				else
				{
					Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.PasswordMismatch);
					Log.Instance.LogDebug("PasswordMismatch");
					result.Add(Constants.CreateMemberSessionKey, AccountActionResult.PasswordMismatch.ToString());
					handleObject.Success = false;
					handleObject.Validated = false;
					handleObject.Messages = result;
					return handleObject;
				}
			}
			else
			{
				passwordValue = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
			}

			var webConfig = new XmlDocument();
			webConfig.Load(HttpContext.Current.Server.MapPath("~/web.config"));

			var umbracoDefaultMemberTypeAlias = webConfig.SelectSingleNode("//add[@defaultMemberTypeAlias]");

			var memberTypes = IO.Container.Resolve<ICMSApplication>().GetAllMemberTypes();

			if (umbracoDefaultMemberTypeAlias == null || umbracoDefaultMemberTypeAlias.Attributes == null)
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.DefaultMemberTypeAliasError);
				Log.Instance.LogError("AccountCreate: DefaultMemberTypeAliasError");
				result.Add(Constants.CreateMemberSessionKey,
					AccountActionResult.DefaultMemberTypeAliasError.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}
			string memberTypevalue = umbracoDefaultMemberTypeAlias.Attributes["defaultMemberTypeAlias"].Value;

			if (memberTypes.All(x => x.Alias != memberTypevalue))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.DefaultMemberTypeAliasNonExistingError);
				Log.Instance.LogError("AccountCreate: DefaultMemberTypeAliasNonExistingError");
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.DefaultMemberTypeAliasNonExistingError.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}
			if (string.IsNullOrEmpty(userName))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.CustomerEmailEmpty);
				Log.Instance.LogError("AccountCreate: Username empty");
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.CustomerEmailEmpty.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}

			var member = Membership.GetUser(userName);

			if (member != null && !ignoreMemberexists)
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MemberExists);
				Log.Instance.LogError("AccountCreate: MemberExists username: " + userName);
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.MemberExists.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}

			// do nothing, but also don't stop the confirmation of the order (most likely situation for this usage)
			if (ignoreMemberexists)
			{
				Log.Instance.LogError("AccountCreate: MemberExists username: " + userName + " BUT Ignore Member Exists");
				handleObject.Success = true;
				handleObject.Validated = true;
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.SuccessMemberExists);
				result.Add(Constants.CreateMemberSessionKey, AccountActionResult.SuccessMemberExists.ToString());
			}

			if (member == null)
			{
				var membershipUser = Membership.CreateUser(userName, passwordValue, userName);

				string roleKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "membergroup");
				string roleValue = requestParameters[roleKey];

				if (!string.IsNullOrEmpty(roleValue))
				{
					if (Roles.RoleExists(roleValue))
					{
						Roles.AddUserToRole(membershipUser.UserName, roleValue);
					}
					else
					{
						Roles.CreateRole(roleValue);

						Roles.AddUserToRole(membershipUser.UserName, roleValue);
					}
				}
				else
				{
					if (!Roles.GetAllRoles().Any())
					{
						const string customersRole = "Customers";

						Roles.CreateRole(customersRole);

						Roles.AddUserToRole(membershipUser.UserName, customersRole);
					}
					else
					{
						Roles.AddUserToRole(membershipUser.UserName, Roles.GetAllRoles().First());
					}
				}

				var profile = ProfileBase.Create(userName);

				foreach (var prop in ProfileBase.Properties)
				{
					try
					{
						var settingsProperty = (SettingsProperty)prop;

						string settingsPropertyName = settingsProperty.Name;

						if (!string.IsNullOrEmpty(requestParameters[settingsPropertyName]))
						{
							profile[settingsPropertyName] = requestParameters[settingsPropertyName];
						}
					}
					catch
					{
						var settingsProperty = (SettingsProperty)prop;

						Log.Instance.LogDebug(
							string.Format("UpdateAccount Failed for Request Property: {0}, currentNodeId: {1}",
								settingsProperty.Name, IO.Container.Resolve<ICMSApplication>().CurrentNodeId()));
					}
				}

				profile.Save();
				handleObject.Item = profile;
				FormsAuthentication.SetAuthCookie(userName, true);

				var currentStore = StoreHelper.GetById(StoreHelper.GetCurrentStore().Id);

				if (string.IsNullOrEmpty(currentStore.AccountCreatedEmail))
				{
					Log.Instance.LogError("CreateAccount: AccountCreatedEmail not set: No email send to customer");
					handleObject.Messages = result;

					handleObject.Success = true;
					handleObject.Validated = true;
				}
				else
				{
					var emailNodeId = Convert.ToInt32(currentStore.AccountCreatedEmail);
					EmailHelper.SendMemberEmailCustomer(emailNodeId, currentStore, userName, passwordValue);

					handleObject.Success = true;
					handleObject.Validated = true;
				}

				if (Session[Constants.CreateMemberSessionKey] == null)
				{
					Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.Success);
				}
				if (!result.ContainsKey(Constants.CreateMemberSessionKey))
				{
					result.Add(Constants.CreateMemberSessionKey, AccountActionResult.Success.ToString());
				}
			}

			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AccountRequestPassword(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject { Action = "RequestPassword", Url = urlReferrer };

			string userNameKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "username") ?? requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "customeremail"); //requestParameters["customerEmail"];
			string userNameValue = requestParameters[userNameKey];

			var result = new Dictionary<string, string>();

			if (string.IsNullOrEmpty(userNameValue))
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.CustomerUserNameEmpty);
				result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.CustomerUserNameEmpty.ToString());
				handleObject.Messages = result;
				handleObject.Success = false;
				return handleObject;
			}

			var currentStore = StoreHelper.GetById(StoreHelper.GetCurrentStore().Id);

			if (string.IsNullOrEmpty(currentStore.AccountForgotPasswordEmail))
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.AccountForgotPasswordEmailNotConfigured);
				result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.AccountForgotPasswordEmailNotConfigured.ToString());
				handleObject.Messages = result;
				handleObject.Success = false;
				return handleObject;
			}
			int emailNodeId = Convert.ToInt32(currentStore.AccountForgotPasswordEmail);

			MembershipUser user = Membership.GetUser(userNameValue);


			if (user == null)
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.MemberNotExists);
				result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.MemberNotExists.ToString());
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}


			if (!Membership.EnablePasswordReset)
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.EnablePasswordResetDisabled);
				result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.EnablePasswordResetDisabled.ToString());
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}

			string resetPassword = user.ResetPassword();

			var profile = ProfileBase.Create(userNameValue);
			if (profile["forceChangePassword"] != null)
			{
				profile.SetPropertyValue("forceChangePassword", "1");
				profile.Save();
			}

			string newPassword = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);

			if (user.ChangePassword(resetPassword, newPassword))
			{
				EmailHelper.SendMemberEmailCustomer(emailNodeId, currentStore, userNameValue, newPassword);
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.Success);
				result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.Success.ToString());
				handleObject.Success = true;
				handleObject.Messages = result;
				handleObject.Validated = true;
				return handleObject;
			}

			Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.ChangePasswordError);
			result.Add(Constants.RequestPasswordSessionKey, AccountActionResult.ChangePasswordError.ToString());
			handleObject.Success = false;
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AccountChangePassword(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject { Action = "ChangePassoword", Url = urlReferrer };
			
			var result = new Dictionary<string, string>();

			var currentPasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "currentpassword"); //requestParameters["currentPassword"];
			var newPasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "newpassword"); //requestParameters["newPassword"];
			var validatePasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "validatenewpassword"); //requestParameters["validateNewPassword"];
			var generatePasswordKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "generatepassword"); //requestParameters["generatePassword"];

			var currentPasswordValue = requestParameters[currentPasswordKey];
			var newPasswordValue = requestParameters[newPasswordKey];
			var validatePasswordValue = requestParameters[validatePasswordKey];
			var generatePasswordValue = requestParameters[generatePasswordKey];

			var memberShipUser = Membership.GetUser();

			if (!string.IsNullOrEmpty(currentPasswordValue) || !string.IsNullOrEmpty(newPasswordValue) || !string.IsNullOrEmpty(validatePasswordValue) || !string.IsNullOrEmpty(generatePasswordValue))
			{
				Log.Instance.LogDebug("Update Account Password Section");
				if (Membership.ValidateUser(memberShipUser.UserName, currentPasswordValue))
				{
					if (!string.IsNullOrEmpty(generatePasswordValue))
					{
						newPasswordValue = Membership.GeneratePassword(Membership.MinRequiredPasswordLength,
							Membership.MinRequiredNonAlphanumericCharacters);
					}
					else
					{
						if (newPasswordValue != validatePasswordValue)
						{
							Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.PasswordMismatch);
							result.Add(Constants.ChangePasswordSessionKey,
								AccountActionResult.PasswordMismatch.ToString());
							
							handleObject.Success = false;
							handleObject.Messages = result;
							return handleObject;
						}

						if (string.IsNullOrEmpty(newPasswordValue))
						{
							Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Failed);
							result.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Failed.ToString());
							handleObject.Success = false;
							handleObject.Messages = result;
							return handleObject;
						}

						if (Membership.MinRequiredPasswordLength > 0)
						{
							if (newPasswordValue.Length < Membership.MinRequiredPasswordLength)
							{
								Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.MinRequiredPasswordLengthError);
								result.Add(Constants.ChangePasswordSessionKey,
									AccountActionResult.MinRequiredPasswordLengthError.ToString());
								handleObject.Messages = result;
								handleObject.Success = false;
								return handleObject;
							}
						}

						if (!string.IsNullOrEmpty(Membership.PasswordStrengthRegularExpression))
						{
							if (!Regex.IsMatch(newPasswordValue, Membership.PasswordStrengthRegularExpression))
							{
								Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.PasswordStrengthRegularExpressionError);
								result.Add(Constants.ChangePasswordSessionKey,
									AccountActionResult.PasswordStrengthRegularExpressionError.ToString());
								handleObject.Messages = result;
								handleObject.Success = false;
								return handleObject;
							}
						}

						if (Membership.MinRequiredNonAlphanumericCharacters > 0)
						{
							int num = newPasswordValue.Where((t, i) => !char.IsLetterOrDigit(newPasswordValue, i)).Count();

							if (num < Membership.MinRequiredNonAlphanumericCharacters)
							{
								Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.MinRequiredNonAlphanumericCharactersError);
								Session.Add(Constants.UpdateMemberSessionKeyAddition, Membership.MinRequiredNonAlphanumericCharacters);
								result.Add(Constants.ChangePasswordSessionKey, AccountActionResult.MinRequiredNonAlphanumericCharactersError.ToString());
								handleObject.Messages = result;
								handleObject.Success = false;
								return handleObject;
							}
						}
					}

					Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Success);
					result.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Success.ToString());

					memberShipUser.ChangePassword(currentPasswordValue, newPasswordValue);

					var profile = ProfileBase.Create(memberShipUser.UserName);
					if (profile["forceChangePassword"] != null)
					{
						profile.SetPropertyValue("forceChangePassword", "0");
						profile.Save();
					}

					handleObject.Success = true;
					handleObject.Messages = result;
					handleObject.Validated = true;
					return handleObject;
				}

				Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.CurrentpasswordError);
				result.Add(Constants.ChangePasswordSessionKey, AccountActionResult.ChangePasswordError.ToString());
				handleObject.Messages = result;
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}

			Session.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Failed);
			result.Add(Constants.ChangePasswordSessionKey, AccountActionResult.Failed.ToString());
			handleObject.Success = false;
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AccountUpdate(NameValueCollection requestParameters, Uri urlReferrer)
		{
			var handleObject = new HandleObject {Action = "UpdateAccount", Url = urlReferrer};
			
			var result = new Dictionary<string, string>();

			var memberShipUser = Membership.GetUser();

			if (memberShipUser == null)
			{
				Log.Instance.LogDebug("UpdateAccount: " + AccountActionResult.MemberNotExists);
				Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.MemberNotExists);

				result.Add(Constants.UpdateMemberSessionKey, AccountActionResult.MemberNotExists.ToString());
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}

			var profile = ProfileBase.Create(memberShipUser.UserName);

			// note don't forget to disable automatic profile save!: <profile defaultProvider="UmbracoMemberProfileProvider" enabled="true" automaticSaveEnabled="false">
			foreach (var prop in ProfileBase.Properties)
			{
				try
				{
					var settingsProperty = (SettingsProperty) prop;

					string settingsPropertyName = settingsProperty.Name;

					if (!string.IsNullOrEmpty(requestParameters[settingsPropertyName]))
					{
						profile[settingsPropertyName] = requestParameters[settingsPropertyName];
					}
				}
				catch
				{
					var settingsProperty = (SettingsProperty) prop;

					Log.Instance.LogDebug(string.Format("UpdateAccount Failed for Request Property: {0}, currentNodeId: {1}",
						settingsProperty.Name, IO.Container.Resolve<ICMSApplication>().CurrentNodeId()));
				}
			}


			var emailKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "email");

			if (string.IsNullOrEmpty(emailKey))
			{
				emailKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "customerEmail");
			}

			if (string.IsNullOrEmpty(emailKey))
			{
				memberShipUser.Email = emailKey;
			}

			// todo: BREAKING with 2.4, only fired when calling changepassword as well!
			//AccountChangePassword(requestParameters, urlReferrer);
			
			profile.Save();
			try
			{
				Membership.UpdateUser(memberShipUser);
			}
			catch
			{
				// Umbraco + SQL CE can't change email: BUG in Umbraco!
			}

			Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.Success);
			result.Add(Constants.UpdateMemberSessionKey, AccountActionResult.Success.ToString());
			handleObject.Success = true;
			handleObject.Messages = result;
			handleObject.Validated = true;
			return handleObject;
		}

		private HandleObject ConfirmOrder(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "ConfirmOrder", Url = requestUri};
			var result = new Dictionary<string, string>();

			var order = GetCreateOrderOrWishList(requestParameters);

			string orderStatusKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "confirm"); //requestParameters["confirm"];
			string acceptTermsKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "acceptterms"); //requestParameters["acceptterms"];
			string orderStatusValue = requestParameters[orderStatusKey];
			string acceptTermsValue = requestParameters[acceptTermsKey];

			if (string.IsNullOrEmpty(orderStatusValue))
			{
				Session.Add(Constants.ConfirmOrderKey, AccountActionResult.Failed);
				result.Add(Constants.ConfirmOrderKey, ConfirmOrderResults.Failed.ToString());
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}

			bool termsAccepted = acceptTermsValue != null && (acceptTermsValue.ToLower() == "true" || acceptTermsValue.ToLower() == "acceptterms" || acceptTermsValue.ToLower() == "on" || acceptTermsValue == "1");

			Uri validateUri = null;
			if (requestUri != null)
			{
				validateUri = new Uri(requestUri, "#validation");
			}

			int confirmOrderNodeId = 0;

			Log.Instance.LogDebug("BasketHandler ConfirmOrder confirm: " + orderStatusValue);

			string confirmOrderNodeRequestKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "confirmordernodeid"); //requestParameters["confirmOrderNodeId"];
			string confirmOrderNodeRequestValue = requestParameters[confirmOrderNodeRequestKey];

			if (!string.IsNullOrEmpty(confirmOrderNodeRequestValue))
			{
				int.TryParse(confirmOrderNodeRequestValue, out confirmOrderNodeId);
			}

			if (!order.ConfirmOrder(termsAccepted, confirmOrderNodeId))
			{
				SetBasket(handleObject, order);
				handleObject.Url = validateUri;
				handleObject.Success = false;
				handleObject.Validated = false;
				Session.Add(Constants.ConfirmOrderKey, AccountActionResult.Failed);
				result.Add(Constants.ConfirmOrderKey, ConfirmOrderResults.Failed.ToString());
				handleObject.Messages = result;
				return handleObject;
			}


			handleObject.Validated = true;
			if (order.PaymentInfo.TransactionMethod == PaymentTransactionMethod.WebClient)
			{
				HttpContext.Current.Response.Write(Encoding.UTF8.GetString(GetBytes(order.PaymentInfo.Parameters)));
			}
			else
			{
				if (string.IsNullOrEmpty(order.RedirectUrl))
				{
					handleObject.Success = true;
					handleObject.Messages = result;
					//todo: check if this is really a successfull situation?
					Session.Add(Constants.ConfirmOrderKey, AccountActionResult.Success);
					result.Add(Constants.ConfirmOrderKey, ConfirmOrderResults.Success.ToString());
					return handleObject;
				}
				SetBasket(handleObject, order);
				
				// todo: nasty hack, because on localhost // somehow makes from // file:// instead of http:// 
				var redirectUrl = order.RedirectUrl;

				if (order.RedirectUrl.StartsWith("//"))
				{
					var http = "http:";
					if (requestUri != null && requestUri.AbsoluteUri.StartsWith("https"))
					{
						http = "https:";
					}

					redirectUrl = string.Format("{0}{1}", http, order.RedirectUrl);
				}

				if(redirectUrl != "inline")
				handleObject.Url = new Uri(redirectUrl);
				handleObject.PostConfirmUrl = new Uri(redirectUrl);

				Log.Instance.LogDebug("CONFIRM ORDER REDIRECT URL: "+ redirectUrl + " (handleObject.PostConfirmUrl) handleObject.PostConfirmUrl.AbsoluteUri: " + handleObject.PostConfirmUrl.AbsoluteUri);

			}

			Session.Add(Constants.ConfirmOrderKey, AccountActionResult.Success);
			result.Add(Constants.ConfirmOrderKey, ConfirmOrderResults.Success.ToString());

			handleObject.Success = true;
			handleObject.Messages = result;

			return handleObject;
		}

		internal static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length*sizeof (char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		private HandleObject AddCoupon(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "AddCoupon", Url = requestUri};
			var result = new Dictionary<string, string>();
			var order = GetCreateOrderOrWishList(requestParameters);

			string couponCodeKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "couponcode"); //requestParameters["couponCode"];
			string removeCouponKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "removecoupon"); //requestParameters["removeCoupon"];

			string couponCodeValue = requestParameters[couponCodeKey];
			string removeCouponValue = requestParameters[removeCouponKey];

			CouponCodeResult codeResult;
			var message = new Dictionary<string, string>();
			bool saveOrder = true;
			if (!string.IsNullOrEmpty(removeCouponValue))
			{
				if (removeCouponValue.ToLower() == "true" || removeCouponValue.ToLower() == "removecoupon" && removeCouponValue.ToLower() != "on" && removeCouponValue != "1")
				{
					codeResult = order.RemoveCoupon(couponCodeValue);
					if (codeResult != CouponCodeResult.Success)
					{
						message = new Dictionary<string, string> { { Constants.CouponCodeSessionKey, codeResult.ToString() } };

						saveOrder = false;
					}
				}
				else
				{
					codeResult = CouponCodeResult.Failed;
					message = new Dictionary<string, string> {{Constants.CouponCodeSessionKey, codeResult.ToString()}};

					saveOrder = false;
				}
			}
			else
			{
				codeResult = order.AddCoupon(couponCodeValue);
				if (codeResult != CouponCodeResult.Success)
				{
					message = new Dictionary<string, string> { { Constants.CouponCodeSessionKey, codeResult.ToString() } };
					saveOrder = false;
				}
			}

			if (saveOrder)
			{
				handleObject.Success = true;
				order.Save();
			}
			else
			{
				handleObject.Success = false;
			}

			handleObject.Messages = message;
			SetBasket(handleObject, order);
			foreach (var mes in message)
			{
				result.Add(mes.Key, mes.Value);
				Session.Add(mes.Key, mes.Value);
			}
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AddOrderLineDetails(NameValueCollection requestParameters, IEnumerable<string> queryStringCollection, int orderlineId, Uri requestUri)
		{
			// todo: updating an account, while there is an order will also update those details on the current order; do we want that?
			var handleObject = new HandleObject {Action = "AddOrderLineDetails", Url = requestUri};
			var result = new Dictionary<string, string>();
			var order = GetCreateOrderOrWishList(requestParameters);

			Dictionary<string, string> fields = queryStringCollection.ToDictionary(s => s, s => requestParameters[s]);

			if (!fields.Any())
			{
				SetBasket(handleObject, order);
				result.Add(Constants.BasketActionResult, BasketActionResult.Failed.ToString());
				Session.Add(Constants.BasketActionResult, BasketActionResult.Failed.ToString());
				handleObject.Messages = result;
				handleObject.Success = false;
				return handleObject;
			}
			order.AddOrderLineDetails(fields, orderlineId);

			order.Save();

			SetBasket(handleObject, order);
			result.Add(Constants.BasketActionResult, BasketActionResult.Success.ToString());
			Session.Add(Constants.BasketActionResult, BasketActionResult.Success.ToString());
			handleObject.Success = true;
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AddCustomerInformation(NameValueCollection requestParameters, IEnumerable<string> queryStringCollection, CustomerDatatypes customerDataType, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "AddCustomerInformation", Url = requestUri};
			var result = new Dictionary<string, string>();

			var order = GetCreateOrderOrWishList(requestParameters);

			var stringCollection = queryStringCollection as string[] ?? queryStringCollection.ToArray();
			var fields = stringCollection.ToDictionary(s => s, s => requestParameters[s]);

			if (!fields.Any())
			{
				SetBasket(handleObject, order);
				handleObject.Success = false;
				handleObject.Validated = false;
				result.Add(Constants.CustomerInformation, CustomerInformationResult.Failed.ToString());
				Session.Add(Constants.CustomerInformation, CustomerInformationResult.Failed.ToString());
				handleObject.Messages = result;
				return handleObject;
			}
			IO.Container.Resolve<IOrderUpdatingService>().AddCustomerFields(order, fields, customerDataType);

			var customerIsShippingKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "customerisshipping"); //requestParameters["customerIsShipping"];

			if (customerIsShippingKey != null)
			{
				var customerIsShippingValue = requestParameters[customerIsShippingKey];

				if (customerIsShippingValue != null && (customerIsShippingValue.ToLower() == "customerisshipping" || customerIsShippingValue.ToLower() == "true" || customerIsShippingValue.ToLower() == "on" || customerIsShippingValue == "1"))
				{
					var shippingFields = stringCollection.ToDictionary(s => s.Replace("customer", "shipping"), s => requestParameters[s]);

					if (!shippingFields.Any()) return handleObject;
					order.AddCustomerFields(shippingFields, CustomerDatatypes.Shipping);
				}
			}

			order.Save();

			SetBasket(handleObject, order);
			handleObject.Success = true;
			handleObject.Validated = true;
			result.Add(Constants.CustomerInformation, CustomerInformationResult.Success.ToString());
			Session.Add(Constants.CustomerInformation, CustomerInformationResult.Success.ToString());
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AddShippingMethod(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "AddShippingMethod", Url = requestUri};

			var order = GetCreateOrderOrWishList(requestParameters);

			string shippingProviderKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "shippingprovider"); //requestParameters["shippingProvider"];

			string shippingProviderValue = requestParameters[shippingProviderKey];

			var result = new Dictionary<string, string>();

			if (shippingProviderValue != null && (shippingProviderValue.ToLower() == "indication" || shippingProviderValue.ToLower() == "auto"))
			{
				ShippingProviderHelper.AutoSelectShippingProvider(order);

				Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.Success);
				result.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.Success.ToString());
				order.Save();

				SetBasket(handleObject, order);
				handleObject.Success = true;
				handleObject.Messages = result;
				handleObject.Validated = true;
				return handleObject;
			}

			if (shippingProviderValue != null)
			{
				if (!shippingProviderValue.Contains("-"))
				{
					Log.Instance.LogDebug("AddShippingMethod: " + ProviderActionResult.NoCorrectInput + " value: " + shippingProviderValue);
					Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput);
					result.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput.ToString());
					SetBasket(handleObject, order);
					handleObject.Success = false;
					handleObject.Validated = false;
					handleObject.Messages = result;
					return handleObject;
				}

				var shippingproviderSplit =  shippingProviderValue.Split('-');

				int shippingProviderId = 0;
				int.TryParse(shippingproviderSplit[0], out shippingProviderId);
				string shippingProviderMethodId = shippingProviderValue.Replace(shippingproviderSplit[0] + "-", "");


				if (shippingProviderId == 0)
				{
					Log.Instance.LogDebug("AddShippingMethod: " + ProviderActionResult.ProviderIdZero + " value: " + shippingProviderValue);
					Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.ProviderIdZero);
					result.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.ProviderIdZero.ToString());
					SetBasket(handleObject, order);
					handleObject.Success = false;
					handleObject.Validated = false;
					handleObject.Messages = result;
					return handleObject;
				}

				var actionResult = order.AddShippingProvider(shippingProviderId, shippingProviderMethodId);

				order.Save();

				Session.Add(Constants.ShippingProviderSessionKey, actionResult);
				result.Add(Constants.ShippingProviderSessionKey, actionResult.ToString());
				SetBasket(handleObject, order);
				handleObject.Success = true;
				handleObject.Validated = true;
				handleObject.Messages = result;
				return handleObject;
			}
			Log.Instance.LogDebug("AddShippingMethod: " + ProviderActionResult.NoCorrectInput + " value: null");
			Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput);

			result.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput.ToString());
			SetBasket(handleObject, order);
			handleObject.Success = false;
			handleObject.Validated = false;
			handleObject.Messages = result;
			return handleObject;
		}

		private HandleObject AddPaymentMethod(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "AddPaymentMethod", Url = requestUri};

			var order = GetCreateOrderOrWishList(requestParameters);

			string paymentProviderKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "paymentprovider"); //requestParameters["paymentProvider"];

			string paymentProviderValue = requestParameters[paymentProviderKey];

			var result = new Dictionary<string, string>();

			if (paymentProviderValue == null)
			{
				Session.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput);
				Log.Instance.LogError("BasketHandler AddPaymentMethod paymentProviderValue == null");
				result.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				SetBasket(handleObject, order);
				handleObject.Messages = result;
				return handleObject;
			}

			if (!paymentProviderValue.Contains("-")) 
				// if the payment provider/method is given, act as PaymentHandler and request payment
			{
				var paymentProvider = IO.Container.Resolve<IPaymentProviderService>().GetPaymentProviderWithName(paymentProviderValue, StoreHelper.CurrentLocalization);

				if (paymentProvider != null)
				{
					new PaymentRequestHandler().HandleuWebshopPaymentRequest(paymentProvider);
					result.Add(Constants.PaymentProviderSessionKey, "HandleuWebshopPaymentRequest");
					handleObject.Success = true;
					handleObject.Validated = true;
					handleObject.Messages = result;
					return handleObject;
				}

				Session.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput);
				Log.Instance.LogError("BasketHandler AddPaymentMethod !paymentProviderValue.Contains(-) && paymentProviderNode != null");
				result.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				handleObject.Messages = result;
				return handleObject;
			}

			if (paymentProviderValue.Split('-').Length <= 0)
			{
				Session.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput);
				Log.Instance.LogError("BasketHandler AddPaymentMethod paymentProviderValue.Split('-').Length <= 0");
				result.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.NoCorrectInput.ToString());
				handleObject.Success = false;
				handleObject.Validated = false;
				SetBasket(handleObject, order);
				return handleObject;
			}

			var paymentProviderSplit = paymentProviderValue.Split('-');

			int paymentProviderId;
			int.TryParse(paymentProviderSplit[0], out paymentProviderId);
			var paymentProviderMethodId = paymentProviderValue.Replace(paymentProviderSplit[0] + "-", "");

			if (paymentProviderId == 0)
			{
				Session.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.ProviderIdZero);
				Log.Instance.LogError("BasketHandler AddPaymentMethod ProviderActionResult.ProviderIdZero");
				result.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.ProviderIdZero.ToString());
				SetBasket(handleObject, order);
				handleObject.Success = false;
				handleObject.Validated = false;
				return handleObject;
			}
			order.AddPaymentProvider(paymentProviderId, paymentProviderMethodId);

			order.Save();

			Session.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.Success);

			result.Add(Constants.PaymentProviderSessionKey, ProviderActionResult.Success.ToString());

			handleObject.Item = order;
			SetBasket(handleObject, order);
			handleObject.Success = true;
			handleObject.Validated = true;
			handleObject.Messages = result;
			return handleObject;
		}

		internal HandleObject WishlistCreate(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject { Action = "RenameWishlist", Url = requestUri };
			var result = new Dictionary<string, string>();

			var wishlist = GetWishlist(requestParameters);
			var wishlistName = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "wishlist");

			if (wishlist == null)
			{
				var newWishlist = OrderHelper.CreateOrder();
				if (newWishlist != null && !string.IsNullOrEmpty(wishlistName))
				{
					newWishlist.Status = OrderStatus.Wishlist;
					newWishlist.Name = requestParameters[wishlistName];
					newWishlist.Save();

					Session.Add(Constants.WishlistActionResult, BasketActionResult.Success);
					result.Add(Constants.WishlistActionResult, BasketActionResult.Success.ToString());
					SetBasket(handleObject, newWishlist);
					handleObject.Success = true;
				}
				else
				{
					Session.Add(Constants.WishlistActionResult, BasketActionResult.Failed);
					result.Add(Constants.WishlistActionResult, BasketActionResult.Failed.ToString());
					handleObject.Success = false;
				}
			}
			else
			{
				Session.Add(Constants.WishlistActionResult, BasketActionResult.AlreadyExists);
				result.Add(Constants.WishlistActionResult, BasketActionResult.AlreadyExists.ToString());
				handleObject.Success = false;
			}

			handleObject.Messages = result;
			return handleObject;
		}

		internal HandleObject WishlistRename(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject { Action = "RenameWishlist", Url = requestUri };

			var result = new Dictionary<string, string>();

			var wishlist = GetOrCreateWishlist(requestParameters);
			var newWishlistName = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "newwishlistname");

			if (wishlist != null && !string.IsNullOrEmpty(newWishlistName))
			{
				wishlist.Name = requestParameters[newWishlistName];
				wishlist.Save();

				Session.Add(Constants.WishlistRenameActionResult, BasketActionResult.Success);
				result.Add(Constants.WishlistRenameActionResult, BasketActionResult.Success.ToString());
				SetBasket(handleObject, wishlist);
				handleObject.Success = true;
			}
			else
			{
				Session.Add(Constants.WishlistRenameActionResult, BasketActionResult.Failed);
				result.Add(Constants.WishlistRenameActionResult, BasketActionResult.Failed.ToString());
				handleObject.Success = false;
			}

			handleObject.Messages = result;
			return handleObject;
		}

		internal HandleObject WishlistRemove(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject { Action = "RemoveWishlist", Url = requestUri };

			var wishlist = GetOrCreateWishlist(requestParameters);

			var result = new Dictionary<string, string>();
			if (wishlist != null)
			{
				wishlist.Status = OrderStatus.Incomplete;
				wishlist.Name = string.Empty;
				wishlist.CustomerInfo.CustomerId = 0;
				wishlist.CustomerInfo.LoginName = string.Empty;
				wishlist.Save();

				Session.Add(Constants.WishlistRemoveActionResult, BasketActionResult.Success);
				result.Add(Constants.WishlistRemoveActionResult, BasketActionResult.Success.ToString());
				handleObject.Success = true;
			}
			else
			{
				Session.Add(Constants.WishlistRemoveActionResult, BasketActionResult.Failed);
				result.Add(Constants.WishlistRemoveActionResult, BasketActionResult.Failed.ToString());
				handleObject.Success = false;
			}

			handleObject.Messages = result;
			return handleObject;
		}

		internal HandleObject WishlistToBasket(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject { Action = "AddWishlistToBasket", Url = requestUri };

			var result = new Dictionary<string, string>();

			var order = OrderHelper.GetOrder();

			if (order == null)
			{
				order = GetCreateOrderOrWishList(requestParameters);

				order.Status = OrderStatus.Incomplete;
			}
			else
			{
				var wishlist = GetOrCreateWishlist(requestParameters);

				order.Status = OrderStatus.Incomplete;

				foreach (var line in wishlist.OrderLines)
				{
					Dictionary<string, string> dictionary = null;
					var xElement = line.CustomData.Element("fields");

					if (xElement != null)
					{
						dictionary = xElement.Elements().ToDictionary(e => e.Name.LocalName, e => e.Value);
					}

					order.AddOrUpdateOrderLine(0, line.ProductInfo.Id, "add", line.ProductInfo.ItemCount.GetValueOrDefault(1), line.ProductInfo.ProductVariants.Select(v => v.Id), dictionary);
				}

				
			}

			Session.Add(Constants.WishlistToBasketActionResult, BasketActionResult.Success);
			result.Add(Constants.WishlistToBasketActionResult, BasketActionResult.Success.ToString());

			order.Save();

			handleObject.Validated = false;
			handleObject.Success = true;
			SetBasket(handleObject, order);
			handleObject.Messages = result;
			return handleObject;
		}

		internal HandleObject OrderToBasket(NameValueCollection requestParameters, Uri requestUri)
		{
			var handleObject = new HandleObject {Action = "AddOrderToBasket", Url = requestUri};

			var result = new Dictionary<string, string>();

			string renewOrderKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "orderguid");
			if (renewOrderKey == null)
			{
				handleObject.Validated = false;
				handleObject.Success = false;
				handleObject.Messages = result;
				return handleObject;
			}

			var orderGuid = Guid.Parse(requestParameters[renewOrderKey]);

			var renewOrder = OrderHelper.GetOrder(orderGuid);

			var currentOrder = OrderHelper.GetOrder();

			if (currentOrder == null)
			{
				currentOrder = OrderHelper.CreateOrder();
			}

			currentOrder.Status = OrderStatus.Incomplete;

			foreach (var line in renewOrder.OrderLines)
			{
				Dictionary<string, string> dictionary = null;
				var xElement = line.CustomData.Element("fields");

				if (xElement != null)
				{
					dictionary = xElement.Elements().ToDictionary(e => e.Name.LocalName, e => e.Value);
				}

				currentOrder.AddOrUpdateOrderLine(0, line.ProductInfo.Id, "add", line.ProductInfo.ItemCount.GetValueOrDefault(1),
					line.ProductInfo.ProductVariants.Select(v => v.Id), dictionary);
			}
			
			Session.Add(Constants.OrderToBasketActionResult, BasketActionResult.Success);
			result.Add(Constants.OrderToBasketActionResult, BasketActionResult.Success.ToString());

			currentOrder.Save();

			handleObject.Validated = false;
			handleObject.Success = true;
			SetBasket(handleObject, currentOrder);
			handleObject.Messages = result;
			return handleObject;
		}


		private ConcurrentDictionary<Guid, object> _orderLocks = new ConcurrentDictionary<Guid, object>();
		private object GetLockForOrder(Guid order)
		{
			return _orderLocks.GetOrAdd(order, new object());
		}

		internal HandleObject AddProduct(NameValueCollection requestParameters, Uri requestUri)
		{
			// you want to lock the order here
			lock (GetLockForOrder(IO.Container.Resolve<IOrderService>().GetOrderIdFromOrderIdCookie()))
			{
				var order = GetCreateOrderOrWishList(requestParameters);

				var handleObject = new HandleObject {Action = "AddProduct", Url = requestUri};

				var orderUpdater = IO.Container.Resolve<IOrderUpdatingService>();

				string productIdKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "productid");
				string orderLineIdKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "orderlineid");

				string productIdValue = requestParameters[productIdKey];
				string orderlineIdValue = requestParameters[orderLineIdKey];

				int productId = 0;
				int orderLineId = 0;

				int.TryParse(productIdValue, out productId);
				int.TryParse(orderlineIdValue, out orderLineId);

				var result = new Dictionary<string, string>();

				if (productId <= 0 && orderLineId <= 0)
				{
					Log.Instance.LogError("ADD TO BASKET ERROR: productId <= 0 && orderLineId <= 0");
					if (HttpContext.Current != null)
					{
						Session.Add(Constants.BasketActionResult, BasketActionResult.NoProductOrOrderLineId);
					}
					result.Add(Constants.BasketActionResult, BasketActionResult.NoProductOrOrderLineId.ToString());
					handleObject.Success = false;
					SetBasket(handleObject, order);
					return handleObject;
				}

				string actionKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "action");
				string actionValue = requestParameters[actionKey];
				string action = !string.IsNullOrEmpty(actionValue) ? actionValue : string.Empty;

				string quantityKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "quantity");
				string quantityValue = requestParameters[quantityKey];

				int quantity;
				int.TryParse(quantityValue, out quantity);

				var variants = requestParameters.Keys.Cast<string>().Where(key => key.ToLower().StartsWith("variant")).Select(key => Common.Helpers.ParseInt(requestParameters[key])).ToList();

				// todo: varianten als key/value pair met groepnaam zodat we dit op order kunnen checken dat er maar 1 per groep is?
				//var variantGroupDictionary = new Dictionary<string, int>();
				//foreach(var variantGroupKey in requestParameters.AllKeys.Where(x => x.StartsWith("variant")))
				//{
				//	var variantIdValue = requestParameters[variantGroupKey];
				//	int variantId;
				//	int.TryParse(variantIdValue, out variantId);

				//	if (variantId != 0)
				//	{
				//		variantGroupDictionary.Add(variantGroupKey, variantId);
				//	}
				//}

				List<string> orderlineDetailsQueryStringCollection = requestParameters.AllKeys.Where(x => x != null).ToList();

				Dictionary<string, string> fields = orderlineDetailsQueryStringCollection.Where(s => !string.IsNullOrEmpty(requestParameters[s])).ToDictionary(s => s, s => requestParameters[s]);

				if (order != null)
				{
					Log.Instance.LogDebug("ADD TO BASKET DEBUG order: " + order.UniqueOrderId + " orderLineId: " + orderLineId + " productId: " + productId + " action: " + action + " quantity: " + quantity + " variantsCount: " + variants.Count() + " fieldsCount: " + fields.Count());

					orderUpdater.AddOrUpdateOrderLine(order, orderLineId, productId, action, quantity, variants, fields);
					// unit test dat dit aangeroepen wordt
					orderUpdater.Save(order);

					if (HttpContext.Current != null)
					{
						Session.Add(order.Status == OrderStatus.Wishlist ? Constants.WishlistActionResult : Constants.BasketActionResult, BasketActionResult.Success);
					}
					result.Add(order.Status == OrderStatus.Wishlist ? Constants.WishlistActionResult : Constants.BasketActionResult, BasketActionResult.Success.ToString());
					handleObject.Success = true;
					SetBasket(handleObject, order);
				}
				else
				{
					if (HttpContext.Current != null)
					{
						Session.Add(Constants.WishlistActionResult, BasketActionResult.OrderNull);
						Session.Add(Constants.BasketActionResult, BasketActionResult.OrderNull);
					}
					result.Add(Constants.WishlistActionResult, BasketActionResult.OrderNull.ToString());
					result.Add(Constants.BasketActionResult, BasketActionResult.OrderNull.ToString());
					handleObject.Success = false;

				}
				handleObject.Messages = result;
				return handleObject;
			}
		}
	}
}