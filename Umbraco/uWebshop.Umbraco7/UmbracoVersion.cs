using System;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using uWebshop.Umbraco.Interfaces;

namespace uWebshop.Umbraco7
{
	internal class UmbracoVersion : IUmbracoVersion
	{
		public IDataTypeDefinition CreateDataTypeDefinition(int parentId, string alias, Guid legacyGuid)
		{
			return new DataTypeDefinition(parentId, alias);
		}

		public IDataTypeDefinition GetDataTypeDefinition(string alias, Guid legacyGuid)
		{
			var dataTypeService = ApplicationContext.Current.Services.DataTypeService;
			return dataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(alias).FirstOrDefault();
		}
	}
}