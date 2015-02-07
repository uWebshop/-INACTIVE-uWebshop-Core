using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.Web.Routing;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using umbraco;
using umbraco.cms.businesslogic.web;
using uWebshop.Umbraco.Businesslogic;
using DomainHelper = uWebshop.Domain.Helpers.DomainHelper;

namespace uWebshop.Umbraco.Mvc
{
	public class Application : IApplicationEventHandler
	{

		public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			if (InternalHelpers.MvcRenderMode)
			{
				ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByPageIdQuery, CatalogContentFinder>();
			}
		}

		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}
	}

	public class UwebshopMvcApplication : IApplicationEventHandler
	{
		public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{

		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			if (InternalHelpers.MvcRenderMode)
			{
				PublishedContentRequest.Prepared += PublishedContentRequest_Prepared;

			}
		}

		private static void PublishedContentRequest_Prepared(object sender, EventArgs e)
		{
			var request = sender as PublishedContentRequest;
			if (request == null) return;

			var currentContent = request.PublishedContent;

			if (currentContent == null)
			{
				return;
			}

			if (ProductVariant.IsAlias(currentContent.DocumentTypeAlias) && currentContent.Parent != null)
			{
				var product = DomainHelper.GetProductById(currentContent.Parent.Id);
				if (product != null)
				{
					UwebshopRequest.Current.Product = (IProduct)product;
				}
			}
			else if (uWebshop.Domain.Product.IsAlias(currentContent.DocumentTypeAlias))
			{
				var product = DomainHelper.GetProductById(currentContent.Id);
				if (product != null)
				{
					UwebshopRequest.Current.Product = (IProduct)product;
				}
			}
			else if (Category.IsAlias(currentContent.DocumentTypeAlias))
			{
				var category = DomainHelper.GetCategoryById(currentContent.Id);
				if (category != null)
				{
					UwebshopRequest.Current.Category = (ICategory)category;
				}
			}
		}

		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{

		}
	}

	public class CatalogContentFinder : IContentFinder
	{
		public bool TryFindContent(PublishedContentRequest contentRequest)
		{
			var stores = StoreHelper.GetAllStores();

			if (!stores.Any())
			{
				return false;
			}

			var uwebshopRequest = UwebshopRequest.Current;
			var content = uwebshopRequest.Product ?? uwebshopRequest.Category ?? uwebshopRequest.PaymentProvider ?? // in case ResolveUwebshopEntityUrl was already called from the module
						  IO.Container.Resolve<IUrlRewritingService>().ResolveUwebshopEntityUrl().Entity;

			if (content is PaymentProvider)
			{
				var paymentProvider = content as PaymentProvider;

				Log.Instance.LogDebug("UmbracoDefaultAfterRequestInit paymentProvider: " + paymentProvider.Name);

				new PaymentRequestHandler().HandleuWebshopPaymentRequest(paymentProvider);

				var publishedContent = contentRequest.RoutingContext.UmbracoContext.ContentCache.GetById(paymentProvider.Id);
				if (publishedContent == null) return false;
				contentRequest.PublishedContent = publishedContent;

				SetRequestCulture(contentRequest);
				return true;
			}

			if (content is Category)
			{
				var categoryFromUrl = content as Category;

				if (categoryFromUrl.Disabled) return false;

				if (Access.HasAccess(categoryFromUrl.Id, categoryFromUrl.Path, Membership.GetUser()))
				{
					var doc = contentRequest.RoutingContext.UmbracoContext.ContentCache.GetById(content.Id);
					if (doc != null)
					{
						contentRequest.PublishedContent = doc;
						var altTemplate = HttpContext.Current.Request["altTemplate"];
						contentRequest.TrySetTemplate(altTemplate);

						SetRequestCulture(contentRequest);
						return true;
					}
				}
				else
				{
					if (HttpContext.Current.User.Identity.IsAuthenticated)
					{
						contentRequest.SetRedirect(library.NiceUrl(Access.GetErrorPage(categoryFromUrl.Path)));
					}
					contentRequest.SetRedirect(library.NiceUrl(Access.GetLoginPage(categoryFromUrl.Path)));
					return true;
				}
			}

			else if (content is Product)
			{
				var productFromUrl = content as Product;
				if (productFromUrl.Disabled) return false;

				if (Access.HasAccess(productFromUrl.Id, productFromUrl.Path, Membership.GetUser()))
				{
					var doc = contentRequest.RoutingContext.UmbracoContext.ContentCache.GetById(content.Id);
					if (doc != null)
					{
						contentRequest.PublishedContent = doc;
						var altTemplate = HttpContext.Current.Request["altTemplate"];
						contentRequest.TrySetTemplate(altTemplate);

						SetRequestCulture(contentRequest);
						return true;
					}
				}
				else
				{
					if (HttpContext.Current.User.Identity.IsAuthenticated)
					{
						contentRequest.SetRedirect(library.NiceUrl(Access.GetErrorPage(productFromUrl.Path)));
					}
					contentRequest.SetRedirect(library.NiceUrl(Access.GetLoginPage(productFromUrl.Path)));
					return true;
				}
			}
			return false;
		}

		private static void SetRequestCulture(PublishedContentRequest contentRequest)
		{
			var store = UwebshopRequest.Current.CurrentStore;
			var language = store.CultureInfo;

			contentRequest.Culture = language;
		}
	}
}