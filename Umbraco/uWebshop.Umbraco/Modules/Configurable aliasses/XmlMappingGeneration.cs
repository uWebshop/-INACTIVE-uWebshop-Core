using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using uWebshop.Common.Interfaces;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.ContentTypes;

namespace uWebshop.Umbraco.Repositories
{

internal interface ICatalogAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class CatalogAliassesService : ICatalogAliassesService
	{
		public CatalogAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Catalog;
		}

		private CatalogAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsCatalog"; } }
}

[XmlRoot(ElementName = "Catalog")]
public struct CatalogAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface ICategoryAliassesService
{
	string ContentTypeAlias { get; }
	string categories { get; }
	string metaTags { get; }
	string images { get; }
	string title { get; }
	string url { get; }
	string metaDescription { get; }
	string description { get; }
}


	internal class CategoryAliassesService : ICategoryAliassesService
	{
		public CategoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Category;
		}

		private CategoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsCategory"; } }
public string categories { get { return _xml.categories ?? "categories"; } }
public string metaTags { get { return _xml.metaTags ?? "metaTags"; } }
public string images { get { return _xml.images ?? "images"; } }
public string title { get { return _xml.title ?? "title"; } }
public string url { get { return _xml.url ?? "url"; } }
public string metaDescription { get { return _xml.metaDescription ?? "metaDescription"; } }
public string description { get { return _xml.description ?? "description"; } }
}

[XmlRoot(ElementName = "Category")]
public struct CategoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string categories;
	public string metaTags;
	public string images;
	public string title;
	public string url;
	public string metaDescription;
	public string description;
}

internal interface ICategoryRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class CategoryRepositoryAliassesService : ICategoryRepositoryAliassesService
	{
		public CategoryRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().CategoryRepository;
		}

		private CategoryRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsCategoryRepository"; } }
}

[XmlRoot(ElementName = "CategoryRepository")]
public struct CategoryRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IDateFolderAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class DateFolderAliassesService : IDateFolderAliassesService
	{
		public DateFolderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DateFolder;
		}

		private DateFolderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderDateFolder"; } }
}

[XmlRoot(ElementName = "DateFolder")]
public struct DateFolderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IDiscountOrderAliassesService
{
	string ContentTypeAlias { get; }
	string items { get; }
	string affectedOrderlines { get; }
	string affectedTags { get; }
	string orderCondition { get; }
	string numberOfItemsCondition { get; }
	string shippingDiscountable { get; }
	string minimumAmount { get; }
	string couponCodes { get; }
	string oncePerCustomer { get; }
	string title { get; }
	string description { get; }
	string disable { get; }
	string discountType { get; }
	string discount { get; }
	string ranges { get; }
	string countdownEnabled { get; }
	string countdown { get; }
	string memberGroups { get; }
}


	internal class DiscountOrderAliassesService : IDiscountOrderAliassesService
	{
		public DiscountOrderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DiscountOrder;
		}

		private DiscountOrderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsDiscountOrder"; } }
public string items { get { return _xml.items ?? "items"; } }
public string affectedOrderlines { get { return _xml.affectedOrderlines ?? "affectedOrderlines"; } }
public string affectedTags { get { return _xml.affectedTags ?? "affectedTags"; } }
public string orderCondition { get { return _xml.orderCondition ?? "orderCondition"; } }
public string numberOfItemsCondition { get { return _xml.numberOfItemsCondition ?? "numberOfItemsCondition"; } }
public string shippingDiscountable { get { return _xml.shippingDiscountable ?? "shippingDiscountable"; } }
public string minimumAmount { get { return _xml.minimumAmount ?? "minimumAmount"; } }
public string couponCodes { get { return _xml.couponCodes ?? "couponCodes"; } }
public string oncePerCustomer { get { return _xml.oncePerCustomer ?? "oncePerCustomer"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
public string discountType { get { return _xml.discountType ?? "discountType"; } }
public string discount { get { return _xml.discount ?? "discount"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string countdownEnabled { get { return _xml.countdownEnabled ?? "countdownEnabled"; } }
public string countdown { get { return _xml.countdown ?? "countdown"; } }
public string memberGroups { get { return _xml.memberGroups ?? "memberGroups"; } }
}

[XmlRoot(ElementName = "DiscountOrder")]
public struct DiscountOrderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string items;
	public string affectedOrderlines;
	public string affectedTags;
	public string orderCondition;
	public string numberOfItemsCondition;
	public string shippingDiscountable;
	public string minimumAmount;
	public string couponCodes;
	public string oncePerCustomer;
	public string title;
	public string description;
	public string disable;
	public string discountType;
	public string discount;
	public string ranges;
	public string countdownEnabled;
	public string countdown;
	public string memberGroups;
}

internal interface IDiscountOrderSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class DiscountOrderSectionAliassesService : IDiscountOrderSectionAliassesService
	{
		public DiscountOrderSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DiscountOrderSection;
		}

		private DiscountOrderSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsDiscountOrderSection"; } }
}

[XmlRoot(ElementName = "DiscountOrderSection")]
public struct DiscountOrderSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IDiscountProductAliassesService
{
	string ContentTypeAlias { get; }
	string products { get; }
	string excludeVariants { get; }
	string title { get; }
	string description { get; }
	string disable { get; }
	string discountType { get; }
	string discount { get; }
	string ranges { get; }
	string countdownEnabled { get; }
	string countdown { get; }
	string memberGroups { get; }
}


	internal class DiscountProductAliassesService : IDiscountProductAliassesService
	{
		public DiscountProductAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DiscountProduct;
		}

		private DiscountProductAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsDiscountProduct"; } }
public string products { get { return _xml.products ?? "products"; } }
public string excludeVariants { get { return _xml.excludeVariants ?? "excludeVariants"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
public string discountType { get { return _xml.discountType ?? "discountType"; } }
public string discount { get { return _xml.discount ?? "discount"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string countdownEnabled { get { return _xml.countdownEnabled ?? "countdownEnabled"; } }
public string countdown { get { return _xml.countdown ?? "countdown"; } }
public string memberGroups { get { return _xml.memberGroups ?? "memberGroups"; } }
}

[XmlRoot(ElementName = "DiscountProduct")]
public struct DiscountProductAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string products;
	public string excludeVariants;
	public string title;
	public string description;
	public string disable;
	public string discountType;
	public string discount;
	public string ranges;
	public string countdownEnabled;
	public string countdown;
	public string memberGroups;
}

internal interface IDiscountProductSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class DiscountProductSectionAliassesService : IDiscountProductSectionAliassesService
	{
		public DiscountProductSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DiscountProductSection;
		}

		private DiscountProductSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsDiscountProductSection"; } }
}

[XmlRoot(ElementName = "DiscountProductSection")]
public struct DiscountProductSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IDiscountRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class DiscountRepositoryAliassesService : IDiscountRepositoryAliassesService
	{
		public DiscountRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().DiscountRepository;
		}

		private DiscountRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsDiscountRepository"; } }
}

[XmlRoot(ElementName = "DiscountRepository")]
public struct DiscountRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IEmailCustomerAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string description { get; }
	string emailtemplate { get; }
	string templatePreview { get; }
}


	internal class EmailCustomerAliassesService : IEmailCustomerAliassesService
	{
		public EmailCustomerAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().EmailCustomer;
		}

		private EmailCustomerAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsEmailTemplateCustomer"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string emailtemplate { get { return _xml.emailtemplate ?? "emailtemplate"; } }
public string templatePreview { get { return _xml.templatePreview ?? "templatePreview"; } }
}

[XmlRoot(ElementName = "EmailCustomer")]
public struct EmailCustomerAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string description;
	public string emailtemplate;
	public string templatePreview;
}

internal interface IEmailCustomerSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class EmailCustomerSectionAliassesService : IEmailCustomerSectionAliassesService
	{
		public EmailCustomerSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().EmailCustomerSection;
		}

		private EmailCustomerSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsEmailTemplateCustomerSection"; } }
}

[XmlRoot(ElementName = "EmailCustomerSection")]
public struct EmailCustomerSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IEmailRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class EmailRepositoryAliassesService : IEmailRepositoryAliassesService
	{
		public EmailRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().EmailRepository;
		}

		private EmailRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsEmailRepository"; } }
}

[XmlRoot(ElementName = "EmailRepository")]
public struct EmailRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IEmailStoreAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string description { get; }
	string emailtemplate { get; }
	string templatePreview { get; }
}


	internal class EmailStoreAliassesService : IEmailStoreAliassesService
	{
		public EmailStoreAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().EmailStore;
		}

		private EmailStoreAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsEmailTemplateStore"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string emailtemplate { get { return _xml.emailtemplate ?? "emailtemplate"; } }
public string templatePreview { get { return _xml.templatePreview ?? "templatePreview"; } }
}

[XmlRoot(ElementName = "EmailStore")]
public struct EmailStoreAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string description;
	public string emailtemplate;
	public string templatePreview;
}

internal interface IEmailStoreSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class EmailStoreSectionAliassesService : IEmailStoreSectionAliassesService
	{
		public EmailStoreSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().EmailStoreSection;
		}

		private EmailStoreSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsEmailTemplateStoreSection"; } }
}

[XmlRoot(ElementName = "EmailStoreSection")]
public struct EmailStoreSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IOrderAliassesService
{
	string ContentTypeAlias { get; }
	string orderStatusPicker { get; }
	string orderPaid { get; }
	string orderDetails { get; }
	string orderGuid { get; }
	string customerEmail { get; }
	string customerFirstName { get; }
	string customerLastName { get; }
}


	internal class OrderAliassesService : IOrderAliassesService
	{
		public OrderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Order;
		}

		private OrderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrder"; } }
public string orderStatusPicker { get { return _xml.orderStatusPicker ?? "orderStatusPicker"; } }
public string orderPaid { get { return _xml.orderPaid ?? "orderPaid"; } }
public string orderDetails { get { return _xml.orderDetails ?? "orderDetails"; } }
public string orderGuid { get { return _xml.orderGuid ?? "orderGuid"; } }
public string customerEmail { get { return _xml.customerEmail ?? "customerEmail"; } }
public string customerFirstName { get { return _xml.customerFirstName ?? "customerFirstName"; } }
public string customerLastName { get { return _xml.customerLastName ?? "customerLastName"; } }
}

[XmlRoot(ElementName = "Order")]
public struct OrderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string orderStatusPicker;
	public string orderPaid;
	public string orderDetails;
	public string orderGuid;
	public string customerEmail;
	public string customerFirstName;
	public string customerLastName;
}

internal interface IOrderedProductAliassesService
{
	string ContentTypeAlias { get; }
	string productId { get; }
	string typeAlias { get; }
	string title { get; }
	string sku { get; }
	string length { get; }
	string width { get; }
	string height { get; }
	string weight { get; }
	string ranges { get; }
	string price { get; }
	string vat { get; }
	string itemCount { get; }
	string orderedProductDiscountPercentage { get; }
	string orderedProductDiscountAmount { get; }
	string orderedProductDiscountExcludingVariants { get; }
}


	internal class OrderedProductAliassesService : IOrderedProductAliassesService
	{
		public OrderedProductAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().OrderedProduct;
		}

		private OrderedProductAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderedProduct"; } }
public string productId { get { return _xml.productId ?? "productId"; } }
public string typeAlias { get { return _xml.typeAlias ?? "typeAlias"; } }
public string title { get { return _xml.title ?? "title"; } }
public string sku { get { return _xml.sku ?? "sku"; } }
public string length { get { return _xml.length ?? "length"; } }
public string width { get { return _xml.width ?? "width"; } }
public string height { get { return _xml.height ?? "height"; } }
public string weight { get { return _xml.weight ?? "weight"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string price { get { return _xml.price ?? "price"; } }
public string vat { get { return _xml.vat ?? "vat"; } }
public string itemCount { get { return _xml.itemCount ?? "itemCount"; } }
public string orderedProductDiscountPercentage { get { return _xml.orderedProductDiscountPercentage ?? "orderedProductDiscountPercentage"; } }
public string orderedProductDiscountAmount { get { return _xml.orderedProductDiscountAmount ?? "orderedProductDiscountAmount"; } }
public string orderedProductDiscountExcludingVariants { get { return _xml.orderedProductDiscountExcludingVariants ?? "orderedProductDiscountExcludingVariants"; } }
}

[XmlRoot(ElementName = "OrderedProduct")]
public struct OrderedProductAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string productId;
	public string typeAlias;
	public string title;
	public string sku;
	public string length;
	public string width;
	public string height;
	public string weight;
	public string ranges;
	public string price;
	public string vat;
	public string itemCount;
	public string orderedProductDiscountPercentage;
	public string orderedProductDiscountAmount;
	public string orderedProductDiscountExcludingVariants;
}

internal interface IOrderedProductVariantAliassesService
{
	string ContentTypeAlias { get; }
	string variantId { get; }
	string typeAlias { get; }
	string title { get; }
	string sku { get; }
	string group { get; }
	string length { get; }
	string width { get; }
	string height { get; }
	string weight { get; }
	string price { get; }
	string ranges { get; }
	string discountPercentage { get; }
	string discountAmount { get; }
}


	internal class OrderedProductVariantAliassesService : IOrderedProductVariantAliassesService
	{
		public OrderedProductVariantAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().OrderedProductVariant;
		}

		private OrderedProductVariantAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderedProductVariant"; } }
public string variantId { get { return _xml.variantId ?? "variantId"; } }
public string typeAlias { get { return _xml.typeAlias ?? "typeAlias"; } }
public string title { get { return _xml.title ?? "title"; } }
public string sku { get { return _xml.sku ?? "sku"; } }
public string group { get { return _xml.group ?? "group"; } }
public string length { get { return _xml.length ?? "length"; } }
public string width { get { return _xml.width ?? "width"; } }
public string height { get { return _xml.height ?? "height"; } }
public string weight { get { return _xml.weight ?? "weight"; } }
public string price { get { return _xml.price ?? "price"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string discountPercentage { get { return _xml.discountPercentage ?? "discountPercentage"; } }
public string discountAmount { get { return _xml.discountAmount ?? "discountAmount"; } }
}

[XmlRoot(ElementName = "OrderedProductVariant")]
public struct OrderedProductVariantAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string variantId;
	public string typeAlias;
	public string title;
	public string sku;
	public string group;
	public string length;
	public string width;
	public string height;
	public string weight;
	public string price;
	public string ranges;
	public string discountPercentage;
	public string discountAmount;
}

internal interface IOrderRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class OrderRepositoryAliassesService : IOrderRepositoryAliassesService
	{
		public OrderRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().OrderRepository;
		}

		private OrderRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderRepository"; } }
}

[XmlRoot(ElementName = "OrderRepository")]
public struct OrderRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IOrderSectionAliassesService
{
	string ContentTypeAlias { get; }
	string orderSection { get; }
}


	internal class OrderSectionAliassesService : IOrderSectionAliassesService
	{
		public OrderSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().OrderSection;
		}

		private OrderSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderSection"; } }
public string orderSection { get { return _xml.orderSection ?? "orderSection"; } }
}

[XmlRoot(ElementName = "OrderSection")]
public struct OrderSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string orderSection;
}

internal interface IOrderStoreFolderAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class OrderStoreFolderAliassesService : IOrderStoreFolderAliassesService
	{
		public OrderStoreFolderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().OrderStoreFolder;
		}

		private OrderStoreFolderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsOrderStoreFolder"; } }
}

[XmlRoot(ElementName = "OrderStoreFolder")]
public struct OrderStoreFolderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IPaymentProviderAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string description { get; }
	string image { get; }
	string type { get; }
	string zone { get; }
	string successNode { get; }
	string errorNode { get; }
	string testMode { get; }
}


	internal class PaymentProviderAliassesService : IPaymentProviderAliassesService
	{
		public PaymentProviderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProvider;
		}

		private PaymentProviderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProvider"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string image { get { return _xml.image ?? "image"; } }
public string type { get { return _xml.type ?? "type"; } }
public string zone { get { return _xml.zone ?? "zone"; } }
public string successNode { get { return _xml.successNode ?? "successNode"; } }
public string errorNode { get { return _xml.errorNode ?? "errorNode"; } }
public string testMode { get { return _xml.testMode ?? "testMode"; } }
}

[XmlRoot(ElementName = "PaymentProvider")]
public struct PaymentProviderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string description;
	public string image;
	public string type;
	public string zone;
	public string successNode;
	public string errorNode;
	public string testMode;
}

internal interface IPaymentProviderMethodAliassesService
{
	string ContentTypeAlias { get; }
	string disable { get; }
	string title { get; }
	string description { get; }
	string image { get; }
	string price { get; }
	string vat { get; }
	string amountType { get; }
}


	internal class PaymentProviderMethodAliassesService : IPaymentProviderMethodAliassesService
	{
		public PaymentProviderMethodAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProviderMethod;
		}

		private PaymentProviderMethodAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProviderMethod"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string image { get { return _xml.image ?? "image"; } }
public string price { get { return _xml.price ?? "price"; } }
public string vat { get { return _xml.vat ?? "vat"; } }
public string amountType { get { return _xml.amountType ?? "amountType"; } }
}

[XmlRoot(ElementName = "PaymentProviderMethod")]
public struct PaymentProviderMethodAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string disable;
	public string title;
	public string description;
	public string image;
	public string price;
	public string vat;
	public string amountType;
}

internal interface IPaymentProviderRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class PaymentProviderRepositoryAliassesService : IPaymentProviderRepositoryAliassesService
	{
		public PaymentProviderRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProviderRepository;
		}

		private PaymentProviderRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProviderRepository"; } }
}

[XmlRoot(ElementName = "PaymentProviderRepository")]
public struct PaymentProviderRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IPaymentProviderSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class PaymentProviderSectionAliassesService : IPaymentProviderSectionAliassesService
	{
		public PaymentProviderSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProviderSection;
		}

		private PaymentProviderSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProviderSection"; } }
}

[XmlRoot(ElementName = "PaymentProviderSection")]
public struct PaymentProviderSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IPaymentProviderZoneAliassesService
{
	string ContentTypeAlias { get; }
	string zone { get; }
}


	internal class PaymentProviderZoneAliassesService : IPaymentProviderZoneAliassesService
	{
		public PaymentProviderZoneAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProviderZone;
		}

		private PaymentProviderZoneAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProviderZone"; } }
public string zone { get { return _xml.zone ?? "zone"; } }
}

[XmlRoot(ElementName = "PaymentProviderZone")]
public struct PaymentProviderZoneAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string zone;
}

internal interface IPaymentProviderZoneSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class PaymentProviderZoneSectionAliassesService : IPaymentProviderZoneSectionAliassesService
	{
		public PaymentProviderZoneSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().PaymentProviderZoneSection;
		}

		private PaymentProviderZoneSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsPaymentProviderZoneSection"; } }
}

[XmlRoot(ElementName = "PaymentProviderZoneSection")]
public struct PaymentProviderZoneSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IProductAliassesService
{
	string ContentTypeAlias { get; }
	string sku { get; }
	string metaTags { get; }
	string categories { get; }
	string images { get; }
	string files { get; }
	string length { get; }
	string width { get; }
	string height { get; }
	string weight { get; }
	string price { get; }
	string ranges { get; }
	string vat { get; }
	string stock { get; }
	string ordered { get; }
	string stockStatus { get; }
	string backorderStatus { get; }
	string useVariantStock { get; }
	string title { get; }
	string url { get; }
	string metaDescription { get; }
	string description { get; }
	string disable { get; }
}


	internal class ProductAliassesService : IProductAliassesService
	{
		public ProductAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Product;
		}

		private ProductAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsProduct"; } }
public string sku { get { return _xml.sku ?? "sku"; } }
public string metaTags { get { return _xml.metaTags ?? "metaTags"; } }
public string categories { get { return _xml.categories ?? "categories"; } }
public string images { get { return _xml.images ?? "images"; } }
public string files { get { return _xml.files ?? "files"; } }
public string length { get { return _xml.length ?? "length"; } }
public string width { get { return _xml.width ?? "width"; } }
public string height { get { return _xml.height ?? "height"; } }
public string weight { get { return _xml.weight ?? "weight"; } }
public string price { get { return _xml.price ?? "price"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string vat { get { return _xml.vat ?? "vat"; } }
public string stock { get { return _xml.stock ?? "stock"; } }
public string ordered { get { return _xml.ordered ?? "ordered"; } }
public string stockStatus { get { return _xml.stockStatus ?? "stockStatus"; } }
public string backorderStatus { get { return _xml.backorderStatus ?? "backorderStatus"; } }
public string useVariantStock { get { return _xml.useVariantStock ?? "useVariantStock"; } }
public string title { get { return _xml.title ?? "title"; } }
public string url { get { return _xml.url ?? "url"; } }
public string metaDescription { get { return _xml.metaDescription ?? "metaDescription"; } }
public string description { get { return _xml.description ?? "description"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
}

[XmlRoot(ElementName = "Product")]
public struct ProductAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string sku;
	public string metaTags;
	public string categories;
	public string images;
	public string files;
	public string length;
	public string width;
	public string height;
	public string weight;
	public string price;
	public string ranges;
	public string vat;
	public string stock;
	public string ordered;
	public string stockStatus;
	public string backorderStatus;
	public string useVariantStock;
	public string title;
	public string url;
	public string metaDescription;
	public string description;
	public string disable;
}

internal interface IProductRepositoryAliassesService
{
	string ContentTypeAlias { get; }
	string productOverview { get; }
}


	internal class ProductRepositoryAliassesService : IProductRepositoryAliassesService
	{
		public ProductRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ProductRepository;
		}

		private ProductRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsProductRepository"; } }
public string productOverview { get { return _xml.productOverview ?? "productOverview"; } }
}

[XmlRoot(ElementName = "ProductRepository")]
public struct ProductRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string productOverview;
}

internal interface IProductVariantAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string sku { get; }
	string disable { get; }
	string description { get; }
	string length { get; }
	string width { get; }
	string height { get; }
	string weight { get; }
	string price { get; }
	string ranges { get; }
	string stock { get; }
	string ordered { get; }
	string stockStatus { get; }
	string backorderStatus { get; }
}


	internal class ProductVariantAliassesService : IProductVariantAliassesService
	{
		public ProductVariantAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ProductVariant;
		}

		private ProductVariantAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsProductVariant"; } }
public string title { get { return _xml.title ?? "title"; } }
public string sku { get { return _xml.sku ?? "sku"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
public string description { get { return _xml.description ?? "description"; } }
public string length { get { return _xml.length ?? "length"; } }
public string width { get { return _xml.width ?? "width"; } }
public string height { get { return _xml.height ?? "height"; } }
public string weight { get { return _xml.weight ?? "weight"; } }
public string price { get { return _xml.price ?? "price"; } }
public string ranges { get { return _xml.ranges ?? "ranges"; } }
public string stock { get { return _xml.stock ?? "stock"; } }
public string ordered { get { return _xml.ordered ?? "ordered"; } }
public string stockStatus { get { return _xml.stockStatus ?? "stockStatus"; } }
public string backorderStatus { get { return _xml.backorderStatus ?? "backorderStatus"; } }
}

[XmlRoot(ElementName = "ProductVariant")]
public struct ProductVariantAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string sku;
	public string disable;
	public string description;
	public string length;
	public string width;
	public string height;
	public string weight;
	public string price;
	public string ranges;
	public string stock;
	public string ordered;
	public string stockStatus;
	public string backorderStatus;
}

internal interface IProductVariantGroupAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string required { get; }
	string description { get; }
}


	internal class ProductVariantGroupAliassesService : IProductVariantGroupAliassesService
	{
		public ProductVariantGroupAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ProductVariantGroup;
		}

		private ProductVariantGroupAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsProductVariantGroup"; } }
public string title { get { return _xml.title ?? "title"; } }
public string required { get { return _xml.required ?? "required"; } }
public string description { get { return _xml.description ?? "description"; } }
}

[XmlRoot(ElementName = "ProductVariantGroup")]
public struct ProductVariantGroupAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string required;
	public string description;
}

internal interface ISettingsAliassesService
{
	string ContentTypeAlias { get; }
	string includingVat { get; }
	string lowercaseUrls { get; }
	string incompleteOrderLifetime { get; }
}


	internal class SettingsAliassesService : ISettingsAliassesService
	{
		public SettingsAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Settings;
		}

		private SettingsAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsSettings"; } }
public string includingVat { get { return _xml.includingVat ?? "includingVat"; } }
public string lowercaseUrls { get { return _xml.lowercaseUrls ?? "lowercaseUrls"; } }
public string incompleteOrderLifetime { get { return _xml.incompleteOrderLifetime ?? "incompleteOrderLifetime"; } }
}

[XmlRoot(ElementName = "Settings")]
public struct SettingsAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string includingVat;
	public string lowercaseUrls;
	public string incompleteOrderLifetime;
}

internal interface IShippingProviderAliassesService
{
	string ContentTypeAlias { get; }
	string title { get; }
	string description { get; }
	string image { get; }
	string type { get; }
	string rangeType { get; }
	string rangeStart { get; }
	string rangeEnd { get; }
	string overrule { get; }
	string zone { get; }
	string testMode { get; }
}


	internal class ShippingProviderAliassesService : IShippingProviderAliassesService
	{
		public ShippingProviderAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProvider;
		}

		private ShippingProviderAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProvider"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string image { get { return _xml.image ?? "image"; } }
public string type { get { return _xml.type ?? "type"; } }
public string rangeType { get { return _xml.rangeType ?? "rangeType"; } }
public string rangeStart { get { return _xml.rangeStart ?? "rangeStart"; } }
public string rangeEnd { get { return _xml.rangeEnd ?? "rangeEnd"; } }
public string overrule { get { return _xml.overrule ?? "overrule"; } }
public string zone { get { return _xml.zone ?? "zone"; } }
public string testMode { get { return _xml.testMode ?? "testMode"; } }
}

[XmlRoot(ElementName = "ShippingProvider")]
public struct ShippingProviderAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string title;
	public string description;
	public string image;
	public string type;
	public string rangeType;
	public string rangeStart;
	public string rangeEnd;
	public string overrule;
	public string zone;
	public string testMode;
}

internal interface IShippingProviderMethodAliassesService
{
	string ContentTypeAlias { get; }
	string disable { get; }
	string title { get; }
	string description { get; }
	string image { get; }
	string price { get; }
	string vat { get; }
}


	internal class ShippingProviderMethodAliassesService : IShippingProviderMethodAliassesService
	{
		public ShippingProviderMethodAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProviderMethod;
		}

		private ShippingProviderMethodAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProviderMethod"; } }
public string disable { get { return _xml.disable ?? "disable"; } }
public string title { get { return _xml.title ?? "title"; } }
public string description { get { return _xml.description ?? "description"; } }
public string image { get { return _xml.image ?? "image"; } }
public string price { get { return _xml.price ?? "price"; } }
public string vat { get { return _xml.vat ?? "vat"; } }
}

[XmlRoot(ElementName = "ShippingProviderMethod")]
public struct ShippingProviderMethodAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string disable;
	public string title;
	public string description;
	public string image;
	public string price;
	public string vat;
}

internal interface IShippingProviderRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class ShippingProviderRepositoryAliassesService : IShippingProviderRepositoryAliassesService
	{
		public ShippingProviderRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProviderRepository;
		}

		private ShippingProviderRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProviderRepository"; } }
}

[XmlRoot(ElementName = "ShippingProviderRepository")]
public struct ShippingProviderRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IShippingProviderSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class ShippingProviderSectionAliassesService : IShippingProviderSectionAliassesService
	{
		public ShippingProviderSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProviderSection;
		}

		private ShippingProviderSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProviderSection"; } }
}

[XmlRoot(ElementName = "ShippingProviderSection")]
public struct ShippingProviderSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IShippingProviderZoneAliassesService
{
	string ContentTypeAlias { get; }
	string zone { get; }
}


	internal class ShippingProviderZoneAliassesService : IShippingProviderZoneAliassesService
	{
		public ShippingProviderZoneAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProviderZone;
		}

		private ShippingProviderZoneAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProviderZone"; } }
public string zone { get { return _xml.zone ?? "zone"; } }
}

[XmlRoot(ElementName = "ShippingProviderZone")]
public struct ShippingProviderZoneAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string zone;
}

internal interface IShippingProviderZoneSectionAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class ShippingProviderZoneSectionAliassesService : IShippingProviderZoneSectionAliassesService
	{
		public ShippingProviderZoneSectionAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().ShippingProviderZoneSection;
		}

		private ShippingProviderZoneSectionAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsShippingProviderZoneSection"; } }
}

[XmlRoot(ElementName = "ShippingProviderZoneSection")]
public struct ShippingProviderZoneSectionAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IStoreAliassesService
{
	string ContentTypeAlias { get; }
	string storeCulture { get; }
	string countryCode { get; }
	string currencies { get; }
	string globalVat { get; }
	string orderNumberPrefix { get; }
	string orderNumberTemplate { get; }
	string orderNumberStartNumber { get; }
	string enableStock { get; }
	string defaultUseVariantStock { get; }
	string defaultCountdownEnabled { get; }
	string storeStock { get; }
	string useBackorders { get; }
	string enableTestmode { get; }
	string storeEmailFrom { get; }
	string storeEmailFromName { get; }
	string storeEmailTo { get; }
	string accountEmailCreated { get; }
	string accountForgotPassword { get; }
	string confirmationEmailStore { get; }
	string confirmationEmailCustomer { get; }
	string onlinePaymentEmailStore { get; }
	string onlinePaymentEmailCustomer { get; }
	string offlinePaymentEmailStore { get; }
	string offlinePaymentEmailCustomer { get; }
	string paymentFailedEmailStore { get; }
	string paymentFailedEmailCustomer { get; }
	string dispatchedEmailStore { get; }
	string dispatchedEmailCustomer { get; }
	string cancelEmailStore { get; }
	string cancelEmailCustomer { get; }
	string closedEmailStore { get; }
	string closedEmailCustomer { get; }
	string pendingEmailStore { get; }
	string pendingEmailCustomer { get; }
	string temporaryOutOfStockEmailStore { get; }
	string temporaryOutOfStockEmailCustomer { get; }
	string undeliverableEmailStore { get; }
	string undeliverableEmailCustomer { get; }
	string returnEmailStore { get; }
	string returnEmailCustomer { get; }
}


	internal class StoreAliassesService : IStoreAliassesService
	{
		public StoreAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().Store;
		}

		private StoreAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsStore"; } }
public string storeCulture { get { return _xml.storeCulture ?? "storeCulture"; } }
public string countryCode { get { return _xml.countryCode ?? "countryCode"; } }
public string currencies { get { return _xml.currencies ?? "currencies"; } }
public string globalVat { get { return _xml.globalVat ?? "globalVat"; } }
public string orderNumberPrefix { get { return _xml.orderNumberPrefix ?? "orderNumberPrefix"; } }
public string orderNumberTemplate { get { return _xml.orderNumberTemplate ?? "orderNumberTemplate"; } }
public string orderNumberStartNumber { get { return _xml.orderNumberStartNumber ?? "orderNumberStartNumber"; } }
public string enableStock { get { return _xml.enableStock ?? "enableStock"; } }
public string defaultUseVariantStock { get { return _xml.defaultUseVariantStock ?? "defaultUseVariantStock"; } }
public string defaultCountdownEnabled { get { return _xml.defaultCountdownEnabled ?? "defaultCountdownEnabled"; } }
public string storeStock { get { return _xml.storeStock ?? "storeStock"; } }
public string useBackorders { get { return _xml.useBackorders ?? "useBackorders"; } }
public string enableTestmode { get { return _xml.enableTestmode ?? "enableTestmode"; } }
public string storeEmailFrom { get { return _xml.storeEmailFrom ?? "storeEmailFrom"; } }
public string storeEmailFromName { get { return _xml.storeEmailFromName ?? "storeEmailFromName"; } }
public string storeEmailTo { get { return _xml.storeEmailTo ?? "storeEmailTo"; } }
public string accountEmailCreated { get { return _xml.accountEmailCreated ?? "accountEmailCreated"; } }
public string accountForgotPassword { get { return _xml.accountForgotPassword ?? "accountForgotPassword"; } }
public string confirmationEmailStore { get { return _xml.confirmationEmailStore ?? "confirmationEmailStore"; } }
public string confirmationEmailCustomer { get { return _xml.confirmationEmailCustomer ?? "confirmationEmailCustomer"; } }
public string onlinePaymentEmailStore { get { return _xml.onlinePaymentEmailStore ?? "onlinePaymentEmailStore"; } }
public string onlinePaymentEmailCustomer { get { return _xml.onlinePaymentEmailCustomer ?? "onlinePaymentEmailCustomer"; } }
public string offlinePaymentEmailStore { get { return _xml.offlinePaymentEmailStore ?? "offlinePaymentEmailStore"; } }
public string offlinePaymentEmailCustomer { get { return _xml.offlinePaymentEmailCustomer ?? "offlinePaymentEmailCustomer"; } }
public string paymentFailedEmailStore { get { return _xml.paymentFailedEmailStore ?? "paymentFailedEmailStore"; } }
public string paymentFailedEmailCustomer { get { return _xml.paymentFailedEmailCustomer ?? "paymentFailedEmailCustomer"; } }
public string dispatchedEmailStore { get { return _xml.dispatchedEmailStore ?? "dispatchedEmailStore"; } }
public string dispatchedEmailCustomer { get { return _xml.dispatchedEmailCustomer ?? "dispatchedEmailCustomer"; } }
public string cancelEmailStore { get { return _xml.cancelEmailStore ?? "cancelEmailStore"; } }
public string cancelEmailCustomer { get { return _xml.cancelEmailCustomer ?? "cancelEmailCustomer"; } }
public string closedEmailStore { get { return _xml.closedEmailStore ?? "closedEmailStore"; } }
public string closedEmailCustomer { get { return _xml.closedEmailCustomer ?? "closedEmailCustomer"; } }
public string pendingEmailStore { get { return _xml.pendingEmailStore ?? "pendingEmailStore"; } }
public string pendingEmailCustomer { get { return _xml.pendingEmailCustomer ?? "pendingEmailCustomer"; } }
public string temporaryOutOfStockEmailStore { get { return _xml.temporaryOutOfStockEmailStore ?? "temporaryOutOfStockEmailStore"; } }
public string temporaryOutOfStockEmailCustomer { get { return _xml.temporaryOutOfStockEmailCustomer ?? "temporaryOutOfStockEmailCustomer"; } }
public string undeliverableEmailStore { get { return _xml.undeliverableEmailStore ?? "undeliverableEmailStore"; } }
public string undeliverableEmailCustomer { get { return _xml.undeliverableEmailCustomer ?? "undeliverableEmailCustomer"; } }
public string returnEmailStore { get { return _xml.returnEmailStore ?? "returnEmailStore"; } }
public string returnEmailCustomer { get { return _xml.returnEmailCustomer ?? "returnEmailCustomer"; } }
}

[XmlRoot(ElementName = "Store")]
public struct StoreAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
	public string storeCulture;
	public string countryCode;
	public string currencies;
	public string globalVat;
	public string orderNumberPrefix;
	public string orderNumberTemplate;
	public string orderNumberStartNumber;
	public string enableStock;
	public string defaultUseVariantStock;
	public string defaultCountdownEnabled;
	public string storeStock;
	public string useBackorders;
	public string enableTestmode;
	public string storeEmailFrom;
	public string storeEmailFromName;
	public string storeEmailTo;
	public string accountEmailCreated;
	public string accountForgotPassword;
	public string confirmationEmailStore;
	public string confirmationEmailCustomer;
	public string onlinePaymentEmailStore;
	public string onlinePaymentEmailCustomer;
	public string offlinePaymentEmailStore;
	public string offlinePaymentEmailCustomer;
	public string paymentFailedEmailStore;
	public string paymentFailedEmailCustomer;
	public string dispatchedEmailStore;
	public string dispatchedEmailCustomer;
	public string cancelEmailStore;
	public string cancelEmailCustomer;
	public string closedEmailStore;
	public string closedEmailCustomer;
	public string pendingEmailStore;
	public string pendingEmailCustomer;
	public string temporaryOutOfStockEmailStore;
	public string temporaryOutOfStockEmailCustomer;
	public string undeliverableEmailStore;
	public string undeliverableEmailCustomer;
	public string returnEmailStore;
	public string returnEmailCustomer;
}

internal interface IStoreRepositoryAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class StoreRepositoryAliassesService : IStoreRepositoryAliassesService
	{
		public StoreRepositoryAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().StoreRepository;
		}

		private StoreRepositoryAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uwbsStoreRepository"; } }
}

[XmlRoot(ElementName = "StoreRepository")]
public struct StoreRepositoryAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

internal interface IUwebshopRootAliassesService
{
	string ContentTypeAlias { get; }
}


	internal class UwebshopRootAliassesService : IUwebshopRootAliassesService
	{
		public UwebshopRootAliassesService(IContentTypeAliassesXmlService contentTypeAliassesXmlService)
		{
			_xml = contentTypeAliassesXmlService.Get().UwebshopRoot;
		}

		private UwebshopRootAliassesXML _xml;

		public string ContentTypeAlias { get { return _xml.ContentTypeAlias ?? "uWebshop"; } }
}

[XmlRoot(ElementName = "UwebshopRoot")]
public struct UwebshopRootAliassesXML {
[XmlAttribute("alias")]
public string ContentTypeAlias;
}

[XmlRoot(ElementName = "Config")]
public struct UwebshopAliassesXMLConfig
{
public CatalogAliassesXML Catalog;
public CategoryAliassesXML Category;
public CategoryRepositoryAliassesXML CategoryRepository;
public DateFolderAliassesXML DateFolder;
public DiscountOrderAliassesXML DiscountOrder;
public DiscountOrderSectionAliassesXML DiscountOrderSection;
public DiscountProductAliassesXML DiscountProduct;
public DiscountProductSectionAliassesXML DiscountProductSection;
public DiscountRepositoryAliassesXML DiscountRepository;
public EmailCustomerAliassesXML EmailCustomer;
public EmailCustomerSectionAliassesXML EmailCustomerSection;
public EmailRepositoryAliassesXML EmailRepository;
public EmailStoreAliassesXML EmailStore;
public EmailStoreSectionAliassesXML EmailStoreSection;
public OrderAliassesXML Order;
public OrderedProductAliassesXML OrderedProduct;
public OrderedProductVariantAliassesXML OrderedProductVariant;
public OrderRepositoryAliassesXML OrderRepository;
public OrderSectionAliassesXML OrderSection;
public OrderStoreFolderAliassesXML OrderStoreFolder;
public PaymentProviderAliassesXML PaymentProvider;
public PaymentProviderMethodAliassesXML PaymentProviderMethod;
public PaymentProviderRepositoryAliassesXML PaymentProviderRepository;
public PaymentProviderSectionAliassesXML PaymentProviderSection;
public PaymentProviderZoneAliassesXML PaymentProviderZone;
public PaymentProviderZoneSectionAliassesXML PaymentProviderZoneSection;
public ProductAliassesXML Product;
public ProductRepositoryAliassesXML ProductRepository;
public ProductVariantAliassesXML ProductVariant;
public ProductVariantGroupAliassesXML ProductVariantGroup;
public SettingsAliassesXML Settings;
public ShippingProviderAliassesXML ShippingProvider;
public ShippingProviderMethodAliassesXML ShippingProviderMethod;
public ShippingProviderRepositoryAliassesXML ShippingProviderRepository;
public ShippingProviderSectionAliassesXML ShippingProviderSection;
public ShippingProviderZoneAliassesXML ShippingProviderZone;
public ShippingProviderZoneSectionAliassesXML ShippingProviderZoneSection;
public StoreAliassesXML Store;
public StoreRepositoryAliassesXML StoreRepository;
public UwebshopRootAliassesXML UwebshopRoot;
}
public static class ModuleFunctionality
{
public static void Register(IIocContainerConfiguration container)
{
container.RegisterType<ICatalogAliassesService, CatalogAliassesService>();
container.RegisterType<ICategoryAliassesService, CategoryAliassesService>();
container.RegisterType<ICategoryRepositoryAliassesService, CategoryRepositoryAliassesService>();
container.RegisterType<IDateFolderAliassesService, DateFolderAliassesService>();
container.RegisterType<IDiscountOrderAliassesService, DiscountOrderAliassesService>();
container.RegisterType<IDiscountOrderSectionAliassesService, DiscountOrderSectionAliassesService>();
container.RegisterType<IDiscountProductAliassesService, DiscountProductAliassesService>();
container.RegisterType<IDiscountProductSectionAliassesService, DiscountProductSectionAliassesService>();
container.RegisterType<IDiscountRepositoryAliassesService, DiscountRepositoryAliassesService>();
container.RegisterType<IEmailCustomerAliassesService, EmailCustomerAliassesService>();
container.RegisterType<IEmailCustomerSectionAliassesService, EmailCustomerSectionAliassesService>();
container.RegisterType<IEmailRepositoryAliassesService, EmailRepositoryAliassesService>();
container.RegisterType<IEmailStoreAliassesService, EmailStoreAliassesService>();
container.RegisterType<IEmailStoreSectionAliassesService, EmailStoreSectionAliassesService>();
container.RegisterType<IOrderAliassesService, OrderAliassesService>();
container.RegisterType<IOrderedProductAliassesService, OrderedProductAliassesService>();
container.RegisterType<IOrderedProductVariantAliassesService, OrderedProductVariantAliassesService>();
container.RegisterType<IOrderRepositoryAliassesService, OrderRepositoryAliassesService>();
container.RegisterType<IOrderSectionAliassesService, OrderSectionAliassesService>();
container.RegisterType<IOrderStoreFolderAliassesService, OrderStoreFolderAliassesService>();
container.RegisterType<IPaymentProviderAliassesService, PaymentProviderAliassesService>();
container.RegisterType<IPaymentProviderMethodAliassesService, PaymentProviderMethodAliassesService>();
container.RegisterType<IPaymentProviderRepositoryAliassesService, PaymentProviderRepositoryAliassesService>();
container.RegisterType<IPaymentProviderSectionAliassesService, PaymentProviderSectionAliassesService>();
container.RegisterType<IPaymentProviderZoneAliassesService, PaymentProviderZoneAliassesService>();
container.RegisterType<IPaymentProviderZoneSectionAliassesService, PaymentProviderZoneSectionAliassesService>();
container.RegisterType<IProductAliassesService, ProductAliassesService>();
container.RegisterType<IProductRepositoryAliassesService, ProductRepositoryAliassesService>();
container.RegisterType<IProductVariantAliassesService, ProductVariantAliassesService>();
container.RegisterType<IProductVariantGroupAliassesService, ProductVariantGroupAliassesService>();
container.RegisterType<ISettingsAliassesService, SettingsAliassesService>();
container.RegisterType<IShippingProviderAliassesService, ShippingProviderAliassesService>();
container.RegisterType<IShippingProviderMethodAliassesService, ShippingProviderMethodAliassesService>();
container.RegisterType<IShippingProviderRepositoryAliassesService, ShippingProviderRepositoryAliassesService>();
container.RegisterType<IShippingProviderSectionAliassesService, ShippingProviderSectionAliassesService>();
container.RegisterType<IShippingProviderZoneAliassesService, ShippingProviderZoneAliassesService>();
container.RegisterType<IShippingProviderZoneSectionAliassesService, ShippingProviderZoneSectionAliassesService>();
container.RegisterType<IStoreAliassesService, StoreAliassesService>();
container.RegisterType<IStoreRepositoryAliassesService, StoreRepositoryAliassesService>();
container.RegisterType<IUwebshopRootAliassesService, UwebshopRootAliassesService>();
}

}public static class InitNodeAliasses{

public static void Initialize(UwebshopAliassesXMLConfig aliasses)
{
Catalog.NodeAlias = aliasses.Catalog.ContentTypeAlias ?? "uwbsCatalog";
Category.NodeAlias = aliasses.Category.ContentTypeAlias ?? "uwbsCategory";
CategoryRepositoryContentType.NodeAlias = aliasses.CategoryRepository.ContentTypeAlias ?? "uwbsCategoryRepository";
DateFolder.NodeAlias = aliasses.DateFolder.ContentTypeAlias ?? "uwbsOrderDateFolder";
DiscountOrder.NodeAlias = aliasses.DiscountOrder.ContentTypeAlias ?? "uwbsDiscountOrder";
DiscountOrderSectionContentType.NodeAlias = aliasses.DiscountOrderSection.ContentTypeAlias ?? "uwbsDiscountOrderSection";
DiscountProduct.NodeAlias = aliasses.DiscountProduct.ContentTypeAlias ?? "uwbsDiscountProduct";
DiscountProductSectionContentType.NodeAlias = aliasses.DiscountProductSection.ContentTypeAlias ?? "uwbsDiscountProductSection";
DiscountRepositoryContentType.NodeAlias = aliasses.DiscountRepository.ContentTypeAlias ?? "uwbsDiscountRepository";
EmailCustomer.NodeAlias = aliasses.EmailCustomer.ContentTypeAlias ?? "uwbsEmailTemplateCustomer";
EmailCustomerSectionContentType.NodeAlias = aliasses.EmailCustomerSection.ContentTypeAlias ?? "uwbsEmailTemplateCustomerSection";
EmailRepositoryContentType.NodeAlias = aliasses.EmailRepository.ContentTypeAlias ?? "uwbsEmailRepository";
EmailStore.NodeAlias = aliasses.EmailStore.ContentTypeAlias ?? "uwbsEmailTemplateStore";
EmailStoreSectionContentType.NodeAlias = aliasses.EmailStoreSection.ContentTypeAlias ?? "uwbsEmailTemplateStoreSection";
Order.NodeAlias = aliasses.Order.ContentTypeAlias ?? "uwbsOrder";
OrderedProduct.NodeAlias = aliasses.OrderedProduct.ContentTypeAlias ?? "uwbsOrderedProduct";
OrderedProductVariant.NodeAlias = aliasses.OrderedProductVariant.ContentTypeAlias ?? "uwbsOrderedProductVariant";
OrderRepositoryContentType.NodeAlias = aliasses.OrderRepository.ContentTypeAlias ?? "uwbsOrderRepository";
OrderSection.NodeAlias = aliasses.OrderSection.ContentTypeAlias ?? "uwbsOrderSection";
OrderStoreFolder.NodeAlias = aliasses.OrderStoreFolder.ContentTypeAlias ?? "uwbsOrderStoreFolder";
PaymentProvider.NodeAlias = aliasses.PaymentProvider.ContentTypeAlias ?? "uwbsPaymentProvider";
PaymentProviderMethod.NodeAlias = aliasses.PaymentProviderMethod.ContentTypeAlias ?? "uwbsPaymentProviderMethod";
PaymentProviderRepositoryContentType.NodeAlias = aliasses.PaymentProviderRepository.ContentTypeAlias ?? "uwbsPaymentProviderRepository";
PaymentProviderSectionContentType.NodeAlias = aliasses.PaymentProviderSection.ContentTypeAlias ?? "uwbsPaymentProviderSection";
PaymentProviderZone.NodeAlias = aliasses.PaymentProviderZone.ContentTypeAlias ?? "uwbsPaymentProviderZone";
PaymentProviderZoneSectionContentType.NodeAlias = aliasses.PaymentProviderZoneSection.ContentTypeAlias ?? "uwbsPaymentProviderZoneSection";
Product.NodeAlias = aliasses.Product.ContentTypeAlias ?? "uwbsProduct";
ProductRepositoryContentType.NodeAlias = aliasses.ProductRepository.ContentTypeAlias ?? "uwbsProductRepository";
ProductVariant.NodeAlias = aliasses.ProductVariant.ContentTypeAlias ?? "uwbsProductVariant";
ProductVariantGroup.NodeAlias = aliasses.ProductVariantGroup.ContentTypeAlias ?? "uwbsProductVariantGroup";
Settings.NodeAlias = aliasses.Settings.ContentTypeAlias ?? "uwbsSettings";
ShippingProvider.NodeAlias = aliasses.ShippingProvider.ContentTypeAlias ?? "uwbsShippingProvider";
ShippingProviderMethod.NodeAlias = aliasses.ShippingProviderMethod.ContentTypeAlias ?? "uwbsShippingProviderMethod";
ShippingProviderRepositoryContentType.NodeAlias = aliasses.ShippingProviderRepository.ContentTypeAlias ?? "uwbsShippingProviderRepository";
ShippingProviderSectionContentType.NodeAlias = aliasses.ShippingProviderSection.ContentTypeAlias ?? "uwbsShippingProviderSection";
ShippingProviderZone.NodeAlias = aliasses.ShippingProviderZone.ContentTypeAlias ?? "uwbsShippingProviderZone";
ShippingProviderZoneSectionContentType.NodeAlias = aliasses.ShippingProviderZoneSection.ContentTypeAlias ?? "uwbsShippingProviderZoneSection";
Store.NodeAlias = aliasses.Store.ContentTypeAlias ?? "uwbsStore";
StoreRepositoryContentType.NodeAlias = aliasses.StoreRepository.ContentTypeAlias ?? "uwbsStoreRepository";
UwebshopRootContentType.NodeAlias = aliasses.UwebshopRoot.ContentTypeAlias ?? "uWebshop";
}
}
public static class GenerateXML {public static string GenerateXMLString()
{
var xml = new UwebshopAliassesXMLConfig();
xml.Catalog.ContentTypeAlias = "uwbsCatalog";
xml.Category.ContentTypeAlias = "uwbsCategory";
xml.Category.categories = "categories";
xml.Category.metaTags = "metaTags";
xml.Category.images = "images";
xml.Category.title = "title";
xml.Category.url = "url";
xml.Category.metaDescription = "metaDescription";
xml.Category.description = "description";
xml.CategoryRepository.ContentTypeAlias = "uwbsCategoryRepository";
xml.DateFolder.ContentTypeAlias = "uwbsOrderDateFolder";
xml.DiscountOrder.ContentTypeAlias = "uwbsDiscountOrder";
xml.DiscountOrder.items = "items";
xml.DiscountOrder.affectedOrderlines = "affectedOrderlines";
xml.DiscountOrder.affectedTags = "affectedTags";
xml.DiscountOrder.orderCondition = "orderCondition";
xml.DiscountOrder.numberOfItemsCondition = "numberOfItemsCondition";
xml.DiscountOrder.shippingDiscountable = "shippingDiscountable";
xml.DiscountOrder.minimumAmount = "minimumAmount";
xml.DiscountOrder.couponCodes = "couponCodes";
xml.DiscountOrder.oncePerCustomer = "oncePerCustomer";
xml.DiscountOrder.title = "title";
xml.DiscountOrder.description = "description";
xml.DiscountOrder.disable = "disable";
xml.DiscountOrder.discountType = "discountType";
xml.DiscountOrder.discount = "discount";
xml.DiscountOrder.ranges = "ranges";
xml.DiscountOrder.countdownEnabled = "countdownEnabled";
xml.DiscountOrder.countdown = "countdown";
xml.DiscountOrder.memberGroups = "memberGroups";
xml.DiscountOrderSection.ContentTypeAlias = "uwbsDiscountOrderSection";
xml.DiscountProduct.ContentTypeAlias = "uwbsDiscountProduct";
xml.DiscountProduct.products = "products";
xml.DiscountProduct.excludeVariants = "excludeVariants";
xml.DiscountProduct.title = "title";
xml.DiscountProduct.description = "description";
xml.DiscountProduct.disable = "disable";
xml.DiscountProduct.discountType = "discountType";
xml.DiscountProduct.discount = "discount";
xml.DiscountProduct.ranges = "ranges";
xml.DiscountProduct.countdownEnabled = "countdownEnabled";
xml.DiscountProduct.countdown = "countdown";
xml.DiscountProduct.memberGroups = "memberGroups";
xml.DiscountProductSection.ContentTypeAlias = "uwbsDiscountProductSection";
xml.DiscountRepository.ContentTypeAlias = "uwbsDiscountRepository";
xml.EmailCustomer.ContentTypeAlias = "uwbsEmailTemplateCustomer";
xml.EmailCustomer.title = "title";
xml.EmailCustomer.description = "description";
xml.EmailCustomer.emailtemplate = "emailtemplate";
xml.EmailCustomer.templatePreview = "templatePreview";
xml.EmailCustomerSection.ContentTypeAlias = "uwbsEmailTemplateCustomerSection";
xml.EmailRepository.ContentTypeAlias = "uwbsEmailRepository";
xml.EmailStore.ContentTypeAlias = "uwbsEmailTemplateStore";
xml.EmailStore.title = "title";
xml.EmailStore.description = "description";
xml.EmailStore.emailtemplate = "emailtemplate";
xml.EmailStore.templatePreview = "templatePreview";
xml.EmailStoreSection.ContentTypeAlias = "uwbsEmailTemplateStoreSection";
xml.Order.ContentTypeAlias = "uwbsOrder";
xml.Order.orderStatusPicker = "orderStatusPicker";
xml.Order.orderPaid = "orderPaid";
xml.Order.orderDetails = "orderDetails";
xml.Order.orderGuid = "orderGuid";
xml.Order.customerEmail = "customerEmail";
xml.Order.customerFirstName = "customerFirstName";
xml.Order.customerLastName = "customerLastName";
xml.OrderedProduct.ContentTypeAlias = "uwbsOrderedProduct";
xml.OrderedProduct.productId = "productId";
xml.OrderedProduct.typeAlias = "typeAlias";
xml.OrderedProduct.title = "title";
xml.OrderedProduct.sku = "sku";
xml.OrderedProduct.length = "length";
xml.OrderedProduct.width = "width";
xml.OrderedProduct.height = "height";
xml.OrderedProduct.weight = "weight";
xml.OrderedProduct.ranges = "ranges";
xml.OrderedProduct.price = "price";
xml.OrderedProduct.vat = "vat";
xml.OrderedProduct.itemCount = "itemCount";
xml.OrderedProduct.orderedProductDiscountPercentage = "orderedProductDiscountPercentage";
xml.OrderedProduct.orderedProductDiscountAmount = "orderedProductDiscountAmount";
xml.OrderedProduct.orderedProductDiscountExcludingVariants = "orderedProductDiscountExcludingVariants";
xml.OrderedProductVariant.ContentTypeAlias = "uwbsOrderedProductVariant";
xml.OrderedProductVariant.variantId = "variantId";
xml.OrderedProductVariant.typeAlias = "typeAlias";
xml.OrderedProductVariant.title = "title";
xml.OrderedProductVariant.sku = "sku";
xml.OrderedProductVariant.group = "group";
xml.OrderedProductVariant.length = "length";
xml.OrderedProductVariant.width = "width";
xml.OrderedProductVariant.height = "height";
xml.OrderedProductVariant.weight = "weight";
xml.OrderedProductVariant.price = "price";
xml.OrderedProductVariant.ranges = "ranges";
xml.OrderedProductVariant.discountPercentage = "discountPercentage";
xml.OrderedProductVariant.discountAmount = "discountAmount";
xml.OrderRepository.ContentTypeAlias = "uwbsOrderRepository";
xml.OrderSection.ContentTypeAlias = "uwbsOrderSection";
xml.OrderSection.orderSection = "orderSection";
xml.OrderStoreFolder.ContentTypeAlias = "uwbsOrderStoreFolder";
xml.PaymentProvider.ContentTypeAlias = "uwbsPaymentProvider";
xml.PaymentProvider.title = "title";
xml.PaymentProvider.description = "description";
xml.PaymentProvider.image = "image";
xml.PaymentProvider.type = "type";
xml.PaymentProvider.zone = "zone";
xml.PaymentProvider.successNode = "successNode";
xml.PaymentProvider.errorNode = "errorNode";
xml.PaymentProvider.testMode = "testMode";
xml.PaymentProviderMethod.ContentTypeAlias = "uwbsPaymentProviderMethod";
xml.PaymentProviderMethod.disable = "disable";
xml.PaymentProviderMethod.title = "title";
xml.PaymentProviderMethod.description = "description";
xml.PaymentProviderMethod.image = "image";
xml.PaymentProviderMethod.price = "price";
xml.PaymentProviderMethod.vat = "vat";
xml.PaymentProviderMethod.amountType = "amountType";
xml.PaymentProviderRepository.ContentTypeAlias = "uwbsPaymentProviderRepository";
xml.PaymentProviderSection.ContentTypeAlias = "uwbsPaymentProviderSection";
xml.PaymentProviderZone.ContentTypeAlias = "uwbsPaymentProviderZone";
xml.PaymentProviderZone.zone = "zone";
xml.PaymentProviderZoneSection.ContentTypeAlias = "uwbsPaymentProviderZoneSection";
xml.Product.ContentTypeAlias = "uwbsProduct";
xml.Product.sku = "sku";
xml.Product.metaTags = "metaTags";
xml.Product.categories = "categories";
xml.Product.images = "images";
xml.Product.files = "files";
xml.Product.length = "length";
xml.Product.width = "width";
xml.Product.height = "height";
xml.Product.weight = "weight";
xml.Product.price = "price";
xml.Product.ranges = "ranges";
xml.Product.vat = "vat";
xml.Product.stock = "stock";
xml.Product.ordered = "ordered";
xml.Product.stockStatus = "stockStatus";
xml.Product.backorderStatus = "backorderStatus";
xml.Product.useVariantStock = "useVariantStock";
xml.Product.title = "title";
xml.Product.url = "url";
xml.Product.metaDescription = "metaDescription";
xml.Product.description = "description";
xml.Product.disable = "disable";
xml.ProductRepository.ContentTypeAlias = "uwbsProductRepository";
xml.ProductRepository.productOverview = "productOverview";
xml.ProductVariant.ContentTypeAlias = "uwbsProductVariant";
xml.ProductVariant.title = "title";
xml.ProductVariant.sku = "sku";
xml.ProductVariant.disable = "disable";
xml.ProductVariant.description = "description";
xml.ProductVariant.length = "length";
xml.ProductVariant.width = "width";
xml.ProductVariant.height = "height";
xml.ProductVariant.weight = "weight";
xml.ProductVariant.price = "price";
xml.ProductVariant.ranges = "ranges";
xml.ProductVariant.stock = "stock";
xml.ProductVariant.ordered = "ordered";
xml.ProductVariant.stockStatus = "stockStatus";
xml.ProductVariant.backorderStatus = "backorderStatus";
xml.ProductVariantGroup.ContentTypeAlias = "uwbsProductVariantGroup";
xml.ProductVariantGroup.title = "title";
xml.ProductVariantGroup.required = "required";
xml.ProductVariantGroup.description = "description";
xml.Settings.ContentTypeAlias = "uwbsSettings";
xml.Settings.includingVat = "includingVat";
xml.Settings.lowercaseUrls = "lowercaseUrls";
xml.Settings.incompleteOrderLifetime = "incompleteOrderLifetime";
xml.ShippingProvider.ContentTypeAlias = "uwbsShippingProvider";
xml.ShippingProvider.title = "title";
xml.ShippingProvider.description = "description";
xml.ShippingProvider.image = "image";
xml.ShippingProvider.type = "type";
xml.ShippingProvider.rangeType = "rangeType";
xml.ShippingProvider.rangeStart = "rangeStart";
xml.ShippingProvider.rangeEnd = "rangeEnd";
xml.ShippingProvider.overrule = "overrule";
xml.ShippingProvider.zone = "zone";
xml.ShippingProvider.testMode = "testMode";
xml.ShippingProviderMethod.ContentTypeAlias = "uwbsShippingProviderMethod";
xml.ShippingProviderMethod.disable = "disable";
xml.ShippingProviderMethod.title = "title";
xml.ShippingProviderMethod.description = "description";
xml.ShippingProviderMethod.image = "image";
xml.ShippingProviderMethod.price = "price";
xml.ShippingProviderMethod.vat = "vat";
xml.ShippingProviderRepository.ContentTypeAlias = "uwbsShippingProviderRepository";
xml.ShippingProviderSection.ContentTypeAlias = "uwbsShippingProviderSection";
xml.ShippingProviderZone.ContentTypeAlias = "uwbsShippingProviderZone";
xml.ShippingProviderZone.zone = "zone";
xml.ShippingProviderZoneSection.ContentTypeAlias = "uwbsShippingProviderZoneSection";
xml.Store.ContentTypeAlias = "uwbsStore";
xml.Store.storeCulture = "storeCulture";
xml.Store.countryCode = "countryCode";
xml.Store.currencies = "currencies";
xml.Store.globalVat = "globalVat";
xml.Store.orderNumberPrefix = "orderNumberPrefix";
xml.Store.orderNumberTemplate = "orderNumberTemplate";
xml.Store.orderNumberStartNumber = "orderNumberStartNumber";
xml.Store.enableStock = "enableStock";
xml.Store.defaultUseVariantStock = "defaultUseVariantStock";
xml.Store.defaultCountdownEnabled = "defaultCountdownEnabled";
xml.Store.storeStock = "storeStock";
xml.Store.useBackorders = "useBackorders";
xml.Store.enableTestmode = "enableTestmode";
xml.Store.storeEmailFrom = "storeEmailFrom";
xml.Store.storeEmailFromName = "storeEmailFromName";
xml.Store.storeEmailTo = "storeEmailTo";
xml.Store.accountEmailCreated = "accountEmailCreated";
xml.Store.accountForgotPassword = "accountForgotPassword";
xml.Store.confirmationEmailStore = "confirmationEmailStore";
xml.Store.confirmationEmailCustomer = "confirmationEmailCustomer";
xml.Store.onlinePaymentEmailStore = "onlinePaymentEmailStore";
xml.Store.onlinePaymentEmailCustomer = "onlinePaymentEmailCustomer";
xml.Store.offlinePaymentEmailStore = "offlinePaymentEmailStore";
xml.Store.offlinePaymentEmailCustomer = "offlinePaymentEmailCustomer";
xml.Store.paymentFailedEmailStore = "paymentFailedEmailStore";
xml.Store.paymentFailedEmailCustomer = "paymentFailedEmailCustomer";
xml.Store.dispatchedEmailStore = "dispatchedEmailStore";
xml.Store.dispatchedEmailCustomer = "dispatchedEmailCustomer";
xml.Store.cancelEmailStore = "cancelEmailStore";
xml.Store.cancelEmailCustomer = "cancelEmailCustomer";
xml.Store.closedEmailStore = "closedEmailStore";
xml.Store.closedEmailCustomer = "closedEmailCustomer";
xml.Store.pendingEmailStore = "pendingEmailStore";
xml.Store.pendingEmailCustomer = "pendingEmailCustomer";
xml.Store.temporaryOutOfStockEmailStore = "temporaryOutOfStockEmailStore";
xml.Store.temporaryOutOfStockEmailCustomer = "temporaryOutOfStockEmailCustomer";
xml.Store.undeliverableEmailStore = "undeliverableEmailStore";
xml.Store.undeliverableEmailCustomer = "undeliverableEmailCustomer";
xml.Store.returnEmailStore = "returnEmailStore";
xml.Store.returnEmailCustomer = "returnEmailCustomer";
xml.StoreRepository.ContentTypeAlias = "uwbsStoreRepository";
xml.UwebshopRoot.ContentTypeAlias = "uWebshop";
	var settings = new XmlWriterSettings();
	settings.OmitXmlDeclaration = true;
	settings.ConformanceLevel = ConformanceLevel.Document;
	settings.CloseOutput = false;
	settings.Indent = true;
	using (var writer = new System.IO.StringWriter())
		{
			var writerr = XmlWriter.Create(writer, settings);
			var x = new System.Xml.Serialization.XmlSerializer(xml.GetType());
			
			x.Serialize(writerr, xml);
				
			writerr.Flush();
			writerr.Close();
			return writer.ToString();
		}
	}
}


}
