using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoShippingProviderMethodRepository : UmbracoMultiStoreEntityRepository<ShippingProviderMethod, ShippingProviderMethod>, IShippingProviderMethodRepository
	{
		private readonly IStoreService _storeService;
		private readonly IShippingProviderMethodAliassesService _aliasses;

		public UmbracoShippingProviderMethodRepository(IStoreService storeService, IShippingProviderMethodAliassesService aliasses)
		{
			_storeService = storeService;
			_aliasses = aliasses;
		}

		public override void LoadDataFromPropertiesDictionary(ShippingProviderMethod entity, IPropertyProvider fields, ILocalization localization)
		{
			var store = _storeService.GetByAlias(localization.StoreAlias);
			
			entity.Localization = localization;
			entity.NodeId = (entity as IUwebshopEntity).Id;
			entity.Id = entity.NodeId.ToString();
            entity.Key = entity.Key;

            entity.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

			entity.PriceInCents = StoreHelper.GetLocalizedPrice(_aliasses.price, localization, fields);

			entity.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields) ?? string.Empty;
			entity.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.description, localization, fields)) ?? string.Empty;

			var testMode = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("testMode", localization, fields).ToLower();
			if (testMode == "default" || testMode == string.Empty)
			{
				entity.TestMode = store.EnableTestmode;
			}
			else
			{
				entity.TestMode = testMode == "enable" || testMode == "1" || testMode == "true";
			}

			entity.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

			entity.ImageId = StoreHelper.GetMultiStoreIntValue(_aliasses.image, localization, fields);
			var vat = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.vat, localization, fields);
			decimal vatPercentage = 0;
			if (string.IsNullOrWhiteSpace(vat) || !decimal.TryParse(vat, out vatPercentage))
			{
				vatPercentage = store.GlobalVat;
			}
			entity.Vat = vatPercentage;
		}

		public override string TypeAlias
		{
			get { return ShippingProviderMethod.NodeAlias; }
		}
	}
}