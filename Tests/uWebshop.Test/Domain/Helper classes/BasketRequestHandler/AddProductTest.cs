using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;
using uWebshop.Domain;
using uWebshop.Domain.Businesslogic;
using uWebshop.Domain.Helpers;
using Moq;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Mocks;

namespace uWebshop.Test.Domain.Helper_classes.AddProductTest
{
	[TestFixture]
	public class AddProductTest
	{
		private BasketRequestHandler _basketRequestHandler;
		private Mock<IOrderUpdatingService> _mock;
		private NameValueCollection _requestParameters;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_requestParameters = new NameValueCollection();
			_basketRequestHandler = new BasketRequestHandler();

			IOC.OrderUpdatingService.Mock(out _mock);
		}

		[Test]
		public void RequestWithoutProductIdAndOrderLineId_ShouldNotCallUpdateLine()
		{
			_requestParameters.Add("action", "update");

			_basketRequestHandler.AddProduct(_requestParameters, null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Never());
		}

		[Test]
		public void RequestToUpdateQuantity_ShouldCallUpdateLineOnce()
		{
			_requestParameters.Add("productId", "1");
			_requestParameters.Add("action", "update");
			_requestParameters.Add("quantity", "1");

			_basketRequestHandler.AddProduct(_requestParameters, null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
		}

		[Test]
		public void RequestWithActionUpdate_ShouldCallUpdateLineOnceWithActionUpdate()
		{
			_requestParameters.Add("productId", "1");
			_requestParameters.Add("action", "update");

			_basketRequestHandler.AddProduct(_requestParameters,  null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), It.IsAny<int>(), It.IsAny<int>(), "update", It.IsAny<int>(), It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
		}

		[Test]
		public void RequestWithProductId_ShouldCallUpdateLineOnceWithThatProductId()
		{
			_requestParameters.Add("productId", "7");

			_basketRequestHandler.AddProduct(_requestParameters, null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), It.IsAny<int>(), 7, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
		}

		[Test]
		public void RequestWithOrderLineId_ShouldCallUpdateLineOnceWithThatOrderLineId()
		{
			_requestParameters.Add("orderLineId", "2");

			_basketRequestHandler.AddProduct(_requestParameters, null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), 2, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
		}

		[Test]
		public void RequestWithQuantity_ShouldCallUpdateLineOnceWithThatQuantity()
		{
			_requestParameters.Add("productId", "1");
			_requestParameters.Add("quantity", "9");

			_basketRequestHandler.AddProduct(_requestParameters, null);

			_mock.Verify(a => a.AddOrUpdateOrderLine(It.IsAny<OrderInfo>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), 9, It.IsAny<IEnumerable<int>>(), It.IsAny<Dictionary<string, string>>()), Times.Exactly(1));
		}
	}
}