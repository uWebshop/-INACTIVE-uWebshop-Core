using System.Collections.Generic;
using uWebshop.Common;
using uWebshop.Domain.ContentTypes;
using Umbraco.Core.Models;

namespace uWebshop.Domain.Interfaces
{
	interface IDataTypeDefinitions
	{
		List<UwebshopDataTypeDefinition> LoadDataTypeDefinitions();
	}
	public class UwebshopDataTypeDefinition
	{
		public DataType DataType;
		public string Alias;
		public string KeyGuid;
		public string Name;
		public DatabaseType Type;
		public Dictionary<string, PreValue> PreValues;
	}
}
