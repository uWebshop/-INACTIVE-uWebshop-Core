using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Linq;
using Examine;
using umbraco;
using umbraco.BasePages;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess.Pocos;
using uWebshop.Domain;
using uWebshop.Domain.Core;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Umbraco.Businesslogic;
using uWebshop.Umbraco.Services;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;
using Catalog = uWebshop.Domain.Catalog;
using Constants = uWebshop.Common.Constants;
using Log = uWebshop.Domain.Log;
using Store = uWebshop.Domain.Store;
using Newtonsoft.Json;
using Umbraco.Web.Routing;

namespace uWebshop.Umbraco
{
	public class ApplicationEventHandler : IApplicationEventHandler
	{
		public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{

		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			UrlProviderResolver.Current.InsertTypeBefore<DefaultUrlProvider, UrlProvider>();
		}

        private static void DoLogged(Action action, string message)
        {
            LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, message + " start: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
            action();
            LogHelper.Debug(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, message + " end: " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"));
        }

		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
            var umbracoonfigurationStatus = ConfigurationManager.AppSettings["umbracoConfigurationStatus"];
		    if (umbracoonfigurationStatus == null)
		    {
                // umbraco is not installed > don't do anything uWebshop related
		        return;
		    }
            var uWebshopDomainAssembly = Assembly.GetExecutingAssembly();
            var uWebshopVersionFromDomain = uWebshopDomainAssembly.GetName().Version;

            var uWebshopConfigurationStatus = ConfigurationManager.AppSettings["uWebshopConfigurationStatus"];

		    if (uWebshopConfigurationStatus == null || uWebshopConfigurationStatus != uWebshopVersionFromDomain.ToString())
		    {
				// if there is no uWebshopConfigurationStatus found, create properties || OR if uwbsCreateMissingProperties = true
				var createMissingProperties = uWebshopConfigurationStatus == null || ConfigurationManager.AppSettings["uwbsCreateMissingProperties"] == "true";

				// if uwbsCreateMissingProperties is set to false, NEVER create properties
				if (ConfigurationManager.AppSettings["uwbsCreateMissingProperties"] == "false")
			    {
				    createMissingProperties = false;
			    }
				
				var config = WebConfigurationManager.OpenWebConfiguration("~");

		        // umbracoConfigurationStatus not yet added to web config or not equal to current version -> Install uWebhsop
		        DoLogged(Initialize.Reboot, "uWebshop Installer initialize uWebshop internal state");
		        DoLogged(() => IO.Container.Resolve<IInstaller>().Install(createMissingProperties), "uWebshop Installer Umbraco installer.Install()");
		        // update web.config
                //config.AppSettings.Settings.Remove("uWebshopConfigurationStatus");
		        //config.AppSettings.Settings.Add("uWebshopConfigurationStatus", uWebshopVersionFromDomain.ToString());
                //config.Save();
		    }

		    try
			{
				Initialize.ContinueInitialization();
				Log.Instance.LogDebug("uWebshop initialized");
			}
			catch (Exception ex)
			{
				LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Error while initializing uWebshop, most likely due to wrong umbraco.config, please republish the site " + ex.Message, ex);
			}
			ContentService.Created += ContentService_Created;
			//ContentService.Saved += ContentService_Saved;
			ContentService.Published += ContentService_Published;
			ContentService.Publishing += ContentService_Publishing;
			ContentService.Trashed += ContentService_Trashed;
			ContentService.UnPublished += ContentService_UnPublished;
			ContentService.Deleted += ContentService_Deleted;
			ContentService.Copied += ContentService_Copied;
                    
			OrderInfo.BeforeStatusChanged += OrderEvents.UpdateOrderNumberIfChangingFromIncompleteToScheduled;
			OrderInfo.AfterStatusChanged += OrderEvents.OrderStatusChanged;

            UmbracoDefault.BeforeRequestInit += UmbracoDefaultBeforeRequestInit;
			UmbracoDefault.AfterRequestInit += UmbracoDefaultAfterRequestInit;

			var indexer = ExamineManager.Instance.IndexProviderCollection[UwebshopConfiguration.Current.ExamineIndexer];
			indexer.GatheringNodeData += GatheringNodeDataHandler;

            // Create SaleManager Section
            var sectionService = applicationContext.Services.SectionService;

            var section = sectionService.GetSections().SingleOrDefault(x => x.Alias == "saleManager");

            if (section == null)
            {
                sectionService.MakeNew("SaleManager", "saleManager", "icon-uwebshop-statistics");
            }

            uWebshop.Domain.StoreCache.StoreDomainCache.FillCache();
        }

		void ContentService_Saved(IContentService sender, SaveEventArgs<IContent> e)
		{
			//try
			//{

				//foreach (var item in e.SavedEntities)
				//{

				//if (item.Level <= 2)
				//{
				//	continue;
				//}

				//if (item.ContentType.Alias != Order.NodeAlias &&
				//    (item.Parent() == null || (!OrderedProduct.IsAlias(item.ContentType.Alias) &&
				//                               (item.Parent().Parent() == null || !OrderedProductVariant.IsAlias(item.ContentType.Alias))))) // todo simplify expression
				//{
				//	continue;
				//}

				//var orderDoc = item.ContentType.Alias == Order.NodeAlias
				//	               ? item
				//	               : (OrderedProduct.IsAlias(item.ContentType.Alias) && !OrderedProductVariant.IsAlias(item.ContentType.Alias)
				//		                  ? contentService.GetById(item.Parent().Id)
				//		                  : contentService.GetById(item.Parent().Parent().Id));

				//if (orderDoc.ContentType.Alias != Order.NodeAlias)
				//	throw new Exception("There was an error in the structure of the order documents");

				//if (!orderDoc.HasProperty("orderGuid"))
				//{
				//	continue;
				//}
				//var orderGuidString = orderDoc.GetValue<string>("orderGuid");

				//if (orderDoc.HasProperty("orderGuid") && string.IsNullOrEmpty(orderGuidString))
				//{
				//	Store store = null;
				//	var storeDoc = item.Ancestors().FirstOrDefault(x => x.ContentType.Alias == OrderStoreFolder.NodeAlias);
				//	if (storeDoc != null)
				//	{
				//		store = StoreHelper.GetAllStores().FirstOrDefault(x => x.Name == storeDoc.Name);
				//	}
				//	if (store == null)
				//	{
				//		store = StoreHelper.GetAllStores().FirstOrDefault();
				//	}
				//	var orderInfo = OrderHelper.CreateOrder(store);
				//	IO.Container.Resolve<IOrderNumberService>().GenerateAndPersistOrderNumber(orderInfo);
				//	orderInfo.Status = OrderStatus.Confirmed;
				//	orderInfo.Save();

				//	item.SetValue("orderGuid", orderInfo.UniqueOrderId.ToString());
				//	contentService.Save(item);
				//}
				//else
				//{
				//	var orderGuid = orderDoc.GetValue<Guid>("orderGuid");
				//	var orderInfo = OrderHelper.GetOrder(orderGuid);

				//	var order = new Order(orderDoc.Id);
				//	orderInfo.CustomerEmail = order.CustomerEmail;
				//	orderInfo.CustomerFirstName = order.CustomerFirstName;
				//	orderInfo.CustomerLastName = order.CustomerLastName;

				//	var dictionaryCustomer = orderDoc.Properties.Where(x => x.Alias.StartsWith("customer"))
				//	                                 .ToDictionary(customerProperty => customerProperty.Alias,
				//	                                               customerProperty => customerProperty.Value != null ? customerProperty.Value.ToString() : null);
				//	orderInfo.AddCustomerFields(dictionaryCustomer, CustomerDatatypes.Customer);

				//	var dictionaryShipping = orderDoc.Properties.Where(x => x.Alias.StartsWith("shipping"))
				//	                                 .ToDictionary(property => property.Alias, property => property.Value != null ? property.Value.ToString() : null);
				//	orderInfo.AddCustomerFields(dictionaryShipping, CustomerDatatypes.Shipping);

				//	var dictionarExtra = orderDoc.Properties.Where(x => x.Alias.StartsWith("extra"))
				//	                             .ToDictionary(property => property.Alias, property => property.Value != null ? property.Value.ToString() : null);
				//	orderInfo.AddCustomerFields(dictionarExtra, CustomerDatatypes.Extra);

				//	var orderPaidProperty = order.Document.getProperty("orderPaid");
				//	if (orderPaidProperty != null && orderPaidProperty.Value != null)
				//	{
				//		orderInfo.Paid = orderPaidProperty.Value == "1";
				//	}
				//	// load data recursively from umbraco documents into order tree
				//	orderInfo.OrderLines = orderDoc.Children().Select(d =>
				//		{
				//			var fields =
				//				d.Properties.Where(x => !OrderedProduct.DefaultProperties.Contains(x.Alias))
				//				 .ToDictionary(s => s.Alias, s => d.GetValue<string>(s.Alias));

				//			var xDoc = new XDocument(new XElement("Fields"));

				//			OrderUpdatingService.AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, d.ContentType.Alias);

				//			var orderedProduct = new OrderedProduct(d.Id);

				//			var productInfo = new ProductInfo(orderedProduct, orderInfo);
				//			productInfo.ProductVariants =
				//				d.Children()
				//				 .Select(cd => new ProductVariantInfo(new OrderedProductVariant(cd.Id), productInfo, productInfo.Vat))
				//				 .ToList();
				//			return new OrderLine(productInfo, orderInfo) {_customData = xDoc};
				//		}).ToList();

				//	IO.Container.Resolve<IOrderRepository>().SaveOrderInfo(orderInfo);
				//}

				//BasePage.Current.ClientTools.SyncTree(item.Parent().Path, false);
				//BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", item.Id));

				//BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Order Updated!", "This order has been updated!");

				//Log.Instance.LogDebug("Saved event fired finshed");
				//}
				//}
				//catch (Exception exception)
				//{
				//	LogHelper.Error<ApplicationEventHandler>("ContentService_Saved", exception);
				//}
		}
	
		private void ContentService_Trashed(IContentService sender, MoveEventArgs<IContent> e)
		{
			if (e.Entity.ContentType.Alias.StartsWith(Store.NodeAlias))
			{
				var reg = new Regex(@"\s*");
				var storeAlias = reg.Replace(e.Entity.Name, "");
				StoreHelper.UnInstallStore(storeAlias);
			}
			ClearCaches(e.Entity);
		}
		
		protected void GatheringNodeDataHandler(object sender, IndexingNodeDataEventArgs e)
		{
			try
			{
				foreach (var xElement in e.Node.Elements().Where(element => element.Name.LocalName.StartsWith("description")))
				{
					if (xElement != null) e.Fields.Add("RTEItem" + xElement.Name.LocalName, xElement.Value);
				}
			}
			catch
			{
			}

			foreach (var field in defaultPriceValues())
			{
				try
				{
					//grab the current data from the Fields collection
					string value;
					if (e.Fields == null || !e.Fields.TryGetValue(field, out value))
						continue;
					var currencyFieldValue = e.Fields[field];

					var currencyValueAsInt = int.Parse(currencyFieldValue);

					//prefix with leading zero's
					currencyFieldValue = currencyValueAsInt.ToString("D8");

					//now put it back into the Fields so we can pretend nothing happened! ;)
					e.Fields[field] = currencyFieldValue;
				}
				catch (Exception ex)
				{
					Log.Instance.LogError("GatheringNodeDataHandler defaultPriceValues Examine: " + ex);
				}
			}

			foreach (var field in DefaultCsvValues())
			{
				try
				{
					string value;
					if (e.Fields == null || !e.Fields.TryGetValue(field, out value))
						continue;
					var csvFieldValue = e.Fields[field];
					csvFieldValue = csvFieldValue.Replace(",", " ");
					e.Fields[field] = csvFieldValue;
				}
				catch (Exception ex)
				{
					Log.Instance.LogError("GatheringNodeDataHandler DefaultCsvValues Examine: " + ex);
				}
			}
		}

		public List<string> defaultPriceValues()
		{
			return new List<string> { "price" };
		}

		public List<string> DefaultCsvValues()
		{
			return new List<string> {"categories", "images", "files"};
		}

		private void ContentService_Copied(IContentService sender, CopyEventArgs<IContent> e)
		{
			if (e.Original.Level > 2 && e.Original.ContentType.Alias == Order.NodeAlias)
			{
				var orderGuid = e.Original.GetValue<Guid>("orderGuid");

				var order = OrderHelper.GetOrder(orderGuid);

				var newOrder = OrderHelper.CreateNewOrderFromExisting(order);

				IO.Container.Resolve<IOrderNumberService>().GenerateAndPersistOrderNumber(order);

				order.OrderNodeId = e.Copy.Id;
				order.Save();

				e.Copy.Name = order.OrderNumber;
				e.Copy.SetValue("orderGuid", newOrder.UniqueOrderId.ToString());
				e.Copy.SetValue("orderStatusPicker", newOrder.Status.ToString());

				sender.Save(e.Copy);

				BasePage.Current.ClientTools.SyncTree(e.Copy.Parent().Path, false);
				BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", e.Copy.Id));
			}
		}

		public static void ResetAll(int id, string nodeTypeAlias)
		{
			IO.Container.Resolve<IApplicationCacheManagingService>().ReloadEntityWithGlobalId(id, nodeTypeAlias);
		}
		
		private void ContentService_UnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
		{
			foreach (var item in e.PublishedEntities)
			{
				ClearCaches(item);
			}
		}

		private void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
		{
			foreach (var item in e.DeletedEntities)
			{
				ClearCaches(item);

                if (item.ContentType.Alias == "uwbsDiscountOrder")
                {
                    RemoveCoupons(item.Id);
                }
			}
		}

		public static void ClearCaches(IContent sender)
		{
			//todo: work with aliasses from config
			if (sender.ContentType.Alias != Order.NodeAlias)
			{
				IO.Container.Resolve<IApplicationCacheManagingService>().UnloadEntityWithGlobalId(sender.Id, sender.ContentType.Alias);
			}
		}

		private static bool GetMultiStorePropertyName(IContentBase sender, IStore store, out string propertyName)
		{
			propertyName = "url_" + store.Alias.ToLower();

			if (sender.HasProperty(propertyName)) return false;
			propertyName = "url_" + store.Alias.ToUpper();

			if (sender.HasProperty(propertyName)) return false;
			propertyName = "url_" + store.Alias;

			return !sender.HasProperty(propertyName);
		}

		private static void FixRootCategoryUrlName(IContentBase sender, IEnumerable<IContent> content, string propertyName)
		{
			var urlName = sender.GetValue<string>(propertyName);
			if (content.Any(x => urlName == x.Name))
			{
				int count = 1;
				var existingRenames = content.Where(x => x.Name.StartsWith(urlName)).Select(x => x.Name).Select(x => Regex.Match(x, urlName + @" [(](\d)[)]").Groups[1].Value).Select(x =>
					{
						int i;
						int.TryParse(x, out i);
						return i;
					});
				if (existingRenames.Any())
				{
					count = existingRenames.Max() + 1;
				}
				sender.SetValue(propertyName, urlName + " (" + count + ")");
			}
		}

		protected void UmbracoDefaultBeforeRequestInit(object sender, RequestInitEventArgs e)
		{
			try
			{
				var request = UmbracoContext.Current.PublishedContentRequest;
				var currentNode = request.PublishedContent;

				if (ProductVariant.IsAlias(currentNode.DocumentTypeAlias))
				{
					var product = Domain.Helpers.DomainHelper.GetProductById(currentNode.Parent.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Product.IsAlias(currentNode.DocumentTypeAlias))
				{
					var product = Domain.Helpers.DomainHelper.GetProductById(currentNode.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Category.IsAlias(currentNode.DocumentTypeAlias))
				{
					var category = Domain.Helpers.DomainHelper.GetCategoryById(currentNode.Id);
					if (category != null) HttpContext.Current.Response.RedirectPermanent( /* todo nicer */RazorExtensions.ExtensionMethods.NiceUrl(category), true);
				}
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception)
			{
				// intentionally left empty, because Umbraco will serve a 404
			}
		}

		private static void UmbracoDefaultAfterRequestInit(object sender, RequestInitEventArgs e)
		{
			var fileService = ApplicationContext.Current.Services.FileService;

			var currentMember = UwebshopRequest.Current.User;

			var currentorder = UwebshopRequest.Current.OrderInfo ?? OrderHelper.GetOrder();

			var orderRepository = IO.Container.Resolve<IOrderRepository>();
			if (currentorder != null && (currentMember != null && currentorder.CustomerInfo.LoginName != currentMember.UserName))
			{
				orderRepository.SetCustomer(currentorder.UniqueOrderId, currentMember.UserName);
				currentorder.CustomerInfo.LoginName = currentMember.UserName;

				if (currentMember.ProviderUserKey != null)
				{
					orderRepository.SetCustomerId(currentorder.UniqueOrderId, (int)currentMember.ProviderUserKey);
					currentorder.CustomerInfo.CustomerId = (int)currentMember.ProviderUserKey;
				}

				currentorder.ResetDiscounts();
				currentorder.Save();
			}

			if (currentorder != null && currentMember == null && !string.IsNullOrEmpty(currentorder.CustomerInfo.LoginName))
			{
				orderRepository.SetCustomer(currentorder.UniqueOrderId, string.Empty);
				currentorder.CustomerInfo.LoginName = string.Empty;

				orderRepository.SetCustomerId(currentorder.UniqueOrderId, 0);
				currentorder.CustomerInfo.CustomerId = 0;

				currentorder.ResetDiscounts();
				currentorder.Save();
			}

			var httpContext = HttpContext.Current;
			var cookie = httpContext.Request.Cookies["StoreInfo"];

			if (cookie != null)
			{
				if (currentMember != null && !string.IsNullOrEmpty(cookie["Wishlist"]))
				{
					var wishlistId = cookie["Wishlist"];

					Guid wishGuid;
					Guid.TryParse(wishlistId, out wishGuid);

					if (wishGuid != default(Guid) || wishGuid != Guid.Empty)
					{
						var wishlist = OrderHelper.GetOrder(wishGuid);

						wishlist.CustomerInfo.LoginName = currentMember.UserName;

						var userKey = 0;
						if (currentMember.ProviderUserKey != null)
						{
							int.TryParse(currentMember.ProviderUserKey.ToString(), out userKey);
							if (userKey != 0)
							{
								wishlist.CustomerInfo.CustomerId = userKey;
							}
						}
						const string wishlistName = "Wishlist";

						var wishlistCount = Customers.GetWishlists(currentMember.UserName).Count() + 1;
						wishlist.Name = string.Format("{0}{1}", wishlistName, wishlistCount);
						wishlist.Save();

						cookie.Values.Remove("Wishlist");
					}
				}
			}

			var paymentProvider = UwebshopRequest.Current.PaymentProvider;
			if (paymentProvider != null)
			{
				new PaymentRequestHandler().HandleuWebshopPaymentRequest(paymentProvider);

				Log.Instance.LogDebug("UmbracoDefaultAfterRequestInit paymentProvider: " + paymentProvider.Name);

				var paymentProviderTemplate = paymentProvider.Node.template;

				((UmbracoDefault)sender).MasterPageFile = template.GetMasterPageName(paymentProviderTemplate);

				return;
			}

			// todo: rebuild to UwebshopRequest.Current.Category
			var currentCategoryId = httpContext.Request["resolvedCategoryId"];
			var currentProductId = httpContext.Request["resolvedProductId"];

			if (!string.IsNullOrEmpty(currentCategoryId))
			{
				int categoryId;
				if (!int.TryParse(currentCategoryId, out categoryId))
					return;
				var categoryFromUrl = Domain.Helpers.DomainHelper.GetCategoryById(categoryId);
				if (categoryFromUrl == null) return;

				if (categoryFromUrl.Disabled)
				{
                    var result = new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    HttpContext.Current.Response.StatusCode = result.StatusCode;
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
					return;
				}

				if (Access.HasAccess(categoryFromUrl.Id, categoryFromUrl.GetUmbracoPath(), UwebshopRequest.Current.User))
				{
					if (categoryFromUrl.Template != 0)
					{
						((UmbracoDefault)sender).MasterPageFile = template.GetMasterPageName(categoryFromUrl.Template);
					}

					var altTemplate = httpContext.Request["altTemplate"];
					if (!string.IsNullOrEmpty(altTemplate))
					{
						var altTemplateFile = fileService.GetTemplate(altTemplate);
						if (altTemplateFile != null)
						{
							((UmbracoDefault)sender).MasterPageFile = template.GetMasterPageName(altTemplateFile.Id);
						}
					}
				}
				else
				{
					if (httpContext.User.Identity.IsAuthenticated)
					{
						httpContext.Response.Redirect(library.NiceUrl(Access.GetErrorPage(categoryFromUrl.GetUmbracoPath())), true);
					}
					httpContext.Response.Redirect(library.NiceUrl(Access.GetLoginPage(categoryFromUrl.GetUmbracoPath())), true);
				}
			}
			else if (!string.IsNullOrEmpty(currentProductId))
			{
				int productId;
				if (!int.TryParse(currentProductId, out productId))
					return;
				var productFromUrl = Domain.Helpers.DomainHelper.GetProductById(productId);
				if (productFromUrl == null) return;

				if (Access.HasAccess(productFromUrl.Id, productFromUrl.Path(), UwebshopRequest.Current.User))
				{
					if (productFromUrl.Template != 0)
					{
						((UmbracoDefault)sender).MasterPageFile = template.GetMasterPageName(productFromUrl.Template);
					}

					var altTemplate = httpContext.Request["altTemplate"];
					if (!string.IsNullOrEmpty(altTemplate))
					{
						var altTemplateId = fileService.GetTemplate(altTemplate);

						if (altTemplateId != null)
						{
							((UmbracoDefault)sender).MasterPageFile = template.GetMasterPageName(altTemplateId.Id);
						}
					}
				}
				else
				{
					if (httpContext.User.Identity.IsAuthenticated)
					{
						httpContext.Response.Redirect(library.NiceUrl(Access.GetErrorPage(productFromUrl.Path())), true);
					}
					httpContext.Response.Redirect(library.NiceUrl(Access.GetLoginPage(productFromUrl.Path())), true);
				}
			}
		}
        
		public static SortedDictionary<string, string> GetCountryList()
		{
			//create a new Generic list to hold the country names returned
			var cultureList = new SortedDictionary<string, string>();

			//create an array of CultureInfo to hold all the cultures found, these include the users local cluture, and all the
			//cultures installed with the .Net Framework
			var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

			//loop through all the cultures found
			foreach (var culture in cultures)
			{
				try
				{
					//pass the current culture's Locale ID (http://msdn.microsoft.com/en-us/library/0h88fahh.aspx)
					//to the RegionInfo contructor to gain access to the information for that culture
					var region = new RegionInfo(culture.LCID);

					//make sure out generic list doesnt already
					//contain this country
					if (!cultureList.ContainsKey(region.EnglishName))
						//not there so add the EnglishName (http://msdn.microsoft.com/en-us/library/system.globalization.regioninfo.englishname.aspx)
						//value to our generic list
						cultureList.Add(region.EnglishName, region.TwoLetterISORegionName);
				}
				catch
				{
				}
			}
			return cultureList;
		}

		private void ContentService_Publishing(IPublishingStrategy sender, PublishEventArgs<IContent> e)
		{

			foreach (var content in e.PublishedEntities)
			{
				if (Category.IsAlias(content.ContentType.Alias))
				{
					SetAliasedPropertiesIfEnabled(content, "categoryUrl");
				}
				if (Product.IsAlias(content.ContentType.Alias))
				{
					SetAliasedPropertiesIfEnabled(content, "productUrl");
				}
				if (content.ContentType.Alias == Order.NodeAlias)
				{
					// order => todo: delete node, update SQL
				}

				if (Category.IsAlias(content.ContentType.Alias) || Product.IsAlias(content.ContentType.Alias))
				{
					// Need to get this into function

					try
					{
						var stores = StoreHelper.GetAllStores();
						var siblings = content.Parent().Children().Where(x => x.Published && x.Id != content.Id && !x.Trashed);

						foreach (var store in stores)
						{

							var slug = NodeHelper.GetStoreProperty(content, "url", store.Alias);

							var title = NodeHelper.GetStoreProperty(content, "title", store.Alias);

							if (string.IsNullOrEmpty(slug) && !string.IsNullOrEmpty(title))
							{
								slug = title.ToUrlSegment().ToLowerInvariant().Trim();

								if (content.HasProperty("url_" + store.Alias))
								{
									content.SetValue("url_" + store.Alias, slug);
								}
								else
								{
									content.SetValue("url", slug);
								}
							}
							
							// Update Slug if Slug Exist on same Level and is Published
							if (!string.IsNullOrEmpty(slug) && siblings != null && siblings.Any(x => NodeHelper.GetStoreProperty(x, "url", store.Alias) == slug.ToLowerInvariant()))
							{
								// Random not a nice solution
								Random rnd = new Random();

								slug = slug + "-" + rnd.Next(1, 150);

								if (content.HasProperty("url_" + store.Alias))
								{
									content.SetValue("url_" + store.Alias, slug);
								}
								else
								{
									content.SetValue("url", slug);
								}

								Log.Instance.LogWarning("Duplicate slug found for product : " + content.Id + " store: " + store.Alias);

								e.Messages.Add(new EventMessage("Duplicate Slug Found.", "Sorry but this slug is already in use, we updated it for you. Store: " + store.Alias, EventMessageType.Warning));
							}
						}

						Log.Instance.LogDebug("Publishing event finished. Category: " + content.Id);
					} catch(Exception ex)
					{
						Log.Instance.LogError(ex, "Failed to save Slug and title.");
					}

				}

			}
		}

		private static void SetAliasedPropertiesIfEnabled(IContentBase sender, string propertyName)
		{
			var property = sender.Properties.FirstOrDefault(x => x.Alias == propertyName);
			if (property == null) return;
			property.Value = property.Value.ToString().ToUrlSegment();

			foreach (var shopAlias in StoreHelper.GetAllStores())
			{
				// todo: check this
				var aliasedEnabled = sender.Properties.FirstOrDefault(x => x.Alias == "enable_" + shopAlias.Alias.ToUpper());
				if (aliasedEnabled == null || aliasedEnabled.Value.ToString() != "1") continue;

				var aliasedproperty = sender.Properties.FirstOrDefault(x => x.Alias == propertyName + "_" + shopAlias.Alias.ToUpper());
				// test == test --> overerf van global
				// test == "" --> overef van global
				if (aliasedproperty != null && aliasedproperty.Value == property.Value || aliasedproperty != null && string.IsNullOrEmpty(aliasedproperty.Value.ToString()))
				{
					aliasedproperty.Value = property.Value.ToString().ToUrlSegment();
				}
				// test == bla --> niets doen
				else if (!(aliasedproperty != null && !string.IsNullOrEmpty(aliasedproperty.Value.ToString())))
				{
					aliasedproperty.Value = aliasedproperty.Value.ToString().ToUrlSegment();
				}
			}
		}

		private static void ContentService_Created(IContentService sender, NewEventArgs<IContent> e)
		{
            try
			{

				if (sender == null)
				{
					return;
				}

                if (e.Entity.Id != 0)
				{
					if (e.Entity.ContentType.Alias.StartsWith(Store.NodeAlias))
					{
						var reg = new Regex(@"\s*");
						var storeAlias = reg.Replace(e.Entity.Name, "");

						Helpers.InstallStore(storeAlias, sender.GetById(e.Entity.Id));

						e.Entity.Name = storeAlias;
						sender.Save(e.Entity);
					}
				}

                IContent parent = null;

                if (e.Entity.Id != 0 && e.Entity != null)
                {
                    parent = sender.GetParent(e.Entity.Id);
                }

				if (parent == null || parent.Id < 0)
				{
					return;
				}

                var parentDoc = parent;

                if (parentDoc.ContentType != null && (Category.IsAlias(e.Entity.ContentType.Alias) && parentDoc.ContentType.Alias == Catalog.CategoryRepositoryNodeAlias))
				{
					var docs = GlobalSettings.HideTopLevelNodeFromPath ? sender.GetRootContent().SelectMany(d => d.Children()).ToArray() : sender.GetRootContent();

					var contentEnum = docs as IContent[] ?? docs.ToArray();
					FixRootCategoryUrlName(e.Entity, contentEnum, "url");

					foreach (var store in StoreHelper.GetAllStores())
					{
						string multiStorePropertyName;
						if (GetMultiStorePropertyName(e.Entity, store, out multiStorePropertyName)) continue;
						FixRootCategoryUrlName(e.Entity, contentEnum, multiStorePropertyName);
					}
				}
			}
			catch (Exception exception)
			{
				LogHelper.Error<ApplicationEventHandler>("ContentService_Created", exception);
			}
		}

        //public delegate void ContentOnAfterUpdateDocumentCacheEventHandler();
        //public static event ContentOnAfterUpdateDocumentCacheEventHandler _ContentOnAfterUpdateDocumentCache;

        public static event ContentOnAfterUpdateDocumentCacheEventHandler _ContentOnAfterUpdateDocumentCache;

        public delegate void ContentOnAfterUpdateDocumentCacheEventHandler(ContentOnAfterUpdateDocumentCacheEventArgs e);

        public class ContentOnAfterUpdateDocumentCacheEventArgs : EventArgs
        {
            public IEnumerable<IContent> PublishedEntities = null;
        }

		private static void ContentService_Published(IPublishingStrategy strategy, PublishEventArgs<IContent> e)
		{
			try
            {
                var umbHelper = new UmbracoHelper(UmbracoContext.Current);
				var contentService = ApplicationContext.Current.Services.ContentService;
				var contents = e.PublishedEntities.Where(c => c.ContentType.Alias.StartsWith(Order.NodeAlias));

                if (contents.Any())
                {
                    strategy.UnPublish(contents, 0);
                }

				Log.Instance.LogDebug("Published Event: Iteriate");

				// when thinking about adding something here, consider ContentOnAfterUpdateDocumentCache!
				foreach (var sender in e.PublishedEntities)
				{
                    var content = sender;
					//todo: work with aliasses from config
					var alias = content.ContentType.Alias;
					// todo: make a nice way for this block
					if (Product.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (ProductVariant.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (Category.IsAlias(alias))
					{
						Log.Instance.LogDebug("Published Event: Category: " + content.Id);

						ResetAll(content.Id, alias);
					}
					else if (PaymentProvider.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (PaymentProviderMethod.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (DiscountProduct.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (DiscountOrder.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (ShippingProvider.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (ShippingProviderMethod.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (Store.IsAlias(alias))
					{
						ResetAll(content.Id, alias);
					}
					else if (alias.StartsWith("uwbs") && alias != Order.NodeAlias)
					{
						ResetAll(content.Id, alias);
					}

                    if (content.HasProperty(Constants.StorePickerAlias))
					{
						var storeId = content.GetValue<int>(Constants.StorePickerAlias);

						var storeService = StoreHelper.StoreService;
						var storeById = storeService.GetById(storeId, null);
						if (storeById != null)
						{
							storeService.TriggerStoreChangedEvent(storeById);
						}
					}

                    if (alias.StartsWith(Settings.NodeAlias))
					{
						IO.Container.Resolve<ISettingsService>().TriggerSettingsChangedEvent(SettingsLoader.GetSettings());
					}

                    if (alias.StartsWith(Store.NodeAlias))
					{
						var storeService = StoreHelper.StoreService;
						storeService.TriggerStoreChangedEvent(storeService.GetById(content.Id, null));
						var node = umbHelper.TypedContent(content.Id);
						if (!content.Name.Equals(node.Name))
						{
							StoreHelper.RenameStore(node.Name, content.Name);
						}
					}

                    if (alias == "uwbsDiscountOrder" || alias == "uwbsDiscountOrderCoupon")
                    {
                        UpdateCoupons(content);
                    }

                    _ContentOnAfterUpdateDocumentCache(new ContentOnAfterUpdateDocumentCacheEventArgs
                    {
                        PublishedEntities = e.PublishedEntities
                    });
                }
			}
			catch (Exception exception)
			{
				LogHelper.Error<ApplicationEventHandler>("ContentService_Published", exception);
			}
		}

        private static void UpdateCoupons(IContent node)
        {
            try
            {

                var couponValue = node.GetValue<string>("couponCodes");

                if (!string.IsNullOrEmpty(couponValue))
                {
                    List<Coupon> coupons = JsonConvert.DeserializeObject<List<Coupon>>(couponValue);

                    RemoveCoupons(node.Id);

                    using (var db = ApplicationContext.Current.DatabaseContext.Database)
                    {

                        foreach (var coupon in coupons)
                        {

                            var newCoupon = new uWebshopCoupons()
                            {
                                DiscountId = node.Id,
                                CouponCode = coupon.CouponCode,
                                NumberAvailable = coupon.NumberAvailable,
                                uniqueID = node.Key
                            };

                            db.Insert(newCoupon);
                        }

                    }

                }

            } catch(Exception ex)
            {
                Log.Instance.LogError(ex, "Update Coupon Failed!");
            }

        }

        private static void RemoveCoupons(int discountId)
        {
            using (var db = ApplicationContext.Current.DatabaseContext.Database)
            {
                var currentCoupons = db.Fetch<uWebshopCoupons>("SELECT * FROM uWebshopCoupons WHERE DiscountId = @0", discountId);

                foreach (var coupon in currentCoupons)
                {
                    db.Delete(coupon);
                }
            }

        }

        public class Coupon
        {
            public string CouponCode { get; set; }
            public int NumberAvailable { get; set; }
        }

        [TableName("uWebshopCoupons")]
        [PrimaryKey("Id", autoIncrement = true)]
        public class uWebshopCoupons
        {
            public int DiscountId { get; set; }
            public string CouponCode { get; set; }
            public int NumberAvailable { get; set; }
            public int Id { get; set; }
            public Guid uniqueID { get; set; }
        }

    }
}
