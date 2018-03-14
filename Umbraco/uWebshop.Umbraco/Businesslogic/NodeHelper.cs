using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace uWebshop.Umbraco.Businesslogic
{
	public static class NodeHelper
	{
		public static string GetStoreProperty(IPublishedContent item, string field, string storeAlias)
		{
			if (item.HasProperty(field + "_" + storeAlias))
			{
				var fieldValue = item.GetPropertyValue<string>(field + "_" + storeAlias);

				// temp fix for 66north  2 disable fields. 'disable' && 'disable_IS'
				if (storeAlias.ToLower() == "is" && (string.IsNullOrEmpty(fieldValue) ||
													 fieldValue == "0"))
				{
					fieldValue = item.GetPropertyValue<string>(field);
				}

				return fieldValue;
			}
			else
			{
				return item.HasProperty(field) ? item.GetPropertyValue<string>(field) : "";
			}
		}

		public static string GetStoreProperty(IContent item, string field, string storeAlias)
		{
			if (item.HasProperty(field + "_" + storeAlias))
			{
				var fieldValue = item.GetValue<string>(field + "_" + storeAlias);

				// temp fix for 66north  2 disable fields. 'disable' && 'disable_IS'
				if (storeAlias.ToLower() == "is" && (string.IsNullOrEmpty(fieldValue) ||
													 fieldValue == "0"))
				{
					fieldValue = item.GetValue<string>(field);
				}

				return fieldValue;
			}
			else
			{
				return item.HasProperty(field) ? item.GetValue<string>(field) : "";
			}
		}
	}
}
