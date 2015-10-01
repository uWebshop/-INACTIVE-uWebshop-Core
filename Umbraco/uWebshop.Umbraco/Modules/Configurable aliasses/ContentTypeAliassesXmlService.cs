using System.Web;
using uWebshop.Domain.Helpers;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Umbraco.Interfaces
{
	internal class ContentTypeAliassesXmlService : IContentTypeAliassesXmlService
	{
		public UwebshopAliassesXMLConfig Get()
		{
			const string path = "/App_Plugins/uWebshop/config/ContentMapping.config";
			if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(path)))
			{
				return new UwebshopAliassesXMLConfig();
			}

			return DomainHelper.DeserializeXmlStringToObject<UwebshopAliassesXMLConfig>(System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(path)));
		}
	}
}