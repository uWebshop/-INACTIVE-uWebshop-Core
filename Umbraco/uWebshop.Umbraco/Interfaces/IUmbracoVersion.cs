using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Core.Models;

namespace uWebshop.Umbraco.Interfaces
{
	public interface IUmbracoVersion
	{
		IDataTypeDefinition CreateDataTypeDefinition(int parentId, string alias, Guid legacyGuid);
		IDataTypeDefinition GetDataTypeDefinition(string alias, Guid legacyGuid);

		bool IsBackendUserAuthenticated { get; }
	
	}
}