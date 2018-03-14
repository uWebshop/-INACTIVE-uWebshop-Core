using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Core;
using uWebshop.Umbraco.Businesslogic;

namespace uWebshop.Umbraco
{
	public class UrlProvider : IUrlProvider
	{
		public string GetUrl(UmbracoContext umbracoContext, int id, Uri current, UrlProviderMode mode)
		{
			return null;
		}

		public IEnumerable<string> GetOtherUrls(UmbracoContext umbracoContext, int id, Uri current)
		{
			var list = new List<string>();

			var stores = Domain.Helpers.StoreHelper.GetAllStores();
			
			var content = umbracoContext.ContentCache.GetById(id);

			if (content != null && (content.DocumentTypeAlias == "uwbsProduct" || content.DocumentTypeAlias == "uwbsCategory"))
			{

				foreach (var store in stores)
				{
					

					StringBuilder builder = new StringBuilder();
					var insert = true;
					foreach (IPublishedContent node in content.AncestorsOrSelf().Where(x => x.DocumentTypeAlias == "uwbsProduct" || x.DocumentTypeAlias == "uwbsCategory").Reverse())
					{
						string slug = NodeHelper.GetStoreProperty(node, "url", store.Alias);

						if (!string.IsNullOrEmpty(slug))
						{
							builder.AppendFormat("/{0}", slug.ToUrlSegment().ToLowerInvariant());
						} else
						{
							insert = false;
						}
						
					}
					if (insert)
					{
						list.Add(store.StoreURL.TrimEnd("/") + builder.ToString());
					}

				}

			}

			return list;
		}
	}
}
