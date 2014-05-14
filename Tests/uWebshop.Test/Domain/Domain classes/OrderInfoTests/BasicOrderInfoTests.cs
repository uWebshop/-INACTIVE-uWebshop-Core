using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;

namespace uWebshop.Test.Domain.Domain_classes.OrderInfoTests
{
	[TestFixture]
	public class BasicOrderInfoTests
	{
		private OrderInfo _orderInfo;

		[SetUp]
		public void Setup()
		{
			_orderInfo = DefaultFactoriesAndSharedFunctionality.CreateOrderInfo();
		}

		[Test]
		public void PaidSet_ToTrue_ShouldSetPaidDate()
		{
			_orderInfo.Paid = true;

			Assert.NotNull(_orderInfo.PaidDate);
		}

		[Test]
		public void PaidSet_ToFalse_ShouldNullPaidDate()
		{
			_orderInfo.Paid = false;

			Assert.Null(_orderInfo.PaidDate);
		}
	}
}