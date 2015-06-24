using System;
using System.Web;
using System.Xml;
using umbraco.BasePages;
using uWebshop.Common;
using uWebshop.DataAccess;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace uWebshop.RazorExtensions
{
	public static class CMS
	{
		public static IContentService ContentService = ApplicationContext.Current.Services.ContentService;

		public static string GetProperty(this IUwebshopUmbracoEntity content, string propertyAlias)
		{
			return Umbraco.ExtensionMethods.GetProperty(content, propertyAlias);
		}

		/// <summary>
		/// Get the order document based on the Request.QueryString["id"]
		/// Needs property with the alias "orderGuid" filled with the orderGuid of the order
		/// </summary>
		/// <returns></returns>
		public static OrderInfo GetOrderFromCurrentDocument()
		{
			var documentId = int.Parse(HttpContext.Current.Request.QueryString["id"]);
			var orderDoc = ContentService.GetById(documentId);
			return GetOrderByDocumentId(orderDoc.Id);
		}

		/// <summary>
		/// Get the order document based on a given document/node Id
		/// Needs property with the alias "orderGuid" filled with the orderGuid of the order
		/// </summary>
		/// <param name="documentId"></param>
		/// <returns></returns>
		public static OrderInfo GetOrderByDocumentId(int documentId)
		{
			var orderDoc = ContentService.GetById(documentId);
			if (orderDoc.HasProperty("orderGuid"))
			{
				var orderGuid = orderDoc.GetValue<Guid>("orderGuid");
				return OrderHelper.GetOrder(orderGuid);
			}
			return null;
		}

		/// <summary>
		/// Returns the OrderStatus based on the Current.Request["orderStatus"]
		/// Defaults to OrderStatus.Confirmed
		/// </summary>
		/// <returns></returns>
		public static OrderStatus OrderStatusFromRequest()
		{
			var orderStatus = HttpContext.Current.Request["orderStatus"];
			if (!string.IsNullOrEmpty(orderStatus))
			{
				return (OrderStatus) Enum.Parse(typeof (OrderStatus), orderStatus);
			}
			return OrderStatus.Confirmed;
		}

		/// <summary>
		/// Creates an orderDocument to view/edit the order based on Current.Request["orderGuid"]
		/// </summary>
		public static void CreateOrderDocument()
		{
			var orderGuid = HttpContext.Current.Request["orderGuid"];
			if (!string.IsNullOrEmpty(orderGuid))
			{
				var guid = Guid.Parse(orderGuid);

				CreateOrderDocument(guid);
			}
		}

		/// <summary>
		/// Creates an orderDocument to view/edit the order based on a uniqueOrderId
		/// </summary>
		public static void CreateOrderDocument(Guid uniqueOrderId)
		{
			API.CMS.CreateOrderDocument(uniqueOrderId);
		}

		/// <summary>
		/// Creates an orderDocument to view/edit the order based on Current.Request["orderGuid"]
		/// </summary>
		public static void OpenNode()
		{
			var nodeIdString = HttpContext.Current.Request["openNodeId"];
			if (!string.IsNullOrEmpty(nodeIdString))
			{
				int nodeId;
				int.TryParse(nodeIdString, out nodeId);

				if (nodeId != 0)
				{
					OpenNode(nodeId);
				}
			}
		}

		public static void OpenNode(int nodeId)
		{
			var doc = ContentService.GetById(nodeId);
			if (!string.IsNullOrEmpty(doc.Path) && BasePage.Current != null)
			{
				BasePage.Current.ClientTools.SyncTree(doc.Path, true);
				BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", nodeId));
			}
		}

		public static bool EditNodeProperties()
		{
			var editNodeId = HttpContext.Current.Request["editNodeId"];
			var storeAlias = HttpContext.Current.Request["storeAlias"];
			var sender = HttpContext.Current.Request["Id"];

			if (string.IsNullOrEmpty(editNodeId))
			{
				return false;
			}

			var nodeId = Helpers.ParseInt(editNodeId);
			if (nodeId == 0)
			{
				return false;
			}

			var publish = false;

			var doc = IO.Container.Resolve<ICMSChangeContentService>().GetById(nodeId);

			foreach (var key in HttpContext.Current.Request.Form.AllKeys)
			{
				if (key != null && !key.StartsWith("ctl00$") && !key.StartsWith("body_TabView") && !key.StartsWith("__EVENT") && !key.StartsWith("__VIEWSTATE") &&
				    !key.StartsWith("__ASYNCPOST") && doc.HasProperty(key))
				{
					var value = HttpContext.Current.Request.Form[key];
					if (!key.StartsWith("stock"))
					{
						if (!string.IsNullOrEmpty(value))
						{
							doc.SetValue(key, value);
							publish = true;
						}
					}
					else
					{
						if (Product.IsAlias(doc.ContentTypeAlias) || ProductVariant.IsAlias(doc.ContentTypeAlias))
						{
							int newStockInt;
							int.TryParse(value, out newStockInt);
							UWebshopStock.UpdateStock(nodeId, newStockInt, false, storeAlias);
						}
					}
				}
			}

			if (publish)
			{
				doc.SaveAndPublish();
				BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", sender));
				return true;
			}

			return false;
		}

		public static void RemoveIncompleOrdersBeforeDate(int daysAgo)
		{
			API.CMS.RemoveIncompleOrdersBeforeDate(daysAgo);
		}

		public static void ProductPostHandler()
		{
			EditNodeProperties();
			OpenNode();
		}

		public static string ReplaceValueWithOrderValue(int emailNodeId, string propertyAlias, OrderInfo order)
		{
			var orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(order);
			var orderInfoXml = new XmlDocument();
			orderInfoXml.LoadXml(orderInfoXmlstring);

			var email = new Email(emailNodeId);

			var propertyValue = email.GetProperty(propertyAlias);
			if (!string.IsNullOrEmpty(propertyValue))
			{
				return EmailHelper.ReplaceStrings(propertyValue, orderInfoXml);
			}

			return string.Empty;
		}
	}
}