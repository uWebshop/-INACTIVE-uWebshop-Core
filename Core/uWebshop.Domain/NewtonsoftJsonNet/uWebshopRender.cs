using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using uWebshop.API;
using uWebshop.Newtonsoft.Json;
using Formatting = uWebshop.Newtonsoft.Json.Formatting;

namespace uWebshop.Domain.NewtonsoftJsonNet
{
	public class JSONXMLRender
	{
		internal static object Render(string className, string functionName, string qs, bool renderXMLInteadofJSON = false)
		{

			var dictionary = new Dictionary<string, string>();

			if (qs != null)
			{
				var dictionaryList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(qs);

			    foreach (var dicItem in dictionaryList)
			    {
			        if (dicItem != null)
			        {
			            var dicItemFirstKey = dicItem.First();

			            if (dicItemFirstKey.Key.ToLowerInvariant() != "key")
			            {
			                if (!dictionary.ContainsKey(dicItemFirstKey.Key))
			                {
			                    dictionary.Add(dicItemFirstKey.Key, dicItemFirstKey.Value);
			                }
			            }
			            else
			            {
                            var dicItemSecondKey = dicItem.Last();

                            if (!dictionary.ContainsKey(dicItemFirstKey.Key))
                            {
                                dictionary.Add(dicItemFirstKey.Value, dicItemSecondKey.Value);
                            }
			            }
			        }
			    }
                
			}

			var methods = new List<MethodInfo>();

			if (!string.IsNullOrEmpty(className))
			{
				var type = Type.GetType("uWebshop.API." + className + ", uWebshop.Domain, Version=" + Assembly.GetExecutingAssembly().GetName().Version + ", Culture=neutral, PublicKeyToken=null");

				if (type != null)
				{
					methods.AddRange(type.GetMethods().Where(m => m.Name.ToLowerInvariant() == functionName.ToLowerInvariant()));
				}
			}

			if (string.IsNullOrEmpty(className) || !methods.Any())
			{
				var types = new[] {typeof (Basket), typeof (API.Catalog), typeof (Customers), typeof (Discounts), typeof (Orders), typeof (Providers), typeof (API.Store)};

				methods.AddRange(types.SelectMany(t => t.GetMethods()).Where(m => m.Name.ToLowerInvariant() == functionName.ToLowerInvariant()));
			}


			MethodInfo simpleargumentMethod = null;

			if (dictionary.Keys.Any())
			{
				simpleargumentMethod = methods.FirstOrDefault(x => x.GetParameters().Any() && x.GetParameters().All(p => p.ParameterType == typeof (string) || p.ParameterType == typeof (int) || p.ParameterType == typeof (bool) || p.ParameterType == typeof (decimal)));
			}
			else
			{
				simpleargumentMethod = methods.FirstOrDefault(x => x.GetParameters().All(p => p.ParameterType == typeof(string) || p.ParameterType == typeof(int) || p.ParameterType == typeof(bool) || p.ParameterType == typeof(decimal)));
			}

			var args = new List<object>();

			
			if (simpleargumentMethod != null)
			{
				foreach (var propertyParameter in simpleargumentMethod.GetParameters())
				{
					var propertyName = propertyParameter.Name.ToLowerInvariant();
					if (dictionary.Any(x => x.Key.ToLowerInvariant() == propertyName))
					{
						var key = dictionary.Keys.FirstOrDefault(x => x.ToLowerInvariant() == propertyName);

					    if (key != null)
					    {
					        var value = dictionary[key];

					        if (propertyParameter.ParameterType == typeof (int))
					        {
					            int valueAsInt;
					            Int32.TryParse(value, out valueAsInt);

					            args.Add(valueAsInt);
					        }
					        else if (propertyParameter.ParameterType == typeof (decimal))
					        {
					            decimal valueAsDecimal;
					            Decimal.TryParse(value, out valueAsDecimal);

					            args.Add(valueAsDecimal);
					        }
					        else if (propertyParameter.ParameterType == typeof (bool))
					        {
					            args.Add(value == "1" || value.ToLowerInvariant() == "true");
					        }
					        else
					        {
					            args.Add(value);
					        }
					    }
					}
					else
					{
						args.Add(null);
					}
				}

				return simpleargumentMethod.Invoke(null, args.ToArray());
			}
			return null;
		}

		internal static void RenderAndOutput(string classAndFuction, string qs, bool renderXMLInteadofJSON = false)
		{
			var functionName = classAndFuction;
			string className = null;

			if (classAndFuction.Contains("."))
			{
				var firstDot = classAndFuction.IndexOf('.');

				className = classAndFuction.Substring(0, firstDot);
				functionName = classAndFuction.Substring(firstDot + 1);
			}
			var returnedObject = Render(className, functionName, qs, renderXMLInteadofJSON);
			RenderOutput(functionName, returnedObject);
		}

		public static string RenderOutput(string methodName, object objectToSerialze, bool renderXMLInteadofJSON = false)
		{
			var successFailedObject = new Dictionary<string, object>();

			try
			{
				if (objectToSerialze != null)
				{
					successFailedObject.Add("item", objectToSerialze);
					successFailedObject.Add("success", true);
				}
				else
				{
					successFailedObject.Add("error", "something went wrong with " + methodName);
					successFailedObject.Add("success", false);
				}

				successFailedObject.Add("messages", new Dictionary<string, string>());
				successFailedObject.Add("validated", false);
				successFailedObject.Add("url", string.Empty);
			}
			catch (Exception ex)
			{
				successFailedObject = new Dictionary<string, object>();

				var messages = new Dictionary<string, string> { { "exception", ex.ToString() } };

				successFailedObject.Add("error", "something went wrong with " + methodName);
				successFailedObject.Add("messages", messages);
				successFailedObject.Add("success", false);
				successFailedObject.Add("validated", false);
				successFailedObject.Add("url", string.Empty);
			}

			//todo: formatting NONE als umbraco NIET in debug draait
			var serializeSettings = new JsonSerializerSettings()
			                        {
				                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
										Formatting = Formatting.Indented
			                        };

			if (renderXMLInteadofJSON)
			{
				var xmlObjectAsJson = JsonConvert.SerializeObject(objectToSerialze, Formatting.Indented, serializeSettings);
				return XMLoutput(xmlObjectAsJson).OuterXml;
			}

			var json = JsonConvert.SerializeObject(successFailedObject, Formatting.Indented, serializeSettings);
			JSONoutput(json);

			return string.Empty;
		}

		/// <summary>
		/// Creates XML output based on a JSON string!
		/// </summary>
		/// <param name="json"></param>
		internal static void JSONoutput(string json)
		{
			HttpContext.Current.Response.ContentType = "application/json";
			HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
			HttpContext.Current.Response.Write(json);
			HttpContext.Current.Response.End();
		}

		/// <summary>
		/// Creates XML output based on a JSON string!
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="json"></param>
		internal static XmlDocument XMLoutput(string json)
		{
			//todo: dit kan wel eens heel handig zijn om voor alles 'snel' XML te produceren.
			//todo: vraag is alleen hoe dit performed (dubbele serialization eigenlijk hier nu...)
			return JsonConvert.DeserializeXmlNode(json, "root");
		}
	}
}
