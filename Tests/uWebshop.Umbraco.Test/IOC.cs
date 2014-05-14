using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain.Interfaces;
using uWebshop.Test;
using uWebshop.Umbraco.Interfaces;
using uWebshop.Umbraco.Repositories;
using uWebshop.Umbraco.Services;

namespace uWebshop.Umbraco.Test
{
	//internal static class IOC
	//{
	//	public static void UnitTest()
	//	{
	//		uWebshop.Test.IOC.UnitTest();
	//	}
	//}

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

		//public static IOCBuilder<IUmbracoDocumentTypeInstaller> Actual(this IOCBuilder<IUmbracoDocumentTypeInstaller> iocBuilder) { iocBuilder.UseType<Umbraco6.UmbracoDocumentTypeInstaller>(); return iocBuilder; }
	}
}