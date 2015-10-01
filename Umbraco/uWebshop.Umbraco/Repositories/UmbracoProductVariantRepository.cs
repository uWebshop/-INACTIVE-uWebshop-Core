using System;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoProductVariantRepository : UmbracoMultiStoreEntityRepository<ProductVariant, ProductVariant>, IProductVariantRepository
	{
		private readonly IStoreService _storeService;
		private readonly IProductVariantAliassesService _aliasses;

		public UmbracoProductVariantRepository(IStoreService storeService, IProductVariantAliassesService productVariantAliassesService)
		{
			_storeService = storeService;
			_aliasses = productVariantAliassesService;
		}

		public override void LoadDataFromPropertiesDictionary(ProductVariant variant, IPropertyProvider fields, ILocalization localization)
		{
			variant.Localization = localization;
			var store = _storeService.GetByAlias(localization.StoreAlias);

			variant.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);
			variant.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields);

			var rteItem = "RTEItem" + _aliasses.description;
			variant.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(
				StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(fields.ContainsKey(rteItem) ? rteItem : _aliasses.description, localization, fields));

			variant.SKU = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.sku, localization, fields);

			variant.OriginalPriceInCents = StoreHelper.GetLocalizedPrice(_aliasses.price, localization, fields);

			variant.Weight = StoreHelper.GetMultiStoreDoubleValue(_aliasses.weight, localization, fields);
			variant.Width = StoreHelper.GetMultiStoreDoubleValue(_aliasses.width, localization, fields);
			variant.Length = StoreHelper.GetMultiStoreDoubleValue(_aliasses.length, localization, fields);
			variant.Height = StoreHelper.GetMultiStoreDoubleValue(_aliasses.height, localization, fields);

			var stockStatus = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.stockStatus, localization, fields);
			if (stockStatus == "default" || stockStatus == string.Empty)
			{
				variant.StockStatus = store.UseStock;
			}
			else
			{
				variant.StockStatus = stockStatus == "enable" || stockStatus == "1" || stockStatus == "true";
			}
			var backorderStatus = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.backorderStatus, localization, fields);
			if (backorderStatus == "default" || backorderStatus == string.Empty)
			{
				variant.BackorderStatus = store.UseBackorders;
			}
			else
			{
				variant.BackorderStatus = backorderStatus == "enable" || backorderStatus == "1" || backorderStatus == "true";
			}

			variant.Group = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("group", localization, fields);

			var value = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("requiredVariant", localization, fields);
			variant.Required = value == "1" || value == "true";

			var rangesString = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.ranges, localization, fields);
			variant.Ranges = StoreHelper.LocalizeRanges(Range.CreateFromString(rangesString), localization);
		}

		public override string TypeAlias
		{
			get { return ProductVariant.NodeAlias; }
		}

		public ICacheRebuilder GetCacheRebuilder()
		{
			throw new NotSupportedException();
		}
	}
}