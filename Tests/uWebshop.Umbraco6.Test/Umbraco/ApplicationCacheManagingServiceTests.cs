using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace uWebshop.Test.Services
{
	[TestFixture]
	public class ApplicationCacheManagingServiceTests
	{
		[Ignore] // too long running and not asserting anything, used for debug stepping
		[Test]
		public void AreBothCategoryCachesCleared_FunctionalDebugCache20150316()
		{
			IOC.IntegrationTest();
			IOC.CategoryService.Actual();
			var service = IOC.ApplicationCacheManagingService.Actual().Resolve();
			service.Initialize();

			service.ReloadEntityWithGlobalId(1, string.Empty);

			Thread.Sleep(10500);
		}
	}
}
