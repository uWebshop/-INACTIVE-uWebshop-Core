using System;
using uWebshop.Common;
using uWebshop.Common.Interfaces;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoPaymentProviderMethodRepository : UmbracoMultiStoreEntityRepository<PaymentProviderMethod, PaymentProviderMethod>, IPaymentProviderMethodRepository
	{
		private readonly IStoreService _storeService;
		private readonly IPaymentProviderMethodAliassesService _aliasses;

		public UmbracoPaymentProviderMethodRepository(IStoreService storeService, IPaymentProviderMethodAliassesService aliasses)
		{
			_storeService = storeService;
			_aliasses = aliasses;
		}

		public override void LoadDataFromPropertiesDictionary(PaymentProviderMethod entity, IPropertyProvider fields, ILocalization localization)
		{
			var store = _storeService.GetByAlias(localization.StoreAlias);

			entity.NodeId = (entity as IUwebshopEntity).Id;
			entity.Id = entity.NodeId.ToString();

			entity.Disabled = StoreHelper.GetMultiStoreDisableExamine(localization, fields);

			entity.Localization = localization;
			entity.Name =  StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields) ?? string.Empty;
			entity.Title = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.title, localization, fields) ?? string.Empty;
			entity.Description = IO.Container.Resolve<ICMSApplication>().ParseInternalLinks(StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.description, localization, fields)) ?? string.Empty;

			var paymentProviderAmountType = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary(_aliasses.amountType, localization, fields);
			PaymentProviderAmountType type;
			entity.AmountType = Enum.TryParse(paymentProviderAmountType, out type) ? type : PaymentProviderAmountType.Amount;

			entity.PriceInCents = StoreHelper.GetLocalizedPrice(_aliasses.price, localization, fields);

			var testMode = StoreHelper.ReadMultiStoreItemFromPropertiesDictionary("testMode", localization, fields);
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
				if (store != null) vatPercentage = store.GlobalVat;
			}
			entity.Vat = vatPercentage;
		}

		public override string TypeAlias
		{
			get { return PaymentProviderMethod.NodeAlias; }
		}
	}
}