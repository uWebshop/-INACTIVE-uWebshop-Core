using System;
using System.Linq;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
    internal class UmbracoPaymentProviderRepository : UmbracoMultiStoreEntityRepository<PaymentProvider, PaymentProvider>, IPaymentProviderRepository
    {
        private readonly IZoneService _zoneService;
        private readonly IStoreService _storeService;
        private readonly IPaymentProviderAliassesService _aliasses;

        public UmbracoPaymentProviderRepository(IZoneService zoneService, IStoreService storeService, IPaymentProviderAliassesService aliasses)
        {
            _zoneService = zoneService;
            _storeService = storeService;
            _aliasses = aliasses;
        }

        public override void LoadDataFromPropertiesDictionary(PaymentProvider entity, IPropertyProvider fields, ILocalization localization)
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

            var values = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.zone, localization, fields);

            if (values.Any())
            {
                entity.Zones =
                    DomainHelper.ParseIntegersFromUwebshopProperty(values)
                        .Select(x => _zoneService.GetByIdOrFallbackZone(x, localization))
                        .ToList();
            }
            else
            {
                entity.Zones = _zoneService.GetFallBackZone(localization);
            }

            entity.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

            var paymentProviderAmountType = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.type, localization, fields);
            PaymentProviderType type;
            entity.Type = Enum.TryParse(paymentProviderAmountType, out type) ? type : PaymentProviderType.Unknown;

            var dllName = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("dllName", localization, fields);
            if (!string.IsNullOrEmpty(dllName))
            {
                entity.DLLName = !dllName.EndsWith(".dll") ? string.Format("{0}.dll", dllName) : dllName;
            }

            entity.ControlNodeId = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("controlNode", localization, fields);
            entity.SuccesNodeId = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.successNode, localization, fields);
            entity.ErrorNodeId = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.errorNode, localization, fields);
            entity.CancelNodeId = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.cancelNode, localization, fields);
        }

        public override string TypeAlias
        {
            get { return PaymentProvider.NodeAlias; }
        }
    }
}