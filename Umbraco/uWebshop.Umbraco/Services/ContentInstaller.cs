using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using uWebshop.Common;
using uWebshop.Domain;

namespace uWebshop.Umbraco.Services
{
	public class ContentInstaller
	{
		/// <summary>
		/// creates the uWebshop tree content nodes and publishes them.
		/// </summary>
		internal static void InstallContent()
		{
			var contentService = ApplicationContext.Current.Services.ContentService;
			var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
			var contentList = new List<IContent>();

			var uWebshopcontentType = contentTypeService.GetContentType("uWebshop");

			if (uWebshopcontentType != null && contentService.GetContentOfContentType(uWebshopcontentType.Id).Any(x => !x.Trashed))
			{
				return;
			}

			var uWebshop = GetOrCreateContent("uWebshop", "uWebshop", contentTypeService, contentService, null, contentList);

			var storeRepository = GetOrCreateContent(Store.StoreRepositoryNodeAlias, "Stores", contentTypeService, contentService, uWebshop, contentList);

			var catalog = GetOrCreateContent(Catalog.NodeAlias, "Catalog", contentTypeService, contentService, uWebshop, contentList);

			var categoryRepo = GetOrCreateContent(Catalog.CategoryRepositoryNodeAlias, "Categories", contentTypeService, contentService, catalog, contentList);

			var productsRepo = GetOrCreateContent(Catalog.ProductRepositoryNodeAlias, "Products", contentTypeService, contentService, catalog, contentList);

			var orders = GetOrCreateContent(Order.OrderRepositoryNodeAlias, "Orders", contentTypeService, contentService, uWebshop, contentList);
			
			var discountRep = GetOrCreateContent(DiscountOrder.RepositoryNodeAlias, "Discounts", contentTypeService, contentService, uWebshop, contentList);

			var discountProductSection = GetOrCreateContent(DiscountProduct.SectionNodeAlias, "Product Discounts", contentTypeService, contentService, discountRep, contentList);

			var discountOrderSection = GetOrCreateContent(DiscountOrder.SectionNodeAlias, "Order Discounts", contentTypeService, contentService, discountRep, contentList);

			var paymentProvidersRep = GetOrCreateContent(PaymentProvider.PaymentProviderRepositoryNodeAlias, "Payment Providers", contentTypeService, contentService, uWebshop, contentList);

			var paymentProviders = GetOrCreateContent(PaymentProvider.PaymentProviderSectionNodeAlias, "Payment Providers", contentTypeService, contentService, paymentProvidersRep, contentList);

			var paymentProviderZones = GetOrCreateContent(PaymentProvider.PaymentProviderZoneSectionNodeAlias, "Payment Provider Zones", contentTypeService, contentService, paymentProvidersRep, contentList);

			var shippingProvidersRep = GetOrCreateContent(ShippingProvider.ShippingProviderRepositoryNodeAlias, "Shipping Providers", contentTypeService, contentService, uWebshop, contentList);

			var shippingProviders = GetOrCreateContent(ShippingProvider.ShippingProviderSectionNodeAlias, "Shipping Providers", contentTypeService, contentService, shippingProvidersRep, contentList);

			var shippingProviderZones = GetOrCreateContent(ShippingProvider.ShippingProviderZoneSectionNodeAlias, "Shipping Provider Zones", contentTypeService, contentService, shippingProvidersRep, contentList);

			var emailRep = GetOrCreateContent(Email.EmailRepositoryNodeAlias, "Email", contentTypeService, contentService, uWebshop, contentList);

			var emailsStore = GetOrCreateContent(Email.EmailTemplateStoreSectionNodeAlias, "Emails to store", contentTypeService, contentService, emailRep, contentList);

			var emailsCustomer = GetOrCreateContent(Email.EmailTemplateCustomerSectionNodeAlias, "Emails to customer", contentTypeService, contentService, emailRep, contentList);

			var settings = GetOrCreateContent(Settings.NodeAlias, "Settings", contentTypeService, contentService, uWebshop, contentList);
			if (settings.Id == 0)
			{
				if (settings.HasProperty("incompleteOrderLifetime"))
				{
					settings.SetValue("incompleteOrderLifetime", 360.ToString());
				}
			}

			contentService.Save(contentList);
			contentService.PublishWithChildrenWithStatus(uWebshop, includeUnpublished:true);

			// hack republish all to make sure examine is properly filled
			// todo: maybe not with updates?
			contentService.RePublishAll(0); // werkt nog niet naar behoren

			//HttpContext.Current.Server.ScriptTimeout = 100000;
			//umbraco.cms.businesslogic.web.Document.RePublishAll();
			//library.RefreshContent();
			//umbraco.cms.businesslogic.web.Document.RegeneratePreviews();
		}

		internal static IContent GetOrCreateContent(string alias, string contentName, IContentTypeService contentTypeService, IContentService contentService, IContent parentContent, List<IContent> contentList)
		{
			var contentType = contentTypeService.GetContentType(alias);

			return GetOrCreateContent(contentType, contentName, contentTypeService, contentService, parentContent, contentList);
		}

		internal static IContent GetOrCreateContent(IContentType contentType, string contentName, IContentTypeService contentTypeService, IContentService contentService, IContent parentContent, List<IContent> contentList)
		{   
			var content = contentService.GetContentOfContentType(contentType.Id).FirstOrDefault(x => !x.Trashed);
			if (content == null)
			{
				if (parentContent == null)
				{
					content = contentService.CreateContent(contentName, -1, contentType.Alias);
				}
				else
				{
					content = contentService.CreateContent(contentName, parentContent, contentType.Alias);
				}
				contentList.Add(content);
			}
			return content;
		}

		private static string MakeOrderSectionValue(OrderStatus orderStatus)
		{
			//<orderSection><![CDATA[<values>
			//  <orderStatus>OfflinePayment</orderStatus>
			//  <orderTimeInDays>1</orderTimeInDays>
			//</values>]]></orderSection>
			return string.Format(@"
<values>
  <orderStatus>{0}</orderStatus>
  <orderTimeInDays>1</orderTimeInDays>
</values>", orderStatus);
		}
	}
}