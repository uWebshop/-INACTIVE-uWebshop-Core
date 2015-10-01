using System;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Umbraco.Businesslogic;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoCategoryRepository : UmbracoMultiStoreEntityRepository<Category, Category>, ICategoryRepository
	{
		private readonly ICategoryAliassesService _aliasses;
		private readonly ICMSContentService _cmsContentService;

		// watch out: it's not possible to use the CategoryService here, since the IOCContainer will loop (currently)
		public UmbracoCategoryRepository(ICategoryAliassesService categoryAliassesService, ICMSContentService cmsContentService)
		{
			_aliasses = categoryAliassesService;
			_cmsContentService = cmsContentService;
		}

		public override void LoadDataFromPropertiesDictionary(Category category, IPropertyProvider fields, ILocalization localization)
		{
			category.Localization = localization;
			category.ClearCachedValues(); // (hack to reload relations)

			category.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

			category.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields);
			var url = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.url, localization, fields);
			
			if (!string.IsNullOrEmpty(url))
			{
				category.URL = url;
			}
			else
			{
				// if there is no url field filled or available, fallback to the Urlname of the node
				category.URL = _cmsContentService.GetReadonlyById(category.Id).UrlName;
			}

			category.MetaDescription = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.metaDescription, localization, fields);

			var rteItem = "RTEItem" + _aliasses.description;
			category.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(
				StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(fields.ContainsKey(rteItem) ? rteItem : _aliasses.description, localization, fields));

			category.SetTemplate(StoreHelper.GetMultiStoreIntValue("template", localization, fields));

			// todo: refactor below to set simpel properties on Category instead of internal fields that are used by product to make the actual properties

			Category.ImageFactory = InternalHelpers.LoadImageWithId;
			var imagesProperty = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.images, localization, fields);
			category.ImageIds = DomainHelper.ParseIntegersFromUwebshopProperty(imagesProperty).ToArray();//.Select(InternalHelpers.LoadImageWithId).ToList();

			// todo: load categories from service (using stubs???)
			var values = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.categories, localization, fields);
			category._categoryIds = DomainHelper.ParseIntegersFromUwebshopProperty(values).ToList();
			category.HasCategories = !string.IsNullOrEmpty(values);

			var tagsValue = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.metaTags, localization, fields);
			category.Tags = InternalHelpers.ParseTagsString(tagsValue);

			// todo: inefficient
			category.ProductsFactory = () => IO.Container.Resolve<IProductService>().GetAll(localization).Where(product => product.ParentId == category.Id || product.HasCategories && product.Categories.Any(cat => cat.Id == category.Id)).Cast<API.IProduct>().ToList();
		}

		public override string TypeAlias
		{
			get { return Category.NodeAlias; }
		}

		public ICacheRebuilder GetCacheRebuilder()
		{
			throw new NotSupportedException();
		}
	}
}