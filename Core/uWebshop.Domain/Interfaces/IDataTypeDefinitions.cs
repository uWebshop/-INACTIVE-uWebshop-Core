using System.Collections.Generic;
using uWebshop.Common;
using uWebshop.Domain.ContentTypes;

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
		public string DefinitionGuid;
		public string Name;
		public DatabaseType Type;
		public List<string> PreValues;
	}
}
