using uWebshop.Umbraco.Interfaces;
using uWebshop.Umbraco.Repositories;

namespace uWebshop.Test
{
	class StubContentTypeAliassesXmlService : IContentTypeAliassesXmlService
	{
		private UwebshopAliassesXMLConfig it;
		public StubContentTypeAliassesXmlService()
		{
			it = new UwebshopAliassesXMLConfig();
			InitNodeAliasses.Initialize(it);
		}
		public UwebshopAliassesXMLConfig Get()
		{
			return it;
		}
	}
}