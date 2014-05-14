using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class DiscountService : IDiscountService
	{
		private readonly IOrderDiscountService _orderDiscountService;
		private readonly IProductDiscountService _productDiscountService;

		public DiscountService(IOrderDiscountService orderDiscountService, IProductDiscountService productDiscountService)
		{
			_orderDiscountService = orderDiscountService;
			_productDiscountService = productDiscountService;
		}

		public IOrderDiscount GetOrderDiscountById(int id, ILocalization localization)
		{
			return _orderDiscountService.GetById(id, localization);
		}

		public DiscountProduct GetProductDiscountById(int id, ILocalization localization)
		{
			return _productDiscountService.GetById(id, localization);
		}

		public IDiscount GetById(int id, ILocalization localization)
		{
			return (IDiscount) GetOrderDiscountById(id, localization) ?? GetProductDiscountById(id, localization);
		}
	}
}