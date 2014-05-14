using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.ActionHandlers;
using uWebshop.Common;
using uWebshop.Domain;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;
using uWebshop.Test.Repositories;
using uWebshop.Umbraco.Businesslogic;

namespace uWebshop.Test.Integration.Domain.OrderTests
{
	[TestFixture]
	public class StatusAndStock
	{
		private OrderInfo _order;
		private Mock<IStockService> _stockServiceMock;

		[SetUp]
		public void Setup()
		{
			IOC.UnitTest();
			_stockServiceMock = IOC.StockService.SetupNewMock();

			var productInfo = DefaultFactoriesAndSharedFunctionality.CreateProductInfo(12990, 1);
			productInfo.Id = TestProductService.ProductId1;
			_order = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(productInfo);
			_order.StoreInfo.Alias = StoreHelper.GetCurrentStore().Alias;
		}

		[Test]
		public void OfflinePayment()
		{
			_order.Status = OrderStatus.OfflinePayment;
			OrderEvents.OrderStatusChanged(_order, new AfterOrderStatusChangedEventArgs {OrderInfo = _order, OrderStatus = OrderStatus.OfflinePayment, SendEmails = false});

			_stockServiceMock.Verify(m => m.SetStock(TestProductService.ProductId1, 1, It.IsAny<bool>(), It.IsAny<string>()));
			Assert.IsTrue(_order.StockUpdated);
		}

		[Test]
		public void OnlinePaymentAndCancelling()
		{
			_order.PaymentInfo.PaymentType = PaymentProviderType.OnlinePayment;
			_order.Status = OrderStatus.ReadyForDispatch;
			OrderEvents.OrderStatusChanged(_order, new AfterOrderStatusChangedEventArgs {OrderInfo = _order, OrderStatus = OrderStatus.ReadyForDispatch, SendEmails = false});

			_stockServiceMock.Verify(m => m.SetStock(TestProductService.ProductId1, 1, It.IsAny<bool>(), It.IsAny<string>()));
			Assert.IsTrue(_order.StockUpdated);

			_order.Status = OrderStatus.Cancelled;
			OrderEvents.OrderStatusChanged(_order, new AfterOrderStatusChangedEventArgs {OrderInfo = _order, OrderStatus = OrderStatus.Cancelled, SendEmails = false});

			_stockServiceMock.Verify(m => m.ReturnStock(TestProductService.ProductId1, 1, It.IsAny<bool>(), It.IsAny<string>()));
			Assert.IsFalse(_order.StockUpdated);
		}
	}
}