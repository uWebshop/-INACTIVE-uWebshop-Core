using System;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using uWebshop.Umbraco.Interfaces;
using Umbraco.Web;

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

		public bool IsBackendUserAuthenticated
		{
			get
			{
				return UmbracoContext.Current.Security.ValidateCurrentUser();
			}
		}
	}
}