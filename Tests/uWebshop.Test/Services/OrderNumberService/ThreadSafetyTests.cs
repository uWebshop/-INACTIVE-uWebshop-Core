using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;

namespace uWebshop.Test.Services.OrderNumberService
{
	[TestFixture]
	public class ThreadSafetyTests
	{
		[Test]
		public void TestAgainstRaceHazard()
		{
			IOC.UnitTest();
			var service = IOC.OrderNumberService.Actual().Resolve();
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo();

			var transactionFailed = false;
			var threadsActive = 0;
			// todo: how to get this deterministic (force which thread goes first without delaying the test or forcing total execution order)
			new Thread(() =>
				{
					using (service.GetTransaction(order))
					{
						if (++threadsActive > 1) transactionFailed = true;
						Console.WriteLine("Hi from thread 2");
						Console.WriteLine("Bye from thread 2");
						threadsActive--;
					}
				}).Start();

			using (service.GetTransaction(order))
			{
				if (++threadsActive > 1) transactionFailed = true;
				Console.WriteLine("Hi from thread 1");
				Thread.Sleep(10);
				threadsActive--;
				Console.WriteLine("Bye from thread 1");
			}

			service.GenerateAndPersistOrderNumber(order);
			Thread.Sleep(10);
			service.GenerateAndPersistOrderNumber(order);

			Console.WriteLine(order.OrderNumber);
			Assert.False(transactionFailed);
		}

		[Test]
		public void IntegrationTest()
		{
			IOC.UnitTest();
			var mock = IOC.OrderRepository.SetupNewMock();
			//mock.Setup(m => m.AssignNewOrderNumberToOrder(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>())).Returns(5);
			var service = IOC.OrderNumberService.Actual().Resolve();
			Assert.False(UwebshopConfiguration.Current.ShareBasketBetweenStores);
			var order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo();
			order.DatabaseId = 1234;
			Guid g = new Guid();
			order.UniqueOrderId = g;
			order.StoreInfo.Alias = StoreHelper.CurrentStoreAlias;

			service.GenerateAndPersistOrderNumber(order);

			mock.Verify(m => m.AssignNewOrderNumberToOrder(1234, StoreHelper.CurrentStoreAlias, It.IsAny<int>()));
			mock.Verify(m => m.SetOrderNumber(g, "0000", StoreHelper.CurrentStoreAlias, It.IsAny<int>()));

		}
	}
}
