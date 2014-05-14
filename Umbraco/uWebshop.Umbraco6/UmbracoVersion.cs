using System;
using Umbraco.Core;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Core.Models;

namespace uWebshop.Umbraco6
{
	internal class UmbracoVersion : IUmbracoVersion
	{
		public IDataTypeDefinition CreateDataTypeDefinition(int parentId, string alias, Guid legacyGuid)
		{
			return new DataTypeDefinition(-1, legacyGuid);
		}

		public IDataTypeDefinition GetDataTypeDefinition(string alias, Guid legacyGuid)
		{
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
			return dataTypeService.GetDataTypeDefinitionById(legacyGuid);
		}
	}
}