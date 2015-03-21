using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using Examine;
using umbraco;
using umbraco.BasePages;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using uWebshop.API;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
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
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;
using Catalog = uWebshop.Domain.Catalog;
using Constants = uWebshop.Common.Constants;
using Log = uWebshop.Domain.Log;
using Store = uWebshop.Domain.Store;

namespace uWebshop.Umbraco
{
	public class NewApplicationEventHandler : IApplicationEventHandler
	{
		public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			try
			{
				Initialize.ContinueInitialization();
				Log.Instance.LogDebug("uWebshop initialized");
			}
			catch (Exception ex)
			{
				LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Error while initializing uWebshop, most likely due to wrong umbraco.config, please republish the site " + ex.Message, ex);
			}
		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			
		}

		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			ContentService.Created += ContentService_Created;
			ContentService.Published += ContentService_Published;
			ContentService.Publishing +=ContentService_Publishing;
			ContentService.Trashed += ContentService_Trashed;
			ContentService.UnPublished += ContentService_UnPublished;
			ContentService.Deleted += ContentService_Deleted;
			ContentService.Copied += ContentService_Copied;

			content.AfterUpdateDocumentCache += ContentOnAfterUpdateDocumentCache;
			OrderInfo.BeforeStatusChanged += OrderBeforeStatusChanged;
			OrderInfo.AfterStatusChanged += OrderEvents.OrderStatusChanged;

			UmbracoDefault.BeforeRequestInit += UmbracoDefaultBeforeRequestInit;
			UmbracoDefault.AfterRequestInit += UmbracoDefaultAfterRequestInit;

			var indexer = ExamineManager.Instance.IndexProviderCollection[UwebshopConfiguration.Current.ExamineIndexer];
			indexer.GatheringNodeData += GatheringNodeDataHandler;
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
					//Log.Instance.LogDebug( "examine MNTP before: " + mntp);
					//let's get rid of those commas!
					csvFieldValue = csvFieldValue.Replace(",", " ");
					//Log.Instance.LogDebug( "examine MNTP after: " + mntp);
					//now put it back into the Fields so we can pretend nothing happened!
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
			return new List<string> {"categories",
				//"metaTags", metatags can't be stripped of comma
				"images", "files"};
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


		private static void ResetAll(int id, string nodeTypeAlias)
		{
			IO.Container.Resolve<IApplicationCacheManagingService>().ReloadEntityWithGlobalId(id, nodeTypeAlias);
			//UmbracoStaticCachedEntityRepository.ResetStaticCache();
		}
		private void ContentOnAfterUpdateDocumentCache(Document document, DocumentCacheEventArgs documentCacheEventArgs)
		{
		
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
			}
		}

	

		private static void ClearCaches(IContent sender)
		{
			//todo: work with aliasses from config
			//if (sender.ContentType.Alias.StartsWith("uwbs") && sender.ContentType.Alias != Order.NodeAlias)
			if (sender.ContentType.Alias != Order.NodeAlias)
			{
				IO.Container.Resolve<IApplicationCacheManagingService>().UnloadEntityWithGlobalId(sender.Id, sender.ContentType.Alias);
				//UmbracoStaticCachedEntityRepository.ResetStaticCache();
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
				//Domain.Core.Initialize.Init();
			}
			catch (Exception)
			{
				//throw;
			}
			try
			{
				var request = UmbracoContext.Current.PublishedContentRequest;
				var currentNode = request.PublishedContent;

				if (ProductVariant.IsAlias(currentNode.DocumentTypeAlias))
				{
					var product = DomainHelper.GetProductById(currentNode.Parent.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Product.IsAlias(currentNode.DocumentTypeAlias))
				{
					var product = DomainHelper.GetProductById(currentNode.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Category.IsAlias(currentNode.DocumentTypeAlias))
				{
					var category = DomainHelper.GetCategoryById(currentNode.Id);
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

			var currentMember = Membership.GetUser();

			var currentorder = UwebshopRequest.Current.OrderInfo ?? OrderHelper.GetOrder();

			var orderRepository = IO.Container.Resolve<IOrderRepository>();
			if (currentorder != null && (currentMember != null && currentorder.CustomerInfo.LoginName != currentMember.UserName))
			{
				orderRepository.SetCustomer(currentorder.UniqueOrderId, currentMember.UserName);
				currentorder.CustomerInfo.LoginName = currentMember.UserName;

				if (currentMember.ProviderUserKey != null)
				{
					orderRepository.SetCustomerId(currentorder.UniqueOrderId, (int)currentMember.ProviderUserKey);
					currentorder.CustomerInfo.CustomerId = (int) currentMember.ProviderUserKey;
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

			var cookie = HttpContext.Current.Request.Cookies["StoreInfo"];

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
						var wishlistName = "Wishlist";

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

			// todo: ombouwen naar UwebshopRequest.Current.Category, UwebshopRequest.Current lostrekken (ivm speed)
			var currentCategoryId = HttpContext.Current.Request["resolvedCategoryId"];
			var currentProductId = HttpContext.Current.Request["resolvedProductId"];

			if (!string.IsNullOrEmpty(currentCategoryId)) //string.IsNullOrEmpty(currentProductUrl))
			{
				int categoryId;
				if (!int.TryParse(currentCategoryId, out categoryId))
					return;
				var categoryFromUrl = DomainHelper.GetCategoryById(categoryId);
				if (categoryFromUrl == null) return;

				if (categoryFromUrl.Disabled)
				{
					HttpContext.Current.Response.StatusCode = 404;
					HttpContext.Current.Response.Redirect(library.NiceUrl(int.Parse(GetCurrentNotFoundPageId())), true);
					return;
				}

				if (Access.HasAccess(categoryFromUrl.Id, categoryFromUrl.GetUmbracoPath(), Membership.GetUser()))
				{
					if (categoryFromUrl.Template != 0)
					{
						//umbraco.cms.businesslogic.template.Template.GetTemplate(currentCategory.Template).TemplateFilePath
						((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(categoryFromUrl.Template);
						//// get the template
						//var t = template.GetMasterPageName(currentCategory.Template);
						//// you did this and it works pre-4.10, right?
						//page.MasterPageFile = t;
						//// now this should work starting with 4.10
						//e.Page.Template = t;
					}

					var altTemplate = HttpContext.Current.Request["altTemplate"];
					if (!string.IsNullOrEmpty(altTemplate))
					{
						
						
						var altTemplateFile = fileService.GetTemplate(altTemplate);

						if (altTemplateFile != null)
						{
							((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(altTemplateFile.Id);
						}
					}
				}
				else
				{
					if (HttpContext.Current.User.Identity.IsAuthenticated)
					{
						HttpContext.Current.Response.Redirect(library.NiceUrl(Access.GetErrorPage(categoryFromUrl.GetUmbracoPath())), true);
					}
					HttpContext.Current.Response.Redirect(library.NiceUrl(Access.GetLoginPage(categoryFromUrl.GetUmbracoPath())), true);
				}
			}
			else if (!string.IsNullOrEmpty(currentProductId)) // else
			{
				int productId;
				if (!int.TryParse(currentProductId, out productId))
					return;
				var productFromUrl = DomainHelper.GetProductById(productId);
				if (productFromUrl == null) return;

				if (Access.HasAccess(productFromUrl.Id, productFromUrl.Path(), Membership.GetUser()))
				{
					if (productFromUrl.Template != 0)
					{
						((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(productFromUrl.Template);
					}

					var altTemplate = HttpContext.Current.Request["altTemplate"];
					if (!string.IsNullOrEmpty(altTemplate))
					{
						var altTemplateId = fileService.GetTemplate(altTemplate);

						if (altTemplateId != null)
						{
							((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(altTemplateId.Id);
						}
					}
				}
				else
				{
					if (HttpContext.Current.User.Identity.IsAuthenticated)
					{
						HttpContext.Current.Response.Redirect(library.NiceUrl(Access.GetErrorPage(productFromUrl.Path())), true);
					}
					HttpContext.Current.Response.Redirect(library.NiceUrl(Access.GetLoginPage(productFromUrl.Path())), true);
				}
			}
		}

		internal static string GetCurrentNotFoundPageId()
		{
			library.GetCurrentDomains(1);
			var error404 = "";
			var error404Node = UmbracoSettings.GetKeyAsNode("/settings/content/errors/error404");
			if (error404Node.ChildNodes.Count > 0 && error404Node.ChildNodes[0].HasChildNodes)
			{
				// try to get the 404 based on current culture (via domain)
				XmlNode cultureErrorNode;
				if (umbraco.cms.businesslogic.web.Domain.Exists(HttpContext.Current.Request.ServerVariables["SERVER_NAME"]))
				{
					var d = umbraco.cms.businesslogic.web.Domain.GetDomain(HttpContext.Current.Request.ServerVariables["SERVER_NAME"]);
					// test if a 404 page exists with current culture
					cultureErrorNode = error404Node.SelectSingleNode(string.Format("errorPage [@culture = '{0}']", d.Language.CultureAlias));
					if (cultureErrorNode != null && cultureErrorNode.FirstChild != null)
						error404 = cultureErrorNode.FirstChild.Value;
				}
				else if (error404Node.SelectSingleNode(string.Format("errorPage [@culture = '{0}']", Thread.CurrentThread.CurrentUICulture.Name)) != null)
				{
					cultureErrorNode = error404Node.SelectSingleNode(string.Format("errorPage [@culture = '{0}']", Thread.CurrentThread.CurrentUICulture.Name));
					if (cultureErrorNode.FirstChild != null)
						error404 = cultureErrorNode.FirstChild.Value;
				}
				else
				{
					cultureErrorNode = error404Node.SelectSingleNode("errorPage [@culture = 'default']");
					if (cultureErrorNode != null && cultureErrorNode.FirstChild != null)
						error404 = cultureErrorNode.FirstChild.Value;
				}
			}
			else
				error404 = UmbracoSettings.GetKey("/settings/content/errors/error404");
			return error404;
		}


		protected void OrderBeforeStatusChanged(OrderInfo orderInfo, BeforeOrderStatusChangedEventArgs e)
		{
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
			foreach (var item in e.PublishedEntities)
			{
				if (Category.IsAlias(item.ContentType.Alias))
				{
					SetAliasedPropertiesIfEnabled(item, "categoryUrl");
				}
				if (Product.IsAlias(item.ContentType.Alias))
				{
					SetAliasedPropertiesIfEnabled(item, "productUrl");
				}

				if (item.ContentType.Alias == Order.NodeAlias)
				{
					// order => todo: delete node, update SQL
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

			var parent = sender.GetParent(e.Entity.Id);

			if (parent == null || parent.Id < 0)
			{
				return;
			}

			var parentDoc = sender.GetById(parent.Id);
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

		private static void ContentService_Published(IPublishingStrategy strategy, PublishEventArgs<IContent> e)
		{
			var umbHelper = new UmbracoHelper(UmbracoContext.Current);
			var contentService = ApplicationContext.Current.Services.ContentService;
			var contents = e.PublishedEntities.Where(c => c.ContentType.Alias.StartsWith(Order.NodeAlias));
			strategy.UnPublish(contents, 0);

			// when thinking about adding something here, consider ContentOnAfterUpdateDocumentCache!

			foreach (var sender in e.PublishedEntities)
			{
				if (sender.Level > 2)
				{
					if (sender.ContentType.Alias == Order.NodeAlias ||
					    sender.Parent() != null &&
					    (OrderedProduct.IsAlias(sender.ContentType.Alias) ||
					     sender.Parent().Parent() != null && OrderedProductVariant.IsAlias(sender.ContentType.Alias)))
					{
						var orderDoc = sender.ContentType.Alias == Order.NodeAlias
							? sender
							: (OrderedProduct.IsAlias(sender.ContentType.Alias) && !OrderedProductVariant.IsAlias(sender.ContentType.Alias)
								? contentService.GetById(sender.Parent().Id)
								: contentService.GetById(sender.Parent().Parent().Id));

						if (orderDoc.ContentType.Alias != Order.NodeAlias)
							throw new Exception("There was an error in the structure of the order documents");

						// load existing orderInfo (why..? => possibly to preserve information not represented in the umbraco documents)

						if (orderDoc.HasProperty("orderGuid"))
						{
							var orderGuidString = orderDoc.GetValue<string>("orderGuid");

							if (orderDoc.HasProperty("orderGuid") && string.IsNullOrEmpty(orderGuidString))
							{
								Store store = null;
								var storeDoc =
									sender.Ancestors().FirstOrDefault(x => x.ContentType.Alias == OrderStoreFolder.NodeAlias);

								if (storeDoc != null)
								{
									store = StoreHelper.GetAllStores().FirstOrDefault(x => x.Name == storeDoc.Name);
								}

								if (store == null)
								{
									store = StoreHelper.GetAllStores().FirstOrDefault();
								}

								var orderInfo = OrderHelper.CreateOrder(store);
								IO.Container.Resolve<IOrderNumberService>().GenerateAndPersistOrderNumber(orderInfo);
								orderInfo.Status = OrderStatus.Confirmed;
								orderInfo.Save();

								sender.SetValue("orderGuid", orderInfo.UniqueOrderId.ToString());
								contentService.Save(sender);
							}
							else
							{
								var orderGuid = orderDoc.GetValue<Guid>("orderGuid");

								var orderInfo = OrderHelper.GetOrder(orderGuid);

								var order = new Order(orderDoc.Id);
								orderInfo.CustomerEmail = order.CustomerEmail;
								orderInfo.CustomerFirstName = order.CustomerFirstName;
								orderInfo.CustomerLastName = order.CustomerLastName;


								var dictionaryCustomer =
									orderDoc.Properties.Where(x => x.Alias.StartsWith("customer"))
										.ToDictionary(customerProperty => customerProperty.Alias,
											customerProperty => customerProperty.Value.ToString());

								orderInfo.AddCustomerFields(dictionaryCustomer, CustomerDatatypes.Customer);

								var dictionaryShipping =
									orderDoc.Properties.Where(x => x.Alias.StartsWith("shipping"))
										.ToDictionary(property => property.Alias, property => property.Value.ToString());
								orderInfo.AddCustomerFields(dictionaryShipping, CustomerDatatypes.Shipping);

								var dictionarExtra =
									orderDoc.Properties.Where(x => x.Alias.StartsWith("extra"))
										.ToDictionary(property => property.Alias, property => property.Value.ToString());
								orderInfo.AddCustomerFields(dictionarExtra, CustomerDatatypes.Extra);

								//orderInfo.SetVATNumber(order.CustomerVATNumber); happens in AddCustomerFields
								var orderPaidProperty = order.Document.getProperty("orderPaid");
								if (orderPaidProperty != null && orderPaidProperty.Value != null)
									orderInfo.Paid = orderPaidProperty.Value == "1";

								// load data recursively from umbraco documents into order tree
								orderInfo.OrderLines = orderDoc.Children().Select(d =>
								{
									var fields =
										d.Properties.Where(x => !OrderedProduct.DefaultProperties.Contains(x.Alias))
											.ToDictionary(s => s.Alias, s => d.GetValue<string>(s.Alias));

									var xDoc = new XDocument(new XElement("Fields"));

									OrderUpdatingService.AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, d.ContentType.Alias);

									var orderedProduct = new OrderedProduct(d.Id);

									var productInfo = new ProductInfo(orderedProduct, orderInfo);
									productInfo.ProductVariants =
										d.Children()
											.Select(cd => new ProductVariantInfo(new OrderedProductVariant(cd.Id), productInfo, productInfo.Vat))
											.ToList();
									return new OrderLine(productInfo, orderInfo) {_customData = xDoc};
								}).ToList();

								// store order
								IO.Container.Resolve<IOrderRepository>().SaveOrderInfo(orderInfo);
							}
							// cancel does give a warning message balloon in Umbraco.
							//e.Cancel = true;

							//if (sender.ContentType.Alias != Order.NodeAlias)
							//{
							//	orderDoc.Publish(new User(0));
							//}

							//if (orderDoc.ParentId != 0)
							//{
							BasePage.Current.ClientTools.SyncTree(sender.Parent().Path, false);
							BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", sender.Id));
							//}
							//orderDoc.delete();
							BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Order Updated!",
								"This order has been updated!");
						}
					}
				}




				var content = contentService.GetById(sender.Id);
				//if (sender.ContentType.Alias.StartsWith("uwbs") && sender.ContentType.Alias != Order.NodeAlias)
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
					var node = umbHelper.Content(content.Id);
					if (!content.Name.Equals(node.Name))
					{
						StoreHelper.RenameStore(node.Name, content.Name);
					}
				}
			}
		}
	}
}