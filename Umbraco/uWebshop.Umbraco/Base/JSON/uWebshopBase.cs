using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Description;
using umbraco;
using Umbraco.Web.BaseRest;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.NewtonsoftJsonNet;
using uWebshop.Newtonsoft.Json;
using Formatting = uWebshop.Newtonsoft.Json.Formatting;

namespace uWebshop.API.JSON
{
	[RestExtension("uWebshopBase")]
	public class uWebshopBase
	{
		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		[RestExtensionMethod(AllowAll =  true, ReturnXml = false)]
		public static void JSON(string classAndFuction)
		{
			string qs = null;

			if (HttpContext.Current.Request.QueryString.AllKeys.Any())
			{
				qs = HttpContext.Current.Request.QueryString[0];
			}
			
			JSONXMLRender.RenderAndOutput(classAndFuction, qs);
		}

		/// <summary>
		/// Validate the full order
		/// </summary>
		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [RestExtensionMethod(AllowAll = true, ReturnXml = false)]
		public static void Handle()
		{
			var successFailed = new Dictionary<string, object>();

			try
			{
				

				var qs = HttpContext.Current.Request.QueryString[0];

				var dictionaryList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(qs);

				var nameValue = new NameValueCollection();

				foreach (var item in dictionaryList)
				{
					nameValue.Add(item.First().Value, item.Last().Value);
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

				var messages = new Dictionary<string, string> {{"exception", ex.ToString()}};
				
				successFailed.Add("success", false);
				successFailed.Add("validated", false);
				successFailed.Add("url", string.Empty);
				successFailed.Add("error", "something went wrong with the Handle method: ");
				successFailed.Add("messages", messages);
			}

			var serializeSettings = new JsonSerializerSettings()
			                        {
				                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
										Formatting = GlobalSettings.DebugMode ? Formatting.Indented : Formatting.None
			                        };

			var json = JsonConvert.SerializeObject(successFailed, Formatting.Indented, serializeSettings);
			HttpContext.Current.Response.ContentType = "application/json";
			HttpContext.Current.Response.Write(json);
		}

	}
}
