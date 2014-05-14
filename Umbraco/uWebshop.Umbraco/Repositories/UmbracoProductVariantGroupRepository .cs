using System;
using System.Linq;
using uWebshop.Domain;
using uWebshop.Domain.BaseClasses;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
    internal class UmbracoProductVariantGroupRepository : UmbracoMultiStoreEntityRepository<ProductVariantGroup, ProductVariantGroup>, IProductVariantGroupRepository
    {
        private readonly IStoreService _storeService;
        private readonly IProductVariantGroupAliassesService _aliasses;

        public UmbracoProductVariantGroupRepository(IStoreService storeService, IProductVariantGroupAliassesService productVariantGroupAliassesService)
        {
            _storeService = storeService;
            _aliasses = productVariantGroupAliassesService;
        }

        public override void LoadDataFromPropertiesDictionary(ProductVariantGroup variantgroup, IPropertyProvider fields, ILocalization localization)
        {
            variantgroup.Localization = localization;
            var store = _storeService.GetByAlias(localization.StoreAlias);

            variantgroup.Variants = Enumerable.Empty<IProductVariant>();
            variantgroup.ProductVariants = Enumerable.Empty<ProductVariant>();

            variantgroup.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields);

            var rteItem = "RTEItem" + _aliasses.description;
            variantgroup.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(
                StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(fields.ContainsKey(rteItem) ? rteItem : _aliasses.description, localization, fields));

            variantgroup.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

            var value = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.required, localization, fields);
            variantgroup.Required = value == "1" || value == "true";

            //variantgroup.ProductVariantFactory = () => IO.Container.Resolve<IProductVariantService>().GetAll(localization).Where(productvariantgroup => productvariantgroup.ParentId == variantgroup.Id).Cast<IProductVariant>().ToList();

            variantgroup.Variants = IO.Container.Resolve<IProductVariantService>().GetAll(localization).Where(variant => variant.ParentId == variantgroup.Id).Cast<IProductVariant>().ToList();
           
        }

        public override string TypeAlias
        {
            get { return ProductVariantGroup.NodeAlias; }
        }

        public ICacheRebuilder GetCacheRebuilder()
        {
            throw new NotSupportedException();
        }
    }
}