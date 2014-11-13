using System.Collections.Generic;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class DocumentTypeAliasList
	{
		/// <summary>
		/// List of all the default uWebshop document types
		/// </summary>
		/// <value>
		/// The list.
		/// </value>
		public static List<string> List
		{
			get { return new List<string> { "uWebshop", Settings.NodeAlias, Store.StoreRepositoryNodeAlias, Store.NodeAlias, Catalog.NodeAlias, Catalog.CategoryRepositoryNodeAlias, Category.NodeAlias, Catalog.ProductRepositoryNodeAlias, Product.NodeAlias, OrderedProduct.NodeAlias, OrderedProductVariant.NodeAlias, ProductVariant.NodeAlias, DiscountRepositoryContentType.NodeAlias, DiscountProduct.SectionNodeAlias, DiscountProduct.NodeAlias, DiscountOrder.SectionNodeAlias, DiscountOrder.NodeAlias, Order.OrderRepositoryNodeAlias, DateFolder.NodeAlias, Order.NodeAlias, OrderStoreFolder.NodeAlias, ShippingProvider.ShippingProviderRepositoryNodeAlias, ShippingProvider.ShippingProviderSectionNodeAlias, ShippingProvider.NodeAlias, ShippingProviderMethod.NodeAlias, ShippingProvider.ShippingProviderZoneSectionNodeAlias, Zone.ShippingZoneNodeAlias, PaymentProvider.NodeAlias, PaymentProviderMethod.NodeAlias, PaymentProvider.PaymentProviderRepositoryNodeAlias, PaymentProvider.PaymentProviderSectionNodeAlias, PaymentProvider.PaymentProviderZoneSectionNodeAlias, Zone.PaymentZoneNodeAlias, Email.EmailRepositoryNodeAlias, Email.EmailTemplateStoreSectionNodeAlias, Email.EmailTemplateCustomerSectionNodeAlias, Email.EmailTemplateStoreNodeAlias, Email.EmailTemplateCustomerNodeAlias }; }
		}
	}
}