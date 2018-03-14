using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using umbraco;
using umbraco.BasePages;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using Log = uWebshop.Domain.Log;

namespace uWebshop.Umbraco.Services
{
	internal class UmbracoDocumentTypeInstaller : IUmbracoDocumentTypeInstaller
	{
		private readonly IContentService _contentService;
		private readonly IContentTypeService _contentTypeService;

		public UmbracoDocumentTypeInstaller(IContentService contentService, IContentTypeService contentTypeService)
		{
			_contentService = contentService;
			_contentTypeService = contentTypeService;
		}

		public void InstallStore(string storeAliasWithoutSpaces)
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
			var contentTypeList = new List<IContentType>();

			var trueFalseDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("92897bc6-a5f3-4ffe-ae27-f2e7e33dda49"));
			var storeTemplatePickerDataTypeDef = dataTypeService.GetDataTypeDefinitionById(new Guid("a20c7c00-09f1-448d-9656-f5cb012107af")) ?? dataTypeService.GetDataTypeDefinitionById(new Guid("2ad05995-470e-47d9-956d-dd2ec892343d"));

			if (storeTemplatePickerDataTypeDef == null)
				throw new Exception("Could not find storeTemplatePickerDataType");

			// add disable true/false propertytypes to the document types that require them
			foreach (var documentTypeAlias in StoreHelper.StoreDependantDocumentTypeAliasList)
			{
				var contentType = contentTypeService.GetContentType(documentTypeAlias);
				if (contentType == null) continue;
				contentTypeList.Add(contentType);

				var disableAlias = string.Format("disable_{0}", storeAliasWithoutSpaces);
				if (contentType.PropertyTypes.All(p => p.Alias != disableAlias))
				{
					CMSInstaller.GetOrAddPropertyGroup(contentType, storeAliasWithoutSpaces).PropertyTypes.Add(new PropertyType(trueFalseDataTypeDef) {Alias = disableAlias, Name = "#Disable", Description = "#DisableDescription",});
				}
				if (Category.IsAlias(documentTypeAlias) || Product.IsAlias(documentTypeAlias))
				{
					var templateAlias = string.Format("template_{0}", storeAliasWithoutSpaces);
					if (contentType.PropertyTypes.All(p => p.Alias != templateAlias))
					{
						CMSInstaller.GetOrAddPropertyGroup(contentType, storeAliasWithoutSpaces).PropertyTypes.Add(new PropertyType(storeTemplatePickerDataTypeDef) {Alias = templateAlias, Name = "#Template", Description = "#TemplateDescription",});
					}
				}
			}
			contentTypeService.Save(contentTypeList);
		}

		public void UnInstallStore(string storeAliasWithoutSpaces)
		{
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var contentTypeList = new List<IContentType>();

			// add disable true/false propertytypes to the document types that require them
			foreach (var documentTypeAlias in StoreHelper.StoreDependantDocumentTypeAliasList)
			{
				var contentType = contentTypeService.GetContentType(documentTypeAlias);
				if (contentType == null) continue;
				contentTypeList.Add(contentType);

				var disableAlias = string.Format("disable_{0}", storeAliasWithoutSpaces);
				var templateAlias = string.Format("template_{0}", storeAliasWithoutSpaces);

				CMSInstaller.GetOrAddPropertyGroup(contentType, storeAliasWithoutSpaces).PropertyTypes.RemoveAll(p => p.Alias == disableAlias || p.Alias == templateAlias);

				if (!contentType.PropertyGroups[storeAliasWithoutSpaces].PropertyTypes.Any())
				{
					contentType.PropertyGroups.Remove(storeAliasWithoutSpaces);
				}
			}

			contentTypeService.Save(contentTypeList);

			library.RefreshContent();
		}

		public IContent CreateOrderContent(OrderInfo orderInfo)
		{
 
			var contentTypeService = _contentTypeService; //ApplicationContext.Current.Services.ContentTypeService;
			var contentService = _contentService; // ApplicationContext.Current.Services.ContentService;

			if (OpenOrderIfAlreadyExisting(orderInfo)) return null;

			var orderRepositoryType = contentTypeService.GetContentType(Order.OrderRepositoryNodeAlias);
			if (orderRepositoryType == null) return null;
			var orderRepository =
				contentService.GetContentOfContentType(orderRepositoryType.Id).FirstOrDefault(x => !x.Trashed);
			if (orderRepository == null)
				return null;

			var contentToSaveAndPublish = new List<IContent>();

			var storeFolderAliasDic = IO.Container.Resolve<ICMSApplication>().GetDictionaryItem("SharedStoreFoldername");

			if (string.IsNullOrEmpty(storeFolderAliasDic))
			{
				storeFolderAliasDic = "Orders";
			}

			var storeFolderName = UwebshopConfiguration.Current.ShareBasketBetweenStores
				? storeFolderAliasDic
				: orderInfo.StoreInfo.Alias;
			var storeFolder = GetOrCreateChildContentWithName(orderRepository, storeFolderName, contentToSaveAndPublish,
				OrderStoreFolder.NodeAlias);

			var disableDateFolders = UwebshopConfiguration.Current.DisableDateFolders;
			var orderParent = storeFolder;
			IContent yearNode = null;
			IContent monthNode = null;
			IContent dayNode = null;

			if (!disableDateFolders)
			{
				Log.Instance.LogDebug("CreateOrderDocument STEP 7");
				var year = orderInfo.ConfirmDate.GetValueOrDefault().ToString("yyyy");
				var month = orderInfo.ConfirmDate.GetValueOrDefault().ToString("MM");
				var day = orderInfo.ConfirmDate.GetValueOrDefault().ToString("dd");

				Log.Instance.LogDebug("CreateOrderDocument STEP 8");
				yearNode = GetOrCreateChildContentWithName(storeFolder, year, contentToSaveAndPublish,
					DateFolder.NodeAlias);

				Log.Instance.LogDebug("CreateOrderDocument STEP 9");
				monthNode = GetOrCreateChildContentWithName(yearNode, month, contentToSaveAndPublish,
					DateFolder.NodeAlias);

				Log.Instance.LogDebug("CreateOrderDocument STEP 10");
				dayNode = GetOrCreateChildContentWithName(monthNode, day, contentToSaveAndPublish, DateFolder.NodeAlias);
				Log.Instance.LogDebug("CreateOrderDocument STEP 11");
				orderParent = dayNode;
			}

			var orderDoc = GetOrCreateChildContentWithName(orderParent, orderInfo.OrderNumber, contentToSaveAndPublish,
				Order.NodeAlias);

			if (orderDoc.HasProperty("orderGuid"))
				orderDoc.SetValue("orderGuid", orderInfo.UniqueOrderId.ToString());
			if (orderDoc.HasProperty("orderDetails"))
				orderDoc.SetValue("orderDetails", orderInfo.UniqueOrderId.ToString());
			if (orderDoc.HasProperty("orderPaid"))
				orderDoc.SetValue("orderPaid", orderInfo.Paid);

			SetCustomProperties(orderInfo.CustomerInfo.CustomerInformation, orderDoc, "customer");
			SetCustomProperties(orderInfo.CustomerInfo.ShippingInformation, orderDoc, "shipping");
			SetCustomProperties(orderInfo.CustomerInfo.ExtraInformation, orderDoc, "extra");

            foreach (var orderline in orderInfo.OrderLines)
			{
				var productInfo = orderline.ProductInfo;
				var contentTypeAlias =
					contentTypeService.GetAllContentTypes()
						.Select(ct => ct.Alias)
						.FirstOrDefault(
							alias =>
								productInfo.DocTypeAlias != null &&
								alias == productInfo.DocTypeAlias.Replace(Product.NodeAlias, OrderedProduct.NodeAlias)) ??
					OrderedProduct.NodeAlias;
				var orderedProductDoc = contentService.CreateContent(productInfo.Title, orderDoc, contentTypeAlias);
				contentToSaveAndPublish.Add(orderedProductDoc);

				SetProperty(orderedProductDoc, "productId", productInfo.Id);
				SetProperty(orderedProductDoc, "title", productInfo.Title);
				SetProperty(orderedProductDoc, "sku", productInfo.SKU);
				SetProperty(orderedProductDoc, "weight", productInfo.Weight.ToString());
				SetProperty(orderedProductDoc, "length", productInfo.Length.ToString());
				SetProperty(orderedProductDoc, "height", productInfo.Height.ToString());
				SetProperty(orderedProductDoc, "width", productInfo.Width.ToString());
				SetProperty(orderedProductDoc, "orderedProductDiscountPercentage",
					productInfo.DiscountPercentage.ToString());
				SetProperty(orderedProductDoc, "orderedProductDiscountAmount", productInfo.DiscountAmountInCents.ToString());
				SetProperty(orderedProductDoc, "vat", productInfo.Vat.ToString());
				SetProperty(orderedProductDoc, "price", productInfo.OriginalPriceInCents.ToString());
				SetProperty(orderedProductDoc, "ranges", productInfo.RangesString ?? string.Empty);
				SetProperty(orderedProductDoc, "itemCount", productInfo.ItemCount.GetValueOrDefault(1).ToString());

				SetCustomProperties(orderline.CustomData, orderedProductDoc, null);

				foreach (var variant in orderline.ProductInfo.ProductVariants)
				{
					var variantContentTypeAlias =
						contentTypeService.GetAllContentTypes()
							.Select(ct => ct.Alias)
							.FirstOrDefault(
								alias =>
									variant.DocTypeAlias != null &&
									alias == variant.DocTypeAlias.Replace(Product.NodeAlias, OrderedProduct.NodeAlias)) ??
						OrderedProductVariant.NodeAlias;

					var variantDoc = contentService.CreateContent(variant.Title, orderedProductDoc, variantContentTypeAlias);
					contentToSaveAndPublish.Add(variantDoc);

					SetProperty(variantDoc, "variantId", variant.Id.ToString());
					SetProperty(variantDoc, "title", variant.Title);
					SetProperty(variantDoc, "group", variant.Group);
					SetProperty(variantDoc, "sku", variant.SKU);
					SetProperty(variantDoc, "weight", variant.Weight.ToString());
					SetProperty(variantDoc, "price", variant.PriceInCents.ToString());
					SetProperty(variantDoc, "ranges", variant.RangesString ?? string.Empty);
					SetProperty(variantDoc, "discountPercentage", variant.DiscountPercentage.ToString());
					SetProperty(variantDoc, "discountAmount", variant.DiscountAmountInCents.ToString());
				}
			}

			contentService.Save(contentToSaveAndPublish);
			contentService.Publish(storeFolder);

			if (!disableDateFolders)
			{
				if (yearNode != null)
				{
					contentService.PublishWithStatus(yearNode);

					if (monthNode != null)
					{
						contentService.PublishWithStatus(monthNode);

						if (dayNode != null)
						{
							contentService.PublishWithStatus(dayNode);
						}
					}
				}
			}

            orderInfo.OrderNodeId = orderDoc.Id;
			orderInfo.Save();

			return orderDoc;
		}

		public int GetOrCreateOrderContent(OrderInfo orderInfo)
		{
			var contentService = _contentService;

			if (orderInfo != null)
			{
				if (orderInfo.OrderNodeId == 0)
				{
					
					var orderDoc = CreateOrderContent(orderInfo);

					if (orderDoc != null)
					{
						return orderDoc.Id;
					}
				}
				else
				{
					var content = contentService.GetById(orderInfo.OrderNodeId);

					if (content.Trashed || content.Name != orderInfo.OrderNumber)
					{
						var orderDoc = CreateOrderContent(orderInfo);

						if (orderDoc != null)
						{
							return orderDoc.Id;
						}
					}

					return orderInfo.OrderNodeId;
				}
			}

			return 0;
		}

		// untested internally used functionality, keep as small as possible
		/// <summary>
		/// Creates the order document.
		/// </summary>
		/// <param name="orderInfo">The order information.</param>
		public void CreateOrderDocument(OrderInfo orderInfo)
		{
			var orderDoc = CreateOrderContent(orderInfo);

			if (orderDoc != null && !string.IsNullOrEmpty(orderDoc.Path))
			{
				if (BasePage.Current != null && orderInfo.OrderNodeId != 0)
				{
					BasePage.Current.ClientTools.SyncTree(orderDoc.Path, true);
					BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=",
						orderInfo.OrderNodeId));
				}
			}
		}

		private static void SetCustomProperties(XElement customerInformation, IContent orderDoc, string propertyAliasStart)
		{
			if (customerInformation != null)
			{
				foreach (var customerProperty in orderDoc.Properties.Where(x => propertyAliasStart == null || x.Alias.StartsWith(propertyAliasStart)))
				{
					var element = customerInformation.Element(customerProperty.Alias);
					if (element != null)
					{
						customerProperty.Value = element.Value;
					}
				}
			}
		}

		private IContent GetOrCreateChildContentWithName(IContent parentContent, string nodeName, List<IContent> contentToSaveAndPublish, string contentTypeAlias)
		{
			var contentService = _contentService; // ApplicationContext.Current.Services.ContentService;
			var yearNode = parentContent == null || parentContent.Id == 0 ? null : contentService.GetChildren(parentContent.Id).FirstOrDefault(n => n.Name == nodeName);
			if (yearNode == null)
			{
				yearNode = contentService.CreateContent(nodeName, parentContent, contentTypeAlias);
				contentToSaveAndPublish.Add(yearNode);
			}
			return yearNode;
		}

		private static bool OpenOrderIfAlreadyExisting(OrderInfo orderInfo)
		{
			if (orderInfo.OrderNodeId != 0)
			{
				try
				{
					var existingDocument = ApplicationContext.Current.Services.ContentService.GetById(orderInfo.OrderNodeId);
					if (!existingDocument.Trashed)
					{
						var path = existingDocument.Path;
						if (path != null && path.Length > 1 && BasePage.Current != null && orderInfo.OrderNodeId != 0)
						{
							BasePage.Current.ClientTools.SyncTree(path, true);
							BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", orderInfo.OrderNodeId));
							return true;
						}
					}
				}
				catch
				{
				}
			}
			return false;
		}

		private static void SetProperty(IContent orderedProductDoc, string propertyAlias, object value)
		{
			if (orderedProductDoc.HasProperty(propertyAlias))
				orderedProductDoc.SetValue(propertyAlias, value);
		}
	}
}