using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Razor;
using System.Web.Routing;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Model;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Macros;
using Umbraco.Web.Mvc;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	///     Helper class with e-mail related functions
	/// </summary>
	public static class EmailHelper
	{
		/// <summary>
		/// Renders the output of a view based on a controller viewname and model
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="viewName"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public static string RenderPartialViewToString(Controller controller, string viewName, object model)
		{
			controller.ViewData.Model = model;

			using (var sw = new StringWriter())
			{
				var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
				var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData,
					controller.TempData, sw);
				viewResult.View.Render(viewContext, sw);

				return sw.GetStringBuilder().ToString();
			}

		}

		/// <summary>
		/// Send order emails
		/// </summary>
		/// <param name="storeEmailContentId"></param>
		/// <param name="customerEmailContentId"></param>
		/// <param name="orderInfo"></param>
		/// <param name="emailAddress"></param>
		/// <param name="parameters"></param>
		public static void SendOrderEmail(int storeEmailContentId, int customerEmailContentId, OrderInfo orderInfo, string emailAddress = null, Dictionary<string, object> parameters = null)
		{
			// set the culture for this request to the store culture, so dictionary items are rendered properly
			var currentCulture = Thread.CurrentThread.CurrentCulture;
			var currentUICulture = Thread.CurrentThread.CurrentUICulture;
			Thread.CurrentThread.CurrentCulture = orderInfo.StoreInfo.CultureInfo;
			Thread.CurrentThread.CurrentUICulture = orderInfo.StoreInfo.CultureInfo;

			string emailBody;
			var receivers = new List<string>();
			if (storeEmailContentId > 0)
			{
				var storeEmail = new Email(storeEmailContentId);
				var model = GenerateEmailModel(storeEmail, orderInfo);
				emailBody = RenderEmailBody(model, storeEmail, parameters);

				var emailTo = orderInfo.StoreInfo.Store.EmailAddressTo;

				if (!emailTo.Contains(";"))
				{
					receivers.Add(emailTo);
				}
				else
				{
					// store email address field can contain a ; to sent to muliple addresses
					var emailToArray = emailTo.Split(';');
					receivers.AddRange(emailToArray);
				}

				foreach (var receiver in receivers)
				{
					SendMail(orderInfo, receiver, orderInfo.StoreInfo.Store.EmailAddressFrom, model.Title, emailBody,
						orderInfo.StoreInfo.Store.EmailAddressFromName);
				}
			}

			if (customerEmailContentId > 0)
			{
				var customerEmail = new Email(customerEmailContentId);
				var model = GenerateEmailModel(customerEmail, orderInfo);
				emailBody = RenderEmailBody(model, customerEmail, parameters);
				receivers.Add(emailAddress ?? OrderHelper.CustomerInformationValue(orderInfo, "customerEmail"));

				foreach (var receiver in receivers)
				{
					SendMail(orderInfo, receiver, orderInfo.StoreInfo.Store.EmailAddressFrom, model.Title, emailBody,
						orderInfo.StoreInfo.Store.EmailAddressFromName);
				}
			}
			
			// change the culture back to what it was
			Thread.CurrentThread.CurrentCulture = currentCulture;
			Thread.CurrentThread.CurrentUICulture = currentUICulture;
		}

		/// <summary>
		/// Creates an EmailRenderModel based on EmailContentId and the OrderInfo Object
		/// </summary>
		/// <param name="email"></param>
		/// <param name="orderInfo"></param>
		/// <returns></returns>
		public static EmailRenderModel GenerateEmailModel(Email email, OrderInfo orderInfo)
		{
			var orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(orderInfo);
			var orderInfoXml = new XmlDocument();
			orderInfoXml.LoadXml(orderInfoXmlstring);
			
			var IOrder = API.Orders.GetOrder(orderInfo.UniqueOrderId);

			return new EmailRenderModel
			{
				Id = email.Id,
				Order = IOrder,
				OrderInfo = orderInfo,
				Title = ReplaceStrings(email.Title, orderInfoXml),
				Description = ReplaceStrings(email.Description, orderInfoXml)
			};
		}

		/// <summary>
		/// Renders de body of the email based on xslt or razor.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="email"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static string RenderEmailBody(EmailRenderModel model, Email email, Dictionary<string, object> parameters = null)
		{
			// legacy email template picker
			if (!string.IsNullOrEmpty(email.Template))
			{
				if (parameters == null) parameters = new Dictionary<string, object>();
				parameters.Add("Order", model.OrderInfo);
				parameters.Add("uniqueOrderId", model.Order.UniqueId.ToString());
				parameters.Add("storeAlias", model.Order.Store.Alias);
				parameters.Add("Title", model.Title);
				parameters.Add("Description", model.Description);

				if (email.Template.EndsWith(".xslt"))
				{
					// generate XML from orderInfo
					var orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(model.OrderInfo);
					var orderInfoXml = new XmlDocument();
					orderInfoXml.LoadXml(orderInfoXmlstring);
					return IO.Container.Resolve<ICMSApplication>().RenderXsltMacro(email.Template, parameters, orderInfoXml);
				}
				if (email.Template.EndsWith(".cshtml"))
				{
					return IO.Container.Resolve<ICMSApplication>().RenderMacro(email.Template, email.Id, parameters);
				}
			}

			//todo: place in Umbraco project instead of here
			var umbracoContext = UmbracoContext.Current;
			var helper = new UmbracoHelper(umbracoContext);
			var emailContent = helper.TypedContent(email.Id);
			var templateAlias = emailContent.GetTemplateAlias();
			var controller = new RenderEmailController();
			var routeData = new RouteData();
			routeData.Values.Add("controller", "RenderEmailController");
			routeData.Values.Add("action", "Index");

			controller.ControllerContext = new ControllerContext(umbracoContext.HttpContext, routeData, controller);

			if (!string.IsNullOrEmpty(templateAlias))
			{
				return RenderPartialViewToString(controller, templateAlias, email);
			}

			Log.Instance.LogError("No template defined for email with Id: " + model.Id);
			return string.Empty;
		}
	
		/// <summary>
		/// Sends the member email customer.
		/// </summary>
		/// <param name="emailNodeId">The email node unique identifier.</param>
		/// <param name="currentStore">The current store.</param>
		/// <param name="loginName">Name of the login.</param>
		/// <param name="password">The password.</param>
		/// <param name="parameters">The XSLT parameters.</param>
		public static void SendMemberEmailCustomer(int emailNodeId, Store currentStore, string loginName, string password, Dictionary<string, object> parameters = null)
		{
			if (emailNodeId != 0 && !string.IsNullOrEmpty(loginName))
			{
				var member = Membership.GetUser(loginName);

				if (member != null)
				{
					var currentCulture = Thread.CurrentThread.CurrentCulture;
					var currentUICulture = Thread.CurrentThread.CurrentUICulture;

					Thread.CurrentThread.CurrentCulture = currentStore.CultureInfo;
					Thread.CurrentThread.CurrentUICulture = currentStore.CultureInfo;

					var email = new Email(emailNodeId);

					if (string.IsNullOrEmpty(email.Template))
					{
						Log.Instance.LogWarning("SendMemberEmailCustomer nodeId: " + emailNodeId + " No Template Defined");

						return;
					}

					var emailTitle = email.Title;
					var emailText = email.Description;

					if (parameters == null) parameters = new Dictionary<string, object>();
					parameters.Add("storeAlias", currentStore.Alias);
					parameters.Add("Title", emailTitle);
					parameters.Add("Description", emailText);
					parameters.Add("UserName", member.UserName);
					parameters.Add("Password", password);

					string body;

					if (email.Template.EndsWith(".xslt"))
					{
						body = IO.Container.Resolve<ICMSApplication>().RenderXsltMacro(email.Template, parameters);
					}
					else
					{
						body = IO.Container.Resolve<ICMSApplication>().RenderMacro(email.Template, email.Id, parameters);
					}

					string emailTo = member.Email;

					if (string.IsNullOrEmpty(emailTo))
					{
						emailTo = member.UserName;
					}

					SendMail(null, emailTo, currentStore.EmailAddressFrom, emailTitle, body, currentStore.EmailAddressFromName);

					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;
				}
			}
			else
			{
				Log.Instance.LogError("SendMemberEmailCustomer: emailNodeId == 0 || member == null");
			}
		}
		
		/// <summary>
		/// Replaces the strings.
		/// </summary>
		/// <param name="stringToReplace">The string automatic replace.</param>
		/// <param name="xmlDoc">The XML document.</param>
		/// <returns></returns>
		public static string ReplaceStrings(string stringToReplace, XmlDocument xmlDoc)
		{
			XDocument doc = XDocument.Parse(xmlDoc.OuterXml);

			MatchCollection matches = Regex.Matches(stringToReplace, "#[a-zA-Z\\d]+#");

			var replacements = new Dictionary<string, string>();
			foreach (Match match in matches)
			{
				XElement element = doc.Descendants().FirstOrDefault(x => x.Name.LocalName == match.Value.Replace("#", string.Empty));

				if (element != null && !replacements.ContainsKey(match.Value))
				{
					replacements.Add(match.Value, element.Value);
				}
			}

			foreach (var replacement in replacements)
			{
				string value = replacement.Value;
				if (replacement.Key == "#ConfirmDate#")
				{
					XElement cultureNode = doc.Descendants("CurrencyCulture").FirstOrDefault();
					if (cultureNode != null)
					{
						DateTime confirmDate = DateTime.Parse(replacement.Value, new CultureInfo(cultureNode.Value));

						value = confirmDate.ToString("f");
					}
				}
				stringToReplace = stringToReplace.Replace(replacement.Key, value);
			}


			return stringToReplace;
		}

		private static void SendMail(OrderInfo order, string to, string @from, string subject, string body)
		{
			SendMail(order, to, @from, subject, body, string.Empty);
		}

		private static void SendMail(OrderInfo order, string to, string @from, string subject, string body, string nameFrom)
		{
			try
			{
				if (string.IsNullOrEmpty(nameFrom))
				{
					nameFrom = @from;
				}
				var msg = new MailMessage {From = new MailAddress(from, nameFrom), Subject = subject, Body = body, IsBodyHtml = true};
				
				msg.To.Add(new MailAddress(to));

				var client = new SmtpClient();

				Log.Instance.LogDebug("SendMail before send event");
				if (BeforeSendEmail != null)
				{
					var beforeSendEmailEventArgs = new BeforeSendEmailEventArgs {Order = order, Email = new UwebshopEmailAdaptor(msg)};

					BeforeSendEmail(beforeSendEmailEventArgs);
				}
				Log.Instance.LogDebug("SendMail before client.send");
				client.Send(msg);
				Log.Instance.LogDebug("SendMail after client.send");
			}
			catch (Exception ex)
			{
				Log.Instance.LogError("The following error occured while trying to send the " + subject + " e-mail: " + ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e">The <see cref="BeforeSendEmailEventArgs"/> instance containing the event data.</param>
		public delegate void BeforeSendEmailEventHandler(BeforeSendEmailEventArgs e);

		/// <summary>
		/// Occurs just before an email is sent.
		/// </summary>
		public static event BeforeSendEmailEventHandler BeforeSendEmail;

		/// <summary>
		/// 
		/// </summary>
		public class BeforeSendEmailEventArgs : EventArgs
		{
			/// <summary>
			/// Gets the order.
			/// </summary>
			/// <value>
			/// The order.
			/// </value>
			public OrderInfo Order { get; internal set; }

			/// <summary>
			/// Gets the email.
			/// </summary>
			/// <value>
			/// The email.
			/// </value>
			public IEmailModifier Email { get; internal set; }
		}

		/// <summary>
		/// 
		/// </summary>
		public interface IEmailModifier
		{
			/// <summary>
			/// Adds the attachment.
			/// </summary>
			/// <param name="fileName">Name of the file.</param>
			void AddAttachment(string fileName);
		}

		private class UwebshopEmailAdaptor : IEmailModifier
		{
			private readonly MailMessage _msg;

			public UwebshopEmailAdaptor(MailMessage msg)
			{
				_msg = msg;
			}

			public void AddAttachment(string fileName)
			{
				_msg.Attachments.Add(new Attachment(fileName));
			}
		}
	}
}