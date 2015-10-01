using System;
using Umbraco.Core.Models;

namespace uWebshop.Umbraco.Interfaces
{
	public interface IUmbracoVersion
	{
		IDataTypeDefinition CreateDataTypeDefinition(int parentId, string alias);
		IDataTypeDefinition GetDataTypeDefinition(string alias);

		bool IsBackendUserAuthenticated { get; }
	
	}
}