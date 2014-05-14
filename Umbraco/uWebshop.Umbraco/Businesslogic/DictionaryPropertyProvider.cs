using System.Collections.Generic;
using Examine;
using uWebshop.Domain.Interfaces;
using System.Linq;

namespace uWebshop.Domain
{
	/// <summary>
	/// 
	/// </summary>
	public class DictionaryPropertyProvider : IPropertyProvider
	{
		private readonly IDictionary<string, string> _properties;
		private readonly SearchResult _searchResult;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryPropertyProvider"/> class.
		/// </summary>
		/// <param name="searchResult">The search result.</param>
		public DictionaryPropertyProvider(SearchResult searchResult)
		{
			_searchResult = searchResult;
		}

		/// <summary>
		/// Determines whether the specified property contains key.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public bool ContainsKey(string property)
		{
			// todo: check efficiency
			property = property.ToLowerInvariant();
			return _searchResult.Fields.Keys.Any(k => k.ToLowerInvariant() == property);
		}

		/// <summary>
		/// Updates the value difference property present.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool UpdateValueIfPropertyPresent(string property, ref string value)
		{
			if (ContainsKey(property))
			{
				value = GetStringValue(property);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the string value.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns></returns>
		public string GetStringValue(string property)
		{
			// todo: check efficiency
			property = property.ToLowerInvariant();
			return _searchResult.Fields.Single(kv => kv.Key.ToLowerInvariant() == property).Value;
		}
	}
}