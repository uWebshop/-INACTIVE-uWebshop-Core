using System;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoShippingProviderRepository : UmbracoMultiStoreEntityRepository<ShippingProvider, ShippingProvider>, IShippingProviderRepository
	{
		private readonly IZoneService _zoneService;
		private readonly IStoreService _storeService;
		private readonly IShippingProviderAliassesService _aliasses;

		public UmbracoShippingProviderRepository(IZoneService zoneService, IStoreService storeService, IShippingProviderAliassesService shippingProviderAliassesService)
		{
			_zoneService = zoneService;
			_storeService = storeService;
			_aliasses = shippingProviderAliassesService;
		}

		public override void LoadDataFromPropertiesDictionary(ShippingProvider entity, IPropertyProvider fields, ILocalization localization)
		{
			var store = _storeService.GetByAlias(localization.StoreAlias);

			entity.Localization = localization;

			entity.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields) ?? string.Empty;
			entity.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.description, localization, fields)) ?? string.Empty;

			var testMode = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.testMode, localization, fields);
			if (testMode == "default" || testMode == string.Empty)
			{
				entity.TestMode = store.EnableTestmode;
			}
			else
			{
				entity.TestMode = testMode == "enable" || testMode == "1" || testMode == "true";
			}

			entity.ImageId = StoreHelper.GetMultiStoreIntValue(_aliasses.image, localization, fields);
			entity.Zone = _zoneService.GetByIdOrFallbackZone(StoreHelper.GetMultiStoreIntValue(_aliasses.zone, localization, fields), localization);
			
			var paymentProviderAmountType = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.type, localization, fields);
			ShippingProviderType type;
			entity.Type = Enum.TryParse(paymentProviderAmountType, out type) ? type : ShippingProviderType.Unknown;

			var shippingRangeType = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.rangeType, localization, fields);
			ShippingRangeType rangeType;
			entity.TypeOfRange = Enum.TryParse(shippingRangeType, out rangeType) ? rangeType : ShippingRangeType.None;
			
			entity.Overrule = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.overrule, localization, fields) == "1";
			
			var dllName = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("dllName", localization, fields);
			if (!string.IsNullOrEmpty(dllName))
			{
				entity.DLLName = !dllName.EndsWith(".dll") ? string.Format("{0}.dll", dllName) : dllName;
			}

			entity.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);
			
			entity.RangeFrom = StoreHelper.GetMultiStoreDecimalValue(_aliasses.rangeStart, localization, fields);
			entity.RangeTo = StoreHelper.GetMultiStoreDecimalValue(_aliasses.rangeEnd, localization, fields);
			if (entity.RangeTo == 0) entity.RangeTo = decimal.MaxValue;
		}

		public override string TypeAlias
		{
			get { return ShippingProvider.NodeAlias; }
		}
	}
}