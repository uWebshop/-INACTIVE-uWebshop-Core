using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using umbraco;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Newtonsoft.Json;

namespace uWebshop.Umbraco.Mvc
{
    [PluginController("uWebshop")]
    public class BasketApiController : UmbracoApiController
    {

        public object Add(FormDataCollection form)
        {

            if (!string.IsNullOrEmpty(form.Get("storeAlias"))) {
                API.Store.SetStore(form.Get("storeAlias"));
            }

            var store = API.Store.GetStore();

            if (store != null)
            {
                Thread.CurrentThread.CurrentCulture = store.CultureInfo;
                Thread.CurrentThread.CurrentUICulture = store.CultureInfo;
            }

            var successFailed = new Dictionary<string, object>();

            try
            {

                var nameValue = new NameValueCollection();

                foreach (var item in form)
                {
                    nameValue.Add(item.Key, item.Value);
                }

                var redirectAfterHandle = new BasketRequestHandler().HandleBasketRequest(nameValue, HttpContext.Current.Request.UrlReferrer);

                var lastItem = redirectAfterHandle.Last();

                if (!successFailed.ContainsKey(lastItem.Action))
                {
                    successFailed.Add("success", redirectAfterHandle.All(x => x.Success));
                    successFailed.Add("validated", redirectAfterHandle.All(x => x.Validated));

                    var messageDictionary = new Dictionary<string, string>();

                    foreach (var message in redirectAfterHandle.SelectMany(handleItem => handleItem.Messages.Where(message => !messageDictionary.ContainsKey(message.Key))))
                    {
                        messageDictionary.Add(message.Key, message.Value);
                    }

                    successFailed.Add("messages", messageDictionary);
                    successFailed.Add("url", lastItem.Url);
                    successFailed.Add("item", lastItem.Item);
                }
            }
            catch (Exception ex)
            {
                successFailed = new Dictionary<string, object>();

                var messages = new Dictionary<string, string> { { "exception", ex.ToString() } };

                successFailed.Add("success", false);
                successFailed.Add("validated", false);
                successFailed.Add("url", string.Empty);
                successFailed.Add("error", "something went wrong with the Handle method: ");
                successFailed.Add("messages", messages);

                Log.Instance.LogError(ex,"BasketApiController Error!");
            }

            return successFailed;

        }

    }
}
