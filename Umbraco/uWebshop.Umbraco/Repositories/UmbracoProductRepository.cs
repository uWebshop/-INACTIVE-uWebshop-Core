using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using umbraco.presentation.install.utills;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Domain.Services;
using uWebshop.Umbraco.Businesslogic;
using Umbraco.Web;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoProductRepository : UmbracoMultiStoreEntityRepository<Product, Product>, IProductRepository
	{
		private readonly ISettingsService _settingsService;
		private readonly IStoreService _storeService;
		private readonly IProductVariantService _variantService;
		private readonly IProductVariantGroupService _variantGroupService;
		private readonly IProductAliassesService _aliasses;
		
		public UmbracoProductRepository(ISettingsService settingsService, IStoreService storeService, IProductVariantService variantService, IProductVariantGroupService variantGroupService, IProductAliassesService productAliassesService)
		{
			_settingsService = settingsService;
			_storeService = storeService;
			_variantService = variantService;
			_variantGroupService = variantGroupService;
			_aliasses = productAliassesService;
		}

		public override void LoadDataFromPropertiesDictionary(Product product, IPropertyProvider fields, ILocalization localization)
		{
			if (product == null) throw new ArgumentNullException("product");
			if (fields == null) throw new ArgumentNullException("fields");
			if (localization == null) throw new ArgumentNullException("localization");
			if (_aliasses == null) throw new NullReferenceException("_aliasses");
			if (_settingsService == null) throw new NullReferenceException("_settingsService");
			if (_storeService == null) throw new NullReferenceException("_storeService");

			product.Localization = localization;

			var store = _storeService.GetByAlias(localization.StoreAlias);
			if (store == null) throw new NullReferenceException("Store with alias " + localization.StoreAlias + " not found! Please rebuild examine index");

			product.ClearCachedValues(); // (hack to reload relations)

			product.PricesIncludingVat = _settingsService.IncludingVat;

			product.SetTemplate(StoreHelper.GetMultiStoreIntValue("template", localization, fields));

			product.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields);
			var url = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.url, localization, fields);

			if (!string.IsNullOrEmpty(url))
			{
				product.URL = url;
			}
			else
			{
				// if there is no url field filled or available, fallback to the Urlname of the node
				product.URL = new UmbracoHelper(UmbracoContext.Current).TypedContent(product.Id).UrlName;
			}

			product.MetaDescription = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.metaDescription, localization, fields);

			var rteItem = "RTEItem" + _aliasses.description;
			product.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(
				StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(fields.ContainsKey(rteItem) ? rteItem : _aliasses.description, localization, fields));
			
			product.SKU = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.sku, localization, fields);
			Product.ImageFactory = InternalHelpers.LoadImageWithId;

			var imagesProperty = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.images, localization, fields);
			product.ImageIds = DomainHelper.ParseIntegersFromUwebshopProperty(imagesProperty).ToArray();

			var filesProperty = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.files, localization, fields);
			Product.FileFactory = InternalHelpers.LoadFileWithId;
			product.FileIds = DomainHelper.ParseIntegersFromUwebshopProperty(filesProperty).ToArray();//.Select(InternalHelpers.LoadFileWithId).Where(x => x !=null).ToList();

			// todo: load categories from service
			var values = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.categories, localization, fields);
			product.CategoryIds = DomainHelper.ParseIntegersFromUwebshopProperty(values).ToList();
			if (!product.CategoryIds.Contains(product.ParentId) /* todo: and parent is not ProductRepo */) product.CategoryIds = new List<int> {product.ParentId}.Concat(product.CategoryIds).ToList();

			product.HasCategories = !string.IsNullOrEmpty(values);

			// todo: use data from range including 1 if present (also in other entitities)
			product.OriginalPriceInCents = StoreHelper.GetLocalizedPrice(_aliasses.price, localization, fields);

			product.TotalItemsOrdered = StoreHelper.GetMultiStoreIntValue(_aliasses.ordered, localization, fields);

			product.Weight = StoreHelper.GetMultiStoreDoubleValue(_aliasses.weight, localization, fields);
			product.Width = StoreHelper.GetMultiStoreDoubleValue(_aliasses.width, localization, fields);
			product.Length = StoreHelper.GetMultiStoreDoubleValue(_aliasses.length, localization, fields);
			product.Height = StoreHelper.GetMultiStoreDoubleValue(_aliasses.height, localization, fields);

			var vatProperty = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.vat, localization, fields);
			if (!string.IsNullOrEmpty(vatProperty))
			{
				var vat = store.GlobalVat;

				if (vatProperty.ToLowerInvariant() != "default")
				{
					vatProperty = vatProperty.Replace(',', '.').Replace("%", string.Empty);

					vat = Convert.ToDecimal(vatProperty, CultureInfo.InvariantCulture);
				}

				product.Vat = vat;
			}
			else
			{
				product.Vat = store.GlobalVat;
			}

			var stockStatus = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.stockStatus, localization, fields);
			if (stockStatus == "default" || stockStatus == string.Empty)
			{
				product.StockStatus = store.UseStock;
			}
			else
			{
				product.StockStatus = stockStatus == "enable" || stockStatus == "1" || stockStatus == "true";
			}
			var backorderStatus = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.backorderStatus, localization, fields);
			if (backorderStatus == "default" || backorderStatus == string.Empty)
			{
				product.BackorderStatus = store.UseBackorders;
			}
			else
			{
				product.BackorderStatus = backorderStatus == "enable" || backorderStatus == "1" || backorderStatus == "true";
			}
			var useVariantStock = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.useVariantStock, localization, fields);
			if (useVariantStock == "default" || useVariantStock == string.Empty)
			{
				product.UseVariantStock = store.UseVariantStock;
			}
			else
			{
				product.UseVariantStock = useVariantStock == "enable" || useVariantStock == "1" || useVariantStock == "true";
			}

			var tagsValue = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.metaTags, localization, fields);
			product.Tags = InternalHelpers.ParseTagsString(tagsValue);

			var rangesString = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.ranges, localization, fields);
			product.Ranges = StoreHelper.LocalizeRanges(Range.CreateFromString(rangesString), localization);


			product.ProductVariantGroupsFactory = () =>
			{
				var productvariantGroups =
					IO.Container.Resolve<IProductVariantGroupService>()
						.GetAll(localization)
						.Where(productvariantgroup => productvariantgroup.ParentId == product.Id)
						.Cast<IProductVariantGroup>()
						.ToList();

				if (productvariantGroups.Any())
				{
					// new situation: actual ProductVariantGroup node
					return productvariantGroups;
				}

				// old situation: variant nodes directly under product node
				var productVariants = IO.Container.Resolve<IProductVariantService>()
										.GetAll(localization)
										.Where(productvariant => productvariant.ParentId == product.Id)
										.ToList();

				var counter = 0;
				return productVariants.GroupBy(variant => variant.Group).Select(g => new ProductVariantGroup(g.Key, g, counter++)).Cast<IProductVariantGroup>().ToList();
			};
			product.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields) || product.Categories.Any() && product.Categories.All(category => category.Disabled); // logic is a tiny bit unsure
		}

		public override string TypeAlias
		{
			get { return Product.NodeAlias; }
		}

		//// nice xml fallback for loading products under a category
		//var iterator = library.GetXmlNodeByXPath(string.Format("//*[starts-with(name(),'" + Product.NodeAlias + "') and not(starts-with(name(),'" + Product.NodeAlias + "Repository'))]|/categories[contains(.,'" + categoryId + "')] | //*[starts-with(name(),'" + Product.NodeAlias + "') and not(starts-with(name(),'" + Product.NodeAlias + "Repository')) and @parentID='" + categoryId + "']"));
		//var objects = new List<Product>();
		//while (iterator.MoveNext())
		//{
		//	if (iterator.Current == null) continue;
		//	var nodeId = Convert.Toint(iterator.Current.GetAttribute("id", iterator.Current.NamespaceURI));

		//	var constructorInfo = typeof (Product).GetConstructor(new[] {typeof (int)});
		//	if (constructorInfo != null)
		//		objects.Add((Product) constructorInfo.Invoke(new object[] {nodeId}));
		//}
		//return objects.Where(product => !product.Disabled).AsQueryable();
		public void InvalidateCache()
		{
			
		}

		public ICacheRebuilder GetCacheRebuilder()
		{
			throw new NotSupportedException();
		}
	}

	internal class ProductRepoData
	{
		public Product Product { get; set; }
		public bool VatLoaded { get; set; }
		public List<int> CategoryIds { get; set; }
		public bool ProductDisabled { get; set; }
	}
}