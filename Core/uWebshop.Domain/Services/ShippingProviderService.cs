using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class ShippingProviderService : IShippingProviderService
	{
		private readonly IShippingProviderRepository _shippingProviderRepository;

		public ShippingProviderService(IShippingProviderRepository shippingProviderRepository)
		{
			_shippingProviderRepository = shippingProviderRepository;
		}

		public IEnumerable<ShippingProvider> GetAll(ILocalization localization)
		{
			return _shippingProviderRepository.GetAll(localization);
		}

		public ShippingProvider GetPaymentProviderWithName(string paymentProviderName, ILocalization localization)
		{
			return _shippingProviderRepository.GetAll(localization).FirstOrDefault(x => x.Name.ToLower() == paymentProviderName.ToLower());
		}

		public ShippingProvider GetById(int id, ILocalization localization)
		{
			return _shippingProviderRepository.GetById(id, localization);
		}

		//public void LoadData(ShippingProvider paymentProvider, ILocalization localization)
		//{
		//	_shippingProviderRepository.ReloadData(paymentProvider, localization);
		//}
	}
}