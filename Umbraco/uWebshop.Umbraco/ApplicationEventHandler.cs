using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using Examine;
using umbraco.businesslogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using umbraco.cms.helpers;
using umbraco.cms.presentation.Trees;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using umbraco.NodeFactory;
using umbraco.presentation.masterpages;
using uWebshop.API;
using uWebshop.Umbraco;
using uWebshop.Umbraco.Services;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.DataAccess;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Umbraco.Businesslogic;
using Catalog = uWebshop.Domain.Catalog;
using Log = uWebshop.Domain.Log;
using Settings = uWebshop.Domain.Settings;
using Store = uWebshop.Domain.Store;
using DeleteEventArgs = umbraco.cms.businesslogic.DeleteEventArgs;

namespace uWebshop.Umbraco6
{
	public class ApplicationEventHandler : ApplicationStartupHandler
	{
		static ApplicationEventHandler()
		{
			try
			{
				Domain.Core.Initialize.ContinueInitialization();
				Log.Instance.LogDebug("uWebshop initialized");
			}
			catch (Exception ex)
			{
				umbraco.BusinessLogic.Log.Add(LogTypes.Error, 0, "Error while initializing uWebshop, most likely due to wrong umbraco.config, please republish the site " + ex.Message);
			}
		}

		#region constructors

		public ApplicationEventHandler()
		{
			ContentService.Created += ContentService_Created;
			ContentService.Published += ContentService_Published;
			Document.BeforePublish += DocumentBeforePublish;
			Document.AfterPublish += DocumentAfterPublish;
			Document.BeforeSave += DocumentBeforeSave;
			Document.AfterSave += DocumentAfterSave;
			Document.AfterMoveToTrash += DocumentAfterMoveToTrash;
			Document.AfterUnPublish += DocumentOnAfterUnPublish;
			Document.AfterDelete += DocumentOnAfterDelete;
			DocumentType.AfterSave += DocumentTypeOnAfterSave;
			Document.AfterCopy += DocumentAfterCopy;
			BaseTree.BeforeNodeRender += BaseContentTree_BeforeNodeRender;
			umbracoPage.Load += UmbracoPageLoad;
			content.AfterUpdateDocumentCache += ContentOnAfterUpdateDocumentCache;
			OrderInfo.BeforeStatusChanged += OrderBeforeStatusChanged;
			OrderInfo.AfterStatusChanged += OrderEvents.OrderStatusChanged;
			//OrderInfo.OrderLoaded += OrderInfoOrderLoaded;	example

			UmbracoDefault.BeforeRequestInit += UmbracoDefaultBeforeRequestInit;
			UmbracoDefault.AfterRequestInit += UmbracoDefaultAfterRequestInit;

            var indexer = ExamineManager.Instance.IndexProviderCollection[UwebshopConfiguration.Current.ExamineIndexer];
            indexer.GatheringNodeData += GatheringNodeDataHandler;
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
                    Domain.Log.Instance.LogError("GatheringNodeDataHandler defaultPriceValues Examine: " + ex);
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
                    Domain.Log.Instance.LogError("GatheringNodeDataHandler DefaultCsvValues Examine: " + ex);
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

		private void DocumentAfterCopy(Document sender, CopyEventArgs e)
		{
			if (sender.Level > 2 && sender.ContentType.Alias == Order.NodeAlias)
			{
				Guid currentGuid;
				var orderGuid = sender.getProperty("orderGuid").Value.ToString();

				Guid.TryParse(orderGuid, out currentGuid);

				var order = OrderHelper.GetOrder(currentGuid);

				var newOrder = OrderHelper.CreateNewOrderFromExisting(order);

				IO.Container.Resolve<IOrderNumberService>().GenerateAndPersistOrderNumber(order);

				order.OrderNodeId = e.NewDocument.Id;
				order.Save();

				e.NewDocument.Text = order.OrderNumber;
				e.NewDocument.SetProperty("orderGuid", newOrder.UniqueOrderId.ToString());
				e.NewDocument.SetProperty("orderStatusPicker", newOrder.Status.ToString());

				e.NewDocument.Save();

				BasePage.Current.ClientTools.SyncTree(e.NewDocument.Parent.Path, false);
				BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", e.NewDocument.Id));
			}
		}

		private void DocumentOnAfterDelete(Document sender, DeleteEventArgs e)
		{
			ClearCaches(sender);
		}

		//public void OrderInfoOrderLoaded(OrderInfo orderInfo)
		//{ // example
		//	const int productLimit = 10;
		//	orderInfo.RegisterCustomOrderValidation(
		//	order => order.OrderLines.All(line => line.ProductInfo.ItemCount.GetValueOrDefault(1) <= productLimit),
		//	order => "TheUniqueKeyThatWillShowInTheError");
		//}

		private void DocumentTypeOnAfterSave(DocumentType sender, SaveEventArgs saveEventArgs)
		{
			//if (UwebshopConfiguration.Current.RebuildExamineIndex && ExamineManager.Instance != null)
			//{
			//	var externalIndex = ExamineManager.Instance.IndexProviderCollection["ExternalIndexer"];
			//	if (externalIndex != null) externalIndex.RebuildIndex();
			//}
			// todo: clear full cache after some time
		}

		private void ResetAll(int id, string nodeTypeAlias)
		{
			IO.Container.Resolve<IApplicationCacheManagingService>().ReloadEntityWithGlobalId(id, nodeTypeAlias);
			//UmbracoStaticCachedEntityRepository.ResetStaticCache();
		}
		private void ContentOnAfterUpdateDocumentCache(Document sender, DocumentCacheEventArgs documentCacheEventArgs)
		{
			//if (sender.ContentType.Alias.StartsWith("uwbs") && sender.ContentType.Alias != Order.NodeAlias)
			//todo: work with aliasses from config
			var alias = sender.ContentType.Alias;
			// todo: make a nice way for this block
			if (Product.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (ProductVariant.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (Category.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (PaymentProvider.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (PaymentProviderMethod.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (DiscountProduct.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (DiscountOrder.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (ShippingProvider.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (ShippingProviderMethod.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (Store.IsAlias(alias))
			{
				ResetAll(sender.Id, alias);
			}
			else if (alias.StartsWith("uwbs") && alias != Order.NodeAlias)
			{
				ResetAll(sender.Id, alias);
			}
			
			var storePickerProperty = sender.getProperty(Constants.StorePickerAlias);
			if (storePickerProperty != null)
			{
				int storeId;
				if (storePickerProperty.Value != null && int.TryParse(storePickerProperty.Value.ToString(), out storeId))
				{
					var storeService = StoreHelper.StoreService;
					var storeById = storeService.GetById(storeId, null);
					if (storeById != null)
					{
						storeService.TriggerStoreChangedEvent(storeById);
					}
				}
			}

			if (alias.StartsWith(Settings.NodeAlias))
			{
				IO.Container.Resolve<ISettingsService>().TriggerSettingsChangedEvent(SettingsLoader.GetSettings());
			}

			if (alias.StartsWith(Store.NodeAlias))
			{
                //todo: naar nieuwe v6+ API omzetten
				var storeService = StoreHelper.StoreService;
				storeService.TriggerStoreChangedEvent(storeService.GetById(sender.Id, null));
				var node = new Node(sender.Id);
				if (!sender.Text.Equals(node.Name))
				{
					StoreHelper.RenameStore(node.Name, sender.Text);
				}
			}
		}


		private void DocumentOnAfterUnPublish(Document sender, UnPublishEventArgs unPublishEventArgs)
		{
			// todo: check whether this event is thrown when node is automatically unpublished by umbraco (after certain datetime)
			ClearCaches(sender);
		}

		private static void ClearCaches(Document sender)
		{
			//todo: work with aliasses from config
			//if (sender.ContentType.Alias.StartsWith("uwbs") && sender.ContentType.Alias != Order.NodeAlias)
			if (sender.ContentType.Alias != Order.NodeAlias)
			{
				IO.Container.Resolve<IApplicationCacheManagingService>().UnloadEntityWithGlobalId(sender.Id, sender.ContentType.Alias);
				//UmbracoStaticCachedEntityRepository.ResetStaticCache();
			}
		}

		private static void DocumentBeforeSave(Document sender, SaveEventArgs e)
		{
			if (sender.ContentType.Alias.StartsWith(Store.NodeAlias))
			{
				var reg = new Regex(@"\s*");
				var storeAlias = reg.Replace(sender.Text, "");

				sender.Text = storeAlias;
				return;
			}
			
			if (StoreHelper.GetAllStores().Any())
			{
				// if page content
				// determine url(s) for this content
				// use to resolve catalog item, if resolve rename

				var cmsApplication = IO.Container.Resolve<ICMSApplication>();

				//var urls = new List<string>();
				//var currentDoc = sender;
				//while (currentDoc.ParentId > 0)
				//{
				//	urls.Add(cmsApplication.ApplyUrlFormatRules(currentDoc.Text.ToLowerInvariant()));
				//	currentDoc = new Document(currentDoc.ParentId);
				//}
				

				// if category/product

				//// todo: wrong
				//var urlNameforDocument = cmsApplication.ApplyUrlFormatRules(sender.Text.ToLowerInvariant());
				//var doesCategoryWithNameAlreadyExists = IO.Container.Resolve<ICatalogUrlSplitterService>().DetermineCatalogUrlComponents("/" + urlNameforDocument, true);
				//if (!Category.IsAlias(sender.ContentType.Alias) && doesCategoryWithNameAlreadyExists.StoreNodeUrl != "/")
				//{
				//	//_catalogUrlResolvingService.GetCategoryFromUrlName(categoryUrlName);
				//	var category = IO.Container.Resolve<ICatalogUrlResolvingService>().GetCategoryFromUrlName(doesCategoryWithNameAlreadyExists.StoreNodeUrl);

				//	if (category != null)
				//	{
				//		var path = IO.Container.Resolve<ICMSEntityRepository>().GetByGlobalId(category.Id).Path;

				//		if (sender.Path.Split(',').Count() == path.Split(',').Count() - 2)
				//		{
				//			sender.Text = sender.Text + " (1)";
				//		}
				//	}
				//}
			}

			var parentId = sender.ParentId;

			if (parentId < 0)
			{
				return;
			}

			var parentDoc = new Document(sender.ParentId);
			if (parentDoc.ContentType != null && (Category.IsAlias(sender.ContentType.Alias) && parentDoc.ContentType.Alias == Catalog.CategoryRepositoryNodeAlias))
			{
				var docs = GlobalSettings.HideTopLevelNodeFromPath ? Document.GetRootDocuments().SelectMany(d => d.Children).ToArray() : Document.GetRootDocuments();

				FixRootCategoryUrlName(sender, docs, "url");

				foreach (var store in StoreHelper.GetAllStores())
				{
					string multiStorePropertyName;
					if (GetMultiStorePropertyName(sender, store, out multiStorePropertyName)) continue;
					FixRootCategoryUrlName(sender, docs, multiStorePropertyName);
				}
			}
		}

		private static bool GetMultiStorePropertyName(Document sender, Store store, out string propertyName)
		{
			propertyName = "url_" + store.Alias.ToLower();

			if (sender.getProperty(propertyName) != null) return false;
			propertyName = "url_" + store.Alias.ToUpper();

			if (sender.getProperty(propertyName) != null) return false;
			propertyName = "url_" + store.Alias;

			return sender.getProperty(propertyName) == null;
		}

		private static void FixRootCategoryUrlName(Document sender, Document[] docs, string propertyName)
		{
			var urlName = sender.getProperty(propertyName).Value.ToString();
			if (docs.Any(x => urlName == x.Text))
			{
				int count = 1;
				var existingRenames = docs.Where(x => x.Text.StartsWith(urlName)).Select(x => x.Text).Select(x => Regex.Match(x, urlName + @" [(](\d)[)]").Groups[1].Value).Select(x =>
					{
						int i;
						int.TryParse(x, out i);
						return i;
					});
				if (existingRenames.Any())
				{
					count = existingRenames.Max() + 1;
				}
				sender.SetProperty(propertyName, urlName + " (" + count + ")");
			}
		}

		private static void UmbracoPageLoad(object sender, EventArgs e)
		{
		}

		private static void DocumentAfterMoveToTrash(Document sender, MoveToTrashEventArgs e)
		{
			if (sender.ContentType.Alias.StartsWith(Store.NodeAlias))
			{
				var reg = new Regex(@"\s*");
				var storeAlias = reg.Replace(sender.Text, "");
				StoreHelper.UnInstallStore(storeAlias);
			}
			ClearCaches(sender);
		}

		//private static void DocumentNew(Document sender, NewEventArgs e)
		//{
		//	if (sender.ContentType.Alias.StartsWith(Store.NodeAlias))
		//	{
		//		var reg = new Regex(@"\s*");
		//		var storeAlias = reg.Replace(sender.Text, "");

		//		Umbraco.Helpers.InstallStore(storeAlias, sender);

		//		sender.Text = storeAlias;
		//		sender.Save();
		//	}
		//}

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
				var currentNode = Node.GetCurrent();

				if (ProductVariant.IsAlias(currentNode.NodeTypeAlias))
				{
					var product = DomainHelper.GetProductById(currentNode.Parent.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Product.IsAlias(currentNode.NodeTypeAlias))
				{
					var product = DomainHelper.GetProductById(currentNode.Id);
					if (product != null) HttpContext.Current.Response.RedirectPermanent(product.NiceUrl(), true);
				}
				else if (Category.IsAlias(currentNode.NodeTypeAlias))
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
						var altTemplateId = umbraco.cms.businesslogic.template.Template.GetTemplateIdFromAlias(altTemplate);

						if (altTemplateId != 0)
						{
							((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(altTemplateId);
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
						var altTemplateId = umbraco.cms.businesslogic.template.Template.GetTemplateIdFromAlias(altTemplate);

						if (altTemplateId != 0)
						{
							((UmbracoDefault) sender).MasterPageFile = template.GetMasterPageName(altTemplateId);
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
				else if (error404Node.SelectSingleNode(string.Format("errorPage [@culture = '{0}']", System.Threading.Thread.CurrentThread.CurrentUICulture.Name)) != null)
				{
					cultureErrorNode = error404Node.SelectSingleNode(string.Format("errorPage [@culture = '{0}']", System.Threading.Thread.CurrentThread.CurrentUICulture.Name));
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

		#endregion

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

		public static void DocumentAfterSave(Document sender, SaveEventArgs e)
		{
		}

		protected void DocumentAfterPublish(Document sender, PublishEventArgs e)
		{
			// when thinking about adding something here, consider ContentOnAfterUpdateDocumentCache!

			if (sender.Level > 2)
			{
				if (sender.ContentType.Alias == Order.NodeAlias || sender.Parent != null && (OrderedProduct.IsAlias(sender.ContentType.Alias) || sender.Parent.Parent != null && OrderedProductVariant.IsAlias(sender.ContentType.Alias)))
				{
					var orderDoc = sender.ContentType.Alias == Order.NodeAlias ? sender : (OrderedProduct.IsAlias(sender.ContentType.Alias) && !OrderedProductVariant.IsAlias(sender.ContentType.Alias) ? new Document(sender.Parent.Id) : new Document(sender.Parent.Parent.Id));

					if (orderDoc.ContentType.Alias != Order.NodeAlias)
						throw new Exception("There was an error in the structure of the order documents");

					// load existing orderInfo (why..? => possibly to preserve information not represented in the umbraco documents)

					if (string.IsNullOrEmpty(orderDoc.getProperty("orderGuid").Value.ToString()))
					{
						Store store = null;
						var storeDoc = sender.GetAncestorDocuments().FirstOrDefault(x => x.ContentType.Alias == OrderStoreFolder.NodeAlias);

						if (storeDoc != null)
						{
							store = StoreHelper.GetAllStores().FirstOrDefault(x => x.Name == storeDoc.Text);
						}

						if (store == null)
						{
							store = StoreHelper.GetAllStores().FirstOrDefault();
						}

						var orderInfo = OrderHelper.CreateOrder(store);
						IO.Container.Resolve<IOrderNumberService>().GenerateAndPersistOrderNumber(orderInfo);
						orderInfo.Status = OrderStatus.Confirmed;
						orderInfo.Save();

						sender.SetProperty("orderGuid", orderInfo.UniqueOrderId.ToString());
						sender.Save();
					}
					else
					{
						var orderGuid = Guid.Parse(orderDoc.getProperty("orderGuid").Value.ToString());

						var orderInfo = OrderHelper.GetOrderInfo(orderGuid);

						var order = new Order(orderDoc.Id);
						orderInfo.CustomerEmail = order.CustomerEmail;
						orderInfo.CustomerFirstName = order.CustomerFirstName;
						orderInfo.CustomerLastName = order.CustomerLastName;

						var dictionaryCustomer = orderDoc.GenericProperties.Where(x => x.PropertyType.Alias.StartsWith("customer")).ToDictionary(customerProperty => customerProperty.PropertyType.Alias, customerProperty => customerProperty.Value.ToString());
						orderInfo.AddCustomerFields(dictionaryCustomer, CustomerDatatypes.Customer);

						var dictionaryShipping = orderDoc.GenericProperties.Where(x => x.PropertyType.Alias.StartsWith("shipping")).ToDictionary(property => property.PropertyType.Alias, property => property.Value.ToString());
						orderInfo.AddCustomerFields(dictionaryShipping, CustomerDatatypes.Shipping);

						var dictionarExtra = orderDoc.GenericProperties.Where(x => x.PropertyType.Alias.StartsWith("extra")).ToDictionary(property => property.PropertyType.Alias, property => property.Value.ToString());
						orderInfo.AddCustomerFields(dictionarExtra, CustomerDatatypes.Extra);

						//orderInfo.SetVATNumber(order.CustomerVATNumber); happens in AddCustomerFields
						var orderPaidProperty = order.Document.getProperty("orderPaid");
						if (orderPaidProperty != null && orderPaidProperty.Value != null)
							orderInfo.Paid = orderPaidProperty.Value == "1";

						// load data recursively from umbraco documents into order tree
						orderInfo.OrderLines = orderDoc.Children.Select(d =>
							{
								var fields = d.GenericProperties.Where(x => !OrderedProduct.DefaultProperties.Contains(x.PropertyType.Alias)).ToDictionary(s => s.PropertyType.Alias, s => d.GetProperty<string>(s.PropertyType.Alias));

								var xDoc = new XDocument(new XElement("Fields"));

								OrderUpdatingService.AddFieldsToXDocumentBasedOnCMSDocumentType(xDoc, fields, d.ContentType.Alias);

								var orderedProduct = new OrderedProduct(d.Id);

								var productInfo = new ProductInfo(orderedProduct, orderInfo);
								productInfo.ProductVariants = d.Children.Select(cd => new ProductVariantInfo(new OrderedProductVariant(cd.Id), productInfo, productInfo.Vat)).ToList();
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
					BasePage.Current.ClientTools.SyncTree(sender.Parent.Path, false);
					BasePage.Current.ClientTools.ChangeContentFrameUrl(string.Concat("editContent.aspx?id=", sender.Id));
					//}
					//orderDoc.delete();
					BasePage.Current.ClientTools.ShowSpeechBubble(BasePage.speechBubbleIcon.success, "Order Updated!", "This order has been updated!");
				}
			}
		}

		protected void BaseContentTree_BeforeNodeRender(ref XmlTree sender, ref XmlTreeNode node, EventArgs e)
		{
		}
		
		// UNPUBLISH & REMOVE Incomplete Orders AFTER EXPIREDATE HAS BEEN PASSED
		protected void DocumentBeforePublish(Document sender, PublishEventArgs e)
		{
			if (Category.IsAlias(sender.ContentType.Alias))
			{
				SetAliasedPropertiesIfEnabled(sender, "categoryUrl");
			}
			if (Product.IsAlias(sender.ContentType.Alias))
			{
				SetAliasedPropertiesIfEnabled(sender, "productUrl");
			}
			
			if (sender.ContentType.Alias == Order.NodeAlias)
			{
				// order => todo: delete node, update SQL
			}
		}

		private static void SetAliasedPropertiesIfEnabled(Document sender, string propertyName)
		{
			var property = sender.getProperty(propertyName);
			if (property == null) return;
			property.Value = url.FormatUrl(property.Value.ToString());

			foreach (var shopAlias in StoreHelper.GetAllStores())
			{
				// todo: check this
				var aliasedEnabled = sender.getProperty("enable_" + shopAlias.Alias.ToUpper());
				if (aliasedEnabled == null || aliasedEnabled.Value.ToString() != "1") continue;

				var aliasedproperty = sender.getProperty(propertyName + "_" + shopAlias.Alias.ToUpper());

				// test == test --> overerf van global
				// test == "" --> overef van global
				if (aliasedproperty != null && aliasedproperty.Value == property.Value || aliasedproperty != null && string.IsNullOrEmpty(aliasedproperty.Value.ToString()))
				{
					aliasedproperty.Value = url.FormatUrl(property.Value.ToString());
				}
					// test == bla --> niets doen
				else if (aliasedproperty != null && !string.IsNullOrEmpty(aliasedproperty.Value.ToString()))
				{
					aliasedproperty.Value = url.FormatUrl(aliasedproperty.Value.ToString());
				}
			}
		}

		private static void ContentService_Created(IContentService sender, global::Umbraco.Core.Events.NewEventArgs<IContent> e)
		{
			if (e.Entity.Id != 0)
			{
				if (e.Entity.ContentType.Alias.StartsWith(Store.NodeAlias))
				{
					var reg = new Regex(@"\s*");
					var storeAlias = reg.Replace(e.Entity.Name, "");

					Umbraco.Helpers.InstallStore(storeAlias, new Document(e.Entity.Id));

					e.Entity.Name = storeAlias;
					sender.Save(e.Entity);
				}
			}
		}

		private static void ContentService_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e)
		{
			var contents = e.PublishedEntities.Where(c => c.ContentType.Alias.StartsWith(Order.NodeAlias));
			sender.UnPublish(contents, 0);
		}
	}
}