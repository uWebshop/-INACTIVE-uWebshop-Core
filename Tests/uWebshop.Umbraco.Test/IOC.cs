using uWebshop.Domain.Interfaces;
using uWebshop.Test;
using uWebshop.Umbraco.Repositories;
using uWebshop.Umbraco.Services;

namespace uWebshop.Umbraco.Test
{
	internal static class IOCBuilderExtensions
	{
		// specific factory methods
		public static IOCBuilder<IProductRepository> Actual(this IOCBuilder<IProductRepository> iocBuilder)
		{
			iocBuilder.UseType<UmbracoProductRepository>();
			return iocBuilder;
		}

		public static IOCBuilder<IStoreService> Actual(this IOCBuilder<IStoreService> iocBuilder)
		{
			iocBuilder.UseType<UmbracoStoreService>();
			return iocBuilder;
		}
	}
}