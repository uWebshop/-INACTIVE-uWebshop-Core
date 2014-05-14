using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Businesslogic
{
	[TestFixture]
	public class UwebshopRequestTests
	{
		[Test]
		public void PersistingStoreUrl()
		{
			IOC.UnitTest();
			var store = new Store();
			UwebshopRequest.Current.SetStoreUrl(store, "abc");

			var actual = UwebshopRequest.Current.GetStoreUrl(store);

			Assert.AreEqual("abc", actual);
		}

		[Test]
		public void Current_ShouldReturnValueFromService()
		{
			IOC.UnitTest();
			//var service = MockConstructors.CreateMockUwebshopRequestService();
			//IOC.UwebshopRequestService.Use(service);

			Assert.AreEqual(UwebshopRequest.Service.Current, UwebshopRequest.Current);
		}
	}
}