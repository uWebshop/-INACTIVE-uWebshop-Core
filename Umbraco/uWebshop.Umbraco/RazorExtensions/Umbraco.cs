using System;
using System.Collections.Generic;
using System.Linq;
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

namespace uWebshop.API
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
		public static IOrder GetOrderFromCurrentDocument()
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
		public static IOrder GetOrderByDocumentId(int documentId)
		{
			var orderDoc = ContentService.GetById(documentId);

			if (orderDoc.HasProperty("orderGuid"))
			{
				var orderGuidValue = orderDoc.GetValue("orderGuid");
				if (orderGuidValue != null && !string.IsNullOrEmpty(orderGuidValue.ToString()))
				{
					var orderGuid = orderDoc.GetValue<Guid>("orderGuid");
					
					return Orders.CreateBasketFromOrderInfo(OrderHelper.GetOrder(orderGuid));
				}
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
			OrderStatus status;
			return Enum.TryParse(HttpContext.Current.Request["orderStatus"], out status) ? status : OrderStatus.Confirmed;
		}

		/// <summary>
		/// Creates an orderDocument to view/edit the order based on Current.Request["orderGuid"]
		/// </summary>
		public static void CreateOrderDocument()
		{
			Guid guid;
			if (Guid.TryParse(HttpContext.Current.Request["orderGuid"], out guid))
			{
				CreateOrderDocument(guid);
			}
		}

		/// <summary>
		/// Creates an orderDocument to view/edit the order based on a uniqueOrderId
		/// </summary>
		public static void CreateOrderDocument(Guid uniqueOrderId)
		{
			var order = IO.Container.Resolve<IOrderRepository>().GetOrderInfo(uniqueOrderId);
			if (order == null) throw new Exception("could not find order with uniqueID " + uniqueOrderId);
			IO.Container.Resolve<IUmbracoDocumentTypeInstaller>().CreateOrderDocument(order);
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

			if (!string.IsNullOrEmpty(editNodeId))
			{
				int nodeId;
				int.TryParse(editNodeId, out nodeId);

				if (nodeId != 0)
				{
					var publish = false;

					//var contentService = ApplicationContext.Current.Services.ContentService;

					var doc = IO.Container.Resolve<ICMSChangeContentService>().GetById(nodeId);

					//var doc = new Document(nodeId);//contentService.GetById(nodeId);

					foreach (var key in HttpContext.Current.Request.Form.AllKeys)
					{
						if (key != null && !key.StartsWith("ctl00$") && !key.StartsWith("body_TabView") && !key.StartsWith("__EVENT") && !key.StartsWith("__VIEWSTATE") && !key.StartsWith("__ASYNCPOST") && doc.HasProperty(key))
						{
							var value = HttpContext.Current.Request.Form[key];

							if (!key.StartsWith("stock"))
							{
								if (!string.IsNullOrEmpty(value))
								{
									doc.SetValue(key, value);

									//doc.SetValue(key, value);

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
						//contentService.SaveAndPublish(doc, 0, true);
						doc.SaveAndPublish();
						//doc.Save();
						//doc.Publish(new User(0));
						BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", sender));
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Removes all incomplete orders of last x days: No Undo!
		/// </summary>
		/// <param name="daysAgo"></param>
		public static void RemoveIncompleOrdersBeforeDate(int daysAgo)
		{
			IO.Container.Resolve<IOrderRepository>().RemoveIncompleOrdersBeforeDate(daysAgo);
		}

		/// <summary>
		/// Removes all orders with the store in testmode: No Undo!
		/// </summary>
		public static void RemoveTestOrders()
		{
			IO.Container.Resolve<IOrderRepository>().RemoveTestOrders();
		}

		/// <summary>
		/// Removes orders: No Undo!
		/// </summary>
		/// <param name="orderList">List of orders</param>
		public static void RemoveOrders(IEnumerable<IOrder> orderList)
		{
			IO.Container.Resolve<IOrderRepository>().RemoveOrders(orderList);
		}

		public static void ProductPostHandler()
		{
			EditNodeProperties();
			OpenNode();
		}

		public static string ReplaceValueWithOrderValue(int emailNodeId, string propertyAlias, IOrder order)
		{
			if (order != null)
			{
				var orderInfo = OrderHelper.GetOrder(order.UniqueId);
				var orderInfoXmlstring = DomainHelper.SerializeObjectToXmlString(orderInfo);
				var orderInfoXml = new XmlDocument();
				orderInfoXml.LoadXml(orderInfoXmlstring);

				var email = new Email(emailNodeId);

				var property = email.Node.GetProperty(propertyAlias);
				if (property != null)
				{
					return EmailHelper.ReplaceStrings(property.Value, orderInfoXml);
				}
			}

			return string.Empty;
		}


		/// <summary>
		/// Return latest, non incomplete, order
		/// </summary>
		/// <returns></returns>
		public static IOrder GetLatestOrder()
		{
			if (IO.Container.Resolve<ICMSApplication>().RequestIsInCMSBackend(HttpContext.Current))
			{
				var order =  OrderHelper.GetAllOrders(StoreHelper.CurrentStoreAlias).LastOrDefault(x => x.Status != OrderStatus.Incomplete);

				if (order != null)
				{
					return Orders.CreateBasketFromOrderInfo(order);
				}
			}

			return null;
		}
	}
}