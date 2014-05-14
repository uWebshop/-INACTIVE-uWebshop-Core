using uWebshop.Domain.Interfaces;
using uWebshop.Test;

namespace uWebshop.Umbraco6.Test
{
	internal static class IOCBuilderExtensions
	{
		// specific factory methods
		public static IOCBuilder<IUmbracoDocumentTypeInstaller> Actual(this IOCBuilder<IUmbracoDocumentTypeInstaller> iocBuilder)
		{
			iocBuilder.UseType<UmbracoDocumentTypeInstaller>();
			return iocBuilder;
		}
	}
}