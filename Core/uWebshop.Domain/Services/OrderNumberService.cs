using System;
using System.Threading;
using uWebshop.DataAccess;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	class OrderNumberService : IOrderNumberService
	{
		private readonly ICMSApplication _cmsApplication;
		private readonly IOrderRepository _orderRepository;

		public OrderNumberService(ICMSApplication cmsApplication, IOrderRepository orderRepository)
		{
			_cmsApplication = cmsApplication;
			_orderRepository = orderRepository;
		}

		public void GenerateAndPersistOrderNumber(OrderInfo order)
		{
			using (var t = GetTransaction(order))
			{
				t.Generate();
				t.Persist();
			}
		}

		public IOrderNumberTransaction GetTransaction(OrderInfo order)
		{
			// NB: this isn't transactional in a load balanced environment
			Monitor.Enter(this);
			var id = 0;
			var nonTransactionalDatabase = _cmsApplication.UsesSQLCEDatabase() || _cmsApplication.UsesMySQLDatabase();
			return new OrderNumberTransaction(order, o =>
				{
					if (order.StoreOrderReferenceId.HasValue && !order.OrderNumber.StartsWith("[INCOMPLETE]-") && !order.OrderNumber.StartsWith("[SCHEDULED]-")) return; // todo: is this necessary?
					if (nonTransactionalDatabase)
					{
						order.OrderNumber = GenerateOrderNumber(order.StoreInfo.Store, order, out id);
					}
					else
					{
						AssignNewOrderNumberToOrder(order, order.StoreInfo.Store);
					}
				}, // todo: highly likely this is not necessary with transactionalDB (functionality kept the same during refactoring)
				o => _orderRepository.SetOrderNumber(order.UniqueOrderId, order.OrderNumber, order.StoreInfo.Alias, id),
				() => Monitor.Exit(this));
		}

		internal string GenerateOrderNumber(Store store, OrderInfo orderInfo, out int lastOrderReferenceNumber)
		{
			lastOrderReferenceNumber = 0;

			var currentHighestOrderNumber = UwebshopConfiguration.Current.ShareBasketBetweenStores ? _orderRepository.GetHighestOrderNumber(ref lastOrderReferenceNumber) : _orderRepository.GetHighestOrderNumberForStore(store.Alias, ref lastOrderReferenceNumber);

			Log.Instance.LogDebug("GenerateOrderNumber currentHighestOrderNumber: " + currentHighestOrderNumber + " lastOrderReferenceNumber: " + lastOrderReferenceNumber);

			var orderNumberPrefix = store.OrderNumberPrefix;
			if (lastOrderReferenceNumber <= 0)
			{
				if (!string.IsNullOrEmpty(currentHighestOrderNumber) && !string.IsNullOrEmpty(orderNumberPrefix) && currentHighestOrderNumber.Length >= orderNumberPrefix.Length)
					int.TryParse(currentHighestOrderNumber.Substring(orderNumberPrefix.Length, currentHighestOrderNumber.Length - orderNumberPrefix.Length), out lastOrderReferenceNumber);
				else
					int.TryParse(currentHighestOrderNumber, out lastOrderReferenceNumber);
			}
			lastOrderReferenceNumber++;
			lastOrderReferenceNumber = Math.Max(lastOrderReferenceNumber, store.OrderNumberStartNumber);

			orderInfo.StoreOrderReferenceId = lastOrderReferenceNumber;

			Log.Instance.LogDebug("GenerateOrderNumber lastOrderReferenceNumber: " + lastOrderReferenceNumber);

			return GenerateOrderNumber(store, orderInfo, lastOrderReferenceNumber, orderNumberPrefix);
		}

		private static string GenerateOrderNumber(Store store, OrderInfo orderInfo, int lastOrderReferenceNumber, string orderNumberPrefix)
		{
			if (string.IsNullOrEmpty(store.OrderNumberTemplate))
			{
				return string.Format("{0}{1}", orderNumberPrefix, lastOrderReferenceNumber.ToString("0000"));
			}

			var template = store.OrderNumberTemplate;
			return template.Replace("#orderId#", lastOrderReferenceNumber.ToString()).Replace("#orderIdPadded#", lastOrderReferenceNumber.ToString("0000")).Replace("#storeAlias#", store.Alias).Replace("#day#", orderInfo.ConfirmDate.GetValueOrDefault().Day.ToString()).Replace("#month#", orderInfo.ConfirmDate.GetValueOrDefault().Month.ToString()).Replace("#year#", orderInfo.ConfirmDate.GetValueOrDefault().Year.ToString());
		}

		internal void AssignNewOrderNumberToOrder(OrderInfo orderInfo, Store store)
		{
			var newNumber = UwebshopConfiguration.Current.ShareBasketBetweenStores ? _orderRepository.AssignNewOrderNumberToOrderSharedBasket(orderInfo.DatabaseId, store.Alias, store.OrderNumberStartNumber) : _orderRepository.AssignNewOrderNumberToOrder(orderInfo.DatabaseId, store.Alias, store.OrderNumberStartNumber);
			orderInfo.StoreOrderReferenceId = newNumber;
			orderInfo.OrderNumber = GenerateOrderNumber(store, orderInfo, newNumber, store.OrderNumberPrefix);
		}

		class OrderNumberTransaction : IOrderNumberTransaction
		{
			private readonly OrderInfo _order;
			private readonly Action<OrderInfo> _generate;
			private readonly Action<OrderInfo> _persist;
			private readonly Action _releaseLock;
			private readonly string _previousOrderNumber;
			private bool _persisted;

			public OrderNumberTransaction(OrderInfo order, Action<OrderInfo> generate, Action<OrderInfo> persist, Action releaseLock)
			{
				_order = order;
				_previousOrderNumber = order.OrderNumber;
				_generate = generate;
				_persist = persist;
				_releaseLock = releaseLock;
			}

			public void Dispose()
			{
				try
				{
					if (!_persisted) Rollback();
				}
				finally
				{
					_releaseLock();
				}
			}

			public void Generate()
			{
				_generate(_order);
			}

			public void Persist()
			{
				_persist(_order);
				_persisted = true;
			}

			public void Rollback()
			{
				_order.OrderNumber = _previousOrderNumber;
				_order.StoreOrderReferenceId = null;
				_order.Save(true);
			}
		}
	}
}