using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
	/// <summary>
	///     Helper class with e-mail related functions
	/// </summary>
	public static class EmailHelper
	{
		/// <summary>
		/// Send an order email based on the emailNode and orderinfo
		/// </summary>
		/// <param name="emailNodeId"></param>
		/// <param name="orderInfo"></param>
		/// <param name="parameters"> </param>
		/// <param name="emailAddress"></param>
		public static void SendOrderEmailCustomer(int emailNodeId, OrderInfo orderInfo, Dictionary<string, object> parameters = null, string emailAddress = null)
		{
			if (emailNodeId != 0)
			{
				Log.Instance.LogDebug("SendOrderEmailCustomer emailNodeId: " + emailNodeId);
				Log.Instance.LogDebug(string.Format("SendOrderEmailCustomer orderInfo.UniqueOrderId: {0} orderInfo.OrderNumber: {1}", orderInfo.UniqueOrderId, orderInfo.OrderNumber));
				var currentCulture = Thread.CurrentThread.CurrentCulture;
				var currentUICulture = Thread.CurrentThread.CurrentUICulture;

				Thread.CurrentThread.CurrentCulture = orderInfo.StoreInfo.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = orderInfo.StoreInfo.CultureInfo;

				var orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(orderInfo);
				var orderInfoXml = new XmlDocument();
				orderInfoXml.LoadXml(orderInfoXmlstring);

				var email = new Email(emailNodeId);

				if (string.IsNullOrEmpty(email.Template))
				{
					Log.Instance.LogWarning("SendOrderEmailCustomer nodeId: " + emailNodeId + " No Template Defined");

					return;
				}

				var emailTitle = ReplaceStrings(email.Title, orderInfoXml);
				var emailText = ReplaceStrings(email.Description, orderInfoXml);

				if (parameters == null) parameters = new Dictionary<string, object>();
				parameters.Add("uniqueOrderId", orderInfo.UniqueOrderId.ToString());
				parameters.Add("storeAlias", orderInfo.StoreInfo.Alias);
				parameters.Add("Title", emailTitle);
				parameters.Add("Description", emailText);

				Log.Instance.LogDebug("SendOrderEmailCustomer email.Template: " + email.Template);

				string body;
				if (email.Template.EndsWith(".xslt"))
				{
					body = IO.Container.Resolve<ICMSApplication>().RenderXsltMacro(email.Template, parameters, orderInfoXml);
				}
				else 
				{
					if (email.Template.Contains("."))
					{
						body = IO.Container.Resolve<ICMSApplication>().RenderMacro(email.Template, email.Id, parameters);
					}
					else
					{
						Log.Instance.LogWarning("SendOrderEmailCustomer email.Template no valid value: " + email.Template + " no email send");
						return;
					}
				}

				var emailTo = emailAddress ?? OrderHelper.CustomerInformationValue(orderInfo, "customerEmail");
				Log.Instance.LogDebug("SendOrderEmailCustomer emailTo: " + emailTo);
				Log.Instance.LogDebug("SendOrderEmailCustomer EmailFrom: " + orderInfo.StoreInfo.Store.EmailAddressFrom);
				Log.Instance.LogDebug("SendOrderEmailCustomer EmailFromName: " + orderInfo.StoreInfo.Store.EmailAddressFromName);
				Log.Instance.LogDebug("SendOrderEmailCustomer emailTitle: " + emailTitle);
				SendMail(orderInfo, emailTo, orderInfo.StoreInfo.Store.EmailAddressFrom, emailTitle, body, orderInfo.StoreInfo.Store.EmailAddressFromName);

				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
			else
			{
				Log.Instance.LogError("SendOrderEmailCustomer: emailNodeId == 0");
			}
		}

		/// <summary>
		/// Sends the order email store.
		/// </summary>
		/// <param name="emailNodeId">The email node unique identifier.</param>
		/// <param name="orderInfo">The order information.</param>
		/// <param name="parameters">The XSLT parameters.</param>
		public static void SendOrderEmailStore(int emailNodeId, OrderInfo orderInfo, Dictionary<string, object> parameters = null)
		{
			if (emailNodeId != 0)
			{
				Log.Instance.LogDebug("SendOrderEmailStore emailNodeId: " + emailNodeId);
				Log.Instance.LogDebug(string.Format("SendOrderEmailStore orderInfo.UniqueOrderId: {0} orderInfo.OrderNumber: {1}", orderInfo.UniqueOrderId, orderInfo.OrderNumber));
				CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
				CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;

				Thread.CurrentThread.CurrentCulture = orderInfo.StoreInfo.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = orderInfo.StoreInfo.CultureInfo;


				string orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(orderInfo);
				var orderInfoXml = new XmlDocument();
				orderInfoXml.LoadXml(orderInfoXmlstring);

				var email = new Email(emailNodeId);

				if (string.IsNullOrEmpty(email.Template))
				{
					Log.Instance.LogWarning("SendOrderEmailCustomer nodeId: " + emailNodeId + " no valid Tempalte value: " + email.Template + " : no email send");

					return;
				}

				string emailTitle = ReplaceStrings(email.Title, orderInfoXml);
				string emailText = ReplaceStrings(email.Description, orderInfoXml);

				if (parameters == null) parameters = new Dictionary<string, object>();
				parameters.Add("uniqueOrderId", orderInfo.UniqueOrderId.ToString());
				parameters.Add("storeAlias", orderInfo.StoreInfo.Alias);
				parameters.Add("Title", emailTitle);
				parameters.Add("Description", emailText);

				string body;
				if (email.Template.EndsWith(".xslt"))
				{
					body = IO.Container.Resolve<ICMSApplication>().RenderXsltMacro(email.Template, parameters, orderInfoXml);
				}
				else
				{
					if (email.Template.Contains("."))
					{
						body = IO.Container.Resolve<ICMSApplication>().RenderMacro(email.Template, email.Id, parameters);
					}
					else
					{
						Log.Instance.LogWarning("SendOrderEmailStore nodeId: " + emailNodeId + " no valid Tempalte value: " + email.Template + " : no email send");
						return;
					}
				}

				var emailTo = orderInfo.StoreInfo.Store.EmailAddressTo;

				if (!emailTo.Contains(";"))
				{
					Log.Instance.LogDebug("SendOrderEmailStore emailTo: " + emailTo);
					Log.Instance.LogDebug("SendOrderEmailStore EmailFrom: " + orderInfo.StoreInfo.Store.EmailAddressFrom);
					Log.Instance.LogDebug("SendOrderEmailStore EmailFromName: " + orderInfo.StoreInfo.Store.EmailAddressFromName);
					Log.Instance.LogDebug("SendOrderEmailStore emailTitle: " + emailTitle);
					SendMail(orderInfo, emailTo, orderInfo.StoreInfo.Store.EmailAddressFrom, emailTitle, body, orderInfo.StoreInfo.Store.EmailAddressFromName);
				}
				else
				{
					string[] emailToArray = emailTo.Split(';');

					foreach (var emailToArrayValue in emailToArray)
					{
						Log.Instance.LogDebug("SendOrderEmailStore Multiple emailTo: " + emailToArrayValue.Trim());
						Log.Instance.LogDebug("SendOrderEmailStore Multiple EmailFrom: " + orderInfo.StoreInfo.Store.EmailAddressFrom);
						Log.Instance.LogDebug("SendOrderEmailStore Multiple EmailFromName: " + orderInfo.StoreInfo.Store.EmailAddressFromName);
						Log.Instance.LogDebug("SendOrderEmailStore Multiple emailTitle: " + emailTitle);
						SendMail(orderInfo, emailToArrayValue.Trim(), orderInfo.StoreInfo.Store.EmailAddressFrom, emailTitle, body, orderInfo.StoreInfo.Store.EmailAddressFromName);
					}
				}

				Thread.CurrentThread.CurrentCulture = currentCulture;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
			else
			{
				Log.Instance.LogError("SendOrderEmailStore: emailNodeId == 0");
			}
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