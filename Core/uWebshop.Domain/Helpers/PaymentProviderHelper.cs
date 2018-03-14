using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Umbraco.Web;
using uWebshop.Common;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Helpers
{
    /// <summary>
    /// Helper class with payment provider related functions
    /// </summary>
    public static class PaymentProviderHelper
    {
        /// <summary>
        /// Returns a list of all payment providers
        /// </summary>
        /// <returns>
        /// List of payment providers
        /// </returns>
        public static List<IPaymentProvider> GetAllIPaymentProviders()
        {
            var toReturn = new List<IPaymentProvider>();

            // Get custom providers from dlls
            toReturn.AddRange(GetInterfaces<IPaymentProvider>());

            // Add dummy providers from nodes
            //var paymentProviders = new OrderRepository().GetAllPaymentProviders().Where(x => x.Type == Common.PaymentProviderType.OfflinePaymentAtCustomer || x.Type == Common.PaymentProviderType.OfflinePaymentInStore);

            //foreach (var paymentProvider in paymentProviders)
            //{
            //    toReturn.Add(new DummyPaymentProvider(paymentProvider.Id));
            //}

            return toReturn;
        }

        /// <summary>
        /// Gets all payment providers.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PaymentProvider> GetAllPaymentProviders(string storeAlias = null, string currencyCode = null)
        {
            return IO.Container.Resolve<IPaymentProviderService>().GetAll(StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode)).Where(x => !x.Disabled);
        }

        /// <summary>
        /// Returns a list of all payment providers
        /// </summary>
        /// <returns>
        /// List of payment request handlers
        /// </returns>
        public static List<IPaymentRequestHandler> GetAllPaymentRequestHandlers()
        {
            return GetInterfaces<IPaymentRequestHandler>();
        }

        /// <summary>
        /// Returns a list of all payment providers
        /// </summary>
        /// <returns>
        /// List of payment response handlers
        /// </returns>
        public static List<IPaymentResponseHandler> GetAllPaymentResponseHandlers()
        {
            return GetInterfaces<IPaymentResponseHandler>();
        }

        /// <summary>
        /// Returns the paymentmethods available for the current order
        /// If the customer has not chosen a country yet, it will use the countrycode from the shop itself to return the
        /// providers.
        /// </summary>
        /// <param name="useCountry">if set to <c>true</c> [use country].</param>
        /// <returns></returns>
        public static List<PaymentProvider> GetPaymentProvidersForOrder(bool useCountry = true)
        {
            return GetPaymentProvidersForOrder(OrderHelper.GetOrder(), useCountry);
        }

        internal static List<PaymentProvider> GetBillingProvidersForOrder(OrderInfo orderInfo, bool useZone = true, string storeAlias = null, string currencyCode = null)
        {
            return GetBillingProvidersForOrder(orderInfo, useZone, StoreHelper.GetLocalizationOrCurrent(storeAlias, currencyCode));
        }
        internal static List<PaymentProvider> GetBillingProvidersForOrder(OrderInfo orderInfo, bool useZone, ILocalization localization)
        {
            if (orderInfo == null)
            {
                Log.Instance.LogError("GetBillingProvidersForOrder: Asking for payment without order");
                return new List<PaymentProvider>();
            }

            var paymentProviders = IO.Container.Resolve<IPaymentProviderService>().GetAll(localization);


            if (useZone)
            {
                var paymentCountry = orderInfo.CustomerInfo.CountryCode;

                if (string.IsNullOrEmpty(paymentCountry))
                {
                    paymentCountry = localization.Store.CountryCode;


                }
                paymentProviders = paymentProviders.Where(paymentProvider => paymentProvider.Type == PaymentProviderType.OfflinePaymentInStore || paymentProvider.Zones.SelectMany(x => x.CountryCodes).Contains(paymentCountry)).Distinct().ToList();

            }

            var paymentProvidersList = paymentProviders.ToList();
            // Sort payment providers by node sort order in Umbraco backend
            paymentProvidersList.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));

            return paymentProvidersList;
        }

        /// <summary>
        /// Returns the paymentmethods available for the current order
        /// If the customer has not chosen a country yet, it will use the countrycode from the shop itself to return the
        /// providers.
        /// </summary>
        /// <param name="orderInfo">The order information.</param>
        /// <param name="useCountry">if set to <c>true</c> [use country].</param>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        public static List<PaymentProvider> GetPaymentProvidersForOrder(OrderInfo orderInfo, bool useCountry = true, Store store = null)
        {
            var storeAlias = store == null ? null : store.Alias;
            return GetBillingProvidersForOrder(orderInfo, useCountry, StoreHelper.GetLocalizationOrCurrent(storeAlias, null));
        }

        private static List<T> GetInterfaces<T>()
        {
            var instances = new List<T>();

            var targetType = typeof(T);

            foreach (var paymentProvider in GetAllPaymentProviders())
            {
                var dllName = paymentProvider.DLLName;

                if (string.IsNullOrEmpty(dllName))
                {
                    dllName = string.Format("uWebshop.Payment.{0}.dll", paymentProvider.Name);
                }

                var assemblyPathName = string.Format(@"{0}\{1}", HttpContext.Current.Server.MapPath("/bin"), dllName);

                if (System.IO.File.Exists(assemblyPathName))
                {
                    var assembly = Assembly.LoadFrom(assemblyPathName);

                    if (assembly != null)
                    {
                        var types = assembly.GetExportedTypes();

                        foreach (var type in types)
                        {
                            if (!targetType.IsAssignableFrom(type)) continue;
                            var operation = (T)Activator.CreateInstance(type);

                            instances.Add(operation);
                        }
                    }
                }
            }

            return instances;
        }

        /// <summary>
        /// Gets the payment zones.
        /// </summary>
        /// <param name="countryCode">The country code.</param>
        /// <returns></returns>
        public static List<Zone> GetPaymentZones(string countryCode)
        {
            return IO.Container.Resolve<IZoneService>().GetAllPaymentZones(StoreHelper.CurrentLocalization).Where(x => x.CountryCodes.Contains(countryCode)).ToList();
        }

        /// <summary>
        /// Generates the base URL.
        /// </summary>
        /// <param name="nodeId">The node unique identifier.</param>
        /// <returns></returns>
        [Obsolete("Use GenerateBaseUrl() since NodeId is not used anymore")]
        public static string GenerateBaseUrl(int nodeId = 0)
        {
            return GenerateBaseUrl();
            //return IO.Container.Resolve<ICMSContentService>().GenerateDomainUrlForContent(nodeId);
        }

        /// <summary>
        /// Generate the base Url to use for payment providers
        /// based on the currentNodeId or the current CMS node
        /// </summary>
        /// <returns></returns>
        public static string GenerateBaseUrl()
        {
            var http = "http://";
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                http = "https://";
            }

            var currentDomain = HttpContext.Current.Request.Url.Authority;
            var baseUrl = string.Format("{0}{1}", http, currentDomain);

            Log.Instance.LogDebug("GenerateBaseUrl to return" + baseUrl);

            return baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Generate the base Url to use for payment providers
        /// based on the currentNodeId or the current CMS node
        /// Includes culture path from UmbracoDomain if present
        /// </summary>
        /// <returns></returns>
        public static string GenerateBaseUrlWithCulturePath()
        {
            var http = "http://";
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                http = "https://";
            }

            var currentDomain = UmbracoContext.Current.PublishedContentRequest.UmbracoDomain.DomainName;
            var baseUrl = string.Format("{0}{1}", http, currentDomain);

            Log.Instance.LogDebug("GenerateBaseUrl to return" + baseUrl);

            return baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Gets the request parameter with name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string Request(string name)
        {
            var context = HttpContext.Current;
            return context == null ? string.Empty : context.Request[name];
        }

        /// <summary>
        /// Gets the URL for content with unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns></returns>
        public static string GetUrlForContentWithId(int id)
        {
            return IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(id);
        }

        /// <summary>
        /// Gets the current node unique identifier.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentNodeId()
        {
            return IO.Container.Resolve<ICMSApplication>().CurrentNodeId();
        }

        /// <summary>
        /// Sets the transaction unique identifier on the order and persists it immediately to the database.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="transactionId">The transaction unique identifier.</param>
        public static void SetTransactionId(OrderInfo order, string transactionId)
        {
            order.PaymentInfo.TransactionId = transactionId;
            IO.Container.Resolve<IOrderRepository>().SetTransactionId(order.UniqueOrderId, transactionId);
        }

        public static void ClearValidationResult(OrderInfo order)
        {
            if (HttpContext.Current.Session[Constants.PaymentValidationResultsKey + order.UniqueOrderId] != null)
                HttpContext.Current.Session.Remove(Constants.PaymentValidationResultsKey + order.UniqueOrderId);
        }

        public static void AddValidationResult(OrderInfo order, int id, string key, string value, string alias = null, string name = null)
        {
            var results = GetPaymentValidationResults(order);
            results.Add(new OrderValidationError { Id = id, Key = key, Value = value, Alias = alias, Name = name });
            HttpContext.Current.Session.Add(Constants.PaymentValidationResultsKey + order.UniqueOrderId, results);
        }
        internal static List<OrderValidationError> GetPaymentValidationResults(OrderInfo order)
        {
            if (HttpContext.Current.Session[Constants.PaymentValidationResultsKey + order.UniqueOrderId] != null)
                return (List<OrderValidationError>)HttpContext.Current.Session[Constants.PaymentValidationResultsKey + order.UniqueOrderId];
            return new List<OrderValidationError>();
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public static class PaymentProviderExtensions
    {

        public static string GetSetting(this PaymentProvider paymentProvider, string helperKey)
        {
            var helper = new PaymentConfigHelper(paymentProvider);

            var value = helper.Settings[helperKey];

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            Log.Instance.LogError(paymentProvider.Name + ": Missing or empty PaymentProvider.Config field with Key: " + helperKey);
            return string.Empty;
        }

        public static XElement GetSettingsXML(this PaymentProvider paymentProvider)
        {

            var helper = new PaymentConfigHelper(paymentProvider);

            return helper.Settings.LoadProviderSettingsXML();

        }

        /// <summary>
        /// Successes the node URL.
        /// </summary>
        /// <param name="paymentProvider">The payment provider.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string SuccessUrl(this PaymentProvider paymentProvider)
        {

            var baseUrl = PaymentProviderHelper.GenerateBaseUrl();

            var successNodeIdAsString = paymentProvider.SuccesNodeId;

            int successNodeId;
            int.TryParse(successNodeIdAsString, out successNodeId);

            if (successNodeId != 0)
            {
                var successUrl = IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(successNodeId);
                if (!successUrl.StartsWith("http"))
                {
                    successUrl = string.Format("{0}/{1}", baseUrl, successUrl.TrimStart('/'));
                }

                return successUrl;
            }

            return baseUrl;
        }

        /// <summary>
        /// Successes the node URL.
        /// </summary>
        /// <param name="paymentProvider">The payment provider.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string ErrorUrl(this PaymentProvider paymentProvider)
        {
            var baseUrl = PaymentProviderHelper.GenerateBaseUrl();

            var successNodeIdAsString = paymentProvider.ErrorNodeId;

            int errorNodeId;
            int.TryParse(successNodeIdAsString, out errorNodeId);

            if (errorNodeId != 0)
            {
                var errorUrl = IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(errorNodeId);
                if (!errorUrl.StartsWith("http"))
                {
                    errorUrl = string.Format("{0}/{1}", baseUrl, errorUrl.TrimStart('/'));
                }

                return errorUrl;
            }

            return baseUrl;
        }

        /// <summary>
        /// Successes the node URL.
        /// </summary>
        /// <param name="paymentProvider">The payment provider.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string CancelUrl(this PaymentProvider paymentProvider)
        {
            var baseUrl = PaymentProviderHelper.GenerateBaseUrl();

            var cancelNodeIdAsString = paymentProvider.CancelNodeId;

            int cancelNodeId;
            int.TryParse(cancelNodeIdAsString, out cancelNodeId);

            if (cancelNodeId != 0)
            {
                var cancelUrl = IO.Container.Resolve<ICMSApplication>().GetUrlForContentWithId(cancelNodeId);
                if (!cancelUrl.StartsWith("http"))
                {
                    cancelUrl = string.Format("{0}/{1}", baseUrl, cancelUrl.TrimStart('/'));
                }

                return cancelUrl;
            }

            return baseUrl;
        }

        /// <summary>
        /// Successes the node URL.
        /// </summary>
        /// <param name="paymentProvider">The payment provider.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string ReportUrl(this PaymentProvider paymentProvider)
        {
            var baseUrl = PaymentProviderHelper.GenerateBaseUrlWithCulturePath();

            var paymentProviderRepositoryCmsNodeName = IO.Container.Resolve<ICMSApplication>().GetPaymentProviderRepositoryCMSNodeUrlName() ?? "PaymentProviders";
            var paymentProviderSectionCmsNodeName = IO.Container.Resolve<ICMSApplication>().GetPaymentProviderSectionCMSNodeUrlName() ?? "PaymentProviders";
            var paymentProviderPath = string.Format("/{0}/{1}/", paymentProviderRepositoryCmsNodeName, paymentProviderSectionCmsNodeName);

            // http://domain.com/paymentproviders/payentproviders/ogone

            return string.Format("{0}{1}{2}", baseUrl, paymentProviderPath, paymentProvider.Name);
        }
    }
}
