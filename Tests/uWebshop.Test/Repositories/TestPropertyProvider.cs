using System.Collections.Generic;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Repositories
{
	public class TestPropertyProvider : IPropertyProvider
	{
		public Dictionary<string, string> Dictionary = new Dictionary<string, string>();

		public bool ContainsKey(string property)
		{
			return Dictionary.ContainsKey(property);
		}

		public bool UpdateValueIfPropertyPresent(string property, ref string value)
		{
			if (Dictionary.ContainsKey(property))
			{
				value = Dictionary[property];
				return true;
			}
			return false;
		}

		public string GetStringValue(string property)
		{
			return Dictionary[property];
		}
	}
}