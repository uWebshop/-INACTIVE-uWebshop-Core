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
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

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

		public static global::Umbraco.Core.Models.Property GetMultiStoreItem(this IContent content, string alias)
		{
			var contentService = ApplicationContext.Current.Services.ContentService;
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
				return content.Properties.FirstOrDefault(x => x.Alias == alias);
			}

			#endregion

			#region backend

			//var nodeId = int.Parse(library.Request("id"));
			//var orderNode = new Order(nodeId);

			var typeAlias = content.ContentType.Alias;
			var orderDoc = content;
			if (OrderedProduct.IsAlias(orderDoc.ContentType.Alias) && !OrderedProductVariant.IsAlias(orderDoc.ContentType.Alias))
				orderDoc = contentService.GetById(orderDoc.ParentId);

			if (typeAlias == Order.NodeAlias || OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
			{
				var orderInfoDoc = OrderHelper.GetOrder(orderDoc.GetValue<Guid>("orderGuid"));
				var store = StoreHelper.GetByAlias(orderInfoDoc.StoreInfo.Alias);

				if (store != null) alias = StoreHelper.CreateMultiStorePropertyAlias(alias, store.Alias);
			}

			var property = content.Properties.FirstOrDefault(x => x.Alias == alias);

			if (property == null || property.Value == null)
			{
				property = content.Properties.FirstOrDefault(x => x.Alias == originalAlias);
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

		public static string GetProperty(this IUwebshopUmbracoEntity variant, string propertyAlias)
		{
			var umbHelper = new UmbracoHelper(UmbracoContext.Current);
			var property = umbHelper.TypedContent(variant.Id).GetMultiStoreItem(propertyAlias);
			return property == null ? string.Empty : property.Value.ToString();
		}

		public static string GetProperty(this MultiStoreUwebshopContent content, string propertyAlias)
		{
			var umbHelper = new UmbracoHelper(UmbracoContext.Current);
			if (propertyAlias != null && content != null)
			{
				var property = umbHelper.TypedContent(content.Id).GetMultiStoreItem(propertyAlias);
				if (property != null && property.Value != null)
				{
					return property.Value.ToString();
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
		public static IPublishedContentProperty GetMultiStoreItem(this IPublishedContent node, string alias, string storeAlias = null)
		{
			var originalAlias = alias;
			if (node == null) return null;

			#region backend

			// todo: fix backend, add inCMSbackend check?
			int id;
			if (!string.IsNullOrEmpty(library.Request("id")) && int.TryParse(library.Request("id"), out id))
			{
				var typeAlias = node.DocumentTypeAlias;

				var orderNode = node;
				if (OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
					orderNode = orderNode.Parent;

				if (typeAlias == Order.NodeAlias || OrderedProduct.IsAlias(typeAlias) && !OrderedProductVariant.IsAlias(typeAlias))
				{
					var orderInfoDoc = OrderHelper.GetOrder(orderNode.GetPropertyValue<Guid>("orderGuid"));
					alias = StoreHelper.CreateMultiStorePropertyAlias(alias, orderInfoDoc.StoreInfo.Alias);

					if (node.HasProperty(alias))
					{
						if (node.HasValue(alias))
						{
							return node.Properties.FirstOrDefault(x => x.Alias == originalAlias);
						}
					}

				}
			}

			#endregion

			#region frontend

			// todo: dit gebeurt duizenden keren!
			if (storeAlias == null)
			{
				storeAlias = StoreHelper.GetCurrentStore().Alias;
			}
			if (storeAlias != null)
				alias = StoreHelper.CreateMultiStorePropertyAlias(alias, storeAlias);

			if (node.HasProperty(alias) && node.HasValue(alias))
			{

				return node.Properties.FirstOrDefault(x => x.Alias == originalAlias);
			}

			return null;

			#endregion
		}
	}
}