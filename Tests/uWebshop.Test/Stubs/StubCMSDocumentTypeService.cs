using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	public class StubCMSDocumentTypeService : ICMSDocumentTypeService
	{
		public IDocumentTypeInfo GetByAlias(string alias)
		{
			return new StubDocumentTypeInfo();
		}
	}
}