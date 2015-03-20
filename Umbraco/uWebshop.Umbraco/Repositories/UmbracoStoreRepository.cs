using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.language;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Repositories
{
	internal class UmbracoStoreRepository : IStoreRepository
	{
		private readonly ICMSApplication _cmsApplication;
		private readonly ICMSEntityRepository _cmsEntityRepository;
		private readonly IStoreAliassesService _aliasses;
		private readonly UmbracoStoreRepo _storeRepo;

		public UmbracoStoreRepository(ICMSApplication cmsApplication, ICMSEntityRepository cmsEntityRepository, IStoreAliassesService aliasses)
		{
			_cmsApplication = cmsApplication;
			_cmsEntityRepository = cmsEntityRepository;
			_aliasses = aliasses;
			_storeRepo = new UmbracoStoreRepo();
		}

		public List<Store> GetAll()
		{
			return _storeRepo.GetAll(null);
		}

		public Store TryGetStoreFromCookie()
		{
			if (_cmsApplication.RequestIsInCMSBackend(HttpContext.Current))
			{
				return null;
			}

			if (HttpContext.Current.Request.Cookies["StoreInfo"] != null)
			{
				var storeAlias = HttpContext.Current.Request.Cookies["StoreInfo"].Values["StoreAlias"];

				if (!string.IsNullOrEmpty(storeAlias))
				{
					return StoreHelper.GetByAlias(storeAlias);
				}
			}

			return null;
		}


		public Store TryGetStoreFromCurrentNode()
		{
			//var currencyCode = UwebshopRequest.Current.CurrentCurrencyCode;
			if (_cmsApplication.RequestIsInCMSBackend(HttpContext.Current)) return null;
			try
			{
				var n = Node.GetCurrent();
				if (n != null)
				{
					while (n.Parent != null && (n.GetProperty(Constants.StorePickerAlias) == null || string.IsNullOrEmpty(n.GetProperty(Constants.StorePickerAlias).Value)))
						n = new Node(n.Parent.Id);
					//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " StoreHelper.GetCurrentStore 7");
					var pickerProperty = n.GetProperty(Constants.StorePickerAlias);
					if (pickerProperty != null && pickerProperty.Value != "0")
					{
						var value = pickerProperty.Value;
						if (!string.IsNullOrEmpty(value) && value != "0")
						{
							//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " StoreHelper.GetCurrentStore maakt new Store object (=Examine call)");
							var currentStore = GetById(Convert.ToInt32(value), null /* todo */); // dit kan nasty zijn omdat de properties via GetMultiStore deze functie aan kunnen roepen
							if (string.IsNullOrEmpty(currentStore.Alias)) throw new Exception("Failed to load store data");
							//Log.Instance.LogDebug(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " StoreHelper.GetCurrentStore 8");
							if (currentStore.NodeTypeAlias == Store.NodeAlias)
							{
								return currentStore;
							}
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		
		public Store GetById(int id, ILocalization localization)
		{
			var store = new Store();
			_storeRepo.LoadDataFromNode(store, new Node(id), localization);
			return store;
		}

		private class UmbracoStoreRepo : UmbracoMultiStoreEntityRepository<Store, Store>
		{
			public override void LoadDataFromPropertiesDictionary(Store store, IPropertyProvider fields, ILocalization localization)
			{
				// note: it's impossible to use StoreHelper.GetMultiStoreItemExamine here (or any multi store)

				if (fields.ContainsKey("incompleteOrderLifetime"))
				{
					double incompleteOrderLifetime;
					if (double.TryParse(fields.GetStringValue("incompleteOrderLifetime"), out incompleteOrderLifetime))
						store.IncompleOrderLifetime = incompleteOrderLifetime;
				}
				if (fields.ContainsKey("globalVat"))
				{
				    var vatProperty = fields.GetStringValue("globalVat");
                    if (!string.IsNullOrEmpty(vatProperty))
                    {
                        if (vatProperty.ToLowerInvariant() != "default")
                        {
                            vatProperty = vatProperty.Replace(',', '.');

                            var vat = Convert.ToDecimal(vatProperty, CultureInfo.InvariantCulture);

                            store.GlobalVat = vat;
                        }
                        else
                        {
                            store.GlobalVat = 0;
                        }
                    }
				}
				if (fields.ContainsKey("storeCulture"))
				{
					var culture = fields.GetStringValue("storeCulture");
					int languageId = 0;

					if (culture != null) int.TryParse(culture, out languageId);

					if (languageId != 0)
					{
						var matchedLanguage = Language.GetAllAsList().FirstOrDefault(x => x.id == languageId);
						if (matchedLanguage != null)
						{
							store.Culture = new Language(languageId).CultureAlias;
						}
						else
						{
							var firstOrDefault = Language.GetAllAsList().FirstOrDefault();
							if (firstOrDefault != null)
							{
								store.Culture = firstOrDefault.CultureAlias;
							}
							else
							{
								store.Culture = string.Empty;
							}
						}
					}
					else
					{
						store.Culture = string.Empty;
					}
				}
				if (fields.ContainsKey("countryCode"))
				{
					store.CountryCode = fields.GetStringValue("countryCode");
				}
				if (fields.ContainsKey("defaultCountryCode"))
				{
					store.DefaultCountryCode = fields.GetStringValue("defaultCountryCode");
				}
				if (fields.ContainsKey("currencyCulture"))
				{
					// this is for backwards compatiblity
					var culture = fields.GetStringValue("currencyCulture");
					var languageId = 0;

					if (culture != null) int.TryParse(culture, out languageId);

					store.CurrencyCulture = languageId != 0 ? new Language(languageId).CultureAlias : string.Empty;
				}
				RegionInfo currencyRegion;
				if (fields.ContainsKey("currencyCulture"))
				{
					var currencyCulture = new CultureInfo(store.CurrencyCulture);
					currencyRegion = new RegionInfo(currencyCulture.LCID);
				}
				else
				{
					currencyRegion = new RegionInfo(store.CultureInfo.LCID);
				}
				var cultureCurrency = currencyRegion.ISOCurrencySymbol;
				if (fields.ContainsKey("currencies"))
				{
					// "EUR|1.1#USD|0.7#JPY|1"
					var currenciesfromfield =  fields.GetStringValue("currencies").Split('#').Select(x => new Currency(x)).ToList();
					
					if (!currenciesfromfield.Any(c => c.ISOCurrencySymbol == cultureCurrency))
					{
						currenciesfromfield.Add(new Currency { ISOCurrencySymbol = cultureCurrency, Ratio = 1, CurrencySymbol = currencyRegion.CurrencySymbol });
					}

					store.Currencies = currenciesfromfield;
					store.CurrencyCodes = store.Currencies.Select(c => c.ISOCurrencySymbol).ToList();
				}
				else
				{
					store.Currencies = new[] {new Currency {ISOCurrencySymbol = cultureCurrency, Ratio = 1, CurrencySymbol = currencyRegion.CurrencySymbol}};
					store.CurrencyCodes = new[] {cultureCurrency};
				}
				if (fields.ContainsKey("orderNumberPrefix"))
				{
					store.OrderNumberPrefix = fields.GetStringValue("orderNumberPrefix");
				}
				if (fields.ContainsKey("orderNumberTemplate"))
				{
					store.OrderNumberTemplate = fields.GetStringValue("orderNumberTemplate");
				}
				if (fields.ContainsKey("orderNumberStartNumber"))
				{
					int orderNumberStartNumber;
					if (int.TryParse(fields.GetStringValue("orderNumberStartNumber"), out orderNumberStartNumber))
						store.OrderNumberStartNumber = orderNumberStartNumber;
				}
				if (fields.ContainsKey("enableStock"))
				{
					var enableStockValue = fields.GetStringValue("enableStock");
					store.UseStock = enableStockValue == "enable" || enableStockValue == "1" || enableStockValue == "true";
				}
				if (fields.ContainsKey("defaultUseVariantStock"))
				{
					var enableStockValue = fields.GetStringValue("defaultUseVariantStock");
					store.UseVariantStock = enableStockValue == "enable" || enableStockValue == "1" || enableStockValue == "true";
				}

				if (fields.ContainsKey("defaultCountdownEnabled"))
				{
					var enableCountdownValue = fields.GetStringValue("defaultCountdownEnabled");
					store.UseCountdown = enableCountdownValue == "enable" || enableCountdownValue == "1" || enableCountdownValue == "true";
				}
				
				if (fields.ContainsKey("storeStock"))
				{
					var storeStockValue = fields.GetStringValue("storeStock");
					var value = storeStockValue == "enable" || storeStockValue == "1" || storeStockValue == "true";
					if (value)
					{
						var productDt = DocumentType.GetByAlias(Product.NodeAlias);
						if (!productDt.PropertyTypes.Any(x => x.Alias.ToLower() == "stock_" + store.Alias.ToLower()))
						{
							value = false;
						}
					}
					store.UseStoreSpecificStock = value;
				}

				if (fields.ContainsKey("useBackorders"))
				{
					var useBackorders = fields.GetStringValue("useBackorders");

					store.UseBackorders = useBackorders == "enable" || useBackorders == "1" || useBackorders == "true";
				}

				if (fields.ContainsKey("enableTestmode"))
				{
					var enableTestmode = fields.GetStringValue("enableTestmode");

					store.EnableTestmode = enableTestmode == "enable" || enableTestmode == "1" || enableTestmode == "true";
				}

				store.EmailAddressFrom = FieldsValueOrEmpty("storeEmailFrom", fields);
				store.EmailAddressFromName = FieldsValueOrEmpty("storeEmailFromName", fields);
				store.EmailAddressTo = FieldsValueOrEmpty("storeEmailTo", fields);
				store.AccountCreatedEmail = FieldsValueOrEmpty("accountEmailCreated", fields);
				store.AccountForgotPasswordEmail = FieldsValueOrEmpty("accountForgotPassword", fields);
				store.ConfirmationEmailStore = FieldsValueOrEmpty("confirmationEmailStore", fields);
				store.ConfirmationEmailCustomer = FieldsValueOrEmpty("confirmationEmailCustomer", fields);
				store.OnlinePaymentEmailStore = FieldsValueOrEmpty("onlinePaymentEmailStore", fields);
				store.OnlinePaymentEmailCustomer = FieldsValueOrEmpty("onlinePaymentEmailCustomer", fields);
				store.OfflinePaymentEmailStore = FieldsValueOrEmpty("offlinePaymentEmailStore", fields);
				store.OfflinePaymentEmailCustomer = FieldsValueOrEmpty("offlinePaymentEmailCustomer", fields);
				store.PaymentFailedEmailStore = FieldsValueOrEmpty("paymentFailedEmailStore", fields);
				store.PaymentFailedEmailCustomer = FieldsValueOrEmpty("paymentFailedEmailCustomer", fields);
				store.DispatchedEmailStore = FieldsValueOrEmpty("dispatchedEmailStore", fields);
				store.DispatchEmailCustomer = FieldsValueOrEmpty("dispatchedEmailCustomer", fields);
				store.CancelEmailStore = FieldsValueOrEmpty("cancelEmailStore", fields);
				store.CancelEmailCustomer = FieldsValueOrEmpty("cancelEmailCustomer", fields);
				store.ClosedEmailStore = FieldsValueOrEmpty("closedEmailStore", fields);
				store.ClosedEmailCustomer = FieldsValueOrEmpty("closedEmailCustomer", fields);
				store.PendingEmailStore = FieldsValueOrEmpty("pendingEmailStore", fields);
				store.PendingEmailCustomer = FieldsValueOrEmpty("pendingEmailCustomer", fields);
				store.TemporaryOutOfStockEmailStore = FieldsValueOrEmpty("temporaryOutOfStockEmailStore", fields);
				store.TemporaryOutOfStockEmailCustomer = FieldsValueOrEmpty("temporaryOutOfStockEmailCustomer", fields);
				store.UndeliverableEmailStore = FieldsValueOrEmpty("undeliverableEmailStore", fields);
				store.UndeliverableEmailCustomer = FieldsValueOrEmpty("undeliverableEmailCustomer", fields);
				store.ReturnedEmailStore = FieldsValueOrEmpty("returnEmailStore", fields);
				store.ReturnedEmailCustomer = FieldsValueOrEmpty("returnEmailCustomer", fields);
			}

			private class Currency : ICurrency
			{
				public Currency(string umbracoStorageString)
				{
					var split = umbracoStorageString.Split('|');
					ISOCurrencySymbol = split[0];
					if (split.Length > 1 && !string.IsNullOrEmpty(ISOCurrencySymbol))
					{
						decimal ratio;
						Ratio = decimal.TryParse(split[1].Replace(',','.'), NumberStyles.Float, new CultureInfo("en-US"), out ratio) ? ratio : 1;
						var culture = IO.Container.Resolve<IDefaultCurrencyCultureService>().GetCultureForCurrency(ISOCurrencySymbol) ?? CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(c => new RegionInfo(c.LCID).ISOCurrencySymbol == ISOCurrencySymbol);
						CurrencySymbol = culture != null ? new RegionInfo(culture.LCID).CurrencySymbol : "?";
					}
				}

				public Currency()
				{
					
				}

				public string ISOCurrencySymbol { get;  set; }
				public string CurrencySymbol { get; set; }
				public decimal Ratio { get;  set; }
			}

			private string FieldsValueOrEmpty(string propertyAlias, IPropertyProvider fields)
			{
				if (fields.ContainsKey(propertyAlias))
				{
					return fields.GetStringValue(propertyAlias);
				}
				return string.Empty;
			}

			public override string TypeAlias
			{
				get { return Store.NodeAlias; }
			}
		}
	}
}