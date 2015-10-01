using uWebshop.Domain.Interfaces;
using umbraco.NodeFactory;
using System.Linq;
using Umbraco.Core.Models;

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
			return _node.Properties.Any(p => p.PropertyTypeAlias.ToLowerInvariant() == property);
		}

		public bool UpdateValueIfPropertyPresent(string property, ref string value)
		{
			property = property.ToLowerInvariant();
			// todo: check efficiency
            var prop = _node.Properties.FirstOrDefault(p => p.PropertyTypeAlias.ToLowerInvariant() == property);
			if (prop != null)
			{
				value = prop.Value.ToString();
				return true;
			}
			return false;
		}

		public string GetStringValue(string property)
		{
			property = property.ToLowerInvariant();
			// todo: check efficiency
			var prop = _node.Properties.FirstOrDefault(p => p.PropertyTypeAlias.ToLowerInvariant() == property);
			return prop != null ? prop.Value.ToString() : null;
		}
	}
}