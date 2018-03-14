using uWebshop.Domain.Interfaces;
using System.Linq;
using Umbraco.Core.Models;
using uWebshop.Domain;
using Umbraco.Web;
using System;
using System.Collections;
using System.Collections.Generic;

namespace uWebshop.Umbraco.Repositories
{
	public class UmbracoNodePropertyProvider : IPropertyProvider
	{
        private readonly IPublishedContent _node;

		public UmbracoNodePropertyProvider(IPublishedContent node)
		{
			_node = node;
		}

		public bool ContainsKey(string property)
		{
            property = property.ToLowerInvariant();
			// todo: check efficiency
			return _node.Properties.Any(p => p.PropertyTypeAlias != null && p.PropertyTypeAlias.ToLowerInvariant() == property);
		}

		public bool UpdateValueIfPropertyPresent(string property, ref string value)
		{
            property = property.ToLowerInvariant();
			// todo: check efficiency
            var prop = _node.Properties.FirstOrDefault(p => p.PropertyTypeAlias != null && p.PropertyTypeAlias.ToLowerInvariant() == property);
			if (prop != null && prop.Value != null)
			{

                value = prop.Value.ToString();
				return true;
			}
			return false;
		}

		public string GetStringValue(string property)
		{
            var oldProp = property;
            property = property.ToLowerInvariant();
			// todo: check efficiency
			var prop = _node.Properties.FirstOrDefault(p => p.PropertyTypeAlias != null && p.PropertyTypeAlias.ToLowerInvariant() == property);

            if (prop != null && prop.Value != null)
            {
                var value = prop.Value;

                try
                {
                    if (prop.Value.GetType().Name == "XmlPublishedContent")
                    {
                        var propNode = (IPublishedContent)prop.Value;
                        if (propNode != null)
                        {
                            value = propNode.Id;
                        }
                    } else
                    {
                        if (IsList(prop.Value))
                        {
                            var propList = (List<IPublishedContent>)prop.Value;

                            if (propList.Any())
                            {
                                value = string.Join(",", propList.Select(x => x.Id));
                            } else
                            {
                                value = string.Empty;
                            }

                        }
                    }
                }
                catch(Exception ex) {
                    Log.Instance.LogDebug("GetStringValue Failed! Message: " + ex.Message);
                }

                return value.ToString();
            }

            return null;
         
		}

        private bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

    }
}
