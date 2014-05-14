using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.API
{
	[KnownType(typeof(BasketValidationResult))]
	[DataContract]
	internal class BasketValidationResults : IValidationResults
	{
		private readonly OrderInfo _order;
		private readonly IOrderService _orderService;

		public BasketValidationResults(OrderInfo order, IOrderService orderService)
		{
			_order = order;
			_orderService = orderService;


		}

		[DataMember]
		public IEnumerable<IValidationResult> All
		{
			get
			{
				return Order.Concat(Stock).Concat(OrderLine).Concat(Custom).Concat(Customer).Concat(Payment).Concat(Shipping);
			}
			set { }
		}

		[DataMember]
		public IEnumerable<IValidationResult> Order
		{
			get
			{
				return _order != null && _order.ConfirmValidationFailed
					       ? _orderService.ValidateGlobalValidations(_order).Select(i => new BasketValidationResult(i))
					       : Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> Stock
		{
			get
			{
				if (_order != null)
				{
					return _orderService.ValidateStock(_order).Select(i => new BasketValidationResult(i));
				}
				return Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> OrderLine
		{ 
			get
			{
				if (_order != null)
				{
					return _orderService.ValidateOrderlines(_order).Select(i => new BasketValidationResult(i));
				}
				return Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> Custom
		{
			get
			{
				if (_order != null)
				{
					return _orderService.ValidateCustomValidations(_order).Select(i => new BasketValidationResult(i));
				}

				return Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> Customer
		{
			get
			{
				if (_order != null && (_order.CustomerValidationFailed || _order.ConfirmValidationFailed))
				{
					return _orderService.ValidateCustomer(_order).Select(i => new BasketValidationResult(i));
				}

				return Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> Payment
		{
			get
			{
				if (_order != null)
				{
					return _orderService.ValidatePayment(_order).Select(i => new BasketValidationResult(i)); 
				}

				return Enumerable.Empty<IValidationResult>();
			}
			set { }
		}
		[DataMember]
		public IEnumerable<IValidationResult> Shipping
		{
			get
			{
				if (_order != null)
				{
					return _orderService.ValidateShipping(_order).Select(i => new BasketValidationResult(i));
				}

				return Enumerable.Empty<IValidationResult>(); 
			}
			set{}
		}
	}
}