using uWebshop.Domain.Interfaces;
using umbraco.NodeFactory;
using System.Linq;

namespace uWebshop.Umbraco.Repositories
{
	public class UmbracoNodePropertyProvider : IPropertyProvider
	{
		private readonly Node _node;

		public UmbracoNodePropertyProvider(Node node)
		{
			_node = node;
		}

		public bool ContainsKey(string property)
		{
			property = property.ToLowerInvariant();
			// todo: check efficiency
			return _node.PropertiesAsList.Any(p => p.Alias.ToLowerInvariant() == property);
		}

		public bool UpdateValueIfPropertyPresent(string property, ref string value)
		{
			property = property.ToLowerInvariant();
			// todo: check efficiency
			var prop = _node.PropertiesAsList.FirstOrDefault(p => p.Alias.ToLowerInvariant() == property);
			if (prop != null)
			{
				value = prop.Value;
				return true;
			}
			return false;
		}

		public string GetStringValue(string property)
		{
			property = property.ToLowerInvariant();
			// todo: check efficiency
			var prop = _node.PropertiesAsList.FirstOrDefault(p => p.Alias.ToLowerInvariant() == property);
			return prop != null ? prop.Value : null;
		}
	}
}