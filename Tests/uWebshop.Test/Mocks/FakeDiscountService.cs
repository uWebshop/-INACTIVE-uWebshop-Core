using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Mocks
{
	internal class FakeDiscountService : IDiscountService
	{
		private readonly IOrderDiscountRepository _orderDiscountRepository;

		public FakeDiscountService(IOrderDiscountRepository orderDiscountRepository)
		{
			_orderDiscountRepository = orderDiscountRepository;
		}

		public List<IOrderDiscount> GetApplicableDiscountsForOrder(OrderInfo orderInfo)
		{
			return _orderDiscountRepository.GetAll(orderInfo.Localization).Cast<IOrderDiscount>().ToList();
		}

		public IOrderDiscount GetOrderDiscountById(int id, ILocalization localization)
		{
			throw new NotImplementedException();
		}

		public DiscountProduct GetProductDiscountById(int id, ILocalization localization)
		{
			throw new NotImplementedException();
		}

		public IDiscount GetById(int id, ILocalization localization)
		{
			throw new NotImplementedException();
		}
	}
}