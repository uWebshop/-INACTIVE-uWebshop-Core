using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using System.Xml;
using Umbraco.Web.Mvc;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using umbraco.NodeFactory;
using umbraco.cms.businesslogic.member;

namespace uWebshop.Umbraco.Mvc
{
    //public class OrderController : SurfaceController
    //{
    //    [HttpPost]
    //    public ActionResult Cancel()
    //    {
            
    //    }
    //}

	public class BasketHandlerController : SurfaceController
	{
		[HttpPost]
		public ActionResult Handle()
		{
			var refferer = HttpContext.Request.UrlReferrer;

			if (!string.IsNullOrEmpty(HttpContext.Request.RawUrl))
			{
				if (HttpContext.Request.Url != null && !HttpContext.Request.RawUrl.StartsWith("http"))
				{
					var strPathAndQuery = HttpContext.Request.Url.PathAndQuery;
					var strUrl = HttpContext.Request.Url.AbsoluteUri.Replace(strPathAndQuery, string.Empty);

					refferer = new Uri(string.Format("{0}{1}", strUrl, HttpContext.Request.RawUrl));
				}
				else if (HttpContext.Request.RawUrl.StartsWith("http"))
				{
					refferer = new Uri(HttpContext.Request.RawUrl);
				}
			}


			var redirectAfterHandle = new BasketRequestHandler().HandleBasketRequest(Request.Form, refferer);

			if (Request.UrlReferrer == null)
			{
				return CurrentUmbracoPage();
			}

			foreach (var result in redirectAfterHandle)
			{
				Log.Instance.LogDebug("Handle Result: " + result.Action + "result.Success: " + result.Success + "result.Validated: " + result.Validated + "result.PostConfirmUrl: " + result.PostConfirmUrl);
			}
			
			var redirectUrl = HttpContext.Request.RawUrl;

			var goToUrlHandler = redirectAfterHandle.LastOrDefault(x => x.Url != null);

			if (goToUrlHandler != null)
			{
				var redirectUri = goToUrlHandler.Url;

				if (redirectUri != null)
				{
					redirectUrl = redirectUri.AbsoluteUri;
				}
			}

			var postConfirmUrlHandler = redirectAfterHandle.LastOrDefault(x => x.PostConfirmUrl != null);

			if (postConfirmUrlHandler != null)
			{
				var postConfirmUrl = postConfirmUrlHandler.PostConfirmUrl;

				if (postConfirmUrl != null)
				{
					redirectUrl = postConfirmUrl.AbsoluteUri;
				}
			}

			var validateOrderReffererKey = Request.Form.AllKeys.FirstOrDefault(x => x.ToLower() == "backtoreferreronerror");
			if (validateOrderReffererKey != null && redirectAfterHandle.Any(x => x.Validated == false))
			{
				var validateOrderRefferer = Request.Form[validateOrderReffererKey];

				if ((validateOrderRefferer.ToLower() == "true" || validateOrderRefferer.ToLower() == "backtoreferreronerror" || validateOrderRefferer.ToLower() == "on" || validateOrderRefferer == "1") && HttpContext.Request.UrlReferrer != null)
				{
					redirectUrl = HttpContext.Request.UrlReferrer.AbsolutePath;
				}
			}

			Log.Instance.LogDebug("Handle Result redirectUrl: " + redirectUrl);

			return Redirect(redirectUrl);

		}
		
		[HttpPost]
		public ActionResult AddProduct(int productId, int quantity, Dictionary<string, int> variants, Dictionary<string, string> property, int orderLineId = 0)
		{
			var order = OrderHelper.GetOrder() ?? OrderHelper.CreateOrder();

			var orderUpdater = IO.Container.Resolve<IOrderUpdatingService>();


			if (productId > 0 || orderLineId > 0)
			{
				orderUpdater.AddOrUpdateOrderLine(order, orderLineId, productId, "add", quantity, variants.Select(v => v.Value), property); // unit test dat dit aangeroepen wordt
				orderUpdater.Save(order);
			}

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult UpdateOrderline(int orderLineId, int quantity, Dictionary<string, int> variants, Dictionary<string, string> property, int productId = 0)
		{
			var order = OrderHelper.GetOrder() ?? OrderHelper.CreateOrder();

			var orderUpdater = IO.Container.Resolve<IOrderUpdatingService>();


			if (productId > 0 || orderLineId > 0)
			{
				orderUpdater.AddOrUpdateOrderLine(order, orderLineId, productId, "update", quantity, variants.Select(v => v.Value), property); // unit test dat dit aangeroepen wordt
				orderUpdater.Save(order);
			}

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ValidateOrder(OrderInfo orderinfo)
		{
			if (OrderHelper.ValidateOrder(orderinfo))
			{
				// order is valid, ok!
				return CurrentUmbracoPage();
			}
			orderinfo.Save(true, ValidateSaveAction.Order);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ValidateCustomValidation(OrderInfo orderinfo)
		{
			if (OrderHelper.ValidateCustomValidation(orderinfo, true))
			{
				// order is valid, ok!
				return CurrentUmbracoPage();
			}
			orderinfo.Save(true, ValidateSaveAction.CustomValidation);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ValidateOrderlines(OrderInfo orderinfo)
		{
			if (OrderHelper.ValidateOrderLines(orderinfo, true))
			{
				// order is valid, ok!
				return CurrentUmbracoPage();
			}
			orderinfo.Save(true, ValidateSaveAction.Orderlines);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ValidateStock(OrderInfo orderinfo)
		{
			if (OrderHelper.ValidateStock(orderinfo, true))
			{
				// order is valid, ok!
				return CurrentUmbracoPage();
			}
			orderinfo.Save(true, ValidateSaveAction.Stock);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ValidateCustomer(OrderInfo orderinfo)
		{
			if (OrderHelper.ValidateCustomer(orderinfo, true))
			{
				// order is valid, ok!
				return CurrentUmbracoPage();
			}
			orderinfo.Save(true, ValidateSaveAction.Customer);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ClearOrderlines(OrderInfo orderinfo)
		{
			orderinfo.OrderLines.Clear();

			orderinfo.Save();

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult ConfirmOrder(OrderInfo orderInfo, bool acceptTerms = false, int fallbackConfirmOrderNodeId = 0)
		{
			if (!orderInfo.ConfirmOrder(acceptTerms, fallbackConfirmOrderNodeId))
			{
				return CurrentUmbracoPage();
			}

			if (orderInfo.PaymentInfo.TransactionMethod == PaymentTransactionMethod.WebClient)
			{
				Response.Write(Encoding.UTF8.GetString(GetBytes(orderInfo.PaymentInfo.Parameters)));
				
			}
			else
			{
				if (!string.IsNullOrEmpty(orderInfo.RedirectUrl))
				{
					Response.Redirect(orderInfo.RedirectUrl, true);
				}
			}

			return CurrentUmbracoPage();
		}

		internal static byte[] GetBytes(string str)
		{
			var bytes = new byte[str.Length*sizeof (char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		[HttpPost]
		public ActionResult AddCoupon(string couponCode)
		{
			var orderInfo = OrderHelper.GetOrder() ?? OrderHelper.CreateOrder();

			var saveOrder = true;

			CouponCodeResult result = orderInfo.AddCoupon(couponCode);
			if (result != CouponCodeResult.Success)
			{
				saveOrder = false;
			}

			if (saveOrder)
			{
				orderInfo.Save();
			}

			Session.Add(Constants.CouponCodeSessionKey, result);

			return CurrentUmbracoPage();
		}

		public ActionResult AddShippingProvider(int shippingProviderId, string shippingProviderMethodId, bool autoselect = false)
		{
			var orderInfo = OrderHelper.GetOrder() ?? OrderHelper.CreateOrder();

			if (autoselect)
			{
				ShippingProviderHelper.AutoSelectShippingProvider(orderInfo);

				orderInfo.Save();
			}
			else
			{
				if (shippingProviderId == 0 || string.IsNullOrEmpty(shippingProviderMethodId))
				{
					Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput);
					return CurrentUmbracoPage();
				}

				var result = orderInfo.AddShippingProvider(shippingProviderId, shippingProviderMethodId);

				orderInfo.Save();

				Session.Add(Constants.ShippingProviderSessionKey, result);
			}

			return CurrentUmbracoPage();
		}

		public ActionResult AddPaymentProvider(int paymentProviderId, string paymentProviderMethodId)
		{
			var orderInfo = OrderHelper.GetOrCreateOrder();


			if (paymentProviderId == 0 || string.IsNullOrEmpty(paymentProviderMethodId))
			{
				Session.Add(Constants.ShippingProviderSessionKey, ProviderActionResult.NoCorrectInput);
				return CurrentUmbracoPage();
			}

			var result = orderInfo.AddPaymentProvider(paymentProviderId, paymentProviderMethodId);

			orderInfo.Save();

			Session.Add(Constants.ShippingProviderSessionKey, result);


			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult RemoveCoupon(string couponCode)
		{
			var orderInfo = OrderHelper.GetOrder();

			var saveOrder = true;

			CouponCodeResult result = orderInfo.RemoveCoupon(couponCode);
			if (result != CouponCodeResult.Success)
			{
				saveOrder = false;
			}


			if (saveOrder)
			{
				orderInfo.Save();
			}

			Session.Add(Constants.CouponCodeSessionKey, result);

			return CurrentUmbracoPage();
		}


		[HttpPost]
		public ActionResult AccountCreate(string email, string password, string validatePassword, Dictionary<string, string> property = null, string username = "", bool generatePassword = false)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.CustomerEmailEmpty);
				return CurrentUmbracoPage();
			}

			if (string.IsNullOrEmpty(username))
			{
				username = email;
			}

			if (!Regex.IsMatch(email, "\\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,4}\\b", RegexOptions.IgnoreCase))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.EmailAddressNotValid);
				return CurrentUmbracoPage();
			}

			if (generatePassword == false)
			{
				if (password == validatePassword)
				{
					if (string.IsNullOrEmpty(password))
					{
						Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.Failed);
						return CurrentUmbracoPage();
					}
					if (Membership.MinRequiredPasswordLength > 0)
					{
						if (password.Length < Membership.MinRequiredPasswordLength)
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MinRequiredPasswordLengthError);
							Session.Add(Constants.CreateMemberSessionKeyAddition, Membership.MinRequiredPasswordLength);
							Log.Instance.LogError("MinRequiredPasswordLengthError");
							return CurrentUmbracoPage();
						}
					}

					if (!string.IsNullOrEmpty(Membership.PasswordStrengthRegularExpression))
					{
						if (!Regex.IsMatch(password, Membership.PasswordStrengthRegularExpression))
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.PasswordStrengthRegularExpressionError);
							Session.Add(Constants.CreateMemberSessionKeyAddition, Membership.PasswordStrengthRegularExpression);
							Log.Instance.LogError("PasswordStrengthRegularExpression");
							return CurrentUmbracoPage();
						}
					}

					if (Membership.MinRequiredNonAlphanumericCharacters > 0)
					{
						var num = password.Where((t, i) => !char.IsLetterOrDigit(password, i)).Count();

						if (num < Membership.MinRequiredNonAlphanumericCharacters)
						{
							Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MinRequiredNonAlphanumericCharactersError);
							Session.Add(Constants.CreateMemberSessionKeyAddition, Membership.MinRequiredNonAlphanumericCharacters);
							Log.Instance.LogError("MinRequiredNonAlphanumericCharacters");
							return CurrentUmbracoPage();
						}
					}
				}
				else
				{
					Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.PasswordMismatch);
					Log.Instance.LogError("PasswordMismatch");
					return CurrentUmbracoPage();
				}
			}
			else
			{
				password = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
			}

			var webConfig = new XmlDocument();
			webConfig.Load(Server.MapPath("~/web.config"));

			var umbracoDefaultMemberTypeAlias = webConfig.SelectSingleNode("//add[@defaultMemberTypeAlias]");

			// todo: is this possible without Umbraco API? MemberType seems to be a problem?
			var memberTypes = MemberType.GetAll;

			if (umbracoDefaultMemberTypeAlias == null || umbracoDefaultMemberTypeAlias.Attributes == null)
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.DefaultMemberTypeAliasError);
				Log.Instance.LogError("DefaultMemberTypeAliasError");
				return CurrentUmbracoPage();
			}
			var memberTypevalue = umbracoDefaultMemberTypeAlias.Attributes["defaultMemberTypeAlias"].Value;

			if (memberTypes.All(x => x.Alias != memberTypevalue))
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.DefaultMemberTypeAliasNonExistingError);
				Log.Instance.LogError("DefaultMemberTypeAliasNonExistingError");
				return CurrentUmbracoPage();
			}

			var member = Membership.GetUserNameByEmail(username);

			if (member != null)
			{
				Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.MemberExists);
				Log.Instance.LogError("MemberExists");
				return CurrentUmbracoPage();
			}

			var membershipUser = Membership.CreateUser(username, password, email);

			//var roleKey = requestParameters.AllKeys.FirstOrDefault(x => x.ToLower() == "membergroup");
			//var roleValue = requestParameters[roleKey];

			//if (!string.IsNullOrEmpty(roleValue))
			//{
			//	if (Roles.RoleExists(roleValue))
			//	{
			//		Roles.AddUserToRole(membershipUser.UserName, roleValue);
			//	}
			//	else
			//	{
			//		Roles.CreateRole(roleValue);

			//		Roles.AddUserToRole(membershipUser.UserName, roleValue);
			//	}
			//}
			//else
			//{
			//	if (!Roles.GetAllRoles().Any())
			//	{
			//		const string customersRole = "Customers";

			//		Roles.CreateRole(customersRole);

			//		Roles.AddUserToRole(membershipUser.UserName, customersRole);
			//	}
			//	else
			//	{
			//		Roles.AddUserToRole(membershipUser.UserName, Roles.GetAllRoles().First());
			//	}
			//}

			var profile = ProfileBase.Create(username);

			foreach (var prop in ProfileBase.Properties)
			{
				try
				{
					var settingsProperty = (SettingsProperty) prop;

					var settingsPropertyName = settingsProperty.Name;

					if (!string.IsNullOrEmpty(property[settingsPropertyName]))
					{
						profile[settingsPropertyName] = property[settingsPropertyName];
					}
				}
				catch
				{
					var settingsProperty = (SettingsProperty) prop;

					Log.Instance.LogDebug(string.Format("UpdateAccount Failed for Request Property: {0}, currentNodeId: {1}", settingsProperty.Name, Node.getCurrentNodeId()));
				}
			}

			profile.Save();

			FormsAuthentication.SetAuthCookie(username, true);

			Session.Add(Constants.CreateMemberSessionKey, AccountActionResult.Success);

			var currentStore = StoreHelper.GetCurrentStore();

			if (string.IsNullOrEmpty(currentStore.AccountCreatedEmail))
			{
				Log.Instance.LogDebug("CreateAccount: AccountCreatedEmail not set: No email send to customer");
				return CurrentUmbracoPage();
			}

			var emailNodeId = Convert.ToInt32(currentStore.AccountCreatedEmail);
			EmailHelper.SendMemberEmailCustomer(emailNodeId, currentStore, username, password);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult AccountSignIn(string login, string password)
		{
			var user = Membership.GetUser(login);


			if (user == null)
			{
				Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.MemberNotExists);
				return CurrentUmbracoPage();
			}

			if (!Membership.ValidateUser(login, password))
			{
				Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.ValidateUserError);
				return CurrentUmbracoPage();
			}

			FormsAuthentication.SetAuthCookie(login, true);

			Session.Add(Constants.SignInMemberSessionKey, AccountActionResult.Success);

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult AccountSignOut()
		{
			FormsAuthentication.SignOut();

			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult AccountRequestPassword(string login)
		{
			if (string.IsNullOrEmpty(login))
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.CustomerEmailEmpty);
				return CurrentUmbracoPage();
			}

			var currentStore = StoreHelper.GetCurrentStore();

			if (string.IsNullOrEmpty(currentStore.AccountForgotPasswordEmail))
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.AccountForgotPasswordEmailNotConfigured);
				return CurrentUmbracoPage();
			}
			var emailNodeId = Convert.ToInt32(currentStore.AccountForgotPasswordEmail);

			var user = Membership.GetUser(login);


			if (user == null)
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.MemberNotExists);
				return CurrentUmbracoPage();
			}


			if (!Membership.EnablePasswordReset)
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.EnablePasswordResetDisabled);
				return CurrentUmbracoPage();
			}

			var resetPassword = user.ResetPassword();

			var newPassword = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);

			if (user.ChangePassword(resetPassword, newPassword))
			{
				EmailHelper.SendMemberEmailCustomer(emailNodeId, currentStore, login, newPassword);
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.Success);
			}
			else
			{
				Session.Add(Constants.RequestPasswordSessionKey, AccountActionResult.ChangePasswordError);
			}


			return CurrentUmbracoPage();
		}

		[HttpPost]
		public ActionResult AccountUpdate(Dictionary<string, string> property = null, bool generatePassword = false, string currentPassword = null, string newPassword = null, string validatePassword = null)
		{
			var memberShipUser = UwebshopRequest.Current.User;

			if (memberShipUser == null)
			{
				Log.Instance.LogDebug("UpdateAccount: " + AccountActionResult.MemberNotExists);
				Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.MemberNotExists);
				return CurrentUmbracoPage();
			}

			var profile = ProfileBase.Create(memberShipUser.UserName);

			// note don't forget to disable automatic profile save!: <profile defaultProvider="UmbracoMemberProfileProvider" enabled="true" automaticSaveEnabled="false">
			foreach (var prop in ProfileBase.Properties)
			{
				try
				{
					var settingsProperty = (SettingsProperty) prop;

					var settingsPropertyName = settingsProperty.Name;

					if (!string.IsNullOrEmpty(property[settingsPropertyName]))
					{
						profile[settingsPropertyName] = property[settingsPropertyName];
					}
				}
				catch
				{
					var settingsProperty = (SettingsProperty) prop;

					Log.Instance.LogDebug(string.Format("UpdateAccount Failed for Request Property: {0}, currentNodeId: {1}", settingsProperty.Name, Node.getCurrentNodeId()));
				}
			}


			if (!string.IsNullOrEmpty(currentPassword) || !string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(validatePassword) || generatePassword)
			{
				Log.Instance.LogDebug("Update Account Password Section");
				if (generatePassword)
				{
					newPassword = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
				}
				else
				{
					if (newPassword != validatePassword)
					{
						Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.PasswordMismatch);
						return CurrentUmbracoPage();
					}

					if (string.IsNullOrEmpty(newPassword))
					{
						Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.Failed);
						return CurrentUmbracoPage();
					}

					if (Membership.MinRequiredPasswordLength > 0)
					{
						if (newPassword.Length < Membership.MinRequiredPasswordLength)
						{
							Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.MinRequiredPasswordLengthError);
							Session.Add(Constants.UpdateMemberSessionKeyAddition, Membership.MinRequiredPasswordLength);
							return CurrentUmbracoPage();
						}
					}

					if (!string.IsNullOrEmpty(Membership.PasswordStrengthRegularExpression))
					{
						if (!Regex.IsMatch(newPassword, Membership.PasswordStrengthRegularExpression))
						{
							Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.PasswordStrengthRegularExpressionError);
							Session.Add(Constants.UpdateMemberSessionKeyAddition, Membership.PasswordStrengthRegularExpression);
							return CurrentUmbracoPage();
						}
					}

					if (Membership.MinRequiredNonAlphanumericCharacters > 0)
					{
						var num = newPassword.Where((t, i) => !char.IsLetterOrDigit(newPassword, i)).Count();

						if (num < Membership.MinRequiredNonAlphanumericCharacters)
						{
							Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.MinRequiredNonAlphanumericCharactersError);
							Session.Add(Constants.UpdateMemberSessionKeyAddition, Membership.MinRequiredNonAlphanumericCharacters);
							return CurrentUmbracoPage();
						}
					}
				}

				if (!string.IsNullOrEmpty(currentPassword))
				{
					memberShipUser.ChangePassword(currentPassword, newPassword);
				}
				else
				{
					Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.ChangePasswordError);
					return CurrentUmbracoPage();
				}
			}

			profile.Save();

			Session.Add(Constants.UpdateMemberSessionKey, AccountActionResult.Success);

			return CurrentUmbracoPage();
		}
	}
}