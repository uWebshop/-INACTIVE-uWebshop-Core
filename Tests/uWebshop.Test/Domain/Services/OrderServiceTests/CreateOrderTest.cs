using System;
using NUnit.Framework;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Stubs;

namespace uWebshop.Test.Domain.Services.OrderServiceTests
{
	[TestFixture]
	public class CreateOrderTest
	{
		private IOrderService _orderService;

		[SetUp]
		public void SetUp()
		{
			_orderService = IOC.OrderService.Actual().Resolve();
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void CreateOrder_NullStore_ThrowsException()
		{
			_orderService.CreateOrder(null);
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void CreateOrder_StoreWithEmptyAlias_ThrowsException()
		{
			_orderService.CreateOrder(new StubStore {Alias = string.Empty});
		}
	}
}