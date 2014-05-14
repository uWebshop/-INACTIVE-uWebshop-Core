using System;
using System.Linq;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;

namespace uWebshop.Umbraco
{
	public static class ExtensionMethods
	{
		public static string GetUmbracoPath(this IUwebshopEntity entity)
		{
			var tryCast = entity as uWebshopEntity;
			if (tryCast != null) return tryCast.Path;
			// todo: speed up
			return IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(entity.Id).Path;
		}

		public static umbraco.cms.businesslogic.property.Property GetMultiStoreItem(this Document document, string alias)
		{
			var originalAlias = alias;

			#region frontend

			var orderInfo = OrderHelper.GetOrder();
			if (string.IsNullOrEmpty(library.Request("id")))
			{
				if (orderInfo != null)
					alias = StoreHelper.CreateMultiStorePropertyAlias(alias, orderInfo.StoreInfo.Alias);
				else
				{
					var sAlias = StoreHelper.GetCurrentStore();
					alias = StoreHelper.CreateMultiStorePropertyAlias(alias, sAlias.Alias);
				}
				return document.getProperty(alias);
			}

			#endregion

			#region backend

			//var nodeId = int.Parse(library.Request("id"));
			//var orderNode = new Order(nodeId);

			var typeAlias = document.ContentType.Alias;
			var orderDoc = document;
			if (OrderedProduct.IsAlias(orderDoc.ContentType.Alias) && !OrderedProductVariant.IsAlias(orderDoc.ContentType.Alias))
				orderDoc = new Document(orderDoc.ParentId);

			if (typeAlias == Order.NodeAlias || OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
			{
				var orderInfoDoc = OrderHelper.GetOrderInfo(Guid.Parse(orderDoc.getProperty("orderGuid").Value.ToString()));
				var store = StoreHelper.GetByAlias(orderInfoDoc.StoreInfo.Alias);

				if (store != null) alias = StoreHelper.CreateMultiStorePropertyAlias(alias, store.Alias);
			}

			var property = document.getProperty(alias);

			if (property == null || property.Value == null)
			{
				property = document.getProperty(originalAlias);
			}

			return property;

			#endregion
		}

		public static string GetDocTypeAlias(this IUwebshopEntity entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}
			if (!string.IsNullOrEmpty(entity.TypeAlias))
			{
				return entity.TypeAlias;
			}
			var tryCast = entity as uWebshopEntity;
			if (tryCast != null) return tryCast.NodeTypeAlias;
			return IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(entity.Id).NodeTypeAlias;
		}

		public static string GetProperty(this IProductVariant variant, string propertyAlias)
		{
			var property = new Node(variant.Id).GetMultiStoreItem(propertyAlias);
			if (property == null) return string.Empty;
			return property.Value;
		}

		public static string GetProperty(this MultiStoreUwebshopContent content, string propertyAlias)
		{
			if (propertyAlias != null && content != null)
			{
				var property = new Node(content.Id).GetMultiStoreItem(propertyAlias);

				if (property != null && property.Value != null)
				{
					return property.Value;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Get the value based on the Store Alias
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="storeAlias">The store alias.</param>
		/// <returns></returns>
		public static IProperty GetMultiStoreItem(this INode node, string alias, string storeAlias = null)
		{
			var originalAlias = alias;
			if (node == null) return null;

			#region backend

			// todo: fix backend, add inCMSbackend check?
			int id;
			if (!string.IsNullOrEmpty(library.Request("id")) && int.TryParse(library.Request("id"), out id))
			{
				var typeAlias = node.NodeTypeAlias;

				var orderNode = node;
				if (OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
					orderNode = orderNode.Parent;

				if (typeAlias == Order.NodeAlias || OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
				{
					var orderInfoDoc = OrderHelper.GetOrder(Guid.Parse(orderNode.GetProperty("orderGuid").Value));
					alias = StoreHelper.CreateMultiStorePropertyAlias(alias, orderInfoDoc.StoreInfo.Alias);

					var propertyDoc = node.GetProperty(alias);

					if (propertyDoc == null || string.IsNullOrEmpty(propertyDoc.Value))
					{
						propertyDoc = node.GetProperty(originalAlias);
					}

					return propertyDoc;
				}
			}

			#endregion

			#region frontend

			// dit gebeurt duizenden keren
			if (storeAlias == null)
			{
				storeAlias = StoreHelper.GetCurrentStore().Alias;
			}
			if (storeAlias != null)
				alias = StoreHelper.CreateMultiStorePropertyAlias(alias, storeAlias);

			var property = node.GetProperty(alias);

			if (property == null || string.IsNullOrEmpty(property.Value))
			{
				property = node.GetProperty(originalAlias);
			}

			return property;

			#endregion
		}
	}
}