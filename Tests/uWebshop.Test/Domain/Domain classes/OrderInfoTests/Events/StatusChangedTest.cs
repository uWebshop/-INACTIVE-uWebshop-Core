using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Common;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes
{
	[TestFixture]
	public class StatusChangedTest
	{
		[Test]
		public void BeforeStatusChangedShouldBeCalledHavingOldStatusValue()
		{
			// Arrange
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
			orderInfo.Status = OrderStatus.Cancelled;
			var fromEvent = OrderStatus.Closed;
			OrderInfo.BeforeStatusChanged += (info, args) => { fromEvent = info.Status; };
			orderInfo.EventsOn = true;

			// Act
			orderInfo.Status = OrderStatus.Confirmed;

			// Assert
			Assert.AreEqual(OrderStatus.Cancelled, fromEvent);
			Assert.AreEqual(OrderStatus.Confirmed, orderInfo.Status);
		}

		[Test]
		public void BeforeStatusChangedShouldBeCalledHavingNewStatusValue()
		{
			// Arrange
			var orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
			orderInfo.Status = OrderStatus.Cancelled;
			var fromEvent = OrderStatus.Closed;
			OrderInfo.AfterStatusChanged += (info, args) => { fromEvent = info.Status; };
			orderInfo.EventsOn = true;

			// Act
			orderInfo.Status = OrderStatus.Confirmed;

			// Assert
			Assert.AreEqual(OrderStatus.Confirmed, fromEvent);
			Assert.AreEqual(OrderStatus.Confirmed, orderInfo.Status);
		}
	}
}